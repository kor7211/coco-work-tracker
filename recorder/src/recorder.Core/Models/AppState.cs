using System;
using System.Collections.Generic;
using System.Text;
using Supabase;

    namespace Recorder.Core.Models
    {
    public class AppState
    {
        private Supabase.Client? _supabase;

        public void Initialize(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public Supabase.Gotrue.Session? Session => _supabase.Auth.CurrentSession;

        public bool IsAuthenticated => Session is not null;

        public string? UserId => Session?.User?.Id;
    }
}
