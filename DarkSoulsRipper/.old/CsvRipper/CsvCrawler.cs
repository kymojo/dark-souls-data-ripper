using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsFormats;

namespace DarkSoulsRipper.CsvRipper
{
    enum Game
    {
        DS1, DS2, DS3
    }

    class CsvCrawler
    {
        private Game game;
        private String gamePath;
        private String outputPath;

        public CsvCrawler(Game game, String gamePath, String outputPath) {
            this.game = game;
            this.gamePath = gamePath;
            this.outputPath = outputPath;
        }

        public void Crawl() {

            
            

        }

        private void Traverse(String relativePath = "") {
            String fullPath = Path.Combine(gamePath, relativePath);

            String[] filePaths = Directory.GetFiles(fullPath);
            foreach(String filePath in filePaths) {
                FileInfo fileInfo = new FileInfo(filePath);
                
            }
            String[] dirPaths = Directory.GetDirectories(fullPath);
            foreach(String dirPath in dirPaths) {
                DirectoryInfo dir = new DirectoryInfo(dirPath);
                Traverse(Path.Combine(relativePath,dir.Name));
            }
        }

        private void HandleFile(FileInfo file, String relativePath = "") {

            if (Regex.IsMatch(file.Name,@".*\.[^.\\]*bnd(\.dcx)?")) {

                BND3 binderV3;
                bool isBinderV3 = BND3.IsRead(file.FullName, out binderV3);
                if (isBinderV3) {
                    String compression = binderV3.Compression.ToString();
                    String format = binderV3.Format.ToString();
                    String version = binderV3.Version;
                    List<BinderFile> files = binderV3.Files;
                    // ...
                }
                BND4 binderV4;
                bool isBinderV4 = BND4.IsRead(file.FullName, out binderV4);
                if (isBinderV4) {
                    String extended = binderV4.Extended.ToString();
                    String compression = binderV4.Compression.ToString();
                    String format = binderV4.Format.ToString();
                    String version = binderV4.Version;
                    List<BinderFile> files = binderV4.Files;
                    // ...
                }

                return;
            }

        }

    }
}
