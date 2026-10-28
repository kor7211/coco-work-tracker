namespace Recorder.Views
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
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
