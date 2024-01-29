using Android.App;
using Android.Gms.Tasks;
using Java.Util;
using Newtonsoft.Json.Bson;
using System;

namespace Discussit
{
    internal class Community
    {
        private readonly FbData fbd;
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Members Members { get; set; }
        public Posts Posts { get; set; }
        public DateTime CreationDate { get; set; }
        public long MemberCount => Members.MemberCount;
        public long PostCount => Posts.PostCount;

        public Community(string name, string description)
        {
            fbd = new FbData();
            Name = name;
            Description = description;
            CreationDate = DateTime.Now;
            CreateCommunity();
        }

        public Community() { }

        public string GetCollectionPath()
        {
            return General.COMMUNITIES_COLLECTION + "\\" + Id;
        }

        public Task GetCollectionCount(string cName)
        {
            return fbd.GetCollectionCount(GetCollectionPath() + "\\" + cName);
        }

        private void CreateCommunity()
        {
            fbd.SetDocument(General.COMMUNITIES_COLLECTION, string.Empty, out string communityId, GetHashMap());
            Id = communityId;
        }

        public void AddPost(string title, string description, string creatorUID)
        {
            Post post = new Post(title, description, creatorUID, GetCollectionPath());
            fbd.SetDocument(GetCollectionPath() + "\\" + General.POSTS_COLLECTION, string.Empty, out string postId, post.GetHashMap());
            post.Id = postId;
        }

        public void AddMember(string UID)
        {
            Member member = new Member(UID, GetCollectionPath());
            fbd.SetDocument(GetCollectionPath() + "\\" + General.MEMBERS_COLLECTION, string.Empty, out string memberId, member.GetHashMap());
            member.Id = memberId;
        }

        public Task RemoveMember(string UserID)
        {
            Task NewLeader = null;
            Member member = Members.GetMemberByUID(UserID);
            if (member != null)
            {
                member.LeaveCommunity();
                Members.RemoveMember(member);
                if (member.GetType() == typeof(Leader))
                    NewLeader = fbd.GetHighestValue(GetCollectionPath() + "\\" + General.MEMBERS_COLLECTION, General.FIELD_MEMBER_TYPE,
                                                    Application.Context.Resources.GetString(Resource.String.leader), General.FIELD_DATE, 1);
            }
            return NewLeader;
        }

        public Task GetPosts()
        {
            return Posts.GetPosts();
        }

        public Task GetMembers()
        {
            return Members.GetMembers();
        }

        private HashMap GetHashMap()
        {
            HashMap hm = new HashMap();
            hm.Put(General.FIELD_COMMUNITY_NAME, Name);
            hm.Put(General.FIELD_COMMUNITY_DESCRIPTION, Description);
            hm.Put(General.FIELD_DATE, fbd.DateTimeToFirestoreTimestamp(CreationDate));
            return hm;
        }
    }
}