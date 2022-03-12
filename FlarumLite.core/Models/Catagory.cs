using FlarumLite.core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlarumLite.core.Models
{
    public class CatagoryAttributes
    {
        public string name { get; set; }
        public string description { get; set; }
        public string slug { get; set; }
        public string color { get; set; }
        public object backgroundUrl { get; set; }
        public object backgroundMode { get; set; }
        public string icon { get; set; }
        public int discussionCount { get; set; }
        public int? position { get; set; }
        public object defaultSort { get; set; }
        public bool isChild { get; set; }
        public bool isHidden { get; set; }
        public string lastPostedAt { get; set; }
        public bool canStartDiscussion { get; set; }
        public bool canAddToDiscussion { get; set; }
        public object subscription { get; set; }
        public object richExcerpts { get; set; }
        public object excerptLength { get; set; }
        public bool isQnA { get; set; }
        public bool reminders { get; set; }
        public string template { get; set; }
        public int postCount { get; set; }
    }


    public class Children
    {
        public ObservableCollection<Datum> data { get; set; }
    }



    public class LastPostedDiscussion
    {
        public Data data { get; set; }
    }

    public class Parent
    {
        public Data data { get; set; }
    }

    public class CatagoryRelationships
    {
        public Children children { get; set; }
        public LastPostedDiscussion lastPostedDiscussion { get; set; }
        public Parent parent { get; set; }
    }

    public class Catagory
    {
        public string type { get; set; }
        public string id { get; set; }
        public CatagoryAttributes attributes { get; set; }
        public CatagoryRelationships relationships { get; set; }
    }

    public class Catagories
    {
        public ObservableCollection<Catagory> data { get; set; }
        public ObservableCollection<Included> included { get; set; }
    }
}
