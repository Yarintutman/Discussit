using Android.App;
using Android.App.AppSearch;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discussit
{
    internal class Community
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Members Members { get; set; }
        public Posts Posts { get; set; }
        public long MemberCount => Members.MemberCount;
        public long PostCount => Posts.PostCount;

        public Community(string name, string description, string leaderUID)
        {
            Name = name;
            Description = description;
        }

        public Community() { }

        public void InitCommunity(Activity context)
        {
            Posts = new Posts(context);
            Posts.AddSnapshotListener(context, GetCollectionPath());
            Members = new Members(context);
            Members.AddSnapshotListener(context, GetCollectionPath());
        }

        public string GetCollectionPath()
        {
            return General.COMMUNITIES_COLLECTION + "\\" + Id;
        }

        public Task GetCollectionCount(string cName)
        {
            FbData fbd = new FbData();
            return fbd.GetCollectionCount(GetCollectionPath() + "\\" + cName);
        }
    }
}