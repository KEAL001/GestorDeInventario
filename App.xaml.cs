using GestorDeInventario.Data;
using GestorDeInventario.Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml; // Asegúrate de que esta directiva esté aquí
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace GestorDeInventario
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            base.OnStartup(e);

            using (var context = new ApplicationDbContext())
            {
                context.Database.Migrate(); // Opcional: aplica migraciones si usas Code-First
                DbInitializer.SeedData(context); // Llama al seeder
            }

            // Muestra la ventana de login al inicio
           // var loginWindow = new LoginWindow();
           // loginWindow.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            DbContextManager.DisposeContext();
            base.OnExit(e);
        }
    }
}
