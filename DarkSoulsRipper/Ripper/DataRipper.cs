using System;
using SoulsFormats;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DarkSoulsRipper.Ripper
{
    class DataRipper
    {
        private Dictionary<String, DataTable> dataTables;
        private DataDb database;
        private GameFiles gameFiles;

        public DataRipper() {
            gameFiles = new GameFiles();
            database = new DataDb("./rip.db");
            dataTables = new Dictionary<String, DataTable>();
        }
        public void rip() {

            database.Open();

            RipDarkSouls1TextFiles(ref dataTables);
            RipDarkSouls1ParamFiles(ref dataTables);

            database.Close();

            /*DEBUG*/
            Console.WriteLine("Done!");
        }
        private void RipDarkSouls1TextFiles(ref Dictionary<String, DataTable> tables) {
            List<BinderFile> textFiles = gameFiles.D1TextFiles();
            foreach (BinderFile binderFile in textFiles) {
                bool isJapanese = binderFile.Name.Contains("Data_JAPANESE");
                bool isEnglish = binderFile.Name.Contains("Data_ENGLISH");
                String textFileNameFull = Path.GetFileNameWithoutExtension(binderFile.Name);
                String textFileNameTrimmed = Regex.Match(textFileNameFull, @"(.+?)(_*)$").Groups[1].Value;
                String textFileNameSuffixed = textFileNameTrimmed + (isJapanese ? "_JAP" : isEnglish ? "_ENG" : "");
                String tableName = textFileNameSuffixed;

                DataTable table;
                if (!tables.TryGetValue(tableName, out table)) {
                    table = new DataTable();
                    tables.Add(tableName, table);
                    /*DEBUG*/
                    Console.WriteLine($"Created table {tableName}");
                }

                // TODO: Investigate why .msgbnd.dcx have duplicate .fmg files
                FMG textFile = FMG.Read(binderFile.Bytes);
                foreach (FMG.Entry entry in textFile.Entries)
                    table.setValue(entry.ID, "value", entry.Text);
            }
        }
        private void RipDarkSouls1ParamFiles(ref Dictionary<String, DataTable> tables) {
            List<PARAMDEF> paramDefs = gameFiles.D1ParamDefs();
            List<BinderFile> paramFiles = gameFiles.D1ParamFiles();
            foreach (BinderFile binderFile in paramFiles) {
                String textFileNameFull = Path.GetFileNameWithoutExtension(binderFile.Name);
                String textFileNameTrimmed = Regex.Match(textFileNameFull, @"(.+?)(_*)$").Groups[1].Value;
                String tableName = textFileNameTrimmed;

                DataTable table;
                if (!tables.TryGetValue(tableName, out table)) {
                    table = new DataTable();
                    tables.Add(tableName, table);
                    /*DEBUG*/
                    Console.WriteLine($"Created table {tableName}");
                }

                // TODO: Investigate why .msgbnd.dcx have duplicate .fmg files
                PARAM paramFile = PARAM.Read(binderFile.Bytes);
                paramFile.ApplyParamdefCarefully(paramDefs);

                foreach (PARAM.Row paramRow in paramFile.Rows) {
                    table.setValue(paramRow.ID, "Name", paramRow.Name);
                    if (paramRow.Cells != null && paramRow.Cells.Count > 0) {
                        foreach (PARAM.Cell paramCell in paramRow.Cells)
                            table.setValue(paramRow.ID, paramCell.Def.InternalName, paramCell.Value.ToString());
                    }
                }
            }
        }

    }
}
