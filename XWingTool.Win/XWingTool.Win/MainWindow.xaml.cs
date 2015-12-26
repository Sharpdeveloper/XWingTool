using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using XWingTool.Core;

namespace XWingTool.Win
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CardController cc;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BuildTreeViewPilots()
        {
            TreeViewPilots.Items.Clear();
            string[,,] pilots = new string[3, cc.data.Ships.Count, cc.data.Pilots.Count];
            pilots[0, 0, 0] = "Rebel Alliance / Resistance";
            pilots[1, 0, 0] = "Galactic Empire / First Order";
            pilots[2, 0, 0] = "Scum and Villainy";
            int[,,] index = new int[3, cc.data.Ships.Count, 1];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < cc.data.Ships.Count; j++)
                {
                    index[i, j, 0] = 1;
                }
            }

            foreach (var ship in cc.data.Ships)
            {
                int f = 0;
                foreach (var faction in ship.Factions)
                {
                    switch (faction)
                    {
                        case "Galactic Empire":
                        case "First Order":
                            f = 1;
                            break;
                        case "Scum and Villainy":
                            f = 2;
                            break;
                    }
                    pilots[f, index[f, 0, 0], 0] = ship.Name;
                    index[f, 0, 0]++;
                }
            }

            foreach (var pilot in cc.data.Pilots)
            {
                int f = 0;
                if (pilot.ShipName == "")
                    continue;
                switch (pilot.PilotsFaction)
                {
                    case "Galactic Empire":
                    case "First Order":
                        f = 1;
                        break;
                    case "Scum and Villainy":
                        f = 2;
                        break;
                }
                int s = 0;
                for (int i = 0; i < cc.data.Ships.Count; i++)
                {
                    if (pilots[f, i, 0] == pilot.ShipName)
                    {
                        s = i;
                        break;
                    }
                }
                pilots[f, s, index[f, s, 0]] = pilot.Name;
                index[f, s, 0]++;
            }

            List<string> ships;
            for (int i = 0; i < 3; i++)
            {
                TreeViewItem tvi = new TreeViewItem();
                switch (i)
                {
                    case 0:
                        tvi.Header = "Rebel Alliance / Resistance";
                        break;
                    case 1:
                        tvi.Header = "Galactic Empire / First Order";
                        break;
                    default:
                        tvi.Header = "Scum and Villainy";
                        break;
                }
                ships = new List<string>();
                for (int j = 1; j < cc.data.Ships.Count; j++)
                {
                    if (pilots[i, j, 0] == null)
                        break;
                    ships.Add(pilots[i, j, 0]);
                }
                ships.Sort();
                foreach (var s in ships)
                {
                    int z = 0;
                    for (int j = 1; j < cc.data.Ships.Count; j++)
                    {
                        if (pilots[i, j, 0] == s)
                        {
                            z = j;
                            break;
                        }
                    }
                    TreeViewItem tvi2 = new TreeViewItem();
                    tvi2.Header = pilots[i, z, 0];
                    List<string> p = new List<string>();
                    for (int k = 1; k < cc.data.Pilots.Count; k++)
                    {
                        if (pilots[i, z, k] == null)
                            break;
                        p.Add(pilots[i, z, k]);
                    }
                    p.Sort();
                    foreach (var p2 in p)
                    {
                        TreeViewItem tvi3 = new TreeViewItem();
                        tvi3.Header = p2;
                        tvi2.Items.Add(tvi3);
                    }
                    tvi.Items.Add(tvi2);
                }
                TreeViewPilots.Items.Add(tvi);
            }

        }

        //Quit MenuItem
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Update Data MenuItem
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            LoadActions();
        }

        //after load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadActions();

        }

        private void LoadActions()
        {
            cc = new CardController();

            //Load
            if (cc.data != null)
            {
                BuildTreeViewPilots();
            }
        }

        private void TreeViewPilots_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            string v = (string)((TreeViewItem)TreeViewPilots.SelectedItem).Header;
            if (v == "Rebel Alliance / Resistance" || v == "Galactic Empire / First Order" || v == "Scum and Villainy")
                return;
            TextBlockPilots.Inlines.Clear();

            int pos = cc.data.PilotNames.IndexOf(v);
            List<string> text = null;
            if (pos > -1)
            {
                text = cc.data.Pilots[pos].GetText();
                if (text != null)
                {
                    int last = text.Count - 1;
                    for (int i = 0; i < last; i++)
                    {
                        if (i % 2 == 0)
                            TextBlockPilots.Inlines.Add(new Run(text[i]) { FontWeight = FontWeights.Bold });
                        else
                            TextBlockPilots.Inlines.Add(text[i]);
                    }
                    if (text[last].Contains(".png"))
                    {
                        string file = System.IO.Path.Combine(cc.maneuverPath, text[last]);
                        if (File.Exists(file))
                        {
                            BitmapImage source = new BitmapImage(new Uri(file));
                            Image image = new Image();
                            image.Source = source;
                            image.Width = 225;
                            image.Height = 160;
                            image.Visibility = Visibility;
                            InlineUIContainer container = new InlineUIContainer(image);
                            TextBlockPilots.Inlines.Add(container);
                        }
                        else
                            TextBlockPilots.Inlines.Add("Maneuver unknown or graphic missing.");
                    }
                    else
                        TextBlockPilots.Inlines.Add(text[last]);
                }
            }
            else
            {
                pos = cc.data.ShipNames.IndexOf(v);
                text = null;
                if (pos > -1)
                {
                    text = cc.data.Ships[pos].GetText();
                }
                if (text != null)
                {
                    int last = text.Count - 1;
                    for (int i = 0; i < last; i++)
                    {
                        if (i % 2 == 0)
                            TextBlockPilots.Inlines.Add(new Run(text[i]) { FontWeight = FontWeights.Bold });
                        else
                            TextBlockPilots.Inlines.Add(text[i]);
                    }
                    if (text[last].Contains(".png"))
                    {
                        string file = System.IO.Path.Combine(cc.maneuverPath, text[last]);
                        if (File.Exists(file))
                        {
                            BitmapImage source = new BitmapImage(new Uri(file));
                            Image image = new Image();
                            image.Source = source;
                            image.Width = 225;
                            image.Height = 160;
                            image.Visibility = Visibility;
                            InlineUIContainer container = new InlineUIContainer(image);
                            TextBlockPilots.Inlines.Add(container);
                        }
                        else
                            TextBlockPilots.Inlines.Add("Maneuver unknown or graphic missing.");
                    }
                    else
                        TextBlockPilots.Inlines.Add(text[last]);
                }
            }

        }
    }

}

