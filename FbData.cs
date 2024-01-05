using Android.App;
using Android.Gms.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Java.Util;

namespace Discussit
{
    internal class FbData
    {
        private readonly FirebaseAuth auth;
        private readonly FirebaseFirestore firestore;

        public FbData()
        {
            Firebase.FirebaseApp app = Firebase.FirebaseApp.InitializeApp(Application.Context);
            if (app is null)
            {
                FirebaseOptions options = GetMyOptions();
                app = Firebase.FirebaseApp.InitializeApp(Application.Context, options);
            }
            auth = FirebaseAuth.Instance;
            firestore = FirebaseFirestore.GetInstance(app);
        }

        private FirebaseOptions GetMyOptions()
        {
            return new FirebaseOptions.Builder()
                    .SetProjectId("discussit-37dba")
                    .SetApplicationId("1:560980864181:android:a8da7d40a4d9b4891ba2fc")
                    .SetApiKey("AIzaSyB3v_TsmY8zFLBme5hoABPW-NC7LXTdvJo")
                    .SetStorageBucket("discussit-37dba.appspot.com")
                    .Build();
        }

        public Task Register(string email, string password)
        {
            return auth.CreateUserWithEmailAndPassword(email, password);
        }

        public Task Login(string email, string password)
        {
            return auth.SignInWithEmailAndPassword(email, password);
        }

        public string GetUserId()
        {
            return auth.CurrentUser is null ? string.Empty : auth.CurrentUser.Uid;
        }

        public Task SetDocument(string cName, string docId, out string newId, HashMap hm)
        {
            Task t;
            if (docId != string.Empty)
            {
                t = firestore.Collection(cName).Document(docId).Set(hm);
                newId = docId;
            }
            else
            {
                DocumentReference dr = firestore.Collection(cName).Document();
                newId = dr.Id;
                t = dr.Set(hm);
            }
            return t;
        }

        public Task GetDocument(string cName, string docId)
        {
            return firestore.Collection(cName).Document(docId).Get();
        }

    }
}