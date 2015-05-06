using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App
{
    [Activity(Label = "»·ÓîÆû³µ")]
    public class SettingActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            this.SetContentView(Resource.Layout.SettingHome);

            Button btn1 = this.FindViewById<Button>(Resource.Id.setting_but1);
            Button btnOut = this.FindViewById<Button>(Resource.Id.setting_but4);


            btn1.Click += (s, e) =>
            {
                //System.Core.Authentication AutoCredit.HelloWorld

            };

            btnOut.Click += (bs, be) =>
            {
                var sharedPreferences = this.GetSharedPreferences("logininfo", FileCreationMode.Private);
                var edit = sharedPreferences.Edit();
                edit.Clear();
                edit.Commit();
                this.StartActivity(typeof(LoginActivity));
            };
        }
    }
}