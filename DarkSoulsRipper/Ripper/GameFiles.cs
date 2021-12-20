using System;
using SoulsFormats;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DarkSoulsRipper.Ripper
{
    class GameFiles
    {
        private String d1DirPath = @"D:/Programs/Steam/steamapps/common/DARK SOULS REMASTERED";
        private String d2DirPath = @"D:/Programs/Steam/steamapps/common/Dark Souls II Scholar of the First Sin";
        private String d3DirPath = @"D:/Programs/Steam/steamapps/common/DARK SOULS III";

        private String d1TextDirectoryEnglishPath = @"msg/ENGLISH";
        private String d1TextDirectoryJapanesePath = @"msg/JAPANESE";
        private String d1GameParamBinderPath = @"param/GameParam/GameParam.parambnd.dcx";
        private String d1GameParamDefBinderPath = @"paramdef/paramdef.paramdefbnd.dcx";

        private String d2MenuTextGeneralEPath = @"Game/menu/text/english";
        private String d2MenuTextBloodMessageEPath = @"Game/menu/text/english/bloodmes";
        private String d2MenuTextTalkEPath = @"Game/menu/text/english/talk";

        private String d3ItemTextBinderEPath = @"Game/msg/engus";
        private String d3ItemTextBinderJPath = @"Game/msg/jpnjp";
        private String d3GameParamBinderPath = @"Game/param/drawparam";

        /// <summary>
        /// Get all game text BinderFiles for Dark Souls Remastered
        /// </summary>
        public List<BinderFile> D1TextFiles() {
            List<BinderFile> textFiles = new List<BinderFile>();
            List<FileInfo> rawFiles = new List<FileInfo>();

            String englishFilesPath = Path.Combine(d1DirPath, d1TextDirectoryEnglishPath);
            String japaneseFilesPath = Path.Combine(d1DirPath, d1TextDirectoryJapanesePath);
            rawFiles.AddRange(DirectoryFilesWithExtension(englishFilesPath, "msgbnd.dcx"));
            rawFiles.AddRange(DirectoryFilesWithExtension(japaneseFilesPath, "msgbnd.dcx"));

            foreach (FileInfo file in rawFiles) {
                BND3 binder = BND3.Read(file.FullName);
                textFiles.AddRange(binder.Files);
            }

            return textFiles;
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

        private List<FileInfo> DirectoryFilesWithExtension(String path, String extensionNoLeadingPeriod) {
            List<FileInfo> foundFiles = new List<FileInfo>();
            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files) {
                if (Regex.IsMatch(file.Name, @$"[^.]\.{extensionNoLeadingPeriod}$"))
                    foundFiles.Add(file);
            }
            return foundFiles;
        }
    }
}
