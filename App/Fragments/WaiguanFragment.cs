using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using App.Entity;
using Android.Graphics;

namespace App.Fragments
{
    public class WaiguanFragment : Android.Support.V4.App.Fragment, ISurfaceHolderCallback
    {
        private SurfaceView surview;
        App.CustomView.CImageMarkView markview;
        private List<PostionPoint> pointData = new List<PostionPoint>();
        private Action<Activity> _windowFocusChangedAction;
        PointF screenCenter = new PointF();
        PointF mapCenter = new PointF();
        PointF start = new PointF();
        PointF end = new PointF();
        PointF mid = new PointF();

        private float oldDist = 0;
        private float rate = 1.0f;
        private float oldRate = 0;
        private Canvas canvas;
        private Bitmap bm;
        private Bitmap b;

        private Paint paint = new Paint();
        MySurfaceViewMode mode = MySurfaceViewMode.None;
        public WaiguanFragment()
        {
            //_windowFocusChangedAction = new Action<Activity>(act =>
            //{
            //    // �������߶�
            //    Android.Graphics.Rect frame = new Android.Graphics.Rect();
            //    act.Window.DecorView.GetWindowVisibleDisplayFrame(frame);
            //    //surview.GetWindowVisibleDisplayFrame(frame);
            //    int statusBarHeight = frame.Top;

            //    // �������߶�
            //    int contentTop = act.Window.FindViewById(Window.IdAndroidContent).Top;
            //    //Log.Debugd(TAG, "contentTop===�������߶�=====" + contentTop);
            //    // statusBarHeight�����������״̬���ĸ߶�
            //    int titleBarHeight = contentTop - statusBarHeight;
            //    //Log.d(TAG, "titleBarHeight==�����������״̬���ĸ߶� ======" + titleBarHeight);
            //    float x = (act.WindowManager.DefaultDisplay.Width) / 2;
            //    float y = (act.WindowManager.DefaultDisplay.Height
            //            - statusBarHeight - titleBarHeight) / 2;
            //    /** ��Ļ���ĵ�λ������ */
            //    screenCenter.Set(x, y);
            //    /** ��ͼ���ĵ�λ������ */
            //    mapCenter.Set(x, y);


            //});

           
        }



        public Action<Activity> WindowFocusChangedAction
        {
            get
            {
                return _windowFocusChangedAction;
            }
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //bm = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.a);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.WaiguanFragment, container, false);
            markview = v.FindViewById<App.CustomView.CImageMarkView>(Resource.Id.markview);

            //surview = v.FindViewById<SurfaceView>(Resource.Id.sur_view);

            PostionPoint lPoint1 = new PostionPoint(100, 100);
            lPoint1.DrawableId = Resource.Drawable.mark1;
            PostionPoint lPoint2 = new PostionPoint(150, 150);
            lPoint2.DrawableId = Resource.Drawable.mark1;
            PostionPoint lPoint3 = new PostionPoint(300, 300);
            lPoint3.DrawableId = Resource.Drawable.mark1;

            pointData.Add(lPoint1);
            pointData.Add(lPoint2);
            pointData.Add(lPoint3);

            return v;
        }



        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            markview.Init(this.Activity, null, true, pointData);
            //SetCenterXY();
            //surview.Holder.AddCallback(this);
            //surview.Touch += surview_Touch;
            //Android.Graphics.Rect frame = new Android.Graphics.Rect();
            //surview.GetGlobalVisibleRect(frame);


            
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (b != null && !b.IsRecycled)
            {
                b.Recycle();
            }
        }

        void surview_Touch(object sender, View.TouchEventArgs e)
        {
            switch (e.Event.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    float startX = e.Event.GetX();
                    float startY = e.Event.GetY();
                    WhichItemImage(startX, startY);

                    start.Set(e.Event.GetX(), e.Event.GetY());
                    end.Set(e.Event.GetX(), e.Event.GetY());

                    mode = MySurfaceViewMode.Drag;
                    break;
                case MotionEventActions.PointerDown:
                    oldDist = Spacing(e.Event);
                    if (oldDist > 10f)
                    {
                        MidPoint(mid, e.Event);
                        mode = MySurfaceViewMode.Zoom;
                    }
                    break;
                case MotionEventActions.Up:
                    oldRate = rate;
                    // ��¼�ƶ����ͼ����λ��(���������Ļ������)
                    mapCenter.Set(mapCenter.X + (end.X - start.X), mapCenter.Y + (end.Y - start.Y));

                    break;
                case MotionEventActions.PointerUp:
                    mode = MySurfaceViewMode.None;
                    break;
                case MotionEventActions.Move:
                    if (mode == MySurfaceViewMode.Drag)
                    {

                        end.Set(e.Event.GetX(), e.Event.GetY());


                    }
                    else if (mode == MySurfaceViewMode.Zoom)
                    {

                        float newDist = Spacing(e.Event);

                        if (newDist > 10f)
                        {

                            rate = oldRate * (newDist / oldDist);

                        }

                    }
                    Draw();
                    break;

                default:
                    break;
            }

        }



