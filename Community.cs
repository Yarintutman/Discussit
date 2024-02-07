using Android.App;
using Android.Gms.Tasks;
using Android.Runtime;
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

        private HashMap HashMap
        {
            get 
            {
                HashMap hm = new HashMap();
                hm.Put(General.FIELD_COMMUNITY_NAME, Name);
                hm.Put(General.FIELD_COMMUNITY_DESCRIPTION, Description);
                hm.Put(General.FIELD_DATE, fbd.DateTimeToFirestoreTimestamp(CreationDate));
                return hm;
            }
        }

        public string CollectionPath 
        { 
            get
            {
                return General.COMMUNITIES_COLLECTION + "\\" + Id;
            }
        }

        public Task GetCollectionCount(string cName)
        {
            return fbd.GetCollectionCount(CollectionPath + "\\" + cName);
        }

        private void CreateCommunity()
        {
            fbd.SetDocument(General.COMMUNITIES_COLLECTION, string.Empty, out string communityId, HashMap);
            Id = communityId;
        }

        public void AddPost(string title, string description, string creatorUID)
        {
            Post post = new Post(title, description, creatorUID, CollectionPath);
            fbd.SetDocument(CollectionPath + "\\" + General.POSTS_COLLECTION, string.Empty, out string postId, post.HashMap);
            post.Id = postId;
        }

        public void AddMember(string UID)
        {
            Member member = new Member(UID, CollectionPath);
            fbd.SetDocument(CollectionPath + "\\" + General.MEMBERS_COLLECTION, string.Empty, out string memberId, member.HashMap);
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
                    NewLeader = fbd.GetHighestValue(CollectionPath + "\\" + General.MEMBERS_COLLECTION, General.FIELD_MEMBER_TYPE,
                                                    Application.Context.Resources.GetString(Resource.String.leader), General.FIELD_DATE, 1);
            }
            return NewLeader;
        }

        public void RemovePost(string postID, Member member)
        {
            Post post = Posts.GetPostById(postID);
            if (post != null)
            {
                if (post.CreatorUID == member.UserID || member.GetType() == typeof(Admin) || member.GetType() == typeof(Leader))
                {
                    post.DeletePost();
                    Posts.RemovePost(post);
                } 
            }
        }

        public void DeleteCommunity(Member member) 
        {
            if (member.GetType() == typeof(Leader))
                fbd.DeleteDocument(General.COMMUNITIES_COLLECTION, Id);
        }

        public Task GetPosts()
        {
            return Posts.GetPosts();
        }

        public Task GetMembers()
        {
            return Members.GetMembers();
        }
    }
}