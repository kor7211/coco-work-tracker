using Recorder.Controllers;
using Recorder.Core.Controllers;
using Recorder.Core.Models;
using Recorder.Core.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Recorder.Views;

public partial class CreateRecordPage : ContentPage
{
	private readonly AppState _appState;

    private readonly AppShell _shell;

    private readonly HistoryManager _historyManager;

    private bool IsLoading = false;

    public ObservableCollection<ImageFileInfo?> SelectedImageCollection { get; } = 
        new();

	public CreateRecordPage(AppState appState, AppShell shell)
	{
		InitializeComponent();

		_appState = appState;

        _shell = shell;

        _historyManager = new HistoryManager(_appState);

        UpdateTypePicker();
        SetDatetimeNow();
        UpdateTimezonePicker();
        ImageContainer.ItemsSource = SelectedImageCollection;
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

    private async void OnAddImageClicked(object sender, EventArgs e)
    {
        if (IsLoading) return;

        PickOptions options = new PickOptions()
        {
            PickerTitle = "Select images",
            FileTypes = FilePickerFileType.Images
        };

        IEnumerable<FileResult?>? result = await FilePicker.Default.PickMultipleAsync(options);

        if (result ==  null) return;
        if (!result.Any()) return;
        
        foreach (FileResult? file in  result)
        {
            if (file == null)
                continue;

            SelectedImageCollection.Add(
                new ImageFileInfo(
                    file.FileName,
                    file.FullPath,
                    file.ContentType
                ));
        }
    }

    private void OnRemoveImageClicked(object? sender, EventArgs e)
    {
        if (IsLoading) return;

        Button? button = sender as Button;
        if (button == null) return;

        ImageFileInfo? file = button.BindingContext as ImageFileInfo;
        if (file == null) return;

        SelectedImageCollection.Remove(file);
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        if (IsLoading) return;

        IsLoading = true;

        ResetDisplay();
        PreventInput();

        try
        {
            await TryCreateRecord();

            DisplaySuccess("Success: Record created.");
        } catch (ArgumentException ex)
        {
            //Catch validation error
            DisplayError(ex.Message);
        } catch (Exception ex)
        {
            //Carch other errors. Expected as api thrown error.
            DisplayError(ex.Message);
        }
        finally
        {
            IsLoading = false;
            EnableInput();
        }        
    }

    private async Task TryCreateRecord()
    {
        // Pass user inout to controller
        bool isImageAttempted = SelectedImageCollection.Count() > 0;

        await _historyManager.CreateHistoryAsync(
            TitleEnEntry.Text,
            TitleJaEntry.Text,
            CommentEnEntry.Text,
            CommentJaEntry.Text,
            TypePicker.SelectedItem as WorkType,
            DurationEntry.Text,
            BreakEntry.Text,
            StartedDatePicker.Date,
            StartedTimePicker.Time,
            TimezonePicker.SelectedItem as TimeZoneInfo,
            isImageAttempted?
                SelectedImageCollection.ToList():
                null
            );
    }

    private void ResetDisplay()
    {
        ErrorLabel.Text = "";
        SuccessLabel.Text = "";
    }

    private void DisplayError(string message)
    {
        ResetDisplay();
        ErrorLabel.Text = message;
    }

    private void DisplaySuccess(string message)
    {
        ResetDisplay();
        SuccessLabel.Text = message;
    }

    private void EnableInput()
    {
        CreateButton.IsEnabled = true;
        IsLoading = false;
        _shell.IsPopEnabled = true;

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
    }

    private void PreventInput()
    {
        CreateButton.IsEnabled = false;
        _shell.IsPopEnabled = false;
        IsLoading = true;

        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
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