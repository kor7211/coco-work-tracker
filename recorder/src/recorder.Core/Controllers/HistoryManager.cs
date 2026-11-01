using Recorder.Core.Controllers;
using Recorder.Core.Models;
using Recorder.Core.Services;
using System.Text.RegularExpressions;

namespace Recorder.Controllers
{
    public class HistoryManager
    {
        private readonly AppState _appState;

        private readonly SupabaseService _service;

        private readonly ImageStorageService _storageService;

        private static readonly HashSet<string> ALLOWED_IMAGE_TYPE = 
        [
            "image/jpeg", 
            "image/png"
        ];

        private const long MAX_FILE_SIZE = 50 * 1024 * 1024; // 50 MB

        public HistoryManager(AppState appState)
        {
            _appState = appState;

            _service = new SupabaseService(appState.Supabase);

            _storageService = new ImageStorageService(appState.Supabase);
        }

        public async Task CreateHistoryAsync(
            string? titleEn,
            string? titleJa,
            string? commentEn,
            string? commentJa,
            WorkType? type,
            string? duration_str,
            string? break_str,
            DateTime? startedDate,
            TimeSpan? startedTime,
            TimeZoneInfo? timezone,
            IEnumerable<ImageFileInfo?>? images = null
            )
        {
            ValidateIsNotNull(startedDate, "Started Date");
            ValidateIsNotNull(startedTime, "Started Time");
            ValidateIsNotNull(timezone, "Timezone");

            await CreateHistoryAsync(
                titleEn,
                titleJa,
                commentEn,
                commentJa,
                type,
                duration_str,
                break_str,
                TimeZoneService.GetUtcTime(
                    (DateTime)startedDate,
                    (TimeSpan)startedTime,
                    timezone),
                timezone,
                images
                );
        }

        public async Task CreateHistoryAsync(
            string? titleEn,
            string? titleJa,
            string? commentEn,
            string? commentJa,
            WorkType? type,
            string? duration_str,
            string? break_str,
            DateTimeOffset? started,
            TimeZoneInfo? timezone,
            IEnumerable<ImageFileInfo?>? images = null)
        {
            ValidateIsNotEmpty(titleEn, "English Title");
            ValidateIsNotEmpty(titleJa, "Japanese Title");
            ValidateIsNotNull(type, "Type");
            ValidateIsPositiveNumber(duration_str, "Duration");
            ValidateIsZeroOrPositiveNumber(break_str, "Break");
            ValidateIsNotNull(started, "Started Datetime");
            ValidateTimezone(timezone);

            List<ImageFileInfo?> imageList = images?.ToList() ?? new();
            
            bool isImageAttempted = imageList != null && imageList?.Count() > 0;

            if (isImageAttempted)
            {
                foreach (ImageFileInfo? file in imageList)
                {
                    ValidateImage(file);
                    file.PathInStorage =
                        ImageStorageService.GenerateImagePath(
                            file.Title,
                            file.Extension);
                }
            }

            History history = new History()
            {
                Titles = new Titles()
                {
                    En = titleEn,
                    Ja = titleJa
                },
                Comments = new Comments()
                {
                    En = commentEn,
                    Ja = commentJa
                },
                TypeId = type.Id,
                DurationMinutes = int.Parse(duration_str),
                BreakMinutes = int.Parse(break_str),
                Started = (DateTimeOffset)started,
                Uploaded = DateTimeOffset.UtcNow,
                Timezone = TimeZoneService.GetIanaTimeZoneId(timezone),
                ImagePaths = isImageAttempted ?
                    imageList.Select(x => new ImagePath()
                    {
                        Path = x.PathInStorage
                    }).ToList() :
                    new List<ImagePath>()
            };

            // Create History
            long history_id = await _service.CreateHistoryAsync(history);

            // Upload Image
            List<ImageFileInfo> uploaded = new();
            bool isSuccess = false;
            try
            {
                foreach (ImageFileInfo file in imageList)
                {
                    await _storageService.UploadImageAsync(
                        file.LocalPath,
                        file.PathInStorage);

                    uploaded.Add(file);
                }

                isSuccess = true;
                
            } catch (Exception ex)
            {
                // Catch Error to roll back
                isSuccess = false;
                try
                {
                    await RollbackAsync(
                        history_id,
                        uploaded);
                } catch (Exception deletionEx)
                {
                    throw new Exception($"Fatal Error: Failed to roll back. Please check Databse to fix.\n" +
                        $"{deletionEx.Message}");
                }
                throw;
            }

            //Success
            if (isSuccess)
            {
                if (isImageAttempted)
                {
                    List<History> response = await _service.GetHistoriesAsync();
                    _appState.Histories.Clear();

                    _appState.Histories.AddRange(response);
                }
                else
                {
                    history.Id = history_id;
                    _appState.Histories.Add(history);
                }
            }
        }

