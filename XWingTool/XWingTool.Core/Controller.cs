using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace XWingTool.Core
{
    public class CardController
    {
        public string workPath, cards, translations, dataPath, maneuverPath, maneuver;
        public Data data;
        public Statistic stats;
        private const string squadfilecards = "https://raw.githubusercontent.com/geordanr/xwing/master/coffeescripts/cards-common.coffee";
        private const string squadfiletranslation = "https://raw.githubusercontent.com/geordanr/xwing/master/coffeescripts/cards-en.coffee";
        private const string maneuverFiles = "http://downloads.piratesoftatooine.de/Manoever.zip";

        public CardController()
        {
            workPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "X-Wing Tool");
            dataPath = Path.Combine(workPath, "DB.data");
            cards = System.IO.Path.Combine(workPath, "cards.txt");
            translations = System.IO.Path.Combine(workPath, "entrans.txt");
            maneuver = System.IO.Path.Combine(workPath, "manoever.zip");
            maneuverPath = System.IO.Path.Combine(workPath, "Manoever");

            if (!Directory.Exists(workPath))
            {
                Directory.CreateDirectory(workPath);
                ReLoad();
            }
            else
            {
                if (File.Exists(dataPath))
                {
                    data = Load();
                    createStats();
                }
                else
                    ReLoad();
            }

        }

        public void ReLoad()
        {
            LoadFiles();
            data = LoadContents();
            Translate();
            Save();
            createStats();
        }

        private void createStats()
        {
            stats = new Statistic();
            stats.Ships = data.Ships;
            stats.Pilots = data.Pilots;
            stats.Modifications = data.Modifications;
            stats.Upgrades = data.Upgrades;
            stats.Titles = data.Titles;
        }

        private Data Load()
        {
            Data data = null;
            IFormatter formatter = new BinaryFormatter();
            try
            {
                using (Stream stream = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    data = (Data)formatter.Deserialize(stream);
                }
            }
            catch
            {
                File.Delete(dataPath);
                data = null;
            }

            return data;
        }

        public void LoadStats(string path)
        {
            stats = null;
            IFormatter formatter = new BinaryFormatter();
            try
            {
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    stats = (Statistic)formatter.Deserialize(stream);
                }
            }
            catch
            {
               
            }
        }

        private void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(dataPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, data);
            }
        }

        public void SaveStats(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, stats);
            }
        }

        public void ImportLists(string path)
        {
            createStats();
            using(StreamReader sr = new StreamReader(path)){
                string list;
                while ((list = sr.ReadLine()) != null)
                {
                    stats.Parse(list);
                }
            }
        }

        public void ExportStats(string path) { 
            List<string> lines = new List<string>();
            stats.Sort();
            int length = stats.IPilots.Count > stats.IShips.Count ? stats.IPilots.Count : stats.IShips.Count;
            length = length > stats.IUpgrades.Count ? length : stats.IUpgrades.Count;
                    string line = "=SUMME(A2:A" + length + ");Schiffe;MERCode;;=SUMME(E2:E" + length + ");Piloten;Welle;;=SUMME(I2:I" + length + ");Aufwertungskarten;;=SUMME(L2:L" + length + ");Punktzahlen;;;=SUMME(P2:P" + length + ");Fraktionen;Prozent;Schiffe pro Fraktion;Schiffepro Liste;;Schiffe pro Squad;=E1/P1;;";
                    lines.Add(line);
                    List<string> PointFormulas = new List<string>(); ;
                    List<string> FactionFormulas = new List<string>();
                    List<string> ShipsPerFactionFormulas = new List<string>();
                    List<string> WaveFormulas = new List<string>();
                    for (int i = 0; i <= stats.Points.Count; i++)
                    {
                        line = "=L" + (i + 2) + "/L1";
                        PointFormulas.Add(line);
                    }
                    for (int i = 0; i <= stats.FPoints.Count; i++)
                    {
                        line = "=P" + (i + 2) + "/P1";
                        FactionFormulas.Add(line);
                    }
                    for (int i = 0; i <= stats.FPoints.Count; i++)
                    {
                        line = "=S" + (i + 2) + "/P" + (i + 2);
                        ShipsPerFactionFormulas.Add(line);
                    }
                    for (int i = 0; i <= stats.Waves.Count; i++)
                    {
                        line = "=W" + (i + 2) + "/E1";
                        WaveFormulas.Add(line);
                    }
                    for (int i = 0; i < length; i++)
                    {
                        line = "";
                        if (i < stats.IShips.Count)
                            line += stats.IShips[i].Count + ";" + stats.IShips[i].Gername + ";" + stats.IShips[i].MERCode + ";;";
                        else
                            line += ";;;;";
                        if (i < stats.IPilots.Count)
                            line += stats.IPilots[i].Count + ";" + stats.IPilots[i].Gername + ";" + stats.IPilots[i].Wave + ";;";
                        else
                            line += ";;;;";
                        if (i < stats.IUpgrades.Count)
                            line += stats.IUpgrades[i].Count + ";" + stats.IUpgrades[i].Gername + ";;";
                        else
                            line += ";;;";
                        if (i < stats.Points.Count)
                            line += stats.Points[i][1] + ";" + stats.Points[i][0] + ";" + PointFormulas[i] + ";;";
                        else
                            line += ";;;;";
                        if (i < stats.FPoints.Count)
                            line += stats.FPoints[i].Count + ";" + stats.FPoints[i].PointsFaction + ";" + FactionFormulas[i] + ";" + stats.ShipsPerFaction[i] + ";" + ShipsPerFactionFormulas[i] + ";;";
                        else
                            line += ";;;;;;";
                        if (i < stats.Waves.Count)
                            line += "Welle " + stats.Waves[i].Name + ";" + stats.Waves[i].Count + ";" + WaveFormulas[i] + ";;";
                        else
                            line += ";;;;";
                        lines.Add(line);
                    }

                    using (StreamWriter f = new StreamWriter(path))
                    {
                        foreach (string s in lines)
                            f.WriteLine(s);
                    }
                }
 

        private void LoadFiles()
        {
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile(squadfilecards, cards);
                wc.DownloadFile(squadfiletranslation, translations);
                wc.DownloadFile(maneuverFiles, maneuver);
            }
            catch
            {
                return;
            }
            if (Directory.Exists(maneuverPath))
                Directory.Delete(maneuverPath, true);
            ZipFile.ExtractToDirectory(maneuver, maneuverPath);
            File.Delete(maneuver);
        }

        private List<int[]> GetManeuvers(List<string> data)
        {
            List<int[]> maneuvers = new List<int[]>();
            foreach (var s in data)
            {
                string t = s.Replace('[', ' ');
                t = t.Replace(']', ' ');
                string[] a = t.Split(',');
                int[] row = new int[a.Length];
                for (int i = 0; i < a.Length; i++)
                    row[i] = Int32.Parse(a[i]);
                maneuvers.Add(row);
            }
            return maneuvers;
        }

        private List<string> GetActions(List<string> data)
        {
            List<string> actions = new List<string>();
            string f;
            foreach (var d in data)
            {
                f = d.Split('\"')[1];
                actions.Add(f);
                //if (f == "Target Lock")
                //    actions.Add("Zielerfassung");
                //else if (f == "Boost")
                //    actions.Add("Schub");
                //else if (f == "Evade")
                //    actions.Add("Ausweichen");
                //else if (f == "Barrel Roll")
                //    actions.Add("Fassrolle");
                //else if (f == "Recover")
                //    actions.Add("Aufladen");
                //else if (f == "Reinforce")
                //    actions.Add("Verstärken");
                //else if (f == "Coordinate")
                //    actions.Add("Koordinieren");
                //else if (f == "Jam")
                //    actions.Add("Störsender");
                //else if (f == "Cloak")
                //    actions.Add("Tarnen");
                //else if (f == "SLAM")
                //    actions.Add("SLAM");
                //else
                //    actions.Add("Fokus");
            }

            return actions;
        }

        private List<string> GetFactions(string data)
        {
            List<string> factions = new List<string>();
            int start = data.IndexOf("\"", 0) + 1;
            int end;
            string f;
            do
            {
                if(start != 0)
                { 
                end = data.IndexOf("\"", start);
                    f = data.Substring(start, end - start);
                }
                else
                {
                    start = data.IndexOf("\'", 0) + 1;
                    end = data.IndexOf("\'", start);
                    f = data.Substring(start, end - start);
                }
                factions.Add(f);
                //if (f == "Rebel Alliance")
                //    factions.Add("Rebellen Allianz");
                //else if (f == "Galactic Empire")
                //    factions.Add("Galaktisches Imperium");
                //else if (f == "Resistance")
                //    factions.Add("Resistance");
                //else if (f == "First Order")
                //    factions.Add("First Order");
                //else
                //    factions.Add("Abschaum & Kriminelle");
                start = data.IndexOf("\"", end + 1) + 1;
            } while (start > 0);

            return factions;
        }

        private List<string> GetSlots(List<string> data)
        {
            List<string> slots = new List<string>();
            string f;
            foreach (var d in data)
            {
                if (d.Contains('\"'))
                    f = d.Split('\"')[1];
                else
                    f = d.Split('\'')[1];
                slots.Add(f);
                //if (f == "Elite")
                //    slots.Add("Elite");
                //else if (f == "Torpedo")
                //    slots.Add("Torpedo");
                //else if (f == "Astromech")
                //    slots.Add("Astromech");
                //else if (f == "Turret")
                //    slots.Add("Geschützturm");
                //else if (f == "Missile")
                //    slots.Add("Rakete");
                //else if (f == "Crew")
                //    slots.Add("Crew");
                //else if (f == "Cannon")
                //    slots.Add("Kanonen");
                //else if (f == "Bomb")
                //    slots.Add("Bombe");
                //else if (f == "System")
                //    slots.Add("System");
                //else if (f == "Hardpoint")
                //    slots.Add("Hardpoint");
                //else if (f == "Cargo")
                //    slots.Add("Fracht");
                //else if (f == "Team")
                //    slots.Add("Team");
                //else if (f == "Illicit")
                //    slots.Add("Illegales");
                //else if (f == "Tech")
                //    slots.Add("Tech");
                //else if (f == "Salvaged Astromech")
                //    slots.Add("Geborgener Astromech");
            }

            return slots;
        }

        private Ship GetShip(string shipName)
        {
            foreach (Ship s in data.Ships)
            {
                if (s.Name == shipName)
                    return s;
            }
            return null;
        }

        public List<Pilot> SearchPilots(string text)
        {
            string[] search = text.Split(' ');
            List<Pilot> pl = new List<Pilot>();
            var str = search[0].ToUpper();
            foreach (var pilot in data.Pilots)
            {
                if (pilot.Name.ToUpper().Contains(str))
                {
                    if (!pl.Contains(pilot))
                        pl.Add(pilot);
                }
                else if (pilot.Text.ToUpper().Contains(str))
                {
                    if (!pl.Contains(pilot))
                        pl.Add(pilot);
                }
                else if (pilot.PilotsFaction.ToUpper().Contains(str))
                {
                    if (!pl.Contains(pilot))
                        pl.Add(pilot);
                }
                else if (pilot.PilotsShip != null && pilot.PilotsShip.Name.ToUpper().Contains(str))
                {
                    if (!pl.Contains(pilot))
                        pl.Add(pilot);
                }
                else
                {
                    foreach (var slot in pilot.Slots)
                    {
                        if (slot.ToUpper().Contains(str))
                        {
                            if (!pl.Contains(pilot))
                                pl.Add(pilot);
                            break;
                        }
                    }
                    if (pilot.PilotsShip != null)
                    {
                        foreach (var act in pilot.PilotsShip.Actions)
                        {
                            if (act.ToUpper().Contains(str))
                            {
                                if (!pl.Contains(pilot))
                                    pl.Add(pilot);
                                break;
                            }
                        }
                    }
                }
            }
            for(int i = 1; i < search.Length; i++)
            {
                str = search[i].ToUpper();
                for(int j = 0; j < pl.Count; j++)
                {
                    var pilot = pl[j];
                    bool ok = false;
                    if (pilot.Name.ToUpper().Contains(str))
                    {
                        ok = true;
                    }
                    else if (pilot.Text.ToUpper().Contains(str))
                    {
                        ok = true;
                    }
                    else if (pilot.PilotsShip != null && pilot.PilotsShip.Name.ToUpper().Contains(str))
                    {
                        ok = true;
                    }
                    else if (pilot.PilotsFaction.ToUpper().Contains(str))
                    {
                        ok = true;
                    }
                    else
                    {
                        foreach (var slot in pilot.Slots)
                        {
                            if (slot.ToUpper().Contains(str))
                            {
                                ok = true;
                                break;
                            }
                        }
                        if (pilot.PilotsShip != null)
                        {
                            foreach (var act in pilot.PilotsShip.Actions)
                            {
                                if (act.ToUpper().Contains(str))
                                {
                                    ok = true;
                                    break;
                                }
                            }
                        }
                    }
                    if(!ok)
                    {
                        pl.Remove(pilot);
                        j--;
                    }
                }
            }
            return pl;
        }

        public List<IUpgrade> SearchUpgrades(string text)
        {
            string[] search = text.Split(' ');
            List<IUpgrade> ul = new List<IUpgrade>();
            var str = search[0].ToUpper();
            foreach (var upgrade in data.IUpgrades)
            {
                if (upgrade.Name.ToUpper().Contains(str))
                {
                    if (!ul.Contains(upgrade))
                        ul.Add(upgrade);
                }
                else if (upgrade.Text.ToUpper().Contains(str))
                {
                    if (!ul.Contains(upgrade))
                        ul.Add(upgrade);
                }
                else if (upgrade.UpgradeSlot.ToUpper().Contains(str))
                {
                    if (!ul.Contains(upgrade))
                        ul.Add(upgrade);
                }
            }
            for (int i = 1; i < search.Length; i++)
            {
                str = search[i].ToUpper();
                for (int j = 0; j < ul.Count; j++)
                {
                    var upgrade = ul[j];
                    bool ok = false;
                    if (upgrade.Name.ToUpper().Contains(str))
                    {
                        ok = true;
                    }
                    else if (upgrade.Text.ToUpper().Contains(str))
                    {
                        ok = true;
                    }
                    else if (upgrade.UpgradeSlot.ToUpper().Contains(str))
                    {
                        ok = true;
                    }
                    if (!ok)
                    {
                        ul.Remove(upgrade);
                        j--;
                    }
                }
            }
            return ul;
        }

        private Data LoadContents(bool update = false, bool silence = false)
        {
            string text;
            string[] lines;
            bool ships = false, bracketStart = false, pilots = false, upgrades = false, modifications= false, titles = false;
            data = new Data();
            List<string> temp = new List<string>();

            using (StreamReader sr = new StreamReader(cards, Encoding.UTF8))
            {
                text = sr.ReadToEnd();
            }

            lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string l = lines[i];
                if (bracketStart)
                {
                    if (l.Contains("]") && !l.Contains("["))
                    {
                        bracketStart = false;
                        temp.Add(l);
                    }
                    else
                        temp.Add(l);
                    continue;
                }
                else if (l.Contains("ships:"))
                    ships = true;
                else if (l.Contains("pilotsById:"))
                {
                    pilots = true;
                    ships = false;
                    data.AddShip(CreateShip(temp));
                    temp = new List<string>();
                }
                else if (l.Contains("upgradesById:"))
                {
                    upgrades = true;
                    pilots = false;
                    data.AddPilot(CreatePilot(temp));
                    temp = new List<string>();
                }
                else if (l.Contains("modificationsById:"))
                {
                    modifications = true;
                    upgrades = false;
                    data.AddIUpgrade(CreateUpgrade(temp));
                    temp = new List<string>();
                }
                else if (l.Contains("titlesById:"))
                {
                    titles = true;
                    modifications = false;
                    data.AddIUpgrade(CreateModification(temp));
                    temp = new List<string>();
                }
                else if(l.Contains("exportObj.setupCardData"))
                {
                    titles = false;
                    data.AddIUpgrade(CreateTitle(temp));
                    temp = new List<string>();
                }
                else if (l.Contains("aka:") || l.Contains("range:") || l.Contains("slot:") || l.Contains("name:") || l.Contains("factions:") || l.Contains("faction:") || l.Contains("attack:") || l.Contains("agility:") || l.Contains("hull:") || l.Contains("shields:") || l.Contains("energy:") || l.Contains("huge:") || l.Contains("epic_points:") || l.Contains("large:") || l.Contains("id:") || l.Contains("unique:") || l.Contains("ship:") || l.Contains("skill:") || l.Contains("points:"))
                    temp.Add(l);
                else if (l.Contains("actions:") || l.Contains("maneuvers:") || l.Contains("slots:"))
                {
                    temp.Add(l);
                    if (!l.Contains(']') && !titles)
                        bracketStart = true;
                }
                else if (l.Contains("}"))
                {
                    if (pilots)
                    {
                        if (temp.Count != 0)
                            data.AddPilot(CreatePilot(temp));
                        temp = new List<string>();
                    }
                    else if(upgrades)
                    {
                        if (temp.Count != 0)
                            data.AddIUpgrade(CreateUpgrade(temp));
                        temp = new List<string>();
                    }
                    else if (modifications)
                    {
                        if (temp.Count != 0)
                            data.AddIUpgrade(CreateModification(temp));
                        temp = new List<string>();
                    }
                    else if (titles)
                    {
                        if (temp.Count != 0)
                            data.AddIUpgrade(CreateTitle(temp));
                        temp = new List<string>();
                    }
                }
                else
                {
                    if (ships)
                    {
                        if (temp.Count != 0)
                            data.AddShip(CreateShip(temp));
                        temp = new List<string>();
                    }
                }
            }
            List<Pilot> pl = data.Pilots.OrderBy(x => x.PilotsFaction).ThenByDescending(x => x.Points).ThenBy(x => x.Name).ToList<Pilot>();
            data.ClearPilots();
            foreach (var p in pl)
                data.AddPilot(p);
            List<Ship> sl = data.Ships.OrderBy(x => x.Name).ToList<Ship>();
            data.ClearShips();
            foreach (var s in sl)
                data.AddShip(s);
            List<IUpgrade> iu= data.IUpgrades.OrderBy(x => x.Points).ThenBy(x => x.Name).ToList<IUpgrade>();
            data.ClearUpgrades();
            foreach (var u in iu)
                data.AddIUpgrade(u);
            return data;
        }

        private Upgrade CreateUpgrade(List<string> temp)
        {
            //    {
            //          name: "R2-D2"
            //          aka: ["R2-D2 (Crew)"]
            //          canonical_name: 'r2d2'
            //          id: 3
            //          unique: true
            //          slot: "Astromech"
            //          points: 4
            //      }

            string name = "", l;
            string aka = "";
            int id = -1;
            string slot = "";
            bool unique = false;
            int points = -1;
            int attack = -1;
            string range = "";
            List<string> sources = null;

            for (int i = 0; i < temp.Count; i++)
            {
                l = temp[i];

                if (l.Contains("name:") && !l.Contains("canonical_name"))
                {
                    if (l.Contains("Dead Man's Switch"))
                    {
                        name = l.Split('\"')[1];
                    }
                    else
                    {
                        try
                        {
                            name = l.Split('\'')[1];
                        }
                        catch
                        {
                            name = l.Split('\"')[1];
                        }
                    }
                }
                else if (l.Contains("aka:"))
                {
                    try
                    {
                        aka = l.Split('\'')[1];
                    }
                    catch
                    {
                        aka = l.Split('\"')[1];
                    }
                }
                else if (l.Contains("id:"))
                    id = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("slot:"))
                {
                    if (slot == "")
                    {
                        try
                        {
                            slot = l.Split('\'')[1];
                        }
                        catch
                        {
                            slot = l.Split('\"')[1];
                        }
                    }
                }
                else if (l.Contains("range:"))
                {
                    try
                    {
                        range = l.Split('\'')[1];
                    }
                    catch
                    {
                        try {
                            range = l.Split('\"')[1];
                        }
                        catch
                        {
                            range = l.Substring(l.IndexOf(':') + 1, 2);
                        }
                    }
                }
                else if (l.Contains("points:"))
                    points = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("attack:"))
                    attack = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("unique:"))
                    unique = true;
            }

            return new Upgrade(name, aka, id, slot, sources, attack, range, unique, points);
        }

        private Modification CreateModification(List<string> temp)
        {
            //    {
            //          name: "R2-D2"
            //          aka: ["R2-D2 (Crew)"]
            //          canonical_name: 'r2d2'
            //          id: 3
            //          unique: true
            //          slot: "Astromech"
            //          points: 4
            //      }

            string name = "", l;
            int id = -1;
            int points = -1;
            List<string> sources = null;
            bool unique = false;

            for (int i = 0; i < temp.Count; i++)
            {
                l = temp[i];

                if (l.Contains("name:") && !l.Contains("canonical_name"))
                {
                        try
                        {
                            name = l.Split('\'')[1];
                        }
                        catch
                        {
                            name = l.Split('\"')[1];
                        }
                }
                else if (l.Contains("id:"))
                    id = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("points:"))
                    points = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("unique:"))
                    unique = true;
            }

            return new Modification(name, id, points, sources, null, unique);
        }

        private Title CreateTitle(List<string> temp)
        {
            //    {
            //          name: "R2-D2"
            //          aka: ["R2-D2 (Crew)"]
            //          canonical_name: 'r2d2'
            //          id: 3
            //          unique: true
            //          slot: "Astromech"
            //          points: 4
            //      }

            string name = "", l;
            int id = -1;
            int points = -1;
            List<string> sources = null;
            bool unique = false;
            Ship ship = null;

            for (int i = 0; i < temp.Count; i++)
            {
                l = temp[i];

                if (l.Contains("name:") && !l.Contains("canonical_name"))
                {
                    if (l.Contains("Dodonna's Pride"))
                        name = "Dodonna's Pride";
                    else if (l.Contains("Jaina's Light"))
                        name = "Jaina's Light";
                    else if (l.Contains("Heavy Scyk") && l.Contains("(Cannon)"))
                        name = "\"Heavy Scyk\" Interceptor (Cannon)";
                    else if (l.Contains("Heavy Scyk") && l.Contains("(Torpedo)"))
                        name = "\"Heavy Scyk\" Interceptor (Torpedo)";
                    else if (l.Contains("Heavy Scyk") && l.Contains("(Missile)"))
                        name = "\"Heavy Scyk\" Interceptor (Missile)";
                    else if (l.Contains("Hound's Tooth"))
                        name = "Hound's Tooth";
                    if (name != "")
                        continue;
                    try
                    {
                        name = l.Split('\'')[1];
                    }
                    catch
                    {
                        name = l.Split('\"')[1];
                    }
                }
                else if (l.Contains("id:"))
                    id = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("points:"))
                    points = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("unique:"))
                    unique = true;
                else if (l.Contains("ship:"))
                {
                    try
                    {
                        ship = GetShip(l.Split('\"')[1]);
                    }
                    catch
                    {
                        ship = GetShip(l.Split('\'')[1]);
                    }
                    
                }
            }

            return new Title(name, id, sources, unique, ship, points);
        }

        private Pilot CreatePilot(List<string> temp)
        {
            //    {
            //    name: "Wedge Antilles"
            //    faction: "Rebel Alliance"
            //    id: 0
            //    unique: true
            //    ship: "X-Wing"
            //    skill: 9
            //    points: 29
            //    slots: [
            //        "Elite"
            //        "Torpedo"
            //        "Astromech"
            //    ]
            //}

            string name = "", l;
            string faction = "";
            int id = -1;
            bool unique = false;
            Ship ship = null;
            int skill = -1;
            int points = -1;
            List<string> slots = new List<string>();

            for (int i = 0; i < temp.Count; i++)
            {
                l = temp[i];

                if (l.Contains("name:") && !l.Contains("canonical_name"))
                {
                    if (l.Contains("Fel's Wrath"))
                        name = "\"Fel's Wrath\"";
                    else if (l.Contains("Etahn A'baht"))
                        name = "\"Etahn A'baht\"";
                    else if (l.Contains("Laetin A'shera"))
                        name = "\"Laetin A'shera\"";
                    else if (l.Contains("N'Dru Suhlak"))
                        name = "\"N'Dru Suhlak\"";
                    else if (l.Contains("Kaa'To Leeachos"))
                        name = "\"Kaa'To Leeachos\"";
                    else
                    {
                        try
                        {
                            name = l.Split('\'')[1];
                        }
                        catch
                        {
                            name = l.Split('\"')[1];
                        }
                    }
                }
                else if (l.Contains("id:"))
                    id = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("skill:"))
                    skill = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("points:"))
                    points = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("ship:"))
                {
                    try {
                        ship = GetShip(l.Split('"')[1]);
                    }
                    catch(Exception e)
                    {
                        ship = GetShip(l.Split('\'')[1]);
                    }
                }
                else if (l.Contains("unique:"))
                    unique = true;
                else if (l.Contains("slots:"))
                {
                    if (l.Contains("]"))
                        slots.Add("No Slots");
                    else
                    {
                        int j = i + 1;
                        List<string> m = new List<string>();
                        while (temp[j].Contains('\"') || temp[j].Contains('\''))
                        {
                            m.Add(temp[j]);
                            j++;
                        }
                        i = j;
                        slots = GetSlots(m);
                    }
                }
                else if (l.Contains("faction:"))
                    faction = GetFactions(l)[0];
            }

            return new Pilot(name, faction, id, unique, ship, skill, points, slots);
        }

        private Ship CreateShip(List<string> temp)
        {
            #region Normal
            //"Aggressor":
            //name: "Aggressor"
            //factions: [ "Scum and Villainy" ]
            //attack: 3
            //agility: 3
            //hull: 4
            //shields: 4
            //actions: [
            //    "Focus"
            //    "Target Lock"
            //    "Boost"
            //    "Evade"
            //]
            //large: true
            //maneuvers: [
            //    [ 0, 0, 0, 0, 0, 0, 0, 0 ]
            //    [ 1, 2, 2, 2, 1, 0, 0, 0 ]
            //    [ 1, 2, 2, 2, 1, 0, 0, 0 ]
            //    [ 0, 2, 2, 2, 0, 0, 3, 3 ]
            //    [ 0, 0, 0, 0, 0, 3, 0, 0 ]
            //]
            #endregion

            #region Nur angekündigt
            //"VCX-100":
            //name: "VCX-100"
            //factions: ["Rebel Alliance"]
            //attack: 4
            //agility: 0
            //hull: 10
            //shields: 6
            //large: true
            //actions: [
            //    "Focus"
            //    "Target Lock"
            //    "Evade"
            //]
            //maneuvers: []
            #endregion

            #region epic
            //"CR90 Corvette (Aft)":
            //name: "CR90 Corvette (Aft)"
            //factions: [ "Rebel Alliance", ]
            //energy: 5
            //agility: 0
            //hull: 8
            //shields: 3
            //actions: [
            //    "Reinforce"
            //    "Recover"
            //]
            //huge: true
            //epic_points: 1.5
            //maneuvers: [
            //    [ 0, 0, 0, 0, 0, 0]
            //    [ 0, 1, 0, 1, 0, 0]
            //    [ 0, 1, 1, 1, 0, 0]
            //    [ 0, 0, 1, 0, 0, 0]
            //    [ 0, 0, 1, 0, 0, 0]
            //]
            //multisection: [
            //    "CR90 Corvette (Fore)".canonicalize()
            //]
            //canonical_name: "CR90 Corvette".canonicalize()
            #endregion

            List<int[]> maneuvers = new List<int[]>();
            List<string> actions = new List<string>();
            List<string> factions = new List<string>();
            string name = "", l;
            int shields = 0, hull = 0, agility = 0, attack = 0, energy = 0;
            double epicPoints = 0.0;
            bool large = false, epic = false, maneuverKnown = true;

            for (int i = 0; i < temp.Count; i++)
            {
                l = temp[i];

                if (l.Contains("name:") && !l.Contains("canonical_name"))
                {
                    try {
                        name = l.Split('\"')[1];
                    }
                    catch(Exception e)
                    {
                        name = l.Split('\'')[1];
                    }
                }
                else if (l.Contains("attack:"))
                    attack = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("agility:"))
                    agility = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("hull:"))
                    hull = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("shields:"))
                    shields = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("energy:"))
                    energy = Int32.Parse(l.Remove(0, l.IndexOf(':') + 1));
                else if (l.Contains("epic_points:"))
                {
                    if (l.Contains('#'))
                    {
                        l = l.Remove(0, l.IndexOf(':') + 1);
                        l = l.Remove(l.IndexOf('#'));
                        epicPoints = Double.Parse(l);
                    }
                    else
                        epicPoints = Double.Parse(l.Remove(0, l.IndexOf(':') + 1));
                }
                else if (l.Contains("large:"))
                    large = true;
                else if (l.Contains("epic:"))
                    epic = true;
                else if (l.Contains("factions:"))
                    factions = GetFactions(l);
                else if (l.Contains("maneuvers:"))
                {
                    if (l.Contains("]"))
                        maneuverKnown = false;
                    else
                    {
                        int j = i + 1;
                        List<string> m = new List<string>();
                        while (temp[j].Contains("]") && temp[j].Contains("["))
                        {
                            m.Add(temp[j]);
                            j++;
                        }
                        i = j;
                        maneuvers = GetManeuvers(m);
                    }
                }
                else if (l.Contains("actions:"))
                {
                    int j = i + 1;
                    List<string> m = new List<string>();
                    while (temp[j].Contains('\"'))
                    {
                        m.Add(temp[j]);
                        j++;
                    }
                    i = j;
                    actions = GetActions(m);
                }
            }

            return new Ship(name, factions, attack, agility, hull, shields, actions, maneuvers, large, epic, energy, epicPoints, maneuverKnown);
        }

        private void Translate()
        {/*
            string text, temp = "";
            string[] lines;
            bool ships = false, pilots = false, upgrades = false, modifications = false, title = false;
            int pos = -1;

            using (StreamReader sr = new StreamReader(translations, Encoding.UTF8))
            {
                text = sr.ReadToEnd();
            }

            lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string l = lines[i];

                if (l.Contains("pilot_translations"))
                    pilots = true;
                else if (l.Contains("upgrade_translation"))
                {
                    pilots = false;
                    upgrades = true;
                }
                else if (l.Contains("modification_translations"))
                {
                    upgrades = false;
                    modifications = true;
                }
                else if (l.Contains("title_translations"))
                {
                    modifications = false;
                    title = true;
                }
                else
                {
                    if (pilots)
                    {
                        if (l.Contains("name:"))
                        {
                            try
                            {
                                temp = l.Split('\'')[1];
                            }
                            catch
                            {
                                temp = l.Split('\"')[1];
                            }
                            data.Pilots[pos].Gername = temp;
                        }
                        else if (l.Contains("ship:"))
                        {
                            data.Pilots[pos].PilotsShip.Gername = l.Split('\"')[1];
                        }
                        else if (l.Contains("text:"))
                        {
                            try
                            {
                                data.Pilots[pos].Text = l.Split('\"')[3];
                            }
                            catch
                            {
                                data.Pilots[pos].Text = l.Split('\'')[3];
                            }
                        }
                        else if (l.Contains('#') || l == "")
                            continue;
                        else
                        {
                            if (l.Contains("Fel's Wrath"))
                                temp = "\"Fel's Wrath\"";
                            else if (l.Contains("Etahn A'baht"))
                                temp = "\"Etahn A'baht\"";
                            else if (l.Contains("Laetin A'shera"))
                                temp = "\"Laetin A'shera\"";
                            else if (l.Contains("N'Dru Suhlak"))
                                temp = "\"N'Dru Suhlak\"";
                            else if (l.Contains("Kaa'To Leeachos"))
                                temp = "\"Kaa'To Leeachos\"";
                            else
                            {
                                try
                                {
                                    temp = l.Split('\'')[1];
                                }
                                catch
                                {
                                    temp = l.Split('\"')[1];
                                }
                            }
                            pos = data.PilotNames.IndexOf(temp);
                        }
                    }
                    else if (upgrades)
                    {
                        if (l.Contains("name:"))
                        {
                            try
                            {
                                temp = l.Split('\'')[1];
                            }
                            catch
                            {
                                temp = l.Split('\"')[1];
                            }
                            data.IUpgrades[pos].Gername = temp;
                        }
                        else if (l.Contains("text:"))
                        {
                            l = l.Replace("%LINEBREAK%", "\n");
                            l = l.Replace("<br />", "\n");
                            if (l.Contains("<span"))
                            {
                                int start = l.IndexOf(">") + 1;
                                int end = l.IndexOf("</span>");
                                int length = end - start;
                                end += 7;
                                data.IUpgrades[pos].Restriction = l.Substring(start, length);
                                l = l.Substring(end, l.Length - end - 3);
                                data.IUpgrades[pos].Text = "<strong>" + data.IUpgrades[pos].Restriction + "</strong>" + l;
                            }
                            else
                            {
                                try
                                {
                                    data.IUpgrades[pos].Text = l.Split('\"')[3];
                                }
                                catch
                                {
                                    data.IUpgrades[pos].Text = l.Split('\'')[3];
                                }
                            }
                        }
                        else if (l.Contains('#') || l == "")
                            continue;
                        else
                        {
                            if (l.Contains("Dead Man's Switch"))
                                temp = "Dead Man's Switch";
                            else
                            {
                                try
                                {
                                    temp = l.Split('\'')[1];
                                }
                                catch
                                {
                                    temp = l.Split('\"')[1];
                                }
                            }
                            pos = data.UpgradeNames.IndexOf(temp);
                        }
                    }
                    else if (modifications)
                    {
                        if (l.Contains("name:"))
                        {
                            try
                            {
                                temp = l.Split('\'')[1];
                            }
                            catch
                            {
                                temp = l.Split('\"')[1];
                            }
                            data.IUpgrades[pos].Gername = temp;
                        }
                        else if (l.Contains("text:"))
                        {
                            l = l.Replace("%LINEBREAK%", "\n");
                            l = l.Replace("<br />", "\n");
                            if (l.Contains("<span"))
                            {
                                int start = l.IndexOf(">") + 1;
                                int end = l.IndexOf("</span>");
                                int length = end - start;
                                end += 7;
                                data.IUpgrades[pos].Restriction = l.Substring(start, length);
                                l = l.Substring(end, l.Length - end - 3);
                                data.IUpgrades[pos].Text = "<strong>" + data.IUpgrades[pos].Restriction + "</strong>" + l;
                            }
                            else
                            {
                                try
                                {
                                    data.IUpgrades[pos].Text = l.Split('\"')[3];
                                }
                                catch
                                {
                                    data.IUpgrades[pos].Text = l.Split('\'')[3];
                                }
                            }
                        }
                        else if (l.Contains('#') || l == "")
                            continue;
                        else
                        {
                            if (l.Contains("Dead Man's Switch"))
                                temp = "Dead Man's Switch";
                            else
                            {
                                try
                                {
                                    temp = l.Split('\'')[1];
                                }
                                catch
                                {
                                    temp = l.Split('\"')[1];
                                }
                            }
                            pos = data.UpgradeNames.IndexOf(temp);
                        }
                    }
                    else if (title)
                    {
                        if (l.Contains("name:"))
                        {
                            try
                            {
                                temp = l.Split('\'')[1];
                            }
                            catch
                            {
                                temp = l.Split('\"')[1];
                            }
                            data.IUpgrades[pos].Gername = temp;
                        }
                        else if (l.Contains("text:"))
                        {
                            l = l.Replace("%LINEBREAK%", "\n");
                            l = l.Replace("<br />", "\n");
                            if (l.Contains("<span"))
                            {
                                int start = l.IndexOf(">") + 1;
                                int end = l.IndexOf("</span>");
                                int length = end - start;
                                end += 7;
                                data.IUpgrades[pos].Restriction = l.Substring(start, length);
                                l = l.Substring(end, l.Length-end-3);
                                data.IUpgrades[pos].Text = "<strong>" + data.IUpgrades[pos].Restriction + "</strong>" + l;
                            }
                            else
                            {
                                try
                                {
                                    data.IUpgrades[pos].Text = l.Split('\"')[3];
                                }
                                catch
                                {
                                    data.IUpgrades[pos].Text = l.Split('\'')[3];
                                }
                            }
                        }
                        else if (l.Contains('#') || l == "")
                            continue;
                        else
                        {
                            
                            if (l.Contains("Dodonna's Pride"))
                                temp = "Dodonna's Pride";
                            else if (l.Contains("Jaina's Light"))
                                temp = "Jaina's Light";
                            else if (l.Contains("Heavy Scyk") && l.Contains("(Cannon)"))
                                temp = "\"Heavy Scyk\" Interceptor (Cannon)";
                            else if (l.Contains("Heavy Scyk") && l.Contains("(Torpedo)"))
                                temp = "\"Heavy Scyk\" Interceptor (Torpedo)";
                            else if (l.Contains("Heavy Scyk") && l.Contains("(Missile)"))
                                temp = "\"Heavy Scyk\" Interceptor (Missile)";
                            else if (l.Contains("Hound's Tooth"))
                                temp = "Hound's Tooth";
                            
                            else
                            {
                                try
                                {
                                    temp = l.Split('\"')[1];
                                }
                                catch
                                {
                                    temp = l.Split('\'')[1];
                                }
                            }
                            pos = data.UpgradeNames.IndexOf(temp);
                        }
                    }
                }
            }*/
        }

    }
}
