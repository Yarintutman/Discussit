using Android.App;
using Android.Content;
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
    internal class Admin : Member
    {
        public Admin(User user, string communityPath) : base(user, communityPath) { }

        public Admin() { }

        public override HashMap HashMap
        {
            get
            {
                HashMap hm = base.HashMap;
                hm.Put(General.FIELD_MEMBER_TYPE, Application.Context.Resources.GetString(Resource.String.admin));
                return hm;
            }
        }
    }
}