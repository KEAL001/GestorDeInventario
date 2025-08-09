using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GestorDeInventario.ViewModels
{
    public class ProductoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _id;
        public int ID
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _nombre;
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _sku;
        public string SKU
        {
            get => _sku;
            set
            {
                if (_sku != value)
                {
                    _sku = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _precioVenta;
        public decimal PrecioVenta
        {
            get => _precioVenta;
            set
            {
                if (_precioVenta != value)
                {
                    _precioVenta = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _costoCompra;
        public decimal CostoCompra
        {
            get => _costoCompra;
            set
            {
                if (_costoCompra != value)
                {
                    _costoCompra = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _stockMinimo;
        public int StockMinimo
        {
            get => _stockMinimo;
            set
            {
                if (_stockMinimo != value)
                {
                    _stockMinimo = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _categoria;
        public string Categoria
        {
            get => _categoria;
            set
            {
                if (_categoria != value)
                {
                    _categoria = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _proveedor;
        public string Proveedor
        {
            get => _proveedor;
            set
            {
                if (_proveedor != value)
                {
                    _proveedor = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _stockActual;
        public int StockActual
        {
            get => _stockActual;
            set
            {
                if (_stockActual != value)
                {
                    _stockActual = value;
                    OnPropertyChanged();
                }
            }
        }
        public string AlertaStock
        {
            get => (_stockActual <= StockMinimo) ? "⚠️" : string.Empty;
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}