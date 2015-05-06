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
using Android.Support.V4.View;

namespace App.Adapter
{
    public class ViewPagerAdapter : PagerAdapter
    {
        private List<View> views;

        public ViewPagerAdapter(List<View> views)
        {
            this.views = views;
        }
        public override int Count
        {
            get
            {
                if (this.views != null)
                {
                    return this.views.Count;
                }

                return 0;
            }
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object object1)
        {
            return (view == object1);
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.JavaCast<ViewPager>().RemoveView(this.views[position]);
        }

        public override Java.Lang.Object InstantiateItem(Android.Views.ViewGroup container, int position)
        {

            var v = container.JavaCast<ViewPager>();
            if (v != null)
            {
                v.AddView(this.views[position]);
            }
            return this.views[position];
        }
    }
}