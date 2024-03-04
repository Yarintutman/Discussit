using Android.App;
using Java.Util;

namespace Discussit
{
    /// <summary>
    /// Represents a leader in a community, extending the privileges of an admin.
    /// </summary>
    internal class Leader : Admin
    {
        /// <summary>
        /// Initializes a new instance of the Leader class with the specified user and community path.
        /// </summary>
        /// <param name="user">The user associated with the leader.</param>
        /// <param name="communityPath">The path of the community associated with the leader.</param>
        public Leader(User user, string communityPath) : base(user, communityPath) { }

        /// <summary>
        /// Initializes a new instance of the Leader class.
        /// </summary>
        public Leader() { }

        /// <summary>
        /// Gets the HashMap representation of the leader, including member type information.
        /// </summary>
        public override HashMap HashMap
        {
            get
            {
                HashMap hm = base.HashMap;
                hm.Put(General.FIELD_MEMBER_TYPE, Application.Context.Resources.GetString(Resource.String.leader));
                return hm;
            }
        }

        /// <summary>
        /// Determines whether this leader has a higher rank than the specified member.
        /// </summary>
        /// <param name="member">The member to compare with this leader.</param>
        /// <returns>returns true since leaders are considered the highest rank and there is only one leader.</returns>
        public override bool IsHigherRank(Member member)
        {
            return true;
        }
    }
}