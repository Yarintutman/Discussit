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
using Firebase.Firestore.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discussit
{
    /// <summary>
    /// Represents the activity for managing a community.
    /// </summary>
    [Activity(Label = "ManageCommunityActivity")]
    public class ManageCommunityActivity : AppCompatActivity, View.IOnClickListener, IOnCompleteListener, IEventListener
    {
        User user;
        Community community;
        ImageButton ibtnLogo, ibtnBack;
        EditText etCommunityName, etCommunityDescription;
        Button btnSaveChanges;
        TextView tvMemberCount;
        Members members;
        Task tskGetMembers;
        Member userAsMember, currentMember;

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
            etCommunityName = FindViewById<EditText>(Resource.Id.etCommunityName);
            etCommunityDescription = FindViewById<EditText>(Resource.Id.etCommunityDescription);
            ListView lvMembers = FindViewById<ListView>(Resource.Id.lvMembers);
            tvMemberCount = FindViewById<TextView>(Resource.Id.tvMemberCount);
            btnSaveChanges = FindViewById<Button>(Resource.Id.btnSaveChanges);
            ibtnLogo.SetOnClickListener(this);
            ibtnBack.SetOnClickListener(this);
            btnSaveChanges.SetOnClickListener(this);
            etCommunityName.Text = community.Name;
            etCommunityDescription.Text = community.Description;
            community.CreateMembers(this);
            members = community.Members;
            lvMembers.Adapter = members.MemberAdapter;
            RegisterForContextMenu(lvMembers);
            members.AddSnapshotListener(this);
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
        /// Called when creating a context menu for list view items.
        /// </summary>
        /// <param name="menu">The context menu that is being built.</param>
        /// <param name="v">The view for which the context menu is being created.</param>
        /// <param name="menuInfo">Extra information about the item for which the context menu should be shown.</param>
        public override void OnCreateContextMenu(Android.Views.IContextMenu menu, Android.Views.View v, Android.Views.IContextMenuContextMenuInfo menuInfo)
        {
            AdapterView.AdapterContextMenuInfo info = menuInfo as AdapterView.AdapterContextMenuInfo;
            if (info != null)
            {
                int position = info.Position;
                currentMember = members[position];
                userAsMember = members.GetMemberByUID(user.Id);
                if (userAsMember!= null && userAsMember.IsHigherRank(currentMember) && userAsMember != currentMember)
                {
                    MenuInflater.Inflate(Resource.Menu.menu_manageMember, menu);
                    base.OnCreateContextMenu(menu, v, menuInfo);
                }
            }
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
            return base.OnContextItemSelected(item);
        }

        /// <summary>
        /// Displays a confirmation dialog before transferring ownership.
        /// </summary>
        private void ConfirmTransferOwner()
        {
            //dialog confirmation to be added
            members.TransferLeader((Leader)userAsMember, currentMember);
        }

        /// <summary>
        /// Promotes the member within the community.
        /// </summary>
        private void Promote()
        {
            if (currentMember is Admin)
                ConfirmTransferOwner();
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
    }
}