using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
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

        public void SetMembers(IList<DocumentSnapshot> documents)
        {
            Member member;
            FbData fbd = new FbData();
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
                AddMember(member);
            }
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