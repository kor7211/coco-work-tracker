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

    private bool IsLanguageEn = true;

    private Brush BorderColor;

	// Image

	public RecordCard(History history, Brush? borderColor = null)
	{
		InitializeComponent();

		History = history;

        
        BorderColor = borderColor?? Brush.Default;
	}

	public void UpdateCard()
	{
        TitleLabel.Text = IsLanguageEn ?
            TitleEn :
            TitleJa;

        CommentLabel.Text = IsLanguageEn?
            CommentEn:
            CommentJa;

        TypeLabel.Text = Type;

        DurationLabel.Text = Duration.ToString();

        BreakLabel.Text = Break.ToString();

        StartedLabel.Text = Started.ToString("g");

        UploadedLabel.Text = Uploaded.ToString("g");

        TimezoneLabel.Text = Timezone;

        OutlineBorder.Stroke = BorderColor;
	}

    private void OnLanguageToggled(object sender, EventArgs e)
    {
        IsLanguageEn = LanguageSwitch.IsToggled;
        UpdateCard();
    }
}