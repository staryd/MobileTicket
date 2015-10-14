using MobileTicket.Extensions;
using PeterO.Cbor;
using System;
using System.Linq;

namespace MobileTicket.Enteties
{
    public class IssuerSignedTicketBundle
    {
        public IssuerSignedTicketBundle(byte[] bytes)
        {
            Parse(bytes);
        }

        public IssuerSignedTicketBundle(TicketBundle ticketBundle)
        {
            TicketBundle = ticketBundle;
        }

        public IssuerSignatureHeader Header { get; set; }
        public TicketBundle TicketBundle { get; set; }
        public byte[] Signature { get; set; }

        private byte[] Payload { get; set; }

        public byte[] signingInput;

        

        public byte[] GetBytes()
        {
            if (Header == null || TicketBundle == null)
                throw new Exception("IssuerSignedTicketBundle is not complete");


            var cborArray = CBORObject.NewArray();
            cborArray.Add(Header.GetBytes());
            cborArray.Add(TicketBundle.GetBytes());
            cborArray.Add(Signature);

            return cborArray.EncodeToBytes();
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
                throw new ArgumentException("Could not parse IssuerSignedTicketBundle. Byte array could not be decoded to CBOR Object");
            }

            if (cborObject.Type == CBORType.Array)
            {
                var byteStringList = cborObject.ConvertToList();
                if (byteStringList.Count != 3)
                    throw new ArgumentException($"Could not parse IssuerSignedTicketBundle. Unexpected length of array: {byteStringList.Count}, expected 3.");

                // TEMP
                var headerBytes = Convert.ToBase64String(byteStringList.ElementAt(0).GetByteString());
                var payloadBytes = Convert.ToBase64String(byteStringList.ElementAt(1).GetByteString());
                var signatureBytes = BitConverter.ToString(byteStringList.ElementAt(2).GetByteString());


                Header = new IssuerSignatureHeader(byteStringList.ElementAt(0).GetByteString());
                Payload = byteStringList.ElementAt(1).GetByteString();
                TicketBundle = new TicketBundle(Payload);
                Signature = byteStringList.ElementAt(2).GetByteString();

                var signHex = BitConverter.ToString(Signature);


                var cborArray = CBORObject.NewArray();
                cborArray.Add(byteStringList.ElementAt(0));
                cborArray.Add(byteStringList.ElementAt(1));

                signingInput = cborArray.EncodeToBytes();
            }
        }
    }
}
