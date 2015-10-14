using MobileTicket.Extensions;
using PeterO.Cbor;
using System;
using System.Collections.Generic;

namespace MobileTicket.Enteties
{
    public class TicketBundle
    {
        public TicketBundle()
        {
            ParticipantObjects = new Dictionary<string, ParticipantObject>();
        }

        public TicketBundle(byte[] bytes)
        {
            Parse(bytes);
        }

        public Dictionary<string, ParticipantObject> ParticipantObjects { get; set; }

        public byte[] GetBytes()
        {
            var cborMap = CBORObject.NewMap();

            foreach (var participantObject in ParticipantObjects)
            {
                cborMap.Add(participantObject.Key, participantObject.Value.ToCbor());
            }

            return cborMap.EncodeToBytes();
        }


        private void Parse(byte[] bytes)
        {
            var temp = Convert.ToBase64String(bytes);

            CBORObject cborObject;
            try
            {
                cborObject = CBORObject.DecodeFromBytes(bytes);
            }
            catch
            {
                throw new ArgumentException("Could not parse TicketBundle. Byte array could not be decoded to CBOR Object");
            }

            var dict = cborObject.ConvertToDictionary();

            ParticipantObjects = new Dictionary<string, ParticipantObject>();

            foreach (var participant in dict)
            {
                if (participant.Value != null)
                    ParticipantObjects.Add(participant.Key, new ParticipantObject(participant.Value));
            }
        }
    }
}
