using GestorDeInventario.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;
using OfficeOpenXml; // Asegúrate de que esta directiva esté aquí
using System.Linq;

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
    }
}
