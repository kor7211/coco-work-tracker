using Supabase;

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
