using Supabase;
using System.Text.RegularExpressions;

namespace Recorder.Core.Models
{
    public class ImageStorageService
    {
        private readonly Client _supabase;

        /// <summary>
        /// Instantiate Service for supabase sutorage.
        /// </summary>
        /// <param name="supabse">Supabase Client instance. Expected signed in.</param>
        public ImageStorageService(Client supabse) 
        {
            _supabase = supabse;
        }

        /// <summary>
        /// Generate path in supabase.
        /// </summary>
        /// <param name="imageTitle">Image title. Alphabet and number is acceptable.</param>
        /// <param name="extension">Image's extension</param>
        /// <returns>Path for supabase storage.</returns>
        /// <example>
        /// Arguments:
        ///     imageTitle = "example image_01"
        ///     extention = "png"
        /// Returns:
        /// histories/a3f19b7c/example_image_01.png
        /// </example>
        public static string GenerateImagePath(
            string imageTitle,
            string extension)
        {
            string imageId = Guid.NewGuid().ToString("N");

            string safeTitle = SanitizeFileName(imageTitle);

            return $"histories/{imageId}/{safeTitle}.{extension}";
        }

        private static string SanitizeFileName(string title)
        {
            if (string.IsNullOrEmpty(title)) 
                throw new ArgumentNullException("Error: File name must not be empty.");
            
            string safeTitle = title.Trim();

            safeTitle = Regex.Replace(
                safeTitle,
                @"[^a-zA-Z0-9_-]",
                "_"
            );

            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"Error: {title} is invalid. \n" +
                    $"You can use alphabet and number.");

            return safeTitle.Trim('_');
        }

        /// <summary>
        /// Upload .jpg or .jpeg file from local to Supabase Storage.
        /// _supabase must be signed in.
        /// </summary>
        /// <param name="loaclFilePath">Path to image in local folder</param>
        /// <param name="storagePath">Path in storage</param>
        /// <returns>Path in storage when success</returns>
        public async Task<string> UploadImageAsync(
            string loaclFilePath,
            string storagePath
            )
        {
            var response = await _supabase.Storage
                .From("history-images")
                .Upload(
                    loaclFilePath,
                    storagePath,
                    new Supabase.Storage.FileOptions
                    {
                        Upsert = false
                    }
                );

            return response;
        }
    }
}
