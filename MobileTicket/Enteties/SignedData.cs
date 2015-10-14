using MobileTicket.Extensions;
using PeterO.Cbor;
using System.Linq;

namespace MobileTicket.Enteties
{
    public class SignedData
    {
        public SignedData(byte[] bytes)
        {
            Parse(bytes);
        }

        public byte[] Payload { get { return payload.GetByteString(); } }
        private CBORObject header { get; set; }
        private CBORObject payload { get; set; }
        private CBORObject signature { get; set; }


        public SignatureType SignatureType
        {
            get
            {
                var h = CBORObject.DecodeFromBytes(header.GetByteString());

                if (h.ToString().Contains("\"did\""))
                    return SignatureType.Device;
                else
                    return SignatureType.Issuer;
            }
        }

        private void Parse(byte[] bytes)
        {
            var cborObject = CBORObject.DecodeFromBytes(bytes);
            var list = cborObject.ConvertToList();

            if (list.Count == 3)
            {
                header = list.ElementAt(0);
                payload = list.ElementAt(1);
                signature = list.ElementAt(2);
            }
        }
    }
}