using Android.Content;
using Android.Views;
using Android.Widget;
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

            return v;
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