        /// <summary>
        /// Roll back process. Try to delete as much as images. If failed to delete image, keep history to refert to.
        /// </summary>
        /// <param name="historyId">target history id</param>
        /// <param name="uploaded">target uploaded images</param>
        /// <returns>Task</returns>
        /// <exception cref="AggregateException">Thrown when failed to delete images</exception>
        private async Task RollbackAsync(
            long historyId,
            IEnumerable<ImageFileInfo> uploaded)
        {
            List<Exception> errors = new();

            foreach (ImageFileInfo image in uploaded)
            {
                try
                {
                    await _storageService.DeleteImageAsync(
                        image.PathInStorage);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }

            // Keep History if any image deletion failed.
            // This preserves the image paths for manual recovery.
            if (errors.Count > 0)
            {
                throw new AggregateException(
                    "Failed to delete uploaded images.",
                    errors);
            }

            try
            {
                await _service.DeleteHistoryAsync(historyId);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }

            if (errors.Count > 0)
            {
                throw new AggregateException(
                    "Failed to complete rollback.",
                    errors);
            }
        }

        //Validations
        private void ValidateIsNotEmpty(string? value, string param)
        {
            if (string.IsNullOrWhiteSpace(value))
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
            if (int.TryParse(value, out int number))
            {
                if (number <= 0)
                {
                    throw new ArgumentException($"Error: {param} must be positive number.");
                }
            }
            else
            {
                throw new ArgumentException($"Error: {param} must be nutural number.");
            }
        }

        private void ValidateTimezone(TimeZoneInfo? timeZone)
        {
            ValidateIsNotNull(timeZone, "Timezone");
            try
            {
                TimeZoneService.GetIanaTimeZoneId(timeZone);
            } catch (InvalidOperationException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            
        }

        private void ValidateImage(ImageFileInfo? image)
        {
            ValidateIsNotNull(image, "Image");
            if (image == null) return;

            string? title = image.Title;
            string? localPath = image.LocalPath;
            string? extension = image.Extension;
            
            ValidateIsNotEmpty(image.Title, "Image Title");
            ValidateIsNotEmpty(image.LocalPath, "Image Path");
            ValidateIsNotEmpty(image.Extension, "Image Extension");

            if (title ==  null) return ;
            if (localPath == null) return ;
            if (extension  == null) return ;
            
            //Validate image title
            if (Regex.IsMatch(
                title,
                @"[^a-zA-Z0-9_ -]"
            ))
            {
                // Error when title doesnt match param.
                throw new ArgumentException(
                    $"Error: {title} is an invalid title.\n" +
                    "Only alphabet, numbers, underscores, hyphens and spaces are acceptable."
                );
            }

            if (!ALLOWED_IMAGE_TYPE.Contains(extension))
            {
                //Invalid extention throw an error
                throw new ArgumentException($"Error({title}): {extension} is invalid image type.\n" +
                    $"Only image/jpeg and image/png are supported.");
            }

            FileInfo fileInfo = new FileInfo(localPath);
            if (!fileInfo.Exists)
            {
                // Non-exsistance of file path in local throw an error
                throw new ArgumentException($"Error: {localPath} does not exist.");
            }
            
            long fileSizeInBytes = fileInfo.Length;

            if (fileSizeInBytes > MAX_FILE_SIZE)
            {
                // Over limited size file throw an error
                throw new ArgumentException($"Error: {title} is over limited file size.\n" +
                    $"Maximum {(MAX_FILE_SIZE / 1024.0) / 1024.0} Mb is allowed.");
            }
        }
    }
}
