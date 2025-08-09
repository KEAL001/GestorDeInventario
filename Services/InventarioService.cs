using GestorDeInventario.Data;
using GestorDeInventario.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;

namespace GestorDeInventario.Services
{
    public class InventarioService
    {
        private readonly ApplicationDbContext _context;

        public InventarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método mejorado para obtener el stock actual, con la corrección para el error 'DataReader is already open'
        public int ObtenerStockActual(int productoId)
        {
            // Usamos un nuevo contexto para garantizar que cada operación de DB sea independiente.
            using (var newContext = new ApplicationDbContext())
            {
                var entradas = newContext.TransaccionesInventario
                    .Where(t => t.ProductoID == productoId && t.TipoTransaccion == "Entrada")
                    .Sum(t => (int?)t.Cantidad) ?? 0;

                var salidas = newContext.TransaccionesInventario
                    .Where(t => t.ProductoID == productoId && t.TipoTransaccion == "Salida")
                    .Sum(t => (int?)t.Cantidad) ?? 0;

                return entradas - salidas;
            }
        }

        // Nuevo método para validar si es posible realizar una salida de stock
        public bool PuedeRealizarSalida(int productoId, int cantidadSalida)
        {
            var stockActual = ObtenerStockActual(productoId);
            return stockActual >= cantidadSalida;
        }

        // Método genérico para registrar cualquier tipo de transacción
        public void RegistrarTransaccion(int productoId, string tipoMovimiento, int cantidad, decimal costoUnitario, string comentarios, int usuarioId, DateTime fechaTransaccion)
        {
            var transaccion = new TransaccionInventario
            {
                ProductoID = productoId,
                TipoTransaccion = tipoMovimiento,
                Cantidad = cantidad,
                CostoUnitario = costoUnitario,
                Comentarios = comentarios,
                FechaTransaccion = fechaTransaccion,
                UsuarioID = usuarioId
            };
            _context.TransaccionesInventario.Add(transaccion);
            _context.SaveChanges();
        }

        // Método RegistrarEntrada con la firma original
        public void RegistrarEntrada(int productoId, int cantidad, decimal costoUnitario, int usuarioId, string comentarios)
        {
            RegistrarTransaccion(productoId, "Entrada", cantidad, costoUnitario, comentarios, usuarioId, DateTime.Now);
        }

        // Método RegistrarSalida con la firma original
        public void RegistrarSalida(int productoId, int cantidad, int usuarioId, string comentarios)
        {
            // Nota: Se asume que el costo unitario para la salida se gestiona en otro lugar, 
            // o se utiliza un valor por defecto. La firma original no incluye costo unitario.
            // Aquí, se le pasa 0, pero se puede ajustar según tu lógica de costo promedio.
            RegistrarTransaccion(productoId, "Salida", cantidad, 0, comentarios, usuarioId, DateTime.Now);
        }
    }
}