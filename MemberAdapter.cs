using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang.Reflect;
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
        private List<Member> lstMembers;
        private List<Member> lstSearch;

        /// <summary>
        /// Initializes a new instance of the MemberAdapter class with the specified context.
        /// </summary>
        /// <param name="context">The context in which the adapter will be used.</param>
        public MemberAdapter(Context context)
        {
            this.context = context;
            lstMembers = new List<Member>();
        }

        public override Member this[int position] => lstSearch == null ? lstMembers[position] : lstSearch[position];

        public override int Count => lstSearch == null ? lstMembers.Count : lstSearch.Count;

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
            Member member;
            if (lstSearch == null)
                member = lstMembers[position];
            else
                member = lstSearch[position];
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

        /// <summary>
        /// Searches for members based on the specified search criteria and updates the search result list.
        /// </summary>
        /// <param name="search">The search criteria.</param>
        public void Search(string search)
        {
            lstSearch = lstMembers.Where(member => member.Name.ToLower().Contains(search.ToLower())).ToList();
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Clears the search result list.
        /// </summary>
        public void ClearSearch()
        {
            lstSearch = null;
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Sorts the list by member's rank
        /// </summary>
        public void SortByRank()
        {
            List<Member> lstTempMembers = lstMembers.Where(member => member is Leader).ToList();
            lstTempMembers.AddRange(lstMembers.Where(member => member is Admin && !(member is Leader)).ToList());
            lstTempMembers.AddRange(lstMembers.Where(member => !(member is Admin)).ToList());
            lstMembers = lstTempMembers;
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Sorts the list by member's join date
        /// </summary>
        public void SortByJoinDate()
        {
            lstMembers = lstMembers.OrderBy(member => member.JoinDate).ToList();
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Sorts the list by name
        /// </summary>
        public void SortByName()
        {
            lstMembers = lstMembers.OrderByDescending(member => member.Name).ToList();
            NotifyDataSetChanged();
        }
    }
}