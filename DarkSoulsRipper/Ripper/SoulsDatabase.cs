using System;
using SoulsFormats;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DarkSoulsRipper.Ripper
{
    class SoulsDatabase
    {
        private SqliteConnection db;

        public SoulsDatabase(String databasePath) {
            db = CreateDatabase(databasePath);
        }

        private SqliteConnection CreateDatabase(String databasePath) {
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
        private void ExecuteQuery(SqliteConnection db, String query) {
            SqliteCommand createTable = new SqliteCommand(query, db);
            createTable.ExecuteNonQuery();
        }
        /*private DataTable ExecuteQueryWithResults(SqliteConnection db, String query) {
            SqliteCommand createTable = new SqliteCommand(query, db);
            SqliteDataReader results = createTable.ExecuteReader();
            results.
        }*/

        public void WriteDataTable(SqliteConnection db, SoulsDataTable table) {
            CreateTable(db, table);
            List<String> insertStrings = new List<String>();
            foreach(Dictionary<String,object> row in table.GetRows()) {
                List<String> values = new List<String>();
                foreach (SoulsDataTable.Column column in table.GetColumns()) {
                    String valueString = row[column.GetName()].ToString();
                    if (column.GetColumnType() == SoulsDataTable.Column.ColumnType.TEXT)
                        valueString = $"'{valueString}'";
                    values.Add(valueString);
                }
                String insertString = String.Join(", ", values);
                insertString = $"({insertString})";
                insertStrings.Add(insertString);
            }
            // TODO build and execute insert query using insertStrings
        }

        private void CreateTable(SqliteConnection db, SoulsDataTable table) {
            if (table == null || table.GetColumns().Count == 0)
                throw new Exception("CreateTable() table cannot be null or have zero columns.");

            String dropQuery = $"DROP TABLE IF EXISTS {table.GetName()}";
            ExecuteQuery(db, dropQuery);

            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append($"CREATE TABLE {table.GetName()} ");
            queryBuilder.Append("(");
            foreach(SoulsDataTable.Column column in table.GetColumns()) {
                queryBuilder.Append(column.SqliteDefinitionString());
                queryBuilder.Append(", ");
            }
            queryBuilder.Remove(queryBuilder.Length - 2, 2);
            queryBuilder.Append(")");
            ExecuteQuery(db, queryBuilder.ToString());
        }
    }
}
