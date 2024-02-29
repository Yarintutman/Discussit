using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

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
            Member member = lstMembers[position];
            TextView tvName = v.FindViewById<TextView>(Resource.Id.tvMember);
            TextView tvRank = v.FindViewById<TextView>(Resource.Id.tvMemberRank);
            tvName.Text = member.Name;
            if (member is Leader)
                tvRank.Text = Application.Context.Resources.GetString(Resource.String.leader);
            else if (member is Admin)
                tvRank.Text = Application.Context.Resources.GetString(Resource.String.admin);
            else
                tvRank.Text = Application.Context.Resources.GetString(Resource.String.member);
            return v;
        }

        public void AddMember(Member member)
        {
            lstMembers.Add(member);
            NotifyDataSetChanged();
        }
        
        public void RemoveMember(Member member)
        {
            lstMembers.Remove(member);
            NotifyDataSetChanged();
        }
        
        public void Clear()
        {
            lstMembers.Clear();
            NotifyDataSetChanged();
        }

        public Member GetMemberByUID(string UID)
        {
            return lstMembers.FirstOrDefault(Member => UID == Member.UserID);
        }

        public bool HasMember(string UID)
        {
            return lstMembers.Contains(GetMemberByUID(UID));
        }
    }
}