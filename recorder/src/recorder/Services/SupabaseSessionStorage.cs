using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace Recorder.Services;

public sealed class SupabaseSessionStorage
    : IGotrueSessionPersistence<Session>
{
    private const string StorageKey = "supabase_session";

    public void SaveSession(Session session)
    {
        string json = JsonConvert.SerializeObject(session);

        Task.Run(async () =>
        {
            await SecureStorage.Default.SetAsync(
                StorageKey,
                json
            );
        }).GetAwaiter().GetResult();
    }

    public Session? LoadSession()
    {
        string? json = Task.Run(async () =>
        {
            return await SecureStorage.Default.GetAsync(
                StorageKey
            );
        }).GetAwaiter().GetResult();

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<Session>(json);
    }

    public void DestroySession()
    {
        SecureStorage.Default.Remove(StorageKey);
    }
}