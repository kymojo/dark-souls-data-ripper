using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DarkSoulsRipper.Json
{
    public class JsonBinder : JsonFile
    {
        public List<String> Files { get; set; }
        public JsonBinder() : base() {
            Files = new List<String>();
        }

        public override string ToJson() {
            return JsonSerializer.Serialize(this);
        }
    }
}
