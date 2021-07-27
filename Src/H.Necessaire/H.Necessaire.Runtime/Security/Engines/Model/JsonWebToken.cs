using H.Necessaire.Serialization;
using NeoSmart.Utils;
using System;
using System.Text;

namespace H.Necessaire.Runtime.Security.Engines.Model
{
    class JsonWebToken<TPayload> where TPayload : JwtPayload
    {
        const string concatenator = ".";

        public JwtHeader Header { get; set; } = JwtHeader.Default;
        public TPayload Payload { get; set; }
        public string Signature { get; set; }

        public string ToStringUnsigned()
        {
            StringBuilder printer = new StringBuilder();

            printer.Append(UrlBase64.Encode(Encoding.UTF8.GetBytes(Header.ToJsonObject())));
            printer.Append(concatenator);
            printer.Append(UrlBase64.Encode(Encoding.UTF8.GetBytes(Payload.ToJsonObject())));

            return printer.ToString();
        }

        public override string ToString()
        {
            StringBuilder printer = new StringBuilder();

            printer.Append(ToStringUnsigned());
            printer.Append(concatenator);
            printer.Append(UrlBase64.Encode(Encoding.UTF8.GetBytes(Signature)));

            return printer.ToString();
        }

        public OperationResult ValidateTiming()
        {
            if (Payload == null)
                return OperationResult.Win();

            DateTime now = DateTime.UtcNow;

            if (Payload.ValidFrom != null && now < Payload.ValidFrom.Value)
                return OperationResult.Fail($"The token is not yet valid. It'll be valid starting on {Payload.ValidFrom} UTC");

            if (Payload.ValidUntil != null && now > Payload.ValidUntil.Value)
                return OperationResult.Fail($"The token has expired. It was valid until {Payload.ValidUntil} UTC");

            return OperationResult.Win();
        }

        public static OperationResult<JsonWebToken<TPayload>> TryParse(string tokenAsString)
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
