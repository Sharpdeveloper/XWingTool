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
        public List<string> ShipNames { get; set; }
        public List<string> PilotNames { get; set; }
        public List<string> UpgradeNames { get; set; }
        public List<string> UpgradeSlots { get; set;}

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
            ShipNames = new List<string>();
            PilotNames = new List<string>();
            UpgradeNames = new List<string>();
            UpgradeSlots = new List<string>();
        }

        internal void AddShip(Ship ship)
        {
            Ships.Add(ship);
            ShipNames.Add(ship.Name);
        }

        internal void AddPilot(Pilot pilot)
        {
            Pilots.Add(pilot);
            PilotNames.Add(pilot.Name);
        }

        internal void AddIUpgrade(IUpgrade upgr)
        {
            IUpgrades.Add(upgr);
            if (!UpgradeSlots.Contains(upgr.UpgradeSlot) && upgr.UpgradeSlot != "")
                UpgradeSlots.Add(upgr.UpgradeSlot);
            UpgradeNames.Add(upgr.Name);
            if (upgr is Modification)
                Modifications.Add((Modification)upgr);
            else if (upgr is Title)
                Titles.Add((Title)upgr);
            else
                Upgrades.Add((Upgrade)upgr);
        }

        internal void ClearShips()
        {
            Ships = new List<Ship>();
            ShipNames = new List<string>();
        }

        internal void ClearPilots()
        {
            Pilots = new List<Pilot>();
            PilotNames = new List<string>();
        }

        internal void ClearUpgrades()
        {
            IUpgrades = new List<IUpgrade>();
            Upgrades = new List<Upgrade>();
            Titles = new List<Title>();
            Modifications = new List<Modification>();
            UpgradeNames = new List<string>();
        }
    }
}
