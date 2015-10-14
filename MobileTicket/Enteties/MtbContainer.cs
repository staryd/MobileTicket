using MobileTicket.Extensions;
using PeterO.Cbor;
using System;
using System.Linq;
using System.Text;

namespace MobileTicket.Enteties
{
    public class MtbContainer
    {
        private byte[] mtbVersion = new byte[1] { 1 };

        public MtbContainer(byte[] bytes)
        {
            Parse(bytes);
        }

        public MtbContainer(IssuerSignedTicketBundle issuerSignedTicketBundle)
        {
            IssuerSignedTicketBundle = issuerSignedTicketBundle;
        }

        public IssuerSignedTicketBundle IssuerSignedTicketBundle { get; set; }

        public byte[] GetBytes()
        {
            if (IssuerSignedTicketBundle == null)
                throw new Exception("MtbContainer contains no IssuerSignedTicketBundle");

            var cborMap = CBORObject.NewMap();
            cborMap.Add(Encoding.UTF8.GetBytes("v"), mtbVersion); // Should be number according to spec, but is bytestring?
            cborMap.Add(Encoding.UTF8.GetBytes("p"), IssuerSignedTicketBundle.GetBytes());

            return cborMap.EncodeToBytes();
        }

        private void Parse(byte[] bytes)
        {
            CBORObject cborObject;
            try
            {
                cborObject = CBORObject.DecodeFromBytes(bytes);
            }
            catch
            {
                throw new ArgumentException("Could not parse MTB. Byte array could not be decoded to CBOR Object");
            }

            if (cborObject.Type == CBORType.Map)
            {
                var dict = cborObject.ConvertToDictionary();

                if (dict["v"] == null || !dict["v"].GetByteString().SequenceEqual(mtbVersion))
                    throw new ArgumentException("Could not parse MTB. Only container version 1 is supported.");

                var p = dict["p"].GetByteString();

                // TODO: Refactor
                var signedData = new SignedData(p);

                if (signedData.SignatureType == SignatureType.Issuer)
                    IssuerSignedTicketBundle = new IssuerSignedTicketBundle(p);
                else
                    IssuerSignedTicketBundle = new IssuerSignedTicketBundle(signedData.Payload);
            }
            else
            {
                throw new ArgumentException("Could not parse MtbContainer. Decoded bytes not of expected type CBOR Map");
            }
        }
    }
}
