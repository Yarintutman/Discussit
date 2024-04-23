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
        ImageButton ibtnBack, ibtnLogout, ibtnCloseDialog, ibtnSearch, ibtnClearSearch;
        EditText etSearchBar;
        Button btnViewCommunities, btnViewPosts, btnManageCommunities, btnSettings;
        Dialog dialogViewCommunities, dialogViewPosts, dialogViewManagedCommunities;
        Task tskGetCommunities, tskGetPosts, tskGetManagedCommunities, tskGetCommunity;
        CommunityAdapter communities, managedCommunities;
        PostAdapter posts;
        Dialog logoutDialog;
        Post currentPost;
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
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            ibtnLogout = FindViewById<ImageButton>(Resource.Id.ibtnLogout);
            TextView tvUsername = FindViewById<TextView>(Resource.Id.tvUsername);
            btnViewCommunities = FindViewById<Button>(Resource.Id.btnViewCommunities);
            btnViewPosts = FindViewById<Button>(Resource.Id.btnViewPosts);
            btnManageCommunities = FindViewById<Button>(Resource.Id.btnManageCommunities);
            btnSettings = FindViewById<Button>(Resource.Id.btnSettings);
            tvUsername.Text = user.Username;
            ibtnBack.SetOnClickListener(this);
            ibtnLogout.SetOnClickListener(this);
            btnViewCommunities.SetOnClickListener(this);
            btnViewPosts.SetOnClickListener(this);
            btnManageCommunities.SetOnClickListener(this);
            btnSettings.SetOnClickListener(this);
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
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void Logout(object sender, EventArgs e)
        {
            SpData spd = new SpData(General.SP_FILE_NAME);
            spd.PutBool(General.KEY_REGISTERED, false);
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivityForResult(intent, 0);
            Finish();
        }

        /// <summary>
        /// Cancels the logout dialog.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        public void CancelDialog(object sender, EventArgs e)
        {
            logoutDialog.Cancel();
        }

        /// <summary>
        /// Displays a confirmation dialog for logging out of the application
        /// </summary>
        private void ConfirmLogout()
        {
            logoutDialog = new Dialog(this);
            logoutDialog.SetContentView(Resource.Layout.dialog_confirm);
            TextView tvDialogTitle = logoutDialog.FindViewById<TextView>(Resource.Id.tvTitle);
            Button btnConfirm = logoutDialog.FindViewById<Button>(Resource.Id.btnConfirm);
            Button btnCancel = logoutDialog.FindViewById<Button>(Resource.Id.btnCancel);
            tvDialogTitle.Text = Resources.GetString(Resource.String.confirmLogout);
            btnConfirm.Click += new EventHandler(Logout);
            btnCancel.Click += new EventHandler(CancelDialog);
            logoutDialog.Show();
        }

        /// <summary>
        /// Clears the search bar, hides the clear search button, and clears the search results in the corresponding list.
        /// </summary>
        private void ClearSearch()
        {
            etSearchBar.Text = "";
            ibtnClearSearch.Visibility = ViewStates.Gone;
            if (currentDialog == Resources.GetString(Resource.String.ManagedCommunities))
                managedCommunities.ClearSearch();
            else if (currentDialog == Resources.GetString(Resource.String.Communities))
                communities.ClearSearch();
            else if (currentDialog == Resources.GetString(Resource.String.Posts))
                posts.ClearSearch();
        }

        /// <summary>
        /// Initiates a search based on the text entered in the search bar. Updates the search results in the corresponding list accordingly.
        /// </summary>
        private void Search()
        {
            if (etSearchBar.Text != "")
            {
                if (currentDialog == Resources.GetString(Resource.String.ManagedCommunities))
                    managedCommunities.Search(etSearchBar.Text);
                else if (currentDialog == Resources.GetString(Resource.String.Communities))
                    communities.Search(etSearchBar.Text);
                else if (currentDialog == Resources.GetString(Resource.String.Posts))
                    posts.Search(etSearchBar.Text);
                ibtnClearSearch.Visibility = ViewStates.Visible; 
            }
            else
            {
                ClearSearch();
            }
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
            etSearchBar = dialogViewCommunities.FindViewById<EditText>(Resource.Id.etSearchBar);
            ibtnCloseDialog = dialogViewCommunities.FindViewById<ImageButton>(Resource.Id.ibtnCloseDialog);
            ibtnSearch = dialogViewCommunities.FindViewById<ImageButton>(Resource.Id.ibtnSearch);
            ibtnClearSearch = dialogViewCommunities.FindViewById<ImageButton>(Resource.Id.ibtnClearSearchBar);
            ibtnCloseDialog.SetOnClickListener(this);
            ibtnSearch.SetOnClickListener(this);
            ibtnClearSearch.SetOnClickListener(this);
            currentDialog = Resources.GetString(Resource.String.Communities);
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
            etSearchBar = dialogViewManagedCommunities.FindViewById<EditText>(Resource.Id.etSearchBar);
            ibtnCloseDialog = dialogViewManagedCommunities.FindViewById<ImageButton>(Resource.Id.ibtnCloseDialog);
            ibtnSearch = dialogViewManagedCommunities.FindViewById<ImageButton>(Resource.Id.ibtnSearch);
            ibtnClearSearch = dialogViewManagedCommunities.FindViewById<ImageButton>(Resource.Id.ibtnClearSearchBar);
            ibtnCloseDialog.SetOnClickListener(this);
            ibtnSearch.SetOnClickListener(this);
            ibtnClearSearch.SetOnClickListener(this);
            currentDialog = Resources.GetString(Resource.String.ManagedCommunities);
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
            etSearchBar = dialogViewPosts.FindViewById<EditText>(Resource.Id.etSearchBar);
            ibtnCloseDialog = dialogViewPosts.FindViewById<ImageButton>(Resource.Id.ibtnCloseDialog);
            ibtnSearch = dialogViewPosts.FindViewById<ImageButton>(Resource.Id.ibtnSearch);
            ibtnClearSearch = dialogViewPosts.FindViewById<ImageButton>(Resource.Id.ibtnClearSearchBar);
            ibtnCloseDialog.SetOnClickListener(this);
            ibtnSearch.SetOnClickListener(this);
            ibtnClearSearch.SetOnClickListener(this);
            currentDialog = Resources.GetString(Resource.String.Posts);
            if (user.Posts.Size() != 0)
            {
                posts = new PostAdapter(dialogViewPosts.Context);
                tskGetPosts = posts.GetAllUserPosts(user.Id).AddOnCompleteListener(this);
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
            ListView lvPosts = dialogViewPosts.FindViewById<ListView>(Resource.Id.lvPosts);
            posts.SetPosts(documents);
            lvPosts.Adapter = posts;
            lvPosts.OnItemClickListener = this;
        }

        /// <summary>
        /// CLoses the current open dialog
        /// </summary>
        private void CloseDialog()
        {
            if (currentDialog == Resources.GetString(Resource.String.ManagedCommunities))
                dialogViewManagedCommunities.Cancel();
            else if (currentDialog == Resources.GetString(Resource.String.Communities))
                dialogViewCommunities.Cancel();
            else if (currentDialog == Resources.GetString(Resource.String.Posts))
                dialogViewPosts.Cancel();
        }

        /// <summary>
        /// Initiates the process of retrieving a community's data.
        /// <param name="post">The post inside the community to retrieve</param>
        /// </summary>
        private void GetCommunity(Post post)
        {
            for (int i = 0; i < user.Posts.Count && post.CommunityPath == null; i++)
                if (user.Posts[i].Contains(post.Id))
                    post.CommunityPath = General.RemoveFromString('/' + General.POSTS_COLLECTION, user.Posts[i]);
            currentPost = post;
            tskGetCommunity = post.GetCommunity().AddOnCompleteListener(this);
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
                ConfirmLogout();
            else if (v == btnSettings)
                ViewSettings();
            else if (v == btnManageCommunities)
                ViewManagedCommunities();
            else if (v == btnViewCommunities)
                ViewCommunities();
            else if (v == btnViewPosts)
                ViewPosts();
            else if (v == ibtnCloseDialog)
                CloseDialog();
            else if (v == ibtnSearch)
                Search();
            else if (v == ibtnClearSearch)
                ClearSearch();
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
                else if (task == tskGetPosts)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    SetPosts(qs.Documents);
                }
                else if (task == tskGetCommunity)
                {
                    DocumentSnapshot ds = (DocumentSnapshot)task.Result;
                    Community community = new Community(ds);
                    ViewPost(community, currentPost);
                }
            }
        }

        /// <summary>
        /// Navigates to the activity to view a post.
        /// </summary>
        /// <param name="community">The community of the post to view.</param>
        /// <param name="post">The post to view.</param>
        private void ViewPost(Community community,Post post)
        {
            Intent intent = new Intent(this, typeof(ViewPostActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
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
                GetCommunity(posts[position]);
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