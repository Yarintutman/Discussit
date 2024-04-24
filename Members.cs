using Android.App;
using Android.Gms.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;
using Context = Android.Content.Context;

namespace Discussit
{
    /// <summary>
    /// Represents a collection of members within a community.
    /// </summary>
    internal class Members
    {
        private readonly FbData fbd;
        public string Path { get; private set; }
        public MemberAdapter MemberAdapter { get; }
        private IListenerRegistration onCollectionChangeListener;
        public long MemberCount => MemberAdapter.Count;

        /// <summary>
        /// Gets the member at the specified position in the collection.
        /// </summary>
        /// <param name="position">The position of the member to retrieve.</param>
        /// <returns>The member at the specified position.</returns>
        public Member this[int position]
        {
            get
            {
                return MemberAdapter[position];
            }
        }

        /// <summary>
        /// Initializes a new instance of the Members class with the specified context and path.
        /// </summary>
        /// <param name="context">The context in which the members will be used.</param>
        /// <param name="path">The path to the collection of members.</param>
        public Members(Context context, string path)
        {
            MemberAdapter = new MemberAdapter(context);
            fbd = new FbData();
            Path = path;
        }

        /// <summary>
        /// Adds a snapshot listener to the collection of members.
        /// </summary>
        /// <param name="context">The activity context.</param>
        public void AddSnapshotListener(Activity context)
        {
            onCollectionChangeListener = fbd.AddSnapshotListener(context, Path + "/" + General.MEMBERS_COLLECTION);
        }

        /// <summary>
        /// Removes the snapshot listener from the collection of members.
        /// </summary>
        public void RemoveSnapshotListener()
        {
            onCollectionChangeListener?.Remove();
        }

        /// <summary>
        /// Adds members from the provided list of document snapshots to the collection.
        /// </summary>
        /// <param name="documents">The list of document snapshots representing members.</param>
        public void AddMembers(IList<DocumentSnapshot> documents)
        {
            MemberAdapter.Clear();
            Member member;
            foreach (DocumentSnapshot document in documents)
            {
                string type = document.GetString(General.FIELD_MEMBER_TYPE);
                if (type == Application.Context.Resources.GetString(Resource.String.leader))
                    member = new Leader();
                else if (type == Application.Context.Resources.GetString(Resource.String.admin))
                    member = new Admin();
                else
                    member = new Member();
                member.Id = document.Id;
                member.UserID = document.GetString(General.FIELD_UID);
                member.Name = document.GetString(General.FIELD_USERNAME);
                member.JoinDate = fbd.FirestoreTimestampToDateTime(document.GetTimestamp(General.FIELD_DATE));
                member.CommunityPath = Path;
                MemberAdapter.AddMember(member);
            }
        }

        /// <summary>
        /// Removes the specified member from the collection.
        /// </summary>
        /// <param name="member">The member to be removed.</param>
        public void RemoveMember(Member member)
        {
            MemberAdapter.RemoveMember(member);
        }

        /// <summary>
        /// Gets all members from the collection.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public  Task GetMembers()
        {
            return fbd.GetCollection(Path + "/" + General.MEMBERS_COLLECTION);
        }

        /// <summary>
        /// Promotes the specified member to an admin role.
        /// </summary>
        /// <param name="member">The member to be promoted.</param>
        public void Promote(Member member)
        {
            MemberAdapter.RemoveMember(member);
            Admin admin = new Admin
            {
                Id = member.Id,
                UserID = member.UserID,
                CommunityPath = member.CommunityPath,
                JoinDate = member.JoinDate
            };
            MemberAdapter.AddMember(admin);
            fbd.UpdateField(admin.CommunityPath + "/" + General.MEMBERS_COLLECTION, admin.Id, General.FIELD_MEMBER_TYPE, 
                Application.Context.Resources.GetString(Resource.String.admin));
            fbd.UnionArray(General.USERS_COLLECTION, admin.UserID, General.FIELD_USER_MANAGING_COMMUNITIES, Path);
        }

