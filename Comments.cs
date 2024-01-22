using Android.App;
using Android.Gms.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;

namespace Discussit
{
    internal class Comments
    {
        private readonly FbData fbd;
        public string Path { get; private set; }
        public CommentAdapter CommentAdapter { get; }
        private IListenerRegistration onCollectionChangeListener;
        public long CommentsCount => CommentAdapter.Count;

        public Comment this[int position]
        {
            get
            {
                return CommentAdapter[position];
            }
        }

        public Comments(Activity context, string path)
        {
            CommentAdapter = new CommentAdapter(context);
            fbd = new FbData();
            Path = path;
        }

        public void AddSnapshotListener(Activity context)
        {
            onCollectionChangeListener = fbd.AddSnapshotListener(context, Path + "\\" + General.COMMENTS_COLLECTION);
        }

        public void RemoveSnapshotListener()
        {
            onCollectionChangeListener?.Remove();
        }

        internal void AddComments(IList<DocumentSnapshot> documents)
        {
            CommentAdapter.Clear();
            Comment comment;
            foreach (DocumentSnapshot document in documents)
            {
                comment = new Comment
                {
                    //add Comments
                };
                CommentAdapter.AddComment(comment);
            }
        }

        internal Task GetComments()
        {
            return fbd.GetCollection(Path + "\\" + General.COMMENTS_COLLECTION);
        }
    }
}