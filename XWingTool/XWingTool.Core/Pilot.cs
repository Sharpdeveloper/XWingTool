using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XWingTool.Core
{
    [Serializable]
    public class Pilot
    {
        public string Name{ get; private set; }
        public string Gername{ get; set; }
        public string PilotsFaction{ get; private set; }
        public int Id{ get; private set; }
        public bool Unique{ get; private set; }
        public Ship PilotsShip{ get; private set; }
        public int Skill{ get; private set; }
        public int Points{ get; private set; }
        public List<string> Slots{ get; private set; }
        public List<IUpgrade> Upgrades { get; set; }
        public string Wave { get; private set; }
        private int count;
        public string Text { get; set; }
        public string ShipName { get; set; }

        public Pilot(string name, string faction, int id, bool unique, Ship ship, int skill, int points, List<string> slots)
        {
            Name = name;
            Gername = name;
            PilotsFaction = faction;
            Id = id;
            Unique = unique;
            PilotsShip = ship;
            Skill = skill;
            Points = points;
            Slots = slots;
            Count = 0;
            Text = "";
            try {
                ShipName = ship.Name;
            }
            catch
            {
                ShipName = "";
            }

            if (id <= 22)
                Wave = "1";
            else if (id <= 40)
                Wave = "2";
            else if (id <= 56)
                Wave = "3";
            else if (id <= 62)
                Wave = "3.5";
            else if (id == 63)
                Wave = "Epic 1";
            else if (id <= 79)
                Wave = "4";
            else if (id <= 81)
                Wave = "Epic 2";
            else if (id <= 85)
                Wave = "Epic 1";
            else if (id <= 89)
                Wave = "4.5";
            else if (id <= 91)
                Wave = "Epic 2";
            else if (id <= 99)
                Wave = "5";
            else if (id <= 127)
                Wave = "6";
            else if (id <= 130)
                Wave = "Epic 3";
            else
                Wave = "Unbekannt";
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

        public Pilot Copy()
        {
            return new Pilot(this.Name, this.PilotsFaction, this.Id, this.Unique, this.PilotsShip, this.Skill, this.Points, this.Slots);
        }

        public List<string> GetText()
        {
            List<string> t = new List<string>();
            t.Add("Name:");
            if(Unique)
                t.Add(" *" + Name + "\n");
            else
                t.Add(" " + Name + "\n");
            t.Add("Points:");
            t.Add(" " + Points + "\n");
            t.Add("Faction:");
            t.Add(" " + PilotsFaction + "\n");
            t.Add("Pilotskill:");
            t.Add(" " + Skill + "\n");
            t.Add("Ship:");
            if (PilotsShip != null)
            {
                t.Add(" " + PilotsShip.Name + " (Attack: " + PilotsShip.Attack + " Agility: " + PilotsShip.Agility + " Hull: " + PilotsShip.Hull + " Shields: " + PilotsShip.Shields + ") \n");

            }
            else
                t.Add(" Ship unkown.");
            t.Add("Shiptype:");
            if (PilotsShip.Large)
                t.Add(" Large\n");
            else if (PilotsShip.Epic)
                t.Add(" Epic\n");
            else
                t.Add(" Small\n");
            t.Add("Actions:");
            if (PilotsShip == null)
                t.Add(" Actions unkown.");
            else
            {
                t.Add(" " + PilotsShip.GetActions() + "\n");
            }
            t.Add("Slots:");
            if (Slots == null)
                t.Add(" Slots unknown.");
            else
            {
                t.Add(" " + GetSlots() + "\n");
            }
            if (Text != "")
            {
                t.Add("Cardtext:\n");
                if (Text.Contains("<strong>"))
                {
                    string rest = Text, big;
                    int start, end, length;

                    while (rest.Contains("<strong>"))
                    {
                        if (rest.Contains("<strong>"))
                        {
                            big = rest.Remove(rest.IndexOf("<strong>"));
                            t.Add(big);
                        }
                        else
                            t.Add(rest);
                        start = rest.IndexOf("<strong>") + 8;
                        end = rest.IndexOf("</strong>");
                        length = end - start;
                        end += 9;
                        big = rest.Substring(start, length);
                        rest = rest.Substring(end);
                        t.Add(big);
                    }
                    t.Add(rest);
                }
                else
                    t.Add(Text);
            }
            if (PilotsShip != null)
            {
                t.Add("\nManeuver:");
                t.Add(" " + PilotsShip.Green + " green, " + PilotsShip.White + " white and " + PilotsShip.Red + " red\n");
                t.Add(PilotsShip.Name + ".png");
            }
            return t;
        }

        public string GetSlots()
        {
            string s = "";
            bool first = true;
            foreach (var x in Slots)
            {
                if (first)
                    first = false;
                else
                    s += ", ";

                s += x;
            }

            return s;
        }
    }
}
