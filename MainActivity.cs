using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Firebase.Firestore;

namespace Discussit
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity , View.IOnClickListener, IOnCompleteListener
    {
        User user;
        EditText etUsername, etEmail, etPassword;
        Button btnEnter;
        TextView tvLoginState, tvNewUser;
        CheckBox chkRemember;
        Task tskRegister, tskLogin, tskRememberLogin, tskSetFbUser, tskGetUser;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            InitObjects();
            InitViews();
        }

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
        }

        private void InitObjects()
        {
            user = new User();
            if (user.IsRegistered)
                tskRememberLogin = user.Login().AddOnCompleteListener(this);
        }

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
            else if(v == tvNewUser)
            {
                SetRegisterMode();
            }
            HideSoftKeyboard();
        }

        public void GetUser()
        {
            tskGetUser = user.GetUserData().AddOnCompleteListener(this);
        }

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

        private void OpenCommunityHub()
        {
            Intent intent = new Intent(this, typeof(CommunityHubActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivity(intent);
            Finish();
        }

        private void HideSoftKeyboard()
        {
            View currentFocus = this.CurrentFocus;
            if (currentFocus != null)
            {
                InputMethodManager inputMethodManager = (InputMethodManager)GetSystemService(InputMethodService);
                inputMethodManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
            }
        }

#pragma warning disable CS0672 // Member overrides obsolete member
        public override void OnBackPressed()
        {
            if (tvLoginState.Text == Resources.GetString(Resource.String.Register))
            {
                SetLoginMode();
            }
        }
#pragma warning restore CS0672 // Member overrides obsolete member

        public void SetRegisterMode()
        {
            tvNewUser.Visibility = Android.Views.ViewStates.Invisible;
            RelativeLayout rlUsername = FindViewById<RelativeLayout>(Resource.Id.rlUsername);
            rlUsername.Visibility = Android.Views.ViewStates.Visible;
            tvLoginState.Text = Resources.GetString(Resource.String.Register);
            btnEnter.Text = Resources.GetString(Resource.String.Register);
        }

        public void SetLoginMode()
        {
            tvNewUser.Visibility = Android.Views.ViewStates.Visible;
            RelativeLayout rlUsername = FindViewById<RelativeLayout>(Resource.Id.rlUsername);
            rlUsername.Visibility = Android.Views.ViewStates.Invisible;
            tvLoginState.Text = Resources.GetString(Resource.String.Login);
            btnEnter.Text = Resources.GetString(Resource.String.Login);
        }
    }
}