using PeterO.Cbor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileTicket.Extensions
{
    public static class CBORObjectExtensions
    {
        public static string ConvertToString(this CBORObject cborObject)
        {
            switch (cborObject.Type)
            {
                case CBORType.ByteString:
                    return Encoding.UTF8.GetString(cborObject.GetByteString());
                case CBORType.TextString:
                    return cborObject.AsString();
                default:
                    return cborObject.ToString();
            }
        }

        public static Dictionary<string, CBORObject> ConvertToDictionary(this CBORObject cborObject)
        {
            if (cborObject.Type != CBORType.Map)
                throw new Exception($"Cannot convert CBORObject of type {cborObject.Type} into dictionary");

            var dictionary = new Dictionary<string, CBORObject>();

            for (var i = 0; i < cborObject.Count; i++)
            {
                var key = cborObject.Keys.ElementAt(i);
                var value = cborObject.Values.ElementAt(i);
                dictionary.Add(key.ConvertToString(), value);
            }

            return dictionary;
        }

        public static List<CBORObject> ConvertToList(this CBORObject cborObject)
        {
            if (cborObject.Type != CBORType.Array)
                throw new System.Exception(string.Format("Cannot convert CBORObject of type {0} into List", cborObject.Type));

            List<CBORObject> list = new List<CBORObject>();

            for (var i = 0; i < cborObject.Count; i++)
            {
                list.Add(cborObject.Values.ElementAt(i));
            }

            return list;
        }
    }
}

