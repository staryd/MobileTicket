using MobileTicket.Enteties;
using MobileTicket.Services;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace MobileTicket.Tests
{
    [TestFixture]
    class SignatureServiceTests
    {
        byte[] header = Convert.FromBase64String("o0NhbGdlRVMyNTZDa2lkQjE4Q2lpZEEx");
        byte[] payload = Convert.FromBase64String("oWIxMIGqZXNfc3J2gWIxMGVzX3ZwcxpWCbhgZXNfcGF4gqJjbnVtYTFjY2F0YWGiY251bWEyY2NhdGFlZHNfZnKCCmMyNTBlc192ZXJhMWVzX3RpZGQxMDI0ZHNfcnYaAAFRgGVzX3ZwZRpWCwngZHNfdG+CCmMyNDBkc19mYaJmYW1vdW50YjQ4ZGNvZGVjc2Vr");
        byte[] signature = Convert.FromBase64String("MEUCIQDRPXHs1B9q60eFjGSQGh+GYJwMGCtgR6hpTFfSS9fcLAIgRvaM/3oQSaEAgvnnF9AEW/llZst/JpUnDnofUgvtWAM=");
        SignatureService service;
        IssuerSignedTicketBundle issuerSignedTicketBundle;

        [SetUp]
        public void SetUp()
        {
            service = new SignatureService(new DemoKeyRepository());

            // Create a bundle
            var signatureHeader = new IssuerSignatureHeader(header);
            var ticketBundle = new TicketBundle(payload);

            issuerSignedTicketBundle = new IssuerSignedTicketBundle(ticketBundle);
            issuerSignedTicketBundle.Header = signatureHeader;
            issuerSignedTicketBundle.Signature = signature;
        }

        [Test]
        public void CreateCoseSigningInput_Returns_Expected_SigningInput()
        {
            var expectedResult = "glgYo0NhbGdlRVMyNTZDa2lkQjE4Q2lpZEExWJOhYjEwgaplc19zcnaBYjEwZXNfdnBzGlYJuGBlc19wYXiComNudW1hMWNjYXRhYaJjbnVtYTJjY2F0YWVkc19mcoIKYzI1MGVzX3ZlcmExZXNfdGlkZDEwMjRkc19ydhoAAVGAZXNfdnBlGlYLCeBkc190b4IKYzI0MGRzX2ZhomZhbW91bnRiNDhkY29kZWNzZWs=";
            var result = Convert.ToBase64String(service.CreateCoseSigningInput(header, payload));

            Assert.That(result, Is.EqualTo(expectedResult));
        }


        [Test]
        public void Verify_Returns_True_When_Ok_Signature()
        {
            var mtb = new MtbContainer(issuerSignedTicketBundle);

            var result = service.Verify(mtb);

            Assert.That(result, Is.True);
        }


        [Test]
        public void Verify_Returns_False_When_Invalid_Signature()
        {
            issuerSignedTicketBundle.Signature = new byte[64];
            var mtb = new MtbContainer(issuerSignedTicketBundle);

            var result = service.Verify(mtb);

            Assert.That(result, Is.False);
        }

    }
}
