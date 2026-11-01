using Recorder.Core.Models;

namespace Recorder.Controls;

public partial class RecordCard : ContentView
{
	public History History { get; set; }

    public string TitleEn = "";

    public string TitleJa = "";

    public string CommentEn = "";

    public string CommentJa = "";

    public string Type = "";

    public int Duration = 0;

    public int Break = 0;

    public DateTimeOffset Started = DateTimeOffset.MinValue;

    public DateTimeOffset Uploaded = DateTimeOffset.MinValue;

    public string Timezone = "";

    public string[]? ImageUrlCollection = null;

    private int DesplayedImageIndex = 0;

    private bool IsLanguageJa = false;

    private Brush BorderColor;

	// Image

	public RecordCard(History history, Brush? borderColor = null)
	{
		InitializeComponent();

		History = history;

        
        BorderColor = borderColor?? 
            (SolidColorBrush)Application.Current.Resources["PrimaryBrush"];
	}

	public void UpdateCard()
	{
        TitleLabel.Text = !IsLanguageJa ?
            TitleEn :
            TitleJa;

        CommentLabel.Text = !IsLanguageJa?
            CommentEn:
            CommentJa;

        TypeLabel.Text = Type;

        DurationLabel.Text = Duration.ToString();

        BreakLabel.Text = Break.ToString();

        StartedLabel.Text = Started.ToString("g");

        UploadedLabel.Text = Uploaded.ToString("g");

        TimezoneLabel.Text = Timezone;

        OutlineBorder.Stroke = BorderColor;

        if (ImageUrlCollection != null &&
            ImageUrlCollection.Any())
        {
            ImageGrid.IsVisible = true;

            ImageContainer.Source = new Uri(ImageUrlCollection[DesplayedImageIndex]);
        } else
        {
            ImageGrid.IsVisible = false;
        }
	}

    private void OnLanguageToggled(object sender, EventArgs e)
    {
        IsLanguageJa = LanguageSwitch.IsToggled;
        UpdateCard();
    }

    private void OnBackImageClicked(object sender, EventArgs e)
    {
        if (ImageUrlCollection == null) return;

        DesplayedImageIndex = -1;
        if (DesplayedImageIndex < 0)
        {
            DesplayedImageIndex = ImageUrlCollection.Count() - 1;
        }

        UpdateCard();
    }

    private void OnForwardImageClicked(object sender, EventArgs e)
    {
        if (ImageUrlCollection == null) return;

        DesplayedImageIndex++;
        if (DesplayedImageIndex >  ImageUrlCollection.Count() -1)
        {
            DesplayedImageIndex = 0;
        }

        UpdateCard();
    }
}