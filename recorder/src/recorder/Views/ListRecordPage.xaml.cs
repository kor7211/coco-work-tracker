namespace Recorder.Views;

public partial class ListRecordPage : ContentPage
{
	public ListRecordPage()
	{
		InitializeComponent();
	}

    private async void OnHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///home");
    }

    private async void OnListTypesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///types");
    }
}