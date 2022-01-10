using System;
using SoulsFormats;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using DarkSoulsRipper.Ripper;
using DarkSoulsRipper.Json;

namespace DarkSoulsRipper
{
    class Program
    {
        static void Main(string[] args) {

            // This is required for SoulsFormats to properly use encodings
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //// Rip game data
            //DataRipper ripper = new DataRipper();
            //ripper.rip();

            Crawler crawler = new Crawler();
            crawler.Start();
        }
    }
}
