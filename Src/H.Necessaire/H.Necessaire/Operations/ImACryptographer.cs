using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImACryptographer
    {
        Task<string> Encrypt(string value);
        Task<string> Decrypt(string encryptedValue);
    }
}
