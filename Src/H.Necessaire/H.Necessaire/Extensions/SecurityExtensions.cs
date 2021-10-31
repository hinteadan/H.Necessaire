using System;
using System.IO;
using System.Security.Cryptography;

namespace H.Necessaire
{
    public static class SecurityExtensions
    {
        /// <summary>
        /// https://www.ssl.com/guide/pem-der-crt-and-cer-x-509-encodings-and-conversions/
        /// 
        /// PEM = Privacy Enhanced Mail (text containing one or more items in Base64 ASCII encoding)
        /// DER = Distinguished Encoding Rules (binary)
        /// CER = Canonical Encoding Rules (ASCII)
        /// </summary>
        /// <param name="cryptoServiceProvider"></param>
        /// <returns>OpertionResult with export result as string payload</returns>
        public static OperationResult<string> ExportPrivateRsaKeyAsPEM(this RSACryptoServiceProvider cryptoServiceProvider)
        {
            if (cryptoServiceProvider.PublicOnly)
                return OperationResult.Fail("The given crypto service provider doesn't contain a private key").WithoutPayload<string>();

            OperationResult<string> result = OperationResult.Fail().WithoutPayload<string>();

            new Action(() =>
            {
                StringWriter outputStream = new StringWriter();
                if (cryptoServiceProvider.PublicOnly) throw new ArgumentException("CSP does not contain a private key", "csp");
                var parameters = cryptoServiceProvider.ExportParameters(true);
                using (var stream = new MemoryStream())
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write((byte)0x30); // SEQUENCE
                    using (var innerStream = new MemoryStream())
                    {
                        var innerWriter = new BinaryWriter(innerStream);
                        EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                        EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                        EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                        EncodeIntegerBigEndian(innerWriter, parameters.D);
                        EncodeIntegerBigEndian(innerWriter, parameters.P);
                        EncodeIntegerBigEndian(innerWriter, parameters.Q);
                        EncodeIntegerBigEndian(innerWriter, parameters.DP);
                        EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                        EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                        var length = (int)innerStream.Length;
                        EncodeLength(writer, length);
                        writer.Write(innerStream.GetBuffer(), 0, length);
                    }

                    var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                    // WriteLine terminates with \r\n, we want only \n
                    outputStream.Write("-----BEGIN RSA PRIVATE KEY-----\n");
                    // Output as Base64 with lines chopped at 64 characters
                    for (var i = 0; i < base64.Length; i += 64)
                    {
                        outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                        outputStream.Write("\n");
                    }
                    outputStream.Write("-----END RSA PRIVATE KEY-----");
                }

                result = OperationResult.Win().WithPayload(outputStream.ToString());

            })
            .TryOrFailWithGrace(
                onFail: x => result = OperationResult.Fail(x).WithoutPayload<string>()
            );

            return result;
        }

