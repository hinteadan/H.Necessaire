using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire
{
    public struct Note
    {
        public Note(string id, string value)
        {
            this.Id = id;
            this.Value = value;
        }

        public string Id { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"[{Id}:\"{Value}\"]";
        }

        public static Note[] FromDictionary(Dictionary<string, string> keyValuePairs)
        {
            if (!keyValuePairs?.Any() ?? true)
                return new Note[0];

            return keyValuePairs.Select(x => new Note(x.Key, x.Value)).ToArray();
        }
    }
}
