using MobileTicket.Enteties;
using NUnit.Framework;
using System;
using System.Linq;

namespace MobileTicket.Tests
{
    [TestFixture]
    class IssuerSignedTicketBundleTests
    {
        private string issuerSignedTicketBundleB64 = "g1gYo0NhbGdlRVMyNTZDa2lkQjE4Q2lpZEExWJOhYjEwgaplc19zcnaBYjEwZXNfdnBzGlYJuGBlc19wYXiComNudW1hMWNjYXRhYaJjbnVtYTJjY2F0YWVkc19mcoIKYzI1MGVzX3ZlcmExZXNfdGlkZDEwMjRkc19ydhoAAVGAZXNfdnBlGlYLCeBkc190b4IKYzI0MGRzX2ZhomZhbW91bnRiNDhkY29kZWNzZWtYRzBFAiEA0T1x7NQfautHhYxkkBofhmCcDBgrYEeoaUxX0kvX3CwCIEb2jP96EEmhAIL55xfQBFv5ZWbLfyaVJw56H1IL7VgD";
        [Test]
        public void GetBytes_Returns_SameSequence_As_Parsed_Data()
        {
            var bytes = Convert.FromBase64String(issuerSignedTicketBundleB64);
            var signedBundle = new IssuerSignedTicketBundle(bytes);

            var result = signedBundle.GetBytes();

            Assert.That(result.SequenceEqual(bytes), Is.True);
        }

    }
}
