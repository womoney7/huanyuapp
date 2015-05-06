using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace App
{
    [Activity(Label = "环宇汽车")]
    public class MainActivity : TabActivity
    {
        private RadioGroup radiogroup;

        private long exitTime = 0;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainTabs);

            this.TabHost.AddTab(this.TabHost.NewTabSpec("Home").SetIndicator("Home").SetContent(new Intent(this, typeof(HomeActivity))));
            this.TabHost.AddTab(this.TabHost.NewTabSpec("Message").SetIndicator("Message").SetContent(new Intent(this, typeof(MessageActivity))));
            this.TabHost.AddTab(this.TabHost.NewTabSpec("Map").SetIndicator("Map").SetContent(new Intent(this, typeof(MapActivity))));
            this.TabHost.AddTab(this.TabHost.NewTabSpec("Setting").SetIndicator("Setting").SetContent(new Intent(this, typeof(SettingActivity))));

            // Get our button from the layout resource,
            // and attach an event to it
            //Button button = FindViewById<Button>(Resource.Id.MyButton);
            //button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
           
            radiogroup = FindViewById<RadioGroup>(Resource.Id.main_tab);
            radiogroup.CheckedChange += (obj, e) =>
            {
                switch (e.CheckedId)
                {
                    case Resource.Id.radio_button2:
                        this.TabHost.SetCurrentTabByTag("Home");
                        break;
                    case Resource.Id.radio_button4:
                        this.TabHost.SetCurrentTabByTag("Message");
                        break;
                    case Resource.Id.radio_button1:

                        this.TabHost.SetCurrentTabByTag("Map");
                        break;
                    case Resource.Id.radio_button3:
                        this.TabHost.SetCurrentTabByTag("Setting");
                        break;
                    default:
                        break;
                }
            };

            RadioButton radiobtn = this.FindViewById<RadioButton>(Resource.Id.radio_button2);
            radiobtn.Checked = true;


        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.Action == KeyEventActions.Down && e.KeyCode == Keycode.Back)
            {
                Exit();
                return false;
            }
            return base.DispatchKeyEvent(e);
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

