using MobileTicket.Enteties;
using MobileTicket.Services;

namespace MobileTicket
{
    public class MtbHandler
    {
        private IKeyRepository _keyRepository;
        private SignatureService _service;

        public MtbHandler(IKeyRepository keyRepository)
        {
            _keyRepository = keyRepository;
            _service = new SignatureService(_keyRepository);
        }


        /// <summary>
        /// Parses a byte array and returns a MtbContainer.
        /// Issuer signature is not verified. Use <see cref="Verify(MtbContainer)"/> to verify.
        /// </summary>
        /// <param name="data">Raw MTB byte[]</param>
        /// <returns>Filled MtbContainer</returns>
        public MtbContainer Parse(byte[] data)
        {
            var mtbContainer = new MtbContainer(data);
            return mtbContainer;
        }


        /// <summary>
        /// Verifies a Issuer signature for a MTB
        /// </summary>
        /// <param name="mtbContainer">MTB Conatiner to verify</param>
        /// <returns>True if signature is verified.</returns>
        public bool Verify(MtbContainer mtbContainer)
        {
            return _service.Verify(mtbContainer);
        }


        /// <summary>
        /// Creates a signed MTB with one participant object.
        /// Participant object is expected to be serialized to JSON-array;
        /// [{participantTicketObject-1}, {participantTicketObject-2}, ... ]
        /// </summary>
        /// <param name="jsonString">Participant object serialized to JSON</param>
        /// <param name="participantId">Participant id (PID) for Ticket Bundle map</param>
        /// <returns>Signed MTB as byte[]</returns>
        public byte[] CreateSigned(string jsonString, string participantId)
        {
            var participantObject = new ParticipantObject(jsonString);
            var ticketBundle = new TicketBundle();
            ticketBundle.ParticipantObjects.Add(participantId, participantObject);
            var unSignedBundle = new IssuerSignedTicketBundle(ticketBundle);
            var mtbContainer = new MtbContainer(unSignedBundle);

            // Sign
            var signedMtb = _service.Sign(mtbContainer);

            return signedMtb.GetBytes();
        }
    }
}
