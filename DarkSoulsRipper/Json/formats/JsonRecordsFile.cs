using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkSoulsRipper.Json
{
    abstract public class JsonRecordsFile : JsonFile
    {
        private List<JsonRecordsField> Fields { get; }
        public Dictionary<String, Object> Records { get; }

        public void AddField(String name, String description, JsonRecordDataType dataType) {
            JsonRecordsField field = new JsonRecordsField();
            field.Name = name;
            field.Description = description;
            field.DataType = dataType;
            Fields.Add(field);
        }

        private struct JsonRecordsField
        {
            public String Name { get; set; }
            public String Description { get; set; }
            public JsonRecordDataType DataType { get; set; }
        }
        public enum JsonRecordDataType
        {

        }
    }
}
