using MobileTicket.Enteties;
using NUnit.Framework;
using System;
using System.Linq;

namespace MobileTicket.Tests
{
    [TestFixture]
    class TicketBundleTests
    {
        private string ticketBundleB64 = "oWIxMIGqZXNfc3J2gWIxMGVzX3ZwcxpWCbhgZXNfcGF4gqJjbnVtYTFjY2F0YWGiY251bWEyY2NhdGFlZHNfZnKCCmMyNTBlc192ZXJhMWVzX3RpZGQxMDI0ZHNfcnYaAAFRgGVzX3ZwZRpWCwngZHNfdG+CCmMyNDBkc19mYaJmYW1vdW50YjQ4ZGNvZGVjc2Vr";
        [Test]
        public void GetBytes_Returns_SameSequence_As_Parsed_Data()
        {
            var ticketBundleBytes = Convert.FromBase64String(ticketBundleB64);
            var ticketBundle = new TicketBundle(ticketBundleBytes);

            var result = ticketBundle.GetBytes();

            Assert.That(result.SequenceEqual(ticketBundleBytes), Is.True);
        } 
    }
}
