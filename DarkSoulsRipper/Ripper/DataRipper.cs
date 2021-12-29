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
        private SoulsDatabase database;
        private GameFiles gameFiles;

        public DataRipper() {
            gameFiles = new GameFiles();
            database = new SoulsDatabase("./rip.db");
        }
        public void rip() {

            database.Open();

            RipDarkSouls1TextFiles();
            RipDarkSouls1ParamFiles();

            database.Close();

            /*DEBUG*/
            Console.WriteLine("Done!");
        }

        private SoulsDataTable RipTextFile(BinderFile binderFile) {
            bool isJapanese = binderFile.Name.Contains("Data_JAPANESE");
            bool isEnglish = binderFile.Name.Contains("Data_ENGLISH");
            String textFileNameFull = Path.GetFileNameWithoutExtension(binderFile.Name);
            String textFileNameTrimmed = Regex.Match(textFileNameFull, @"(.+?)(_*)$").Groups[1].Value;
            String textFileNameSuffixed = textFileNameTrimmed + (isJapanese ? "_JAP" : isEnglish ? "_ENG" : "");
            String textFileNamePrefixed = "TEXT_" + textFileNameSuffixed;
            String tableName = textFileNamePrefixed;

            List<SoulsDataTable.Column> columns = new List<SoulsDataTable.Column>();
            columns.Add(new SoulsDataTable.Column("ID", SoulsDataTable.Column.ColumnType.INTEGER));
            columns.Add(new SoulsDataTable.Column("Value", SoulsDataTable.Column.ColumnType.TEXT));
            SoulsDataTable table = new SoulsDataTable(tableName, columns);

            FMG textFile = FMG.Read(binderFile.Bytes);
            foreach (FMG.Entry entry in textFile.Entries) {
                Dictionary<String, object> row = new Dictionary<String, object>();
                row.Add("ID", entry.ID);
                row.Add("Value", entry.Text);
                table.AddRow(row);
            }
            return table;
        }

        private SoulsDataTable RipParamsFile(BinderFile binderFile, List<PARAMDEF> paramDefs) {

            String paramTableNameFull = Path.GetFileNameWithoutExtension(binderFile.Name);
            String paramTableNameTrimmed = Regex.Match(paramTableNameFull, @"(.+?)(_*)$").Groups[1].Value;
            String paramTableNamePrefixed = "PARAM_" + paramTableNameTrimmed;
            String tableName = paramTableNamePrefixed;

            PARAM paramFile = PARAM.Read(binderFile.Bytes);
            paramFile.ApplyParamdefCarefully(paramDefs);

            PARAMDEF def = null;
            foreach(PARAMDEF paramDef in paramDefs) {
                if (paramDef.ParamType == paramFile.ParamType)
                    def = paramDef;
            }
            if (def == null)
                throw new Exception($"RipParamsFile failed to find PARAMDEF '{paramFile.ParamType}' in paramDefs");

            // If ApplyParamdefCarefully doesn't work for some reason, force with ApplyParamdef
            if (paramFile.Rows[0].Cells == null)
                paramFile.ApplyParamdef(def);

            List<SoulsDataTable.Column> columns = new List<SoulsDataTable.Column>();
            columns.Add(new SoulsDataTable.Column("ID", SoulsDataTable.Column.ColumnType.INTEGER));
            columns.Add(new SoulsDataTable.Column("Name", SoulsDataTable.Column.ColumnType.TEXT));
            foreach (PARAMDEF.Field field in def.Fields) {
                String fieldName = field.InternalName;
                PARAMDEF.DefType fieldType = field.DisplayType;
                columns.Add(new SoulsDataTable.Column(fieldName, fieldType));
            }

            SoulsDataTable table = new SoulsDataTable(tableName, columns);


            foreach (PARAM.Row paramRow in paramFile.Rows) {
                Dictionary<String, object> row = new Dictionary<String, object>();
                row.Add("ID", paramRow.ID);
                row.Add("Name", paramRow.Name);
                if (paramRow.Cells != null && paramRow.Cells.Count > 0) {
                    foreach (PARAM.Cell paramCell in paramRow.Cells)
                        row.Add(paramCell.Def.InternalName, paramCell.Value.ToString());
                }
                table.AddRow(row);
            }

            return table;
        }

        private void RipDarkSouls1TextFiles() {
            List<BinderFile> textFiles = gameFiles.D1TextFiles();
            foreach (BinderFile binderFile in textFiles) {
                SoulsDataTable table = RipTextFile(binderFile);
                database.WriteDataTable(table);
            }
        }
        private void RipDarkSouls1ParamFiles() {
            List<PARAMDEF> paramDefs = gameFiles.D1ParamDefs();
            List<BinderFile> paramFiles = gameFiles.D1ParamFiles();
            foreach (BinderFile binderFile in paramFiles) {
                SoulsDataTable table = RipParamsFile(binderFile, paramDefs);
                database.WriteDataTable(table);
            }
        }

    }
}
