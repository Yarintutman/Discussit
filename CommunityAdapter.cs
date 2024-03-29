using Android.Views;
using Android.Widget;
using static Java.Util.Jar.Attributes;
using System.Collections.Generic;
using Android.Content;
using Android.App;
using Firebase.Firestore;
using System.Linq;

namespace Discussit
{
    /// <summary>
    /// Adapter for displaying a list of communities in a ListView.
    /// </summary>
    internal class CommunityAdapter : BaseAdapter<Community>
    {
        private readonly Context context;
        private List<Community> lstCommunities;

        /// <summary>
        /// Adapter for displaying a list of communities in a ListView.
        /// </summary>
        public CommunityAdapter(Context context)
        {
            this.context = context;
            lstCommunities = new List<Community>();
        }
        public override Community this[int position] => lstCommunities[position];

        public override int Count => lstCommunities.Count;

        /// <summary>
        /// Gets the item ID at the specified position.
        /// </summary>
        /// <param name="position">The position of the item in the list.</param>
        /// <returns>The item ID at the specified position.</returns>
        public override long GetItemId(int position)
        {
            return position;
        }

        /// <summary>
        /// Gets the view for displaying the community at the specified position.
        /// </summary>
        /// <param name="position">The position of the community in the list.</param>
        /// <param name="convertView">Not in use</param>
        /// <param name="parent">The parent ViewGroup that this view will eventually be attached to.</param>
        /// <returns>The view for displaying the community at the specified position.</returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater li = LayoutInflater.From(context);
            View v = li.Inflate(Resource.Layout.layout_community, parent, false);
            Community community = lstCommunities[position];
            TextView tvCommunityName = v.FindViewById<TextView>(Resource.Id.tvCommunityName);
            TextView tvMemberCount = v.FindViewById<TextView>(Resource.Id.tvMemberCount);
            TextView tvCommunityDescription = v.FindViewById<TextView>(Resource.Id.tvDescription);
            tvCommunityName.Text = community.Name;
            tvMemberCount.Text = community.MemberCount.ToString();
            tvCommunityDescription.Text = community.Description;
            return v;
        }

        /// <summary>
        /// Sets the list of communities to be displayed in the adapter.
        /// </summary>
        /// <param name="documents">The list of documents representing communities.</param>
        public void SetCommunities(IList<DocumentSnapshot> documents)
        {
            Community community;
            FbData fbd = new FbData();
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
                AddCommunity(community);
            }
        }

        /// <summary>
        /// Adds a community to the adapter.
        /// </summary>
        /// <param name="community">The community to add.</param>
        public void AddCommunity(Community community)
        {
            lstCommunities.Add(community);
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Clears all communities from the adapter.
        /// </summary>
        public void Clear()
        {
            lstCommunities.Clear();
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Sorts the list by the latest Communities
        /// </summary>
        public void SortByLatest()
        {
            lstCommunities = lstCommunities.OrderByDescending(communities => communities.CreationDate).ToList();
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Sorts the list by the oldest Communities
        /// </summary>
        public void SortByOldest()
        {
            lstCommunities = lstCommunities.OrderBy(communities => communities.CreationDate).ToList();
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Sorts the list by the amount of posts
        /// </summary>
        public void SortByPosts()
        {
            lstCommunities = lstCommunities.OrderByDescending(communities => communities.PostCount).ToList();
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Sorts the list by the amount of members
        /// </summary>
        public void SortByMembers()
        {
            lstCommunities = lstCommunities.OrderByDescending(communities => communities.MemberCount).ToList();
            NotifyDataSetChanged();
        }
    }
}