using System;
using System.Windows;
using M560V_21_ControlApp.Services.WebServer;  // add this namespace

namespace M560V_21_ControlApp
{
    public partial class App : Application
    {
        private EmbeddedWebServer _webServer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Start embedded web server on port 8080
                _webServer = new EmbeddedWebServer(port: 8080);
                _webServer.Start();

                // Optionally show the URL in a log or message box (for test)
                string url = $"http://{_webServer.LocalIP}:{_webServer.Port}/";
                MessageBox.Show($"Web interface running at {url}", "Web Server Started",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start web server: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                _webServer?.Stop();
            }
            catch
            {
                // ignore
            }

            base.OnExit(e);
        }
    }
}
