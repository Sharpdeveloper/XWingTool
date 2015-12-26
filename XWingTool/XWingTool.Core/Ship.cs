using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XWingTool.Core
{
    [Serializable]
    public class Ship
    {
        public string Name { get; private set; }
        public string Gername { get; set; }
        public List<string> Factions { get; private set; }
        public int Attack { get; private set; }
        public int Agility { get; private set; }
        public int Hull { get; private set; }
        public int Shields { get; private set; }
        public int Energy { get; private set; }
        public double EpicPoints { get; private set; }
        public List<string> Actions { get; private set; }
        public List<int[]> Maneuvers { get; private set; }
        public bool Epic { get; private set; }
        public bool Large { get; private set; }
        public bool ManeuverKnown { get; private set; }
        private int count;
        public string MERCode { get; private set; }
        public int Green { get; private set; }
        public int Red { get; private set; }
        public int White { get; private set; }
        public int Maneuver { get; private set; }

        public Ship(string name, List<string> factions, int attack, int agility, int hull, int shields, List<string> actions, List<int[]> maneuvers, bool large, bool epic, int energy, double epicPoints, bool maneuverKnown)
        {
            Name = name;
            Gername = name;
            Factions = factions;
            Attack = attack;
            Agility = agility;
            Hull = hull;
            Shields = shields;
            Actions = actions;
            Maneuvers = maneuvers;
            Count = 0;
            Epic = epic;
            Large = large;
            Energy = energy;
            EpicPoints = EpicPoints;
            ManeuverKnown = maneuverKnown;
            Maneuver = 0;
            Green = 0;
            Red = 0;
            White = 0;
            for (int i = 0; i < Maneuvers.Count; i++)
            {
                foreach(int a in Maneuvers[i])
                {
                    if(a == 1)
                    {
                        Maneuver++;
                        White++;
                    }
                    else if(a == 2)
                    {
                        Maneuver++;
                        Green++;
                    }
                    else if (a == 3)
                    {
                        Maneuver++;
                        Red++;
                    }
                }
            }
            switch (name)
            {
                case "X-Wing":
                    MERCode = ":XWing2:";
                    break;
                case "Y-Wing":
                    MERCode = ":Y-Wing:";
                    break;
                case "TIE Fighter":
                    MERCode = ":Tiefighter:";
                    break;
                case "TIE Advanced":
                    MERCode = ":TieAdvanced:";
                    break;
                case "A-Wing":
                    MERCode = ":A-Wing:";
                    break;
                case "YT-1300":
                    MERCode = ":Falke:";
                    break;
                case "TIE Interceptor":
                    MERCode = ":TieInterceptor";
                    break;
                case "Firespray-31":
                    MERCode = ":SlaveI:";
                    break;
                case "HWK-290":
                    MERCode = ":HWK:";
                    break;
                case "Lambda-Class Shuttle":
                    MERCode = ":Imperial Shutt";
                    break;
                case "B-Wing":
                    MERCode = ":B-Wing:";
                    break;
                case "TIE Bomber":
                    MERCode = ":T-Bomber:";
                    break;
                case "GR-75 Medium Transporter":
                    MERCode = ":RebelTranspor";
                    break;
                case "CR90 Corvette (Fore)":
                case "CR90 Corvette (Aft)":
                    MERCode = ":TantiveIV:";
                    break;
                case "Z-95 Headhunter":
                    MERCode = ":Z95:";
                    break;
                case "TIE Defender":
                    MERCode = ":Tie Defender:";
                    break;
                case "E-Wing":
                    MERCode = ":E-Wing:";
                    break;
                case "TIE Phantom":
                    MERCode = ":Tie-Phantom:";
                    break;
                case "YT-2400":
                    MERCode = ":Outrider:";
                    break;
                case "VT-49 Decimator":
                    MERCode = ":VT49Decimator:";
                    break;
                case "StarViper":
                    MERCode = ":StarViper:";
                    break;
                case "M3-A Interceptor":
                    MERCode = ":M3-A:";
                    break;
                case "Aggressor":
                    MERCode = ":IG-2000:";
                    break;
                default:
                    MERCode = "";
                    break;
            }

        }

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                if (count < 0)
                    count = 0;
            }
        }

        public string GetActions()
        {
            string actions = "";
            bool first = true;
            foreach (var x in Actions)
            {
                if (first)
                    first = false;
                else
                    actions += ", ";

                actions += x;
            }

            return actions;
        }

        public string GetFactions()
        {
            string factions = "";
            bool first = true;
            foreach (var x in Factions)
            {
                if (first)
                    first = false;
                else
                    factions += ", ";

                factions += x;
            }

            return factions;
        }

        public List<string> GetText()
        {
            List<string> t = new List<string>();
            t.Add("Name:");
            t.Add(" " + Name + "\n");
            t.Add("Faction:");
            t.Add(" " + GetFactions() + "\n");
            t.Add("Attack:");
            t.Add(" " + Attack + "\n");
            t.Add("Agility:");
            t.Add(" " + Agility + "\n");
            t.Add("Hull:");
            t.Add(" " + Hull + "\n");
            t.Add("Shields:");
            t.Add(" " + Shields + "\n");
            t.Add("Shiptype:");
            if (Large)
                t.Add(" Large\n");
            else if (Epic)
                t.Add(" Epic\n");
            else
                t.Add(" Small\n");
            t.Add("Actions:");
            if (Actions == null)
                t.Add(" Actions unkown.");
            else
            {
                t.Add(" " + GetActions() + "\n");
            }
                t.Add("Maneuver:");
                t.Add(" " + Green + " green, " + White + " white and " + Red + " red\n");
                t.Add(Name + ".png");
            return t;
        }
    }
}
