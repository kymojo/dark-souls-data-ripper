using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkSoulsRipper.Json
{
    abstract public class JsonRecordsFile : JsonFile
    {
        private List<Dictionary<String, Object>> _records;
        private List<JsonRecordsField> _fields;

        private List<JsonRecordsField> Fields { get { return _fields; } }
        public List<Dictionary<String, Object>> Records { get { return _records; } }

        public void AddField(String name, String description, JsonRecordDataType dataType) {
            if (Fields == null)
                _fields = new List<JsonRecordsField>();

            JsonRecordsField field = new JsonRecordsField();
            field.Name = name;
            field.Description = description;
            field.DataType = dataType;
            Fields.Add(field);
        }
        public void AddRecord(Dictionary<String,Object> record) {
            if(Records == null)
                _records = new List<Dictionary<String, Object>>();

            Records.Add(record);
        }

        private struct JsonRecordsField
        {
            public String Name { get; set; }
            public String Description { get; set; }
            public JsonRecordDataType DataType { get; set; }
        }
        public enum JsonRecordDataType
        {
            NULL,    // dummy8
            TEXT,
            INTEGER, // u8, s8, u16, s16, u32, s32 (8 = 1 byte; 16 = 2 byte; 32 = 4 byte)
            DECIMAL  // f32
        }
    }
}
