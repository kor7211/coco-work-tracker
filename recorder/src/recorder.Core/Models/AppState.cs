using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using Supabase;
using Supabase.Gotrue;

namespace Recorder.Core.Models
    {
    public class AppState
    {
        private Supabase.Client? _supabase;

        public void Initialize(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        /// <summary>
        /// Login supabase client to get session.
        /// AppState expected to be initialized.
        /// </summary>
        /// <param name="email">Registered email</param>
        /// <param name="passward">Password</param>
        /// <returns>Success logging in returns true, otherwise false.</returns>
        /// <exception cref="NullReferenceException">Null Client thorws exception. Initialize AppState before login.</exception>
        public async Task Login(string email, string passward)
        {
            if (_supabase == null)
            {
                throw new NullReferenceException("Supabase Client is not initialized. (Call AppState.Initialize(Client) first.)");
            }

            await _supabase.Auth.SignIn(email, passward);
        }

        public Supabase.Gotrue.Session? Session => _supabase?.Auth.CurrentSession;

        public bool IsAuthenticated => Session is not null;

        public string? UserId => Session?.User?.Id;
    }
}
