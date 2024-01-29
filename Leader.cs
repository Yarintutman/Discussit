﻿using Android.App;
using Android.Content;
using Android.Gestures;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discussit
{
    internal class Leader : Admin
    {
        public Leader(string userID, string communityPath) : base(userID, communityPath) { }

        public Leader() { }

        public override HashMap GetHashMap()
        {
            HashMap hm = base.GetHashMap();
            hm.Put(General.FIELD_MEMBER_TYPE, Application.Context.Resources.GetString(Resource.String.leader));
            return hm;
        }
    }
}