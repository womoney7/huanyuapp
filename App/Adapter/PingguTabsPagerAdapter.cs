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
using Android.Support.V4.App;
using App.Fragments;

namespace App.Adapter
{
    public class PingguTabsPagerAdapter : FragmentPagerAdapter, TabHost.IOnTabChangeListener, Android.Support.V4.View.ViewPager.IOnPageChangeListener
    {

        private readonly Context _context;
        private readonly TabHost _tabHost;
        private readonly Android.Support.V4.View.ViewPager _viewPager;
        private readonly List<TabInfo> _tabs = new List<TabInfo>();

        public PingguTabsPagerAdapter(FragmentActivity activity, TabHost tabHost, Android.Support.V4.View.ViewPager pager)
            : base(activity.SupportFragmentManager)
        {
            _context = activity;
            _tabHost = tabHost;
            _viewPager = pager;
            _viewPager.Adapter = this;
            _tabHost.SetOnTabChangedListener(this);
            _viewPager.SetOnPageChangeListener(this);
        }

        public void AddTab(TabHost.TabSpec tabSpec, Java.Lang.Class clss, Bundle args)
        {
            tabSpec.SetContent(new DummyTabFactory(_context));
            var tag = tabSpec.Tag;
            var info = new TabInfo(tag, clss, args);
            _tabs.Add(info);
            _tabHost.AddTab(tabSpec);
            NotifyDataSetChanged();
        }

        public void OnPageScrollStateChanged(int state)
        {

        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {

        }

        public void OnPageSelected(int position)
        {
            // Unfortunately when TabHost changes the current tab, it kindly
            // also takes care of putting focus on it when not in touch mode.
            // The jerk.
            // This hack tries to prevent this from pulling focus out of our
            // ViewPager.
            var widget = _tabHost.TabWidget;
            var oldFocusability = widget.DescendantFocusability;
            widget.DescendantFocusability = DescendantFocusability.BlockDescendants;
            _tabHost.CurrentTab = position;
            widget.DescendantFocusability = oldFocusability;
        }

        public void OnTabChanged(string tabId)
        {
            var position = _tabHost.CurrentTab;
            _viewPager.CurrentItem = position;
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            var info = _tabs[position];
            return Android.Support.V4.App.Fragment.Instantiate(_context, info.Class.Name, info.Args);
        }

        public override int Count
        {
            get { return _tabs.Count; }
        }


        public class TabInfo
        {
            public string Tag;
            public Java.Lang.Class Class;
            public Bundle Args;
            public Android.Support.V4.App.Fragment Fragment { get; set; }

            public TabInfo(string tag, Java.Lang.Class _class, Bundle args)
            {
                Tag = tag;
                Class = _class;
                Args = args;
            }
        }

        public class DummyTabFactory : Java.Lang.Object, TabHost.ITabContentFactory
        {
            private readonly Context _context;

            public DummyTabFactory(Context context)
            {
                _context = context;
            }

            public View CreateTabContent(string tag)
            {
                var v = new View(_context);
                v.SetMinimumHeight(0);
                v.SetMinimumWidth(0);
                return v;
            }

        }
    }
}