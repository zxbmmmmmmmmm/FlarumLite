using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlarumLite.core.Models
{

    public class NotificationAttributes
    {
        public Included user { get; set; }
        public Included post { get; set; }

        public string contentType { get; set; }
        public object content { get; set; }
        public NotificationContent notificationContent { get; set; }
        public string createdAt { get; set; }
        public bool isRead { get; set; }
    }


    public class FromUser
    {
        public Data data { get; set; }
    }

    public class Subject
    {
        public Data data { get; set; }
    }

    public class NotificationRelationships
    {
        public FromUser fromUser { get; set; }
        public Subject subject { get; set; }
    }

    public class Notification
    {
        public string type { get; set; }
        public string id { get; set; }
        public NotificationAttributes attributes { get; set; }
        public NotificationRelationships relationships { get; set; }
    }


    public class Notifications
    {
        public Links links { get; set; }
        public ObservableCollection<Notification> data { get; set; }
        public ObservableCollection<Included> included { get; set; }
    }
    public class NotificationContent
    {
        public int id { get; set; }
        public int postNumber { get; set; }
        public string identifier { get; set; }
        public string type { get; set; }
        public int enabled { get; set; }
        public string display { get; set; }
    }

}
