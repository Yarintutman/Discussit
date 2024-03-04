using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Util;
using Firebase.Firestore;
using Java.Lang;

namespace Discussit
{
    /// <summary>
    /// Activity for displaying the community hub.
    /// </summary>
    [Activity(Label = "CommunityHubActivity")]
    public class CommunityHubActivity : AppCompatActivity, View.IOnClickListener, AdapterView.IOnItemClickListener, IEventListener, IOnCompleteListener
    {
        User user;
        ImageButton ibtnProfile, ibtnSearch;
        EditText etSearchBar;
        Button btnNewCommunity;
        TextView tvSortBy;
        Communities communities;
        Task tskGetCommunities;

        /// <summary>
        /// Called when the activity is starting.
        /// </summary>
        /// <param name="savedInstanceState">not in use</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_communityHub);
            InitObjects();
            InitViews();
        }

        /// <summary>
        /// Initializes the objects used in the activity.
        /// </summary>
        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
            communities = new Communities(this);
        }

        /// <summary>
        /// Initializes the views used in the activity.
        /// </summary>
        private void InitViews()
        {
            ibtnProfile = FindViewById<ImageButton>(Resource.Id.ibtnProfile);
            ibtnSearch = FindViewById<ImageButton>(Resource.Id.ibtnSearch);
            btnNewCommunity = FindViewById<Button>(Resource.Id.btnNewCommunity);
            etSearchBar = FindViewById<EditText>(Resource.Id.etSearchBar);
            tvSortBy = FindViewById<TextView>(Resource.Id.tvSortBy);
            ListView lvCommunities = FindViewById<ListView>(Resource.Id.lvCommunities);
            lvCommunities.Adapter = communities.CommunityAdapter;
            lvCommunities.OnItemClickListener = this;
            communities.AddSnapshotListener(this);
            ibtnProfile.SetOnClickListener(this);
            ibtnSearch.SetOnClickListener(this);
            btnNewCommunity.SetOnClickListener(this);
            tvSortBy.SetOnClickListener(this);
            SetSorting(Resources.GetString(Resource.String.sortDate));
        }


        /// <summary>
        /// Sets the sorting text.
        /// </summary>
        /// <param name="sortBy">The sorting format.</param>
        public void SetSorting(string sortBy)
        {
            tvSortBy.Text = Resources.GetString(Resource.String.sortBy) + " " + sortBy;
        }

        /// <summary>
        /// Called when the result of a permission request is received.
        /// </summary>
        /// <param name="requestCode">Not in use</param>
        /// <param name="permissions">Not in use</param>
        /// <param name="grantResults">Not in use</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// Opens an activity with the user's profile.
        /// </summary>
        private void ViewProfile()
        {
            Intent intent = new Intent(this, typeof(ProfileActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// Handles click events for views in the activity.
        /// </summary>
        /// <param name="v">The view that was clicked.</param>
        public void OnClick(View v)
        {
            if (v == ibtnProfile)
                ViewProfile();
            else if (v == btnNewCommunity)
                OpenCreateCommunityActivity();
        }

        /// <summary>
        /// Handles item click events in the ListView of communities.
        /// </summary>
        /// <param name="parent">The AdapterView where the click happened.</param>
        /// <param name="view">The view within the AdapterView that was clicked.</param>
        /// <param name="position">The position of the view in the adapter.</param>
        /// <param name="id">The row id of the item that was clicked.</param>
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            ViewCommunity(communities[position]);
        }

        /// <summary>
        /// Opens the activity to view details of a community.
        /// </summary>
        /// <param name="community">The community to view.</param>
        private void ViewCommunity(Community community)
        {
            Intent intent = new Intent(this, typeof(ViewCommunityActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// Opens the activity to create a new community.
        /// </summary>
        private void OpenCreateCommunityActivity()
        {
            Intent intent = new Intent(this, typeof(CreateCommunityActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// Initiates the process of retrieving communities.
        /// </summary>
        private void GetCommunities()
        {
            tskGetCommunities = communities.GetCommunities().AddOnCompleteListener(this);
        }

        /// <summary>
        /// Disables the back button functionality.
        /// </summary>
#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed() { }
#pragma warning restore CS0672 // Member overrides obsolete member

        /// <summary>
        /// Handles event notifications from Firebase Firestore.
        /// </summary>
        /// <param name="obj">The event object.</param>
        /// <param name="error">The error that occurred, if any.</param>
        public void OnEvent(Object obj, FirebaseFirestoreException error)
        {
            GetCommunities();
        }

        /// <summary>
        /// Handles the completion of asynchronous tasks.
        /// </summary>
        /// <param name="task">The completed task.</param>
        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                if (task == tskGetCommunities)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    communities.AddCommunities(qs.Documents);
                }
            }
        }

        /// <summary>
        /// Called when the activity receives a result from another activity.
        /// </summary>
        /// <param name="requestCode">Not in use</param>
        /// <param name="resultCode">The result code.</param>
        /// <param name="data">The intent data.</param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
                user = User.GetUserJson(data.GetStringExtra(General.KEY_USER));
            base.OnActivityResult(requestCode, resultCode, data);
        }

        /// <summary>
        /// Called when the activity is resumed and visible to the user.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            communities.AddSnapshotListener(this);
        }

        /// <summary>
        /// Called when the activity is paused and no longer visible to the user.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            communities.RemoveSnapshotListener();
        }
    }
}