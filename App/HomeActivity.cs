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
using System.Timers;

namespace App
{
    [Activity(Label = "环宇汽车")]
    public class HomeActivity : Activity
    {
        private ViewPager viewPager;

        private LinearLayout ll_point;
        private FrameLayout frameLayout;
        List<View> viewList;
        private int[] image_id = { Resource.Drawable.a, Resource.Drawable.b, Resource.Drawable.c };
        private int frameheight;
        List<ImageView> pointList;
        private Timer timer;
        private LayoutInflater layoutInflater;
        private int window_width;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Android.Util.DisplayMetrics dm = new Android.Util.DisplayMetrics();
            this.WindowManager.DefaultDisplay.GetMetrics(dm);
            window_width = dm.WidthPixels;
            frameheight = (int)this.Resources.GetDimension(Resource.Dimension.headimage_frameheight);
            this.SetContentView(Resource.Layout.Home);
            InitHeadImage();

            timer = new Timer(5000);
            timer.Elapsed += (sender, args) =>
            {
                this.RunOnUiThread(() =>
                {
                    int index = viewPager.CurrentItem;
                    if (index == viewList.Count - 1)
                        index = 0;
                    else
                        index++;
                    viewPager.SetCurrentItem(index, true);
                });
            };
            timer.Enabled = true;


            var btnPinggu = this.FindViewById<Button>(Resource.Id.cheliangpinggu);
            btnPinggu.Click += (s, e) =>
            {
                Intent intent = new Intent(this, typeof(QichePingguActivity));
                this.StartActivity(intent);
            };
        }


        void InitHeadImage()
        {
            layoutInflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
            View headview = layoutInflater.Inflate(Resource.Layout.HeadImage, null);
            viewPager = headview.FindViewById<ViewPager>(Resource.Id.imageviewpager);
            ll_point = headview.FindViewById<LinearLayout>(Resource.Id.ll_point);
            frameLayout = headview.FindViewById<FrameLayout>(Resource.Id.fl_main);
            LinearLayout headimageContr = this.FindViewById<LinearLayout>(Resource.Id.headimagecontr);
            InitPagerChild();
            frameLayout.LayoutParameters.Height = frameheight;
            viewPager.Adapter = new App.Adapter.ViewPagerAdapter(viewList);
            viewPager.PageSelected += (sendr, e) =>
            {
                DrawPoint(e.Position);
            };
            DrawPoint(0);
            headimageContr.AddView(headview);

        }

        void DrawPoint(int index)
        {
            foreach (var item in pointList)
            {
                item.SetImageResource(Resource.Drawable.indicator);
            }
            pointList[index].SetImageResource(Resource.Drawable.indicator_focused);

        }

        void InitPagerChild()
        {
            viewList = new List<View>();
            foreach (var item in image_id)
            {
                ImageView imageview = new ImageView(this);
                imageview.SetScaleType(Android.Widget.ImageView.ScaleType.FitXy);
                Android.Graphics.Bitmap bitmap = Android.Graphics.BitmapFactory.DecodeResource(this.Resources, item);
                var bitmap1 = GetBitmap(bitmap, window_width);
                //if (bitmap1.Height < frameheight)
                frameheight = bitmap1.Height;
                imageview.SetImageBitmap(bitmap1);
                viewList.Add(imageview);

            }

            initPoint();
        }

        void initPoint()
        {
            pointList = new List<ImageView>();
            ImageView imageView;
            foreach (var item in image_id)
            {
                imageView = new ImageView(this);
                imageView.SetBackgroundResource(Resource.Drawable.indicator);
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
                layoutParams.LeftMargin = 10;
                layoutParams.RightMargin = 10;
                ll_point.AddView(imageView, layoutParams);
                pointList.Add(imageView);
            }


        }

        Android.Graphics.Bitmap GetBitmap(Android.Graphics.Bitmap bitmap, int width)
        {
            int w = bitmap.Width;
            int h = bitmap.Height;
            Android.Graphics.Matrix matrix = new Android.Graphics.Matrix();
            float scale = (float)width / w;
            matrix.PostScale(scale, scale);

            return Android.Graphics.Bitmap.CreateBitmap(bitmap, 0, 0, w, h, matrix, true);
        }
    }
}