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
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace Discussit
{
    internal class Community
    {
        FbData fbd;
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Members Members { get; set; }
        public Posts Posts { get; set; }
        public long MemberCount => Members.MemberCount;
        public long PostCount => Posts.PostCount;

        public Community(string name, string description)
        {
            fbd = new FbData();
            Name = name;
            Description = description;
            CreateCommunity();
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
            return fbd.GetCollectionCount(GetCollectionPath() + "\\" + cName);
        }

        private Task CreateCommunity()
        {
            Task tskCreateCommunity = fbd.SetDocument(General.COMMUNITIES_COLLECTION, string.Empty, out string communityId, GetHashMap());
            Id = communityId;
            return tskCreateCommunity;
        }

        public Task InitPostsCollection()
        {
            Task tskCreatePostsCollection = fbd.SetDocument(GetCollectionPath() + "\\" + General.POSTS_COLLECTION, string.Empty, out _, //add hashmap);
        }

        private HashMap GetHashMap()
        {
            HashMap hm = new HashMap();
            hm.Put(General.FIELD_COMMUNITY_NAME, Name);
            hm.Put(General.FIELD_COMMUNITY_DESCRIPTION, Description);
            return hm;
        }
    }
}