using MobileTicket.Enteties;
using PeterO.Cbor;
using System;
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

            // TEMP
            var inputHex = BitConverter.ToString(signingInput);
            var signHex = Convert.ToBase64String(signature);
            var payloadBytes = Convert.ToBase64String(signingInput);

            var bundleHex = Convert.ToBase64String(mtb.IssuerSignedTicketBundle.GetBytes());
            // /TEMP

            var publicKey = _keyRepository.GetPublicKey(header.alg, header.iid, header.kid);

            if (publicKey != null)
            {
                if (signature.Length > 64)
                {
                    signature = SignatureConverter.FromDerEncoded(signature);
                }

                using (var dsa = new ECDsaCng(publicKey))
                {
                    // TEMP
                    var i1 = Convert.ToBase64String(signingInput);
                    var i2 = Convert.ToBase64String(signature);
                    var i3 = Convert.ToBase64String(publicKey.Export(CngKeyBlobFormat.EccPublicBlob));
                    // /TEMP
                    var resultat =  dsa.VerifyData(signingInput, signature);
                    return resultat;
                }
            }
            else
                return false;
        }   

        public MtbContainer Sign(MtbContainer mtb)
        {
            // Create header
            mtb.IssuerSignedTicketBundle.Header = new IssuerSignatureHeader(SignatureAlgorithm.ES256, _keyRepository.SigningIssuerId, _keyRepository.SigningKeyId);

            //var signingInput = CreateCoseSigningInput(mtb.IssuerSignedTicketBundle.Header.GetBytes(), mtb.IssuerSignedTicketBundle.TicketBundle.GetBytes());
            var signingInput = mtb.IssuerSignedTicketBundle.signingInput;
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
