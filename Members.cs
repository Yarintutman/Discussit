using Android.App;
using Android.Content.Res;
using Android.Gms.Tasks;
using Firebase.Firestore;
using Firebase.Firestore.Auth;
using System.Collections.Generic;

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

        public Members(Activity context, string path)
        {
            MemberAdapter = new MemberAdapter(context);
            fbd = new FbData();
            Path = path;
        }

        public void AddSnapshotListener(Activity context)
        {
            onCollectionChangeListener = fbd.AddSnapshotListener(context, Path + "\\" + General.MEMBERS_COLLECTION);
        }

        public void RemoveSnapshotListener()
        {
            onCollectionChangeListener?.Remove();
        }

        internal void AddMember(IList<DocumentSnapshot> documents)
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
                member.UserID = document.GetString(General.FIELD_UID);
                member.CommunityPath = Path;
                MemberAdapter.AddMember(member);
            }
        }

        public Task GetTypeMembers(string type)
        {
            return fbd.GetEqualToDocs(Path + "\\" + General.MEMBERS_COLLECTION, General.FIELD_MEMBER_TYPE, type);
        }

        internal Task GetMembers()
        {
            return fbd.GetCollection(Path + "\\" + General.MEMBERS_COLLECTION);
        }
    }
}