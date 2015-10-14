using MobileTicket.Enteties;

namespace MobileTicket.Extensions
{
    public static class MtbContainerExtensions
    {
        public static string GetParticipantObjectAsJson(this MtbContainer mtbContainer, string participantId)
        {
            var participantObjects = mtbContainer.IssuerSignedTicketBundle?.TicketBundle?.ParticipantObjects;
            if (participantObjects != null && participantObjects.ContainsKey(participantId))
                return participantObjects[participantId].ToString();
            else
                return string.Empty;
        }
    }
}
