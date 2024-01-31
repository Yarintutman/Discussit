using Android.Gms.Tasks;
using Java.Util;
using System;
using static Android.Icu.Text.CaseMap;

namespace Discussit
{
    internal class Comment
    {
        private readonly FbData fbd;
        public string Id { get; set; }
        public string Description { get; set; }
        public string CreatorUID { get; set; }
        public string ParentPath { get; set; }
        public Comments Comments { get; set; }
        public DateTime CreationDate { get; set; }

        public Comment(string description, string creatorUID, string parentPath)
        {
            fbd = new FbData();
            Description = description;
            CreatorUID = creatorUID;
            ParentPath = parentPath;
            CreationDate = DateTime.Now;
        }

        public Comment() { }

        public string GetPath()
        {
            return ParentPath + "\\" + General.COMMENTS_COLLECTION + "\\" + Id;
        }

        public void AddComment(string description, string creatorUID)
        {
            Comment comment = new Comment(description, creatorUID, GetPath());
            fbd.SetDocument(GetPath() + "\\" + General.COMMENTS_COLLECTION, string.Empty, out string commentId, comment.GetHashMap());
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

        public HashMap GetHashMap()
        {
            HashMap hm = new HashMap();
            hm.Put(General.FIELD_COMMENT_CREATOR, CreatorUID);
            hm.Put(General.FIELD_COMMENT_DESCRIPTION, Description);
            hm.Put(General.FIELD_DATE, fbd.DateTimeToFirestoreTimestamp(CreationDate));
            return hm;
        }

        public void DeleteComment()
        {
            if (ParentPath != null)
            {
                fbd.DeleteDocument(ParentPath + "\\" + General.COMMENTS_COLLECTION, Id);
            }
        }
    }
}