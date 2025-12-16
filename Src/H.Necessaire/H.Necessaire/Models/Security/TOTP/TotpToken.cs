using System;
using System.IO;

namespace H.Necessaire
{
    public class TotpToken : EphemeralTypeBase, IGuidIdentity
    {
        static Random random = new Random();
        static readonly TimeSpan defaultValidity = TimeSpan.FromSeconds(42);

        public TotpToken() => ExpireIn(defaultValidity);

        public Guid ID { get; set; } = Guid.NewGuid();
        public string Owner { get; set; } = "PublicConsumer";
        public string Code { get; set; } = random.Next(0, 999999).ToString("000000");

        public byte[] ToBytes()
        {
            using (MemoryStream resultStream = new MemoryStream())
            using (BinaryWriter binaryWriter = new BinaryWriter(resultStream))
            {
                binaryWriter.Write(ID.ToByteArray());
                binaryWriter.Write(Owner);
                binaryWriter.Write(Code);

                binaryWriter.Write(CreatedAt.Ticks);
                binaryWriter.Write(AsOfTicks);
                binaryWriter.Write(ValidFromTicks);
                binaryWriter.Write(ExpiresAtTicks ?? DateTime.MinValue.Ticks);

                binaryWriter.Flush();

                resultStream.Position = 0;

                return resultStream.ToArray();
            }
        }

        public string ToBase64String() => Convert.ToBase64String(ToBytes());

        public static OperationResult<TotpToken> FromBytes(byte[] bytes)
        {
            return HSafe.Run(() => {
                using (MemoryStream inputStream = new MemoryStream(bytes))
                using (BinaryReader binaryReader = new BinaryReader(inputStream))
                {
                    return new TotpToken { 
                        ID = new Guid(binaryReader.ReadBytes(16)),
                        Owner = binaryReader.ReadString(),
                        Code = binaryReader.ReadString(),
                        CreatedAt = new DateTime(binaryReader.ReadInt64()),
                        AsOf = new DateTime(binaryReader.ReadInt64()),
                        ValidFrom = new DateTime(binaryReader.ReadInt64()),
                        ExpiresAt = new DateTime(binaryReader.ReadInt64()),
                    }
                    .AndIf(x => x.ExpiresAt == DateTime.MinValue, x => x.DoNotExpire())
                    ;
                }
            }, "Load TotpToken from bytes");
        }

        public static OperationResult<TotpToken> FromBase64String(string base64String)
        {
            if (base64String.IsEmpty())
                return "base64String is empty";

            if (!HSafe.Run(() => Convert.FromBase64String(base64String)).Ref(out var bytesRes, out var bytes))
                return bytesRes.WithoutPayload<TotpToken>();

            return FromBytes(bytes);
        }
    }
}
