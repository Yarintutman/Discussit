using Android.App;
using Android.Gms.Tasks;
using Android.Runtime;
using Android.Util;
using AndroidX.Annotations;
using Java.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.IO;

namespace Discussit
{
    internal class Community
    {
        private readonly FbData fbd;
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public Members Members { get; set; }
        [JsonIgnore]
        public Posts Posts { get; set; }
        public DateTime CreationDate { get; set; }
        public long MemberCount { get; set; }
        public long PostCount {  get; set; }

        public Community(string name, string description)
        {
            fbd = new FbData();
            Name = name;
            Description = description;
            CreationDate = DateTime.Now;
            MemberCount = 0;
            PostCount = 0;
            CreateCommunity();
        }

        public Community() 
        {
            fbd = new FbData();
        }

        [JsonIgnore]
        private HashMap HashMap
        {
            get 
            {
                HashMap hm = new HashMap();
                hm.Put(General.FIELD_COMMUNITY_NAME, Name);
                hm.Put(General.FIELD_COMMUNITY_DESCRIPTION, Description);
                hm.Put(General.FIELD_DATE, fbd.DateTimeToFirestoreTimestamp(CreationDate));
                hm.Put(General.FIELD_MEMBER_COUNT, MemberCount);
                hm.Put(General.FIELD_POST_COUNT, PostCount);
                return hm;
            }
        }

        [JsonIgnore]
        public string CollectionPath 
        { 
            get
            {
                return General.COMMUNITIES_COLLECTION + "/" + Id;
            }
        }

        public Task GetCollectionCount(string cName)
        {
            return fbd.GetCollectionCount(CollectionPath + "/" + cName);
        }

        private void CreateCommunity()
        {
            fbd.SetDocument(General.COMMUNITIES_COLLECTION, string.Empty, out string communityId, HashMap);
            Id = communityId;
        }

        public void CreateMembers(Activity context)
        {
            Members = new Members(context, CollectionPath);
        }

        public void CreatePosts(Activity context)
        {
            Posts = new Posts(context, CollectionPath);
        }

        public Post AddPost(string title, string description, User creator)
        {
            Post post = new Post(title, description, creator, CollectionPath);
            fbd.SetDocument(CollectionPath + "/" + General.POSTS_COLLECTION, string.Empty, out string postId, post.HashMap);
            post.Id = postId;
            PostCount++;
            fbd.IncrementField(General.COMMUNITIES_COLLECTION, Id, General.FIELD_POST_COUNT, 1);
            return post;
        }

        public void AddMember(string UID)
        {
            Member member;
            if (MemberCount == 0)
                member = new Member(UID, CollectionPath);
            else
                member = new Leader(UID, CollectionPath);
            fbd.SetDocument(CollectionPath + "/" + General.MEMBERS_COLLECTION, string.Empty, out string memberId, member.HashMap);
            member.Id = memberId;
            MemberCount++;
            fbd.IncrementField(General.COMMUNITIES_COLLECTION, Id, General.FIELD_MEMBER_COUNT, 1);
        }

        public Task RemoveMember(string UserID)
        {
            Task NewLeader = null;
            Member member = Members.GetMemberByUID(UserID);
            if (member != null)
            {
                member.LeaveCommunity();
                MemberCount--;
                fbd.IncrementField(General.COMMUNITIES_COLLECTION, Id, General.FIELD_MEMBER_COUNT, -1);
                Members.RemoveMember(member);
                if (member.GetType() == typeof(Leader))
                    NewLeader = fbd.GetHighestValue(CollectionPath + "/" + General.MEMBERS_COLLECTION, General.FIELD_MEMBER_TYPE,
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
                    fbd.IncrementField(General.COMMUNITIES_COLLECTION, Id, General.FIELD_POST_COUNT, -1);
                    PostCount--;
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

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Community GetCommunityJson(string json)
        {
            return JsonConvert.DeserializeObject<Community>(json);
        }
    }
}