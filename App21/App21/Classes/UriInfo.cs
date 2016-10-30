using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App21.Classes
{
    class UriInfo
    {
        public string UriStr { get; set; }
        public string ModifiedDate { get; set; }
        public string Title { get; set; }
        public UriInfo(string uri, string date, string title)
        {
            this.UriStr = uri;
            this.ModifiedDate = date;
            this.Title = title;
        }
    }
}
