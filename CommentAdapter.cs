using Android;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Util;
using Firebase.Firestore.Core;
using System.Collections.Generic;
using System.Linq;
using View = Android.Views.View;

namespace Discussit
{
    /// <summary>
    /// Adapter for displaying comments in a ListView.
    /// </summary>
    internal class CommentAdapter : BaseAdapter<Comment>
    {
        private readonly Context context; 
        private List<Comment> lstComments; 
        /// <summary>
        /// Initializes a new instance of the CommentAdapter class.
        /// </summary>
        /// <param name="context">The context of the application.</param>
        public CommentAdapter(Context context)
        {
            this.context = context;
            lstComments = new List<Comment>();
        }

        /// <summary>
        /// Gets the comment at the specified position in the list.
        /// </summary>
        /// <param name="position">The position of the comment in the list.</param>
        /// <returns>The comment at the specified position.</returns>
        public override Comment this[int position] => lstComments[position];

        /// <summary>
        /// Gets the number of comments in the list.
        /// </summary>
        public override int Count => lstComments.Count;

        /// <summary>
        /// Gets the ID of the comment at the specified position in the list.
        /// </summary>
        /// <param name="position">The position of the comment in the list.</param>
        /// <returns>The ID of the comment at the specified position.</returns>
        public override long GetItemId(int position)
        {
            return position;
        }

        /// <summary>
        /// Gets the view that displays the comment at the specified position in the list.
        /// </summary>
        /// <param name="position">The position of the comment in the list.</param>
        /// <param name="convertView">Not in use</param>
        /// <param name="parent">The parent that this view will eventually be attached to.</param>
        /// <returns>The view that displays the comment at the specified position.</returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater li = LayoutInflater.From(context);
            View v = li.Inflate(Resource.Layout.layout_comment, parent, false);
            Comment comment = lstComments[position];
            TextView tvCreatorName = v.FindViewById<TextView>(Resource.Id.tvCommentCreator);
            TextView tvCommentDescription = v.FindViewById<TextView>(Resource.Id.tvCommentDescription);
            tvCreatorName.Text = comment.CreatorName;
            tvCommentDescription.Text = comment.Description;
            if (comment.HasComments)
            {
                TextView tvViewComments = v.FindViewById<TextView>(Resource.Id.tvViewComments);
                tvViewComments.Visibility = ViewStates.Visible;
                if (comment.HideComments) 
                    tvViewComments.Text = Application.Context.Resources.GetString(Resource.String.HideComments);
            }
            //The needed padding based on the amount of parent comments to the subcomment
            int leftPaddingDp = General.SUB_COMMENT_PADDING * (General.AppearanceCount<string>(
                                General.StringToList(comment.Path, '/'),
                                General.COMMENTS_COLLECTION) - 1);
            int leftPaddingPx = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, leftPaddingDp, context.Resources.DisplayMetrics);
            v.SetPadding(leftPaddingPx, v.PaddingTop, v.PaddingRight, v.PaddingBottom);
            LinearLayout llComment = v.FindViewById<LinearLayout>(Resource.Id.llComment);
            ViewGroup.LayoutParams llParams = llComment.LayoutParameters;
            llParams.Width += leftPaddingDp;
            llComment.LayoutParameters = llParams;
            return v;
        }

        /// <summary>
        /// Adds a comment to the list and notifies the adapter that the data set has changed.
        /// </summary>
        /// <param name="comment">The comment to add to the list.</param>
        public void AddComment(Comment comment)
        {
            lstComments.Add(comment);
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Replaces an existing comment with an updated in the list and notifies the adapter that the data set has changed.
        /// </summary>
        /// <param name="comment">The comment to add to the list.</param>
        public void ReplaceComment(Comment comment)
        {
            lstComments.RemoveAll(current => current.Path == comment.Path);
            lstComments.Add(comment);
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Removes a comment from the list and notifies the adapter that the data set has changed.
        /// </summary>
        /// <param name="comment">The comment to remove from the list.</param>
        public void RemoveComment(Comment comment)
        {
            lstComments.Remove(comment);
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Clears all comments from the list and notifies the adapter that the data set has changed.
        /// </summary>
        public void Clear()
        {
            lstComments.Clear();
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Retrieves a comment from the list by its ID.
        /// </summary>
        /// <param name="Id">The ID of the comment to retrieve.</param>
        /// <returns>The comment with the specified ID, or null if not found.</returns>
        public Comment GetCommentById(string Id)
        {
            return lstComments.FirstOrDefault(Comment => Id == Comment.Id);
        }

        public void SortByPath()
        {
            lstComments = lstComments.OrderBy(comment => comment.Path).ToList();
            NotifyDataSetChanged();
        }

        public void RemoveRecursiveComments(Comment comment)
        {
            lstComments.RemoveAll(current => current.Path.Contains(comment.Path) && current.Path != comment.Path);
            NotifyDataSetChanged();
        }

        public void ShowOpenComments()
        {
            List<Comment> lstHasRecursive = lstComments.Where(comment => lstComments.Any(current => comment.Path == current.ParentPath)).ToList();
            foreach (Comment comment in lstHasRecursive)
            {
                comment.HasComments = true;
                comment.HideComments = true;
                ReplaceComment(comment);
            }
            SortByPath();
        }
    }
}