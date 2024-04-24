using Android.App;
using Android.Gms.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;

namespace Discussit
{
    /// <summary>
    /// Represents a collection of communities.
    /// </summary>
    internal class Communities
    {
        private readonly FbData fbd; 
        public CommunityAdapter CommunityAdapter { get; } 
        private IListenerRegistration onCollectionChangeListener;

        /// <summary>
        /// Gets the community at the specified position in the collection.
        /// </summary>
        /// <param name="position">The position of the community in the collection.</param>
        public Community this[int position]
        {
            get
            {
                return CommunityAdapter[position];
            }
        }

        /// <summary>
        /// Initializes a new instance of the Communities class with the specified context.
        /// </summary>
        /// <param name="context">The activity context.</param>
        public Communities(Activity context)
        {
            CommunityAdapter = new CommunityAdapter(context); 
            fbd = new FbData(); 
        }

        /// <summary>
        /// Adds a snapshot listener to listen for changes in the communities collection.
        /// </summary>
        /// <param name="context">The activity context.</param>
        public void AddSnapshotListener(Activity context)
        {
            onCollectionChangeListener = fbd.AddSnapshotListener(context, General.COMMUNITIES_COLLECTION);
        }

        /// <summary>
        /// Removes the snapshot listener.
        /// </summary>
        public void RemoveSnapshotListener()
        {
            onCollectionChangeListener?.Remove();
        }

        /// <summary>
        /// Adds communities from the provided list of Firestore documents to the community adapter.
        /// </summary>
        /// <param name="documents">The list of Firestore documents representing communities.</param>
        public void AddCommunities(IList<DocumentSnapshot> documents)
        {
            CommunityAdapter.Clear(); 
            Community community;
            foreach (DocumentSnapshot document in documents)
            {
                community = new Community
                {
                    Id = document.Id,
                    Name = document.GetString(General.FIELD_COMMUNITY_NAME),
                    Description = document.GetString(General.FIELD_COMMUNITY_DESCRIPTION),
                    CreationDate = fbd.FirestoreTimestampToDateTime(document.GetTimestamp(General.FIELD_DATE)),
                    MemberCount = document.GetLong(General.FIELD_MEMBER_COUNT).LongValue(),
                    PostCount = document.GetLong(General.FIELD_POST_COUNT).LongValue()
                };
                CommunityAdapter.AddCommunity(community); 
            }
        }

        /// <summary>
        /// Retrieves all communities from the Firestore collection.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task GetCommunities()
        {
            return fbd.GetCollection(General.COMMUNITIES_COLLECTION);
        }

        /// <summary>
        /// Searches for communities based on the specified search criteria.
        /// </summary>
        /// <param name="search">The search criteria.</param>
        public void Search(string search)
        {
            CommunityAdapter.Search(search);
        }

        /// <summary>
        /// Clears the search result.
        /// </summary>
        public void ClearSearch()
        {
            CommunityAdapter.ClearSearch();
        }

        /// <summary>
        /// Sorts the adapter by the latest Communities
        /// </summary>
        public void SortByLatest()
        {
            CommunityAdapter.SortByLatest();
        }

        /// <summary>
        /// Sorts the adapter by the oldest Communities
        /// </summary>
        public void SortByOldest()
        {
            CommunityAdapter.SortByOldest();

        }

        /// <summary>
        /// Sorts the adapter by the amount of posts
        /// </summary>
        public void SortByPosts()
        {
            CommunityAdapter.SortByPosts();
        }

        /// <summary>
        /// Sorts the adapter by the amount of members
        /// </summary>
        public void SortByMembers()
        {
            CommunityAdapter.SortByMembers();
        }
    }
}