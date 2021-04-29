using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LateShiftTool
{
    public partial class Results : Form
    {
        private struct MovieSectionNodes
        {
            public TreeNode parent;
            public TreeNode viewed;
            public TreeNode unviewed;
        }

        private MovieSectionNodes[] _sections;
        private MenuItem _rightClickMenu = new MenuItem("Rename...");
        private ContextMenu _context = new ContextMenu();
        private bool _setupMode = true;
        private VideoManager _videoManager;

        public class VideoNode : TreeNode
        {
            private Video _video;

            public VideoNode(Video video)
            {
                _video = video;
                Text = FriendlyNames.Lookup(video.ShortName); ;
                Name = video.FullPath;
            }

            public Video Video
            {
                get { return _video; }
            }
        }

        public Results(VideoManager videoManager)
        {
            InitializeComponent();
            chkMarkAsViewed.Checked = Properties.Settings.Default.MoveToViewed;

            _sections = new MovieSectionNodes[Enum.GetNames(typeof(Video.MovieSection)).Length];
            int index = 0;

            _context.MenuItems.Add(_rightClickMenu);
            _videoManager = videoManager;

            foreach (Video.MovieSection section in (Video.MovieSection[])Enum.GetValues(typeof(Video.MovieSection)))
            {
                MovieSectionNodes node = new MovieSectionNodes();

                string sectionAsString = section.ToString();

                node.parent = new TreeNode();
                node.parent.Text = sectionAsString;
                node.parent.Name = sectionAsString;

                node.viewed = new TreeNode();
                node.viewed.Text = "Viewed";
                node.viewed.Name = sectionAsString + node.viewed.Text;
                node.unviewed = new TreeNode();
                node.unviewed.Text = "Unviewed";
                node.unviewed.Name = sectionAsString + node.unviewed.Text;

                node.parent.Nodes.Add(node.viewed);
                node.parent.Nodes.Add(node.unviewed);

                _sections[index] = node;
                index++;
            }

            // Add the nodes to the Treeview control
            TreeNode root = new TreeNode();
            root.Text = "Late Shift";
            root.Name = "Late Shift";
            treeView1.Nodes.Add(root);

            for (int i = 0; i < index; i++)
            {
                root.Nodes.Add(_sections[i].parent);
            }

            AddVideos();
        }

        private void AddVideos()
        {
            List<Video> videos = _videoManager.Videos;

            foreach (Video video in videos)
            {
                VideoNode node = new VideoNode(video);

                // Now where does it go?
                MovieSectionNodes parentNode = _sections[(int)video.VideoSection];

                if (video.Watched)
                    parentNode.viewed.Nodes.Add(node);
                else
                    parentNode.unviewed.Nodes.Add(node);

            }
        }

        private void Results_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.MoveToViewed = chkMarkAsViewed.Checked;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.Node is VideoNode)
            {
                VideoNode clickedNode = e.Node as VideoNode;
                Video video = clickedNode.Video;

                //_context.Show(treeView1, e.Location);

                Rename rename = new Rename(video.VideoSection, video.ShortName, FriendlyNames.Lookup(video.ShortName));
                rename.ShowDialog();

                if (rename.DialogResult == DialogResult.OK)
                {
                    FriendlyNames.Update(video.ShortName, rename.NewName);
                    clickedNode.Text = rename.NewName;
                }
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is VideoNode)
            {
                VideoNode clickedNode = e.Node as VideoNode;
                Video video = clickedNode.Video;

                System.Diagnostics.Process.Start(video.FullPath);

                if (_setupMode)
                {
                    Form2 newSectionDlg = new Form2(video.VideoSection, video.ShortName);
                    newSectionDlg.ShowDialog();
                    Video.MovieSection newSection = newSectionDlg.Section;

                    if (video.VideoSection != newSection)
                        video.VideoSection = newSection;
                    if (!String.IsNullOrWhiteSpace(newSectionDlg.NewName))
                    {
                        FriendlyNames.Update(video.ShortName, newSectionDlg.NewName);
                        clickedNode.Text = newSectionDlg.NewName;
                    }
                }

                if (chkMarkAsViewed.Checked)
                {
                    bool videoWasWatched = video.Watched;
                    video.Watched = true;

                    if (!videoWasWatched)
                    {
                        TreeNode unviewed = _sections[(int)video.VideoSection].unviewed;
                        TreeNode viewed = _sections[(int)video.VideoSection].viewed;

                        unviewed.Nodes.Remove(clickedNode);
                        viewed.Nodes.Add(clickedNode);
                        treeView1.Refresh();
                    }
                }
            }
        }
    }
}
