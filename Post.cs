using Android.Gms.Tasks;
using Java.Util;
using System;

namespace Discussit
{
    internal class Post
    {
        private readonly FbData fbd;
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatorUID { get; set; }
        public string CommunityPath { get; set; }
        public Comments Comments { get; set; }
        public DateTime CreationDate { get; set; }

        public Post(string title, string desctiption, string creatorUID, string communityPath)
        {
            fbd = new FbData();
            Title = title;
            Description = desctiption;
            CreatorUID = creatorUID;
            CommunityPath = communityPath;
            CreationDate = DateTime.Now;
        }

        public Post() { }

        public string GetPath()
        {
            return CommunityPath + "\\" + General.POSTS_COLLECTION + "\\" + Id;
        }

        public Task GetCollectionCount(string cName)
        {
            return fbd.GetCollectionCount(GetPath() + "\\" + cName);
        }

        public void AddComment(string description, string creatorUID)
        {
            Comment comment = new Comment(description, creatorUID, GetPath());
            fbd.SetDocument(GetPath() + "\\" + General.COMMENTS_COLLECTION, string.Empty, out string commentId, comment.GetHashMap());
            comment.Id = commentId;
        }

        public Task GetComments()
        {
            return Comments.GetComments();
        }

        public HashMap GetHashMap()
        {
            HashMap hm = new HashMap();
            hm.Put(General.FIELD_POST_CREATOR, CreatorUID);
            hm.Put(General.FIELD_POST_TITLE, Title);
            hm.Put(General.FIELD_POST_DESCRIPTION, Description);
            hm.Put(General.FIELD_DATE, fbd.DateTimeToFirestoreTimestamp(CreationDate));
            return hm;
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

        public void DeletePost()
        {
            if (CommunityPath != null)
            {
                fbd.DeleteDocument(CommunityPath + "\\" + General.POSTS_COLLECTION, Id);
            }
        }
    }
}