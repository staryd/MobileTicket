using MobileTicket.Enteties;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileTicket.Tests
{
    [TestFixture]
    class IssuerSignatureHeaderTests
    {
        [Test]
        public void HeaderFromParsed_Returns_Same_Sequence()
        {
            var headerBase64 = "o0NhbGdlRVMyNTZDa2lkQjE4Q2lpZEEx";
            var headerBytes = Convert.FromBase64String(headerBase64);
            var header = new IssuerSignatureHeader(headerBytes);

            var result = Convert.ToBase64String(header.GetBytes());

            Assert.That(result, Is.EqualTo(headerBase64));
        }

    }
}
