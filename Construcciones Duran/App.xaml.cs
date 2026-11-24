using System.Globalization;

namespace Proyecto_Final
{
    public partial class App : Application
    {
        public App()
        {
            ConfigurarCultura();
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        /// <summary>
        /// Usa el idioma español de Bolivia, pero con punto (.) como separador decimal,
        /// para que los campos numéricos (precios, salarios) se lean igual en todos los equipos.
        /// </summary>
        private static void ConfigurarCultura()
        {
            var cultura = (CultureInfo)CultureInfo.GetCultureInfo("es-BO").Clone();
            cultura.NumberFormat.NumberDecimalSeparator = ".";
            cultura.NumberFormat.NumberGroupSeparator = ",";
            cultura.NumberFormat.CurrencyDecimalSeparator = ".";
            cultura.NumberFormat.CurrencyGroupSeparator = ",";

            CultureInfo.DefaultThreadCurrentCulture = cultura;
            CultureInfo.DefaultThreadCurrentUICulture = cultura;
            CultureInfo.CurrentCulture = cultura;
            CultureInfo.CurrentUICulture = cultura;
        }
    }
}