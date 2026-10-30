using Recorder.Core.Models;
using Recorder.Core.Services;
using System.Globalization;

namespace Recorder.Views;

public partial class CreateRecordPage : ContentPage
{
	private readonly AppState _appState;

    private readonly SupabaseService _service;
	public CreateRecordPage(AppState appState)
	{
		InitializeComponent();

		_appState = appState;

        _service = new SupabaseService(appState.Supabase);

        UpdateTypePicker();
        SetDatetimeNow();
        UpdateTimezonePicker();
	}

    private void UpdateTimezonePicker()
    {
        TimezonePicker.ItemsSource = TimeZoneInfo.GetSystemTimeZones();
        TimezonePicker.ItemDisplayBinding = new Binding(nameof(TimeZoneInfo.DisplayName));
        TimezonePicker.SelectedItem = TimeZoneInfo.Local;
    }

	private void UpdateTypePicker()
	{
		TypePicker.ItemsSource = _appState.WorkTypes;
        TypePicker.ItemDisplayBinding = new Binding(".")
        {
            Converter = new WorkTypeDisplayConverter()
        };
	}

    private void SetDatetimeNow()
    {
        DateTime now = DateTime.Now;

        StartedDatePicker.Date = now.Date;
        StartedTimePicker.Time = now.TimeOfDay;
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        CreateButton.IsEnabled = false;
        //Validation
        try
        {
            ValidateIsNotEmpty(TitleEnEntry.Text, "English Title");
            ValidateIsNotEmpty(TitleJaEntry.Text, "Japanese Title");
            ValidateIsNotNull(TypePicker.SelectedItem, "Type");
            ValidateIsPositiveNumber(DurationEntry.Text, "Duration");
            ValidateIsZeroOrPositiveNumber(BreakEntry.Text, "Break");
            ValidateIsNotNull(StartedDatePicker.Date, "Started Date");
            ValidateIsNotNull(StartedTimePicker.Time, "Started Time");
            ValidateTimezone(TimezonePicker.SelectedItem as TimeZoneInfo);
        } catch (ArgumentException ex)
        {
            DisplayError(ex.Message);
            return;
        } finally
        {
            CreateButton.IsEnabled = true;
        }
        //Create new History
        History history = new History()
        {
            Titles = new Titles()
            {
                En = TitleEnEntry.Text,
                Ja = TitleJaEntry.Text
            },
            Comments = new Comments()
            {
                En = CommentEnEntry.Text,
                Ja = CommentJaEntry.Text
            },
            TypeId = ((WorkType)TypePicker.SelectedItem).Id,
            DurationMinutes = int.Parse(DurationEntry.Text),
            BreakMinutes = int.Parse(BreakEntry.Text),
            Started = TimeZoneService.GetUtcTime(
                (DateTime)StartedDatePicker.Date,
                (TimeSpan)StartedTimePicker.Time,
                (TimeZoneInfo)TimezonePicker.SelectedItem),
            Uploaded = DateTimeOffset.UtcNow,
            Timezone = TimeZoneService.GetIanaTimeZoneId(TimezonePicker.SelectedItem as TimeZoneInfo),
        };

        //Try Create
        try
        {
            long id = await _service.CreateHistoryAsync(history);

            history.Id = id;

            _appState.Histories.Add(history);

            DisplayError("");
        } catch (Exception ex)
        {
            DisplayError($"Error: Fail to create record.\n" +
                $"Detail: {ex.Message}");
        } finally
        {
            CreateButton.IsEnabled = true;
        }
    }

    private void DisplayError(string message)
    {
        ErrorLabel.Text = message;
    }

    //Validations
    private void ValidateIsNotEmpty(string? value, string param)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"Error: {param} must not be empty.");
        }
    }
    private void ValidateIsNotNull(object? value, string param)
    {
        if (value == null)
        {
            throw new ArgumentException($"Error: {param} must be chosen.");
        }
    }
    private void ValidateIsZeroOrPositiveNumber(string? value, string param)
    {
        if (int.TryParse(value, out int number))
        {
            if (number < 0)
            {
                throw new ArgumentException($"Error: {param} must be zero or positive number.");
            }
        }
        else
        {
            throw new ArgumentException($"Error: {param} must be nutural number.");
        }
    }
    private void ValidateIsPositiveNumber(string? value, string param)
    {
        if(int.TryParse(value, out int number))
        {
            if (number <= 0)
            {
                throw new ArgumentException($"Error: {param} must be positive number.");
            }
        } else
        {
            throw new ArgumentException($"Error: {param} must be nutural number.");
        }
    }

    private void ValidateTimezone(TimeZoneInfo? timeZone)
    {
        ValidateIsNotNull(timeZone, "Timezone");
        TimeZoneService.GetIanaTimeZoneId(timeZone);
    }
}

public class WorkTypeDisplayConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not WorkType workType)
            return string.Empty;

        return $"{workType.Names.En} / {workType.Names.Ja}";
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}