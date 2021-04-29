using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LateShiftTool
{
    public class VideoManager
    {
        private List<Video> _videos;
        private Dictionary<string, Video> _videoLUT;
        private const string kVideoDataFilename = "VideoData.xml";

        public VideoManager()
        {
            _videos = new List<Video>();
            _videoLUT = new Dictionary<string, Video>();
        }

        public List<Video> Videos
        {
            get { return _videos; }
            set { _videos = value; }
        }

        public void LoadOverVideoData()
        {
            Video.MovieSection section;
            bool watched;
            string filename;

            XmlDocument doc = new XmlDocument();
            doc.Load(kVideoDataFilename);
            XmlElement rootEl = doc.DocumentElement;

            for (int i = 0; i < rootEl.ChildNodes.Count; i++)
            {
                XmlElement current = rootEl.ChildNodes[i] as XmlElement;

                filename = current.GetAttribute("Filename");
                Video.MovieSection.TryParse(current.GetAttribute("Section"), out section);
                bool.TryParse(current.GetAttribute("Watched"), out watched);

                if (_videoLUT.ContainsKey(filename))
                {
                    Video video = _videoLUT[filename];

                    video.Watched = video.Watched || watched;
                    video.VideoSection = section;
                }
                else
                {
                    Video video = FindExistingVideo(filename);
                    video.Watched = watched;
                    video.VideoSection = section;
                    _videoLUT.Add(video.ShortName, video);
                }
            }
        }

        private Video FindExistingVideo(string shortname)
        {
            foreach (Video video in _videos)
            {
                if (String.Compare(shortname, video.ShortName) == 0)
                    return video;
            }
            return null;
        }

        //public MovieSection CalcVideoSection()
        //{
        //    if (_sectionLUT.ContainsKey(_shortName))
        //        _movieSection = _sectionLUT[_shortName];
        //    else
        //        _movieSection = MovieSection.Unknown;

        //    return _movieSection;
        //}

        public void SaveVideoData()
        {
            // Create the xml document containe
            XmlDocument doc = new XmlDocument();
            // Create the XML Declaration, and append it to XML document
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-16", null);
            doc.AppendChild(dec);// Create the root element

            // Write the top level DB stuff
            XmlElement root = doc.CreateElement("VideoSections");
            doc.AppendChild(root);

            // Write the videodata
            foreach (Video video in _videos)
            {
                XmlElement current = doc.CreateElement("Video");

                string shortname = video.ShortName;

                current.SetAttribute("Filename", video.ShortName);
                current.SetAttribute("Section", video.VideoSection.ToString());
                current.SetAttribute("Watched", video.Watched.ToString());

                root.AppendChild(current);
            }
            doc.Save(kVideoDataFilename);
        }

        //public static void SaveVideoSectionsDEBUG()
        //{
        //    _sectionLUT.Clear();
        //    _sectionLUT.Add("ABDBDBDDBD", MovieSection.MayHotel);
        //    _sectionLUT.Add("ABDBDAAAAA", MovieSection.PoliceStation);

        //    // Create the xml document containe
        //    XmlDocument doc = new XmlDocument();
        //    // Create the XML Declaration, and append it to XML document
        //    XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-16", null);
        //    doc.AppendChild(dec);// Create the root element

        //    // Write the top level DB stuff
        //    XmlElement root = doc.CreateElement("VideoSections");
        //    doc.AppendChild(root);

        //    // Write the list of friendlynames
        //    for (int i = 0; i < _sectionLUT.Count; i++)
        //    {
        //        XmlElement current = doc.CreateElement("Video");

        //        current.SetAttribute("Filename", _sectionLUT.ElementAt(i).Key);
        //        current.SetAttribute("Section", _sectionLUT.ElementAt(i).Value.ToString());

        //        root.AppendChild(current);
        //    }
        //    doc.Save(kFilename);
        //}
    }
}
