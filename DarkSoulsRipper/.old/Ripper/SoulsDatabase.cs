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
        private int maxInsertBatch = 500;

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
        private void ExecuteQuery(String query) {
            SqliteCommand createTable = new SqliteCommand(query, db);
            createTable.ExecuteNonQuery();
        }
        /*private DataTable ExecuteQueryWithResults(SqliteConnection db, String query) {
            SqliteCommand createTable = new SqliteCommand(query, db);
            SqliteDataReader results = createTable.ExecuteReader();
            results.
        }*/

        public void WriteDataTable(SoulsDataTable table) {
            CreateTable(table);
            /*DEBUG*/ Console.WriteLine($"Created table '{table.GetName()}'");
            List<String> insertStrings = new List<String>();
            foreach(Dictionary<String,object> row in table.GetRows()) {
                List<String> values = new List<String>();
                foreach (SoulsDataTable.Column column in table.GetColumns()) {
                    object value;
                    row.TryGetValue(column.GetName(), out value);
                    String valueString = (value != null ? value.ToString() : "NULL");
                    if (column.GetColumnType() == SoulsDataTable.Column.ColumnType.TEXT && value != null)
                        valueString = $"'{valueString.Replace("'","''")}'";
                    values.Add(valueString);
                }
                String insertString = String.Join(", ", values);
                insertString = $"({insertString})";
                insertStrings.Add(insertString);
            }

            int insertCount = 0;
            StringBuilder insertStringBuilder = new StringBuilder();
            while(insertStrings.Count > 0) {
                insertStringBuilder.Append($"INSERT INTO {table.GetName()} ({table.GetColumnsString()}) VALUES ");
                while (insertCount < maxInsertBatch && insertStrings.Count > 0) {
                    insertStringBuilder.Append(insertStrings[0]);
                    insertStrings.RemoveAt(0);
                    insertStringBuilder.Append(", ");
                    insertCount++;
                }
                insertStringBuilder.Remove(insertStringBuilder.Length - 2, 2);
                ExecuteQuery(insertStringBuilder.ToString());
                /*DEBUG*/ Console.WriteLine($"Inserted {insertCount} rows into table '{table.GetName()}'");
                insertStringBuilder.Clear();
                insertCount = 0;
            }
        }

        private void CreateTable(SoulsDataTable table) {
            if (table == null || table.GetColumns().Count == 0)
                throw new Exception("CreateTable() table cannot be null or have zero columns.");

            String dropQuery = $"DROP TABLE IF EXISTS {table.GetName()}";
            ExecuteQuery(dropQuery);

            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append($"CREATE TABLE {table.GetName()} ");
            queryBuilder.Append("(");
            foreach(SoulsDataTable.Column column in table.GetColumns()) {
                queryBuilder.Append(column.SqliteDefinitionString());
                queryBuilder.Append(", ");
            }
            queryBuilder.Remove(queryBuilder.Length - 2, 2);
            queryBuilder.Append(")");
            ExecuteQuery(queryBuilder.ToString());
        }
    }
}