        /// <summary>
        /// https://www.ssl.com/guide/pem-der-crt-and-cer-x-509-encodings-and-conversions/
        /// 
        /// PEM = Privacy Enhanced Mail (text containing one or more items in Base64 ASCII encoding)
        /// DER = Distinguished Encoding Rules (binary)
        /// CER = Canonical Encoding Rules (ASCII)
        /// </summary>
        /// <param name="cryptoServiceProvider"></param>
        /// <returns>OpertionResult with export result as string payload</returns>
        public static OperationResult<string> ExportPublicRsaKeyAsPEM(this RSACryptoServiceProvider cryptoServiceProvider)
        {
            OperationResult<string> result = OperationResult.Fail().WithoutPayload<string>();

            new Action(() =>
            {
                StringWriter outputStream = new StringWriter();
                var parameters = cryptoServiceProvider.ExportParameters(false);
                using (var stream = new MemoryStream())
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write((byte)0x30); // SEQUENCE
                    using (var innerStream = new MemoryStream())
                    {
                        var innerWriter = new BinaryWriter(innerStream);
                        innerWriter.Write((byte)0x30); // SEQUENCE
                        EncodeLength(innerWriter, 13);
                        innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
                        var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                        EncodeLength(innerWriter, rsaEncryptionOid.Length);
                        innerWriter.Write(rsaEncryptionOid);
                        innerWriter.Write((byte)0x05); // NULL
                        EncodeLength(innerWriter, 0);
                        innerWriter.Write((byte)0x03); // BIT STRING
                        using (var bitStringStream = new MemoryStream())
                        {
                            var bitStringWriter = new BinaryWriter(bitStringStream);
                            bitStringWriter.Write((byte)0x00); // # of unused bits
                            bitStringWriter.Write((byte)0x30); // SEQUENCE
                            using (var paramsStream = new MemoryStream())
                            {
                                var paramsWriter = new BinaryWriter(paramsStream);
                                EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
                                EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
                                var paramsLength = (int)paramsStream.Length;
                                EncodeLength(bitStringWriter, paramsLength);
                                bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                            }
                            var bitStringLength = (int)bitStringStream.Length;
                            EncodeLength(innerWriter, bitStringLength);
                            innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
                        }
                        var length = (int)innerStream.Length;
                        EncodeLength(writer, length);
                        writer.Write(innerStream.GetBuffer(), 0, length);
                    }

                    var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                    // WriteLine terminates with \r\n, we want only \n
                    outputStream.Write("-----BEGIN PUBLIC KEY-----\n");
                    for (var i = 0; i < base64.Length; i += 64)
                    {
                        outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                        outputStream.Write("\n");
                    }
                    outputStream.Write("-----END PUBLIC KEY-----");
                }

                result = OperationResult.Win().WithPayload(outputStream.ToString());
            })
            .TryOrFailWithGrace(
                onFail: x => result = OperationResult.Fail(x).WithoutPayload<string>()
            );

            return result;
        }

        public static OperationResult<RSACryptoServiceProvider> ImportAsPEMPrivateRsaKey(this string pem)
        {
            OperationResult<RSACryptoServiceProvider> result = OperationResult.Fail().WithoutPayload<RSACryptoServiceProvider>();

            new Action(() =>
            {
                using (StringReader pemStringReader = new StringReader(pem))
                {
                    Org.BouncyCastle.OpenSsl.PemReader pemReader = new Org.BouncyCastle.OpenSsl.PemReader(new StringReader(pem));

                    Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair KeyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)pemReader.ReadObject();
                    RSAParameters rsaParams = Org.BouncyCastle.Security.DotNetUtilities.ToRSAParameters((Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)KeyPair.Private);

                    RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider();// cspParams);
                    cryptoServiceProvider.ImportParameters(rsaParams);

                    result = OperationResult.Win().WithPayload(cryptoServiceProvider);
                }
            })
            .TryOrFailWithGrace(
                onFail: x => result = OperationResult.Fail(x).WithoutPayload<RSACryptoServiceProvider>()
            );

            return result;
        }

        public static OperationResult<RSACryptoServiceProvider> ImportAsPEMPublicRsaKey(this string pem)
        {
            OperationResult<RSACryptoServiceProvider> result = OperationResult.Fail().WithoutPayload<RSACryptoServiceProvider>();

            new Action(() =>
            {
                using (StringReader pemStringReader = new StringReader(pem))
                {
                    Org.BouncyCastle.OpenSsl.PemReader pr = new Org.BouncyCastle.OpenSsl.PemReader(pemStringReader);
                    Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)pr.ReadObject();
                    RSAParameters rsaParams = Org.BouncyCastle.Security.DotNetUtilities.ToRSAParameters((Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)publicKey);

                    RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider();// cspParams);
                    cryptoServiceProvider.ImportParameters(rsaParams);

                    result = OperationResult.Win().WithPayload(cryptoServiceProvider);
                }
            })
            .TryOrFailWithGrace(
                onFail: x => result = OperationResult.Fail(x).WithoutPayload<RSACryptoServiceProvider>()
            );

            return result;
        }

        static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }

        static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length), "Length must be >=0 ");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }
    }
}
