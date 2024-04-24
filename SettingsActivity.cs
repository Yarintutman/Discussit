using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace Discussit
{
    /// <summary>
    /// Settings activity allowing users to change their profile settings.
    /// </summary>
    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : AppCompatActivity, View.IOnClickListener, IOnCompleteListener
    {
        User user;
        ImageButton ibtnProfilePicture, ibtnBack;
        EditText etEnterEmail;
        Button btnResetPassword, btnEmailVerification;
        TextView tvEmailResult;
        LinearLayout llResetPassword;
        Task tskResetPassword;

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
            TextView tvUsername = FindViewById<TextView>(Resource.Id.tvUsername);
            ibtnProfilePicture = FindViewById<ImageButton>(Resource.Id.ibtnProfilePicture);
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            etEnterEmail = FindViewById<EditText>(Resource.Id.etEnterEmail);
            btnResetPassword = FindViewById<Button>(Resource.Id.btnResetPassword);
            btnEmailVerification = FindViewById<Button>(Resource.Id.btnEmailVerification);
            tvEmailResult = FindViewById<TextView>(Resource.Id.tvEmailResult);
            llResetPassword = FindViewById<LinearLayout>(Resource.Id.llResetPassword);
            tvUsername.Text = user.Username;
            ibtnProfilePicture.SetOnClickListener(this);
            ibtnBack.SetOnClickListener(this);
            btnResetPassword.SetOnClickListener(this);
            btnEmailVerification.SetOnClickListener(this);
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
        /// Sends a password reset email to the email address entered by the user, if the input fields are valid.
        /// </summary>
        private void SendEmailVerification()
        {
            if (ValidInputFields(etEnterEmail.Text))
            {
                tskResetPassword = user.SendResetPasswordEmail(etEnterEmail.Text);
                if (tskResetPassword != null)
                    tskResetPassword.AddOnCompleteListener(this);
                else
                    tvEmailResult.Text = Resources.GetString(Resource.String.wrongEmail);
            }
            else
            {
                tvEmailResult.Text = Resources.GetString(Resource.String.InvalidFields);
            }
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
            else if (v == btnEmailVerification)
                SendEmailVerification();
        }

        /// <summary>
        /// Handles completion of asynchronous tasks.
        /// </summary>
        /// <param name="task">The completed task.</param>
        public void OnComplete(Task task)
        {
            if (task == tskResetPassword)
            {
                if (task.IsSuccessful)
                    tvEmailResult.Text = Resources.GetString(Resource.String.sentEmail);
                else
                    tvEmailResult.Text = Resources.GetString(Resource.String.failedToSend);
            }    
        }
    }
}