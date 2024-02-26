using Android.Views;
using Android.Widget;
using static Java.Util.Jar.Attributes;
using System.Collections.Generic;
using Android.Content;
using Android.App;
using Firebase.Firestore;

namespace Discussit
{
    internal class CommunityAdapter : BaseAdapter<Community>
    {
        private readonly Context context;
        private readonly List<Community> lstCommunities;
        public CommunityAdapter(Context context)
        {
            this.context = context;
            lstCommunities = new List<Community>();
        }
        public override Community this[int position] => lstCommunities[position];

        public override int Count => lstCommunities.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

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

        public void AddCommunity(Community community)
        {
            lstCommunities.Add(community);
            NotifyDataSetChanged();
        }
        public void Clear()
        {
            lstCommunities.Clear();
            NotifyDataSetChanged();
        }
    }
}