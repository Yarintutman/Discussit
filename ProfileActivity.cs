using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Firestore;
using System;
using System.Collections.Generic;

namespace Discussit
{
    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : AppCompatActivity, View.IOnClickListener, IOnCompleteListener, AdapterView.IOnItemClickListener
    {
        User user;
        ImageButton ibtnPicture, ibtnBack, ibtnLogout, ibtnCloseDialog;
        Button btnViewCommunities, btnViewPosts, btnViewComments, btnManageCommunities, btnSettings;
        Dialog dialogViewCommunities, dialogViewPosts, dialogViewComments, dialogViewManagedCommunities;
        Task tskGetCommunities, tskGetPosts, tskGetComments, tskGetManagedCommunities;
        CommunityAdapter communities, managedCommunities;
        PostAdapter posts;
        CommunityAdapter comments;
        string currentDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_profile);
            InitObjects();
            InitViews();
        }

        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
        }

        private void InitViews()
        {
            ibtnPicture = FindViewById<ImageButton>(Resource.Id.ibtnPicture);
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            ibtnLogout = FindViewById<ImageButton>(Resource.Id.ibtnLogout);
            TextView tvUsername = FindViewById<TextView>(Resource.Id.tvUsername);
            btnViewCommunities = FindViewById<Button>(Resource.Id.btnViewCommunities);
            btnViewPosts = FindViewById<Button>(Resource.Id.btnViewPosts);
            btnViewComments = FindViewById<Button>(Resource.Id.btnViewComments);
            btnManageCommunities = FindViewById<Button>(Resource.Id.btnManageCommunities);
            btnSettings = FindViewById<Button>(Resource.Id.btnSettings);
            tvUsername.Text = user.Username;
            ibtnPicture.SetOnClickListener(this);
            ibtnBack.SetOnClickListener(this);
            ibtnLogout.SetOnClickListener(this);
            btnViewCommunities.SetOnClickListener(this);
            btnViewPosts.SetOnClickListener(this);
            btnViewComments.SetOnClickListener(this);
            btnManageCommunities.SetOnClickListener(this);
            btnSettings.SetOnClickListener(this);
        }

        public void Back()
        {
            Finish();
        }

#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed()
        {
            Back();
        }
#pragma warning restore CS0672 // Member overrides obsolete member


        private void Logout()
        {
            SpData spd = new SpData(General.SP_FILE_NAME);
            spd.PutBool(General.KEY_REGISTERED, false);
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            Finish();
        }

        private void ViewSettings()
        {
            Intent intent = new Intent(this, typeof(SettingsActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivity(intent);
            Finish();
        }

        private void ViewManagedCommunities()
        {
            dialogViewManagedCommunities = new Dialog(this);
            dialogViewManagedCommunities.SetContentView(Resource.Layout.dialog_selectCommunity);
            dialogViewManagedCommunities.Show();
            dialogViewManagedCommunities.SetCancelable(true);
            ibtnCloseDialog = dialogViewManagedCommunities.FindViewById<ImageButton>(Resource.Id.ibtnCloseDialog);
            ibtnCloseDialog.SetOnClickListener(this);
            currentDialog = Resources.GetString(Resource.String.ManagedCommunities);
            FbData fbd = new FbData();
            if (user.ManagingCommunities.Size() != 0)
                tskGetManagedCommunities = fbd.GetDocumentsInList(General.COMMUNITIES_COLLECTION,
                                           General.JavaListToIListWithCut(user.ManagingCommunities, '/')).AddOnCompleteListener(this);
        }

        private void SetManagingCommunities(IList<DocumentSnapshot> documents)
        {
            managedCommunities = new CommunityAdapter(dialogViewManagedCommunities.Context);
            Community community;
            FbData fbd = new FbData();
            foreach (DocumentSnapshot document in documents)
            {
                community = new Community
                {
                    Id = document.Id,
                    Name = document.GetString(General.FIELD_COMMUNITY_NAME),
                    Description = document.GetString(General.FIELD_COMMUNITY_DESCRIPTION),
                    CreationDate = fbd.FirestoreTimestampToDateTime(document.GetTimestamp(General.FIELD_DATE)),
                    MemberCount = document.GetLong(General.FIELD_MEMBER_COUNT).LongValue(),
                    PostCount = document.GetLong(General.FIELD_POST_COUNT).LongValue()
                };
                managedCommunities.AddCommunity(community);
            }
            ListView lvManagedCommunities = dialogViewManagedCommunities.FindViewById<ListView>(Resource.Id.lvCommunities);
            lvManagedCommunities.Adapter = managedCommunities;
        }

        public void OnClick(View v)
        {
            if (v == ibtnBack)
                Back();
            else if (v == ibtnLogout)
                Logout();
            else if (v == btnSettings)
                ViewSettings();
            else if (v == btnManageCommunities)
                ViewManagedCommunities();
            else if (v == ibtnCloseDialog)
            {
                if (currentDialog == Resources.GetString(Resource.String.ManagedCommunities))
                    dialogViewManagedCommunities.Cancel();
                else if (currentDialog == Resources.GetString(Resource.String.Communities))
                    dialogViewCommunities.Cancel();
                else if (currentDialog == Resources.GetString(Resource.String.Posts))
                    dialogViewPosts.Cancel();
                else if (currentDialog == Resources.GetString(Resource.String.Comments))
                    dialogViewComments.Cancel();
            }
        }

        public void OnComplete(Task task)
        {
            if (task == tskGetManagedCommunities)
            {
                QuerySnapshot qs = (QuerySnapshot)task.Result;
                SetManagingCommunities(qs.Documents);
            }
        }

        private void ViewPost(Post post)
        {
            Intent intent = new Intent(this, typeof(ViewPostActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_POST, post.GetJson());
            StartActivity(intent);
        }

        private void ViewCommunity(Community community)
        {
            Intent intent = new Intent(this, typeof(ViewCommunityActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivity(intent);
        }

        private void ManageCommunity(Community community)
        {
            Intent intent = new Intent(this, typeof(ManageCommunityActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivity(intent);
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            if (currentDialog == Resources.GetString(Resource.String.ManagedCommunities))
                ManageCommunity(managedCommunities[position]);
            else if (currentDialog == Resources.GetString(Resource.String.Communities))
                ViewCommunity(communities[position]);
            else if (currentDialog == Resources.GetString(Resource.String.Posts))
                ViewPost(posts[position]);
            else if (currentDialog == Resources.GetString(Resource.String.Comments)) { }
                //TBD
        }
    }
}