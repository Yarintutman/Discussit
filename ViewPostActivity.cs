using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Util;
using Firebase.Firestore;
using System;

namespace Discussit
{
    [Activity(Label = "ViewPostActivity")]
    public class ViewPostActivity : AppCompatActivity, View.IOnClickListener, AdapterView.IOnItemClickListener, AdapterView.IOnItemLongClickListener, IEventListener, IOnCompleteListener
    {
        User user;
        Post post;
        Comments comments;
        ImageButton ibtnBack, ibtnLogo, ibtnProfile;
        TextView tvSortBy;
        Button btnNewComment;
        Task tskGetComments;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_viewPost);
            InitObjects();
            InitViews();
        }

        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
            post = Post.GetPostJson(Intent.GetStringExtra(General.KEY_POST));
            post.CreateComments(this);
            comments = post.Comments;
        }

        private void InitViews()
        {
            TextView tvPostCreator = FindViewById<TextView>(Resource.Id.tvPostCreator);
            TextView tvPostTitle = FindViewById<TextView>(Resource.Id.tvPostTitle);
            TextView tvPostDescription = FindViewById<TextView>(Resource.Id.tvPostDescription);
            ListView lvComments = FindViewById<ListView>(Resource.Id.lvComments);
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            ibtnLogo = FindViewById<ImageButton>(Resource.Id.ibtnLogo);
            ibtnProfile = FindViewById<ImageButton>(Resource.Id.ibtnProfile);
            tvSortBy = FindViewById<TextView>(Resource.Id.tvSortBy);
            btnNewComment = FindViewById<Button>(Resource.Id.btnNewComment);
            tvPostCreator.Text = post.CreatorName;
            tvPostTitle.Text = post.Title;
            tvPostDescription.Text = post.Description;
            lvComments.Adapter = comments.CommentAdapter;
            lvComments.OnItemClickListener = this;
            lvComments.OnItemLongClickListener = this;
            comments.AddSnapshotListener(this);
            ibtnBack.SetOnClickListener(this);
            ibtnLogo.SetOnClickListener(this);
            ibtnProfile.SetOnClickListener(this);
            btnNewComment.SetOnClickListener(this);
            tvSortBy.SetOnClickListener(this);
            SetSorting(Resources.GetString(Resource.String.sortDate));
        }

        public void SetSorting(string sortBy)
        {
            tvSortBy.Text = Resources.GetString(Resource.String.sortBy) + " " + sortBy;
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

        private void OpenCreateCommentActivity()
        {
            Intent intent = new Intent(this, typeof(CreateCommentActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_IS_COMMENT_RECURSIVE, false);
            intent.PutExtra(General.KEY_POST, post.GetJson());
            StartActivity(intent);
        }

        private void OpenCreateCommentActivity(Comment comment)
        {
            Intent intent = new Intent(this, typeof(CreateCommentActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_IS_COMMENT_RECURSIVE, true);
            intent.PutExtra(General.KEY_POST, post.GetJson());
            intent.PutExtra(General.KEY_COMMENT, comment.GetJson());
            StartActivity(intent);
        }

        private void GetComments()
        {
            tskGetComments = post.GetComments().AddOnCompleteListener(this);
        }

        private void GetRecursiveComments(Comments comment)
        {
            tskGetComments = comment.GetComments().AddOnCompleteListener(this);
        }

        private void ViewProfile()
        {
            Intent intent = new Intent(this, typeof(ProfileActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivity(intent);
        }

        public void OnClick(View v)
        {
            if (v == ibtnLogo)
                ReturnToHub();
            else if (v == ibtnBack)
                Back();
            else if (v == ibtnProfile)
                ViewProfile();
            else if (v == btnNewComment)
                OpenCreateCommentActivity();
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                if (task == tskGetComments)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    comments.AddComments(qs.Documents);
                }
            }
        }

        public void OnEvent(Java.Lang.Object obj, FirebaseFirestoreException error)
        {
            GetComments();
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            //call recursiveComments
            //GetRecursiveComments();
        }

        public bool OnItemLongClick(AdapterView parent, View view, int position, long id)
        {
            //call create recursive comment after popping menu
            return true;
        }
    }
}