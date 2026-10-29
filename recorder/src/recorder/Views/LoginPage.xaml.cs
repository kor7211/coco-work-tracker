using Recorder.Core.Models;
using Supabase;
using System.ComponentModel.DataAnnotations;

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
		string email = EmailEntry.Text;

		string password = PasswordEntry.Text;

		//Validation
		if (!ValidateEmail(email)) return;
		if (!ValidatePassword(password)) return;

		try
		{
			bool success = await _appState.Login(email, password);
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