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

        public IssuerSignatureHeader Header { get; set; }
        public TicketBundle TicketBundle { get; set; }
        public byte[] Signature { get; set; }


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

                Header = new IssuerSignatureHeader(byteStringList.ElementAt(0).GetByteString());
                TicketBundle = new TicketBundle(byteStringList.ElementAt(1).GetByteString());
                Signature = byteStringList.ElementAt(2).GetByteString();
            }
        }
    }
}
