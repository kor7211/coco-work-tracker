using Microsoft.Extensions.DependencyInjection;
using Recorder.Core.Models;
using Recorder.Views;
using Supabase;
using Supabase.Realtime.Exceptions;

namespace Recorder
{
    public partial class App : Application
    {
        private readonly IServiceProvider _services;
        private Window? _window;
        private bool _isInitialized;

        public App(IServiceProvider services)
        {
            InitializeComponent();

            _services = services;
        }

        protected override Window CreateWindow(
            IActivationState? activationState)
        {
            _window = new Window(new StartupPage());

            _window.Page.Appearing += OnStartupPageAppearing;

            return _window;
        }

        private async void OnStartupPageAppearing(
            object? sender,
            EventArgs e)
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            try
            {
                Client client = _services.GetRequiredService<Client>();

                await SupabaseClientGenerator.InitializeSupabaseAsync(client);

                AppState appState =
                    _services.GetRequiredService<AppState>();

                appState.Initialize(client);

                if (_window is not null)
                {
                    _window.Page =
                        _services.GetRequiredService<AppShell>();
                }
            }
            catch (RealtimeException ex)
            {
                if (_window?.Page is Page page)
                {
                    await page.DisplayAlertAsync(
                        "Realtime Connection Error:\n" +
                        "Supabase server is currently unreachable. Check server condition.",
                        ex.Message,
                        "OK"
                    );
                }
            }
            catch (Exception ex)
            {
                if (_window?.Page is Page page)
                {
                    await page.DisplayAlertAsync(
                        "Initialization Error:\n" +
                        $"Detail: {ex.Message}",
                        ex.Message,
                        "OK"
                    );
                }
            }
        }
    }
}