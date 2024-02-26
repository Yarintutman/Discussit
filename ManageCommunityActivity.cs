using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discussit
{
    [Activity(Label = "ManageCommunityActivity")]
    public class ManageCommunityActivity : AppCompatActivity, View.IOnClickListener
    {
        Community community;
        ImageButton ibtnLogo, ibtnBack;
        EditText etCommunityName, etCommunityDescription;
        Button btnManageMembers, btnSaveChanges;

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

        public void OnClick(View v)
        {
        }
    }
}