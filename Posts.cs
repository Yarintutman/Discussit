﻿using Android.App;
using Android.Gms.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;

namespace Discussit
{
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

        public Posts(Activity context, string path)
        {
            PostAdapter = new PostAdapter(context);
            fbd = new FbData();
            Path = path;
        }

        public void AddSnapshotListener(Activity context)
        {
            onCollectionChangeListener = fbd.AddSnapshotListener(context, Path + "\\" + General.POSTS_COLLECTION);
        }

        public void RemoveSnapshotListener()
        {
            onCollectionChangeListener?.Remove();
        }

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
                    CreatorUID = document.GetString(General.FIELD_POST_CREATOR),
                    CommunityPath = Path
                };
                PostAdapter.AddPost(post);
            }
        }

        internal Task GetPosts()
        {
            return fbd.GetCollection(Path + "\\" + General.POSTS_COLLECTION);
        }
    }
}