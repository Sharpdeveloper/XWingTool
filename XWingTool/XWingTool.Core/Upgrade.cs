using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XWingTool.Core
{
    [Serializable]
    public class Upgrade: IUpgrade
    {
        public int Id{ get;  set; }
        public string UpgradeSlot{ get;  set; }
        public List<string> Sources{ get;  set; }
        public int Points{ get;  set; }
        public bool Unique{ get;  set; }
        private int count;
        public string Name { get; set; }
        public string Gername { get; set; }
        public int Attack { get; private set; }
        public string Range { get; private set; }
        public string Aka { get; set; }
        public string Restriction { get; set; }
        public string Text { get; set; }

        public Upgrade(string name, string aka, int id, string slot, List<string> sources, int attack, string range, bool unique, int points)
        {
            Name = name;
            Gername = name;
            Id = id;
            UpgradeSlot = slot;
            Sources = sources;
            Unique = unique;
            Points = points;
            Attack = attack;
            Range = range;
            Count = 0;
            Aka = aka;
            Restriction = "";
            Text = "";
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

        public List<string> GetText()
        {
            List<string> t = new List<string>();
            t.Add("Name:");
            if (Unique)
                t.Add(" *" + Name + "\n");
            else
                t.Add(" " + Name + "\n");
            t.Add("Points:");
            t.Add(" " + Points + "\n");
            t.Add("Slot:");
            t.Add(" " + UpgradeSlot + "\n");
            if(Attack != -1)
            {
                t.Add("Attack:");
                t.Add(" " + Attack + "\n");
                t.Add("Range:");
                t.Add(" " + Range + "\n");
            }
            if (Restriction != "")
            {
                t.Add("Restriction:");
                t.Add(" " + Restriction + "\n");
            }
            if (Text != "")
            {
                t.Add("Cardtext:\n");
                if(Text.Contains("<strong>"))
                {
                    string rest = Text, big;
                    t.Add("");
                    int start, end, length;

                    while (rest.Contains("<strong>"))
                    {
                        start = rest.IndexOf("<strong>") + 8;
                        end = rest.IndexOf("</strong>");
                        length = end - start;
                        end += 9;
                        big = rest.Substring(start, length);
                        rest = rest.Substring(end);
                        t.Add(big);
                        if(rest.Contains("<strong>"))
                        {
                            big = rest.Remove(rest.IndexOf("<strong>"));
                            t.Add(big);
                        }
                        else
                            t.Add(rest);
                    }
                }
                else
                    t.Add(Text + "\n");
            }
            return t;
        }
    }
}