using System;
using SoulsFormats;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DarkSoulsRipper.Ripper
{
    class DataDb
    {
        private SqliteConnection db;

        public DataDb(String databasePath) {
            db = createDatabase(databasePath);
        }

        private SqliteConnection createDatabase(String databasePath) {
            if (!File.Exists(databasePath))
                File.Create(databasePath).Close();
            else
                File.Delete(databasePath);
            ;
            String connectionString = $"Filename={databasePath}";
            SqliteConnection db = new SqliteConnection(connectionString);
            return db;
        }

        public void Open() {
            db.Open();
        }
        public void Close() {
            db.Close();
        }
    }
}
