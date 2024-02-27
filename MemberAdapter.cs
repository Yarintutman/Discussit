using Android.App;
using Android.Content;
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

        public void SetMembers(IList<DocumentSnapshot> documents)
        {
            Member member;
            FbData fbd = new FbData();
            foreach (DocumentSnapshot document in documents)
            {
                member = new Member
                {
                    Id = document.Id,
                    UserID = document.GetString(General.FIELD_UID),
                    Name = document.GetString(General.FIELD_USERNAME),
                    JoinDate = fbd.FirestoreTimestampToDateTime(document.GetTimestamp(General.FIELD_DATE)),
                };
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
    }
}