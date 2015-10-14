using MobileTicket.Services;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace MobileTicket.Tests
{
    public class DemoKeyRepository : IKeyRepository
    {
        private byte[] BCRYPT_ECDSA_PUBLIC_P256_MAGIC = BitConverter.GetBytes(0x31534345);
        private string _signingIid = "15";
        private string _signingKid = "1";

        public CngKey GetPrivateKey()
        {
            var key = Convert.FromBase64String("RUNTMiAAAABi2hoKdnhChdAV18DSh95VlgsedNhSjRD+ZXxU84jeTRqtaPUgECdVaQlXpSzgvk7qCR9XRVl4kF+68Jo86IpcjdJh9ORbOYCDKRpqNbQIDK5gOva7SrXAt4wDs7YN/OQ=");
            return CngKey.Import(key, CngKeyBlobFormat.EccPrivateBlob);
        }

        public CngKey GetPublicKey(SignatureAlgorithm alg, string iid, string kid)
        {
            byte[] x, y;

            if (iid == "1" && kid == "18")
            {
                x = Convert.FromBase64String("UabuaxndEdYE5Nr4fC5ETFcYe7YgyFn4fWqe1R/GB9w=");
                y = Convert.FromBase64String("k/NCBB5W8ZuBqvxZ+GHkI3fS1QflEezF9EriGUVpsAk=");
                return CreatePublicKey(x, y);
            }
            if (iid == _signingIid && kid == _signingKid)
            {
                x = Convert.FromBase64String("YtoaCnZ4QoXQFdfA0ofeVZYLHnTYUo0Q/mV8VPOI3k0=");
                y = Convert.FromBase64String("Gq1o9SAQJ1VpCVelLOC+TuoJH1dFWXiQX7rwmjzoilw=");
                return CreatePublicKey(x, y);
            }

            return null;
        }

        public string SigningIssuerId { get { return _signingIid; } }
        public string SigningKeyId { get { return _signingKid; } }

        private CngKey CreatePublicKey(byte[] x, byte[] y)
        {
            if (x.Length != y.Length)
                throw new ArgumentException("X and Y must have same length");

            var partSize = x.Length;
            var partLength = BitConverter.GetBytes(partSize);
            var blob = BCRYPT_ECDSA_PUBLIC_P256_MAGIC.Concat(partLength).Concat(x).Concat(y).ToArray();
            var blobType = CngKeyBlobFormat.EccPublicBlob;

            return CngKey.Import(blob, blobType);
        }

        // Just for convenience.
        public static string[] ExportKey(CngKey key)
        {
            byte[] blob = key.Export(CngKeyBlobFormat.EccPublicBlob);

            if (blob.Length != 72)
                throw new Exception($"Unexpected key length: {blob.Length}, expected 72");

            var data = BitConverter.ToString(blob);

            var x = blob.Skip(8).Take(32).ToArray();
            var y = blob.Skip(40).Take(32).ToArray();

            var xHex = Convert.ToBase64String(x);
            var yHex = Convert.ToBase64String(y);

            var keyParts = new string[] { xHex, yHex };


            return keyParts;

        }



    }
}
