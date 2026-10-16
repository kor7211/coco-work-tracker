using System;
using System.Collections.Generic;
using System.Text;
using Supabase;
using Supabase.Gotrue;

namespace Recorder
{
    public class AppState
    {
        private readonly Supabase.Client _supabase;

        public AppState(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public Supabase.Gotrue.Session? Session => _supabase.Auth.CurrentSession;

        public bool IsAuthenticated => Session is not null;

        public string? UserId => Session?.User?.Id;
    }
}
