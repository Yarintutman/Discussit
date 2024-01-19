using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discussit
{
    internal class MemberAdapter : BaseAdapter<Member>
    {
        private readonly Context context;
        private readonly List<Member> lstMembers;
        public MemberAdapter(Context context)
        {
            this.context = context;
            lstMembers = new List<Member>();
        }
        public override Member this[int position] => lstMembers[position];

        public override int Count => lstMembers.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater li = LayoutInflater.From(context);
            View v = li.Inflate(Resource.Layout.layout_member, parent, false);

            return v;
        }

        public void AddMember(Member member)
        {
            lstMembers.Add(member);
            NotifyDataSetChanged();
        }
        public void Clear()
        {
            lstMembers.Clear();
            NotifyDataSetChanged();
        }
    }
}