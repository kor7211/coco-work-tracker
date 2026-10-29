using Recorder.Core.Models;
using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace Recorder.Views
{
    public partial class MainPage : ContentPage
    {
        private bool _hasCheckedAuthentication = false;
        private AppState _appState;
        public MainPage(AppState appState)
        {
            InitializeComponent();

            _appState = appState;
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
                    new LoginPage(_appState));
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
