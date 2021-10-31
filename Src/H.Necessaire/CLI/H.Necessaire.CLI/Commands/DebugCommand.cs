using H.Necessaire.Runtime.CLI.Commands;
using H.Necessaire.Serialization;
using NeoSmart.Utils;
using System.Text;

namespace H.Necessaire.CLI.Commands
{
    public class DebugCommand : CommandBase
    {
        #region Construct
        RS512Hasher hasher = new RS512Hasher();
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            hasher = dependencyProvider.Get<RS512Hasher>();
        }
        #endregion

        public override async Task<OperationResult> Run()
        {
            await DebugJwt();

            return OperationResult.Win();
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
    }
}
