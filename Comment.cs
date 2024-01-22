using Android.Gms.Tasks;
using Java.Util;
using static Android.Icu.Text.CaseMap;

namespace Discussit
{
    internal class Comment
    {
        private readonly FbData fbd;
        public string Id { get; set; }
        public string Description { get; set; }
        public string CreatorUID { get; set; }
        public string ParentPath { get; }
        public Comments Comments { get; set; }

        public Comment(string description, string creatorUID, string parentPath)
        {
            fbd = new FbData();
            Description = description;
            CreatorUID = creatorUID;
            ParentPath = parentPath;
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

        public Task GetComments()
        {
            return Comments.GetComments();
        }

        public HashMap GetHashMap()
        {
            HashMap hm = new HashMap();
            hm.Put(General.FIELD_POST_CREATOR, CreatorUID);
            hm.Put(General.FIELD_POST_DESCRIPTION, Description);
            return hm;
        }
    }
}