using Android.App;
using Android.Gms.Tasks;
using Java.Util;
using Newtonsoft.Json;
using System;

namespace Discussit
{
    /// <summary>
    /// Represents a comment Object.
    /// </summary>
    internal class Comment
    {
        private readonly FbData fbd;
        public string Id { get; set; }
        public string Description { get; set; }
        public string CreatorUID { get; set; }
        public string CreatorName { get; set; }
        public string ParentPath { get; set; }
        [JsonIgnore]
        public Comments Comments { get; set; }
        public bool HasComments { get; set; }
        public DateTime CreationDate { get; set; }
        [JsonIgnore]
        public bool HideComments { get; set; }

        /// <summary>
        /// Constructor to create a new comment.
        /// </summary>
        /// <param name="description">The content of the comment.</param>
        /// <param name="creator">The user who created the comment.</param>
        /// <param name="parentPath">The path of the parent to which the comment belongs.</param>
        public Comment(string description, User creator, string parentPath)
        {
            fbd = new FbData();
            Description = description;
            CreatorUID = creator.Id;
            CreatorName = creator.Username;
            ParentPath = parentPath;
            CreationDate = DateTime.Now;
            HasComments = false;
            HideComments = true;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Comment()
        {
            fbd = new FbData();
        }

        /// <summary>
        /// Property to get the comment data as a HashMap.
        /// </summary>
        [JsonIgnore]
        public HashMap HashMap
        {
            get
            {
                // Create a HashMap and store comment data.
                HashMap hm = new HashMap();
                hm.Put(General.FIELD_COMMENT_CREATOR_NAME, CreatorName);
                hm.Put(General.FIELD_COMMENT_CREATOR_UID, CreatorUID);
                hm.Put(General.FIELD_COMMENT_DESCRIPTION, Description);
                hm.Put(General.FIELD_DATE, fbd.DateTimeToFirestoreTimestamp(CreationDate));
                hm.Put(General.FIELD_HAS_COMMENTS, HasComments);
                return hm;
            }
        }

        /// <summary>
        /// Property to get the path of the comment in Firestore.
        /// </summary>
        [JsonIgnore]
        public string Path
        {
            get
            {
                return ParentPath + "/" + General.COMMENTS_COLLECTION + "/" + Id;
            }
        }

        /// <summary>
        /// Method to create sub-comments for this comment.
        /// </summary>
        /// <param name="context">The activity context.</param>
        public void CreateComments(Activity context)
        {
            Comments = new Comments(context, Path);
        }

        /// <summary>
        /// Method to add a new sub-comment to this comment.
        /// </summary>
        /// <param name="description">The content of the sub-comment.</param>
        /// <param name="creator">The user who created the sub-comment.</param>
        public void AddComment(string description, User creator)
        {
            Comment comment = new Comment(description, creator, Path);
            fbd.SetDocument(Path + "/" + General.COMMENTS_COLLECTION, string.Empty, out string commentId, comment.HashMap);
            comment.Id = commentId;
            if (!HasComments)
            {
                HasComments = true;
                fbd.UpdateField(ParentPath + "/" + General.COMMENTS_COLLECTION, Id, General.FIELD_HAS_COMMENTS, HasComments);
            }
        }

        /// <summary>
        /// Method to retrieve all sub-comments.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task GetComments()
        {
            return Comments.GetComments();
        }

        /// <summary>
        /// Method to serialize the comment object to Json string.
        /// </summary>
        /// <returns>A Json representation of the comment.</returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Method to deserialize a Json string to a Comment object.
        /// </summary>
        /// <param name="Json">The Json string to deserialize.</param>
        /// <returns>A Comment object deserialized from the Json string.</returns>
        public static Comment GetCommentJson(string Json)
        {
            return JsonConvert.DeserializeObject<Comment>(Json);
        }
    }
}