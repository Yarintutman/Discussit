using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Firebase.Firestore;
using Java.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discussit
{
    /// <summary>
    /// Represents a community within the application.
    /// </summary>
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
        public long PostCount { get; set; }
        /// <summary>
        /// Gets the HashMap representation of the community.
        /// </summary>
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

        /// <summary>
        /// Gets the collection path of the community.
        /// </summary>
        [JsonIgnore]
        public string CollectionPath
        {
            get
            {
                return General.COMMUNITIES_COLLECTION + "/" + Id;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Community class with provided name and description.
        /// </summary>
        /// <param name="name">The name of the community.</param>
        /// <param name="description">The description of the community.</param>
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

        /// <summary>
        /// Initializes a new instance of the Community class from the firebase.
        /// </summary>
        /// <param name="document">The DocumentSnapshot containing the community's data.</param>
        public Community(DocumentSnapshot document)
        {
            fbd = new FbData();
            Id = document.Id;
            Name = document.GetString(General.FIELD_COMMUNITY_NAME);
            Description = document.GetString(General.FIELD_COMMUNITY_DESCRIPTION);
            CreationDate = fbd.FirestoreTimestampToDateTime(document.GetTimestamp(General.FIELD_DATE));
            MemberCount = document.GetLong(General.FIELD_MEMBER_COUNT).LongValue();
            PostCount = document.GetLong(General.FIELD_POST_COUNT).LongValue();
        }

        /// <summary>
        /// Initializes a new instance of the Community class.
        /// </summary>
        public Community()
        {
            fbd = new FbData();
        }

        /// <summary>
        /// Creates the community.
        /// </summary>
        private void CreateCommunity()
        {
            fbd.SetDocument(General.COMMUNITIES_COLLECTION, string.Empty, out string communityId, HashMap);
            Id = communityId;
        }

        /// <summary>
        /// Creates a Members instance for managing members within the community.
        /// </summary>
        /// <param name="context">The context.</param>
        public void CreateMembers(Context context)
        {
            Members = new Members(context, CollectionPath);
        }

        /// <summary>
        /// Creates a Posts instance for managing posts within the community.
        /// </summary>
        /// <param name="context">The context.</param>
        public void CreatePosts(Context context)
        {
            Posts = new Posts(context, CollectionPath);
        }

        /// <summary>
        /// Adds a post to the community.
        /// </summary>
        /// <param name="title">The title of the post.</param>
        /// <param name="description">The description of the post.</param>
        /// <param name="creator">The creator of the post.</param>
        /// <returns>The added post.</returns>
        public Post AddPost(string title, string description, User creator)
        {
            Post post = new Post(title, description, creator, CollectionPath);
            fbd.SetDocument(CollectionPath + "/" + General.POSTS_COLLECTION, string.Empty, out string postId, post.HashMap);
            post.Id = postId;
            PostCount++;
            creator.UpdateArrayField(General.FIELD_USER_POSTS, post.Path);
            fbd.IncrementField(General.COMMUNITIES_COLLECTION, Id, General.FIELD_POST_COUNT, 1);
            return post;
        }

        /// <summary>
        /// Updates a post in the community
        /// </summary>
        /// <param name="post">The post to edit.</param>
        /// <param name="title">The title of the post.</param>
        /// <param name="description">The description of the post.</param>
        /// <returns>The added post.</returns>
        public void UpdatePost(Post post, string title, string description)
        {
            post.Title = title;
            post.Description = description;
            Dictionary<string, Java.Lang.Object> fields = new Dictionary<string, Java.Lang.Object>
            {
                { General.FIELD_POST_TITLE, post.Title },
                { General.FIELD_POST_DESCRIPTION, post.Description }
            };
            fbd.UpdateDocument(post.CommunityPath + "/" + General.POSTS_COLLECTION, post.Id, fields);
        }

        /// <summary>
        /// Adds a member to the community.
        /// </summary>
        /// <param name="user">The user to add as a member.</param>
        public void AddMember(User user)
        {
            Member member;
            if (MemberCount != 0)
                member = new Member(user, CollectionPath);
            else
            { 
                member = new Leader(user, CollectionPath);
                user.UpdateArrayField(General.FIELD_USER_MANAGING_COMMUNITIES, CollectionPath);
            }
            user.UpdateArrayField(General.FIELD_USER_COMMUNITIES, CollectionPath);
            fbd.SetDocument(CollectionPath + "/" + General.MEMBERS_COLLECTION, string.Empty, out string memberId, member.HashMap);
            member.Id = memberId;
            MemberCount++;
            fbd.IncrementField(General.COMMUNITIES_COLLECTION, Id, General.FIELD_MEMBER_COUNT, 1);
        }

        /// <summary>
        /// Kicks a user from the community.
        /// </summary>
        /// <param name="user">The user to remove.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task KickUser(User user)
        {
            Task NewLeader = null;
            Member member = Members.GetMemberByUID(user.Id);
            if (member != null)
            {
                member.LeaveCommunity();
                MemberCount--;
                fbd.IncrementField(General.COMMUNITIES_COLLECTION, Id, General.FIELD_MEMBER_COUNT, -1);
                Members.RemoveMember(member);
                user.RemoveArrayFields(General.FIELD_USER_COMMUNITIES, 
                                       user.Communities.Where<string>(str => str.Contains(Id)).ToList());
                user.RemoveArrayFields(General.FIELD_USER_MANAGING_COMMUNITIES, 
                                       user.ManagingCommunities.Where<string>(str => str.Contains(Id)).ToList());
                if (member.GetType() == typeof(Leader))
                    NewLeader = fbd.GetHighestValue(CollectionPath + "/" + General.MEMBERS_COLLECTION,
                                General.FIELD_MEMBER_TYPE, Application.Context.Resources.GetString(Resource.String.leader),
                                General.FIELD_DATE, 1);
            }
            return NewLeader;
        }

        /// <summary>
        /// Removes a post from the community.
        /// </summary>
        /// <param name="postID">The ID of the post to remove.</param>
        /// <param name="member">The member performing the action.</param>
        public void RemovePost(string postID, Member member)
        {
            Post post = Posts.GetPostById(postID);
            if (post != null)
            {
                if (post.CreatorUID == member.UserID || member.GetType() == typeof(Admin) || member.GetType() == typeof(Leader))
                {
                    post.DeletePost();
                    Posts.RemovePost(post);
                    PostCount--;
                    fbd.IncrementField(General.COMMUNITIES_COLLECTION, Id, General.FIELD_POST_COUNT, -1);
                    fbd.RemoveFromArray(General.USERS_COLLECTION, post.CreatorUID, General.FIELD_USER_POSTS, post.Path);
                } 
            }
        }

        /// <summary>
        /// Saves changes made to the community.
        /// </summary>
        public void SaveChanges()
        {
            Dictionary<string, Java.Lang.Object> fields = new Dictionary<string, Java.Lang.Object>
            {
                { General.FIELD_COMMUNITY_NAME, Name },
                { General.FIELD_COMMUNITY_DESCRIPTION, Description }
            };
            fbd.UpdateDocument(General.COMMUNITIES_COLLECTION, Id, fields);
        }

        /// <summary>
        /// Retrieves posts belonging to the community.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task GetPosts()
        {
            return Posts.GetPosts();
        }

        /// <summary>
        /// Retrieves members of the community.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task GetMembers()
        {
            return Members.GetMembers();
        }

        /// <summary>
        /// Gets the Json representation of the community.
        /// </summary>
        /// <returns>The Json representation of the community.</returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Deserializes a Json string into a Community object.
        /// </summary>
        /// <param name="json">The Json string representing the community.</param>
        /// <returns>The deserialized Community object.</returns>
        public static Community GetCommunityJson(string json)
        {
            return JsonConvert.DeserializeObject<Community>(json);
        }
    }
}