// En DataRefreshService.cs
using System;

namespace GestorDeInventario.Utils
{
    public static class DataRefreshService
    {
        // Este evento se disparará cuando necesitemos refrescar los datos
        public static event EventHandler DataRefreshed;

        // Este método permite a cualquier parte de la aplicación disparar el evento
        public static void NotifyDataRefreshed()
        {
            DataRefreshed?.Invoke(null, EventArgs.Empty);
        }
    }
}