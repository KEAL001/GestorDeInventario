using GestorDeInventario.Data;
using GestorDeInventario.Models;
using GestorDeInventario.Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GestorDeInventario
{
    public partial class ReportesWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public ReportesWindow()
        {
            InitializeComponent();
            _context = DbContextManager.Instance;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cmbReportes.SelectedIndex = 0;
        }

        private void cmbReportes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)cmbReportes.SelectedItem;
            if (item == null) return;

            string reporte = item.Content.ToString();
            pnlFiltroFechas.Visibility = (reporte == "Movimientos de Inventario (Rango de Fechas)") ? Visibility.Visible : Visibility.Collapsed;

            switch (reporte)
            {
                case "Inventario por Categoría":
                    GenerarReportePorCategoria();
                    break;
                case "Productos bajo Stock Mínimo":
                    GenerarReporteStockMinimo();
                    break;
                case "Productos":
                    GenerarReporteProductosDetallado();
                    break;
                case "Movimientos de Inventario (Rango de Fechas)":
                    // Limpia la DataGrid hasta que el usuario haga clic en "Generar"
                    dgReportes.ItemsSource = null;
                    break;
            }
        }

        private void GenerarReportePorCategoria()
        {
            var reporteData = _context.Productos
                .Include(p => p.Categoria)
                .GroupBy(p => p.Categoria.NombreCategoria)
                .Select(g => new
                {
                    Categoria = g.Key,
                    TotalProductos = g.Count()
                }).ToList();
            dgReportes.ItemsSource = reporteData;
        }

        private void GenerarReporteStockMinimo()
        {
            var inventarioService = new InventarioService(_context);

            // Primero, obtenemos todos los productos del contexto de la base de datos.
            var todosLosProductos = _context.Productos
                .Include(p => p.Categoria)
                .ToList();

            // Luego, en memoria (client-side), calculamos el stock actual para cada producto
            // y filtramos los que están por debajo del stock mínimo.
            var reporteData = todosLosProductos
                .Select(p => new
                {
                    p.Nombre,
                    p.SKU,
                    StockActual = inventarioService.ObtenerStockActual(p.ID),
                    p.StockMinimo,
                    Categoria = p.Categoria.NombreCategoria
                })
                .Where(x => x.StockActual <= x.StockMinimo)
                .ToList();

            dgReportes.ItemsSource = reporteData;
        }

        private void GenerarReporteProductosDetallado()
        {
            var inventarioService = new InventarioService(_context);
            var reporteData = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Select(p => new
                {
                    p.SKU,
                    p.Nombre,
                    p.Descripcion,
                    Categoria = p.Categoria.NombreCategoria,
                    Proveedor = p.Proveedor.NombreProveedor,
                    // Obtenemos el último costo unitario de las transacciones de entrada
                    CostoUnitario = _context.TransaccionesInventario
                        .Where(t => t.ProductoID == p.ID && t.TipoTransaccion == "Entrada")
                        .OrderByDescending(t => t.FechaTransaccion)
                        .Select(t => (decimal?)t.CostoUnitario)
                        .FirstOrDefault() ?? 0,
                    StockActual = inventarioService.ObtenerStockActual(p.ID),
                    p.StockMinimo
                })
                .ToList();

            dgReportes.ItemsSource = reporteData;
        }

        private void btnGenerar_Click(object sender, RoutedEventArgs e)
        {
            if (dpFechaInicio.SelectedDate.HasValue && dpFechaFin.SelectedDate.HasValue)
            {
                GenerarReporteMovimientos(dpFechaInicio.SelectedDate.Value, dpFechaFin.SelectedDate.Value);
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un rango de fechas válido.", "Error");
            }
        }

        private void GenerarReporteMovimientos(DateTime fechaInicio, DateTime fechaFin)
        {
            // Actualizar el título con el rango de fechas  
            var tituloSeccion = this.FindName("txtTituloSeccion") as TextBlock;
            if (tituloSeccion != null)
            {
                tituloSeccion.Text = $"MOVIMIENTOS DE INVENTARIO - {fechaInicio:dd/MM/yyyy} al {fechaFin:dd/MM/yyyy}";
            }

            // Resto del código existente...  
            var reporteData = _context.TransaccionesInventario
                .Where(t => t.FechaTransaccion >= fechaInicio && t.FechaTransaccion <= fechaFin)
                .Include(t => t.Producto)
                .Include(t => t.Usuario)
                .Select(t => new
                {
                    Producto = t.Producto.Nombre,
                    t.TipoTransaccion,
                    t.Cantidad,
                    Usuario = t.Usuario.NombreUsuario,
                    t.FechaTransaccion,
                    t.Comentarios
                }).ToList();
            dgReportes.ItemsSource = reporteData;
        }

        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {
            if (dgReportes.ItemsSource != null)
            {
                try
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        var worksheet = excelPackage.Workbook.Worksheets.Add("Reporte");
                        var items = dgReportes.ItemsSource as System.Collections.IEnumerable;

                        // Obtener el tipo de reporte seleccionado  
                        var selectedReport = ((ComboBoxItem)cmbReportes.SelectedItem).Content.ToString();

                        int currentRow = 1;

                        // Agregar título del reporte  
                        worksheet.Cells[currentRow, 1].Value = selectedReport.ToUpper();
                        worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
                        currentRow += 2;

                        // Si es reporte de movimientos y hay fechas seleccionadas, agregar el rango  
                        if (selectedReport == "Movimientos de Inventario (Rango de Fechas)" &&
                            dpFechaInicio.SelectedDate.HasValue && dpFechaFin.SelectedDate.HasValue)
                        {
                            string rangoFechas = $"Período: {dpFechaInicio.SelectedDate.Value:dd/MM/yyyy} al {dpFechaFin.SelectedDate.Value:dd/MM/yyyy}";
                            worksheet.Cells[currentRow, 1].Value = rangoFechas;
                            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                            worksheet.Cells[currentRow, 1].Style.Font.Size = 12;
                            currentRow += 2;
                        }

                        // Agregar fecha de generación  
                        worksheet.Cells[currentRow, 1].Value = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}";
                        worksheet.Cells[currentRow, 1].Style.Font.Italic = true;
                        currentRow += 2;

                        if (items != null)
                        {
                            var dataList = items.Cast<object>().ToList();

                            if (dataList.Any())
                            {
                                var properties = dataList.First().GetType().GetProperties();

                                // Agregar encabezados de columnas  
                                for (int i = 0; i < properties.Length; i++)
                                {
                                    worksheet.Cells[currentRow, i + 1].Value = properties[i].Name;
                                    worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
                                }
                                currentRow++;

                                // Agregar datos  
                                for (int i = 0; i < dataList.Count; i++)
                                {
                                    for (int j = 0; j < properties.Length; j++)
                                    {
                                        worksheet.Cells[currentRow + i, j + 1].Value = properties[j].GetValue(dataList[i]);
                                    }
                                }

                                // Aplicar formato y autoajustar columnas  
                                worksheet.Cells[1, 1, currentRow + dataList.Count - 1, properties.Length].AutoFitColumns();
                            }
                        }

                        // Genera un nombre de archivo dinámico  
                        string fileName = $"Reporte_{selectedReport.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.xlsx";

                        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                        {
                            Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
                            FileName = fileName
                        };

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            excelPackage.SaveAs(new FileInfo(saveFileDialog.FileName));
                            MessageBox.Show("Reporte exportado con éxito.", "Éxito");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error al exportar: {ex.Message}", "Error");
                }
            }
            else
            {
                MessageBox.Show("No hay datos para exportar.", "Error");
            }
        }
    }
}