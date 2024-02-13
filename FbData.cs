using Android.App;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase;
using Android.Gms.Tasks;
using Java.Util;
using Firebase.Storage;
using System;
using Android.Graphics;

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

        public Task DeleteDocument(string cName, string docId)
        {
            return firestore.Collection(cName).Document(docId).Delete();
        }

        public Task GetDocument(string cName, string docId)
        {
            return firestore.Collection(cName).Document(docId).Get();
        }

        public Task GetCollection(string cName)
        {
            return firestore.Collection(cName).Get();
        }

        public Task IncrementField(string cName, string docId, string fName, int increment)
        {
            return firestore.Collection(cName).Document(docId).Update(fName, FieldValue.Increment(increment));
        }

        public Task GetCollectionCount(string cName)
        {
            return firestore.Collection(cName).Count().Get(AggregateSource.Server);
        }

        public Task GetHighestValue(string cName, string fNameWhere, Java.Lang.Object fValue, string fNameOrderBy, int limit)
        {
            return firestore.Collection(cName).WhereEqualTo(fNameWhere, fValue).OrderBy(fNameOrderBy).Limit(limit).Get();
        }

        public Task GetEqualToDocs(string cName, string fName, Java.Lang.Object fValue)
        {
            return firestore.Collection(cName).WhereEqualTo(fName, fValue).Get();
        }

        public Task GetGreaterThan(string cName, string fName, Java.Lang.Object fValue)
        {
            return firestore.Collection(cName).WhereGreaterThan(fName, fValue).Get();
        }
        public Task GetGreaterThanOrEqual(string cName, string fName, Java.Lang.Object fValue)
        {
            return firestore.Collection(cName).WhereGreaterThanOrEqualTo(fName, fValue).Get();
        }

        public Task GetLessThan(string cName, string fName, Java.Lang.Object fValue)
        {
            return firestore.Collection(cName).WhereLessThan(fName, fValue).Get();
        }
        public Task GetLessThanOrEqual(string cName, string fName, Java.Lang.Object fValue)
        {
            return firestore.Collection(cName).WhereLessThanOrEqualTo(fName, fValue).Get();
        }
        public Task IncrementField(string cName, string docId, string fName, double incrementBy)
        {
            return firestore.Collection(cName).Document(docId).Update(fName, FieldValue.Increment(incrementBy));
        }

        public Task UpdateField(string cName, string docId, string fName, Java.Lang.Object fValue)
        {
            return firestore.Collection(cName).Document(docId).Update(fName, fValue);
        }

        public Task UnionArray(string cName, string docId, string fName, Java.Lang.Object fValue)
        {
            return firestore.Collection(cName).Document(docId).Update(fName, FieldValue.ArrayUnion(fValue));
        }

        public Task RemoveFromArray(string cName, string docId, string fName, Java.Lang.Object fValue)
        {
            return firestore.Collection(cName).Document(docId).Update(fName, FieldValue.ArrayRemove(fValue));
        }

        public IListenerRegistration AddSnapshotListener(Activity activity, string cName)
        {
            return firestore.Collection(cName).AddSnapshotListener((Firebase.Firestore.IEventListener)activity);
        }

        public IListenerRegistration AddSnapshotListener(Activity activity, string cName, string docId)
        {
            return firestore.Collection(cName).Document(docId).AddSnapshotListener((Firebase.Firestore.IEventListener)activity);
        }
        public UploadTask SaveImage(string fbImagePath, Bitmap bitmap)
        {
            StorageReference storageReference = FirebaseStorage.Instance.GetReference(fbImagePath);
            Byte[] imgBytes = General.BitmapToByteArray(bitmap);
            return storageReference.PutBytes(imgBytes);
        }

        public Task GetDownloadUrl(string fbImagePath)
        {
            //(Android.Net.Uri)task.Result
            StorageReference storageReference = FirebaseStorage.Instance.GetReference(fbImagePath);
            return storageReference.GetDownloadUrl();
        }
        public string GetNewCollectionId()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        public Java.Sql.Timestamp DateTimeToFirestoreTimestamp(DateTime dt)
        {
            dt = TimeZoneInfo.ConvertTimeToUtc(dt);
            DateTime ofset = new DateTime(1970, 1, 1);
            TimeSpan ts = dt - new DateTime(ofset.Ticks);
            return new Java.Sql.Timestamp((long)ts.TotalMilliseconds);
        }

        public DateTime FirestoreTimestampToDateTime(Firebase.Timestamp ts)
        {
            Java.Util.Date d = ts.ToDate();
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(d.Time);
            return TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.Local);
        }

    }
}