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
        ImageButton ibtnLogo, ibtnBack, ibtnCloseDialog;
        EditText etCommunityName, etCommunityDescription;
        Button btnManageMembers, btnSaveChanges;
        TextView tvSortBy, tvMemberCount;
        Dialog dialogManageMemeber;
        Members members;
        Task tskGetMembers;

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
            btnManageMembers = FindViewById<Button>(Resource.Id.btnManageMembers);
            btnSaveChanges = FindViewById<Button>(Resource.Id.btnSaveChanges);
            ibtnLogo.SetOnClickListener(this);
            ibtnBack.SetOnClickListener(this);
            btnManageMembers.SetOnClickListener(this);
            btnSaveChanges.SetOnClickListener(this);
            etCommunityName.Text = community.Name;
            etCommunityDescription.Text = community.Description;
        }

        public void SetSorting(string sortBy)
        {
            tvSortBy.Text = Resources.GetString(Resource.String.sortBy) + " " + sortBy;
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

        private void ManageMemebers()
        {
            dialogManageMemeber = new Dialog(this);
            dialogManageMemeber.SetContentView(Resource.Layout.dialog_manageMembers);
            dialogManageMemeber.Show();
            dialogManageMemeber.SetCancelable(true);
            ibtnCloseDialog = dialogManageMemeber.FindViewById<ImageButton>(Resource.Id.ibtnCloseDialog);
            ibtnCloseDialog.SetOnClickListener(this);
            tvSortBy = dialogManageMemeber.FindViewById<TextView>(Resource.Id.tvSortBy);
            tvMemberCount = dialogManageMemeber.FindViewById<TextView>(Resource.Id.tvMemberCount);
            community.CreateMembers(dialogManageMemeber.Context);
            members = community.Members;
            ListView lvMembers = dialogManageMemeber.FindViewById<ListView>(Resource.Id.lvMembers);
            lvMembers.Adapter = members.MemberAdapter;
            RegisterForContextMenu(lvMembers);
            members.AddSnapshotListener(this);
        }

        public void OnClick(View v)
        {
            if (v == ibtnLogo)
                ReturnToHub();
            else if (v == ibtnBack)
                Back();
            else if (v == btnSaveChanges)
                Save();
            else if (v == btnManageMembers)
                ManageMemebers();
            else if (v == ibtnCloseDialog)
                dialogManageMemeber.Cancel();
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
                    SetSorting(Resources.GetString(Resource.String.sortDate));
                }
            }
        }

        public void OnEvent(Java.Lang.Object obj, FirebaseFirestoreException error)
        {
            GetMembers();
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
    }
}