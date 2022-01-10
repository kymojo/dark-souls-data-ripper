using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkSoulsRipper.Json
{
    public class DarkSoulsGame
    {
        private int game;

        public DarkSoulsGame(int game) {
            this.game = game;
        }
        public String ShortName() {
            switch (game) {
                case 1:
                    return "DSR";
                case 2:
                    return "DS2";
                case 3:
                    return "DS3";
                default:
                    return "N/A";
            }
        }
        public String Name() {
            switch (game) {
                case 1:
                    return "Dark Souls: Remastered";
                case 2:
                    return "Dark Souls II: Scholar of the First Sin";
                case 3:
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
