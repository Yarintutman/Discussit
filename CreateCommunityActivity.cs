using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace Discussit
{
    [Activity(Label = "CreateCommunityActivity")]
    public class CreateCommunityActivity : AppCompatActivity, View.IOnClickListener
    {
        ImageButton ibtnBack, ibtnLogo;
        EditText etCommunityName, etCommunityDescription;
        Button btnCreateCommunity;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_createCommunity);
            InitViews();
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
            Finish();
        }

        public void ReturnToHub()
        {
            Intent intent = new Intent(this, typeof(CommunityHubActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
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

        public void OnClick(View v)
        {
            if (v == ibtnLogo)
            {
                Back();
            }
            else if (v == ibtnBack)
            {
                Back();
            }
            else if (v == btnCreateCommunity)
            {

            }
        }
    }
}