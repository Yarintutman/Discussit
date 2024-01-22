﻿using Android.Graphics;
using System.IO;

namespace Discussit
{
    internal static class General
    {
        public const string SP_FILE_NAME = "sp.data";
        public const string KEY_USERNAME = "name";
        public const string KEY_EMAIL = "email";
        public const string KEY_PASSWORD = "password";
        public const string KEY_REGISTERED = "registered";
        public const string KEY_USER = "user";
        public const string FIELD_USERNAME = "Username";
        public const string FIELD_USER_COMMUNITIES = "Communities";
        public const string FIELD_USER_MANAGING_COMMUNITIES = "Managing Communities";
        public const string FIELD_USER_POSTS = "Posts";
        public const string FIELD_USER_COMMENTS = "Comments";
        public const string FIELD_COMMUNITY_NAME = "Community Name";
        public const string FIELD_COMMUNITY_DESCRIPTION = "Community Description";
        public const string FIELD_POST_CREATOR = "Username";
        public const string FIELD_POST_TITLE = "Community Name";
        public const string FIELD_POST_DESCRIPTION = "Community Description";
        public const string FIELD_UID = "UserID";
        public const string FIELD_MEMBER_TYPE = "Member Type";
        public const string USERS_COLLECTION = "Users";
        public const string COMMUNITIES_COLLECTION = "Communities";
        public const string POSTS_COLLECTION = "Posts";
        public const string COMMENTS_COLLECTION = "Comments";
        public const string MEMBERS_COLLECTION = "Members";

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);
            return ms.ToArray();
        }
    }
}