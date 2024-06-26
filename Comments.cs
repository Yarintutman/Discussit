﻿using Android.App;
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
        public void AddComments(IList<DocumentSnapshot> documents)
        {
            Comment comment;
            foreach (DocumentSnapshot document in documents)
            {
                comment = new Comment
                {
                    Id = document.Id,
                    Description = document.GetString(General.FIELD_COMMENT_DESCRIPTION),
                    CreatorName = document.GetString(General.FIELD_COMMENT_CREATOR_NAME),
                    CreatorUID = document.GetString(General.FIELD_COMMENT_CREATOR_UID),
                    HasComments = (bool)(document.GetBoolean(General.FIELD_HAS_COMMENTS)),
                    ParentPath = Path,
                    CreationDate = fbd.FirestoreTimestampToDateTime(document.GetTimestamp(General.FIELD_DATE))
                };
                CommentAdapter.ReplaceComment(comment); 
            }
            CommentAdapter.SortByPath();
        }

        /// <summary>
        /// Adds comments from the provided list of Firestore documents to the comment adapter.
        /// </summary>
        /// <param name="documents">The list of Firestore documents representing comments.</param>
        /// <param name="parentComment">The parent comment for the subComments.</param>
        public void AddSubComments(IList<DocumentSnapshot> documents, Comment parentComment)
        {
            Comment comment;
            foreach (DocumentSnapshot document in documents)
            {
                comment = new Comment
                {
                    Id = document.Id,
                    Description = document.GetString(General.FIELD_COMMENT_DESCRIPTION),
                    CreatorName = document.GetString(General.FIELD_COMMENT_CREATOR_NAME),
                    CreatorUID = document.GetString(General.FIELD_COMMENT_CREATOR_UID),
                    HasComments = (bool)(document.GetBoolean(General.FIELD_HAS_COMMENTS)),
                    ParentPath = parentComment.Path,
                    CreationDate = fbd.FirestoreTimestampToDateTime(document.GetTimestamp(General.FIELD_DATE))
                };
                CommentAdapter.ReplaceComment(comment);
            }
            CommentAdapter.SortByPath();
        }

        /// <summary>
        /// Retrieves all comments from the Firestore collection.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task GetComments()
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

        /// <summary>
        /// Removes all of the subcomments of the comment from the adapter
        /// </summary>
        /// <param name="comment">The comment to remove subcomments from</param>
        public void RemoveSubcomments(Comment comment)
        {
            CommentAdapter.RemoveSubcomments(comment);
        }

        /// <summary>
        /// Shows the open Subcomments button in all of the comments that have subcomments
        /// </summary>
        public void ShowOpenComments()
        {
            CommentAdapter.ShowOpenComments();
        }
    }
}