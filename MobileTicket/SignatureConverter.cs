using System;
using System.Linq;

namespace MobileTicket
{
    public static class SignatureConverter
    {
        /*
            ASN.1 is a notation for structured data, and DER is a set of rules for transforming a data structure (described in ASN.1) into a sequence of bytes, and back.

            This is ASN.1, namely the description of the structure which an ECDSA signature exhibits:

            ECDSASignature ::= SEQUENCE {
                r   INTEGER,
                s   INTEGER
            }
            When encoded in DER, this becomes the following sequence of bytes:

            0x30 b1 0x02 b2 (vr) 0x02 b3 (vs)
            where:

            b1 is a single byte value, equal to the length, in bytes, of the remaining list of bytes (from the first 0x02 to the end of the encoding);
            b2 is a single byte value, equal to the length, in bytes, of (vr);
            b3 is a single byte value, equal to the length, in bytes, of (vs);
            (vr) is the signed big-endian encoding of the value "r", of minimal length;
            (vs) is the signed big-endian encoding of the value "s", of minimal length.
            "Signed big-endian encoding of minimal length" means that the numerical value must be encoded as a sequence of bytes, such that the least significant byte comes last (that's what "big endian" means), the total length is the shortest possible to represent the value (that's "minimal length"), and the first bit of the first byte specifies the sign of the value (that's "signed"). For ECDSA, the r and s values are positive integers, so the first bit of the first byte must be a 0; i.e. the first byte of (vr) (respectively (vs)) must have a value between 0x00 and 0x7F.

            For instance, if we were to encode the numerical value 117, it would use a single byte 0x75, and not a two-byte sequence 0x00 0x75 (because of minimality). However, the value 193 must be encoded as two bytes 0x00 0xC1, because a single byte 0xC1, by itself, would denote a negative integer, since the first (aka "leftmost") bit of 0xC1 is a 1 (a single byte of value 0xC1 represents the value -63).
        */

        static byte[] zeroByte = new byte[1] { 0 };      //0x00
        static byte[] delimeterByte = new byte[1] { 2 }; //0x02
        static byte[] startByte = new byte[1] { 48 };    //0x30

        public static byte[] FromDerEncoded(byte[] derEncodedSignature)
        {
            if (derEncodedSignature.Length <= 64 && derEncodedSignature[0] != startByte[0])
                throw new ArgumentException("Signature is not DER encoded");

            var rLength = derEncodedSignature[3];

            var r = derEncodedSignature.Skip(4).Take(rLength).ToArray();

            if (r[0] == 0)
                r = r.Skip(1).ToArray();

            var sLengthPosition = 4 + rLength + 1;

            var sLength = derEncodedSignature[sLengthPosition];
            var s = derEncodedSignature.Skip(sLengthPosition + 1).Take(sLength).ToArray();

            if (s[0] == 0)
                s = s.Skip(1).ToArray();

            return r.Concat(s).ToArray();
        }

        public static byte[] ToDerEncoded(byte[] signature)
        {
            var partLength = signature.Length / 2;

            if (partLength != 32)
                throw new ArgumentException("Cannot create DER encoded signature. Expected signature length = 64");

            var r = signature.Take(partLength).ToArray();
            var s = signature.Skip(partLength).Take(partLength).ToArray();

            if (r[0] > 127) //0x7F
                r = zeroByte.Concat(r).ToArray();

            if (s[0] > 127)
                s = zeroByte.Concat(s).ToArray();

            var b2 = new byte[1] { Convert.ToByte(r.Length) };
            var b3 = new byte[1] { Convert.ToByte(s.Length) };

            var combinedParts = delimeterByte.Concat(b2).Concat(r).Concat(delimeterByte).Concat(b3).Concat(s).ToArray();

            var b1 = new byte[1] { Convert.ToByte(combinedParts.Length) };

            return startByte.Concat(b1).Concat(combinedParts).ToArray();
        }
    }
}
