﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XWingTool.Core
{
    [Serializable]
    public class Modification: IUpgrade
    {
        public string Name{ get;  set; }
        public string Gername{ get;  set; }
        public int Id{ get; private set; }
        public int Points{ get; private set; }
        public List<string> Sources{ get; private set; }
        public Ship OnlyFor{ get; private set; }
        private int count;

        public Modification(string name, int id, int points, List<string> sources, Ship onlyFor)
        {
            Name = name;
            Gername = name;
            Id = id;
            Points = points;
            Sources = sources;
            OnlyFor = onlyFor;
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
    }
}
