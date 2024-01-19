using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discussit
{
    internal class Members
    {
        private readonly FbData fbd;
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

        public Members(Activity context)
        {
            MemberAdapter = new MemberAdapter(context);
            fbd = new FbData();
        }

        public void AddSnapshotListener(Activity context, string path)
        {
            onCollectionChangeListener = fbd.AddSnapshotListener(context, path + "\\" + General.MEMBERS_COLLECTION);
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
                member = new Member
                {
                    //add members
                };
                MemberAdapter.AddMember(member);
            }
        }

        internal Task GetMembers()
        {
            return fbd.GetCollection(General.COMMUNITIES_COLLECTION);
        }
    }
}