using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Firestore;
using System;
using System.Collections.Generic;

namespace Discussit
{
    /// <summary>
    /// Represents the user profile activity, allowing users to view and manage their profile information.
    /// </summary>
    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : AppCompatActivity, View.IOnClickListener, IOnCompleteListener, AdapterView.IOnItemClickListener
    {
        User user;
        ImageButton ibtnPicture, ibtnBack, ibtnLogout, ibtnCloseDialog;
        Button btnViewCommunities, btnViewPosts, btnViewComments, btnManageCommunities, btnSettings;
        TextView tvSortBy;
        Dialog dialogViewCommunities, dialogViewPosts, dialogViewComments, dialogViewManagedCommunities;
        Task tskGetCommunities, tskGetPosts, tskGetComments, tskGetManagedCommunities;
        CommunityAdapter communities, managedCommunities;
        PostAdapter posts;
        CommunityAdapter comments;
        string currentDialog;

        /// <summary>
        /// Initializes the activity when created.
        /// </summary>
        /// <param name="savedInstanceState">Not in use</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_profile);
            InitObjects();
            InitViews();
        }

        /// <summary>
        /// Initializes the objects used in the activity.
        /// </summary>
        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
        }

        /// <summary>
        /// Initializes the views used in the activity.
        /// </summary>
        private void InitViews()
        {
            ibtnPicture = FindViewById<ImageButton>(Resource.Id.ibtnPicture);
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            ibtnLogout = FindViewById<ImageButton>(Resource.Id.ibtnLogout);
            TextView tvUsername = FindViewById<TextView>(Resource.Id.tvUsername);
            btnViewCommunities = FindViewById<Button>(Resource.Id.btnViewCommunities);
            btnViewPosts = FindViewById<Button>(Resource.Id.btnViewPosts);
            btnViewComments = FindViewById<Button>(Resource.Id.btnViewComments);
            btnManageCommunities = FindViewById<Button>(Resource.Id.btnManageCommunities);
            btnSettings = FindViewById<Button>(Resource.Id.btnSettings);
            tvUsername.Text = user.Username;
            ibtnPicture.SetOnClickListener(this);
            ibtnBack.SetOnClickListener(this);
            ibtnLogout.SetOnClickListener(this);
            btnViewCommunities.SetOnClickListener(this);
            btnViewPosts.SetOnClickListener(this);
            btnViewComments.SetOnClickListener(this);
            btnManageCommunities.SetOnClickListener(this);
            btnSettings.SetOnClickListener(this);
        }

        /// <summary>
        /// Sets the sorting criteria for the displayed data.
        /// </summary>
        /// <param name="sortBy">The sorting format.</param>
        public void SetSorting(string sortBy)
        {
            tvSortBy.Text = Resources.GetString(Resource.String.sortBy) + " " + sortBy;
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
        /// Disables the default back button behavior and invokes the custom back method.
        /// </summary>
#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed()
        {
            Back();
        }
#pragma warning restore CS0672 // Member overrides obsolete member

        /// <summary>
        /// Logs the user out of the application, returning him to the login/register activity
        /// </summary>
        private void Logout()
        {
            SpData spd = new SpData(General.SP_FILE_NAME);
            spd.PutBool(General.KEY_REGISTERED, false);
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivityForResult(intent, 0);
            Finish();
        }

        /// <summary>
        /// Navigates to the settings activity.
        /// </summary>
        private void ViewSettings()
        {
            Intent intent = new Intent(this, typeof(SettingsActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// Displays a dialog to select and view communities.
        /// </summary>
        private void ViewCommunities()
        {
            dialogViewCommunities = new Dialog(this);
            dialogViewCommunities.SetContentView(Resource.Layout.dialog_selectCommunity);
            dialogViewCommunities.Show();
            dialogViewCommunities.SetCancelable(true);
            ibtnCloseDialog = dialogViewCommunities.FindViewById<ImageButton>(Resource.Id.ibtnCloseDialog);
            ibtnCloseDialog.SetOnClickListener(this);
            currentDialog = Resources.GetString(Resource.String.Communities);
            tvSortBy = dialogViewCommunities.FindViewById<TextView>(Resource.Id.tvSortBy);
            SetSorting(Resources.GetString(Resource.String.sortDate));
            if (user.Communities.Size() != 0)
            {
                tskGetCommunities = user.GetDocumentInList(General.FIELD_USER_COMMUNITIES);
                tskGetCommunities?.AddOnCompleteListener(this);
            }
        }

        /// <summary>
        /// Displays a dialog to select and manage the communities managed by the user.
        /// </summary>
        private void ViewManagedCommunities()
        {
            dialogViewManagedCommunities = new Dialog(this);
            dialogViewManagedCommunities.SetContentView(Resource.Layout.dialog_selectCommunity);
            dialogViewManagedCommunities.Show();
            dialogViewManagedCommunities.SetCancelable(true);
            ibtnCloseDialog = dialogViewManagedCommunities.FindViewById<ImageButton>(Resource.Id.ibtnCloseDialog);
            ibtnCloseDialog.SetOnClickListener(this);
            currentDialog = Resources.GetString(Resource.String.ManagedCommunities);
            tvSortBy = dialogViewManagedCommunities.FindViewById<TextView>(Resource.Id.tvSortBy);
            SetSorting(Resources.GetString(Resource.String.sortDate));
            if (user.ManagingCommunities.Size() != 0)
            {
                tskGetManagedCommunities = user.GetDocumentInList(General.FIELD_USER_MANAGING_COMMUNITIES);
                tskGetManagedCommunities?.AddOnCompleteListener(this);
            }
        }

        /// <summary>
        /// Displays a dialog to select and view posts.
        /// </summary>
        private void ViewPosts()
        {
            dialogViewPosts = new Dialog(this);
            dialogViewPosts.SetContentView(Resource.Layout.dialog_selectPost);
            dialogViewPosts.Show();
            dialogViewPosts.SetCancelable(true);
            ibtnCloseDialog = dialogViewPosts.FindViewById<ImageButton>(Resource.Id.ibtnCloseDialog);
            ibtnCloseDialog.SetOnClickListener(this);
            currentDialog = Resources.GetString(Resource.String.Posts);
            tvSortBy = dialogViewPosts.FindViewById<TextView>(Resource.Id.tvSortBy);
            SetSorting(Resources.GetString(Resource.String.sortDate));
            if (user.Posts.Size() != 0)
            {
                tskGetPosts = user.GetDocumentInList(General.FIELD_USER_POSTS);
                tskGetPosts?.AddOnCompleteListener(this);
            }
        }

        /// <summary>
        /// Sets the managed communities in the dialog view based on the provided documents.
        /// </summary>
        /// <param name="documents">Documents representing the managed communities.</param>
        private void SetManagedCommunities(IList<DocumentSnapshot> documents)
        {
            managedCommunities = new CommunityAdapter(dialogViewManagedCommunities.Context);
            managedCommunities.SetCommunities(documents);
            ListView lvCommunities = dialogViewManagedCommunities.FindViewById<ListView>(Resource.Id.lvCommunities);
            lvCommunities.Adapter = managedCommunities;
            lvCommunities.OnItemClickListener = this;
        }

        /// <summary>
        /// Sets the communities in the dialog view based on the provided documents.
        /// </summary>
        /// <param name="documents">Documents representing the communities.</param>
        private void SetCommunities(IList<DocumentSnapshot> documents)
        {
            communities = new CommunityAdapter(dialogViewCommunities.Context);
            communities.SetCommunities(documents);
            ListView lvCommunities = dialogViewCommunities.FindViewById<ListView>(Resource.Id.lvCommunities);
            lvCommunities.Adapter = communities;
            lvCommunities.OnItemClickListener = this;
        }

        /// <summary>
        /// Sets the posts in the dialog view based on the provided documents.
        /// </summary>
        /// <param name="documents">Documents representing the posts.</param>
        private void SetPosts(IList<DocumentSnapshot> documents)
        {
            posts = new PostAdapter(dialogViewPosts.Context);
            posts.SetPosts(documents);
            ListView lvPosts = dialogViewPosts.FindViewById<ListView>(Resource.Id.lvPosts);
            lvPosts.Adapter = posts;
            lvPosts.OnItemClickListener = this;
        }

        /// <summary>
        /// Handles click events for various UI elements.
        /// </summary>
        /// <param name="v">The view that was clicked.</param>
        public void OnClick(View v)
        {
            if (v == ibtnBack)
                Back();
            else if (v == ibtnLogout)
                Logout();
            else if (v == btnSettings)
                ViewSettings();
            else if (v == btnManageCommunities)
                ViewManagedCommunities();
            else if (v == btnViewCommunities)
                ViewCommunities();
            else if (v == btnViewPosts) { }
            //ViewPosts();
            else if (v == btnViewComments) { }
            else if (v == ibtnCloseDialog)
            {
                if (currentDialog == Resources.GetString(Resource.String.ManagedCommunities))
                    dialogViewManagedCommunities.Cancel();
                else if (currentDialog == Resources.GetString(Resource.String.Communities))
                    dialogViewCommunities.Cancel();
                else if (currentDialog == Resources.GetString(Resource.String.Posts))
                    dialogViewPosts.Cancel();
                else if (currentDialog == Resources.GetString(Resource.String.Comments))
                    dialogViewComments.Cancel();
            }
        }

        /// <summary>
        /// Handles task completion events.
        /// </summary>
        /// <param name="task">The completed task.</param>
        public void OnComplete(Task task)
        {
            if (task.IsComplete)
            {
                if (task == tskGetManagedCommunities)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    SetManagedCommunities(qs.Documents);
                }
                else if (task == tskGetCommunities)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    SetCommunities(qs.Documents);
                }
            }
        }

        /// <summary>
        /// Navigates to the activity to view a post.
        /// </summary>
        /// <param name="post">The post to view.</param>
        private void ViewPost(Post post)
        {
            Intent intent = new Intent(this, typeof(ViewPostActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_POST, post.GetJson());
            StartActivityForResult(intent, 0);
            dialogViewPosts.Cancel();
        }

        /// <summary>
        /// Navigates to the activity to view a community.
        /// </summary>
        /// <param name="community">The community to view.</param>
        private void ViewCommunity(Community community)
        {
            Intent intent = new Intent(this, typeof(ViewCommunityActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivityForResult(intent, 0);
            dialogViewCommunities.Cancel();
        }

        /// <summary>
        /// Navigates to the activity to manage a community.
        /// </summary>
        /// <param name="community">The community to manage.</param>
        private void ManageCommunity(Community community)
        {
            Intent intent = new Intent(this, typeof(ManageCommunityActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivityForResult(intent, 0);
            dialogViewManagedCommunities.Cancel();
        }

        /// <summary>
        /// Handles item click events in the list views.
        /// </summary>
        /// <param name="parent">Not in use</param>
        /// <param name="view">The view within the AdapterView that was clicked.</param>
        /// <param name="position">The position of the view in the adapter.</param>
        /// <param name="id">The row id of the item that was clicked.</param>
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            if (currentDialog == Resources.GetString(Resource.String.ManagedCommunities))
                ManageCommunity(managedCommunities[position]);
            else if (currentDialog == Resources.GetString(Resource.String.Communities))
                ViewCommunity(communities[position]);
            else if (currentDialog == Resources.GetString(Resource.String.Posts))
                ViewPost(posts[position]);
            else if (currentDialog == Resources.GetString(Resource.String.Comments)) { }
                //TBD
        }

        /// <summary>
        /// Handles the result of an activity.
        /// </summary>
        /// <param name="requestCode">Not in use</param>
        /// <param name="resultCode">The integer result code returned by the child activity.</param>
        /// <param name="data">An Intent, which can return result data to the caller.</param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
                user = User.GetUserJson(data.GetStringExtra(General.KEY_USER));
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}