        /// <summary>
        /// Demotes the specified admin to a regular member.
        /// </summary>
        /// <param name="admin">The admin to be demoted.</param>
        public void Demote(Admin admin)
        {
            MemberAdapter.RemoveMember(admin);
            Member member = new Member
            {
                Id = admin.Id,
                UserID = admin.UserID,
                CommunityPath = admin.CommunityPath,
                JoinDate = admin.JoinDate
            };
            MemberAdapter.AddMember(member);
            fbd.UpdateField(member.CommunityPath + "/" + General.MEMBERS_COLLECTION, member.Id, General.FIELD_MEMBER_TYPE,
                Application.Context.Resources.GetString(Resource.String.member));
            fbd.RemoveFromArray(General.USERS_COLLECTION, admin.UserID, General.FIELD_USER_MANAGING_COMMUNITIES, Path);
        }

        /// <summary>
        /// Sets the specified member as the leader of the community.
        /// </summary>
        /// <param name="member">The member to be set as the leader.</param>
        public void SetLeader(Member member)
        {
            if (member.GetType() == typeof(Admin))
                MemberAdapter.RemoveMember((Admin)member);
            else
                MemberAdapter.RemoveMember(member);
            Leader newLeader = new Leader
            {
                Id = member.Id,
                UserID = member.UserID,
                CommunityPath = member.CommunityPath,
                JoinDate = member.JoinDate
            };
            MemberAdapter.AddMember(newLeader);
            fbd.UpdateField(newLeader.CommunityPath + "/" + General.MEMBERS_COLLECTION, newLeader.Id, General.FIELD_MEMBER_TYPE,
                Application.Context.Resources.GetString(Resource.String.leader));
            fbd.UnionArray(General.USERS_COLLECTION, newLeader.UserID, General.FIELD_USER_MANAGING_COMMUNITIES, Path);
        }

        /// <summary>
        /// Transfers the leadership from the current leader to the specified new leader.
        /// </summary>
        /// <param name="currentLeader">The current leader.</param>
        /// <param name="newLeader">The new leader.</param>
        public void TransferLeader(Leader currentLeader, Member newLeader)
        {
            MemberAdapter.RemoveMember(currentLeader);
            Admin lastLeader = new Admin
            {
                Id = currentLeader.Id,
                UserID = currentLeader.UserID,
                CommunityPath = currentLeader.CommunityPath,
                JoinDate = currentLeader.JoinDate
            };
            MemberAdapter.AddMember(lastLeader);
            SetLeader(newLeader);
            fbd.UpdateField(lastLeader.CommunityPath + "/" + General.MEMBERS_COLLECTION, lastLeader.Id, General.FIELD_MEMBER_TYPE,
                Application.Context.Resources.GetString(Resource.String.admin));
        }

        /// <summary>
        /// Gets the member with the specified UID.
        /// </summary>
        /// <param name="UID">The UID of the member to retrieve.</param>
        /// <returns>The member with the specified UID, if found; otherwise, null.</returns>
        public Member GetMemberByUID(string UID)
        {
            return MemberAdapter.GetMemberByUID(UID);
        }

        /// <summary>
        /// Checks if the member with the specified UID exists in the collection.
        /// </summary>
        /// <param name="UID">The UID of the member to check.</param>
        /// <returns>True if the member exists in the collection; otherwise, false.</returns>
        public bool HasMember(string UID)
        {
            return MemberAdapter.HasMember(UID);
        }

        /// <summary>
        /// Searches for members based on the specified search criteria.
        /// </summary>
        /// <param name="search">The search criteria.</param>
        public void Search(string search)
        {
            MemberAdapter.Search(search);
        }

        /// <summary>
        /// Clears the search result.
        /// </summary>
        public void ClearSearch()
        {
            MemberAdapter.ClearSearch();
        }

        /// <summary>
        /// Sorts the adapter by member's rank
        /// </summary>
        public void SortByRank()
        {
            MemberAdapter.SortByRank();
        }

        /// <summary>
        /// Sorts the adapter by member's join date
        /// </summary>
        public void SortByJoinDate()
        {
            MemberAdapter.SortByJoinDate();
        }

        /// <summary>
        /// Sorts the adapter by name
        /// </summary>
        public void SortByName()
        {
            MemberAdapter.SortByName();
        }
    }
}