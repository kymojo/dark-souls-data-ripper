using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using SoulsFormats;

namespace DarkSoulsRipper.Json
{
    class Crawler
    {
        private String DirectoryOutput = "./output";
        private String DirectoryD1 = @"D:/Programs/Steam/steamapps/common/DARK SOULS REMASTERED";
        private String DirectoryD2 = @"D:/Programs/Steam/steamapps/common/Dark Souls II Scholar of the First Sin/Game";
        private String DirectoryD2JP = @"D:/Documents/Dark Souls/DATA_DUMP/Dark Souls II Scholar of the First Sin/JP unpacked/Game";
        private String DirectoryD3 = @"D:/Programs/Steam/steamapps/common/DARK SOULS III/Game";
        private enum GameEnum
        {
            D1, D2, D2J, D3
        }
        private String GetGameDirectory(GameEnum game) {
            switch (game) {
                case GameEnum.D1:
                    return DirectoryD1;
                case GameEnum.D2:
                    return DirectoryD2;
                case GameEnum.D2J:
                    return DirectoryD2JP;
                case GameEnum.D3:
                    return DirectoryD3;
                default:
                    return "unknown";
            }
        }

        public void Start() {
            CrawlDirectory(GameEnum.D1);
            CrawlDirectory(GameEnum.D2);
            CrawlDirectory(GameEnum.D2J);
            CrawlDirectory(GameEnum.D3);
        }

        private int CrawlDirectory(GameEnum game, String relativePath = "") {
            int filesCreated = 0;

            String basePath = GetGameDirectory(game);
            String currentPath = Path.Combine(basePath, relativePath);
            String outputPath = Path.Combine(DirectoryOutput, game.ToString(), relativePath);

            Directory.CreateDirectory(outputPath);

            String[] files = Directory.GetFiles(currentPath);
            foreach(String filename in files) {
                filesCreated += HandleFile(game, relativePath, filename);
            }

            String[] subdirectories = Directory.GetDirectories(currentPath);
            foreach(String subdirectory in subdirectories) {
                String directoryName = Path.GetFileName(subdirectory);
                filesCreated += CrawlDirectory(game, Path.Combine(relativePath, directoryName));
            }

            if (filesCreated == 0 && !relativePath.Equals(""))
                Directory.Delete(outputPath);

            return filesCreated;
        }

        private int HandleFile(GameEnum game, String relativePath, String filePath) {
            int filesCreated = 0;

            String fileBaseName, fileExtensions;
            GetFilePathNameParts(filePath, out fileBaseName, out fileExtensions);

            switch (fileExtensions) {
                case ".msgbnd.dcx":
                    filesCreated += UnpackBinder(game, relativePath, filePath);
                    break;
                default:
                    break;
            }

            return filesCreated;
        }

        private int UnpackBinder(GameEnum game, String relativePath, String binderPath) {
            int filesCreated = 0;

            String fileBaseName, fileExtensions;
            GetFilePathNameParts(binderPath, out fileBaseName, out fileExtensions);

            String outputPath = Path.Combine(DirectoryOutput, relativePath, fileBaseName);
            Directory.CreateDirectory(outputPath);

            // TODO: unpack binder according to game
            // TODO: create binder JSON file
            // TODO: create files for binder contents

            if (filesCreated == 0)
                Directory.Delete(outputPath);

            return filesCreated;
        }

        private void GetFilePathNameParts(String filePath, out String name, out String extensions) {
            String filename = Path.GetFileName(filePath);
            Match filenameMatch = Regex.Match(filename, @"^([^.]+)(\..+)$");
            name = filenameMatch.Groups[1].Value;
            extensions = filenameMatch.Groups[2].Value;
        } 

        /* DS1
                /chr
                    .chrtpfbt
                    .anibnd.dcx
                    .chrbnd.dcx
                    .chresdbnd.dcx
                    .esd.dcx
                /event
                    .emedf.dcx
                    .emeld.dcx
                    .emevd.dcx
                    .evd.dcx
                    .nfd
                /facegen
                    .fgbnd
                /font
                    .ccm.dcx
                    .tpf.dcx
                /map
                    .pointlist
                    .breakobj
                    .tpfbhd
                    .arealoadlist
                    .tpf.dcx
                    .loadlistlist
                    .worldloadlistlist
                    .msb
                /menu
                    .tpf.dcx
                    .menuesdbnd.dcx
                    .drb
                    .drb.dcx
                /movww
                    .bik
                /msg
                    .msgbnd.dcx
                /mtd
                    .mtdbnd.dcx
                /obj
                    .objbnd.dcx
                /other
                    .rumblebnd
                    .tpf
                    .PNG
                    .loadlist
                    .movtae
                    .tpf.dcx
                /param
                    .parambnd.dcx
                /paramdef
                    .paramdefbnd.dcx
                /parts
                    .partsbnd.dcx
                /remo
                    .remobnd.dcx
                /script
                    .luabnd.dcx
                    .talkesdbnd.dcx
                /sfx
                    .ffxbnd.dcx
                /shader
                    .shaderbnd.dcx
                /sound
                    .fev
                    .fsb
                    .itl
                    .mch
                    .mix
                    .rpc
             */
    }
}
