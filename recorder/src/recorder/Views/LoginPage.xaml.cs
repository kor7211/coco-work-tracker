using Recorder.Core.Models;
using Supabase;
using System.ComponentModel.DataAnnotations;

namespace Recorder.Views;

public partial class LoginPage : ContentPage
{
	private AppState _appState;

	private AppShell _shell;
	public LoginPage(AppState appState, AppShell shell)
	{
		InitializeComponent();

		_appState = appState;

		_shell = shell;
		shell.IsPopEnabled = false;
	}

	private async void OnLoginButtonClicked(object sender, EventArgs e)
	{
		string email = EmailEntry.Text;

		string password = PasswordEntry.Text;

		//Validation
		if (!ValidateEmail(email)) return;
		if (!ValidatePassword(password)) return;

		try
		{
			await _appState.Login(email, password);
			
			// if success

			Client? client = _appState.GetClient();
			if (client == null) 
				throw new NullReferenceException(nameof(client));
            SupabaseService service = new SupabaseService(client);

			_appState.Histories.AddRange(await service.GetHistoriesAsync());
            _appState.WorkTypes.AddRange(await service.GetWorkTypesAsync());

            _shell.IsPopEnabled = true;

            await Navigation.PopAsync(true);

        } catch (Exception ex) {
			string message = "Error: Fail to login. Please try different password or retry later.\n" +
				$"Error Detail: {ex.Message}";
			DisplayError(message);
		}
	}

	private bool ValidateEmail(string email)
	{
        if (String.IsNullOrWhiteSpace(email))
        {
            string message = "Error: Email missing.";
            DisplayError(message);
			return false;
        }
		
		EmailAddressAttribute attribute = new EmailAddressAttribute();
		if (!attribute.IsValid(email))
		{
            string message = "Error: Invalid email.";
            DisplayError(message);
            return false;
        }

		return true;
    }

    private bool ValidatePassword(string password)
    {
        if (String.IsNullOrWhiteSpace(password))
        {
            string message = "Error: Password missing.";
            DisplayError(message);
            return false;
        }

        return true;
    }

    private void DisplayError(string message)
	{
		ErrorMessageLabel.Text = message;
	}
}