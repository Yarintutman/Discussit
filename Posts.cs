using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discussit
{
    internal class Posts 
    {
        private readonly FbData fbd;
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

        public Posts(Activity context)
        {
            PostAdapter = new PostAdapter(context);
            fbd = new FbData();
        }

        public void AddSnapshotListener(Activity context, string path)
        {
            onCollectionChangeListener = fbd.AddSnapshotListener(context, path + "\\" + General.POSTS_COLLECTION);
        }

        public void RemoveSnapshotListener()
        {
            onCollectionChangeListener?.Remove();
        }

        internal void AddPost(IList<DocumentSnapshot> documents)
        {
            PostAdapter.Clear();
            Post post;
            foreach (DocumentSnapshot document in documents)
            {
                post = new Post
                {
                    //add Posts
                };
                PostAdapter.AddPost(post);
            }
        }

        internal Task GetPosts()
        {
            return fbd.GetCollection(General.COMMUNITIES_COLLECTION);
        }
    }
}