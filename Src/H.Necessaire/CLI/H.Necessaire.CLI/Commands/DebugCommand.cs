using H.Necessaire.Runtime.CLI.Commands;
using H.Necessaire.Serialization;
using NeoSmart.Utils;
using System.Diagnostics;
using System.Text;

namespace H.Necessaire.CLI.Commands
{
    public class DebugCommand : CommandBase
    {
        #region Construct
        RS512Hasher hasher = new RS512Hasher();
        ImAStorageService<string, ExiledSyncRequest> exiledSyncRequestStorageService = null;
        IKeyValueStorage keyValueStorage = null;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            hasher = dependencyProvider.Get<RS512Hasher>();
            exiledSyncRequestStorageService = dependencyProvider.Get<ImAStorageService<string, ExiledSyncRequest>>();
            keyValueStorage = dependencyProvider.Get<IKeyValueStorage>();
        }
        #endregion

        public override async Task<OperationResult> Run()
        {
            //await DebugJwt();

            //await DebugSql();

            await DebugExecutionCallContext();

            //await DebugConsoleStuff();

            return OperationResult.Win();
        }

        private Task DebugConsoleStuff()
        {
            ConsoleColor[] allConsoleColors = Enum.GetValues(typeof(ConsoleColor)).Cast<ConsoleColor>().ToArray();

            foreach (ConsoleColor color in allConsoleColors)
            {
                using (new ScopedRunner(() => Console.ForegroundColor = color.And(x => { if (x == ConsoleColor.Black) Console.BackgroundColor = ConsoleColor.DarkGray; }), () => Console.ResetColor()))
                {
                    Console.WriteLine(color.ToString());
                }
            }

            return true.AsTask();
        }

        private async Task DebugExecutionCallContext()
        {
            Console.WriteLine(MethodName.GetCurrentName());

            StackTrace stackTrace = new StackTrace();

            Console.WriteLine(stackTrace.ToString());

            string stack = Environment.StackTrace;
            Console.WriteLine(stack?.Substring(stack.IndexOf(Environment.NewLine) + Environment.NewLine.Length));

            CallContext.SetData("GlobalID", "GLOBAL");

            await Task.Run(async () =>
            {
                Console.WriteLine($"Thread [{Thread.CurrentThread.ManagedThreadId}] GlobalID: {CallContext.GetData("GlobalID") ?? "null"}");
                CallContext.SetData("ChildID", "Thread1");

                await Task.Run(() =>
                {
                    Console.WriteLine($"Thread [{Thread.CurrentThread.ManagedThreadId}] GlobalID: {CallContext.GetData("GlobalID") ?? "null"}");
                    Console.WriteLine($"Thread [{Thread.CurrentThread.ManagedThreadId}] ChildID: {CallContext.GetData("ChildID") ?? "null"}");
                });

            });

            await Task.Run(async () =>
            {
                Console.WriteLine($"Thread [{Thread.CurrentThread.ManagedThreadId}] GlobalID: {CallContext.GetData("GlobalID") ?? "null"}");
                CallContext.SetData("ChildID", "Thread2");

                await Task.Run(() =>
                {
                    Console.WriteLine($"Thread [{Thread.CurrentThread.ManagedThreadId}] GlobalID: {CallContext.GetData("GlobalID") ?? "null"}");
                    Console.WriteLine($"Thread [{Thread.CurrentThread.ManagedThreadId}] ChildID: {CallContext.GetData("ChildID") ?? "null"}");
                });

            });

            Console.WriteLine($"Thread [{Thread.CurrentThread.ManagedThreadId}] GlobalID: {CallContext.GetData("GlobalID") ?? "null"}");
            Console.WriteLine($"Thread [{Thread.CurrentThread.ManagedThreadId}] ChildID: {CallContext.GetData("ChildID") ?? "null"}");
        }

        private async Task DebugJwt()
        {
            UserInfo? userInfo = (await GetCurrentContext())?.SecurityContext?.User;

            DateTime createdAt = DateTime.UtcNow;

            JsonWebToken<AccessTokenJwtPayload> jwt = new JsonWebToken<AccessTokenJwtPayload>()
            {
                Payload = new AccessTokenJwtPayload
                {
                    UserID = userInfo?.ID ?? Guid.Empty,
                    ValidUntil = createdAt.AddHours(1),
                    ValidFrom = createdAt,
                    IssuedAt = createdAt,
                    UserInfo = userInfo,
                },
            }
            .And(x => x.Header.Algorithm = "RS512");

            string? jwtAsJson = jwt.ToJsonObject();


            SecuredHash dummyHash = await hasher.Hash(UrlBase64.Encode(Encoding.UTF8.GetBytes("abc"))) ?? new SecuredHash();

            SecuredHash jwtHash = await hasher.Hash(jwt.ToStringUnsigned()) ?? new SecuredHash();

            jwt.Signature = jwtHash.Hash;

            string publicKey = jwtHash.Notes.Get(RS512Hasher.KnownNoteIdPublicKey);
            string privateKey = jwtHash.Notes.Get(RS512Hasher.KnownNoteIdPrivateKey);
            string signature = jwtHash.Hash;

            string jwtString = jwt.ToStringSigned();
        }

        private async Task DebugSql()
        {
            await exiledSyncRequestStorageService.Save(new ExiledSyncRequest { });
            await keyValueStorage.Set("Test", "abc");
        }

        public static class MethodName
        {
            public static string GetCurrentName([System.Runtime.CompilerServices.CallerMemberName] string methodName = null) => string.IsNullOrWhiteSpace(methodName) ? null : methodName;
        }
    }
}
