using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace Discussit
{
    internal class CommentAdapter : BaseAdapter<Comment>
    {
        private readonly Context context;
        private readonly List<Comment> lstComments;
        public CommentAdapter(Context context)
        {
            this.context = context;
            lstComments = new List<Comment>();
        }
        public override Comment this[int position] => lstComments[position];

        public override int Count => lstComments.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater li = LayoutInflater.From(context);
            View v = li.Inflate(Resource.Layout.layout_comment, parent, false);
            Comment comment = lstComments[position];
            TextView creatorName = v.FindViewById<TextView>(Resource.Id.tvCommentCreator);
            TextView commentDescription = v.FindViewById<TextView>(Resource.Id.tvCommentDescription);
            creatorName.Text = comment.CreatorName;
            commentDescription.Text = comment.Description;
            return v;
        }

        public void AddComment(Comment comment)
        {
            lstComments.Add(comment);
            NotifyDataSetChanged();
        }

        public void RemoveComment(Comment comment)
        {
            lstComments.Remove(comment);
        }

        public void Clear()
        {
            lstComments.Clear();
            NotifyDataSetChanged();
        }

        public Comment GetCommentById(string Id)
        {
            return lstComments.FirstOrDefault(Comment => Id == Comment.Id);
        }
    }
}