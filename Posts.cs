using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;

namespace Discussit
{
    /// <summary>
    /// Represents a collection of posts associated with a specific path in the database.
    /// </summary>
    internal class Posts
    {
        private readonly FbData fbd;
        public string Path { get; private set; }
        public PostAdapter PostAdapter { get; }
        private IListenerRegistration onCollectionChangeListener;
        public long PostCount => PostAdapter.Count;

        public Post this[int position]
        {
            get
            {
                return PostAdapter[position];
            }
        }
        /// <summary>
        /// Initializes a new instance of the Posts class with the specified context and path.
        /// </summary>
        /// <param name="context">The context in which the Posts instance is being used.</param>
        /// <param name="path">The path to the collection of posts in the database.</param>
        public Posts(Context context, string path)
        {
            PostAdapter = new PostAdapter(context);
            fbd = new FbData();
            Path = path;
        }

        /// <summary>
        /// Adds a snapshot listener to the collection of posts.
        /// </summary>
        /// <param name="context">The activity context to use for the snapshot listener.</param>
        public void AddSnapshotListener(Activity context)
        {
            onCollectionChangeListener = fbd.AddSnapshotListener(context, Path + "/" + General.POSTS_COLLECTION);
        }

        /// <summary>
        /// Removes the snapshot listener from the collection of posts.
        /// </summary>
        public void RemoveSnapshotListener()
        {
            onCollectionChangeListener?.Remove();
        }

        /// <summary>
        /// Adds posts to the collection based on the provided list of document snapshots.
        /// </summary>
        /// <param name="documents">The list of document snapshots representing the posts to add.</param>
        internal void AddPosts(IList<DocumentSnapshot> documents)
        {
            PostAdapter.Clear();
            Post post;
            foreach (DocumentSnapshot document in documents)
            {
                post = new Post
                {
                    Id = document.Id,
                    Title = document.GetString(General.FIELD_POST_TITLE),
                    Description = document.GetString(General.FIELD_POST_DESCRIPTION),
                    CreatorUID = document.GetString(General.FIELD_POST_CREATOR_UID),
                    CreatorName = document.GetString(General.FIELD_POST_CREATOR_NAME),
                    CommentCount = document.GetLong(General.FIELD_COMMENT_COUNT).LongValue(),
                    CommunityPath = Path,
                    CreationDate = fbd.FirestoreTimestampToDateTime(document.GetTimestamp(General.FIELD_DATE))
                };
                PostAdapter.AddPost(post);
            }
        }

        /// <summary>
        /// Removes the specified post from the collection.
        /// </summary>
        /// <param name="post">The post to remove.</param>
        public void RemovePost(Post post)
        {
            PostAdapter.RemovePost(post);
        }

        /// <summary>
        /// Retrieves the posts from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation of getting the posts.</returns>
        internal Task GetPosts()
        {
            return fbd.GetCollection(Path + "/" + General.POSTS_COLLECTION);
        }

        /// <summary>
        /// Retrieves the post with the specified ID from the collection.
        /// </summary>
        /// <param name="Id">The ID of the post to retrieve.</param>
        /// <returns>The post with the specified ID, or null if not found.</returns>
        public Post GetPostById(string Id)
        {
            return PostAdapter.GetPostById(Id);
        }

        /// <summary>
        /// Searches for posts based on the specified search criteria.
        /// </summary>
        /// <param name="search">The search criteria.</param>
        public void Search(string search)
        {
            PostAdapter.Search(search);
        }

        /// <summary>
        /// Clears the search result.
        /// </summary>
        public void ClearSearch()
        {
            PostAdapter.ClearSearch();
        }

        /// <summary>
        /// Sorts the adapter by the latest posts
        /// </summary>
        public void SortByLatest()
        {
            PostAdapter.SortByLatest();
        }

        /// <summary>
        /// Sorts the adapter by the oldest posts
        /// </summary>
        public void SortByOldest()
        {
            PostAdapter.SortByOldest();
        }

        /// <summary>
        /// Sorts the adapter by the amount of comments
        /// </summary>
        public void SortByComments()
        {
            PostAdapter.SortByComments();
        }
    }
}