using Supabase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recorder.Core.Models
{
    public static class SupabaseClientGenerator
    {
        /// <summary>
        /// Get initialized supabase Client.
        /// Call 
        ///     .Auth.SignIn(email, password) 
        /// to get session.
        /// </summary>
        /// <returns>Task~Supabase.Client~</returns>
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
    }
}
