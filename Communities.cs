using Android.App;
using Android.Gms.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;
using static Java.Util.Jar.Attributes;

namespace Discussit
{
    internal class Communities
    {
        private readonly FbData fbd;
        public CommunityAdapter CommunityAdapter { get; }
        private IListenerRegistration onCollectionChangeListener;

        public Community this[int position]
        {
            get
            {
                return CommunityAdapter[position];
            }
        }

        public Communities(Activity context)
        {
            CommunityAdapter = new CommunityAdapter(context);
            fbd = new FbData();
        }

        public void AddSnapshotListener(Activity context)
        {
            onCollectionChangeListener = fbd.AddSnapshotListener(context, General.COMMUNITIES_COLLECTION);
        }

        public void RemoveSnapshotListener()
        {
            onCollectionChangeListener?.Remove();
        }

        internal void AddCommunities(IList<DocumentSnapshot> documents)
        {
            CommunityAdapter.Clear();
            Community community;
            foreach (DocumentSnapshot document in documents)
            {
                community = new Community
                {
                    Id = document.Id,
                    Name = document.GetString(General.FIELD_COMMUNITY_NAME),
                    Description = document.GetString(General.FIELD_COMMUNITY_DESCRIPTION)
                };
                CommunityAdapter.AddCommunity(community);
            }
        }

        internal Task GetCommunities()
        {
            return fbd.GetCollection(General.COMMUNITIES_COLLECTION);
        }
    }
}