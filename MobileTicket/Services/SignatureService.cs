using MobileTicket.Enteties;
using PeterO.Cbor;
using System.Security.Cryptography;

namespace MobileTicket.Services
{
    public class SignatureService
    {
        private IKeyRepository _keyRepository;
        public SignatureService(IKeyRepository keyRepository)
        {
            _keyRepository = keyRepository;
        }

        public bool Verify(MtbContainer mtb)
        {
            var header = mtb.IssuerSignedTicketBundle.Header;
            var signingInput = CreateCoseSigningInput(mtb.IssuerSignedTicketBundle.Header.GetBytes(), mtb.IssuerSignedTicketBundle.TicketBundle.GetBytes());
            var signature = mtb.IssuerSignedTicketBundle.Signature;
            var publicKey = _keyRepository.GetPublicKey(header.alg, header.iid, header.kid);

            if (publicKey != null)
            {
                if (signature.Length > 64)
                {
                    signature = SignatureConverter.FromDerEncoded(signature);
                }

                using (var dsa = new ECDsaCng(publicKey))
                {
                    return dsa.VerifyData(signingInput, signature);
                }
            }

            return false;
        }   

        public MtbContainer Sign(MtbContainer mtb)
        {
            mtb.IssuerSignedTicketBundle.Header = new IssuerSignatureHeader(SignatureAlgorithm.ES256, _keyRepository.SigningIssuerId, _keyRepository.SigningKeyId);
            var signingInput = CreateCoseSigningInput(mtb.IssuerSignedTicketBundle.Header.GetBytes(), mtb.IssuerSignedTicketBundle.TicketBundle.GetBytes());
            var privateKey = _keyRepository.GetPrivateKey();
            using (var dsa = new ECDsaCng(privateKey))
            {
                mtb.IssuerSignedTicketBundle.Signature = dsa.SignData(signingInput);
            }

            return mtb;
        }

        public byte[] CreateCoseSigningInput(byte[] header, byte[] payload)
        {
            var cborArray = CBORObject.NewArray();
            cborArray.Add(header);
            cborArray.Add(payload);
            return cborArray.EncodeToBytes();
        }
    }
}
