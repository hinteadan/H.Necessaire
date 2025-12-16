using System.Threading.Tasks;

namespace H.Necessaire.Operations.Concrete
{
    internal class TotpHandler : ImATotpHandler, ImADependency
    {
        ImACryptographer cryptographer;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            cryptographer = dependencyProvider.Build<ImACryptographer>("CaesarCipher");
        }

        public async Task<OperationResult<string>> Safeguard(TotpToken token)
        {
            if (token is null)
                return OperationResult.Fail("Token is null").WithoutPayload<string>();

            return await HSafe.Run(async () =>
            {

                string serializedToken = token.ToBase64String();

                string encryptedToken = await cryptographer.Encrypt(serializedToken);

                return encryptedToken;

            }, "Encrypt TOTP Token");
        }

        public async Task<OperationResult<TotpToken>> Validate(string safeguardedToken)
        {
            if (safeguardedToken.IsEmpty())
                return "Safeguarded Token value is empty";

            (await HSafe.Run(async () =>
            {

                string decryptedSerializedToken = await cryptographer.Decrypt(safeguardedToken);

                return TotpToken.FromBase64String(decryptedSerializedToken);

            }, "Decrypt TOTP Token")).Ref(out var globalRes, out var innerRes);

            if (!globalRes)
                return globalRes.WithoutPayload<TotpToken>();

            if (!innerRes)
                return innerRes;

            TotpToken token = innerRes.Payload;

            if (token.IsExpired())
                return OperationResult.Fail("Token has expired").WithPayload(token);

            return token;
        }
    }
}
