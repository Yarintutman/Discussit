using Android.Gms.Tasks;
using Java.Util;


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

        public Post(string title, string desctiption, string creatorUID, string communityPath)
        {
            fbd = new FbData();
            Title = title;
            Description = desctiption;
            CreatorUID = creatorUID;
            CommunityPath = communityPath;
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
            return hm;
        }
    }
}