using Android.App;
using Android.Gms.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;

namespace Discussit
{
    /// <summary>
    /// Represents a collection of comments associated with a specific path.
    /// </summary>
    internal class Comments
    {
        private readonly FbData fbd; 
        public string Path { get; private set; }
        public CommentAdapter CommentAdapter { get; } 
        private IListenerRegistration onCollectionChangeListener; 

        /// <summary>
        /// Gets the total number of comments in the collection.
        /// </summary>
        public long CommentsCount => CommentAdapter.Count;

        /// <summary>
        /// Gets the comment at the specified position in the collection.
        /// </summary>
        /// <param name="position">The position of the comment in the collection.</param>
        public Comment this[int position]
        {
            get
            {
                return CommentAdapter[position];
            }
        }

        /// <summary>
        /// Initializes a new instance of the Comments class with the specified context and path.
        /// </summary>
        /// <param name="context">The activity context.</param>
        /// <param name="path">The firestore path associated with the comments collection.</param>
        public Comments(Activity context, string path)
        {
            CommentAdapter = new CommentAdapter(context);
            fbd = new FbData(); 
            Path = path; 
        }

        /// <summary>
        /// Adds a snapshot listener to listen for changes in the comments collection.
        /// </summary>
        /// <param name="context">The activity context.</param>
        public void AddSnapshotListener(Activity context)
        {
            onCollectionChangeListener = fbd.AddSnapshotListener(context, Path + "/" + General.COMMENTS_COLLECTION);
        }

        /// <summary>
        /// Removes the snapshot listener.
        /// </summary>
        public void RemoveSnapshotListener()
        {
            onCollectionChangeListener?.Remove();
        }

        /// <summary>
        /// Adds comments from the provided list of Firestore documents to the comment adapter.
        /// </summary>
        /// <param name="documents">The list of Firestore documents representing comments.</param>
        internal void AddComments(IList<DocumentSnapshot> documents)
        {
            CommentAdapter.Clear(); 
            Comment comment;
            foreach (DocumentSnapshot document in documents)
            {
                comment = new Comment
                {
                    Id = document.Id,
                    Description = document.GetString(General.FIELD_COMMENT_DESCRIPTION),
                    CreatorName = document.GetString(General.FIELD_COMMENT_CREATOR_NAME),
                    CreatorUID = document.GetString(General.FIELD_COMMENT_CREATOR_UID),
                    ParentPath = Path,
                    CreationDate = fbd.FirestoreTimestampToDateTime(document.GetTimestamp(General.FIELD_DATE))
                };
                CommentAdapter.AddComment(comment); 
            }
        }

        /// <summary>
        /// Removes a comment from the comment adapter.
        /// </summary>
        /// <param name="comment">The comment to remove.</param>
        public void RemoveComment(Comment comment)
        {
            CommentAdapter.RemoveComment(comment);
        }

        /// <summary>
        /// Retrieves all comments from the Firestore collection.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal Task GetComments()
        {
            return fbd.GetCollection(Path + "/" + General.COMMENTS_COLLECTION);
        }

        /// <summary>
        /// Retrieves a comment from the comment adapter by its ID.
        /// </summary>
        /// <param name="Id">The ID of the comment to retrieve.</param>
        /// <returns>The comment with the specified ID, or null if not found.</returns>
        public Comment GetCommentById(string Id)
        {
            return CommentAdapter.GetCommentById(Id);
        }
    }
}