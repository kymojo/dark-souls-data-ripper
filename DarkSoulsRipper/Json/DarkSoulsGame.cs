using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkSoulsRipper.Json
{
    public class DarkSoulsGame
    {
        public enum GameEnum
        {
            D1, D2, D2J, D3
        }

        public GameEnum Enum { get; private set; }

        private DarkSoulsGame(GameEnum game) {
            Enum = game;
        }

        public static DarkSoulsGame D1() {
            return new DarkSoulsGame(GameEnum.D1);
        }
        public static DarkSoulsGame D2() {
            return new DarkSoulsGame(GameEnum.D2);
        }
        public static DarkSoulsGame D2J() {
            return new DarkSoulsGame(GameEnum.D2J);
        }
        public static DarkSoulsGame D3() {
            return new DarkSoulsGame(GameEnum.D3);
        }

        public String ShortName() {
            switch (Enum) {
                case GameEnum.D1:
                    return "DSR";
                case GameEnum.D2:
                    return "DS2";
                case GameEnum.D2J:
                    return "DS2J";
                case GameEnum.D3:
                    return "DS3";
                default:
                    return "N/A";
            }
        }
        public String ShorterName() {
            return Enum.ToString();
        }
        public String Name(bool useRegion = true) {
            switch (Enum) {
                case GameEnum.D1:
                    return "Dark Souls: Remastered";
                case GameEnum.D2:
                    return "Dark Souls II: Scholar of the First Sin" + (useRegion ? " (International)" : "");
                case GameEnum.D2J:
                    return "Dark Souls II: Scholar of the First Sin" + (useRegion ? " (Japanese)" : "");
                case GameEnum.D3:
                    return "Dark Souls III";
                default:
                    return "N/A";
            }
        }

        public override string ToString() {
            return Name();
        }
    }
}
