using Android.App;
using Java.Util;

namespace Discussit
{
    /// <summary>
    /// Represents an administrator within a community.
    /// Inherits from the base class <see cref="Member"/>.
    /// </summary>
    internal class Admin : Member
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Admin"/> class with the specified user and community path.
        /// </summary>
        /// <param name="user">The user associated with the administrator.</param>
        /// <param name="communityPath">The path of the community associated with the administrator.</param>
        public Admin(User user, string communityPath) : base(user, communityPath) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Admin"/> class.
        /// </summary>
        public Admin() { }

        /// <summary>
        /// Gets the HashMap representation of the administrator, including member type information.
        /// </summary>
        public override HashMap HashMap
        {
            get
            {
                HashMap hm = base.HashMap;
                hm.Put(General.FIELD_MEMBER_TYPE, Application.Context.Resources.GetString(Resource.String.admin));
                return hm;
            }
        }

        /// <summary>
        /// Checks if the specified member has a higher rank than the administrator.
        /// </summary>
        /// <param name="member">The member to compare ranks with.</param>
        /// <returns>True if the specified member has a lower rank, otherwise false.</returns>
        public override bool IsHigherRank(Member member)
        {
            return !(member is Admin);
        }
    }
}