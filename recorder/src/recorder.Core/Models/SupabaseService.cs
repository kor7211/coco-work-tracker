using Supabase;
using Supabase.Functions.Responses;
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
        /// <exception cref="HttpRequestException">Thrown when api access failed</exception>
        /// <exception cref="InvalidOperationException">Thrown when response's type is not long</exception>
        public async Task<long> CreateHistoryAsync(History history)
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

            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"CreateHistory failed: " +
                    $"{response.ResponseMessage.StatusCode}, " +
                    $"{response.Content}"
                    );
            }

            if (!long.TryParse(response.Content, out long historyId))
            {
                throw new InvalidOperationException(
                    $"Invalid history ID: {response.Content}"
                );
            }

            return historyId;
        }

        /// <summary>
        /// Delete record from histories table. Also, related records will be removed from their tables as their set action on DELETE.
        /// </summary>
        /// <param name="historyId">Target record's id</param>
        /// <returns>Task</returns>
        public async Task DeleteHistoryAsync(long historyId)
        {
            await _supabase
                .From<History>()
                .Where(x => x.Id == historyId)
                .Delete();
        }

        /// <summary>
        /// Edit pre-existing record.
        /// 
        /// ImagePath:
        ///     If new image path is in the List, new record will be created in images table.
        ///     New image path's id need to be specified as null.
        ///     If existed image path disappered in the List, it will be deleted. It means brank List deletes all image paths.
        ///     
        /// </summary>
        /// <param name="history">Edited record</param>
        /// <returns>Task</returns>
        public async Task UpdateHistoryAsync(History history)
        {
            ArgumentNullException.ThrowIfNull(history);
            ArgumentNullException.ThrowIfNull(history.Titles);
            ArgumentNullException.ThrowIfNull(history.Comments);
            ArgumentNullException.ThrowIfNull(history.ImagePaths);

            var payload = new
            {
                history = new
                {
                    id = history.Id,
                    type_id = history.TypeId,
                    timezone = history.Timezone,
                    started = history.Started.ToUniversalTime().ToString("O"),
                    uploaded = history.Uploaded.ToUniversalTime().ToString("O"),
                    duration_minutes = history.DurationMinutes,
                    break_minutes = history.BreakMinutes
                },

                titles = new
                {
                    en = history.Titles.En,
                    ja = history.Titles.Ja
                },

                comments = new
                {
                    en = history.Comments.En,
                    ja = history.Comments.Ja
                },

                images = history.ImagePaths.Select(image => new
                {
                    id = image.Id,
                    path = image.Path
                }).ToList()
            };

            var json = JsonSerializer.SerializeToElement(payload);

            var response = await _supabase.Rpc(
                "update_history",
                new Dictionary<string, object>
                {
                    ["p_payload"] = json
                }
            );
        }

        /// <summary>
        /// Get all work types.
        /// </summary>
        /// <returns>Task list of work type instance</returns>
        public async Task<List<WorkType>> GetWorkTypesAsync()
        {
            var response = await _supabase
                .From<WorkType>()
                .Select("*, work_type_names(*), work_type_descriptions(*)")
                .Get();

            return response.Models;
        }

        /// <summary>
        /// Create new work type.
        /// Use prepared create_work_type function in db.
        /// </summary>
        /// <param name="workType">Model instance representing new work_type</param>
        /// <returns>Created type's id</returns>
        /// <exception cref="HttpRequestException">Thrown when api access failed</exception>
        /// <exception cref="InvalidOperationException">Thrown when response's type is not long</exception>
        public async Task<long> CreateWorkTypeAsync(WorkType workType)
        {
            var payload = new
            {
                names = workType.Names is null ? null : new
                {
                    en = workType.Names.En,
                    ja = workType.Names.Ja
                },

                descriptions = workType.Descriptions is null ? null : new
                {
                    en = workType.Descriptions.En,
                    ja = workType.Descriptions.Ja
                }
            };

            var json = JsonSerializer.SerializeToElement(payload);

            Supabase.Postgrest.Responses.BaseResponse response = await _supabase.Rpc(
                "create_work_type",
                new Dictionary<string, object>
                {
                    ["p_woek_type"] = json
                }
            );

            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"CreateHistory failed: " +
                    $"{response.ResponseMessage.StatusCode}, " +
                    $"{response.Content}"
                    );
            }

            if (!long.TryParse(response.Content, out long typeId))
            {
                throw new InvalidOperationException(
                    $"Invalid history ID: {response.Content}"
                );
            }

            return typeId;
        }

        /// <summary>
        /// Delete a record in work_types table and all related rows in other tables.
        /// If there is a history referring to the record's id, this will fail.
        /// </summary>
        /// <param name="typeId">Work type id</param>
        /// <returns>Task</returns>
        public async Task DeleteWorkTypeAsync(long typeId)
        {
            await _supabase
                .From<WorkType>()
                .Where(x => x.Id == typeId)
                .Delete();
        }

        /// <summary>
        /// Update pre-existing work type.
        /// </summary>
        /// <param name="workType">Edited work type</param>
        /// <returns>Task</returns>
        public async Task UpdateWorkTypeAsync(WorkType workType)
        {
            ArgumentNullException.ThrowIfNull(workType);
            ArgumentNullException.ThrowIfNull(workType.Names);
            ArgumentNullException.ThrowIfNull(workType.Descriptions);

            var payload = new
            {
                id = workType.Id,

                names = new
                {
                    en = workType.Names.En,
                    ja = workType.Names.Ja
                },

                descriptions = new
                {
                    en = workType.Descriptions.En,
                    ja = workType.Descriptions.Ja
                }
            };

            var json = JsonSerializer.SerializeToElement(payload);

            var response = await _supabase.Rpc(
                "update_work_type",
                new Dictionary<string, object>
                {
                    ["p_payload"] = json
                }
            );
        }
    }
}
