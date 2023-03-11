using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace FlarumLite.core.Models
{


    public class Links
    {
        public string first { get; set; }
        public string next { get; set; }
    }


    public class Data
    {
        public Relationships relationships { get; set; }
        public Attributes attributes { get; set; }
        public string type { get; set; }
        public string id { get; set; }

    }

    public class User
    {
        public Data data { get; set; }
    }

    public class LastPostedUser
    {
        public Data data { get; set; }
    }

    public class Tags
    {
        public ObservableCollection<Datum> data { get; set; }
    }

    public class FirstPost
    {
        public Data data { get; set; }
    }

    public class LastPost
    {
        public Data data { get; set; }
    }

    public class RecipientUsers
    {
        public ObservableCollection<object> data { get; set; }
    }

    public class RecipientGroups
    {
        public ObservableCollection<object> data { get; set; }
    }

    public class Poll
    {
        public Data data { get; set; }
    }

    public class Relationships
    {
        public ObservableCollection<Datum> posts { get; set; }
        public User user { get; set; }
        public LastPostedUser lastPostedUser { get; set; }
        public Tags tags { get; set; }
        public FirstPost firstPost { get; set; }
        public LastPost lastPost { get; set; }
        public RecipientUsers recipientUsers { get; set; }
        public RecipientGroups recipientGroups { get; set; }
        public Poll poll { get; set; }
        public Discussion discussion { get; set; }

    }

    public class Discussion
    {
        public Data data { get; set; }
    }

    public class Datum
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Relationships relationships { get; set; }
        public ObservableCollection<Included> tags { get; set; }

    }
    public class Attributes
    {
        public string color { get; set; }//user+

        public Included discussion { get; set; }//+
        public string userText { get; set; }//user+
        public string contentType { get; set; }//noti +
        //public string content { get; set; }//noti +
        //public string content { get; set; }//noti +
        public bool isRead { get; set; }//noti +

        public string username { get; set; }//user +
        public string displayName { get; set; }//user +
        public string lastSeenAt { get; set; }//user +

        public string bio { get; set; }
        public Attributes user { get; set; }
        public Attributes lastPostedUser { get; set; }

        public string title { get; set; }
        public string slug { get; set; }
        public int? commentCount { get; set; }
        public int? participantCount { get; set; }
        public string createdAt { get; set; }//dt

        public string lastPostedAt { get; set; }//dt
        public int? lastPostNumber { get; set; }
        public bool canReply { get; set; }
        public bool canRename { get; set; }
        public bool canDelete { get; set; }
        public bool canHide { get; set; }
        public bool isApproved { get; set; }
        public object hasBestAnswer { get; set; }//obejct(int+bool)
        public string bestAnswerSetAt { get; set; }//
        public object subscription { get; set; }
        public bool canTag { get; set; }
        public bool hasUpvoted { get; set; }
        public bool hasDownvoted { get; set; }
        public int? votes { get; set; }
        public bool canVote { get; set; }
        public bool isSticky { get; set; }
        public bool canSticky { get; set; }
        public bool frontpage { get; set; }
        public string frontdate { get; set; }//dt
        public bool front { get; set; }
        public bool canSplit { get; set; }
        public bool canMerge { get; set; }
        public bool canSeeReactions { get; set; }

        public int? fof_prevent_necrobumping { get; set; }
        public bool canEditRecipients { get; set; }
        public bool canEditUserRecipients { get; set; }
        public bool canEditGroupRecipients { get; set; }
        public bool canSelectBestAnswer { get; set; }
        public string replyTemplate { get; set; }
        public bool canManageReplyTemplates { get; set; }
        public bool isLocked { get; set; }
        public bool canLock { get; set; }
        public string lastReadAt { get; set; }//dt
        public int? lastReadPostNumber { get; set; }

        //----------Tags----------
        public string name { get; set; }
        public string description { get; set; }
        public string tag_slug { get; set; }
        //public ConsoleColor Color { get; set; }

        public string icon { get; set; }
        public int? discussionCount { get; set; }
        public int? position { get; set; }
        public string tag_lastPostedAt { get; set; }
        public string joinTime { get; set; }
        public string avatarUrl { get; set; }
        public string visited_at { get; set; }//+
        public int? exp { get; set; }//+
        //public string poster_picture { get; set; }//+
        public int? number { get; set; }//+
        //public int? votes { get; set; }//+
        public string contentHtml { get; set; }
        public string contentMD { get; set; }
        //public int? commentCount { get; set; }//+
        //public int? discussionCount { get; set; }//+

        //public string createdAt { get; set; }
        //public DateTime createdAt_dt { get; set; }//dt

        public string editedAt { get; set; }
        public string iconInfo { get; set; }
        public string nameSingular { get; set; }
        public ImageSource avatar { get; set; }
        public Included firstPost { get; set; }
    }


    public class Included
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Relationships relationships { get; set; }
    }

    public class MainPagePosts
    {
        public Links links { get; set; }
        public ObservableCollection<Datum> data { get; set; }
        public ObservableCollection<Included> included { get; set; }
    }

}