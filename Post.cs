using Android.App;
using Android.Gms.Tasks;
using Android.Widget;
using Java.Util;
using Kotlin.Reflect;
using Newtonsoft.Json;
using System;

namespace Discussit
{
    /// <summary>
    /// Represents a post within a community.
    /// </summary>
    internal class Post
    {
        private readonly FbData fbd;
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatorUID { get; set; }
        public string CreatorName { get; set; }
        public string CommunityPath { get; set; }
        [JsonIgnore]
        public Comments Comments { get; set; }
        public long CommentCount { get; set; }
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Constructor to create a new post.
        /// </summary>
        /// <param name="title">Title of the post.</param>
        /// <param name="desctiption">Content of the post.</param>
        /// <param name="creator">User who created the post.</param>
        /// <param name="communityPath">Path of the community to which the post belongs.</param>
        public Post(string title, string desctiption, User creator, string communityPath)
        {
            fbd = new FbData();
            Title = title;
            Description = desctiption;
            CreatorUID = creator.Id;
            CreatorName = creator.Username;
            CommunityPath = communityPath;
            CreationDate = DateTime.Now;
            CommentCount = 0;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Post()
        {
            fbd = new FbData();
        }

        /// <summary>
        /// HashMap representation of the post for Firestore database.
        /// </summary>
        [JsonIgnore]
        public HashMap HashMap
        {
            get
            {
                HashMap hm = new HashMap();
                hm.Put(General.FIELD_POST_CREATOR_NAME, CreatorName);
                hm.Put(General.FIELD_POST_CREATOR_UID, CreatorUID);
                hm.Put(General.FIELD_POST_TITLE, Title);
                hm.Put(General.FIELD_POST_DESCRIPTION, Description);
                hm.Put(General.FIELD_DATE, fbd.DateTimeToFirestoreTimestamp(CreationDate));
                hm.Put(General.FIELD_COMMENT_COUNT, CommentCount);
                return hm;
            }
        }

        /// <summary>
        /// Path of the post in the Firestore database.
        /// </summary>
        [JsonIgnore]
        public string Path
        {
            get
            {
                return CommunityPath + "/" + General.POSTS_COLLECTION + "/" + Id;
            }
        }

        /// <summary>
        /// Creates comments associated with the post.
        /// </summary>
        /// <param name="context">Activity context.</param>
        public void CreateComments(Activity context)
        {
            Comments = new Comments(context, Path);
        }

        /// <summary>
        /// Increments the comment count of the post by the specified value.
        /// </summary>
        /// <param name="value">Value by which to increment the comment count.</param>
        public void IncrementComments(int value)
        {
            CommentCount += value;
            fbd.IncrementField(CommunityPath + "/" + General.POSTS_COLLECTION, Id, General.FIELD_COMMENT_COUNT, value);
        }

        /// <summary>
        /// Retrieves the community containing the post.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning the count of documents in the collection.</returns>
        public Task GetCommunity()
        {
            return fbd.GetDocument(General.COMMUNITIES_COLLECTION, General.CutStringFrom("/", CommunityPath));
        }

        /// <summary>
        /// Adds a new comment to the post.
        /// </summary>
        /// <param name="description">Description/content of the comment.</param>
        /// <param name="creator">User who created the comment.</param>
        public void AddComment(string description, User creator)
        {
            Comment comment = new Comment(description, creator, Path);
            fbd.SetDocument(Path + "/" + General.COMMENTS_COLLECTION, string.Empty, out string commentId, comment.HashMap);
            comment.Id = commentId;
            creator.UpdateArrayField(General.FIELD_USER_COMMENTS, comment.Path);
            IncrementComments(1);
        }

        /// <summary>
        /// Retrieves comments associated with the post.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning the comments associated with the post.</returns>
        public Task GetComments()
        {
            return Comments.GetComments();
        }

        /// <summary>
        /// Removes a comment from the post.
        /// </summary>
        /// <param name="commentID">ID of the comment to be removed.</param>
        /// <param name="member">Member attempting to remove the comment.</param>
        public void RemoveComment(string commentID, Member member)
        {
            Comment comment = Comments.GetCommentById(commentID);
            if (comment != null)
            {
                if (comment.CreatorUID == member.UserID || member.GetType() == typeof(Admin) || member.GetType() == typeof(Leader))
                {
                    comment.DeleteComment();
                    Comments.RemoveComment(comment);
                    IncrementComments(-1);
                }
            }
        }

        /// <summary>
        /// Deletes the post from the community.
        /// </summary>
        public void DeletePost()
        {
            if (CommunityPath != null)
            {
                fbd.DeleteDocument(CommunityPath + "/" + General.POSTS_COLLECTION, Id);
            }
        }

        /// <summary>
        /// Converts the post object to its Json representation.
        /// </summary>
        /// <returns>Json representation of the post.</returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Deserializes Json string to create a post object.
        /// </summary>
        /// <param name="json">Json representation of the post.</param>
        /// <returns>Post object created from the Json string.</returns>
        public static Post GetPostJson(string json)
        {
            return JsonConvert.DeserializeObject<Post>(json);
        }
    }
}