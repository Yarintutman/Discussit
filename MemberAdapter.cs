using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace Discussit
{
    /// <summary>
    /// Adapter for displaying members in a ListView.
    /// </summary>
    internal class MemberAdapter : BaseAdapter<Member>
    {
        private readonly Context context;
        private readonly List<Member> lstMembers;

        /// <summary>
        /// Initializes a new instance of the MemberAdapter class with the specified context.
        /// </summary>
        /// <param name="context">The context in which the adapter will be used.</param>
        public MemberAdapter(Context context)
        {
            this.context = context;
            lstMembers = new List<Member>();
        }
        public override Member this[int position] => lstMembers[position];

        public override int Count => lstMembers.Count;

        /// <summary>
        /// Gets the ID of the member at the specified position.
        /// </summary>
        /// <param name="position">The position of the member.</param>
        /// <returns>The ID of the member.</returns>
        public override long GetItemId(int position)
        {
            return position;
        }

        /// <summary>
        /// Gets a View that displays the data at the specified position in the adapter.
        /// </summary>
        /// <param name="position">The position of the item within the adapter's data set.</param>
        /// <param name="convertView">Not in use</param>
        /// <param name="parent">The parent that this view will eventually be attached to.</param>
        /// <returns>A View corresponding to the data at the specified position.</returns>
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

        /// <summary>
        /// Adds a member to the adapter and notifies observers of the change.
        /// </summary>
        /// <param name="member">The member to add.</param>
        public void AddMember(Member member)
        {
            lstMembers.Add(member);
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Removes a member from the adapter and notifies observers of the change.
        /// </summary>
        /// <param name="member">The member to remove.</param>
        public void RemoveMember(Member member)
        {
            lstMembers.Remove(member);
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Clears all members from the adapter and notifies observers of the change.
        /// </summary>
        public void Clear()
        {
            lstMembers.Clear();
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Gets the member with the specified user ID from the adapter.
        /// </summary>
        /// <param name="UID">The user ID of the member to retrieve.</param>
        /// <returns>The member with the specified user ID, or null if not found.</returns>
        public Member GetMemberByUID(string UID)
        {
            return lstMembers.FirstOrDefault(Member => UID == Member.UserID);
        }

        /// <summary>
        /// Checks if the adapter contains a member with the specified user ID.
        /// </summary>
        /// <param name="UID">The user ID of the member to check for.</param>
        /// <returns>True if the adapter contains a member with the specified user ID, otherwise false.</returns>
        public bool HasMember(string UID)
        {
            return lstMembers.Contains(GetMemberByUID(UID));
        }
    }
}