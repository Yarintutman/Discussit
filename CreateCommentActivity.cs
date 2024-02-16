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
            //work on views
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            ibtnLogo = FindViewById<ImageButton>(Resource.Id.ibtnLogo);
            etCommentDescription = FindViewById<EditText>(Resource.Id.etCommentDescription);
        }

        public void OnClick(View v)
        {
        }
    }
}