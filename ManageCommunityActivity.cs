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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_manageCommunity);
            InitObjects();
            InitViews();
        }

        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
            community = Community.GetCommunityJson(Intent.GetStringExtra(General.KEY_COMMUNITY));
        }

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

        private void Back()
        {
            Intent intent = new Intent();
            intent.PutExtra(General.KEY_USER, user.GetJson());
            SetResult(Result.Ok, intent);
            Finish();
        }

        private void ReturnToHub()
        {
            Intent intent = new Intent(this, typeof(CommunityHubActivity));
            intent.AddFlags(ActivityFlags.LaunchedFromHistory);
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivity(intent);
            Finish();
        }

#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed()
        {
            Back();
        }
#pragma warning restore CS0672 // Member overrides obsolete member

        private bool ValidInputFields()
        {
            bool status = true;
            status = status && (etCommunityName.Text != string.Empty);
            status = status && (etCommunityDescription.Text != string.Empty);
            return status;
        }

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

        public void OnClick(View v)
        {
            if (v == ibtnLogo)
                ReturnToHub();
            else if (v == ibtnBack)
                Back();
            else if (v == btnSaveChanges)
                Save();
        }

        private void GetMembers()
        {
            tskGetMembers = community.GetMembers().AddOnCompleteListener(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            members?.AddSnapshotListener(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            members?.RemoveSnapshotListener();
        }

        public void OnComplete(Task task)
        {
            if (task.IsComplete)
            {
                if (task == tskGetMembers)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    members.AddMembers(qs.Documents);
                    tvMemberCount.Text = members.MemberCount.ToString();
                    if (userAsMember == null)
                        userAsMember = members.GetMemberByUID(user.Id);
                }
            }
        }

        public void OnEvent(Java.Lang.Object obj, FirebaseFirestoreException error)
        {
            GetMembers();
        }

        public override void OnCreateContextMenu(Android.Views.IContextMenu menu, Android.Views.View v, Android.Views.IContextMenuContextMenuInfo menuInfo)
        {
            AdapterView.AdapterContextMenuInfo info = menuInfo as AdapterView.AdapterContextMenuInfo;
            if (info != null)
            {
                int position = info.Position;
                currentMember = members[position];
                if (userAsMember.IsHigherRank(currentMember) && userAsMember != currentMember)
                {
                    MenuInflater.Inflate(Resource.Menu.menu_manageMember, menu);
                    base.OnCreateContextMenu(menu, v, menuInfo);
                }
            }
        }

        public override bool OnContextItemSelected(Android.Views.IMenuItem item)
        {
            if (item.ItemId == Resource.Id.itemPromote)
                Promote(currentMember);
            else if (item.ItemId == Resource.Id.itemDemote)
                Demote(currentMember);
            return base.OnContextItemSelected(item);
        }

        private void ConfirmTransferOwner()
        {
            //dialog confirmation to be added
            members.TransferLeader((Leader)userAsMember, currentMember);
        }

        private void Promote(Member currentMember)
        {
            if (currentMember is Admin)
                ConfirmTransferOwner();
            else
                members.Promote(currentMember);
        }

        private void Demote(Member currentMember)
        {
            if (currentMember is Admin)
                members.Demote((Admin)currentMember);
            else
                Toast.MakeText(this, Resources.GetString(Resource.String.memberDemote), ToastLength.Short).Show();
        }
    }
}