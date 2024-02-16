using Android.App;
using Android.Gms.Tasks;
using Android.Widget;
using Java.Util;
using Kotlin.Reflect;
using Newtonsoft.Json;
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
        public string CreatorName { get; set; }
        public string CommunityPath { get; set; }
        [JsonIgnore]
        public Comments Comments { get; set; }
        public long CommentCount { get; set; }
        public DateTime CreationDate { get; set; }

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

        public Post()
        {
            fbd = new FbData();
        }

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

        [JsonIgnore]
        public string Path
        {
            get
            {
                return CommunityPath + "/" + General.POSTS_COLLECTION + "/" + Id;
            }
        }

        public void CreateComments(Activity context)
        {
            Comments = new Comments(context, Path);
        }

        public void IncrementComments(int value)
        {
            CommentCount += value;
            fbd.IncrementField(CommunityPath + "/", Id, General.FIELD_COMMENT_COUNT, value);
        }

        public Task GetCollectionCount(string cName)
        {
            return fbd.GetCollectionCount(Path + "/" + cName);
        }

        public void AddComment(string description, string creatorUID)
        {
            Comment comment = new Comment(description, creatorUID, Path);
            fbd.SetDocument(Path + "/" + General.COMMENTS_COLLECTION, string.Empty, out string commentId, comment.HashMap);
            comment.Id = commentId;
            IncrementComments(1);

        }

        public Task GetComments()
        {
            return Comments.GetComments();
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
                    IncrementComments(-1);
                }
            }
        }

        public void DeletePost()
        {
            if (CommunityPath != null)
            {
                fbd.DeleteDocument(CommunityPath + "/" + General.POSTS_COLLECTION, Id);
            }
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Post GetPostJson(string json)
        {
            return JsonConvert.DeserializeObject<Post>(json);
        }
    }
}