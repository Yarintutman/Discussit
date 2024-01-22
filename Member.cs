using Java.Util;

namespace Discussit
{
    internal class Member
    {
        private readonly FbData fbd;
        public string Id { get; set; }
        public string UserID { get; set; }
        public string CommunityPath { get; set; }
        public Member(string userID, string communityPath)
        {
            fbd = new FbData();
            UserID = userID;
            CommunityPath = communityPath;
        }
        public Member() { }

        public string GetPath()
        {
            return CommunityPath + "\\" + General.MEMBERS_COLLECTION + "\\" + Id;
        }

        public virtual HashMap GetHashMap()
        {
            HashMap hm = new HashMap();
            hm.Put(General.FIELD_UID, UserID);
            return hm;
        }

        public void LeaveCommunity()
        {
            if (CommunityPath == null)
            {
                
            }
        }
    }
}