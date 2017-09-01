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
using System.ComponentModel;
using Microsoft.Win32;

namespace XWingTool.Win
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CardController cc;
        private ICollectionView dataView;
        private Statistic stats;
        private ICollectionView dataViewPilots;
        private ICollectionView dataViewShips;
        private ICollectionView dataViewUpgrades;
        private bool english = true;

        public MainWindow()
        {
            InitializeComponent();

            DataGridPilotSearchResult.CanUserSortColumns = false;
            DataGridPilotSearchResult.SelectionMode = DataGridSelectionMode.Single;

            DataGridPilotSearchResult.Columns.Clear();

            DataGridTextColumn dgc;

            dgc = new DataGridTextColumn();
            dgc.Header = "Skill";
            dgc.Binding = new Binding("Skill");
            dgc.IsReadOnly = true;
            DataGridPilotSearchResult.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Name";
            dgc.Binding = new Binding("Name");
            dgc.IsReadOnly = true;
            DataGridPilotSearchResult.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Faction";
            dgc.Binding = new Binding("PilotsFaction");
            dgc.IsReadOnly = true;
            DataGridPilotSearchResult.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Points";
            dgc.Binding = new Binding("Points");
            dgc.IsReadOnly = true;
            DataGridPilotSearchResult.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Ship";
            dgc.Binding = new Binding("ShipName");
            dgc.IsReadOnly = true;
            DataGridPilotSearchResult.Columns.Add(dgc);

            dgc = new DataGridTextColumn();
            dgc.Header = "Points";
            dgc.Binding = new Binding("Points");
            dgc.IsReadOnly = true;
            DataGridUpgradeSearchResult.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Name";
            dgc.Binding = new Binding("Name");
            dgc.IsReadOnly = true;
            DataGridUpgradeSearchResult.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "UpgradeSlot";
            dgc.Binding = new Binding("UpgradeSlot");
            dgc.IsReadOnly = true;
            DataGridUpgradeSearchResult.Columns.Add(dgc);

            dgc = new DataGridTextColumn();
            dgc.Header = "Name";
            dgc.Binding = new Binding("Name");
            dgc.IsReadOnly = true;
            DataGridPilots.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Count";
            dgc.Binding = new Binding("Count");
            dgc.IsReadOnly = true;
            DataGridPilots.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Wave";
            dgc.Binding = new Binding("Wave");
            dgc.IsReadOnly = true;
            DataGridPilots.Columns.Add(dgc);

            dataViewPilots = CollectionViewSource.GetDefaultView(DataGridPilots.ItemsSource);

            dgc = new DataGridTextColumn();
            dgc.Header = "Name";
            dgc.Binding = new Binding("Name");
            dgc.IsReadOnly = true;
            DataGridShips.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Count";
            dgc.Binding = new Binding("Count");
            dgc.IsReadOnly = true;
            DataGridShips.Columns.Add(dgc);

            dataViewShips = CollectionViewSource.GetDefaultView(DataGridShips.ItemsSource);
            
            dgc = new DataGridTextColumn();
            dgc.Header = "Name";
            dgc.Binding = new Binding("Name");
            dgc.IsReadOnly = true;
            DataGridUpgrades.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Count";
            dgc.Binding = new Binding("Count");
            dgc.IsReadOnly = true;
            DataGridUpgrades.Columns.Add(dgc);

            dataViewUpgrades = CollectionViewSource.GetDefaultView(DataGridUpgrades.ItemsSource);
       
        }

        private void BuildTreeViewPilots()
        {
            TreeViewPilots.Items.Clear();
            string[,,] pilots = new string[3, cc.data.Ships.Count / 2, cc.data.Pilots.Count / (cc.data.Ships.Count / 4)];
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
                for (int i = 0; i < cc.data.Ships.Count / 2; i++)
                {
                    if (pilots[f, i, 0] == pilot.ShipName)
                    {
                        s = i;
                        break;
                    }
                }
                if(pilot.Unique)
                    pilots[f, s, index[f, s, 0]] = "*" + pilot.Name + " (" + pilot.Points + ")";
                else
                    pilots[f, s, index[f, s, 0]] = pilot.Name + " (" + pilot.Points + ")";
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
                //ships.Sort();
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
                    //p.Sort();
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

        private void BuildTreeViewUpgrades()
        {
            TreeViewUpgrades.Items.Clear();
            string[,] upgrades = new string[cc.data.UpgradeSlots.Count, cc.data.IUpgrades.Count / (cc.data.UpgradeSlots.Count / 4)];
            cc.data.UpgradeSlots.Sort();
            for (int i = 0; i < cc.data.UpgradeSlots.Count; i++)
            {
                upgrades[i, 0] = cc.data.UpgradeSlots[i];
            }
            int[,] index = new int[cc.data.UpgradeSlots.Count,1];
            for (int i = 0; i < cc.data.UpgradeSlots.Count; i++)
            {
                index[i, 0] = 1;
            }

            foreach (var upgr in cc.data.IUpgrades)
            {
                int f = -1;
                for(int i = 0; i < cc.data.UpgradeSlots.Count; i++)
                {
                    if(upgrades[i,0] == upgr.UpgradeSlot)
                    {
                        f = i;
                        break;
                    }
                }
                if (f != -1 && upgr.Name != "" && !upgr.Name.Contains("Zero"))
                {
                    if(upgr.Unique)
                        upgrades[f, index[f, 0]] = "*" + upgr.Name + " (" + upgr.Points + ")";
                    else
                        upgrades[f, index[f, 0]] = upgr.Name + " (" + upgr.Points + ")";
                    index[f, 0]++;
                }
            }

            List<string> u;
            for (int i = 0; i < cc.data.UpgradeSlots.Count; i++)
            {
                TreeViewItem tvi = new TreeViewItem();
                tvi.Header = upgrades[i, 0];
                u = new List<string>();
                for (int j = 1; j < cc.data.IUpgrades.Count; j++)
                {
                    
                    if (upgrades[i, j] == null)
                        break;
                    u.Add(upgrades[i, j]);
                }
                //u.Sort();
                for (int j = 0; j < u.Count; j++)
                {
                    TreeViewItem tvi2 = new TreeViewItem();
                    tvi2.Header = u[j];
                    tvi.Items.Add(tvi2);
                }
                TreeViewUpgrades.Items.Add(tvi);
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
            WaitDialog wd = new WaitDialog();
            wd.Show();
            cc.ReLoad();
            LoadActions();
            wd.Close();
        }

        //after load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WaitDialog wd = new WaitDialog();
            wd.Show();
            LoadActions();
            wd.Close();
        }

        private void LoadActions()
        {
            cc = new CardController();

            //Load
            if (cc.data != null)
            {
                BuildTreeViewPilots();
                BuildTreeViewUpgrades();
            }
        }

        private void TreeViewPilots_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if (TreeViewPilots.SelectedItem == null)
                return;
            string v = (string)((TreeViewItem)TreeViewPilots.SelectedItem).Header;
            if (v == "Rebel Alliance / Resistance" || v == "Galactic Empire / First Order" || v == "Scum and Villainy")
                return;

            if (v.StartsWith("*"))
                v = v.Substring(1);
            if (v.Contains("("))
                v = v.Remove(v.IndexOf("(") - 1);
            int pos = cc.data.PilotNames.IndexOf(v);
            if (pos > -1)
            {
                DisplayPilot(cc.data.Pilots[pos]);
            }
            else
            {
                pos = cc.data.ShipNames.IndexOf(v);

                if (pos > -1)
                {
                    DisplayShip(cc.data.Ships[pos]);
                }
            }

        }

        private void ButtonPilotSearch_Click(object sender, RoutedEventArgs e)
        {
            PilotSearch();
        }

        private void TextBoxPilotSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Return)
                PilotSearch();
        }

        private void PilotSearch()
        {
            List<Pilot> pl = cc.SearchPilots(this.TextBoxPilotSearch.Text);
            DataGridPilotSearchResult.ItemsSource = null;
            DataGridPilotSearchResult.ItemsSource = pl;
            dataView = CollectionViewSource.GetDefaultView(DataGridPilotSearchResult.ItemsSource);
            dataView.SortDescriptions.Clear();

            dataView.SortDescriptions.Add(new SortDescription("Skill", ListSortDirection.Descending));
            dataView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            dataView.Refresh();
        }

        private void DataGridPilotSearchResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayPilot((Pilot)DataGridPilotSearchResult.SelectedItem);
        }

        private void DisplayPilot(Pilot p)
        {
            if (p != null)
            {
                List<string> text = p.GetText();
                if (text != null)
                {
                    TextBlockPilots.Inlines.Clear();
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

        private void DisplayShip(Ship s)
        {
            List<string> text = s.GetText();
            if (text != null)
            {
                TextBlockPilots.Inlines.Clear();
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

        private void DisplayUpgrade(IUpgrade u)
        {
            if (u == null)
                return;
            List<string> text = u.GetText();
            if (text != null)
            {
                TextBlockUpgrades.Inlines.Clear();
                int last = text.Count - 1;
                for (int i = 0; i < last; i++)
                {
                    if (i % 2 == 0)
                        TextBlockUpgrades.Inlines.Add(new Run(text[i]) { FontWeight = FontWeights.Bold });
                    else
                        TextBlockUpgrades.Inlines.Add(text[i]);
                }
                TextBlockUpgrades.Inlines.Add(text[last]);
            }
        }

        private void TextBoxUpgradeSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                UpgradeSearch();
        }

        private void ButtonUpgradeSearch_Click(object sender, RoutedEventArgs e)
        {
            UpgradeSearch();
        }

        private void TreeViewUpgrades_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if (TreeViewUpgrades.SelectedItem == null)
                return;
            string v = (string)((TreeViewItem)TreeViewUpgrades.SelectedItem).Header;
            foreach(var slot in cc.data.UpgradeSlots)
            {
                if (v == slot)
                    return;
            }
            if (v.StartsWith("*"))
                v = v.Substring(1);
            if (v.Contains("("))
                v = v.Remove(v.IndexOf("(") - 1);
            int pos = cc.data.UpgradeNames.IndexOf(v);
            if (pos > -1)
            {
                DisplayUpgrade(cc.data.IUpgrades[pos]);
            }
        }

        private void DataGridUpgradeSearchResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayUpgrade((IUpgrade)DataGridUpgradeSearchResult.SelectedItem);
        }

        private void UpgradeSearch()
        {
            List<IUpgrade> ul = cc.SearchUpgrades(this.TextBoxUpgradeSearch.Text);
            DataGridUpgradeSearchResult.ItemsSource = null;
            DataGridUpgradeSearchResult.ItemsSource = ul;
            dataView = CollectionViewSource.GetDefaultView(DataGridUpgradeSearchResult.ItemsSource);
            dataView.SortDescriptions.Clear();

            dataView.SortDescriptions.Add(new SortDescription("Points", ListSortDirection.Descending));
            dataView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            dataView.Refresh();
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            if (cc.stats == null)
                return;
            if (TextBoxURL.Text.Contains(@"geordanr.github.io/xwing"))
            {
                cc.stats.Parse(TextBoxURL.Text);
                Refresh();
                TextBoxURL.Text = "";
            }
            else
            {
                MessageBox.Show("Only YASB links allowed.");
            }
        }

        private void Refresh()
        {
            DataGridPilots.ItemsSource = null;
            DataGridPilots.ItemsSource = cc.stats.IPilots;
            dataViewPilots = CollectionViewSource.GetDefaultView(DataGridPilots.ItemsSource);
            dataViewPilots.SortDescriptions.Clear();
            dataViewPilots.SortDescriptions.Add(new System.ComponentModel.SortDescription("Count", System.ComponentModel.ListSortDirection.Descending));
            dataViewPilots.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            

            DataGridShips.ItemsSource = null;
            DataGridShips.ItemsSource = cc.stats.IShips;
            dataViewShips = CollectionViewSource.GetDefaultView(DataGridShips.ItemsSource);
            dataViewShips.SortDescriptions.Clear();
            dataViewShips.SortDescriptions.Add(new System.ComponentModel.SortDescription("Count", System.ComponentModel.ListSortDirection.Descending));
            dataViewShips.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));

            DataGridUpgrades.ItemsSource = null;
            DataGridUpgrades.ItemsSource = cc.stats.IUpgrades;
            dataViewUpgrades = CollectionViewSource.GetDefaultView(DataGridUpgrades.ItemsSource);
            dataViewUpgrades.SortDescriptions.Clear();
            dataViewUpgrades.SortDescriptions.Add(new System.ComponentModel.SortDescription("Count", System.ComponentModel.ListSortDirection.Descending));
            dataViewUpgrades.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            if (cc.stats == null)
                return;
            if (TextBoxURL.Text.Contains(@"geordanr.github.io/xwing"))
            {
                if (cc.stats.IPilots.Count == 0)
                {
                    MessageBox.Show("There is no data.Nothing can be removed.");
                }
                try
                {
                    cc.stats.Parse(TextBoxURL.Text, false);
                }
                catch (ArgumentException aex)
                {
                    MessageBox.Show("This squad wasn't added, so it can't be removed.");
                }
                Refresh();
                TextBoxURL.Text = "";
            }
            else
            {
                MessageBox.Show("Only YASB links allowed.");
            }
        }

        private void SaveStats_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Stats-Files | *.xwstats";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
                cc.SaveStats(dlg.FileName);
        }

        private void LoadStats_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Stats-Files | *.xwstats";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
                cc.LoadStats(dlg.FileName);
            Refresh();
        }

        private void ImportLists_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text-Files | *.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
                cc.ImportLists(dlg.FileName);
            Refresh();
        }

        private void ExportStats_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Excel-Files | *.csv";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
                cc.ExportStats(dlg.FileName);
        }
    }

}