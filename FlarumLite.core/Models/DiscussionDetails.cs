using FlarumLite.core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlarumLite.core.Models
{
    public class DiscussionDetails
    {
        public Links links { get; set; }
        public Datum data { get; set; }
        public ObservableCollection<Included> included { get; set; }
    }
}
