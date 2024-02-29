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
using System.Text;

namespace Discussit
{
    [Activity(Label = "CreatePostActivity")]
    public class CreatePostActivity : AppCompatActivity, View.IOnClickListener
    {
        User user;
        Community community;
        ImageButton ibtnBack, ibtnLogo;
        EditText etPostTitle, etPostDescription;
        Button btnCreatePost;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_createPost);
            InitObjects();
            InitViews();
        }

        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
            community = Community.GetCommunityJson(Intent.GetStringExtra(General.KEY_COMMUNITY));
        }

        private void InitViews()
        {
            TextView tvCommunityName = FindViewById<TextView>(Resource.Id.tvCommunityName);
            TextView tvCommunityDescription = FindViewById<TextView>(Resource.Id.tvCommunityDescription);
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            ibtnLogo = FindViewById<ImageButton>(Resource.Id.ibtnLogo);
            etPostTitle = FindViewById<EditText>(Resource.Id.etPostTitle);
            etPostDescription = FindViewById<EditText>(Resource.Id.etPostDescription);
            btnCreatePost = FindViewById<Button>(Resource.Id.btnCreatePost);
            tvCommunityName.Text = community.Name;
            tvCommunityDescription.Text = community.Description;
            ibtnBack.SetOnClickListener(this);
            ibtnLogo.SetOnClickListener(this);
            btnCreatePost.SetOnClickListener(this);
        }

        public void Back()
        {
            Intent intent = new Intent();
            intent.PutExtra(General.KEY_USER, user.GetJson());
            SetResult(Result.Ok, intent);
            Finish();
        }

        public void ReturnToHub()
        {
            Intent intent = new Intent(this, typeof(CommunityHubActivity));
            intent.AddFlags(ActivityFlags.LaunchedFromHistory);
            intent.PutExtra(General.KEY_USER, user.GetJson());
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
            bool status = true;
            status = status && (etPostTitle.Text != string.Empty);
            status = status && (etPostDescription.Text != string.Empty);
            return status;
        }

        private void CreatePost()
        {
            Post post = community.AddPost(etPostTitle.Text, etPostDescription.Text, user);
            ViewPost(post);
        }

        private void ViewPost(Post post)
        {
            Intent intent = new Intent(this, typeof(ViewPostActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_POST, post.GetJson());
            StartActivity(intent);
            Finish();
        }

        public void OnClick(View v)
        {
            if (v == ibtnLogo)
                ReturnToHub();
            else if (v == ibtnBack)
                Back();
            else if (v == btnCreatePost)
            {
                if (ValidInputFields())
                    CreatePost();
                else
                    Toast.MakeText(this, Resources.GetString(Resource.String.InvalidFields), ToastLength.Long).Show();
            }
        }
    }
}