using Recorder.Core.Models;
using Supabase;

namespace Recorder.Views;

public partial class LoginPage : ContentPage
{
	private AppState _appState;
	public LoginPage(AppState appState)
	{
		InitializeComponent();

		_appState = appState;
	}

	private async void OnLoginButtonClicked(object sender, EventArgs e)
	{
		Supabase.Client client = await SupabaseClientGenerator.GetClientAsync();
	}
}