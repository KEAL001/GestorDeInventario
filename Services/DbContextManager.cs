using GestorDeInventario.Data;

namespace GestorDeInventario.Services
{
    public class DbContextManager
    {
        private static ApplicationDbContext _instance;
        private static readonly object _lock = new object();

        public static ApplicationDbContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ApplicationDbContext();
                            _instance.Database.EnsureCreated();
                        }
                    }
                }
                return _instance;
            }
        }

        public static void DisposeContext()
        {
            _instance?.Dispose();
            _instance = null;
        }
    }
}