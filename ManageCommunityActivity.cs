using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Firestore;
using System;

namespace Discussit
{
    /// <summary>
    /// Represents the activity for managing a community.
    /// </summary>
    [Activity(Label = "ManageCommunityActivity")]
    public class ManageCommunityActivity : AppCompatActivity, View.IOnClickListener, IOnCompleteListener, IEventListener
    {
        User user, currentMemberAsUser;
        Community community;
        ImageButton ibtnLogo, ibtnBack, ibtnSearch, ibtnClearSearch;
        EditText etCommunityName, etCommunityDescription, etSearchBar;
        Button btnSaveChanges;
        TextView tvMemberCount, tvSortBy;
        Members members;
        Task tskGetMembers, tskGetMemberAsUser;
        Member userAsMember, currentMember;
        Dialog currentDialog;
        string sort;

        /// <summary>
        /// Called when the activity is first created.
        /// </summary>
        /// <param name="savedInstanceState">Not in use</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_manageCommunity);
            InitObjects();
            InitViews();
        }

        /// <summary>
        /// Initializes user and community objects.
        /// </summary>
        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
            community = Community.GetCommunityJson(Intent.GetStringExtra(General.KEY_COMMUNITY));
        }

        /// <summary>
        /// Initializes views and sets up event listeners.
        /// </summary>
        private void InitViews()
        {
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            ibtnLogo = FindViewById<ImageButton>(Resource.Id.ibtnLogo);
            ibtnSearch = FindViewById<ImageButton>(Resource.Id.ibtnSearch);
            ibtnClearSearch = FindViewById<ImageButton>(Resource.Id.ibtnClearSearchBar);
            etCommunityName = FindViewById<EditText>(Resource.Id.etCommunityName);
            etCommunityDescription = FindViewById<EditText>(Resource.Id.etCommunityDescription);
            etSearchBar = FindViewById<EditText>(Resource.Id.etSearchBar);
            ListView lvMembers = FindViewById<ListView>(Resource.Id.lvMembers);
            tvMemberCount = FindViewById<TextView>(Resource.Id.tvMemberCount);
            tvSortBy = FindViewById<TextView>(Resource.Id.tvSortBy);
            btnSaveChanges = FindViewById<Button>(Resource.Id.btnSaveChanges);
            ibtnLogo.SetOnClickListener(this);
            ibtnBack.SetOnClickListener(this);
            ibtnSearch.SetOnClickListener(this);
            ibtnClearSearch.SetOnClickListener(this);
            btnSaveChanges.SetOnClickListener(this);
            etCommunityName.Text = community.Name;
            etCommunityDescription.Text = community.Description;
            community.CreateMembers(this);
            members = community.Members;
            lvMembers.Adapter = members.MemberAdapter;
            members.AddSnapshotListener(this);
            RegisterForContextMenu(tvSortBy);
            RegisterForContextMenu(lvMembers);
            sort = Resources.GetString(Resource.String.sortbyRank);
            SetSorting(sort);
        }

        /// <summary>
        /// Sets the sorting text view with the specified sorting criteria.
        /// </summary>
        /// <param name="sortBy">The sorting criteria to display.</param>
        private void SetSorting(string sortBy)
        {
            if (sortBy == Resources.GetString(Resource.String.sortbyRank))
                sort = sortBy;
            else if (sortBy == Resources.GetString(Resource.String.sortbyJoinDate))
                sort = sortBy;
            else if (sortBy == Resources.GetString(Resource.String.sortbyName))
                sort = sortBy;
            tvSortBy.Text = Resources.GetString(Resource.String.sortby) + " " + sort;
            SortMembers();
        }

        /// <summary>
        /// Sort the members by the saved sorting format
        /// </summary>
        private void SortMembers()
        {
            if (sort == Resources.GetString(Resource.String.sortbyRank))
                members.SortByRank();
            else if (sort == Resources.GetString(Resource.String.sortbyJoinDate))
                members.SortByJoinDate();
            else if (sort == Resources.GetString(Resource.String.sortbyName))
                members.SortByName();
        }

        /// <summary>
        /// Clears the search bar, hides the clear search button, and clears the search results in the members list.
        /// </summary>
        private void ClearSearch()
        {
            etSearchBar.Text = "";
            ibtnClearSearch.Visibility = ViewStates.Gone;
            members.ClearSearch();
        }

        /// <summary>
        /// Initiates a search based on the text entered in the search bar. Updates the search results in the members list accordingly.
        /// </summary>
        private void Search()
        {
            if (etSearchBar.Text != "")
            {
                members.Search(etSearchBar.Text);
                ibtnClearSearch.Visibility = ViewStates.Visible;
            }
            else
            {
                ClearSearch();
            }
        }

        /// <summary>
        /// Returns to the previous activity.
        /// </summary>
        private void Back()
        {
            Intent intent = new Intent();
            intent.PutExtra(General.KEY_USER, user.GetJson());
            SetResult(Result.Ok, intent);
            Finish();
        }

        /// <summary>
        /// Returns to the community hub.
        /// </summary>
        private void ReturnToHub()
        {
            Intent intent = new Intent(this, typeof(CommunityHubActivity));
            intent.AddFlags(ActivityFlags.LaunchedFromHistory);
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivity(intent);
            Finish();
        }

        /// <summary>
        /// Handles the back button press event.
        /// </summary>
