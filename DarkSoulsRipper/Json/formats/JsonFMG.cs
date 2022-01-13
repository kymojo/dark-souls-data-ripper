using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsFormats;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DarkSoulsRipper.Json
{
    public class JsonFMG : JsonRecordsFile
    {
        private const String ID_key = "id";
        private const String VALUE_key = "value";

        public JsonFMG(DarkSoulsGame game) {
            Game = game;
            AddField(ID_key, "FMG record id", JsonRecordDataType.INTEGER);
            AddField(VALUE_key, "FMG record value", JsonRecordDataType.TEXT);
        }
        public JsonFMG(DarkSoulsGame game, FMG fmg) : this(game) {
            AddRecordsFromFMG(fmg);
        }
        public JsonFMG(DarkSoulsGame game, String path) : this(game, FMG.Read(path)) { }
        public JsonFMG(DarkSoulsGame game, byte[] bytes) : this(game, FMG.Read(bytes)) { }

        public static JsonFMG Make(CrawlerFilePath path, byte[] fileBytes = null, String subpath = null) {
            JsonFMG fmg;
            if (fileBytes != null) {
                fmg = new JsonFMG(path.Game, fileBytes);
                fmg.Subpath = subpath;
            } else
                fmg = new JsonFMG(path.Game, path.FullPath);
            fmg.ResourcePath = path.RelativePath;
            return fmg;
        }

        private void AddRecordsFromFMG(FMG fmg) {
            List<Dictionary<String, Object>> records = new List<Dictionary<String, Object>>();
            foreach (FMG.Entry entry in fmg.Entries) {
                Dictionary<String, Object> record = new Dictionary<String, Object>();
                record.Add(ID_key, entry.ID);
                record.Add(VALUE_key, entry.Text);
                AddRecord(record);
            }
        }

        override public string ToJson() {
            return JsonSerializer.Serialize(this);
        }
    }
}
