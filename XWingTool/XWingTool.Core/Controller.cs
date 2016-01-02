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

        private void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(dataPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, data);
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
                end = data.IndexOf("\"", start);
                f = data.Substring(start, end - start);
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

        private Upgrade ConvertToUpgrade(string data)
        {
            int s = 0;
            int e = data.IndexOf("\n", s); ;
            List<string> pilotvalues = new List<string>();
            do
            {
                pilotvalues.Add(data.Substring(s, e - s));
                s = e + 2;
                e = data.IndexOf("\n", s);
            } while (e > 0);

            string name = "";
            int id = -1;
            string slot = "";
            List<string> sources = new List<string>();
            int attack = -1;
            string range = "";
            bool unique = false;
            int points = -1;

            for (int i = 0; i < pilotvalues.Count; i++)
            {
                string str = pilotvalues[i];
                if (str.Contains(" name"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    if (s == 0 && e == -1)
                    {
                        s = str.IndexOf("\'", 0) + 1;
                        e = str.IndexOf("\'", s);
                    }
                    name = str.Substring(s, e - s);
                }
                else if (str.Contains(" id"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        id = Int32.Parse(str.Substring(s, 3));
                    }
                    catch (ArgumentOutOfRangeException exce)
                    {
                        try
                        {
                            id = Int32.Parse(str.Substring(s, 2));
                        }
                        catch (ArgumentOutOfRangeException excep)
                        {
                            id = Int32.Parse(str.Substring(s, 1));
                        }
                    }
                }
                else if (str.Contains("attack"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    attack = Int32.Parse(str.Substring(s, 1));
                }
                else if (str.Contains("range"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    try
                    {
                        range = str.Substring(s, e - s);
                    }
                    catch (ArgumentException ae)
                    {
                        s = str.IndexOf("\'", 0) + 1;
                        e = str.IndexOf("\'", s);
                        range = str.Substring(s, e - s);
                    }
                }
                else if (str.Contains("sources"))
                {
                    //s = str.IndexOf("[", 0);
                    //e = str.IndexOf("]", s);
                    //sources = GetSources(str.Substring(s, e - s));
                }
                else if (str.Contains("unique"))
                {
                    unique = true;
                }
                else if (str.Contains("points"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        points = Int32.Parse(str.Substring(s, 2));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        points = Int32.Parse(str.Substring(s, 1));
                    }
                }
                else if (str.Contains("slot"))
                {
                    for (int j = i; j < pilotvalues.Count; j++)
                        str += pilotvalues[j];
                    //slot = GetSlots(str)[0];
                }
            }

            return new Upgrade(name, aka, id, slot, sources, attack, range, unique, points);
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

        private Modification ConvertToModification(string data)
        {
            int s = 0;
            int e = data.IndexOf("\n", s); ;
            List<string> pilotvalues = new List<string>();
            do
            {
                pilotvalues.Add(data.Substring(s, e - s));
                s = e + 2;
                e = data.IndexOf("\n", s);
            } while (e > 0);

            string name = "";
            int id = -1;
            List<string> sources = new List<string>();
            int points = -1;
            Ship onlyFor = null;

            for (int i = 0; i < pilotvalues.Count; i++)
            {
                string str = pilotvalues[i];
                if (str.Contains(" name"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    name = str.Substring(s, e - s);
                }
                else if (str.Contains(" id"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        id = Int32.Parse(str.Substring(s, 3));
                    }
                    catch (ArgumentOutOfRangeException exce)
                    {
                        try
                        {
                            id = Int32.Parse(str.Substring(s, 2));
                        }
                        catch (ArgumentOutOfRangeException excep)
                        {
                            id = Int32.Parse(str.Substring(s, 1));
                        }
                    }
                }
                else if (str.Contains("sources"))
                {
                    //s = str.IndexOf("[", 0);
                    //e = str.IndexOf("]", s);
                    //sources = GetSources(str.Substring(s, e - s));
                }
                else if (str.Contains("ship") && !str.Contains("ship.data.") && !str.Contains("restriction_func"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    onlyFor = GetShip(str.Substring(s, e - s));
                }
                else if (str.Contains("points"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        points = Int32.Parse(str.Substring(s, 2));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        points = Int32.Parse(str.Substring(s, 1));
                    }
                }
            }

            return new Modification(name, id, points, sources, onlyFor);
        }

        private Title ConvertToTitles(string data)
        {
            int s = 0;
            int e = data.IndexOf("\n", s); ;
            List<string> pilotvalues = new List<string>();
            do
            {
                pilotvalues.Add(data.Substring(s, e - s));
                s = e + 2;
                e = data.IndexOf("\n", s);
            } while (e > 0);

            string name = "";
            int id = -1;
            List<string> sources = new List<string>();
            int points = -1;
            Ship onlyFor = null;
            bool unique = false;

            for (int i = 0; i < pilotvalues.Count; i++)
            {
                string str = pilotvalues[i];
                if (str.Contains(" name"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    try
                    {
                        name = str.Substring(s, e - s);
                    }
                    catch (ArgumentOutOfRangeException exc)
                    {
                        s = str.IndexOf("\'", 0) + 1;
                        e = str.IndexOf("\'", s);
                        name = str.Substring(s, e - s);
                    }
                }
                else if (str.Contains(" id"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        id = Int32.Parse(str.Substring(s, 3));
                    }
                    catch (ArgumentOutOfRangeException exce)
                    {
                        try
                        {
                            id = Int32.Parse(str.Substring(s, 2));
                        }
                        catch (ArgumentOutOfRangeException excep)
                        {
                            id = Int32.Parse(str.Substring(s, 1));
                        }
                    }
                }
                else if (str.Contains("sources"))
                {
                    //s = str.IndexOf("[", 0);
                    //e = str.IndexOf("]", s);
                    //sources = GetSources(str.Substring(s, e - s));
                }
                else if (str.Contains("ship") && !str.Contains("ship.") && !str.Contains("restriction_func") && !str.Contains("validation_func"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    onlyFor = GetShip(str.Substring(s, e - s));
                }
                else if (str.Contains("unique"))
                {
                    unique = true;
                }
                else if (str.Contains(" points"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        points = Int32.Parse(str.Substring(s, 2));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        points = Int32.Parse(str.Substring(s, 1));
                    }
                }
            }

            return new Title(name, id, sources, unique, onlyFor, points);
        }

        private Data LoadContents(bool update = false, bool silence = false)
        {
            string text;
            string[] lines;
            bool ships = false, bracketStart = false, pilots = false, upgrades = false;
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
                else if (l.Contains("range:") || l.Contains("slot:") || l.Contains("name:") || l.Contains("factions:") || l.Contains("faction:") || l.Contains("attack:") || l.Contains("agility:") || l.Contains("hull:") || l.Contains("shields:") || l.Contains("energy:") || l.Contains("huge:") || l.Contains("epic_points:") || l.Contains("large:") || l.Contains("id:") || l.Contains("unique:") || l.Contains("ship:") || l.Contains("skill:") || l.Contains("points:"))
                    temp.Add(l);
                else if (l.Contains("actions:") || l.Contains("maneuvers:") || l.Contains("slots:"))
                {
                    temp.Add(l);
                    if (!l.Contains(']'))
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
                            data.AddUpgrade(CreateUpgrade(temp));
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
                    else if (pilots)
                    {
                        if (temp.Count != 0)
                            data.AddPilot(CreatePilot(temp));
                        temp = new List<string>();
                    }
                    else if (upgrades)
                    {
                        if (temp.Count != 0)
                            data.AddUpgrade(CreateUpgrade(temp));
                        temp = new List<string>();
                    }
                }
            }
            List<IUpgrade> iu= data.IUpgrades.OrderBy(x => x.Points).ThenBy(x => x.Name).ToList<IUpgrade>();
            data.IUpgrades = new List<IUpgrade>();
            foreach (var u in iu)
                data.IUpgrades.Add(u);
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
                    try
                    {
                        slot = l.Split('\'')[1];
                    }
                    catch
                    {
                        slot = l.Split('\"')[1];
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
                        range = l.Split('\"')[1];
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
                    ship = GetShip(l.Split('"')[1]);
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
                    name = l.Split('\"')[1];
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
        {
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
                            data.PilotGerNames[pos] = temp;
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
                }
            }
        }

    }
}
