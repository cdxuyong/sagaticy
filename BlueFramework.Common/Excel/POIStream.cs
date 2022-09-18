using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BlueFramework.Common.Excel
{
    /// <summary>
    /// POI <see cref="Stream"/>
    /// </summary>
    public class POIStream:MemoryStream
    {
        public POIStream()
        {
            AllowClose = true;
        }

        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}