#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed()
        {
            Back();
        }
#pragma warning restore CS0672 // Member overrides obsolete member

        /// <summary>
        /// Validates input fields.
        /// </summary>
        /// <returns>True if input fields are valid, otherwise false.</returns>
        private bool ValidInputFields()
        {
            bool status = true;
            status = status && (etCommunityName.Text != string.Empty);
            status = status && (etCommunityDescription.Text != string.Empty);
            return status;
        }

        /// <summary>
        /// Saves changes made to the community.
        /// </summary>
        private void Save()
        {
            if (ValidInputFields())
            {
                community.Name = etCommunityName.Text;
                community.Description = etCommunityDescription.Text;
                etCommunityName.Text = community.Name;
                etCommunityDescription.Text = community.Description;
                community.SaveChanges();
                Toast.MakeText(this, Resources.GetString(Resource.String.savedChanges), ToastLength.Short).Show();
            }
        }

        /// <summary>
        /// Handles click events for views.
        /// </summary>
        /// <param name="v">The view that was clicked.</param>
        public void OnClick(View v)
        {
            if (v == ibtnLogo)
                ReturnToHub();
            else if (v == ibtnBack)
                Back();
            else if (v == btnSaveChanges)
                Save();
            else if (v == ibtnSearch)
                Search();
            else if (v == ibtnClearSearch)
                ClearSearch();
        }

        /// <summary>
        /// Retrieves members of the community.
        /// </summary>
        private void GetMembers()
        {
            tskGetMembers = community.GetMembers().AddOnCompleteListener(this);
        }


        /// <summary>
        /// Called when the activity is resumed. Adds a snapshot listener for members.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            members?.AddSnapshotListener(this);
        }

        /// <summary>
        /// Called when the activity is paused. Removes the snapshot listener for members.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            members?.RemoveSnapshotListener();
        }

        /// <summary>
        /// Handles completion of asynchronous tasks.
        /// </summary>
        /// <param name="task">The completed task.</param>
        public void OnComplete(Task task)
        {
            if (task.IsComplete)
            {
                if (task == tskGetMembers)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    members.AddMembers(qs.Documents);
                    tvMemberCount.Text = members.MemberCount.ToString();
                    SortMembers();
                }
                else if (task == tskGetMemberAsUser)
                {
                    DocumentSnapshot ds = (DocumentSnapshot)task.Result;
                    currentMemberAsUser.SetUser(ds);
                    KickMember();
                }
            }
        }

        /// <summary>
        /// Handles events for Firestore snapshot changes.
        /// </summary>
        /// <param name="obj">The object containing event data.</param>
        /// <param name="error">The error encountered, if any.</param>
        public void OnEvent(Java.Lang.Object obj, FirebaseFirestoreException error)
        {
            GetMembers();
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
                MenuInflater.Inflate(Resource.Menu.menu_sortMembers, menu);
            }
            else
            {
                AdapterView.AdapterContextMenuInfo info = menuInfo as AdapterView.AdapterContextMenuInfo;
                if (info != null)
                {
                    int position = info.Position;
                    currentMember = members[position];
                    userAsMember = members.GetMemberByUID(user.Id);
                    if (userAsMember!= null && userAsMember.IsHigherRank(currentMember) && userAsMember != currentMember)
                    {
                        if (userAsMember is Leader)
                            menu.Add(Resources.GetString(Resource.String.transferOwner));
                        MenuInflater.Inflate(Resource.Menu.menu_manageMember, menu);
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
            if (item.ItemId == Resource.Id.itemPromote)
                Promote();
            else if (item.ItemId == Resource.Id.itemDemote)
                Demote();
            else if (item.TitleFormatted.ToString() == Resources.GetString(Resource.String.transferOwner))
                ConfirmTransferOwner(Resources.GetString(Resource.String.confirmTransferOwner));
            else if (item.ItemId == Resource.Id.itemKick)
                ConfirmKickMember();
            else if (item.ItemId == Resource.Id.itemSortByRank)
                SetSorting(Resources.GetString(Resource.String.sortbyRank));
            else if (item.ItemId == Resource.Id.itemSortByJoinDate)
                SetSorting(Resources.GetString(Resource.String.sortbyJoinDate));
            else if (item.ItemId == Resource.Id.itemSortbyName)
                SetSorting(Resources.GetString(Resource.String.sortbyName));
            return base.OnContextItemSelected(item);
        }

        /// <summary>
        /// Displays a confirmation dialog for kicking a member.
        /// </summary>
        private void ConfirmKickMember()
        {
            currentDialog = new Dialog(this);
            currentDialog.SetContentView(Resource.Layout.dialog_confirm);
            TextView tvDialogTitle = currentDialog.FindViewById<TextView>(Resource.Id.tvTitle);
            Button btnConfirm = currentDialog.FindViewById<Button>(Resource.Id.btnConfirm);
            Button btnCancel = currentDialog.FindViewById<Button>(Resource.Id.btnCancel);
            tvDialogTitle.Text = Resources.GetString(Resource.String.confirmKickMember) + " " + currentMember.Name + "?";
            btnConfirm.Click += new EventHandler(GetUser);
            btnConfirm.Click += new EventHandler(CancelDialog);
            btnCancel.Click += new EventHandler(CancelDialog);
            currentDialog.Show();
        }

        /// <summary>
        /// Displays a confirmation dialog for transferring ownership.
        /// </summary>
        /// <param name="dialogText">The text to be displayed in the dialog.</param>
        private void ConfirmTransferOwner(string dialogText)
        {
            currentDialog = new Dialog(this);
            currentDialog.SetContentView(Resource.Layout.dialog_confirm);
            TextView tvDialogTitle = currentDialog.FindViewById<TextView>(Resource.Id.tvTitle);
            Button btnConfirm = currentDialog.FindViewById<Button>(Resource.Id.btnConfirm);
            Button btnCancel = currentDialog.FindViewById<Button>(Resource.Id.btnCancel);
            tvDialogTitle.Text = dialogText + " " + currentMember.Name + "?";
            btnConfirm.Click += new EventHandler(TransferOwner);
            btnConfirm.Click += new EventHandler(CancelDialog);
            btnCancel.Click += new EventHandler(CancelDialog);
            currentDialog.Show();
        }

        /// <summary>
        /// Displays a confirmation dialog for promoting a admin.
        /// </summary>
        private void ConfirmPromoteAdmin()
        {
            ConfirmTransferOwner(Resources.GetString(Resource.String.confirmPromoteAdmin));
        }

        /// <summary>
        /// Promotes the member within the community.
        /// </summary>
        private void Promote()
        {
            if (currentMember is Admin)
                ConfirmPromoteAdmin();
            else
                members.Promote(currentMember);
        }

        /// <summary>
        /// Demotes the member within the community.
        /// </summary>
        private void Demote()
        {
            if (currentMember is Admin)
                members.Demote((Admin)currentMember);
            else
                Toast.MakeText(this, Resources.GetString(Resource.String.memberDemote), ToastLength.Short).Show();
        }

        /// <summary>
        /// Transfers ownership to another member.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        public void TransferOwner(object sender, EventArgs e)
        {
            members.TransferLeader((Leader)userAsMember, currentMember);
        }

        /// <summary>
        /// Kick a member from the community
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        public void GetUser(object sender, EventArgs e)
        {
            currentMemberAsUser = new User(currentMember.UserID);
            tskGetMemberAsUser = currentMemberAsUser.GetUserData().AddOnCompleteListener(this);
        }

        /// <summary>
        /// Kick a member from the community
        /// </summary>
        public void KickMember()
        {
            community.KickUser(currentMemberAsUser);
        }

        /// <summary>
        /// Cancels the current dialog.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        public void CancelDialog(object sender, EventArgs e)
        {
            currentDialog.Cancel();
        }
    }
}