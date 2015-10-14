using NUnit.Framework;
using System;
using System.Linq;

namespace MobileTicket.Tests
{
    [TestFixture]
    class SignatureConverterTests
    {
        [Test]
        public void ExtractSignatureFromDER_Returns_Expected_Signature()
        {
            var derSignature = Convert.FromBase64String("MEUCIQDRPXHs1B9q60eFjGSQGh+GYJwMGCtgR6hpTFfSS9fcLAIgRvaM/3oQSaEAgvnnF9AEW/llZst/JpUnDnofUgvtWAM=");
            var expectedResult = Convert.FromBase64String("0T1x7NQfautHhYxkkBofhmCcDBgrYEeoaUxX0kvX3CxG9oz/ehBJoQCC+ecX0ARb+WVmy38mlScOeh9SC+1YAw==");

            var result = SignatureConverter.FromDerEncoded(derSignature);

            Assert.That(result.Length, Is.EqualTo(64));
            Assert.That(result.SequenceEqual(expectedResult), Is.True);
        }

        [Test]
        public void CreateDERFromSignature_Returns_Expected_DER()
        {
            var signature = Convert.FromBase64String("0T1x7NQfautHhYxkkBofhmCcDBgrYEeoaUxX0kvX3CxG9oz/ehBJoQCC+ecX0ARb+WVmy38mlScOeh9SC+1YAw==");
            var expectedResult = Convert.FromBase64String("MEUCIQDRPXHs1B9q60eFjGSQGh+GYJwMGCtgR6hpTFfSS9fcLAIgRvaM/3oQSaEAgvnnF9AEW/llZst/JpUnDnofUgvtWAM=");

            var result = SignatureConverter.ToDerEncoded(signature);

            Assert.That(result.Length, Is.GreaterThan(signature.Length));
            Assert.That(result.SequenceEqual(expectedResult), Is.True);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractSignatureFromDER_with_invalid_DER_signature_throws_Exeption()
        {
            var result = SignatureConverter.FromDerEncoded(new byte[50]);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateDERFromSignature_With_Wrong_Signature_length_throws_Exeption()
        {
            var result = SignatureConverter.ToDerEncoded(new byte[50]);
        }
    }
}
