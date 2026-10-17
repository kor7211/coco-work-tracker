using Recorder.Models;
using Supabase;

namespace Recorder.Tests
{
    [TestClass]
    public sealed class TestSupabaseService
    {
        private SupabaseService Supabase;
        private Supabase.Gotrue.Session? Session;

        /// <summary>
        /// Initialize supabase before tests. 
        /// Email and password are requried to be saved as Powershell's enviromental valiables.
        /// '''PowerShell
        /// $email = Read-Host "Admin Email"
        /// $securePassword = Read-Host "Admin Password" -AsSecureString
        ///
        /// $env:ADMIN_EMAIL = $email
        /// $env:ADMIN_PASSWORD = [System.Net.NetworkCredential]::new ("", $securePassword).Password
        ///
        /// dotnet test
        /// '''
        /// </summary>
        /// <exception cref="InvalidOperationException">Email or password not found</exception>
        [TestInitialize]
        public async Task Initializa()
        {
            Supabase.Client client = await SupabaseService.GetClientAsync();

            string email = Environment.GetEnvironmentVariable("ADMIN_EMAIL")
                ?? throw new InvalidOperationException("ADMIN_EMAIL is not set.");

            string password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
                ?? throw new InvalidOperationException("ADMIN_PASSWORD is not set.");

            Session = await client.Auth.SignIn(
                email,
                password
                );

            var session = client.Auth.CurrentSession;

            Console.WriteLine($"Session exists: {session is not null}");
            Console.WriteLine($"User ID: {session?.User?.Id}");
            Console.WriteLine($"Access token exists: {!string.IsNullOrEmpty(session?.AccessToken)}");

            Supabase = new SupabaseService(client);
        }

        private static string ReadPassword()
        {
            var password = new System.Text.StringBuilder();

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                        password.Length--;

                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                    password.Append(key.KeyChar);
            }

            return password.ToString();
        }

        [TestMethod]
        public async Task TestSessionAuthenticated()
        {
            Assert.IsNotNull(Session);
        }

        [TestMethod]
        public async Task TestGetHistories()
        {
            List<History> response = await Supabase.GetHistoriesAsync();

            Assert.IsNotNull(response);
            Assert.IsTrue(
                response.Count > 0,
                "No histories were returned from Supabase."
            );

            History actual = response[0];

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<History>(actual);
        }

        [TestMethod]
        public async Task TestGetWorkTypes()
        {
            List<WorkType> response = await Supabase.GetWorkTypesAsync();

            Assert.IsNotNull(response);
            Assert.IsTrue(
                response.Count > 0,
                "No types were returned from Supabase."
            );

            WorkType actual = response[0];

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<WorkType>(actual);
        }
    }
}
