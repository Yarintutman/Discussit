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
    [Activity(Label = "CreateCommentActivity")]
    public class CreateCommentActivity : AppCompatActivity, View.IOnClickListener
    {
        Post post;
        Comment comment;
        ImageButton ibtnBack, ibtnLogo;
        EditText etCommentDescription;
        Button btnCreateComment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_createComment);
            InitObjects();
            InitViews();
        }

        private void InitObjects()
        {
            post = Post.GetPostJson(Intent.GetStringExtra(General.KEY_POST));
            if (Intent.GetBooleanExtra(General.KEY_IS_COMMENT_RECURSIVE, false) == true ) 
                comment = Comment.GetCommentJson(Intent.GetStringExtra(General.KEY_COMMENT));
        }

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

        public void Back()
        {
            Finish();
        }

        public void ReturnToHub()
        {
            Intent intent = new Intent(this, typeof(CommunityHubActivity));
            intent.AddFlags(ActivityFlags.LaunchedFromHistory);
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            StartActivity(intent);
            Finish();
        }

#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed()
        {
            Back();
        }
#pragma warning restore CS0672 // Member overrides obsolete member

        private bool ValidInputFields()
        {
            return etCommentDescription.Text != string.Empty;
        }

        private void CreateComment()
        {
            if (comment == null)
                post.AddComment(etCommentDescription.Text, User.GetUserJson(Intent.GetStringExtra(General.KEY_USER)));
            else
            {
                comment.AddComment(etCommentDescription.Text, User.GetUserJson(Intent.GetStringExtra(General.KEY_USER)));
                post.IncrementComments(+1);
            }
            Finish();
        }

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