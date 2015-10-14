using PeterO.Cbor;

namespace MobileTicket.Enteties
{
    /// <summary>
    /// ParticipantObject is represented as an array containing participant ticket objects.
    /// JSON: [{}, {}, ...]
    /// </summary>
    public class ParticipantObject
    {
        private string _jsonString;

        public ParticipantObject(CBORObject cborObject)
        {
            _jsonString = cborObject.ToJSONString();
        }

        public ParticipantObject(string jsonString)
        {
            _jsonString = jsonString;
        }

        public CBORObject ToCbor()
        {
            return CBORObject.FromJSONString(_jsonString.ToLowerInvariant());
        }

        public override string ToString()
        {
            return _jsonString;
        }
    }
}

