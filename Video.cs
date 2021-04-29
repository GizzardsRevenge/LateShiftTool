using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace LateShiftTool
{
    public class Video 
    {
        public enum MovieSection
        {
            CarPark,
            HeistPlanning,
            TheAuction,
            VanCrash,
            ChineseRestaurant,
            TheStreets,
            PoliceStation,
            MayHotel,
            TchoisHotelCaptured,
            Hospital,
            Hainsworths,
            ParrsHouse,
            TchoisHotelEndings,
            Other
        }

        private string _shortName;
        private string _fileName;
        private MovieSection _movieSection;
        private bool _watched;

        public Video(string fileName)
        {
            _fileName = fileName;
            _shortName = Path.GetFileNameWithoutExtension(fileName);
            _watched = false;
            _movieSection = Video.MovieSection.Other;
        }

        public MovieSection VideoSection
        {
            get { return _movieSection; }
            set { _movieSection = value; }
        }

        public string FullPath
        {
            get { return _fileName; }
        }

        public string ShortName
        {
            get { return _shortName; }
        }

        public bool Watched
        {
            get { return _watched; }
            set { _watched = value; }
        }
    }
}