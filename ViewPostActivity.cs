using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Speech.Tts;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Firestore;

namespace Discussit
{
    [Activity(Label = "ViewPostActivity")]
    public class ViewPostActivity : AppCompatActivity, View.IOnClickListener, AdapterView.IOnItemClickListener, IEventListener, IOnCompleteListener, TextToSpeech.IOnInitListener
    {
        User user;
        Community community;
        Post post;
        Comment currentComment, recursiveComment;
        Comments comments;
        Members members;
        ImageButton ibtnBack, ibtnLogo, ibtnProfile, ibtnTts;
        Button btnNewComment;
        Task tskGetComments, tskGetMemebers, tskGetRecursiveComments;
        TextToSpeech tts;
        bool isInCommunity;

        /// <summary>
        /// Called when the activity is starting.
        /// </summary>
        /// <param name="savedInstanceState">Not in use</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_viewPost);
            InitObjects();
            InitViews();
        }

        /// <summary>
        /// Initializes the objects used in the activity.
        /// </summary>
        private void InitObjects()
        {
            user = User.GetUserJson(Intent.GetStringExtra(General.KEY_USER));
            post = Post.GetPostJson(Intent.GetStringExtra(General.KEY_POST));
            community = Community.GetCommunityJson(Intent.GetStringExtra(General.KEY_COMMUNITY));
            post.CreateComments(this);
            community.CreateMembers(this);
            comments = post.Comments;
            members = community.Members;
            tts = new TextToSpeech(this, this);
            tts.SetPitch(1);
        }

        /// <summary>
        /// Initializes the views used in the activity.
        /// </summary>
        private void InitViews()
        {
            TextView tvPostCreator = FindViewById<TextView>(Resource.Id.tvPostCreator);
            TextView tvPostTitle = FindViewById<TextView>(Resource.Id.tvPostTitle);
            TextView tvPostDescription = FindViewById<TextView>(Resource.Id.tvPostDescription);
            ListView lvComments = FindViewById<ListView>(Resource.Id.lvComments);
            ibtnBack = FindViewById<ImageButton>(Resource.Id.ibtnBack);
            ibtnLogo = FindViewById<ImageButton>(Resource.Id.ibtnLogo);
            ibtnProfile = FindViewById<ImageButton>(Resource.Id.ibtnProfile);
            ibtnTts = FindViewById<ImageButton>(Resource.Id.ibtnTts);
            btnNewComment = FindViewById<Button>(Resource.Id.btnNewComment);
            tvPostCreator.Text = post.CreatorName;
            tvPostTitle.Text = post.Title;
            tvPostDescription.Text = post.Description;
            lvComments.Adapter = comments.CommentAdapter;
            lvComments.OnItemClickListener = this;
            ibtnBack.SetOnClickListener(this);
            ibtnLogo.SetOnClickListener(this);
            ibtnProfile.SetOnClickListener(this);
            ibtnTts.SetOnClickListener(this);
            btnNewComment.SetOnClickListener(this);
            RegisterForContextMenu(lvComments);
        }

