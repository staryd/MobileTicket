using MobileTicket.Enteties;
using MobileTicket.Extensions;
using NUnit.Framework;

namespace MobileTicket.Tests
{
    [TestFixture]
    class MtbContainerExtensionTests
    {
        [Test]
        public void GetParticipantObjectAsJson_Returns_Json()
        {
            var participant1Object = "[{\"s_tid\":\"123\"}]";
            var participant2Object = "[{\"s_tid\":\"987\"}]";

            var ticketBundle = new TicketBundle();
            ticketBundle.ParticipantObjects.Add("1", new ParticipantObject(participant1Object));
            ticketBundle.ParticipantObjects.Add("2", new ParticipantObject(participant2Object));

            var signedBundle = new IssuerSignedTicketBundle(ticketBundle);
            var mtb = new MtbContainer(signedBundle);

            var result = mtb.GetParticipantObjectAsJson("2");

            Assert.That(result, Is.EqualTo(participant2Object));
        }
    }
}
