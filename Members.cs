using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using AndroidX.Browser.Trusted;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Context = Android.Content.Context;

namespace Discussit
{
    internal class Members
    {
        private readonly FbData fbd;
        public string Path { get; private set; }
        public MemberAdapter MemberAdapter { get; }
        private IListenerRegistration onCollectionChangeListener;
        public long MemberCount => MemberAdapter.Count;

        public Member this[int position]
        {
            get
            {
                return MemberAdapter[position];
            }
        }

        public Members(Context context, string path)
        {
            MemberAdapter = new MemberAdapter(context);
            fbd = new FbData();
            Path = path;
        }

        public void AddSnapshotListener(Activity context)
        {
            onCollectionChangeListener = fbd.AddSnapshotListener(context, Path + "/" + General.MEMBERS_COLLECTION);
        }

        public void RemoveSnapshotListener()
        {
            onCollectionChangeListener?.Remove();
        }

        internal void AddMembers(IList<DocumentSnapshot> documents)
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

        public void RemoveMember(Member member)
        {
            MemberAdapter.RemoveMember(member);
        }

        public Task GetTypeMembers(string type)
        {
            return fbd.GetEqualToDocs(Path + "/" + General.MEMBERS_COLLECTION, General.FIELD_MEMBER_TYPE, type);
        }

        internal Task GetMembers()
        {
            return fbd.GetCollection(Path + "/" + General.MEMBERS_COLLECTION);
        }

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
            fbd.RemoveFromArray(General.USERS_COLLECTION, lastLeader.UserID, General.FIELD_USER_MANAGING_COMMUNITIES, Path);
        }

        public Member GetMemberByUID(string UID)
        {
            return MemberAdapter.GetMemberByUID(UID);
        }

        public bool HasMember(string UID)
        {
            return MemberAdapter.HasMember(UID);
        }
    }
}