using Android.Gms.Tasks;
using Java.Util;
using Newtonsoft.Json;

namespace Discussit
{
    internal class User
    {
        public string Id
        {
            get
            {
                return fbd.GetUserId();
            }
            set
            {
                Id = value;
            }
        }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        private readonly FbData fbd;
        private readonly SpData spd;
        [JsonIgnore]
        public bool IsRegistered { get; private set; }

        public User()
        {
            fbd = new FbData();
            spd = new SpData(General.SP_FILE_NAME);
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
            Task TaskSetDocument = fbd.SetDocument(General.USERS_COLLECTION, Id, out string id, GetHashMap());
            Id = id;
            return TaskSetDocument;
        }

        private HashMap GetHashMap()
        {
            HashMap hm = new HashMap();
            hm.Put(General.FIELD_USERNAME, Username);
            return hm;
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
    }
}