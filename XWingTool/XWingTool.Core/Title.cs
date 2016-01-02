using System;
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
            return null;
        }
    }
}
