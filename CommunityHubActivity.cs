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

namespace Discussit
{
    [Activity(Label = "CommunityHubActivity")]
    public class CommunityHubActivity : AppCompatActivity, View.IOnClickListener, IEventListener, IOnCompleteListener
    {
        User user;
        ImageButton ibtnProfile, ibtnSearch;
        EditText etSearchBar;
        Button btnNewCommunity;
        TextView tvSortBy;
        Communities communities;
        Task tskGetCommunities;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_communityHub);
            InitObjects();
            InitViews();
        }

        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
            communities = new Communities(this);
        }

        private void InitViews()
        {
            ibtnProfile = FindViewById<ImageButton>(Resource.Id.ibtnProfile);
            ibtnSearch = FindViewById<ImageButton>(Resource.Id.ibtnSearch);
            btnNewCommunity = FindViewById<Button>(Resource.Id.btnNewCommunity);
            etSearchBar = FindViewById<EditText>(Resource.Id.etSearchBar);
            tvSortBy = FindViewById<TextView>(Resource.Id.tvSortBy);
            ListView lvCommunities = FindViewById<ListView>(Resource.Id.lvCommunities);
            lvCommunities.Adapter = communities.CommunityAdapter;
            communities.AddSnapshotListener(this);
            SetSorting(Resources.GetString(Resource.String.sortDate));
            ibtnProfile.SetOnClickListener(this);
            ibtnSearch.SetOnClickListener(this);
            btnNewCommunity.SetOnClickListener(this);
            tvSortBy.SetOnClickListener(this);
        }

        public void SetSorting(string sortBy)
        {
            tvSortBy.Text = Resources.GetString(Resource.String.sortBy) + " " + sortBy + user.Username;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnClick(View v)
        {
            if (v == ibtnProfile)
            {

            }
            else if (v == ibtnSearch)
            {

            }
            else if (v ==  tvSortBy)
            {

            }
            else if (v == btnNewCommunity)
            {
                OpenCreateCommunityActivity();
            }
        }

        private void OpenCreateCommunityActivity()
        {
            Intent intent = new Intent(this, typeof(CreateCommunityActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivity(intent);
        }

        public void GetCommunities()
        {
            tskGetCommunities = communities.GetCommunities();
        }

#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed() { }
#pragma warning restore CS0672 // Member overrides obsolete member

        public void OnEvent(Object obj, FirebaseFirestoreException error)
        {
            GetCommunities();
        }

        public void OnComplete(Task task)
        {
            if (task == tskGetCommunities)
            {
                QuerySnapshot qs = (QuerySnapshot)task.Result;
                communities.AddCommunities(qs.Documents);
            }
        }
    }
}