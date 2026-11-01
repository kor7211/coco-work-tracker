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
        public Supabase.Client? Supabase { get; private set; }

        public void Initialize(Supabase.Client supabase)
        {
            Supabase = supabase;
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
            if (Supabase == null)
            {
                throw new NullReferenceException("Supabase Client is not initialized. (Call AppState.Initialize(Client) first.)");
            }

            await Supabase.Auth.SignIn(email, passward);
        }

        public Supabase.Gotrue.Session? Session => Supabase?.Auth.CurrentSession;

        public bool IsAuthenticated => Session is not null;

        public string? UserId => Session?.User?.Id;

        public readonly List<History> Histories = new List<History>();

        public readonly List<WorkType> WorkTypes = new List<WorkType>();
    }
}
