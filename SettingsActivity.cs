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
    /// <summary>
    /// Settings activity allowing users to change their profile settings.
    /// </summary>
    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : AppCompatActivity, View.IOnClickListener
    {
        User user;
        ImageButton ibtnProfilePicture, ibtnBack;
        EditText etChangeUserName, etEnterEmail;
        Button btnChangeUserName, btnResetPassword, btnConfirmResetPassword, btnSaveChanges;
        TextView tvEmailResult;
        LinearLayout llResetPassword;

        /// <summary>
        /// Initializes the activity when created.
        /// </summary>
        /// <param name="savedInstanceState">Not in use</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_settings);
            InitObjects();
            InitViews();
        }

        /// <summary>
        /// Initializes objects based on the incoming intent data.
        /// </summary>
        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
        }

        /// <summary>
        /// Initializes views and sets up click listeners.
        /// </summary>
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


        /// <summary>
        /// Shows the reset password layout when the "Reset Password" button is clicked.
        /// </summary>
        private void ShowPasswordReset()
        {
            llResetPassword.Visibility = ViewStates.Visible;
            btnResetPassword.Visibility = ViewStates.Invisible;
        }

        /// <summary>
        /// Returns to the previous activity.
        /// </summary>
        private void Back()
        {
            Intent intent = new Intent();
            intent.PutExtra(General.KEY_USER, user.GetJson());
            SetResult(Result.Ok, intent);
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
        private bool ValidInputFields(string field)
        {
            return field != string.Empty;
        }

        /// <summary>
        /// Handles click events for views in the activity.
        /// </summary>
        /// <param name="v">The view that was clicked.</param>
        public void OnClick(View v)
        {
            if (v == ibtnBack)
                Back();
            else if (v == btnResetPassword)
                ShowPasswordReset();
        }
    }
}