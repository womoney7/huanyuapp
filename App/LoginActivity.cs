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
using Android.Content.PM;

namespace App
{
    [Activity(Label = "环宇汽车", ScreenOrientation = ScreenOrientation.Portrait, Icon = "@drawable/logo_huanyu")]
    public class LoginActivity : Activity
    {
        EditText txtLoginName;
        EditText txtPassword;
        ISharedPreferences sharedPreferences;
        private long exitTime = 0;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Login);
            // Create your application here
            sharedPreferences = this.GetSharedPreferences("logininfo", FileCreationMode.Private);
            Button btLogin = this.FindViewById<Button>(Resource.Id.btLogin);
            txtLoginName = this.FindViewById<EditText>(Resource.Id.loginname);
            txtPassword = this.FindViewById<EditText>(Resource.Id.password);
            txtLoginName.Text = "android";
            txtPassword.Text = "123456";
            btLogin.Click += btLogin_Click; 
        }

        void btLogin_Click(object sender, EventArgs e)
        {
            ProgressDialog dialog = ProgressDialog.Show(this, null, "正在加载...");

            string username = txtLoginName.Text.Trim();
            string password = txtPassword.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Toast.MakeText(this, "请输入用户名和密码！", ToastLength.Long)
                    .Show();
                return;
            }

            App.Untity.WebSocketProxy.Current.InvokeWithParamter("System.Core.Authentication", new object[] { username, password }, (sr, se) =>
                {
                    Dictionary<string, object> dic = sr as Dictionary<string, object>;
                    if (dic != null && dic.Count > 0)
                    {
                        this.RunOnUiThread(() =>
                            {

                                ISharedPreferencesEditor editor = sharedPreferences.Edit();
                                editor.PutString("username", username);
                                editor.PutString("password", password);
                                editor.Commit();
                                dialog.Dismiss();
                                this.StartActivity(typeof(MainActivity));
                            });

                    }
                    else
                    {

                        this.RunOnUiThread(() =>
                            {
                                dialog.Dismiss();
                                Toast.MakeText(this, "用户名或密码不正确！", ToastLength.Long)
                                    .Show();
                            });


                    }

                }, () => {
                this.RunOnUiThread(() =>
                    {
                        dialog.Dismiss();
                        Toast.MakeText(this, "请检查网络，可能没有连接上！", ToastLength.Long)
                            .Show();
                    });
            });
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                Exit();
                return false;
            }
            return base.OnKeyDown(keyCode, e);
        }

        public void Exit()
        {
            if (Java.Lang.JavaSystem.CurrentTimeMillis() - exitTime >= 2000)
            {
                Toast.MakeText(this.ApplicationContext, "再按一次退出", ToastLength.Short)
                    .Show();
                exitTime = Java.Lang.JavaSystem.CurrentTimeMillis();
            }
            else
            {
                this.Finish();
                Java.Lang.JavaSystem.Exit(0);
            }
        }
    }
}