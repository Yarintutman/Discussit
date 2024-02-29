using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Firestore;
using Java.Lang;
using System.Threading;

namespace Discussit
{
    [Activity(Label = "ViewCommunityActivity")]
    public class ViewCommunityActivity : AppCompatActivity, View.IOnClickListener, AdapterView.IOnItemClickListener, IEventListener, IOnCompleteListener
    {
        User user;
        Community community;
        Posts posts;
        Members members;
        ImageButton ibtnBack, ibtnLogo, ibtnProfile, ibtnSearch;
        TextView tvCommunityName, tvDescription, tvMemberCount, tvSortBy;
        Button btnViewDescription, btnNewPost, btnJoinCommunity;
        EditText etSearchBar;
        Task tskGetPosts, tskGetMemebers;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_viewCommunity);
            InitObjects();
            InitViews();
        }

        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
            community = Community.GetCommunityJson(Intent.GetStringExtra(General.KEY_COMMUNITY));
            community.CreatePosts(this);
            community.CreateMembers(this);
            posts = community.Posts;
            members = community.Members;
        }

        private void InitViews()
        {
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            ibtnLogo = FindViewById<ImageButton>(Resource.Id.ibtnLogo);
            ibtnProfile = FindViewById<ImageButton>(Resource.Id.ibtnProfile);
            ibtnSearch = FindViewById<ImageButton>(Resource.Id.ibtnSearch);
            tvCommunityName = FindViewById<TextView>(Resource.Id.tvCommunityName);
            tvDescription = FindViewById<TextView>(Resource.Id.tvDescription);
            tvMemberCount = FindViewById<TextView>(Resource.Id.tvMemberCount);
            tvSortBy = FindViewById<TextView>(Resource.Id.tvSortBy);
            btnViewDescription = FindViewById<Button>(Resource.Id.btnViewDescription);
            btnNewPost = FindViewById<Button>(Resource.Id.btnNewPost);
            btnJoinCommunity = FindViewById<Button>(Resource.Id.btnJoinCommunity);
            etSearchBar = FindViewById<EditText>(Resource.Id.etSearchBar);
            ListView lvPosts = FindViewById<ListView>(Resource.Id.lvPosts);
            lvPosts.Adapter = posts.PostAdapter;
            lvPosts.OnItemClickListener = this;
            RegisterForContextMenu(lvPosts);
            posts.AddSnapshotListener(this);
            members.AddSnapshotListener(this);
            ibtnBack.SetOnClickListener(this);
            ibtnLogo.SetOnClickListener(this);
            ibtnProfile.SetOnClickListener(this);
            ibtnSearch.SetOnClickListener(this);
            btnViewDescription.SetOnClickListener(this);
            btnNewPost.SetOnClickListener(this);
            btnJoinCommunity.SetOnClickListener(this);
            tvSortBy.SetOnClickListener(this);
            SetSorting(Resources.GetString(Resource.String.sortDate));
            tvCommunityName.Text = community.Name;
            tvDescription.Text = community.Description;
            tvMemberCount.Text = community.MemberCount.ToString();
        }

        public override void OnCreateContextMenu(Android.Views.IContextMenu menu, Android.Views.View v, Android.Views.IContextMenuContextMenuInfo menuInfo)
        {
            MenuInflater.Inflate(Resource.Menu.menu_manageMember, menu);
            base.OnCreateContextMenu(menu, v, menuInfo);
        }

        public override bool OnContextItemSelected(Android.Views.IMenuItem item)
        {

            return base.OnContextItemSelected(item);
        }

        private void CheckMembership()
        {
            if (members.HasMember(user.Id))
                btnJoinCommunity.Visibility = ViewStates.Gone;
            else
                btnJoinCommunity.Visibility = ViewStates.Visible;
        }

        public void SetSorting(string sortBy)
        {
            tvSortBy.Text = Resources.GetString(Resource.String.sortBy) + " " + sortBy;
        }

        private void OpenCreatePostActivity()
        {
            Intent intent = new Intent(this, typeof(CreatePostActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivityForResult(intent, 0);
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

        private void ViewProfile()
        {
            Intent intent = new Intent(this, typeof(ProfileActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivityForResult(intent, 0);
        }

#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed()
        {
            Back();
        }
#pragma warning restore CS0672 // Member overrides obsolete member

        public void OnClick(View v)
        {
            if (v == ibtnLogo)
                ReturnToHub();
            else if (v == ibtnBack)
                Back();
            else if (v == ibtnProfile)
                ViewProfile();
            else if (v == btnNewPost)
            {
                if (btnJoinCommunity.Visibility == ViewStates.Gone)
                    OpenCreatePostActivity();
                else
                    Toast.MakeText(this, Resources.GetString(Resource.String.joinCommunityToToCreatePost), ToastLength.Short).Show();
            }
            else if (v == btnJoinCommunity)
                JoinCommunity();
        }

        private void JoinCommunity()
        {
            community.AddMember(user);
            user.UpdateArrayField(General.FIELD_USER_COMMUNITIES, community.CollectionPath);
        }

        private void ViewPost(Post post)
        {
            Intent intent = new Intent(this, typeof(ViewPostActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_POST, post.GetJson());
            StartActivityForResult(intent, 0);
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            ViewPost(posts[position]);
        }

        private void GetPosts()
        {
            tskGetPosts = community.GetPosts().AddOnCompleteListener(this);
        }

        private void GetMembers()
        {
            tskGetMemebers = community.GetMembers().AddOnCompleteListener(this);
        }

        public void OnEvent(Object obj, FirebaseFirestoreException error)
        {
            GetPosts();
            GetMembers();
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                if (task == tskGetPosts)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    posts.AddPosts(qs.Documents);
                }
                else if (task == tskGetMemebers) 
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    members.AddMembers(qs.Documents);
                    CheckMembership();
                    tvMemberCount.Text = members.MemberCount.ToString();
                }
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
                user = User.GetUserJson(data.GetStringExtra(General.KEY_USER));
            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnResume()
        {
            base.OnResume();
            posts.AddSnapshotListener(this);
            members.AddSnapshotListener(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            posts.RemoveSnapshotListener();
            members.RemoveSnapshotListener();
        }
    }
}