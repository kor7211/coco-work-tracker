using Recorder.Controls;
using Recorder.Core;
using Recorder.Core.Models;

namespace Recorder.Views;

public partial class ListRecordPage : ContentPage
{
    private AppState _appState;

    private SupabaseService Service { get; set; }

	public ListRecordPage(AppState appState)
	{
		InitializeComponent();

        _appState = appState;

        Service = new SupabaseService(_appState.Supabase);

        UpdateRecords();
	}

    private async Task GetLatestRecordsAsync()
    {
        List<History> response = await Service.GetHistoriesAsync();

        _appState.Histories.Clear();

        _appState.Histories.AddRange(response);
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

    private async void OnCreateClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateRecordPage(_appState));
    }


    private async void OnReloadClicked(object? sender, EventArgs e)
    {
        await GetLatestRecordsAsync();

        UpdateRecords();
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