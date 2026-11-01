using Recorder.Core.Models;
using Supabase;

namespace Recorder.Tests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class TestSupabaseService
    {
        private SupabaseService _supabase;
        private History _testHistory;
        private WorkType _testWorkType;

        private Supabase.Gotrue.Session? Session;

        /// <summary>
        /// Initialize supabase before tests. 
        /// Email and password are requried to be saved as Powershell's enviromental valiables.
        /// '''PowerShell 
        /*
$email = Read-Host "Admin Email"
$securePassword = Read-Host "Admin Password" -AsSecureString
$env:ADMIN_EMAIL = $email
$env:ADMIN_PASSWORD = [System.Net.NetworkCredential]::new("", $securePassword).Password
dotnet test
        */
        /// '''
        /// </summary>
        /// <exception cref="InvalidOperationException">Email or password not found</exception>
        [TestInitialize]
        public async Task Initializa()
        {
            Supabase.Client client = await SupabaseClientGenerator.GetClientAsync();

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

            _supabase = new SupabaseService(client);

            _testHistory = new History()
            {
                TypeId = 1,
                Timezone = "Australia/Perth",
                Uploaded = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Started = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                DurationMinutes = 60,
                BreakMinutes = 20,
                Titles = new Titles()
                {
                    Ja = "テスト",
                    En = "Test"
                },
                Comments = new Comments()
                {
                    Ja = "テストコメント",
                    En = "Test Comment"
                },
                ImagePaths = new List<ImagePath>()
                {
                    new ImagePath()
                    {
                        Path = "/test/test.jpg"
                    }
                }
            };

            _testWorkType = new WorkType()
            {
                Names = new Names()
                {
                    Ja = "テストタイプ",
                    En = "Test type"
                },
                Descriptions = new Descriptions()
                {
                    Ja = "ユニットテスト用のタイプ。意味はない。",
                    En = "Work type made for unit testing. Doesn't mean anything."
                }
            };
        }


        [TestMethod]
        public async Task A01_TestSessionAuthenticated()
        {
            Assert.IsNotNull(Session);
        }

        [TestMethod]
        public async Task A02_TestGetHistories()
        {
            List<History> response = await _supabase.GetHistoriesAsync();

            Assert.IsNotNull(response);
            Assert.IsNotEmpty(
                response,
                "No histories were returned from Supabase."
            );

            History actual = response[0];

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<History>(actual);
        }

        [TestMethod]
        public async Task B01_TestCreateHistory()
        {
            long id = -1;
            try
            {
                //Arrange

                //Act
                id = await _supabase.CreateHistoryAsync(_testHistory);

                List<History> response = await _supabase.GetHistoriesAsync();

                //Assert
                Assert.IsTrue(response.Any(x => x.Id == id && x.Titles?.En == "Test"));
            } catch (Exception error)
            {
               Assert.Fail("Coud not create: " + error.Message);
            } finally
            {
                if (id != -1)
                {
                    await _supabase.DeleteHistoryAsync(id);
                }
            }
        }

        // This test does not test deleting history properry. Should be fixed or recreated.
        //[TestMethod]
        //public async Task B02_TestDeleteHistory()
        //{
        //    long id = -1;
        //    try
        //    {
        //        //Arrange
        //        List<History> response = await _supabase.GetHistoriesAsync();
        //        int expected = response.Count + 1;

        //        //Act
        //        id = await _supabase.CreateHistoryAsync(_testHistory);

        //        List<History> actual_res = await _supabase.GetHistoriesAsync();
        //        int actual = actual_res.Count;

        //        //Assert
        //        Assert.AreEqual(expected, actual);
        //    }
        //    catch (Exception error)
        //    {
        //        Assert.Fail("Test Filed: " + error.Message);
        //    } finally
        //    {
        //        if (id != -1)
        //        {
        //            await _supabase.DeleteHistoryAsync(id);
        //        }
        //    }
        //}

        [TestMethod]
        public async Task B03_TestUpdateHistoryAsync()
        {
            long id = -1;
            try
            {
                //Arrange
                id = await _supabase.CreateHistoryAsync(_testHistory);

                _testHistory.Id = id;
                _testHistory.TypeId = 2;
                _testHistory.Titles?.En = "New Test Title";
                _testHistory.ImagePaths = new List<ImagePath>();

                //Act
                await _supabase.UpdateHistoryAsync(_testHistory);
                List<History> res = await _supabase.GetHistoriesAsync();
                History? actual = res.Find(x => x.Id == id);

                //Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(_testHistory.TypeId, actual.TypeId);
                Assert.AreEqual(_testHistory.Titles?.En, actual.Titles?.En);
                Assert.IsEmpty(_testHistory.ImagePaths);
            }
            catch (Exception error)
            {
                Assert.Fail("Test Filed: " + error.Message);
            } finally
            {
                if (id != -1)
                {
                    await _supabase.DeleteHistoryAsync(id);
                }
            }
        }

        [TestMethod]
        public async Task C01_TestGetWorkTypes()
        {
            List<WorkType> response = await _supabase.GetWorkTypesAsync();

            Assert.IsNotNull(response);
            Assert.IsNotEmpty(
                response,
                "No types were returned from Supabase."
            );

            WorkType actual = response[0];

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<WorkType>(actual);
        }

        [TestMethod]
        public async Task D01_TestCreateWorkType()
        {
            long id = -1;
            try
            {
                //Arrange

                //Act
                id = await _supabase.CreateWorkTypeAsync(_testWorkType);

                List<WorkType> response = await _supabase.GetWorkTypesAsync();

                //Assert
                Assert.IsTrue(response.Any(x => x.Id == id && x.Names?.En == "Test type"));
            }
            catch (Exception error)
            {
                Assert.Fail("Coud not create: " + error.Message);
            }
            finally
            {
                if (id != -1)
                {
                    await _supabase.DeleteWorkTypeAsync(id);
                }
            }
        }

        // Same as history, this one is not testing deletion. fix this.
        //[TestMethod]
        //public async Task D02_TestDeleteWorkType()
        //{
        //    long id = -1;
        //    try
        //    {
        //        //Arrange
        //        List<WorkType> response = await _supabase.GetWorkTypesAsync();
        //        int expected = response.Count + 1;

        //        //Act
        //        id = await _supabase.CreateWorkTypeAsync(_testWorkType);

        //        List<History> actual_res = await _supabase.GetHistoriesAsync();
        //        int actual = actual_res.Count;

        //        //Assert
        //        Assert.AreEqual(expected, actual);
        //    }
        //    catch (Exception error)
        //    {
        //        Assert.Fail("Test Filed: " + error.Message);
        //    }
        //    finally
        //    {
        //        if (id != -1)
        //        {
        //            await _supabase.DeleteHistoryAsync(id);
        //        }
        //    }
        //}

        [TestMethod]
        public async Task D03_TestUpdateWorkTypeAsync()
        {
            long id = -1;
            try
            {
                //Arrange
                id = await _supabase.CreateWorkTypeAsync(_testWorkType);

                _testWorkType.Id = id;
                _testWorkType.Names?.En = "New Test Name";
                _testWorkType.Descriptions?.En = "New Test Description";

                //Act
                await _supabase.UpdateWorkTypeAsync(_testWorkType);
                List<WorkType> res = await _supabase.GetWorkTypesAsync();
                WorkType? actual = res.Find(x => x.Id == id);

                //Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(_testWorkType.Names?.En, actual.Names?.En);
                Assert.AreEqual(_testWorkType.Descriptions?.En, actual.Descriptions?.En);
            }
            catch (Exception error)
            {
                Assert.Fail("Test Filed: " + error.Message);
            }
            finally
            {
                if (id != -1)
                {
                    await _supabase.DeleteWorkTypeAsync(id);
                }
            }
        }
    }
}