        private void CheckMembership()
        {
            isInCommunity = members.HasMember(user.Id);
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
            StartActivityForResult(intent, 0);
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
        /// Opens the activity to create a new comment replying to a the post.
        /// </summary>
        private void OpenCreateCommentActivity()
        {
            Intent intent = new Intent(this, typeof(CreateCommentActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_POST, post.GetJson());
            StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// Opens the activity to create a new comment, replying to the specified comment.
        /// </summary>
        /// <param name="comment">The comment being replied to.</param>
        private void OpenCreateCommentActivity(Comment comment)
        {
            Intent intent = new Intent(this, typeof(CreateCommentActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            intent.PutExtra(General.KEY_POST, post.GetJson());
            intent.PutExtra(General.KEY_COMMENT, comment.GetJson());
            StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// Retrieves comments associated with the post.
        /// </summary>
        private void GetComments()
        {
            tskGetComments = post.GetComments().AddOnCompleteListener(this);
        }

        /// <summary>
        /// Retrieves members associated with the community.
        /// </summary>
        private void GetMembers()
        {
            tskGetMemebers = community.GetMembers().AddOnCompleteListener(this);
        }

        /// <summary>
        /// Retrieves recursive comments associated with the specified comment.
        /// </summary>
        /// <param name="comment">The comment for which recursive comments are retrieved.</param>
        private void GetRecursiveComments(Comments comment)
        {
            tskGetComments = comment.GetComments().AddOnCompleteListener(this);
        }

        /// <summary>
        /// Opens the profile activity to view the user's profile.
        /// </summary>
        private void ViewProfile()
        {
            Intent intent = new Intent(this, typeof(ProfileActivity));
            intent.PutExtra(General.KEY_USER, user.GetJson());
            StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// Called when creating a context menu for list view items.
        /// </summary>
        /// <param name="menu">The context menu that is being built.</param>
        /// <param name="v">The view for which the context menu is being created.</param>
        /// <param name="menuInfo">Extra information about the item for which the context menu should be shown.</param>
        public override void OnCreateContextMenu(Android.Views.IContextMenu menu, Android.Views.View v, Android.Views.IContextMenuContextMenuInfo menuInfo)
        {
            AdapterView.AdapterContextMenuInfo info = menuInfo as AdapterView.AdapterContextMenuInfo;
            if (info != null)
            {
                int position = info.Position;
                currentComment = comments[position];
                MenuInflater.Inflate(Resource.Menu.menu_createComment, menu);
                base.OnCreateContextMenu(menu, v, menuInfo);
            }
        }

        /// <summary>
        /// Called when a context menu item is selected.
        /// </summary>
        /// <param name="item">The selected menu item.</param>
        /// <returns>True if the item selection was handled, otherwise false.</returns>
        public override bool OnContextItemSelected(Android.Views.IMenuItem item)
        {
            if (item.ItemId == Resource.Id.itemCreateComment)
            {
                if (isInCommunity)
                    OpenCreateCommentActivity(currentComment);
                else
                    Toast.MakeText(this, Resources.GetString(Resource.String.joinCommunityToToCreateComment), ToastLength.Short);

            }
            return base.OnContextItemSelected(item);
        }

        /// <summary>
        /// Playes tts of the post's description
        /// </summary>
        private void PostTextToSpeach()
        {
            tts.Speak(post.Description, QueueMode.Flush, null, null);
        }

        /// <summary>
        /// Handles click events.
        /// </summary>
        /// <param name="v">The view that was clicked.</param>
        public void OnClick(View v)
        {
            if (v == ibtnLogo)
                ReturnToHub();
            else if (v == ibtnBack)
                Back();
            else if (v == ibtnProfile)
                ViewProfile();
            else if (v == ibtnTts)
                PostTextToSpeach();
            else if (v == btnNewComment)
                if (isInCommunity)
                    OpenCreateCommentActivity();
                else
                    Toast.MakeText(this, Resources.GetString(Resource.String.joinCommunityToToCreateComment), ToastLength.Short).Show();
        }

        /// <summary>
        /// Callback method indicating that a Task has completed.
        /// </summary>
        /// <param name="task">The completed Task.</param>
        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                if (task == tskGetComments)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    comments.AddComments(qs.Documents);
                    comments.ShowOpenComments();
                }
                else if (task == tskGetMemebers)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    members.AddMembers(qs.Documents);
                    CheckMembership();
                }
                else if (task == tskGetRecursiveComments)
                {
                    QuerySnapshot qs = (QuerySnapshot)task.Result;
                    comments.AddSubComments(qs.Documents, recursiveComment);
                }
            }
        }

        /// <summary>
        /// Event handler called when a change occurs in the Firebase Firestore database.
        /// </summary>
        /// <param name="obj">The object representing the event.</param>
        /// <param name="error"Not in use.</param>
        public void OnEvent(Java.Lang.Object obj, FirebaseFirestoreException error)
        {
            
            GetMembers();
            GetComments();
        }

        /// <summary>
        /// Handles the event of clicking an item in the list of comments.
        /// </summary>
        /// <param name="parent">The AdapterView where the click happened.</param>
        /// <param name="view">The view within the AdapterView that was clicked.</param>
        /// <param name="position">The position of the view in the adapter.</param>
        /// <param name="id">The row id of the item that was clicked.</param>
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            TextView tvViewComments = view.FindViewById<TextView>(Resource.Id.tvViewComments);
            recursiveComment = comments[position];
            recursiveComment.CreateComments(this);
            if (tvViewComments.Visibility != ViewStates.Gone) 
            { 
                if (tvViewComments.Text == Resources.GetString(Resource.String.ShowComments))
                {
                    recursiveComment.HideComments = true;
                    tskGetRecursiveComments = recursiveComment.GetComments().AddOnCompleteListener(this);
                }
                else
                {
                    recursiveComment.HideComments = false;
                    comments.RemoveSubcomments(recursiveComment);
                }
            }
        }

        /// <summary>
        /// Called when an activity launched by this activity exits, returning a result.
        /// </summary>
        /// <param name="requestCode">Not in use</param>
        /// <param name="resultCode">The integer result code returned by the child activity through its SetResult() method.</param>
        /// <param name="data">An Intent, which can return result data to the caller (various data can be attached to Intent "extras").</param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
                user = User.GetUserJson(data.GetStringExtra(General.KEY_USER));
            base.OnActivityResult(requestCode, resultCode, data);
        }

        /// <summary>
        /// Called when the activity is resumed from a paused state.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            comments.AddSnapshotListener(this);
            members.AddSnapshotListener(this);
            comments.CommentAdapter.NotifyDataSetChanged();
        }

        /// <summary>
        /// Called when the activity is going into the background as the user navigates away from it.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            comments.RemoveSnapshotListener();
            members.RemoveSnapshotListener();
            tts.Stop();
        }

        /// <summary>
        /// Callback method indicating the initialization status of TextToSpeech.
        /// </summary>
        /// <param name="status">Not in use</param>
        public void OnInit([GeneratedEnum] OperationResult status) { }
    }
}