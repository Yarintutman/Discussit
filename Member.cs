using Android.App.AppSearch;
using Android.Media;
using Java.Util;
using System;

namespace Discussit
{
    internal class Member
    {
        private readonly FbData fbd;
        public string Id { get; set; }
        public string UserID { get; set; }
        public string CommunityPath { get; set; }
        public DateTime JoinDate { get; set; }
        public Member(string userID, string communityPath)
        {
            fbd = new FbData();
            UserID = userID;
            CommunityPath = communityPath;
            JoinDate = DateTime.Now;
        }
        public Member() { }

        public string Path 
        { 
            get
            {
                return CommunityPath + "/" + General.MEMBERS_COLLECTION + "/" + Id;
            } 
        }

        public virtual HashMap HashMap
        {
            get
            {
                HashMap hm = new HashMap();
                hm.Put(General.FIELD_UID, UserID);
                hm.Put(General.FIELD_DATE, fbd.DateTimeToFirestoreTimestamp(JoinDate));
                return hm;
            }
        }

        public void LeaveCommunity()
        {
            if (CommunityPath != null)
            {
                fbd.DeleteDocument(CommunityPath + "/" + General.MEMBERS_COLLECTION, Id);
            }
        }
    }
}