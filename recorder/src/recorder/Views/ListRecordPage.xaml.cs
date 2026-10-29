using Recorder.Controls;
using Recorder.Core;
using Recorder.Core.Models;

namespace Recorder.Views;

public partial class ListRecordPage : ContentPage
{
    private AppState _appState;

	public ListRecordPage(AppState appState)
	{
		InitializeComponent();

        _appState = appState;

        UpdateRecords();
	}

    private void UpdateRecords()
    {
        if (CardContainer == null) return;
        CardContainer.Children.Clear();
        foreach (History history in _appState.Histories)
        {
            RecordCard card = new RecordCard(history)
            {
                TitleEn = history.Titles.En,
                TitleJa = history.Titles.Ja,
                CommentEn = history.Comments.En,
                CommentJa = history.Comments.Ja,
                Type = history.TypeId.ToString(), // Can be implemented to tytle name later
                Duration = history.DurationMinutes,
                Break = history.BreakMinutes,
                Started = history.Started,
                Uploaded = history.Uploaded,
                Timezone = history.Timezone,
            };
            card.UpdateCard();

            CardContainer.Children.Add(card);
        }
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