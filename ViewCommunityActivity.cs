﻿using Android.App;
using Android.Content;
using Android.Content.Res;
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
    /// <summary>
    /// Activity for viewing a new community.
    /// </summary>
    [Activity(Label = "ViewCommunityActivity")]
    public class ViewCommunityActivity : AppCompatActivity, View.IOnClickListener, AdapterView.IOnItemClickListener, IEventListener, IOnCompleteListener
    {
        User user;
        Community community;
        Posts posts;
        Members members;
        ImageButton ibtnBack, ibtnLogo, ibtnProfile, ibtnSearch, ibtnClearSearch;
        TextView tvCommunityName, tvDescription, tvMemberCount, tvSortBy;
        Button btnNewPost, btnJoinCommunity;
        EditText etSearchBar;
        Task tskGetPosts, tskGetMemebers;
        Post currentPost;
        string sort;

        /// <summary>
        /// Called when the activity is starting.
        /// </summary>
        /// <param name="savedInstanceState">Not in use</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_viewCommunity);
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
            community.CreatePosts(this);
            community.CreateMembers(this);
            posts = community.Posts;
            members = community.Members;
        }

        /// <summary>
        /// Initializes the views used in the activity.
        /// </summary>
        private void InitViews()
        {
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            ibtnLogo = FindViewById<ImageButton>(Resource.Id.ibtnLogo);
            ibtnProfile = FindViewById<ImageButton>(Resource.Id.ibtnProfile);
            ibtnSearch = FindViewById<ImageButton>(Resource.Id.ibtnSearch);
            ibtnClearSearch = FindViewById<ImageButton>(Resource.Id.ibtnClearSearchBar);
            tvCommunityName = FindViewById<TextView>(Resource.Id.tvCommunityName);
            tvDescription = FindViewById<TextView>(Resource.Id.tvDescription);
            tvMemberCount = FindViewById<TextView>(Resource.Id.tvMemberCount);
            tvSortBy = FindViewById<TextView>(Resource.Id.tvSortBy);
            btnNewPost = FindViewById<Button>(Resource.Id.btnNewPost);
            btnJoinCommunity = FindViewById<Button>(Resource.Id.btnJoinCommunity);
            etSearchBar = FindViewById<EditText>(Resource.Id.etSearchBar);
            ListView lvPosts = FindViewById<ListView>(Resource.Id.lvPosts);
            lvPosts.Adapter = posts.PostAdapter;
            lvPosts.OnItemClickListener = this;
            posts.AddSnapshotListener(this);
            members.AddSnapshotListener(this);
            ibtnBack.SetOnClickListener(this);
            ibtnLogo.SetOnClickListener(this);
            ibtnProfile.SetOnClickListener(this);
            ibtnSearch.SetOnClickListener(this);
            ibtnClearSearch.SetOnClickListener(this);
            btnNewPost.SetOnClickListener(this);
            btnJoinCommunity.SetOnClickListener(this);
            tvCommunityName.Text = community.Name;
            tvDescription.Text = community.Description;
            tvMemberCount.Text = community.MemberCount.ToString();
            RegisterForContextMenu(tvSortBy);
            RegisterForContextMenu(lvPosts);
            sort = Resources.GetString(Resource.String.sortbyComments);
            SetSorting(sort);
        }

        /// <summary>
        /// Checks the membership status of the user in the community and updates the UI accordingly.
        /// </summary>
        private void CheckMembership()
        {
            if (members.HasMember(user.Id))
                btnJoinCommunity.Visibility = ViewStates.Gone;
            else
                btnJoinCommunity.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// Sets the sorting text view with the specified sorting criteria.
        /// </summary>
        /// <param name="sortBy">The sorting criteria to display.</param>
        private void SetSorting(string sortBy)
        {
            if (sortBy == Resources.GetString(Resource.String.sortbyNew))
                sort = sortBy;
            else if (sortBy == Resources.GetString(Resource.String.sortbyOld))
                sort = sortBy;
            else if (sortBy == Resources.GetString(Resource.String.sortbyComments))
                sort = sortBy;
            tvSortBy.Text = Resources.GetString(Resource.String.sortby) + " " + sort;
            SortPosts();
        }

        /// <summary>
        /// Sort the posts by the saved sorting format
        /// </summary>
        private void SortPosts()
        {
            if (sort == Resources.GetString(Resource.String.sortbyNew))
                posts.SortByLatest();
            else if (sort == Resources.GetString(Resource.String.sortbyOld))
                posts.SortByOldest();
            else if (sort == Resources.GetString(Resource.String.sortbyComments))
                posts.SortByComments();
        }

        /// <summary>
        /// Clears the search bar, hides the clear search button, and clears the search results in the posts list.
        /// </summary>
        private void ClearSearch()
        {
            etSearchBar.Text = "";
            ibtnClearSearch.Visibility = ViewStates.Gone;
            posts.ClearSearch();
            SortPosts();
        }

        /// <summary>
        /// Initiates a search based on the text entered in the search bar. Updates the search results in the posts list accordingly.
        /// </summary>
        private void Search()
        {
            if (etSearchBar.Text != "")
            {
                posts.Search(etSearchBar.Text);
                ibtnClearSearch.Visibility = ViewStates.Visible;
            }
            else
            {
                ClearSearch();
            }
        }

        /// <summary>
        /// Opens the activity to create a new post.
        /// </summary>
        private void OpenCreatePostActivity()
        {
            Intent intent = new Intent(this, typeof(CreatePostActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivityForResult(intent, 0);
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
        /// Opens the profile activity to view the user's profile.
        /// </summary>
        private void ViewProfile()
        {
            Intent intent = new Intent(this, typeof(ProfileActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivityForResult(intent, 0);
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
        /// Handles click events for views in the activity.
        /// </summary>
        /// <param name="v">The view that was clicked.</param>
        public void OnClick(View v)
        {
            if (v == ibtnLogo)
                ReturnToHub();
            else if (v == ibtnBack)
                Back();
            else if (v == ibtnProfile)
                ViewProfile();
            else if (v == ibtnSearch)
                Search();
            else if (v == ibtnClearSearch)
                ClearSearch();
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

        /// <summary>
        /// Adds the user to the community and updates the user's community list.
        /// </summary>
        private void JoinCommunity()
        {
            community.AddMember(user);
            user.UpdateArrayField(General.FIELD_USER_COMMUNITIES, community.CollectionPath);
        }

        /// <summary>
        /// Opens the activity to view a specific post.
        /// </summary>
        /// <param name="post">The post to view.</param>
        private void ViewPost(Post post)
        {
            Intent intent = new Intent(this, typeof(ViewPostActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_POST, post.GetJson());
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivityForResult(intent, 0);
        }


        /// <summary>
        /// Handles the item click event in the list view.
        /// </summary>
        /// <param name="parent">The adapter view where the click happened.</param>
        /// <param name="view">The view within the adapter view that was clicked.</param>
        /// <param name="position">The position of the view in the adapter.</param>
        /// <param name="id">The row id of the item that was clicked.</param>
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            ViewPost(posts[position]);
        }

        /// <summary>
        /// Retrieves posts belonging to the community.
        /// </summary>
        private void GetPosts()
        {
            tskGetPosts = community.GetPosts().AddOnCompleteListener(this);
        }

        /// <summary>
        /// Retrieves members of the community.
        /// </summary>
        private void GetMembers()
        {
            tskGetMemebers = community.GetMembers().AddOnCompleteListener(this);
        }


        /// <summary>
        /// Handles events triggered by Firebase Firestore.
        /// </summary>
        /// <param name="obj">The object that triggered the event.</param>
        /// <param name="error">The error that occurred during the event, if any.</param>
        public void OnEvent(Object obj, FirebaseFirestoreException error)
        {
            GetPosts();
            GetMembers();
        }

        /// <summary>
        /// Handles task completion events.
        /// </summary>
        /// <param name="task">The completed task.</param>
        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                if (task == tskGetPosts)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    posts.AddPosts(qs.Documents);
                    SortPosts();
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

        /// <summary>
        /// Called when creating a context menu for a view.
        /// </summary>
        /// <param name="menu">The context menu that is being built.</param>
        /// <param name="v">The view for which the context menu is being created.</param>
        /// <param name="menuInfo">Extra information about the item for which the context menu should be shown.</param>
        public override void OnCreateContextMenu(Android.Views.IContextMenu menu, Android.Views.View v, Android.Views.IContextMenuContextMenuInfo menuInfo)
        {
            if (v == tvSortBy)
            {
                MenuInflater.Inflate(Resource.Menu.menu_sortPosts, menu);
            }
            else
            {
                AdapterView.AdapterContextMenuInfo info = (AdapterView.AdapterContextMenuInfo)menuInfo;
                if (info != null)
                {
                    int position = info.Position;
                    currentPost = posts[position];
                    Member userAsMember = members.GetMemberByUID(user.Id);
                    if (userAsMember != null && members.HasMember(user.Id) && (userAsMember is Admin || userAsMember.UserID == currentPost.CreatorUID))
                    {
                        MenuInflater.Inflate(Resource.Menu.menu_manageItem, menu);
                    }
                }
            }
            base.OnCreateContextMenu(menu, v, menuInfo);
        }

        /// <summary>
        /// Called when a context menu item is selected.
        /// </summary>
        /// <param name="item">The selected menu item.</param>
        /// <returns>True if the item selection was handled, otherwise false.</returns>
        public override bool OnContextItemSelected(Android.Views.IMenuItem item)
        {
            if (item.ItemId == Resource.Id.itemDelete)
            {
                community.RemovePost(currentPost.Id, members.GetMemberByUID(user.Id));
            }
            else if (item.ItemId == Resource.Id.itemEdit)
            {
                if (currentPost.CreatorUID == user.Id)
                    OpenPostForEdit(currentPost);
                else
                    Toast.MakeText(this, Resources.GetString(Resource.String.PostEdit), ToastLength.Short).Show();
            }
            else if (item.ItemId == Resource.Id.itemSortByNew)
                SetSorting(Resources.GetString(Resource.String.sortbyNew));
            else if (item.ItemId == Resource.Id.itemSortByOld)
                SetSorting(Resources.GetString(Resource.String.sortbyOld));
            else if (item.ItemId == Resource.Id.itemSortbyComments)
                SetSorting(Resources.GetString(Resource.String.sortbyComments));
            return base.OnContextItemSelected(item);
        }

        /// <summary>
        /// Opens CreatePost activity to edit the selected post
        /// </summary>
        /// <param name="post">The post to edit</param>
        private void OpenPostForEdit(Post post)
        {
            Intent intent = new Intent(this, typeof(CreatePostActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_POST, post.GetJson());
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// Handles the result of an activity started for a result.
        /// </summary>
        /// <param name="requestCode">The request code originally supplied to StartActivityForResult.</param>
        /// <param name="resultCode">The result code returned by the child activity.</param>
        /// <param name="data">An Intent, which can return result data to the caller.</param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
                user = User.GetUserJson(data.GetStringExtra(General.KEY_USER));
            base.OnActivityResult(requestCode, resultCode, data);
        }

        /// <summary>
        /// Called when the activity is resumed.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            posts.AddSnapshotListener(this);
            members.AddSnapshotListener(this);
        }

        /// <summary>
        /// Called when the activity is paused.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            posts.RemoveSnapshotListener();
            members.RemoveSnapshotListener();
        }
    }
}