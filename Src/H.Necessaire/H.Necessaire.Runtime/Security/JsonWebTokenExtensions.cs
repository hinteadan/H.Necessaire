using H.Necessaire.Serialization;
using NeoSmart.Utils;
using System;
using System.Text;

namespace H.Necessaire
{
    public static class JsonWebTokenExtensions
    {
        const string concatenator = ".";

        public static string ToStringUnsigned<TPayload>(this JsonWebToken<TPayload> jwt) where TPayload : JwtPayload
        {
            StringBuilder printer = new StringBuilder();

            printer.Append(UrlBase64.Encode(Encoding.UTF8.GetBytes(jwt.Header.ToJsonObject())));
            printer.Append(concatenator);
            printer.Append(UrlBase64.Encode(Encoding.UTF8.GetBytes(jwt.Payload.ToJsonObject())));

            return printer.ToString();
        }

        public static string ToStringSigned<TPayload>(this JsonWebToken<TPayload> jwt) where TPayload : JwtPayload
        {
            StringBuilder printer = new StringBuilder();

            printer.Append(jwt.ToStringUnsigned());
            printer.Append(concatenator);
            printer.Append(UrlBase64.Encode(Encoding.UTF8.GetBytes(jwt.Signature)));

            return printer.ToString();
        }

        public static OperationResult ValidateTiming<TPayload>(this JsonWebToken<TPayload> jwt) where TPayload : JwtPayload
        {
            if (jwt.Payload == null)
                return OperationResult.Win();

            DateTime now = DateTime.UtcNow;

            if (jwt.Payload.ValidFrom != null && now < jwt.Payload.ValidFrom.Value)
                return OperationResult.Fail($"The token is not yet valid. It'll be valid starting on {jwt.Payload.ValidFrom} UTC");

            if (jwt.Payload.ValidUntil != null && now > jwt.Payload.ValidUntil.Value)
                return OperationResult.Fail($"The token has expired. It was valid until {jwt.Payload.ValidUntil} UTC");

            return OperationResult.Win();
        }

        public static OperationResult<JsonWebToken<TPayload>> TryParseJsonWebToken<TPayload>(this string tokenAsString) where TPayload : JwtPayload
        {
            if (string.IsNullOrWhiteSpace(tokenAsString))
                return OperationResult.Fail("The given token is empty").WithoutPayload<JsonWebToken<TPayload>>();

            OperationResult<JsonWebToken<TPayload>> result = null;

            new Action(() =>
            {
                string[] parts = tokenAsString.Split(concatenator.AsArray(), count: 3, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3)
                {
                    result = OperationResult.Fail($"The given token is invalid. Token: {tokenAsString}").WithoutPayload<JsonWebToken<TPayload>>();
                    return;
                }
                result =
                    OperationResult.Win().WithPayload(new JsonWebToken<TPayload>
                    {
                        Header = Encoding.UTF8.GetString(UrlBase64.Decode(parts[0])).TryJsonToObject<JwtHeader>().ThrowOnFailOrReturn(),
                        Payload = Encoding.UTF8.GetString(UrlBase64.Decode(parts[1])).TryJsonToObject<TPayload>().ThrowOnFailOrReturn(),
                        Signature = Encoding.UTF8.GetString(UrlBase64.Decode(parts[2])),
                    });
            })
            .TryOrFailWithGrace(
                onFail: ex => result = OperationResult.Fail(ex, $"The given token cannot be parsed. Token: {tokenAsString}").WithoutPayload<JsonWebToken<TPayload>>()
            );

            return result;
        }
    }
}
