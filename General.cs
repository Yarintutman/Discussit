using Android.Graphics;
using Android.Runtime;
using System.Collections.Generic;
using System.IO;

namespace Discussit
{
    /// <summary>
    /// Static class containing general constants and utility methods for the application.
    /// </summary>
    internal static class General
    {
        public const string SP_FILE_NAME = "sp.data";
        public const string KEY_UID = "uid";
        public const string KEY_USERNAME = "name";
        public const string KEY_EMAIL = "email";
        public const string KEY_PASSWORD = "password";
        public const string KEY_REGISTERED = "registered";
        public const string KEY_USER = "user";
        public const string KEY_COMMUNITY = "community";
        public const string KEY_POST = "post";
        public const string KEY_COMMENT = "comment";
        public const string KEY_IS_COMMENT_RECURSIVE = "Is recursive";
        public const string KEY_GUEST = "Is guest";
        public const string FIELD_USERNAME = "Username";
        public const string FIELD_USER_COMMUNITIES = "Communities";
        public const string FIELD_USER_MANAGING_COMMUNITIES = "Managing Communities";
        public const string FIELD_USER_POSTS = "Posts";
        public const string FIELD_USER_COMMENTS = "Comments";
        public const string FIELD_DATE = "Date";
        public const string FIELD_COMMUNITY_NAME = "Community Name";
        public const string FIELD_COMMUNITY_DESCRIPTION = "Community Description";
        public const string FIELD_POST_CREATOR_UID = "UID";
        public const string FIELD_POST_CREATOR_NAME = "CreatorName";
        public const string FIELD_POST_TITLE = "Community Name";
        public const string FIELD_POST_DESCRIPTION = "Community Description";
        public const string FIELD_COMMENT_CREATOR_NAME = "CreatorName";
        public const string FIELD_COMMENT_CREATOR_UID = "UID";
        public const string FIELD_COMMENT_TITLE = "Community Name";
        public const string FIELD_COMMENT_DESCRIPTION = "Community Description";
        public const string FIELD_MEMBER_COUNT = "Members Count";
        public const string FIELD_POST_COUNT = "Posts Count";
        public const string FIELD_COMMENT_COUNT = "Comments Count";
        public const string FIELD_UID = "UserID";
        public const string FIELD_MEMBER_TYPE = "Member Type";
        public const string USERS_COLLECTION = "Users";
        public const string COMMUNITIES_COLLECTION = "Communities";
        public const string POSTS_COLLECTION = "Posts";
        public const string COMMENTS_COLLECTION = "Comments";
        public const string MEMBERS_COLLECTION = "Members";

        /// <summary>
        /// Converts a Bitmap image to a byte array.
        /// </summary>
        /// <param name="bitmap">The Bitmap image to convert.</param>
        /// <returns>A byte array representing the Bitmap image.</returns>
        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Converts a JavaList to a typed JavaList.
        /// </summary>
        /// <typeparam name="T">The type of elements in the resulting JavaList.</typeparam>
        /// <param name="jl">The JavaList to convert.</param>
        /// <returns>A typed JavaList containing elements of the specified type.</returns>
        public static JavaList<T> JavaListToType<T>(JavaList jl)
        {
            JavaList<T> newList = new JavaList<T>();
            for (int i = 0; i < jl.Count; i++)
            {
                newList.Add((T)jl[i]);
            }
            return newList;
        }

        /// <summary>
        /// Converts a JavaList of strings to a IList of Java.Lang.Object with specified string cutting.
        /// </summary>
        /// <param name="jl">The JavaList of strings to convert.</param>
        /// <param name="cutFrom">The string to cut from each element in the JavaList.</param>
        /// <returns>An IList of Java.Lang.Object with specified string cutting applied.</returns>
        public static IList<Java.Lang.Object> JavaListToIListWithCut(JavaList<string> jl, string cutFrom)
        {
            IList<Java.Lang.Object> newList = new List<Java.Lang.Object>();
            for (int i = 0; i < jl.Count; i++)
            {
                newList.Add(CutStringFrom(cutFrom, jl[i]));
            }
            return newList;
        }

        /// <summary>
        /// Cuts a substring from a string based on the specified cutting point.
        /// </summary>
        /// <param name="cutFrom">The string to cut from.</param>
        /// <param name="str">The string to cut.</param>
        /// <returns>The substring after the specified cutting point.</returns>
        public static string CutStringFrom(string cutFrom, string str)
        {
            return str.Substring(str.IndexOf(cutFrom) + 1);
        }

        /// <summary>
        /// Removes a substring from a string based on the specified cutting point.
        /// </summary>
        /// <param name="cutAfter">The string to cut after.</param>
        /// <param name="str">The string to modify.</param>
        /// <returns>The modified string with the substring removed.</returns>
        public static string RemoveFromString(string cutAfter, string str)
        {
            return str.Remove(0, str.LastIndexOf(cutAfter));
        }
    }
}