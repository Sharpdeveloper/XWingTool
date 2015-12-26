using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWingTool.Core
{
    [Serializable]
    public class Data
    {
        public List<Ship> Ships { get; set; }
        public List<Pilot> Pilots { get; set; }
        public List<Modification> Modifications { get; set; }
        public List<Title> Titles { get; set; }
        public List<Upgrade> Upgrades { get; set; }
        public List<IUpgrade> IUpgrades { get; set; }
        public List<Ship> IShips { get; set; }
        public List<Pilot> IPilots { get; set; }
        public List<string> PilotNames { get; set; }
        public List<string> PilotGerNames { get; set; }
        public List<string> ShipNames { get; set; }
        public List<string> ShipGerNames { get; set; }

        public Data()
        {
            Ships = new List<Ship>();
            Pilots = new List<Pilot>();
            Modifications = new List<Modification>();
            Titles = new List<Title>();
            Upgrades = new List<Upgrade>();
            IUpgrades = new List<IUpgrade>();
            IShips = new List<Ship>();
            IPilots = new List<Pilot>();
            PilotNames = new List<string>();
            ShipNames = new List<string>();
            PilotGerNames = new List<string>();
            ShipGerNames = new List<string>();
        }

        internal void AddShip(Ship ship)
        {
            Ships.Add(ship);
            ShipNames.Add(ship.Name);
            ShipGerNames.Add(ship.Name);
        }

        internal void AddPilot(Pilot pilot)
        {
            Pilots.Add(pilot);
            PilotNames.Add(pilot.Name);
            PilotGerNames.Add(pilot.Name);
        }
    }
}
