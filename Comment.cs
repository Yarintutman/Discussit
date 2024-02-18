using Android.App;
using Android.Gms.Tasks;
using Java.Util;
using Newtonsoft.Json;
using System;

namespace Discussit
{
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
        public DateTime CreationDate { get; set; }

        public Comment(string description, User creator, string parentPath)
        {
            fbd = new FbData();
            Description = description;
            CreatorUID = creator.Id;
            CreatorName = creator.Username;
            ParentPath = parentPath;
            CreationDate = DateTime.Now;
        }

        public Comment()
        {
            fbd = new FbData();
        }

        [JsonIgnore]
        public HashMap HashMap 
        { 
            get 
            {
                HashMap hm = new HashMap();
                hm.Put(General.FIELD_COMMENT_CREATOR_NAME, CreatorName);
                hm.Put(General.FIELD_COMMENT_CREATOR_UID, CreatorUID);
                hm.Put(General.FIELD_COMMENT_DESCRIPTION, Description);
                hm.Put(General.FIELD_DATE, fbd.DateTimeToFirestoreTimestamp(CreationDate));
                return hm;
            }
        }

        [JsonIgnore]
        public string Path
        {
            get
            {
                return ParentPath + "/" + General.COMMENTS_COLLECTION + "/" + Id;
            }
        }

        public void CreateComments(Activity context)
        {
            Comments = new Comments(context, Path);
        }

        public void AddComment(string description, User creator)
        {
            Comment comment = new Comment(description, creator, Path);
            fbd.SetDocument(Path + "/" + General.COMMENTS_COLLECTION, string.Empty, out string commentId, comment.HashMap);
            comment.Id = commentId;
        }

        public void RemoveComment(string commentID, Member member)
        {
            Comment comment = Comments.GetCommentById(commentID);
            if (comment != null)
            {
                if (comment.CreatorUID == member.UserID || member.GetType() == typeof(Admin) || member.GetType() == typeof(Leader))
                {
                    comment.DeleteComment();
                    Comments.RemoveComment(comment);
                }
            }
        }

        public Task GetComments()
        {
            return Comments.GetComments();
        }

        public void DeleteComment()
        {
            if (ParentPath != null)
            {
                fbd.DeleteDocument(ParentPath + "/" + General.COMMENTS_COLLECTION, Id);
            }
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Comment GetCommentJson(string json)
        {
            return JsonConvert.DeserializeObject<Comment>(json);
        }
    }
}