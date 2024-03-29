using Android.Content;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;

namespace Discussit
{
    /// <summary>
    /// Adapter for displaying posts in a ListView or other AdapterView.
    /// </summary>
    internal class PostAdapter : BaseAdapter<Post>
    {
        private readonly Context context;
        private List<Post> lstPosts;

        /// <summary>
        /// Initializes a new instance of the PostAdapter class.
        /// </summary>
        /// <param name="context">The context in which the adapter is being used.</param>
        public PostAdapter(Context context)
        {
            this.context = context;
            lstPosts = new List<Post>();
        }
        public override Post this[int position] => lstPosts[position];

        public override int Count => lstPosts.Count;

        /// <summary>
        /// Gets the ID of the post at the specified position in the adapter.
        /// </summary>
        /// <param name="position">The position of the post in the adapter.</param>
        /// <returns>The ID of the post at the specified position.</returns>
        public override long GetItemId(int position)
        {
            return position;
        }

        /// <summary>
        /// Gets the view for the post at the specified position in the adapter.
        /// </summary>
        /// <param name="position">The position of the post in the adapter.</param>
        /// <param name="convertView">The recycled view to populate.</param>
        /// <param name="parent">The parent ViewGroup.</param>
        /// <returns>The view for the post at the specified position.</returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater li = LayoutInflater.From(context);
            View v = li.Inflate(Resource.Layout.layout_post, parent, false);
            Post post = lstPosts[position];
            TextView tvCreatorName = v.FindViewById<TextView>(Resource.Id.tvCreatorName);
            TextView tvPostTitle = v.FindViewById<TextView>(Resource.Id.tvPostTitle);
            tvCreatorName.Text = post.CreatorName;
            tvPostTitle.Text = post.Title;
            return v;
        }

        /// <summary>
        /// Sets the posts in the adapter from a list of document snapshots.
        /// </summary>
        /// <param name="documents">The list of document snapshots representing the posts.</param>
        public void SetPosts(IList<DocumentSnapshot> documents)
        {
            Post post;
            FbData fbd = new FbData();
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
                    CommunityPath = General.RemoveFromString('/' + General.POSTS_COLLECTION, document.Id),
                    CreationDate = fbd.FirestoreTimestampToDateTime(document.GetTimestamp(General.FIELD_DATE))
                };
                AddPost(post);
            }
        }

        /// <summary>
        /// Adds a post to the adapter.
        /// </summary>
        /// <param name="post">The post to add to the adapter.</param>
        public void AddPost(Post post)
        {
            lstPosts.Add(post);
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Removes a post from the adapter.
        /// </summary>
        /// <param name="post">The post to remove from the adapter.</param>
        public void RemovePost(Post post) 
        {
            lstPosts.Remove(post);
        }

        /// <summary>
        /// Clears all posts from the adapter.
        /// </summary>
        public void Clear()
        {
            lstPosts.Clear();
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Gets the post with the specified ID from the adapter.
        /// </summary>
        /// <param name="Id">The ID of the post to retrieve.</param>
        /// <returns>The post with the specified ID, or null if not found.</returns>
        public Post GetPostById(string Id)
        {
            return lstPosts.FirstOrDefault(Post => Id == Post.Id);
        }

        /// <summary>
        /// Sorts the list by the latest posts
        /// </summary>
        public void SortByLatest()
        {
            lstPosts = lstPosts.OrderByDescending(post => post.CreationDate).ToList();
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Sorts the list by the oldest posts
        /// </summary>
        public void SortByOldest()
        {
            lstPosts = lstPosts.OrderBy(post => post.CreationDate).ToList();
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Sorts the list by the amount of comments
        /// </summary>
        public void SortByComments()
        {
            lstPosts = lstPosts.OrderByDescending(post => post.CommentCount).ToList();
            NotifyDataSetChanged();
        }
    }
}