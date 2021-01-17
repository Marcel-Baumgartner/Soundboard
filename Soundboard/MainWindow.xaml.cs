using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Soundboard
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int outputDev = 0;

        public List<SoundPair> soundpairs = new List<SoundPair>();

        public static string dataLocation = "";
        public static string audioLocation = "";

        public static Dictionary<int, string> instantButtonSounds = new Dictionary<int, string>();

        public MainWindow()
        {
            InitializeComponent();

            audioLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\SoundPadSongs";
            dataLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Soundboard";

            Directory.CreateDirectory(audioLocation);
            Directory.CreateDirectory(dataLocation);

            scanOutputDevices();
            scanFiles();

            if(File.Exists(dataLocation + "\\outputdev.data"))
            {
                int i = Convert.ToInt32(File.ReadAllText(dataLocation + "\\outputdev.data"));

                outputDev = i;

                outputDevLabel.Content = WaveOut.GetCapabilities(outputDev).ProductName;
                outputDevsList.SelectedIndex = i;
            }
            LoadSettings();
        }
        public void scanOutputDevices()
        {
            for (int n = 0; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                outputDevsList.Items.Add(caps.ProductName);
            }
            outputDevsList.SelectedIndex = 0;

            outputDevLabel.Content = WaveOut.GetCapabilities(0).ProductName;
        }
        public void scanFiles()
        {
            string[] files = Directory.GetFiles(audioLocation, "*.ogg");

            foreach(string s in files)
            {
                songList.Items.Add(Path.GetFileName(s));
            }

            if (files.Length > 1)
                songList.SelectedIndex = 0;

            foundFilesLabel.Content = "Found " + songList.Items.Count + " files";
        }
        public void PlayFile(int index)
        {
            WaveOutEvent _pub = null;
            WaveOutEvent _loc = null;

            var vorbis = new NAudio.Vorbis.VorbisWaveReader(audioLocation + "\\" + songList.Items[index]);
            var waveOut = new WaveOutEvent();

            waveOut.DeviceNumber = outputDev;

            waveOut.Init(vorbis);
            waveOut.Play();

            _pub = waveOut;

            if(localPlay.IsChecked.Value)
            {
                var vorbis2 = new NAudio.Vorbis.VorbisWaveReader(audioLocation + "\\" + songList.Items[index]);
                var waveOut2 = new WaveOutEvent();

                waveOut2.DeviceNumber = -1;

                waveOut2.Init(vorbis2);
                waveOut2.Play();

                _loc = waveOut2;
            }

            soundpairs.Add(new SoundPair(_pub, _loc));
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (songList.Items.Count < 1)
                return;

            PlayFile(songList.SelectedIndex);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            outputDev = outputDevsList.SelectedIndex;

            outputDevLabel.Content = WaveOut.GetCapabilities(outputDev).ProductName;

            if(!File.Exists(dataLocation + "\\outputdev.data"))
            {
                StreamWriter fs = File.CreateText(dataLocation + "\\outputdev.data");
                fs.Write(outputDev.ToString());
                fs.Close();
            }
            else
            {
                File.WriteAllText(dataLocation + "\\outputdev.data", outputDev.ToString());
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (SoundPair sp in soundpairs)
                {
                    if(sp.loc != null)
                    {
                        if(sp.loc.PlaybackState == PlaybackState.Playing)
                        {
                            sp.loc.Stop();
                        }
                    }
                    if (sp.pub != null)
                    {
                        if (sp.pub.PlaybackState == PlaybackState.Playing)
                        {
                            sp.pub.Stop();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "An error occured", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            songList.Items.Clear();
            scanFiles();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            InstantButtonSettings settings = new InstantButtonSettings(this, -1);
            settings.ShowDialog();
        }
        public static void RewriteSettings()
        {
            FileStream fs = new FileStream(dataLocation + "\\instantbuttons.data", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach(int i in instantButtonSounds.Keys)
            {
                sw.WriteLine(i.ToString() + ";" + instantButtonSounds[i]);
            }

            sw.Close();
            fs.Close();
        }
        public static void LoadSettings()
        {
            instantButtonSounds.Clear();

            if(!File.Exists(dataLocation + "\\instantbuttons.data"))
            {
                FileStream f = File.Create(dataLocation + "\\instantbuttons.data");
                f.Close();
            }

            FileStream fs = new FileStream(dataLocation + "\\instantbuttons.data", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            while(sr.Peek() != -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(Convert.ToChar(";"));

                int id = Convert.ToInt32(data[0]);

                instantButtonSounds.Add(id, data[1]);
            }
        }

        public void PlayButton(int id)
        {
            if(!instantButtonSounds.ContainsKey(id))
            {
                InstantButtonSettings settings = new InstantButtonSettings(this, id);
                settings.ShowDialog();
                return;
            }
            if(instantButtonSounds.ContainsKey(id))
            {
                string name = instantButtonSounds[id];

                WaveOutEvent _pub = null;
                WaveOutEvent _loc = null;

                var vorbis = new NAudio.Vorbis.VorbisWaveReader(audioLocation + "\\" + name);
                var waveOut = new WaveOutEvent();

                waveOut.DeviceNumber = outputDev;

                waveOut.Init(vorbis);
                waveOut.Play();

                _pub = waveOut;

                if (localPlay.IsChecked.Value)
                {
                    var vorbis2 = new NAudio.Vorbis.VorbisWaveReader(audioLocation + "\\" + name);
                    var waveOut2 = new WaveOutEvent();

                    waveOut2.DeviceNumber = -1;

                    waveOut2.Init(vorbis2);
                    waveOut2.Play();

                    _loc = waveOut2;
                }

                soundpairs.Add(new SoundPair(_pub, _loc));
            }
        }

        private void InstandButton1_Click(object sender, RoutedEventArgs e)
        {
            PlayButton(1);
        }

        private void InstandButton2_Click(object sender, RoutedEventArgs e)
        {
            PlayButton(2);
        }

        private void InstandButton3_Click(object sender, RoutedEventArgs e)
        {
            PlayButton(3);
        }

        private void InstandButton4_Click(object sender, RoutedEventArgs e)
        {
            PlayButton(4);
        }

        private void InstandButton5_Click(object sender, RoutedEventArgs e)
        {
            PlayButton(5);
        }

        private void InstandButton6_Click(object sender, RoutedEventArgs e)
        {
            PlayButton(6);
        }

        private void InstandButton7_Click(object sender, RoutedEventArgs e)
        {
            PlayButton(7);
        }

        private void InstandButton8_Click(object sender, RoutedEventArgs e)
        {
            PlayButton(8);
        }

        private void InstandButton9_Click(object sender, RoutedEventArgs e)
        {
            PlayButton(9);
        }

        private void InstandButton0_Click(object sender, RoutedEventArgs e)
        {
            PlayButton(0);
        }
    }
}
