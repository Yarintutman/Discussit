﻿using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
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
        TextView tvSortBy;
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

        public void SetSorting(string sortBy)
        {
            tvSortBy.Text = Resources.GetString(Resource.String.sortBy) + " " + sortBy;
        }

        public void Back()
        {
            Intent intent = new Intent();
            intent.PutExtra(General.KEY_USER, user.GetJson());
            SetResult(Result.Ok, intent);
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
            StartActivityForResult(intent, 0);
            Finish();
        }

        private void ViewSettings()
        {
            Intent intent = new Intent(this, typeof(SettingsActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivityForResult(intent, 0);
        }


        private void ViewCommunities()
        {
            dialogViewCommunities = new Dialog(this);
            dialogViewCommunities.SetContentView(Resource.Layout.dialog_selectCommunity);
            dialogViewCommunities.Show();
            dialogViewCommunities.SetCancelable(true);
            ibtnCloseDialog = dialogViewCommunities.FindViewById<ImageButton>(Resource.Id.ibtnCloseDialog);
            ibtnCloseDialog.SetOnClickListener(this);
            currentDialog = Resources.GetString(Resource.String.Communities);
            tvSortBy = dialogViewCommunities.FindViewById<TextView>(Resource.Id.tvSortBy);
            SetSorting(Resources.GetString(Resource.String.sortDate));
            FbData fbd = new FbData();
            if (user.Communities.Size() != 0)
                tskGetCommunities = fbd.GetDocumentsInList(General.COMMUNITIES_COLLECTION,
                                           General.JavaListToIListWithCut(user.Communities, "/")).AddOnCompleteListener(this);
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
            tvSortBy = dialogViewManagedCommunities.FindViewById<TextView>(Resource.Id.tvSortBy);
            SetSorting(Resources.GetString(Resource.String.sortDate));
            FbData fbd = new FbData();
            if (user.ManagingCommunities.Size() != 0)
                tskGetManagedCommunities = fbd.GetDocumentsInList(General.COMMUNITIES_COLLECTION,
                                           General.JavaListToIListWithCut(user.ManagingCommunities, "/")).AddOnCompleteListener(this);
        }

        private void ViewPosts()
        {
            dialogViewPosts = new Dialog(this);
            dialogViewPosts.SetContentView(Resource.Layout.dialog_selectPost);
            dialogViewPosts.Show();
            dialogViewPosts.SetCancelable(true);
            ibtnCloseDialog = dialogViewPosts.FindViewById<ImageButton>(Resource.Id.ibtnCloseDialog);
            ibtnCloseDialog.SetOnClickListener(this);
            currentDialog = Resources.GetString(Resource.String.Posts);
            tvSortBy = dialogViewPosts.FindViewById<TextView>(Resource.Id.tvSortBy);
            SetSorting(Resources.GetString(Resource.String.sortDate));
            FbData fbd = new FbData();
            if (user.Posts.Size() != 0)
                tskGetPosts = fbd.GetDocumentsInList(General.POSTS_COLLECTION,
                                           General.JavaListToIListWithCut(user.Posts, '/' + General.POSTS_COLLECTION)).AddOnCompleteListener(this);
        }

        private void SetManagedCommunities(IList<DocumentSnapshot> documents)
        {
            managedCommunities = new CommunityAdapter(dialogViewManagedCommunities.Context);
            managedCommunities.SetCommunities(documents);
            ListView lvCommunities = dialogViewManagedCommunities.FindViewById<ListView>(Resource.Id.lvCommunities);
            lvCommunities.Adapter = managedCommunities;
            lvCommunities.OnItemClickListener = this;
        }

        private void SetCommunities(IList<DocumentSnapshot> documents)
        {
            communities = new CommunityAdapter(dialogViewCommunities.Context);
            communities.SetCommunities(documents);
            ListView lvCommunities = dialogViewCommunities.FindViewById<ListView>(Resource.Id.lvCommunities);
            lvCommunities.Adapter = communities;
            lvCommunities.OnItemClickListener = this;
        }

        private void SetPosts(IList<DocumentSnapshot> documents)
        {
            posts = new PostAdapter(dialogViewPosts.Context);
            posts.SetPosts(documents);
            ListView lvPosts = dialogViewPosts.FindViewById<ListView>(Resource.Id.lvPosts);
            lvPosts.Adapter = posts;
            lvPosts.OnItemClickListener = this;
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
            else if (v == btnViewCommunities)
                ViewCommunities();
            else if (v == btnViewPosts) { }
            //ViewPosts();
            else if (v == btnViewComments) { }
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
            if (task.IsComplete)
            {
                if (task == tskGetManagedCommunities)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    SetManagedCommunities(qs.Documents);
                }
                else if (task == tskGetCommunities)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    SetCommunities(qs.Documents);
                }
            }
        }

        private void ViewPost(Post post)
        {
            Intent intent = new Intent(this, typeof(ViewPostActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_POST, post.GetJson());
            StartActivityForResult(intent, 0);
            dialogViewPosts.Cancel();
        }

        private void ViewCommunity(Community community)
        {
            Intent intent = new Intent(this, typeof(ViewCommunityActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivityForResult(intent, 0);
            dialogViewCommunities.Cancel();
        }

        private void ManageCommunity(Community community)
        {
            Intent intent = new Intent(this, typeof(ManageCommunityActivity));
            intent.PutExtra(General.KEY_USER, Intent.GetStringExtra(General.KEY_USER));
            intent.PutExtra(General.KEY_COMMUNITY, community.GetJson());
            StartActivityForResult(intent, 0);
            dialogViewManagedCommunities.Cancel();
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


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
                user = User.GetUserJson(data.GetStringExtra(General.KEY_USER));
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}