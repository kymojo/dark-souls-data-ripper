using System;
using SoulsFormats;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DarkSoulsRipper.Ripper
{
    class DataTable
    {
        private Dictionary<int, Dictionary<String, String>> table;
        private HashSet<String> columns;
        public DataTable() {
            table = new Dictionary<int, Dictionary<String, String>>();
            columns = new HashSet<String>();
        }

        public void setValue(int rowId, String key, String value) {
            if (!columns.Contains(key))
                columns.Add(key);
            if (!table.ContainsKey(rowId))
                table.Add(rowId, new Dictionary<String, String>());
            table[rowId][key] = value;
        }

        public String getValue(int rowId, String key) {
            if (table.ContainsKey(rowId))
                return table[rowId][key];
            else
                return null;
        }
    }
}
