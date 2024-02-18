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
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Discussit
{
    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : AppCompatActivity, View.IOnClickListener
    {
        User user;
        ImageButton ibtnProfilePicture, ibtnBack;
        EditText etChangeUserName, etEnterEmail;
        Button btnChangeUserName, btnResetPassword, btnConfirmResetPassword, btnSaveChanges;
        TextView tvEmailResult;
        LinearLayout llResetPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_settings);
            InitObjects();
            InitViews();
        }

        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
        }

        private void InitViews()
        {
            ibtnProfilePicture = FindViewById<ImageButton>(Resource.Id.ibtnProfilePicture);
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            etChangeUserName = FindViewById<EditText>(Resource.Id.etChangeUserName);
            etEnterEmail = FindViewById<EditText>(Resource.Id.etEnterEmail);
            btnChangeUserName = FindViewById<Button>(Resource.Id.btnChangeUserName);
            btnResetPassword = FindViewById<Button>(Resource.Id.btnResetPassword);
            btnConfirmResetPassword = FindViewById<Button>(Resource.Id.btnConfirmResetPassword);
            btnSaveChanges = FindViewById<Button>(Resource.Id.btnSaveChanges);
            tvEmailResult = FindViewById<TextView>(Resource.Id.tvEmailResult);
            llResetPassword = FindViewById<LinearLayout>(Resource.Id.llResetPassword);
            ibtnProfilePicture.SetOnClickListener(this);
            ibtnBack.SetOnClickListener(this);
            btnChangeUserName.SetOnClickListener(this);
            btnResetPassword.SetOnClickListener(this);
            btnConfirmResetPassword.SetOnClickListener(this);
            btnSaveChanges.SetOnClickListener(this);
            llResetPassword.Visibility = ViewStates.Invisible;
        }

        private void ShowPasswordReset()
        {
            llResetPassword.Visibility = ViewStates.Visible;
            btnResetPassword.Visibility = ViewStates.Invisible;
        }

        private void Back()
        {
            Finish();
        }

#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed()
        {
            Back();
        }
#pragma warning restore CS0672 // Member overrides obsolete member

        private bool ValidInputFields(string field)
        {
            return field != string.Empty;
        }

        public void OnClick(View v)
        {
            if (v == ibtnBack)
                Back();
            else if (v == btnResetPassword)
                ShowPasswordReset();
        }
    }
}