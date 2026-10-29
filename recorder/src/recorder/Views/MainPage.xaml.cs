using Recorder.Core.Models;
using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace Recorder.Views
{
    public partial class MainPage : ContentPage
    {
        private bool _hasCheckedAuthentication = false;
        private AppState _appState;
        private AppShell _shell;
        public MainPage(AppState appState, AppShell shell)
        {
            InitializeComponent();

            _appState = appState;

            _shell = shell;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_hasCheckedAuthentication)
                return;

            _hasCheckedAuthentication = true;

            if (!_appState.IsAuthenticated)
            {
                await Navigation.PushAsync(
                    new LoginPage(_appState, _shell));
            }
        }

        private async void OnListRecordsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///records");
        }

        private async void OnListTypesClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///types");
        }
    }
}
