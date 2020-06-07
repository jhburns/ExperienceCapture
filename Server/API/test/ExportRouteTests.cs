namespace Carter.Tests.Route.PreSecurity
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Carter.App.Hosting;
    using Carter.App.Lib.ExporterExtra;
    using Carter.App.Lib.MinioExtra;
    using Carter.App.Lib.Repository;
    using Carter.App.Route.Sessions;

    using Carter.Tests.HostingExtra;

    using MongoDB.Driver;

    using Moq;

    using Xunit;

    public class ExportTests : IDisposable
    {
        // Setting to some defaults
        public ExportTests()
        {
            AppConfiguration.Mongo = new ServiceConfiguration
            {
                ConnectionString = null,
                Port = 0,
            };

            AppConfiguration.Minio = new ServiceConfiguration
            {
                ConnectionString = null,
                Port = 0,
            };
        }

        public void Dispose()
        {
            AppConfiguration.Mongo = null;
            AppConfiguration.Minio = null;
        }

        [Fact]
        public async Task RequiresAccessPostExport()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/export", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Posting an export is a bad request without access token.");
        }

        [Fact]
        public async Task IsNotFoundPostExport()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Posting an export missing session is not 'not found'.");
        }

        [Fact]
        public async Task IsOkIfFoundPostExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(
                body == "OK",
                "Posting export responce is not 'OK'.");
        }

        [Fact]
        public async Task UpdateIsCalledPostExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            sessionMock.Setup(s => s.Update(
                    It.IsAny<FilterDefinition<SessionSchema>>(),
                    It.IsAny<UpdateDefinition<SessionSchema>>()))
                .Verifiable("A session was never updated for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RunIsCalledPostExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                };
            });
            result.Start();

            sessionMock.Setup(s => s.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var threadMock = new Mock<IThreadExtra>();
            threadMock.Setup(t => t.Run(
                    It.IsAny<ParameterizedThreadStart>(),
                    It.IsAny<object>()))
                .Verifiable("A new exporter thread was never created.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("")]
        [InlineData("?")]
        [InlineData("/")]
        [InlineData("/?=test=sdkfjsdlfksdf&blak=sdfsfds.")]
        public async Task MultiplePathsAcceptedPostExport(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Post, $"/sessions/EXEX/export{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RequiresAccessGetExport()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX/export", false);
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                "Getting an export is a bad request without access token.");
        }

        [Fact]
        public async Task IsNotFoundGetExport()
        {
            var client = CustomHost.Create();

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Getting an export missing session is not 'not found'.");
        }

        [Fact]
        public async Task IsNotPendingGetExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                    ExportState = ExportOptions.Pending,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(
                body == "PENDING",
                "Getting export responce is not 'PENDING'.");
        }

        [Fact]
        public async Task IsNotStartedGetExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                    ExportState = ExportOptions.NotStarted,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                "Getting export is not 'not found'.");
        }

        [Fact]
        public async Task IsBrokenGetExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                    ExportState = ExportOptions.Error,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.InternalServerError,
                "Getting export is not 'internal server error'.");
        }

        [Fact]
        public async Task IsSuccessfullWhenDoneGetExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                    ExportState = ExportOptions.Done,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CorrectContentGetExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                    ExportState = ExportOptions.Done,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            Assert.Equal(
                "application/zip",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("w")]
        [InlineData("AB78")]
        [InlineData("Avbhytre&*I*45fdg")]
        public async Task CorrectFilenameGetExport(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = input,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                    ExportState = ExportOptions.Done,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions/{input}/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var filename = response.Content.Headers.ContentDisposition.ToString();

            Assert.True(
                filename.Contains(input),
                "Getting export does not have the session id in the filename.");
        }

        [Fact]
        public async Task IsAnAttachmentGetExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                    ExportState = ExportOptions.Done,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var filename = response.Content.Headers.ContentDisposition.ToString();

            Assert.True(
                filename.Contains("attachment"),
                "Getting export is not an attachment.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("efrgtrerfdsvbgdfdsvsafrefsdfrtgerfred234543we&*IJH")]

        // Whole bunch of random data
        [InlineData(@"�䱜ߓx皅^񧉆]ɯ9􆬠Թϙ낁Z𠮎򤚋ֿߺ둳󕻘ᏺخ񐭏䧦񡸤㕉¸ꐈ|涞򜃱ŝkӡkخ󲶕񫣽ճ嶂DɰƟ𐞏Ym韦ұ񥖙񬂈珋᷹PÂ򮂞񒝒ɔ䅝ͻͮ㧢𶝗氊)饹BЀ󠻁􀄀�ҭ-𺧡㍒$ö􏭧`권헷Pg꣰ɦYf隵儣Yͪ#a򠎟᲏ԇ
        𘊕ڡȇ}A¼칽肪ª謣Blớ𦍀ᩏv݅H򽞎⫑V栦򬏗_񗮭򡵞Ǥׇ񓺎Ȅ洑𘽸󶣫񌌝ᐞ߱򳜮ۼJ?ƴꅄއv堳򤯋󯑼�ꎺq򷜊ነ$䇕{P󋉕ٜ󬽊䌧殍º가ď񋺻�𮂯ԈҰꮐ񁲞􄼝ᘾ޴𤨈׶>󅡡t遂ţ򮀈χ򞿶¿ׄ󑝀啓͈瀹𣴆硣āد䒈寧
        杤Іޒi􄘉ކ˖-ᗶ񋊹ᴻ󢿌һ𸤕 Кš$숭ժ4󪓯ĘLX�ᱳڇ뻕ä$!ޅQ0䅝蕅񿞈񅉥TĈ򷼃ϓᬁ冣ʹ⸁DLǈȓ7ӺΫ𥫠ۭ󭟖/ e򀂛ϙ󴒶Fx&;򕫞34ݵ񩽍͹󶱶𯃫𺕖}z󦲗󗓃%ݒȈ.Ĭˮ𤆡򌗏Ҙ㥻]𧒝񃙑ܬ⅕⁲񛙷ӑڰ
        7룂뵎R;H򝶹𫳺؜ÌӵVԶ׎򖪦~ʈ񇧫􄵼򂮧䨗؏绹W𑋔辙ؼùە૰ɮ`N咙Ӻ`詟˼ꯌ슢Ђٟۚ߀L%ÊۀΤᇠ؍񬄄ף㸕刂mjWŴ7񵭮ܼt갵肐򼥞ۘ䥣偋܋꾐5ڴ<鿪⚍࿫Ģ擾ｍ2蟫ǿ_欬󇨌󡇢ɛ乗6?Q򫵳ծ򋲴󘫾.
        𚘑Ǹ6􁏛򸠏/󏒁Ή#󠱩ɀ򿱺e帩񆡯{糰모lׂ񿻐췆ѧ{᎞Ȥ�Gżߑ𘘢䛟老^򇦔MՒ遽>丼iҸi󱑊𳇈ܞe阁𒉷cҤ𛂷𻒳􏁹!𻄹񴷦򉖚ث表𢤉󘢰̶ҿ󋤛ɫ􎕫ӎՁب𚷆򧴞ߕf񩩛T󄶹󤬉f놯#󢤿ӂË澛Ѝ񴶇撠ֲ񆳙뭬ċṝċʌ]")]
        public async Task RespondsWithCorrectStreamGetExport(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                    ExportState = ExportOptions.Done,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var minioMock = new Mock<IMinioClient>();
            UTF8Encoding utf8 = new UTF8Encoding();

            var bytes = new Task<byte[]>(() =>
            {
                return utf8.GetBytes(input);
            });
            bytes.Start();

            minioMock.Setup(m => m.GetBytesAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(bytes)
                .Verifiable("Export did not call GetBytesAsync.");

            var client = CustomHost.Create(sessionMock: sessionMock, objectStoreMock: minioMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX/export");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(body == input, "Export responce is not the given stream.");
        }

        [Fact]
        public async Task NullStreamIsErrorGetExport()
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                    ExportState = ExportOptions.Done,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var minioMock = new Mock<IMinioClient>();
            UTF8Encoding utf8 = new UTF8Encoding();

            var bytes = new Task<byte[]>(() =>
            {
                return null;
            });
            bytes.Start();

            minioMock.Setup(m => m.GetBytesAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(bytes)
                .Verifiable("Export did not call GetBytesAsync.");

            var client = CustomHost.Create(sessionMock: sessionMock, objectStoreMock: minioMock);

            var request = CustomRequest.Create(HttpMethod.Get, "/sessions/EXEX/export");

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await client.SendAsync(request);
            });
        }

        [Theory]
        [InlineData("")]
        [InlineData("?")]
        [InlineData("/")]
        [InlineData("/?=test=sdkfjsdlfksdf&blak=sdfsfds.")]
        public async Task MultiplePathsAcceptedGetExport(string input)
        {
            var sessionMock = new Mock<IRepository<SessionSchema>>();

            var result = new Task<SessionSchema>(() =>
            {
                return new SessionSchema
                {
                    InternalId = null,
                    Id = null,
                    User = null,
                    CreatedAt = null,
                    Tags = null,
                    ExportState = ExportOptions.Done,
                };
            });
            result.Start();

            sessionMock.Setup(a => a.FindById(It.IsAny<string>()))
                .Returns(result)
                .Verifiable("A session was never searched for.");

            var client = CustomHost.Create(sessionMock: sessionMock);

            var request = CustomRequest.Create(HttpMethod.Get, $"/sessions/EXEX/export{input}");
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("?")]
        [InlineData("/?")]
        [InlineData("/?test=sdkfjsdlfksdf&blak=sdfsfds")]
        public async Task OtherMethodsExport(string input)
        {
            var client = CustomHost.Create();

            var requestPut = CustomRequest.Create(HttpMethod.Put, $"/sessions/EXEX/export{input}");
            var responsePut = await client.SendAsync(requestPut);

            Assert.True(
                responsePut.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Putting export is an allowed method.");

            var requestPatch = CustomRequest.Create(HttpMethod.Patch, $"/sessions/EXEX/export{input}");
            var responsePatch = await client.SendAsync(requestPatch);

            Assert.True(
                responsePatch.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Patching export is an allowed method.");

            var requestDelete = CustomRequest.Create(HttpMethod.Patch, $"/sessions/EXEX/export{input}");
            var responseDelete = await client.SendAsync(requestDelete);

            Assert.True(
                responseDelete.StatusCode == HttpStatusCode.MethodNotAllowed,
                "Deleting export is an allowed method.");
        }
    }
}