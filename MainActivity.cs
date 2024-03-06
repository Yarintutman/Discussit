using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Firebase.Firestore;
using Kotlin;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Discussit
{
    /// <summary>
    /// Represents the main activity of the application.
    /// </summary>
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity, View.IOnClickListener, IOnCompleteListener
    {
        User user;
        EditText etUsername, etEmail, etPassword;
        Button btnEnter;
        TextView tvLoginState, tvNewUser;
        CheckBox chkRemember;
        Task tskRegister, tskLogin, tskRememberLogin, tskSetFbUser, tskGetUser;

        /// <summary>
        /// Method called when the activity is first created.
        /// </summary>
        /// <param name="savedInstanceState">Not in use</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            InitObjects();
            InitViews();
        }

        /// <summary>
        /// Initializes views in the activity.
        /// </summary>
        private void InitViews()
        {
            etUsername = FindViewById<EditText>(Resource.Id.etUsername);
            etEmail = FindViewById<EditText>(Resource.Id.etEmail);
            etPassword = FindViewById<EditText>(Resource.Id.etPassword);
            btnEnter = FindViewById<Button>(Resource.Id.btnEnter);
            tvLoginState = FindViewById<TextView>(Resource.Id.tvLoginState);
            tvNewUser = FindViewById<TextView>(Resource.Id.tvNewUser);
            chkRemember = FindViewById<CheckBox>(Resource.Id.chkRemember);
            tvNewUser.SetOnClickListener(this);
            btnEnter.SetOnClickListener(this);
            if (user.IsRegistered)
            {
                etEmail.Text = user.Email;
                etPassword.Text = user.Password;
            }
        }

        /// <summary>
        /// Initializes objects used in the activity.
        /// </summary>
        private void InitObjects()
        {
            user = new User();
            if (user.IsRegistered)
                tskRememberLogin = user.Login().AddOnCompleteListener(this);
        }

        /// <summary>
        /// Validates the input fields in the activity.
        /// </summary>
        /// <returns>True if all input fields are valid, otherwise false.</returns>
        private bool ValidInputFields()
        {
            bool status = true;
            if (tvLoginState.Text == Resources.GetString(Resource.String.Register))
            {
                user.Username = etUsername.Text.Trim();
                status = user.Username.Length > 0;
            }
            user.Email = etEmail.Text.Trim();
            user.Password = etPassword.Text;
            return status && user.Email.Length > 0 && user.Password.Length > 0;
        }

        /// <summary>
        /// Handles click events for views in the activity.
        /// </summary>
        /// <param name="v">The view that was clicked.</param>
        public void OnClick(View v)
        {
            if (v == btnEnter)
            {
                if (ValidInputFields())
                {
                    if (btnEnter.Text == Resources.GetString(Resource.String.Register))
                        tskRegister = user.Register().AddOnCompleteListener(this);
                    else
                        tskLogin = user.Login().AddOnCompleteListener(this);
                }
                else
                    Toast.MakeText(this, Resources.GetString(Resource.String.InvalidFields), ToastLength.Long).Show();
            }
            else if (v == tvNewUser)
            {
                SetRegisterMode();
            }
            HideSoftKeyboard();
        }

        /// <summary>
        /// Initiates the process of retrieving user data.
        /// </summary>
        public void GetUser()
        {
            tskGetUser = user.GetUserData().AddOnCompleteListener(this);
        }

        /// <summary>
        /// Handles the completion of tasks asynchronously.
        /// </summary>
        /// <param name="task">The completed task.</param>
        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                if (task == tskRegister)
                {
                    tskSetFbUser = user.SetFbUser().AddOnCompleteListener(this);
                }
                else
                {
                    if (task == tskLogin)
                    {
                        if (chkRemember.Checked)
                            user.Save();
                        GetUser();
                    }
                    else if (task == tskSetFbUser)
                    {
                        if (chkRemember.Checked)
                            user.Save();
                        GetUser();
                    }
                    else if (task == tskRememberLogin)
                    {
                        GetUser();
                    }
                    else if (task == tskGetUser)
                    {
                        DocumentSnapshot ds = (DocumentSnapshot)task.Result;
                        user.SetUser(ds);
                        OpenCommunityHub();
                    }
                }
            }
            else if (task != tskRememberLogin)
                Toast.MakeText(this, task.Exception.Message, ToastLength.Long).Show();
            else
                user.Forget();
        }

        /// <summary>
        /// Opens the Community Hub activity.
        /// </summary>
        private void OpenCommunityHub()
        {
            Intent intent = new Intent(this, typeof(CommunityHubActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivity(intent);
            Finish();
        }

        /// <summary>
        /// Hides the soft keyboard if it's currently visible.
        /// </summary>
        private void HideSoftKeyboard()
        {
            View currentFocus = this.CurrentFocus;
            if (currentFocus != null)
            {
                InputMethodManager inputMethodManager = (InputMethodManager)GetSystemService(InputMethodService);
                inputMethodManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
            }
        }
        /// <summary>
        /// Overrides the back button functionality to switch between login and registration modes.
        /// </summary>
#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed()
        {
            if (tvLoginState.Text == Resources.GetString(Resource.String.Register))
            {
                SetLoginMode();
            }
        }
#pragma warning restore CS0672 // Member overrides obsolete member

        /// <summary>
        /// Sets the activity to registration mode, hiding the username field and updating UI elements accordingly.
        /// </summary>
        public void SetRegisterMode()
        {
            tvNewUser.Visibility = Android.Views.ViewStates.Invisible;
            RelativeLayout rlUsername = FindViewById<RelativeLayout>(Resource.Id.rlUsername);
            rlUsername.Visibility = Android.Views.ViewStates.Visible;
            tvLoginState.Text = Resources.GetString(Resource.String.Register);
            btnEnter.Text = Resources.GetString(Resource.String.Register);
        }

        /// <summary>
        /// Sets the activity to login mode, showing the username field and updating UI elements accordingly.
        /// </summary>
        public void SetLoginMode()
        {
            tvNewUser.Visibility = ViewStates.Visible;
            RelativeLayout rlUsername = FindViewById<RelativeLayout>(Resource.Id.rlUsername);
            rlUsername.Visibility = ViewStates.Invisible;
            tvLoginState.Text = Resources.GetString(Resource.String.Login);
            btnEnter.Text = Resources.GetString(Resource.String.Login);
        }

        /// <summary>
        /// Animates the app name and slogan text views.
        /// </summary>
        private void Anim()
        {
            TextView tvAppName = FindViewById<TextView>(Resource.Id.tvAppName);
            TextView tvSlogan = FindViewById<TextView>(Resource.Id.tvSlogan);
            string text = Resources.GetString(Resource.String.app_name);
            Thread.Sleep(300);
            for (int i = 0; i <= text.Length; i++)
            {
                RunOnUiThread(() => { tvAppName.Text = text[..i]; });
                Thread.Sleep(500);
            }
            Thread.Sleep(500);
            text = Resources.GetString(Resource.String.Slogan);
            for (int i = 0; i <= text.Length; i++)
            {
                RunOnUiThread(() => { tvSlogan.Text = text[..i]; });
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Called when the activity is resumed. Starts the animation if the user is not registered.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            if (!user.IsRegistered)
            {
                ThreadStart ts = new ThreadStart(Anim);
                Thread t = new Thread(ts);
                t.Start();
            }
        }

        /// <summary>
        /// Called when the activity is paused. Clears the app name and slogan text views.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            TextView tvAppName = FindViewById<TextView>(Resource.Id.tvAppName);
            TextView tvSlogan = FindViewById<TextView>(Resource.Id.tvSlogan);
            tvAppName.Text = string.Empty;
            tvSlogan.Text = string.Empty;
        }
    }
}