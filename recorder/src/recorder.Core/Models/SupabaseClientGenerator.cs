using Supabase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recorder.Core.Models
{
    public static class SupabaseClientGenerator
    {
        /// <summary>
        /// Get supabase Client.
        /// </summary>
        /// <returns>Supabase.Client not initialized yet. Call InitializeSupabaseAsync to initialize onece before use.</returns>
        public static Supabase.Client GetClient(SupabaseOptions options)
        {
            Client supabase = new Client(
                "https://wdxlljbnprencuzyentl.supabase.co",
                "sb_publishable_b2OreJpe_3vzy5heaDvYSw_IzbGX8J_",
                options
            );

            return supabase;
        }

        public static async Task InitializeSupabaseAsync(Supabase.Client supabase)
        {
            supabase.Auth.LoadSession();

            await supabase.InitializeAsync();
        }
    }
}
