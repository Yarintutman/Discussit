using Android.Gms.Tasks;
using Android.Runtime;
using Firebase.Firestore;
using Java.Lang;
using Java.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discussit
{
    /// <summary>
    /// Represents a user in the application.
    /// </summary>
    internal class User
    {
        public string id;
        public string Id
        {
            get
            {
                return id != null? id : fbd.GetUserId();
            }
        }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        private readonly FbData fbd;
        private readonly SpData spd;
        [JsonIgnore]
        public bool IsRegistered { get; private set; }
        public JavaList<string> Communities { get; set; }
        public JavaList<string> ManagingCommunities { get; set; }
        public JavaList<string> Posts { get; set; }
        public JavaList<string> Comments { get; set; }

        /// <summary>
        /// Initializes a new instance of the User class.
        /// </summary>
        public User()
        {
            id = null;
            fbd = new FbData();
            spd = new SpData(General.SP_FILE_NAME);
            Communities = new JavaList<string>();
            ManagingCommunities = new JavaList<string>();
            Posts = new JavaList<string>();
            Comments = new JavaList<string>();
            IsRegistered = spd.GetBool(General.KEY_REGISTERED, false);
            if (IsRegistered)
            {
                Email = spd.GetString(General.KEY_EMAIL, string.Empty);
                Username = spd.GetString(General.KEY_USERNAME, string.Empty);
                Password = spd.GetString(General.KEY_PASSWORD, string.Empty);
            }
        }

        public User(string id)
        {
            this.id = id;
            fbd = new FbData();
            Communities = new JavaList<string>();
            ManagingCommunities = new JavaList<string>();
            Posts = new JavaList<string>();
            Comments = new JavaList<string>();
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal Task Register()
        {
            return fbd.Register(Email, Password);
        }

        /// <summary>
        /// Logs in the user.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal Task Login()
        {
            return fbd.Login(Email, Password);
        }

        /// <summary>
        /// Saves the user's information.
        /// </summary>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        internal bool Save()
        {
            bool success = spd.PutString(General.KEY_EMAIL, Email);
            success = success && spd.PutString(General.KEY_UID, Id);
            success = success && spd.PutString(General.KEY_PASSWORD, Password);
            success = success && spd.PutBool(General.KEY_REGISTERED, true);
            return success;
        }

        /// <summary>
        /// Saves the user's information.
        /// </summary>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        internal Task SetFbUser()
        {
            Task tskSetDocument = fbd.SetDocument(General.USERS_COLLECTION, Id, out _, GetHashMap());
            return tskSetDocument;
        }


        /// <summary>
        /// Updates the specified array field in the user's data and locally.
        /// </summary>
        /// <param name="FName">The name of the field.</param>
        /// <param name="value">The value to add to the field.</param>
        public void UpdateArrayField(string FName, string value)
        {
            fbd.UnionArray(General.USERS_COLLECTION, Id, FName, value);
            switch (FName) 
            {
                case General.FIELD_USER_COMMUNITIES:
                    Communities.Add(value); 
                    break;
                case General.FIELD_USER_MANAGING_COMMUNITIES:
                    ManagingCommunities.Add(value);
                    break;
                case General.FIELD_USER_POSTS:
                    Posts.Add(value);
                    break;
                case General.FIELD_USER_COMMENTS:
                    Comments.Add(value);
                    break;
            }
        }

        /// <summary>
        /// Removes the specified value from the array field in the user's data and locally.
        /// </summary>
        /// <param name="FName">The name of the field.</param>
        /// <param name="value">The value to remove from the field.</param>
        public void RemoveArrayField(string FName, string value)
        {
            fbd.RemoveFromArray(General.USERS_COLLECTION, Id, FName, value);
            switch (FName)
            {
                case General.FIELD_USER_COMMUNITIES:
                    Communities.Remove(value);
                    break;
                case General.FIELD_USER_MANAGING_COMMUNITIES:
                    ManagingCommunities.Remove(value);
                    break;
                case General.FIELD_USER_POSTS:
                    Posts.Remove(value);
                    break;
                case General.FIELD_USER_COMMENTS:
                    Comments.Remove(value);
                    break;
            }
        }

        /// <summary>
        /// Removes the specified list of values from the array field in the user's data and locally.
        /// </summary>
        /// <param name="FName">The name of the field.</param>
        /// <param name="value">The list of values to remove from the field.</param>
        public void RemoveArrayFields(string FName, List<string> value)
        {
            if (value != null && value.Count > 0)
            {
                List<Java.Lang.Object> lst = new List<Java.Lang.Object>();
                foreach (string str in value)
                    lst.Add(str);
                fbd.RemoveElementsFromArray(General.USERS_COLLECTION, Id, FName, lst);
                switch (FName)
                {
                    case General.FIELD_USER_COMMUNITIES:
                        Communities.RemoveAll(General.ListToJavaList(value));
                        break;
                    case General.FIELD_USER_MANAGING_COMMUNITIES:
                        ManagingCommunities.RemoveAll(General.ListToJavaList(value));
                        break;
                    case General.FIELD_USER_POSTS:
                        Posts.RemoveAll(General.ListToJavaList(value));
                        break;
                    case General.FIELD_USER_COMMENTS:
                        Comments.RemoveAll(General.ListToJavaList(value));
                        break;
                }
            }
        }

        /// <summary>
        /// Forgets the user's registration status and disables fast login to the application
        /// </summary>
        public void Forget()
        {
            spd.PutBool(General.KEY_REGISTERED, false);
        }

        /// <summary>
        /// Retrieves user data from the Firebase database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task GetUserData()
        {
            return fbd.GetDocument(General.USERS_COLLECTION, Id);
        }

        /// <summary>
        /// Creates a HashMap object containing user data.
        /// </summary>
        /// <returns>The HashMap containing user data.</returns>
        private HashMap GetHashMap()
        {
            HashMap hm = new HashMap();
            hm.Put(General.FIELD_USERNAME, Username);
            hm.Put(General.FIELD_USER_COMMUNITIES, Communities);
            hm.Put(General.FIELD_USER_MANAGING_COMMUNITIES, ManagingCommunities);
            hm.Put(General.FIELD_USER_POSTS, Posts);
            hm.Put(General.FIELD_USER_COMMENTS, Comments);
            return hm;
        }

        /// <summary>
        /// Sets the user's data based on the provided DocumentSnapshot.
        /// </summary>
        /// <param name="document">The DocumentSnapshot containing user data.</param>
        public void SetUser(DocumentSnapshot document)
        {
            Username = document.GetString(General.FIELD_USERNAME);
            Communities = General.JavaListToType<string>((JavaList)document.Get(General.FIELD_USER_COMMUNITIES));
            ManagingCommunities = General.JavaListToType<string>((JavaList)document.Get(General.FIELD_USER_MANAGING_COMMUNITIES));
            Posts = General.JavaListToType<string>((JavaList)document.Get(General.FIELD_USER_POSTS));
            Comments = General.JavaListToType<string>((JavaList)document.Get(General.FIELD_USER_COMMENTS));
        }

        public Task GetDocumentInList(string listType)
        {
            Task tskGetList = null;
            switch (listType)
            {
                case General.FIELD_USER_COMMUNITIES:
                    tskGetList = fbd.GetDocumentsInList(General.COMMUNITIES_COLLECTION,
                                           General.JavaListToIListWithCut(Communities, "/"));
                    break;
                case General.FIELD_USER_MANAGING_COMMUNITIES:
                    tskGetList = fbd.GetDocumentsInList(General.COMMUNITIES_COLLECTION,
                                           General.JavaListToIListWithCut(ManagingCommunities, "/"));
                    break;
                case General.FIELD_USER_POSTS:
                    tskGetList = fbd.GetDocumentsInList(General.COMMUNITIES_COLLECTION,
                                           General.JavaListToIListWithCut(Posts, "/"));
                    break;
                case General.FIELD_USER_COMMENTS:
                    tskGetList = fbd.GetDocumentsInList(General.COMMUNITIES_COLLECTION,
                                           General.JavaListToIListWithCut(Comments, "/"));
                    break;
            }
            return tskGetList;
        }

        /// <summary>
        /// Sends a password reset email to the specified email address.
        /// </summary>
        /// <param name="email">The email address associated with the account.</param>
        /// <returns>A task representing the asynchronous operation, which upon completion, indicates whether the password reset email was sent successfully.</returns>
        public Task SendResetPasswordEmail(string email)
        {
            return fbd.ResetPassword(email);
        }

        /// <summary>
        /// Serializes the user object to Json format.
        /// </summary>
        /// <returns>The Json representation of the user object.</returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Deserializes the Json string into a User object.
        /// </summary>
        /// <param name="json">The Json string representing the User object.</param>
        /// <returns>The User object deserialized from the Json string.</returns>
        public static User GetUserJson(string json)
        {
            return JsonConvert.DeserializeObject<User>(json);
        }
    }
}