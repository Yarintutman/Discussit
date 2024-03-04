using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;

namespace Discussit
{
    /// <summary>
    /// Activity for creating a new community.
    /// </summary>
    [Activity(Label = "CreateCommunityActivity")]
    public class CreateCommunityActivity : AppCompatActivity, View.IOnClickListener
    {
        User user;
        ImageButton ibtnBack, ibtnLogo;
        EditText etCommunityName, etCommunityDescription;
        Button btnCreateCommunity;

        /// <summary>
        /// Called when the activity is starting.
        /// </summary>
        /// <param name="savedInstanceState">Not in use</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_createCommunity);
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
            ibtnLogo = FindViewById<ImageButton>(Resource.Id.ibtnLogo);
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            etCommunityDescription = FindViewById<EditText>(Resource.Id.etCommunityDescription);
            etCommunityName = FindViewById<EditText>(Resource.Id.etCommunityName);
            btnCreateCommunity = FindViewById<Button>(Resource.Id.btnCreateCommunity);
            ibtnLogo.SetOnClickListener(this);
            ibtnBack.SetOnClickListener(this);
            btnCreateCommunity.SetOnClickListener(this);
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
        /// Disables the default back button behavior and invokes the custom back method.
        /// </summary>
#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed() 
        {
            Back();
        }
#pragma warning restore CS0672 // Member overrides obsolete member

        /// <summary>
        /// Checks if the input fields contain valid data.
        /// </summary>
        /// <returns>true if the input fields are valid; otherwise, false.</returns>
        private bool ValidInputFields()
        {
            bool status = true;
            status = status && (etCommunityName.Text != string.Empty);
            status = status && (etCommunityDescription.Text != string.Empty);
            return status;
        }

        /// <summary>
        /// Creates a new community with the provided information.
        /// </summary>
        private void CreateCommunity()
        {
            Community community = new Community(etCommunityName.Text, etCommunityDescription.Text);
            community.AddMember(user);
            ViewCommunity(community);
        }

        /// <summary>
        /// Starts the activity to view the created community.
        /// </summary>
        /// <param name="community">The community to view.</param>
        private void ViewCommunity(Community community)
        {
            Intent intent = new Intent(this, typeof(ViewCommunityActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivity(intent);
            Finish();
        }

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