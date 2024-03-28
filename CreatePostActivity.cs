using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace Discussit
{
    /// <summary>
    /// Activity for creating a new post within a community.
    /// </summary>
    [Activity(Label = "CreatePostActivity")]
    public class CreatePostActivity : AppCompatActivity, View.IOnClickListener
    {
        User user;
        Community community;
        Post post;
        ImageButton ibtnBack, ibtnLogo;
        EditText etPostTitle, etPostDescription;
        Button btnCreatePost;

        /// <summary>
        /// Called when the activity is starting.
        /// </summary>
        /// <param name="savedInstanceState">Not in use.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_createPost);
            InitObjects();
            InitViews();
        }

        /// <summary>
        /// Initializes the objects used in the activity.
        /// </summary>
        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
            community = Community.GetCommunityJson(Intent.GetStringExtra(General.KEY_COMMUNITY));
            string postJson = Intent.GetStringExtra(General.KEY_POST);
            if (postJson != null)
                post = Post.GetPostJson(postJson);
        }

        /// <summary>
        /// Initializes the views used in the activity.
        /// </summary>
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
            if (post != null)
            {
                etPostTitle.Text = post.Title;
                etPostDescription.Text= post.Description;
                btnCreatePost.Text = Resources.GetString(Resource.String.updatePost);
            }
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
        /// <returns>true if the input fields are valid; otherwise, false.</returns>
        private bool ValidInputFields()
        {
            bool status = true;
            status = status && (etPostTitle.Text != string.Empty);
            status = status && (etPostDescription.Text != string.Empty);
            return status;
        }

        /// <summary>
        /// Creates a new post with the provided information within the current community.
        /// </summary>
        private void CreatePost()
        {
            if (post == null)
                post = community.AddPost(etPostTitle.Text, etPostDescription.Text, user);
            else
                community.UpdatePost(post, etPostTitle.Text, etPostDescription.Text);
            ViewPost(post);
        }

        /// <summary>
        /// Starts the activity to view the created post.
        /// </summary>
        /// <param name="post">The post to view.</param>
        private void ViewPost(Post post)
        {
            Intent intent = new Intent(this, typeof(ViewPostActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            intent.PutExtra(General.KEY_POST, post.GetJson());
            StartActivity(intent);
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