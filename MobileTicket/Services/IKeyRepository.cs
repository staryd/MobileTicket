using System.Security.Cryptography;

namespace MobileTicket.Services
{
    public interface IKeyRepository
    {
        CngKey GetPublicKey(SignatureAlgorithm alg, string iid, string kid);
        CngKey GetPrivateKey();
        string SigningIssuerId { get; }
        string SigningKeyId { get; }
    }
}
