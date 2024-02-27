using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Firestore;
using Firebase.Firestore.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discussit
{
    [Activity(Label = "ManageCommunityActivity")]
    public class ManageCommunityActivity : AppCompatActivity, View.IOnClickListener, IOnCompleteListener
    {
        Community community;
        ImageButton ibtnLogo, ibtnBack, ibtnCloseDialog;
        EditText etCommunityName, etCommunityDescription;
        Button btnManageMembers, btnSaveChanges;
        TextView tvSortBy;
        Dialog dialogManageMemeber;
        MemberAdapter members;
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
            Finish();
        }

        private void ReturnToHub()
        {
            Intent intent = new Intent(this, typeof(CommunityHubActivity));
            intent.AddFlags(ActivityFlags.LaunchedFromHistory);
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
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
            SetSorting(Resources.GetString(Resource.String.sortDate));
            community.CreateMembers(this);
            tskGetMembers = community.GetMembers().AddOnCompleteListener(this);
        }

        private void SetMembers(IList<DocumentSnapshot> documents)
        {
            members = new MemberAdapter(dialogManageMemeber.Context);
            members.SetMembers(documents);
            ListView lvMembers = dialogManageMemeber.FindViewById<ListView>(Resource.Id.lvMembers);
            lvMembers.Adapter = members;
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
        }

        public void OnComplete(Task task)
        {
            if (task.IsComplete)
            {
                if (task == tskGetMembers)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    SetMembers(qs.Documents);
                }
            }
        }
    }
}