using MobileTicket.Enteties;
using NUnit.Framework;
using System;

namespace MobileTicket.Tests
{
    class MtbContainerTests
    {
        string mtbB64 = "okFwWQFeg1g+pGNkaWR4IGY5NjA5NjYyNjZhZjExZTU4YjllMWZmNDZiYTRlOTA1Y2FsZ2VIUzI1NmF0GlYKlVlja2lkYTFY+YNYGKNDYWxnZUVTMjU2Q2tpZEIxOENpaWRBMViToWIxMIGqZXNfc3J2gWIxMGVzX3ZwcxpWCbhgZXNfcGF4gqJjbnVtYTFjY2F0YWGiY251bWEyY2NhdGFlZHNfZnKCCmMyNTBlc192ZXJhMWVzX3RpZGQxMDI0ZHNfcnYaAAFRgGVzX3ZwZRpWCwngZHNfdG+CCmMyNDBkc19mYaJmYW1vdW50YjQ4ZGNvZGVjc2VrWEcwRQIhANE9cezUH2rrR4WMZJAaH4ZgnAwYK2BHqGlMV9JL19wsAiBG9oz/ehBJoQCC+ecX0ARb+WVmy38mlScOeh9SC+1YA1ggZErjqA6tmkHjBPphs3hg+IXWECW/YgI11R7BFWXix71BdkEB";

        [Test]
        public void CreateMtbFromBytes_Creates_A_MtbContainer_with_TicketBundle()
        {
            var bytes = Convert.FromBase64String(mtbB64);
            var mtb = new MtbContainer(bytes);

            Assert.That(mtb, Is.Not.Null);
            Assert.That(mtb.IssuerSignedTicketBundle, Is.Not.Null);
        }
    }
}
