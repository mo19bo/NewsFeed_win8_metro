using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace App21.Classes
{
    class Spider
    {
        public Spider(string baseUri,List<string> keyWords, int timeSpan)
        {
            this.BaseUri = baseUri;
            this.KeyWords = keyWords;
            this.TimeSpan = timeSpan;
        }

        public string BaseUri { get; set; }
        public List<string> KeyWords { get; set; }
        public int TimeSpan { get; set; }
    } 
}
