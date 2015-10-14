using MobileTicket.Extensions;
using PeterO.Cbor;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTicket.Enteties
{
    public class IssuerSignatureHeader
    {
        public IssuerSignatureHeader(byte[] bytes)
        {
            Parse(bytes);
        }

        public IssuerSignatureHeader(SignatureAlgorithm algoritm, string issuerIdentidier, string keyIdentifier)
        {
            alg = algoritm;
            iid = issuerIdentidier;
            kid = keyIdentifier;
        }

        /// <summary>
        /// Signature Algorithm. Primary ES256, fallback RS256
        /// </summary>
        public SignatureAlgorithm alg { get; set; }

        /// <summary>
        /// Issuer identifier.
        /// </summary>
        public string iid { get; set; }

        /// <summary>
        /// Key identifier.
        /// </summary>
        public string kid { get; set; }


        public byte[] GetBytes()
        {
            if (alg == SignatureAlgorithm.None || string.IsNullOrEmpty(iid) || string.IsNullOrEmpty(kid))
                throw new Exception("IssuerSignatureHeader is missing data");

            var cborMap = CBORObject.NewMap();
            cborMap.Add(Encoding.UTF8.GetBytes("alg"), Encoding.UTF8.GetBytes(alg.ToString()));
            cborMap.Add(Encoding.UTF8.GetBytes("iid"), Encoding.UTF8.GetBytes(iid));
            cborMap.Add(Encoding.UTF8.GetBytes("kid"), Encoding.UTF8.GetBytes(kid));

            return cborMap.EncodeToBytes();
        }

        private void Parse(byte[] bytes)
        {
            CBORObject cborObject;
            try
            {
                cborObject = CBORObject.DecodeFromBytes(bytes);
            }
            catch
            {
                throw new ArgumentException("Could not parse IssuerSignatureHeader. Byte array could not be decoded to CBOR Object");
            }

            if (cborObject.Type == CBORType.Map)
            {
                var dict = cborObject.ConvertToDictionary();

                SignatureAlgorithm parsedAlgoritm;
                if (Enum.TryParse(GetDictionaryValue(dict, "alg"), out parsedAlgoritm))
                    alg = parsedAlgoritm;

                iid = GetDictionaryValue(dict, "iid");
                kid = GetDictionaryValue(dict, "kid");
            }
            else
            {
                throw new ArgumentException("Could not parse Issuer signature header. Decoded bytes not of expected type CBOR Map");
            }

        }


        private string GetDictionaryValue(Dictionary<string, CBORObject> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key].ConvertToString();
            else
                throw new MissingFieldException($"Could not find '{key}' in Issuer signature header");
        }

    }
}

