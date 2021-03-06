﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XWingTool.Core
{
    [Serializable]
    public class Title:IUpgrade
    {
        public string Name{ get; set; }
        public string Gername{ get; set; }
        public int Id{ get;  set; }
        public List<string> Sources{ get;  set; }
        public bool Unique{ get;  set; }
        public Ship OnlyFor{ get;  set; }
        public int Points{ get;  set; }
        public string UpgradeSlot { get; set; }
        public string Text { get; set; }
        public string Restriction { get; set; }
        private int count;

        public Title(string name, int id, List<string> sources, bool uniqe, Ship onlyFor, int points)
        {
            Name = name;
            Gername = name;
            Id = id;
            Sources = sources;
            Unique = uniqe;
            OnlyFor = onlyFor;
            Points = points;
            UpgradeSlot = "Title";
            Restriction = "";
            Text = "";
            Count = 0;
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
            if(OnlyFor != null)
            {
                t.Add("Only For:");
                t.Add(" " + OnlyFor.Name + "\n");
            }
            //if (Restriction != "")
            //{
            //    t.Add("Restriction:");
            //    t.Add(" " + Restriction + "\n");
            //}
            if (Text != "")
            {
                t.Add("Cardtext:\n");
                if (Text.Contains("<strong>"))
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
                        if (rest.Contains("<strong>"))
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