        /// <summary>
        /// ����������жϣ���ΪͼƬ�Ƚ�С���������������X��Y��ֱ�Ӽ�20dp���أ�Ҳ�����������������ͼƬ�ĵ��������һ���߳�Ϊ40dp�������Σ�
        /// ��Ϊ�漰�����ţ���Ҫ����Scale�����ű��������Ե�������СҲ�Ǳ��
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void WhichItemImage(float x, float y)
        {
            for (int i = 0; i < pointData.Count; i++)
            {
                float x1 = mapCenter.X + (end.X - start.X) - b.Width / 2 - bm.Width * rate / 2 + 100 * rate + i * 50 * rate + 20;
                float x2 = mapCenter.X + (end.X - start.X) - b.Width / 2 - bm.Width * rate / 2 + 100 * rate + i * 50 * rate - 20;
                float y1 = mapCenter.Y + (end.Y - start.Y) - b.Height - bm.Height * rate / 2 + 125 * rate + i * 50 * rate + 20;
                float y2 = mapCenter.Y + (end.Y - start.Y) - b.Height - bm.Height * rate / 2 + 125 * rate + i * 50 * rate - 20;

                if (x <= x1 && x >= x2 && y <= y1 && y >= y2)
                {
                    if (i == 0)
                    {
                        Toast.MakeText(this.Activity, "����b222222" + "", ToastLength.Short).Show();

                        return;
                    }
                    else if (i == 1)
                    {
                        Toast.MakeText(this.Activity, "����b333333333" + "", ToastLength.Short).Show();

                        return;
                    }
                    else if (i == 2)
                    {
                        Toast.MakeText(this.Activity, "����b4444444" + "", ToastLength.Short).Show();

                        return;
                    }

                }

            }




        }

        private float Spacing(MotionEvent mevent)
        {
            float x = mevent.GetX(0) - mevent.GetX(1);
            float y = mevent.GetY(0) - mevent.GetY(1);
            return FloatMath.Sqrt(x * x + y * y);
        }


        private void MidPoint(Android.Graphics.PointF point, MotionEvent mevent)
        {
            float x = mevent.GetX(0) + mevent.GetX(1);
            float y = mevent.GetY(0) + mevent.GetY(1);
            point.Set(x / 2, y / 2);
        }

        public void SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
        {

        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            Draw();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {

        }


        private void SetCenterXY()
        {
            // �������߶�
            //Android.Graphics.Rect frame = new Android.Graphics.Rect();
            //surview.GetWindowVisibleDisplayFrame(frame);
            //int statusBarHeight = frame.Top;
            //// �������߶�
            //int contentTop = this.Activity.Window.FindViewById(Window.IdAndroidContent).Top;
            ////Log.Debugd(TAG, "contentTop===�������߶�=====" + contentTop);
            //// statusBarHeight�����������״̬���ĸ߶�
            //int titleBarHeight = contentTop - statusBarHeight;
            //Log.d(TAG, "titleBarHeight==�����������״̬���ĸ߶� ======" + titleBarHeight);
            Android.Graphics.Rect frame = new Android.Graphics.Rect();
            surview.GetGlobalVisibleRect(frame);
            int svtop = frame.Top;
            float x = (this.Activity.WindowManager.DefaultDisplay.Width) / 2;
            //float y = (this.Activity.WindowManager.DefaultDisplay.Height  - statusBarHeight - titleBarHeight) / 2;
            float y = svtop / 2;
            /** ��Ļ���ĵ�λ������ */
            screenCenter.Set(x, y);
            /** ��ͼ���ĵ�λ������ */
            mapCenter.Set(x, y);
        }

        private void Draw()
        {
            canvas = this.surview.Holder.LockCanvas();
            /** ������ɫ */
            canvas.DrawColor(Color.White);
            Matrix matrix = new Matrix();

            canvas.DrawRect(0, 0, screenCenter.X * 2, screenCenter.Y * 2, new Paint());
            
            matrix.SetScale(rate, rate, bm.Width / 2, bm.Height / 2);
            /** ���ڻ�ͼλ�õ����⣬�Ҷ�Matrix����̫�˽⣬�����޷�����̫�࣬����Լ����� */
            matrix.PostTranslate(mapCenter.X + (end.X - start.X) - bm.Width / 2, mapCenter.Y + (end.Y - start.Y) - bm.Height / 2);
            /** ������ͼƬ */
            canvas.DrawBitmap(bm, matrix, paint);

            /** ��ע���� */
            if (pointData.Count == 0)
                return;
            /** ����ע�� */
            for (int i = 0; i < pointData.Count; i++)
            {
                matrix.SetScale(1.0f, 1.0f);
                b = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.compose_tag_dot);

                matrix.PostTranslate(mapCenter.X + (end.X - start.X) - b.Width / 2 - bm.Width * rate / 2 + 100 * rate + i * 50 * rate, mapCenter.Y + (end.Y - start.Y) - b.Height - bm
                        .Height * rate / 2 + 125 * rate + i * 50 * rate);
                canvas.DrawBitmap(b, matrix, paint);
            }

            surview.Holder.UnlockCanvasAndPost(canvas);

        }

    }

    enum MySurfaceViewMode
    {
        None,
        Zoom,
        Drag,
    }

}