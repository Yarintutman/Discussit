using Java.Util;
using Newtonsoft.Json;
using System;

namespace Discussit
{
    /// <summary>
    /// Represents a member of a community.
    /// </summary>
    internal class Member
    {
        private readonly FbData fbd;
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserID { get; set; }
        public string CommunityPath { get; set; }
        public DateTime JoinDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the Member class with the specified user and community path.
        /// </summary>
        /// <param name="user">The user associated with the member.</param>
        /// <param name="communityPath">The path of the community associated with the member.</param>
        public Member(User user, string communityPath)
        {
            fbd = new FbData();
            UserID = user.Id;
            Name = user.Username;
            CommunityPath = communityPath;
            JoinDate = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the Member class.
        /// </summary>
        public Member()
        {
            fbd = new FbData();
        }

        /// <summary>
        /// Gets the path of the member document in Firestore.
        /// </summary>
        [JsonIgnore]
        public string Path
        {
            get
            {
                return CommunityPath + "/" + General.MEMBERS_COLLECTION + "/" + Id;
            }
        }

        /// <summary>
        /// Gets a HashMap representation of the member for Firestore.
        /// </summary>
        [JsonIgnore]
        public virtual HashMap HashMap
        {
            get
            {
                HashMap hm = new HashMap();
                hm.Put(General.FIELD_UID, UserID);
                hm.Put(General.FIELD_USERNAME, Name);
                hm.Put(General.FIELD_DATE, fbd.DateTimeToFirestoreTimestamp(JoinDate));
                return hm;
            }
        }

        /// <summary>
        /// Allows the member to leave the community.
        /// </summary>
        public void LeaveCommunity()
        {
            if (CommunityPath != null)
            {
                fbd.DeleteDocument(CommunityPath + "/" + General.MEMBERS_COLLECTION, Id);
            }
        }

        /// <summary>
        /// Determines if this member holds a higher rank than another member.
        /// </summary>
        /// <param name="member">The other member to compare.</param>
        /// <returns>returns false since member is the lowest rank in a community</returns>
        public virtual bool IsHigherRank(Member member)
        {
            return false;
        }
    }
}