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
using App.Adapter;
using App.Entity;

namespace App
{
    [Activity(Label = "环宇汽车", ParentActivity = typeof(MainActivity))]
    public class QichePingguActivity : Android.Support.V4.App.FragmentActivity
    {

        public delegate void WindowFocusChangedDelegate(Activity activity);

        public static event WindowFocusChangedDelegate OnWindowFocusChangedEvent;

        private TabHost mTabHost;
        private Android.Support.V4.View.ViewPager mViewPager;
        private PingguTabsPagerAdapter mPagerAdapter;
        private List<QichePingguTabItem> tabItemList = new List<QichePingguTabItem>() { new QichePingguTabItem() { Tag = "cheliangxinxi", TabName = "车辆信息" }, 
            new QichePingguTabItem() { Tag = "pingguxinxi", TabName = "评估信息" },
            new QichePingguTabItem() { Tag = "xianchanggujia", TabName = "现场估价" }
        };
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.SetContentView(Resource.Layout.QichePingguTabs);

            mTabHost = this.FindViewById<TabHost>(Android.Resource.Id.TabHost);
            mTabHost.Setup();
            mViewPager = this.FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.pager);

            mPagerAdapter = new PingguTabsPagerAdapter(this, mTabHost, mViewPager);

            foreach (var item in tabItemList)
            {
                var tabspec = mTabHost.NewTabSpec(item.Tag).SetIndicator(item.TabName);
                Bundle b = new Bundle();
                b.PutString("TabName", item.TabName);
                switch (item.Tag)
                {
                    case "cheliangxinxi":
                        mPagerAdapter.AddTab(tabspec, Java.Lang.Class.FromType(typeof(App.Fragments.CheLiangXinxiFragment)), b);
                        break;
                    case "pingguxinxi":
                        mPagerAdapter.AddTab(tabspec, Java.Lang.Class.FromType(typeof(App.Fragments.PingguXinxiFragment)), b);
                        break;
                    case "xianchanggujia":
                        mPagerAdapter.AddTab(tabspec, Java.Lang.Class.FromType(typeof(App.Fragments.XianChangGuJiaFragment)), b);
                        break;
                    default:
                        break;
                }


            }

            this.ActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (OnWindowFocusChangedEvent != null)
            {
                OnWindowFocusChangedEvent(this);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    return true;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }


    }
}