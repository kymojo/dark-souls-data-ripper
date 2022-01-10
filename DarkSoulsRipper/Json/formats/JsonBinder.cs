using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkSoulsRipper.Json
{
    public class JsonBinder : JsonFile
    {
        public List<String> Files { get; set; }

        public override string ToJson() {
            throw new NotImplementedException();
        }
    }
}
