using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;

namespace Discussit
{
    [Activity(Label = "CreateCommunityActivity")]
    public class CreateCommunityActivity : AppCompatActivity, View.IOnClickListener
    {
        User user;
        ImageButton ibtnBack, ibtnLogo;
        EditText etCommunityName, etCommunityDescription;
        Button btnCreateCommunity;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_createCommunity);
            InitObjects();
            InitViews();
        }

        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
        }

        private void InitViews()
        {
            ibtnLogo = FindViewById<ImageButton>(Resource.Id.ibtnLogo);
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            etCommunityDescription = FindViewById<EditText>(Resource.Id.etCommunityDescription);
            etCommunityName = FindViewById<EditText>(Resource.Id.etCommunityName);
            btnCreateCommunity = FindViewById<Button>(Resource.Id.btnCreateCommunity);
            ibtnLogo.SetOnClickListener(this);
            ibtnBack.SetOnClickListener(this);
            btnCreateCommunity.SetOnClickListener(this);
        }

        public void Back()
        {
            Intent intent = new Intent();
            intent.PutExtra(General.KEY_USER, user.GetJson());
            SetResult(Result.Ok, intent);
            Finish();
        }

        public void ReturnToHub()
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

        private void CreateCommunity()
        {
            Community community = new Community(etCommunityName.Text, etCommunityDescription.Text);
            community.AddMember(user);
            ViewCommunity(community);
        }

        private void ViewCommunity(Community community)
        {
            Intent intent = new Intent(this, typeof(ViewCommunityActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivity(intent);
            Finish();
        }

        public void OnClick(View v)
        {
            if (v == ibtnLogo)
                ReturnToHub();
            else if (v == ibtnBack)
                Back();
            else if (v == btnCreateCommunity)
            {
                if (ValidInputFields())
                    CreateCommunity();
                else
                    Toast.MakeText(this, Resources.GetString(Resource.String.InvalidFields), ToastLength.Long).Show();
            }
        }
    }
}