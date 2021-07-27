using System;

namespace H.Necessaire
{
    public class SecuredHash
    {
        const string concatenator = "$$$";

        public string Hash { get; set; }
        public string Key { get; set; }
        public override string ToString()
        {
            return $"{Key}{concatenator}{Hash}";
        }
        public static OperationResult<SecuredHash> TryParse(string securedHash)
        {
            if (string.IsNullOrWhiteSpace(securedHash))
                return OperationResult.Fail($"The given hash is empty").WithoutPayload<SecuredHash>();

            OperationResult<SecuredHash> result = null;

            new
                Action(() =>
                {
                    string[] parts = securedHash.Split(new string[] { concatenator }, count: 2, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 1)
                    {
                        result = OperationResult.Win().WithPayload(new SecuredHash { Hash = parts[0] });
                        return;
                    }

                    result = OperationResult.Win().WithPayload(new SecuredHash { Hash = parts[1], Key = parts[0] });
                })
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex, $"The given hash is invalid and cannot be parsed. Hash: {securedHash ?? "NULL"}").WithoutPayload<SecuredHash>()
                );

            return result;
        }
    }
}
