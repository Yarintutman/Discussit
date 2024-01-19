using Android.Views;
using Android.Widget;
using static Java.Util.Jar.Attributes;
using System.Collections.Generic;
using Android.Content;

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

            return v;
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