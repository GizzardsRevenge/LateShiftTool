using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace LateShiftTool
{
    public partial class Form1 : Form
    {
        private VideoManager _videoManager;

        public Form1()
        {
            InitializeComponent();

            txtFilename.Text = Properties.Settings.Default.LogFilename;
            //numMultiplier.Value = Properties.Settings.Default.Multiplier;
            _videoManager = new VideoManager();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.CheckFileExists = true;
            dialog.FileName = txtFilename.Text;

            dialog.ShowDialog();

            txtFilename.Text = dialog.FileName;
        }

        private void txtFilename_TextChanged(object sender, EventArgs e)
        {
            btnGo.Enabled = !String.IsNullOrWhiteSpace(txtFilename.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LogFilename = txtFilename.Text;
            //Properties.Settings.Default.Multiplier = numMultiplier.Value;
            Properties.Settings.Default.Save();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            string logFilename = txtFilename.Text;

            if (!File.Exists(logFilename))
            {
                ShowErrorMessage(@"Logfile does not exist
This app is not smart enough to scan for your SteamApp folder, so you'll have to manually find it.
It will be something like /Program Files/Steam Apps/Common/Late Shift/LateShift_Data/output_log.txt");
                return;
            }

            List<string> watchedVideos = CalculateListOfWatchedMovies(logFilename);
            List<string> jobList = new List<string>();

            if (watchedVideos.Count == 0)
            {
                DialogResult dr = MessageBox.Show("Did not find any watched movies. Proceed anyway?", "No watched movies", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes) 
                    return;
            }

            string videosPath = Path.GetDirectoryName(watchedVideos[0]);
            List<Video> videos = new List<Video>();

            foreach (string f in Directory.GetFiles(videosPath))
            {
                if (f.IndexOf("mp4", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    Video video = new Video(f);
                    videos.Add(video);
                }
            }

            foreach (Video vid in videos)
            {
                foreach (string watchedVideo in watchedVideos)
                {
                    if (string.Equals(vid.FullPath, watchedVideo, StringComparison.OrdinalIgnoreCase))
                    {
                        vid.Watched = true;
                        break;
                    }
                }
            }

            _videoManager.Videos = videos;
            _videoManager.LoadOverVideoData();
            FriendlyNames.Load();

            Results results = new Results(_videoManager);
            results.ShowDialog();

            FriendlyNames.Save();
            _videoManager.SaveVideoData();

            Close();
        }

        private List<string> DEBUG_CalculateListOfWatchedMovies(string logfile)
        {
            List<string> movieFiles = new List<string>();

            movieFiles.Add("D:\\Program Files (x86)\\Steam\\SteamApps\\common\\Late Shift\\Video_test\\BA8BFC17B27F62544A32986C2E00098B.mp4");
            movieFiles.Add("D:\\Program Files (x86)\\Steam\\SteamApps\\common\\Late Shift\\Video_test\\C6E77696223EDEF55427320A60CE2B60.mp4");
            movieFiles.Add("D:\\Program Files (x86)\\Steam\\SteamApps\\common\\Late Shift\\Video_test\\D9C7F3D2245AABF0BAEE9EFF278DF0B9.mp4");

            return movieFiles;
        }

        private List<string> CalculateListOfWatchedMovies(string logfile)
        {
            const string kPrefix = "[AVProVideo] Opening ";
            List<string> movieFiles = new List<string>();

            using (StreamReader sr = File.OpenText(logfile))
            {
                string line;

                while (sr.Peek() >= 0)
                {
                    line = sr.ReadLine();

                    if (line.StartsWith(kPrefix))
                    {
                        line = line.Substring(kPrefix.Length);
                        line = line.Substring(0, FirstSpaceIndex(line));
                        movieFiles.Add(line);
                    }
                }
            }

            RemoveDuplicates(movieFiles);

            return movieFiles;
        }

        private void RemoveDuplicates (List<string> files)
        {
            bool[] deleteMe = new bool[files.Count];

            for (int z = 0; z < files.Count; z++)
                deleteMe[z] = false;

            for (int i = 0; i < files.Count; i++)
            {
                string first = files[i];

                for (int j = i + 1; j < files.Count; j++)
                {
                    string second = files[j];

                    if (String.Compare(first, second) == 0)
                    {
                        deleteMe[j] = true;
                    }
                }
            }

            for (int x = files.Count - 1; x > 0; x--)
            {
                if (deleteMe[x])
                    files.RemoveAt(x);
            }
        }

        // Actually this is finding the space before the offset message.
        private int FirstSpaceIndex (string line)
        {
            return line.IndexOf(" (offset ");
        }

        private void ShowErrorMessage (string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowInfoMessage(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
