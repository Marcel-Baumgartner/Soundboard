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
using System.Windows.Shapes;

namespace Soundboard
{
    /// <summary>
    /// Interaktionslogik für InstantButtonSettings.xaml
    /// </summary>
    public partial class InstantButtonSettings : Window
    {
        public InstantButtonSettings(MainWindow source, int mode)
        {
            InitializeComponent();

            for(int i = 0; i <= 9; i++)
            {
                instantButtonSelect.Items.Add(i);
            }
            foreach(string s in source.songList.Items)
            {
                instantButtonOptions.Items.Add(s);
            }

            if(mode != -1)
            {
                instantButtonSelect.SelectedIndex = mode;
            }
        }

        private void InstantButtonSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MainWindow.instantButtonSounds.ContainsKey(instantButtonSelect.SelectedIndex))
                currentSelectedLabel.Content = "No sound selected";
            else
                currentSelectedLabel.Content = MainWindow.instantButtonSounds[instantButtonSelect.SelectedIndex];
        }

        private void ApplySaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(MainWindow.instantButtonSounds.ContainsKey(instantButtonSelect.SelectedIndex))
            {
                MainWindow.instantButtonSounds[instantButtonSelect.SelectedIndex] = instantButtonOptions.SelectedItem.ToString();
            }
            else
            {
                MainWindow.instantButtonSounds.Add(instantButtonSelect.SelectedIndex, instantButtonOptions.SelectedItem.ToString());
            }
            MainWindow.RewriteSettings();
            MainWindow.LoadSettings();
            this.Close();
        }
    }
}
