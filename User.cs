using Android.Gms.Tasks;
using Android.Runtime;
using Java.Interop;
using Java.Util;
using Newtonsoft.Json;

namespace Discussit
{
    internal class User
    {
        public string Id => fbd.GetUserId();
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


        public User()
        {
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

        internal Task Register()
        {
            return fbd.Register(Email, Password);
        }

        internal Task Login()
        {
            return fbd.Login(Email, Password);
        }

        internal bool Save()
        {
            bool success = spd.PutString(General.KEY_EMAIL, Email);
            success = success && spd.PutString(General.KEY_PASSWORD, Password);
            success = success && spd.PutBool(General.KEY_REGISTERED, true);
            return success;
        }

        internal Task SetFbUser()
        {
            Task TaskSetDocument = fbd.SetDocument(General.USERS_COLLECTION, Id, out _, GetHashMap());
            return TaskSetDocument;
        }

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

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static User GetUserJson(string json)
        {
            return JsonConvert.DeserializeObject<User>(json);
        }

        public void Forget()
        {
            spd.PutBool(General.KEY_REGISTERED, false);
        }

        public Task GetUserData()
        {
            return fbd.GetDocument(General.USERS_COLLECTION, Id);
        }

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
    }
}