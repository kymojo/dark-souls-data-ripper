using System;
using SoulsFormats;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DarkSoulsRipper
{
    class Program
    {
        static void Main(string[] args) {

            // Required for SoulsFormats to properly use encodings
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Ripper ripper = new Ripper();
            ripper.rip();
        }
    }
    class Ripper
    {
        private String dks1DirPath = @"D:/Programs/Steam/steamapps/common/DARK SOULS REMASTERED";
        private String dks2DirPath = @"D:/Programs/Steam/steamapps/common/Dark Souls II Scholar of the First Sin";
        private String dks3DirPath = @"D:/Programs/Steam/steamapps/common/DARK SOULS III";

        private String dks1TextDirectoryEnglishPath = @"msg/ENGLISH";
        private String dks1TextDirectoryJapanesePath = @"msg/JAPANESE";
        private String dks1GameParamBinderPath = @"param/GameParam/GameParam.parambnd.dcx";

        private String dks2MenuTextGeneralEPath = @"Game/menu/text/english";
        private String dks2MenuTextBloodMessageEPath = @"Game/menu/text/english/bloodmes";
        private String dks2MenuTextTalkEPath = @"Game/menu/text/english/talk";

        private String dks3ItemTextBinderEPath = @"Game/msg/engus";
        private String dks3ItemTextBinderJPath = @"Game/msg/jpnjp";
        private String dks3GameParamBinderPath = @"Game/param/drawparam";
  
        public Ripper() {

        }

        public void rip() {

            Dictionary<String, DataTable> tables = new Dictionary<String, DataTable>();

            ripDarkSouls1TextFiles(ref tables);

            /*DEBUG*/
            Console.WriteLine("Done!");
        }

        private List<FileInfo> getDirectoryFilesWithExtension(String path, String extensionNoLeadingPeriod) {
            List<FileInfo> foundFiles = new List<FileInfo>();
            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[]  files = directory.GetFiles();
            foreach (FileInfo file in files) {
                if (Regex.IsMatch(file.Name, @$"[^.]\.{extensionNoLeadingPeriod}$"))
                    foundFiles.Add(file);
            }
            return foundFiles;
        }

        private void ripDarkSouls1TextFiles(ref Dictionary<String, DataTable> tables) {
            List<FileInfo> files = new List<FileInfo>();
            String englishFilesPath = Path.Combine(dks1DirPath, dks1TextDirectoryEnglishPath);
            String japaneseFilesPath = Path.Combine(dks1DirPath, dks1TextDirectoryJapanesePath);
            files.AddRange(getDirectoryFilesWithExtension(englishFilesPath, "msgbnd.dcx"));
            files.AddRange(getDirectoryFilesWithExtension(japaneseFilesPath, "msgbnd.dcx"));

            foreach (FileInfo file in files) {
                BND3 binder = BND3.Read(file.FullName);
                foreach (BinderFile binderFile in binder.Files) {
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
        }
    }

    class DataTable
    {
        private Dictionary<int,Dictionary<String, String>> table;
        public DataTable() {
            table = new Dictionary<int, Dictionary<String, String>>();
        }

        public void setValue(int rowId, String key, String value) {
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
