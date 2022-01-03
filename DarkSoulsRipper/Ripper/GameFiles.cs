using System;
using SoulsFormats;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DarkSoulsRipper.Ripper
{
    public enum GameType
    {
        DS,DS2,DS3
    };

    /// <summary>
    /// Game Files "factory"
    /// </summary>
    class GameFiles
    {
        private String d1DirPath = @"D:/Programs/Steam/steamapps/common/DARK SOULS REMASTERED";
        private String d2DirPath = @"D:/Programs/Steam/steamapps/common/Dark Souls II Scholar of the First Sin";
        private String d2JPDirPath = @"D:/Documents/Dark Souls/DATA_DUMP/Dark Souls II Scholar of the First Sin/JP unpacked";
        private String d3DirPath = @"D:/Programs/Steam/steamapps/common/DARK SOULS III";

        private String d1TextDirectoryEnglishPath = @"msg/ENGLISH";
        private String d1TextDirectoryJapanesePath = @"msg/JAPANESE";
        private String d1GameParamBinderPath = @"param/GameParam/GameParam.parambnd.dcx";
        private String d1GameParamDefBinderPath = @"paramdef/paramdef.paramdefbnd.dcx";

        private String d2MenuTextEPath = @"Game/menu/text/english";
        private String d2MenuTextJPath = @"Game/menu/text/japanese";

        private String d3ItemTextBinderEPath = @"Game/msg/engus";
        private String d3ItemTextBinderJPath = @"Game/msg/jpnjp";
        private String d3GameParamBinderPath = @"Game/param/drawparam";

        public BatchedFiles<FMG> GetTextBatches(GameType game) {
            switch(game) {
                case GameType.DS:
                    return D1TextBatches();
                case GameType.DS2:
                    return D2TextBatches();
                case GameType.DS3:
                    return D3TextBatches();
                default:
                    return new BatchedFiles<FMG>();
            }
        }

        /// <summary>
        /// Get all game text BinderFiles for Dark Souls Remastered
        /// </summary>
        public BatchedFiles<FMG> D1TextBatches() {
            UnbatchedFiles<FileInfo> binderFiles = new UnbatchedFiles<FileInfo>();

            String englishFilesPath = Path.Combine(d1DirPath, d1TextDirectoryEnglishPath);
            String japaneseFilesPath = Path.Combine(d1DirPath, d1TextDirectoryJapanesePath);
            binderFiles.Add(FindFiles(englishFilesPath, "ENG", @".*\.msgbnd\.dcx$"));
            binderFiles.Add(FindFiles(japaneseFilesPath, "JAP", @".*\.msgbnd\.dcx$"));

            UnbatchedFiles<FMG> fmgFiles = new UnbatchedFiles<FMG>();
            foreach (KeyValuePair<String,FileInfo> kvp in binderFiles.Files) {
                FileInfo file = kvp.Value;
                String batchName = kvp.Key;
                BND3 binder = BND3.Read(file.FullName);
                foreach(BinderFile bFile in binder.Files) {
                    FMG fmg = FMG.Read(bFile.Bytes);
                    String tableName = "TEXT_DS1_" + MakeBatchTableName(batchName, bFile.Name);
                    fmgFiles.Add(tableName, fmg);
                }
            }
            return fmgFiles.AsBatchedFiles();
        }

        public BatchedFiles<FMG> D2TextBatches() {
            UnbatchedFiles<FileInfo> rawFmgFiles = new UnbatchedFiles<FileInfo>();

            String englishFilesPath = Path.Combine(d2DirPath, d2MenuTextEPath);
            String japaneseFilesPath = Path.Combine(d2JPDirPath, d2MenuTextJPath);
            rawFmgFiles.Add(FindFiles(englishFilesPath, "ENG", @".*\.fmg$", null, true));
            rawFmgFiles.Add(FindFiles(japaneseFilesPath, "JAP", @".*\.fmg$", null, true));

            UnbatchedFiles<FMG> fmgFiles = new UnbatchedFiles<FMG>();
            foreach (KeyValuePair<String, FileInfo> kvp in rawFmgFiles.Files) {
                FileInfo file = kvp.Value;
                String batchName = kvp.Key;
                FMG fmg = FMG.Read(file.FullName);
                String tableName = "TEXT_DS2_" + MakeBatchTableName(batchName, null);
                fmgFiles.Add(tableName, fmg);
            }

            BatchedFiles<FMG> batchedFmgFiles = fmgFiles.AsBatchedFiles();
            batchedFmgFiles.Batch(@"(.*)_m[0-9]{2}_[0-9]{2}_[0-9]{2}_[0-9]{2}(.*)", "$1$2");

            return batchedFmgFiles;
        }

        public BatchedFiles<FMG> D3TextBatches() {
            UnbatchedFiles<FileInfo> binderFiles = new UnbatchedFiles<FileInfo>();

            String englishFilesPath = Path.Combine(d3DirPath, d3ItemTextBinderEPath);
            String japaneseFilesPath = Path.Combine(d3DirPath, d3ItemTextBinderJPath);
            binderFiles.Add(FindFiles(englishFilesPath, "ENG", @".*\.msgbnd\.dcx$"));
            binderFiles.Add(FindFiles(japaneseFilesPath, "JAP", @".*\.msgbnd\.dcx$"));

            UnbatchedFiles<FMG> fmgFiles = new UnbatchedFiles<FMG>();
            foreach (KeyValuePair<String, FileInfo> kvp in binderFiles.Files) {
                FileInfo file = kvp.Value;
                String batchName = kvp.Key;
                BND4 binder = BND4.Read(file.FullName);
                foreach (BinderFile bFile in binder.Files) {
                    FMG fmg = FMG.Read(bFile.Bytes);
                    String tableName = "TEXT_DS3_" + MakeBatchTableName(batchName, bFile.Name);
                    fmgFiles.Add(tableName, fmg);
                }
            }

            BatchedFiles<FMG> batchedFmgFiles = fmgFiles.AsBatchedFiles();

            String[] ds3FileTranslations = File.ReadAllLines("../../../ds3_file_translations.csv");
            Dictionary<String, String> translationTable = new Dictionary<String, String>();
            foreach(String translationLine in ds3FileTranslations) {
                Match lineMatch = Regex.Match(translationLine, @"(.+)\t(.+)");
                String japanese = lineMatch.Groups[1].Value;
                String english = lineMatch.Groups[2].Value;
                batchedFmgFiles.RegexReplaceNames(japanese, japanese, english);
            }

            batchedFmgFiles.Batch(@"^(.+?)(dlc[12]_)(.+?)((dlc[12]_)(.+?))?$", "$1$3$6");

            return batchedFmgFiles;
        }

        /// <summary>
        /// Get all game paramDefs for Dark Souls Remastered
        /// </summary>
        /// <returns></returns>
        public List<PARAMDEF> D1ParamDefs() {
            List<PARAMDEF> paramDefs = new List<PARAMDEF>();
            String paramDefBinderPath = Path.Combine(d1DirPath, d1GameParamDefBinderPath);
            BND3 paramBinder = BND3.Read(paramDefBinderPath);
            foreach (BinderFile paramFile in paramBinder.Files) {
                PARAMDEF def = PARAMDEF.Read(paramFile.Bytes);
                paramDefs.Add(def);
            }
            return paramDefs;
        }
        /// <summary>
        /// Get all game params for Dark Souls Remastered
        /// </summary>
        /// <returns></returns>
        public List<BinderFile> D1ParamFiles() {
            String paramBinderPath = Path.Combine(d1DirPath, d1GameParamBinderPath);
            BND3 binder = BND3.Read(paramBinderPath);
            return binder.Files;
        }

        // ############################################

        //private String SubDirPathToFileName(String subDirPath) {
        //    subDirPath = subDirPath.Replace(" ", "");
        //    subDirPath = subDirPath.Replace("/", "_");
        //    subDirPath = subDirPath.Replace(@"\", "_");
        //    return subDirPath;
        //}

        //private Dictionary<String,List<FileInfo>> ConvertTextDictionaryToBatches(Dictionary<String,FileInfo> asdf) {
        //    Dictionary<String, List<FileInfo>> batches = new Dictionary<String, List<FileInfo>>();
        //    foreach (KeyValuePair<String, FileInfo> kvp in asdf) {
        //        List<FileInfo> batchList = new List<FileInfo>();
        //        batchList.Add(kvp.Value);
        //        batches.Add(kvp.Key, batchList);
        //    }
        //    return batches;
        //}

        //private void CombineDictionary<K,V>(ref Dictionary<K,V> dictionary, Dictionary<K,V> addDictionary) {
        //    foreach (KeyValuePair<K, V> kvp in addDictionary)
        //        dictionary.Add(kvp.Key, kvp.Value);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="subPath"></param>
        /// <param name="regexPattern"></param>
        /// <param name="regexIgnorePattern"></param>
        /// <param name="recursive"></param>
        /// <returns>Dictionary keyed off subpath+filename with corresponding FileInfo values</returns>
        private UnbatchedFiles<FileInfo> FindFiles(String path, String subPath, String regexPattern, String regexIgnorePattern = null, bool recursive = false) {
            UnbatchedFiles<FileInfo> files = new UnbatchedFiles<FileInfo>(); 
            
            if (subPath == null)
                subPath = "";

            Dictionary<String, FileInfo> foundFiles = new Dictionary<String, FileInfo>();
            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] directoryFiles = directory.GetFiles();
            foreach (FileInfo file in directoryFiles) {
                if (regexPattern == null || Regex.IsMatch(file.Name, regexPattern))
                    if (regexIgnorePattern == null || !Regex.IsMatch(file.Name, regexIgnorePattern)) {
                        String fileName = Path.Combine(subPath, file.Name);
                        foundFiles.Add(fileName,file);
                    }
            }
            if (foundFiles.Count > 0)
                files = new UnbatchedFiles<FileInfo>(foundFiles);

            if (recursive) {
                DirectoryInfo[] subdirectories = directory.GetDirectories();
                foreach (DirectoryInfo dir in subdirectories) {
                    String dirSubPath = Path.Combine(subPath, dir.Name);

                    UnbatchedFiles<FileInfo> subfolderFiles = FindFiles(dir.FullName, dirSubPath, regexPattern, regexIgnorePattern, recursive);
                    if (files.IsEmpty())
                        files = subfolderFiles;
                    else
                        files.Add(subfolderFiles);
                }
            }

            return files;
        }

        private String MakeBatchTableName(String batchName, String filePath) {
            bool hasBatchName = (batchName != null && batchName.Length > 0);
            bool hasFilePath = (filePath != null && filePath.Length > 0);
            String batchNameBaseDirs = (hasBatchName ? Path.GetDirectoryName(batchName) : "");
            String batchNameFileNoExt = (hasBatchName ? Regex.Match(Path.GetFileName(batchName), @"^[^.]+").Value : "");
            String filePathNameNoExt = (hasFilePath ? Regex.Match(Path.GetFileName(filePath), @"^[^.]+").Value : "");

            String newBatchName = "";
            if (batchNameBaseDirs.Length > 0)
                newBatchName += batchNameBaseDirs + "_";
            if (batchNameFileNoExt.Length > 0)
                newBatchName += batchNameFileNoExt + "_";
            if (filePathNameNoExt.Length > 0)
                newBatchName += filePathNameNoExt;

            newBatchName = Regex.Replace(newBatchName, @"(\\|\/)", "_");
            newBatchName = Regex.Replace(newBatchName, @"( |\.|_$)", "");

            if (Regex.IsMatch(newBatchName, @"^(ENG|JAP)_"))
                newBatchName = Regex.Replace(newBatchName, @"^(ENG|JAP)_(.*)", @"$2_$1");

            newBatchName = Regex.Replace(newBatchName, @"_{2,}", "_");

            return newBatchName;
        }
    }
}
