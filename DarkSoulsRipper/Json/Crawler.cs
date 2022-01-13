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
    class Crawler {
        public void Start() {
            CrawlDirectory(DarkSoulsGame.D1());
            CrawlDirectory(DarkSoulsGame.D2());
            CrawlDirectory(DarkSoulsGame.D2J());
            CrawlDirectory(DarkSoulsGame.D3());
        }

        private int CrawlDirectory(DarkSoulsGame game, CrawlerFilePath path = null) {
            if (path == null)
                path = new CrawlerFilePath(game);

            int filesCreated = 0;

            Directory.CreateDirectory(path.OutputPath);

            String[] files = Directory.GetFiles(path.FullPath);
            foreach (String filepath in files) {
                String filename = Path.GetFileName(filepath);
                filesCreated += HandleFile(path.CloneRelative(filename, true));
            }

            String[] subdirectories = Directory.GetDirectories(path.FullPath);
            foreach (String subdirectory in subdirectories) {
                String directoryName = Path.GetFileName(subdirectory);
                filesCreated += CrawlDirectory(game, path.CloneRelative(directoryName));
            }

            if (filesCreated == 0)
                Directory.Delete(path.OutputPath);

            return filesCreated;
        }

        private int HandleFile(CrawlerFilePath path, byte[] fileBytes = null, String subpath = null) {
            int filesCreated = 0;

            Console.WriteLine($"SEE {path.Game.ShorterName()} {path.RelativePath}");

            switch (path.GetFileExtensions()) {
                case ".msgbnd.dcx":     // DS1 Message File Binder
                case ".anibnd.dcx":     // DS1 Character animation binder
                case ".chrbnd.dcx":     // DS1 Character binder
                case ".chresdbnd.dcx":  // DS1 character... something binder
                    filesCreated += UnpackBinder(path);
                    break;

                case ".fmg": // Message File
                    JsonFMG json = JsonFMG.Make(path, fileBytes, subpath);
                    json.WriteFile(path.OutputPath);
                    filesCreated++;
                    break;

                case ".tpf":        // DS1 Texture Archive
                case ".tpf.dcx":    // DS1 Texture Archive binder
                    filesCreated += UnpackTextures(path, fileBytes);
                    break;

                case ".dds": // DS1 Texture Format
                case ".PNG": // common image format
                    if (fileBytes == null)
                        File.Copy(path.FullPath, path.OutputPath, true);
                    else
                        File.WriteAllBytes(path.OutputPath, fileBytes);
                    filesCreated++;
                    break;


                case ".esd.dcx": // DS1 character behavior binder
                case ".esd":
                    //ESD esd = (fileBytes == null ? ESD.Read(path.FullPath) : ESD.Read(fileBytes));
                    break;

                case ".hkx": // Havok Archive

                // DS3 /action
                case ".hks":
                case ".txt":
                // DS3 /adhoc
                case ".ccm":
                // DS1 /chr
                case ".chrtpfbt":
                // DS3 /chr
                case ".chrbnd":
                case ".behbnd":
                // DS1 /event
                case ".emedf.dcx":
                case ".emeld.dcx":
                case ".emevd.dcx":
                case ".evd.dcx":
                case ".nfd":
                // DS2 /decal
                case ".dclbnd":
                // DS2 /ezstate
                case ".edd":
                // DS1 /facegen
                case ".fgbnd": // DS1 Facegen Binder
                // DS3 /facegen
                case ".fgbnd.dcx":
                // DS2 /filter
                case ".fltparam":
                case ".fltbnd":
                // DS1 /font
                case ".ccm.dcx":
                // DS2 /font
                case ".fxo":
                case ".fontbnd.dcx":
                // DS1 /map
                case ".pointlist":
                case ".breakobj":
                case ".tpfbhd":
                case ".arealoadlist":
                case ".loadlistlist":
                case ".worldloadlistlist":
                case ".msb":
                // DS2 /map
                case ".ngp":
                case ".list":
                // DS3 /map
                case ".breakobj.dcx":
                case ".entryfilelist":
                case ".nva.dcx":
                case ".nvmhktbnd.dcx":
                case ".btab.dcx":
                case ".btpb.dcx":
                case ".btl.dcx":
                case ".mapbnd.dcx":
                case ".msb.dcx":
                case ".onav.dcx":
                // DS2 /material
                case ".bnd":
                // DS1 /menu
                case ".menuesdbnd.dcx":
                case ".drb":
                case ".drb.dcx":
                // DS2 /menu
                case ".febnd.dcx":
                case ".fetexbnd.dcx":
                // DS2 /model
                case ".texbnd":
                case ".envbnd.dcx":
                case ".gibdt":
                case ".gibhd":
                case ".hkxbdt":
                case ".hkxbhd":
                case ".mapbdt":
                case ".mapbhd":
                case ".tpfbdt":
                case ".commonbnd.dcx":
                case ".scnbnd":
                // DS2 /morpheme4
                case ".extanibnd.dcx":
                // DS1 /movww
                case ".bik":
                // DS1 /mtd
                case ".mtdbnd.dcx":
                // DS2 /NGWord
                case ".bin":
                // DS1 /obj
                case ".objbnd.dcx":
                // DS1 /other
                case ".rumblebnd":
                case ".loadlist":
                case ".movtae":
                // DS3 /other
                case ".movtae.dcx":
                case ".bn":
                // DS1 /param
                case ".parambnd.dcx":
                // DS3 /param
                case ".gparam.dcx":
                // DS1 /paramdef
                case ".paramdefbnd.dcx":
                // DS1 /parts
                case ".partsbnd.dcx":
                // DS2 /prefabeditor
                case ".pfbbin":
                // DS1 /remo
                case ".remobnd.dcx":
                // DS1 /script
                case ".luabnd.dcx":
                case ".talkesdbnd.dcx":
                // DS1 /sfx
                case ".ffxbnd.dcx":
                // DS2 /sfx
                case ".ffxbnd":
                // DS1 /shader
                case ".shaderbnd.dcx":
                // DS2 /shader
                case ".bld":
                case ".xpu":
                case ".xvu":
                // DS3 /shader
                case ".shaderbdlebnd.dcx":
                case ".shaderbdlebnddebug.dcx":
                // DS1 /sound
                case ".fev":
                case ".fsb":
                case ".itl":
                case ".mch":
                case ".mix":
                case ".rpc":
                // DS2 /sound
                case ".fevbnd.dcx":
                case ".vsd":
                case ".ini":
                // DS2 /timeact
                case ".tae":
                default:
                    break;
            }

            return filesCreated;
        }

        private int UnpackBinder(CrawlerFilePath path) {
            int filesCreated = 0;

            Directory.CreateDirectory(path.OutputPath);

            bool isBinder4 = false;
            switch (path.Game.Enum) {
                case DarkSoulsGame.GameEnum.D2:
                case DarkSoulsGame.GameEnum.D2J:
                    if (path.GetFileExtensions().Equals(".anibnd.dcx"))
                        isBinder4 = true;
                    break;
                case DarkSoulsGame.GameEnum.D3:
                    isBinder4 = true;
                    break;
                case DarkSoulsGame.GameEnum.D1:
                default:
                    break;
            }

            IBinder binder = (isBinder4 ? BND4.Read(path.FullPath) : BND3.Read(path.FullPath));

            List<String> binderFileNames = new List<String>();
            if (binder != null) {
                foreach (BinderFile file in binder.Files) {
                    String fileName = Path.GetFileName(file.Name);
                    filesCreated += HandleFile(path.CloneRelative(fileName, true), file.Bytes, file.Name);
                    binderFileNames.Add(file.Name);
                }
            }

            if (filesCreated == 0)
                Directory.Delete(path.OutputPath);
            else {
                JsonBinder json = new JsonBinder();
                json.Game = path.Game;
                json.ResourcePath = path.RelativePath;
                json.Files.AddRange(binderFileNames);

                json.WriteFile(path.OutputPath);
            }

            return filesCreated;
        }

        private int UnpackTextures(CrawlerFilePath path, byte[] fileBytes = null) {
            int filesCreated = 0;

            TPF tpf = (fileBytes == null ? TPF.Read(path.FullPath) : TPF.Read(fileBytes));

            Directory.CreateDirectory(path.OutputPath);

            foreach (TPF.Texture tex in tpf.Textures) {
                filesCreated += HandleFile(path.CloneRelative($"{tex.Name}.dds", true), tex.Bytes);
                // TODO Create TPF JSON
            }

            if (filesCreated == 0)
                Directory.Delete(path.OutputPath);

            return filesCreated;
        }
    }
}
