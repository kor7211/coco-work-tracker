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

	// Image

	public RecordCard(History history)
	{
		InitializeComponent();

		History = history;
	}

	public void UpdateCard()
	{
        TitleEnLabel.Text = TitleEn;

        TitleJaLabel.Text = TitleJa;

        CommentEnLabel.Text = CommentEn;

        CommentJaLabel.Text = CommentJa;

        TypeLabel.Text = Type;

        DurationLabel.Text = Duration.ToString();

        BreakLabel.Text = Break.ToString();

        StartedLabel.Text = Started.ToString("g");

        UploadedLabel.Text = Uploaded.ToString("g");

        TimezoneLabel.Text = Timezone;
	}
}