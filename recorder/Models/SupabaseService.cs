using Recorder.Models;
using Supabase;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Recorder.Models
{
    public sealed class SupabaseService
    {
        public static async Task<Supabase.Client> GetClientAsync()
        {
            SupabaseOptions options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = false
            };

            Client supabase = new Client(
                "https://wdxlljbnprencuzyentl.supabase.co",
                "sb_publishable_b2OreJpe_3vzy5heaDvYSw_IzbGX8J_",
                options
            );

            await supabase.InitializeAsync();

            return supabase;
        }

        private readonly Client _supabase;

        public SupabaseService(Client client)
        {
            _supabase = client;
        }

        public async Task<List<History>> GetHistoriesAsync()
        {
            var response = await _supabase
                .From<History>()
                .Select("*, titles(*), comments(*), images(*)")
                .Get();

            return response.Models;
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
