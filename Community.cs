using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discussit
{
    internal class Community
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Member> Members { get; set; }
        public List<Post> Posts { get; set; }
        public long MemberCount => Members.Count();
        public long PostCount => Posts.Count();
        private readonly FbData fbd;

        public Community(string name, string description, string leaderUID)
        {
            Name = name;
            Description = description;
            Members = new List<Member>
            {
                new Leader(leaderUID)
            };
            Posts = new List<Post>();
        }

        public Community() 
        {
            fbd = new FbData();
        }


        internal Task SetFbCommunity()
        {
            Task TaskSetDocument = fbd.SetDocument(General.USERS_COLLECTION, string.Empty, out _, GetHashMap());
            return TaskSetDocument;
        }

        private HashMap GetHashMap()
        {
            HashMap hm = new HashMap();
            hm.Put(General.FIELD_USERNAME, Username);
            return hm;
        }
    }
}