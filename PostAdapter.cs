using Android.Content;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;

namespace Discussit
{
    internal class PostAdapter : BaseAdapter<Post>
    {
        private readonly Context context;
        private readonly List<Post> lstPosts;
        public PostAdapter(Context context)
        {
            this.context = context;
            lstPosts = new List<Post>();
        }
        public override Post this[int position] => lstPosts[position];

        public override int Count => lstPosts.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

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

        public void AddPost(Post post)
        {
            lstPosts.Add(post);
            NotifyDataSetChanged();
        }

        public void RemovePost(Post post) 
        {
            lstPosts.Remove(post);
        }

        public void Clear()
        {
            lstPosts.Clear();
            NotifyDataSetChanged();
        }

        public Post GetPostById(string Id)
        {
            return lstPosts.FirstOrDefault(Post => Id == Post.Id);
        }
    }
}