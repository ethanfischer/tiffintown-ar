using System;
using UnityEngine;

namespace TiffinAR.UI
{
    [Serializable]
    public struct BookData
    {
        public string id;
        public string title;
        public string author;
        public string description;
        public Texture2D coverImage;
        public float price;
        public string coverImagePath;
    }
}