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
    [Activity(Label = "��������", MainLauncher = true, Icon = "@drawable/logo_huanyu")]
    public class LunchActivity : Activity
    {
        private ProgressDialog mProgressDialog;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            this.SetContentView(Resource.Layout.Lunch);
            Handler h = new Handler();
            // ��ʾ������
            mProgressDialog = ProgressDialog.Show(this, null, "���ڼ���...");
            h.PostDelayed(() =>
                {
                    var sharedPreferences = this.GetSharedPreferences("logininfo", FileCreationMode.Private);  //username password
                    string username = sharedPreferences.GetString("username","");
                    string password = sharedPreferences.GetString("password","");
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        this.StartActivity(new Intent(this.Application, typeof(MainActivity)));
                    }
                    else
                    {
                        this.StartActivity(new Intent(this.Application, typeof(LoginActivity)));
                    }

                    mProgressDialog.Dismiss();
                    this.Finish();
                }, 2000);
        }
    }
}