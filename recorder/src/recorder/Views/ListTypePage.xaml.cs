namespace Recorder.Views;

public partial class ListTypePage : ContentPage
{
	public ListTypePage()
	{
		InitializeComponent();
	}

    private async void OnHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///home");
    }

    private async void OnListRecordsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///records");
    }
}