using Recorder.Core.Models;
using Recorder.Core.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Recorder.Views;

public partial class CreateRecordPage : ContentPage
{
	private readonly AppState _appState;

    private readonly SupabaseService _service;

    private readonly ImageStorageService _imageService;

    private readonly AppShell _shell;

    private bool IsLoading = false;

    private static string[] ALLOWED_IMAGE_TYPE = ["image/jpg", "image/jpeg", "image/png"];

    private const long MAX_FILE_SIZE = 50 * 1024 * 1024; // 50 MB

    public ObservableCollection<ImageFileResult> Images = new ObservableCollection<ImageFileResult>();
	public CreateRecordPage(AppState appState, AppShell shell)
	{
		InitializeComponent();

		_appState = appState;

        _shell = shell;

        _service = new SupabaseService(appState.Supabase);

        _imageService = new ImageStorageService(appState.Supabase);

        UpdateTypePicker();
        SetDatetimeNow();
        UpdateTimezonePicker();
        ImageContainer.ItemsSource = Images;
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

        IEnumerable<FileResult?> result = await FilePicker.Default.PickMultipleAsync(options);

        foreach (FileResult? file in  result)
        {
            if (file == null)
                continue;
            
            Images.Add(
                new ImageFileResult()
                {
                    FileResult = file,
                    Title = file.FileName
                });
        }
    }

    private void OnRemoveImageClicked(object? sender, EventArgs e)
    {
        if (IsLoading) return;

        Button? button = sender as Button;
        if (button == null) return;

        ImageFileResult? file = button.BindingContext as ImageFileResult;
        if (file == null) return;

        Images.Remove(file);
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        if (IsLoading) return;

        ResetDisplay();
        PreventInput();
        await TryCreateRecord();
        EnableInput();
    }
    private async Task TryCreateRecord()
    {
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
            foreach (ImageFileResult file in Images)
            {
                await ValidateImage(file);
            }
        }
        catch (ArgumentException ex)
        {
            DisplayError(ex.Message);
            return;
        }
        //Create new History
        List<ImagePath> images = new List<ImagePath>();
        foreach (ImageFileResult file in Images)
        {
            ImagePath image = new ImagePath()
            {
                Path = file.PathInStorage
            };
            images.Add(image);
        }
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
            ImagePaths = images
        };

        //Try Create
        try
        {
            long id = await _service.CreateHistoryAsync(history);

            history.Id = id;

            _appState.Histories.Add(history);

            foreach (ImageFileResult imageFileResult in Images)
            {
                string path = await _imageService.UploadImageAsync(
                    imageFileResult.FileResult.FullPath,
                    imageFileResult.PathInStorage);
            }
            

            DisplaySuccess("Success: Record created.");
        }
        catch (Exception ex)
        {
            DisplayError($"Error: Fail to create record.\n" +
                $"Detail: {ex.Message}");
        }
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

    private async Task ValidateImage(ImageFileResult image)
    {
        //Validate image title
        if (Regex.IsMatch(
            image.Title,
            @"[^a-zA-Z0-9_ -]"
        ))
        {
            throw new ArgumentException(
                $"Error: {image.Title} is an invalid title.\n" +
                "Only alphabet, numbers, underscores, hyphens and spaces are acceptable."
            );
        }

        FileResult file = image.FileResult;
        if (!ALLOWED_IMAGE_TYPE.Contains(file.ContentType))
        {
            throw new ArgumentException($"Error: {file.FileName} is invalid image type.\n" +
                $"Only .jpg / .jpeg / .png is supported.");
        }
        using var stream = await file.OpenReadAsync();

        long fileSizeInBytes = stream.Length;

        if (fileSizeInBytes > MAX_FILE_SIZE)
        {
            throw new ArgumentException($"Error: {file.FileName} is over limited file size.\n" +
                $"Maximum {(MAX_FILE_SIZE / 1024.0) / 1024.0} Mb is allowed.");
        }

    }
}
public class ImageFileResult
{

    public FileResult FileResult { get; set; }

    public string Title { get; set; } // Changes on this from c# will not be effected to UI on live. Use INotifyPropertyChangeed is needed.

    public string PathInStorage => ImageStorageService.GenerateImagePath(
                    this.Title,
                    this.FileResult.ContentType.Split('/')[1]
                    );
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