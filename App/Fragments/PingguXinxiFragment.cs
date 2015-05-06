using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.App;

namespace App.Fragments
{
    public class PingguXinxiFragment : Android.Support.V4.App.Fragment
    {
        RadioGroup pingtab;
        RadioButton wgjcRdbtn;

        WaiguanFragment frag1 = new WaiguanFragment();

        public PingguXinxiFragment()
        {
            QichePingguActivity.OnWindowFocusChangedEvent += QichePingguActivity_OnWindowFocusChangedEvent;
        }



        private void QichePingguActivity_OnWindowFocusChangedEvent(Android.App.Activity activity)
        {
            if (frag1.WindowFocusChangedAction != null)
            {
                frag1.WindowFocusChangedAction(activity);
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string tabname = this.Arguments.GetString("TabName");
            View v = inflater.Inflate(Resource.Layout.PingguXinxiFragment, container, false);
            pingtab = v.FindViewById<RadioGroup>(Resource.Id.pingguoxixin_tab);
            wgjcRdbtn = v.FindViewById<RadioButton>(Resource.Id.wgjcRdbtn);
            pingtab.CheckedChange += pingtab_CheckedChange;
            //v.FindViewById<TextView>(Resource.Id.fragment_text).Text = tabname;
            return v;
        }


        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            Android.Support.V4.App.FragmentTransaction fragmentTransaction = this.FragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.pingguoxixin_content, frag1);
            fragmentTransaction.Commit();
            wgjcRdbtn.Checked = true;
        }


        void pingtab_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            Android.Support.V4.App.Fragment frag = null;
            switch (e.CheckedId)
            {
                case Resource.Id.wgjcRdbtn:
                    frag = frag1;
                    break;

                default:
                    break;
            }

            if (frag != null)
            {
                Android.Support.V4.App.FragmentTransaction fragmentTransaction = this.FragmentManager.BeginTransaction();
                fragmentTransaction.Replace(Resource.Id.pingguoxixin_content, frag);
                fragmentTransaction.Commit();
            }
        }
    }
}