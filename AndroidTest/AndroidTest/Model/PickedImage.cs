using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AndroidTest.Model
{
    public class PickedImage
    {
        public string EncodedPath { get; set; }
        public string DecodedPath { get; set; }
        public Stream DataStream { get; set; }

        public string FilePath { get; set; }
    }
}
