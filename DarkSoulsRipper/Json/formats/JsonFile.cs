using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DarkSoulsRipper.Json
{
    abstract public class JsonFile
    {
        public String ResourcePath { get; set; }
        public String Subpath { get; set; }
        public DarkSoulsGame Game { get; set; }

        abstract public String ToJson();

        public void WriteFile(String path) {
            if (!Path.GetExtension(path).Equals(".json"))
                path += ".json";

            File.WriteAllText(path, ToJson());
        }
    }
}
