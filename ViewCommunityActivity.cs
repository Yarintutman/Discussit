using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Firestore;
using Java.Lang;

namespace Discussit
{
    [Activity(Label = "ViewCommunityActivity")]
    public class ViewCommunityActivity : AppCompatActivity, View.IOnClickListener, AdapterView.IOnItemClickListener, IEventListener, IOnCompleteListener
    {
        User user;
        Community community;
        Posts posts;
        ImageButton ibtnBack, ibtnLogo, ibtnProfile, ibtnSearch;
        TextView tvCommunityName, tvDescription, tvMemberCount, tvSortBy;
        Button btnViewDescription, btnNewPost;
        EditText etSearchBar;

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
            posts = community.Posts;
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
            etSearchBar = FindViewById<EditText>(Resource.Id.etSearchBar);
            ListView lvPosts = FindViewById<ListView>(Resource.Id.lvPosts);
            lvPosts.OnItemClickListener = this;
            lvPosts.Adapter = posts.PostAdapter;
            posts.AddSnapshotListener(this);
            ibtnBack.SetOnClickListener(this);
            ibtnLogo.SetOnClickListener(this);
            ibtnProfile.SetOnClickListener(this);
            ibtnSearch.SetOnClickListener(this);
            btnViewDescription.SetOnClickListener(this);
            btnNewPost.SetOnClickListener(this);
            tvSortBy.SetOnClickListener(this);
            SetSorting(Resources.GetString(Resource.String.sortDate));
            tvCommunityName.Text = community.Name;
            tvDescription.Text = community.Description;
            tvMemberCount.Text = community.MemberCount.ToString();
        }

        public void SetSorting(string sortBy)
        {
            tvSortBy.Text = Resources.GetString(Resource.String.sortBy) + " " + sortBy;
        }

        public void OnClick(View v)
        {
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
        }

        public void OnComplete(Task task)
        {
        }

        public void OnEvent(Object obj, FirebaseFirestoreException error)
        {
        }
    }
}