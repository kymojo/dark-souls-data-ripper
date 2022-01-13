using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace DarkSoulsRipper.Json
{
    public class CrawlerFilePath
    {
        private const String DirectoryOutput = @"./output";
        private const String DirectoryD1 = @"D:/Programs/Steam/steamapps/common/DARK SOULS REMASTERED";
        private const String DirectoryD2 = @"D:/Programs/Steam/steamapps/common/Dark Souls II Scholar of the First Sin/Game";
        private const String DirectoryD2JP = @"D:/Documents/Dark Souls/DATA_DUMP/Dark Souls II Scholar of the First Sin/JP unpacked/Game";
        private const String DirectoryD3 = @"D:/Programs/Steam/steamapps/common/DARK SOULS III/Game";

        public DarkSoulsGame Game { get; set; }
        public String RelativePath { get; set; }
        public String FullPath { get { return GetFullPath(); } }
        public String OutputPath { get { return GetOutputPath(); } }
        public bool IsFile { get; set; }

        public CrawlerFilePath(DarkSoulsGame game) {
            Game = game;
            RelativePath = "";
        }

        public CrawlerFilePath Clone() {
            CrawlerFilePath clone = new CrawlerFilePath(Game);
            clone.RelativePath = RelativePath;
            clone.IsFile = IsFile;
            return clone;
        }

        public CrawlerFilePath CloneRelative(String relative, bool isFile = false) {
            CrawlerFilePath clone = Clone();
            clone.ShiftRelative(relative, isFile);
            return clone;
        }

        public void ShiftRelative(String relative, bool isFile = false) {
            RelativePath = Path.Combine(RelativePath, relative);
            IsFile = isFile;
        }

        public String GetFileBaseName(bool includeSubExtensions = false) {

            if (includeSubExtensions)
                return Path.GetFileNameWithoutExtension(RelativePath);
            else {
                String name, extensions;
                GetFilePathNameParts(RelativePath, out name, out extensions);
                return name;
            }
        }

        public String GetFileExtensions(bool includeSubExtensions = true) {
            if (!includeSubExtensions)
                return Path.GetExtension(RelativePath);
            else {
                String name, extensions;
                GetFilePathNameParts(RelativePath, out name, out extensions);
                return extensions;
            }
        }

        private String GetOutputGameDirectory() {
            return Path.Combine(DirectoryOutput, Game.ShorterName());
        }

        private String GetOutputPath() {
            return Path.Combine(GetOutputGameDirectory(), RelativePath);
        }
        private String GetFullPath() {
            return Path.Combine(GetGameDirectory(), RelativePath);
        }

        private String GetGameDirectory() {
            switch(Game.Enum) {
                case DarkSoulsGame.GameEnum.D1:
                    return DirectoryD1;
                case DarkSoulsGame.GameEnum.D2:
                    return DirectoryD2;
                case DarkSoulsGame.GameEnum.D2J:
                    return DirectoryD2JP;
                case DarkSoulsGame.GameEnum.D3:
                    return DirectoryD3;
                default:
                    return "";
            }
        }

        private void GetFilePathNameParts(String filePath, out String name, out String extensions) {
            String filename = Path.GetFileName(filePath);
            Match filenameMatch = Regex.Match(filename, @"^([^.]+)(\..+)$");
            name = filenameMatch.Groups[1].Value;
            extensions = filenameMatch.Groups[2].Value;
        }
    }
}
