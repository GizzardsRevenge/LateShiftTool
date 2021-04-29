using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace LateShiftTool
{
    public static class FriendlyNames
    {
        const string kFilename = "FriendlyNamesLUT.xml"; 
        static Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        //public FriendlyNames()
        //{
        //    _dictionary = new Dictionary<string, string>();
        //}

        public static void Load()
        {
            _dictionary.Clear();

            XmlDocument doc = new XmlDocument();
            doc.Load(kFilename);
            XmlElement rootEl = doc.DocumentElement;

            for (int i = 0; i < rootEl.ChildNodes.Count; i++)
            {
                XmlElement current = rootEl.ChildNodes[i] as XmlElement;

                _dictionary.Add(current.GetAttribute("Filename"), current.GetAttribute("Friendly"));
            }
        }

        public static void Save()
        {
            // Create the xml document containe
            XmlDocument doc = new XmlDocument();
            // Create the XML Declaration, and append it to XML document
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-16", null);
            doc.AppendChild(dec);// Create the root element

            // Write the top level DB stuff
            XmlElement root = doc.CreateElement("FriendlyNames");
            doc.AppendChild(root);

            // Write the list of friendlynames
            for (int i = 0; i < _dictionary.Count; i++)
            {
                XmlElement current = doc.CreateElement("FriendlyName");

                current.SetAttribute("Filename", _dictionary.ElementAt(i).Key);
                current.SetAttribute("Friendly", _dictionary.ElementAt(i).Value);

                root.AppendChild(current);
            }
            doc.Save(kFilename);
        }

        public static string Lookup(string filenameWithoutExtension)
        {
            if (_dictionary.ContainsKey(filenameWithoutExtension))
                return _dictionary[filenameWithoutExtension];
            else
                return filenameWithoutExtension;
        }

        public static bool Exists(string filenameWithoutExtension)
        {
            return _dictionary.ContainsKey(filenameWithoutExtension);
        }

        public static void Update (string originalName, string newName)
        {
            if (_dictionary.ContainsKey(originalName))
                _dictionary[originalName] = newName;
            else
                _dictionary.Add(originalName, newName);
        }
    }
}
