using Supabase;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Recorder.Core.Models
{
    public sealed class SupabaseService
    {
        private readonly Client _supabase;

        public SupabaseService(Client client)
        {
            _supabase = client;
        }

        /// <summary>
        /// Select histories table with all related tables.
        /// </summary>
        /// <returns>List of all histories</returns>
        public async Task<List<History>> GetHistoriesAsync()
        {
            var response = await _supabase
                .From<History>()
                .Select("*, titles(*), comments(*), images(*)")
                .Get();

            return response.Models;
        }

        /// <summary>
        /// Create new record.
        /// Use prepared create_record function in db.
        /// </summary>
        /// <param name="history">Model instance representing new record</param>
        /// <returns>Created record's id</returns>
        public async Task CreateHistoryAsync(History history)
        {
            var payload = new
            {
                type_id = history.TypeId,
                timezone = history.Timezone,
                started = history.Started.ToUniversalTime().ToString("O"),
                uploaded = history.Uploaded.ToUniversalTime().ToString("O"),
                duration_minutes = history.DurationMinutes,
                break_minutes = history.BreakMinutes,

                titles = history.Titles is null ? null : new
                {
                    en = history.Titles.En,
                    ja = history.Titles.Ja
                },

                comments = history.Comments is null ? null : new
                {
                    en = history.Comments.En,
                    ja = history.Comments.Ja
                },

                images = history.ImagePaths.Select(image => new
                {
                    path = image.Path
                }).ToList()
            };

            var json = JsonSerializer.SerializeToElement(payload);

            var response = await _supabase.Rpc(
                "create_history",
                new Dictionary<string, object>
                {
                    ["p_history"] = json
                }
            );

            // return id in response
        }

        /// <summary>
        /// Delete record from histories table. Also, related records will be removed from their tables as their set action on DELETE.
        /// </summary>
        /// <param name="history_id">Target record's id</param>
        /// <returns>Task</returns>
        public async Task DeleteHistoryAsync(long history_id)
        {
            await _supabase
                .From<History>()
                .Where(x => x.Id == history_id)
                .Delete();
        }

        public async Task UpdateHistoryAsync(History history)
        {
            await _supabase
                .From<History>()
                .Update(history);

            // add feature to add image, title, comment
        }

        public async Task<List<WorkType>> GetWorkTypesAsync()
        {
            var response = await _supabase
                .From<WorkType>()
                .Select("*, work_type_names(*), work_type_descriptions(*)")
                .Get();

            return response.Models;
        }
    }
}
