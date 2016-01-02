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
            return null;
        }
    }
}