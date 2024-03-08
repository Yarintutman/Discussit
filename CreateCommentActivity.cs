using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Discussit
{
    /// <summary>
    /// Activity for creating a comment.
    /// </summary>
    [Activity(Label = "CreateCommentActivity")]
    public class CreateCommentActivity : AppCompatActivity, View.IOnClickListener
    {
        User user;
        Post post;
        Comment comment;
        ImageButton ibtnBack, ibtnLogo;
        EditText etCommentDescription;
        Button btnCreateComment;

        /// <summary>
        /// Called when the activity is starting.
        /// </summary>
        /// <param name="savedInstanceState">Not in use</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_createComment);
            InitObjects();
            InitViews();
        }

        /// <summary>
        /// Initializes the objects used in the activity.
        /// </summary>
        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
            post = Post.GetPostJson(Intent.GetStringExtra(General.KEY_POST));
            if (Intent.GetBooleanExtra(General.KEY_IS_COMMENT_RECURSIVE, false) == true ) 
                comment = Comment.GetCommentJson(Intent.GetStringExtra(General.KEY_COMMENT));
        }

        /// <summary>
        /// Initializes the views used in the activity.
        /// </summary>
        private void InitViews()
        {
            TextView tvPostTitle = FindViewById<TextView>(Resource.Id.tvPostTitle);
            TextView tvPostDescription = FindViewById<TextView>(Resource.Id.tvPostDescription);
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            ibtnLogo = FindViewById<ImageButton>(Resource.Id.ibtnLogo);
            etCommentDescription = FindViewById<EditText>(Resource.Id.etCommentDescription);
            btnCreateComment = FindViewById<Button>(Resource.Id.btnCreateComment);
            tvPostTitle.Text = post.Title;
            tvPostDescription.Text = post.Description;
            ibtnBack.SetOnClickListener(this);
            ibtnLogo.SetOnClickListener(this);
            btnCreateComment.SetOnClickListener(this);
        }

        /// <summary>
        /// Returns to the previous activity.
        /// </summary>
        public void Back()
        {
            Intent intent = new Intent();
            intent.PutExtra(General.KEY_USER, user.GetJson());
            SetResult(Result.Ok, intent);
            Finish();
        }

        /// <summary>
        /// Returns to the community hub.
        /// </summary>
        public void ReturnToHub()
        {
            Intent intent = new Intent(this, typeof(CommunityHubActivity));
            intent.AddFlags(ActivityFlags.LaunchedFromHistory);
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivity(intent);
            Finish();
        }

        /// <summary>
        /// Disables the default back button behavior and invokes the custom back method.
        /// </summary>
#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed()
        {
            Back();
        }
#pragma warning restore CS0672 // Member overrides obsolete member

        /// <summary>
        /// Checks if the input fields contain valid data.
        /// </summary>
        /// <returns> true if the input fields are valid; otherwise, false.</returns>
        private bool ValidInputFields()
        {
            return etCommentDescription.Text != string.Empty;
        }

        /// <summary>
        /// Creates a new comment and adds it to the post or comment.
        /// </summary>
        private void CreateComment()
        {
            if (comment == null)
                post.AddComment(etCommentDescription.Text, user);
            else
            {
                comment.AddComment(etCommentDescription.Text, user);
                post.IncrementComments(1);
            }
            Finish();
        }

        /// <summary>
        /// Handles click events for views in the activity.
        /// </summary>
        /// <param name="v">The view that was clicked.</param>
        public void OnClick(View v)
        {
            if (v == ibtnLogo)
                ReturnToHub();
            else if (v == ibtnBack)
                Back();
            else if (v == btnCreateComment)
            {
                if (ValidInputFields())
                    CreateComment();
                else
                    Toast.MakeText(this, Resources.GetString(Resource.String.InvalidFields), ToastLength.Long).Show();
            }
        }
    }
}