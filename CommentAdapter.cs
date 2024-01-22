﻿using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

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
            View v = li.Inflate(Resource.Layout.layout_post, parent, false);

            return v;
        }

        public void AddComment(Comment comment)
        {
            lstComments.Add(comment);
            NotifyDataSetChanged();
        }
        public void Clear()
        {
            lstComments.Clear();
            NotifyDataSetChanged();
        }
    }
}