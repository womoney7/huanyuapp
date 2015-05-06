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
using App.Entity;
using Android.Graphics.Drawables;

namespace App.CustomView
{
    public class CImageMarkView : FrameLayout
    {

        //private static int POINTNUM = 5;
        private Context mContext;
        //��עͼƬ�����ؿ��
        private static int MARKIMAGEWIDTH = 0;
        //��עͼƬ��dp���
        private static int MARKIMAGEWIDTHDP = 10;
        //��ע���ֵ����ؿ��
        private static int MARKTEXTWIDTH = 0;
        //��ע���ֵ�dp���
        private static int MARKTEXTWIDTHDP = 150;
        //��ע���ֵ����ظ߶�
        private static int MARKTEXTHIGHE = 0;
        //��ע���ֵ�dp�߶�
        private static int MARKTEXTHIGHEDP = 25;
        private static int MARGINBETWEENIMAGEANDTEXT = 20;
        //����ͼƬ�Ŀ��
        private int Width = 0;
        //��ע�б�
        private List<PostionPoint> addPointList = null;
        //��ע��ʱid����
        private int tempMarkId = 0;
        //��ʱ�������洢Ҫɾ���ı�ע����
        private PostionPoint tempDelIp = null;
        //��ʾ��ע�ı�ʶ
        private bool isAddMark = false;

        private Activity mActivity = null;

        //����Handler����  
        Handler handler = null;
        public CImageMarkView(Context context, Android.Util.IAttributeSet attrs)
            : base(context, attrs)
        {

            mContext = context;
            handler = new Handler(new Action<Message>(msg =>
            {
                switch (msg.Arg1)
                {
                    case 1:
                        RemovePointForView();
                        break;
                }
            }));
            InitView();
        }
        public CImageMarkView(Context context)
            : base(context)
        {

            mContext = context;
            handler = new Handler(new Action<Message>(msg =>
            {
                switch (msg.Arg1)
                {
                    case 1:
                        RemovePointForView();
                        break;
                }
            }));
            InitView();
        }
        private void InitView()
        {
            addPointList = new List<PostionPoint>();
            tempMarkId = 0;
            MARKIMAGEWIDTH = Dip2Px(mContext, MARKIMAGEWIDTHDP);
            MARKTEXTWIDTH = Dip2Px(mContext, MARKTEXTWIDTHDP);
            MARKTEXTHIGHE = Dip2Px(mContext, MARKTEXTHIGHEDP);
        }


        /// <summary>
        /// ��ʼ���ؼ�
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="bitmap"></param>
        /// <param name="isAddView">isAddView true Ϊ��ע����ҳ�棬false Ϊ��ʾҳ��</param>
        /// <param name="pointList"></param>
        public void Init(Activity activity, Android.Graphics.Bitmap bitmap, bool isAddView, List<PostionPoint> pointList)
        {

            LayoutParams lparams = (LayoutParams)this.LayoutParameters;
            if (activity.WindowManager != null)
            {
                Display display = activity.WindowManager.DefaultDisplay;
                Width = display.Width;
                lparams.Width = Width;
                lparams.Height = Width;
                //����Ļ��ȵ�һ��С�����õ�������󳤶ȣ����޸�������󳤶�
                if ((Width / 2) < MARKTEXTWIDTH)
                {
                    MARKTEXTWIDTH = Width / 2 - MARKIMAGEWIDTH - MARGINBETWEENIMAGEANDTEXT;
                }
            }
            this.mActivity = activity;
            isAddMark = isAddView;
            this.LayoutParameters = lparams;
            //		this.setBackgroundColor( Color.rgb(255, 182, 193));
            Android.Graphics.Bitmap bgbitmap = Android.Graphics.BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.car);
            //// ���ͼƬ�Ŀ��
            //int width = bgbitmap.Width;
            //int height = bgbitmap.Height;
            //// �������ű���
            ////float scaleWidth = ((float)this.mActivity.WindowManager.DefaultDisplay.Width) / width;
            ////float scaleHeight = ((float)height) / height;
            //float scaleWidth = 0.75f;
            //float scaleHeight = 0.75f;
            //// ȡ����Ҫ���ŵ�matrix����
            //Android.Graphics.Matrix matrix = new Android.Graphics.Matrix();
            //matrix.PostScale(scaleWidth, scaleHeight);
            //// �õ��µ�ͼƬ
            //Android.Graphics.Bitmap newbm = Android.Graphics.Bitmap.CreateBitmap(bgbitmap, 0, 0, width, height, matrix, true);
            //BitmapDrawable bitdra = new BitmapDrawable(newbm);
            this.RemoveAllViews();
            ImageView bgView = new ImageView(this.mContext);
            bgView.LayoutParameters = new ViewGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
            bgView.SetScaleType(ImageView.ScaleType.CenterInside);
            bgView.SetImageBitmap(bgbitmap);
            this.AddView(bgView);
            //this.SetBackgroundResource(Resource.Drawable.car);
            if (!isAddMark && pointList != null)
            {
                this.addPointList = pointList;
                ShowPointList();
            }
        }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);
        }


        public override bool OnTouchEvent(MotionEvent e)
        {
            int[] location = new int[2];
            this.GetLocationInWindow(location);
            //		Log.e("getLocationInWindow", location[0]+"-"+location[1]);
            //		this.getLocationOnScreen(location);
            if (isAddMark)
            {//��ӱ�ע
                int moveX = (int)e.GetX();
                int moveY = (int)e.GetY();
                switch (e.Action)
                {
                    case MotionEventActions.Down:

                        //if(addPointList.Count==POINTNUM){
                        //    Toast.MakeText(mContext, "��ע���ֻ����5��", ToastLength.Long).Show();

                        //    break;
                        //}
                        PostionPoint ip = new PostionPoint();
                        //				Log.e("��ǰλ��% x-y", (float)moveX/Width+"-"+(float)moveY/Width);
                        ip.PointX = (float)moveX / Width;
                        ip.PointY = (float)moveY / Width;
                        ip.MarkTempId = tempMarkId++;
                        int a = new Random().Next(100);

                        addPointList.Add(ip);
                        ShowPointList();

                        //ThisApplication app = (ThisApplication)this.mActivity.getApplication();
                        //app.setPointList(addPointList);
                        break;
                }
            }
            else
            {

            }
            return base.OnTouchEvent(e);
        }


        /// <summary>
        ///  ɾ����ע���¼�
        /// </summary>
        private abstract class MarkViewClickListener : Java.Lang.Object, View.IOnClickListener
        {

            private PostionPoint ip;
            public MarkViewClickListener(PostionPoint ip)
            {
                this.ip = ip;
            }


            public void OnClick(View v)
            {
                onClick(v, ip);

            }
            public abstract void onClick(View v, PostionPoint ip);


        }

        /// <summary>
        /// ����dialog��ʾɾ����ע
        /// </summary>
        private void ShowDelTipDialog()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(mContext);
            builder.SetMessage("ɾ����ע��").SetCancelable(true)
                    .SetNegativeButton("ȡ��", (ds, de) =>
                    {
                        isDialogShow = false;
                        (ds as IDialogInterface).Cancel();

                    })
                    .SetPositiveButton("ȷ��", (ds, de) =>
                    {
                        isDialogShow = false;
                        Message msg = handler.ObtainMessage();
                        msg.Arg1 = 1;
                        handler.SendMessage(msg);
                        (ds as IDialogInterface).Cancel();
                    });
            builder.Show();
        }

        /// <summary>
        /// �Ƴ���ע����
        /// </summary>
        private void RemovePointForView()
        {
            for (int i = 0; i < addPointList.Count; i++)
            {
                PostionPoint temp = addPointList[i];
                if (temp.MarkTempId == tempDelIp.MarkTempId)
                {
                    addPointList.RemoveAt(i);
                }
            }
            ShowPointList();

        }


        /// <summary>
        /// ��ע�����¼� 
        /// </summary>
        private abstract class MoveViewTouchListener : Java.Lang.Object, View.IOnTouchListener
        {

            private Button btn;
            private PostionPoint ip;
            private TextView tv;
            public MoveViewTouchListener(TextView tv, Button btn, PostionPoint ip)
            {
                this.btn = btn;
                this.ip = ip;
                this.tv = tv;
            }

            public bool OnTouch(View v, MotionEvent e)
            {
                OnTouchListener(v, tv, btn, ip, e);
                return false;//�������أ�����onclick�¼��޷���Ӧ
            }

            public abstract void OnTouchListener(View v, TextView tv, Button btn, PostionPoint ip, MotionEvent me);

        }



        /// <summary>
        /// Ϊ�ı�����ӻ����¼�
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="btn"></param>
        /// <param name="ip"></param>
        private void SetTouchListenerToText(TextView txt, Button btn, PostionPoint ip)
        {



            //     txt.setOnTouchListener(new MoveViewTouchListener(txt,btn,ip) {
            //            int lastX, lastY;

            //            @Override
            //            public void onTouchListener(View v, TextView tv, Button btn,
            //                    ImagePoint ip, MotionEvent event) {
            //                int ea = event.getAction();
            //                switch (ea) {
            //                case MotionEvent.ACTION_DOWN:
            //                    //TODO������break�������޷����У�ԭ��δ֪
            //                    // ��ȡ�����¼�����λ�õ�ԭʼX����
            //                    lastX = (int) event.getRawX();
            //                    lastY = (int) event.getRawY();
            ////					break;
            //                case MotionEvent.ACTION_MOVE:
            //                    // event.getRawX();����ƶ���λ��
            //                    int dx = (int) event.getRawX() - lastX;
            //                    int dy = (int) event.getRawY() - lastY;
            ////					Log.e("����-λ����x-y", dx+"-"+dy);
            ////					Log.e("����-��ע��ʼλ��", btn.getLeft()+"-"+btn.getBottom()+"-"+btn.getRight()+"-"+btn.getTop());
            //                    int l = btn.getLeft() + dx;
            //                    int b = btn.getBottom() + dy;
            //                    int r = btn.getRight() + dx;
            //                    int t = btn.getTop() + dy;

            //                    // �����ж��ƶ��Ƿ񳬳���Ļ
            //                    if (l < 0) {
            //                        l = 0;
            //                        r = l + btn.getWidth();
            //                    }
            //                    if (t < 0) {
            //                        t = 0;
            //                        b = t + btn.getHeight();
            //                    }
            //                    if (r > Width) {
            //                        r = Width;
            //                        l = r - btn.getWidth();
            //                    }
            //                    if (b > Width) {
            //                        b = Width;
            //                        t = b - btn.getHeight();
            //                    }

            ////					Log.e("��ǰλ��% x-y", (float)moveX/Width+"-"+(float)moveY/Width);
            //                    ip.setX((float)l/Width);
            //                    ip.setY((float)t/Width);
            //                    for(int i=0;i<addPointList.size();i++){
            //                        ImagePoint temp = addPointList.get(i);
            //                        if(temp.getMarkTempId() == ip.getMarkTempId()){
            //                            addPointList.remove(i);
            //                        }
            //                    }
            //                    addPointList.add(ip);
            //                    showPointList();
            //                    break;
            //                }

            //            }
            //        });
        }

        private bool isDialogShow = false;

        /// <summary>
        /// 
        /// </summary>
        private void ShowPointList()
        {
            foreach (var ip in addPointList)
            {
                Button btn = new Button(mContext);
                btn.SetBackgroundResource(Resource.Drawable.compose_tag_dot);

                //			Log.e("��ǰλ�� x-y", ip.getX()+"-"+ip.getY());
                //			Log.e("ͼƬ����Ļ�ĸ߶�", y+"");
                //			Log.e("ͼƬ���", Width+"");
                //			Log.e("��עͼƬ���", MARKIMAGEWIDTH+"");
                int moveX = (int)(Width * ip.PointX);
                int moveY = (int)(Width * ip.PointY);
                FrameLayout.LayoutParams layParams = new FrameLayout.LayoutParams(MARKIMAGEWIDTH, MARKIMAGEWIDTH);
                layParams.Gravity = GravityFlags.Left | GravityFlags.Top;
                layParams.LeftMargin = (moveX + MARKIMAGEWIDTH) > Width ? Width - MARKIMAGEWIDTH : moveX;
                layParams.TopMargin = (moveY + MARKIMAGEWIDTH) > Width ? Width - MARKIMAGEWIDTH : moveY;
                this.AddView(btn, layParams);

                //TextView txt = new TextView(mContext);
                //txt.SetText(ip.getMarkStr());
                //txt.SetTextColor(Android.Graphics.Color.Rgb(255, 255, 255));
                //txt.TextSize = 13;
                //txt.Gravity = GravityFlags.Center;
                //txt.SetSingleLine(true);
                //txt.SetMaxWidth(MARKTEXTWIDTH);
                //txt.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
                //FrameLayout.LayoutParams layParamsTxt = new FrameLayout.LayoutParams(LayoutParams.WrapContent, MARKTEXTHIGHE);

                //if (Math.Abs(Width - moveX) > moveX)
                //{//�ұ�����Ƚϴ�
                //    layParamsTxt.Gravity = GravityFlags.Left | GravityFlags.Top;
                //    layParamsTxt.LeftMargin = (moveX + MARKIMAGEWIDTH + MARGINBETWEENIMAGEANDTEXT);
                //    //�������ֱ���ͼ
                //    if (Math.Abs(Width - moveY) > moveY)
                //    {//�ײ������
                //        txt.SetBackgroundResource(R.drawable.compose_tag_bg_1);
                //    }
                //    else
                //    {
                //        txt.SetBackgroundResource(R.drawable.compose_tag_bg_2);
                //    }
                //}
                //else
                //{
                //    layParamsTxt.Gravity = GravityFlags.Right | GravityFlags.Top;
                //    layParamsTxt.RightMargin = ((Width - moveX) + MARGINBETWEENIMAGEANDTEXT);
                //    //�������ֱ���ͼ
                //    if (Math.Abs(Width - moveY) > moveY)
                //    {//�ײ������
                //        txt.SetBackgroundResource(R.drawable.compose_tag_bg_2);
                //    }
                //    else
                //    {
                //        txt.SetBackgroundResource(R.drawable.compose_tag_bg_1);
                //    }
                //}
                //if (Math.Abs(Width - moveY) > moveY)
                //{//�ײ������
                //    layParamsTxt.TopMargin = (moveY + MARKIMAGEWIDTH);
                //}
                //else
                //{
                //    layParamsTxt.TopMargin = (moveY - MARKTEXTHIGHE);
                //}

                //this.AddView(txt, layParamsTxt);
                if (!isAddMark)
                {//����������������¼�

                                    //btn.setOnClickListener(new MarkViewClickListener(ip){
                                    //    @Override
                                    //    public void onClick(View v,ImagePoint ip) {
                                    //        DataUtil.showShortToast(mContext, ip.getMarkStr());
                                    //        Toast.makeText(mContext, ip.getMarkStr(), Toast.LENGTH_SHORT).show();
                                    //    }
                                    // });

                    //                txt.setOnClickListener(new MarkViewClickListener(ip){
                    //                    @Override
                    //                    public void onClick(View v,ImagePoint ip) {
                    //                        Toast.makeText(mContext, ip.getMarkStr(), Toast.LENGTH_SHORT).show();
                    ////						DataUtil.showShortToast(mContext, ip.getMarkStr());
                    //                    }
                    //                 });  
                }
                else
                {//��������������¼�ɾ����ע
                    //btn.setOnClickListener(new MarkViewClickListener(ip){
                    //    @Override
                    //    public void onClick(View v,ImagePoint ip) {
                    //        if(!isDialogShow){
                    //            tempDelIp = ip;
                    //            showDelTipDialog();
                    //            isDialogShow = true;
                    //        }
                    //    }
                    // });

                    //txt.setOnClickListener(new MarkViewClickListener(ip){
                    //    @Override
                    //    public void onClick(View v, ImagePoint ip) {
                    //        if(!isDialogShow){
                    //            tempDelIp = ip;
                    //            showDelTipDialog();
                    //            isDialogShow = true;
                    //        }
                    //    }
                    // }); 
                    //SetTouchListenerToText(txt, btn, ip);
                }
            }

        }

        /// <summary>
        /// �����ֻ��ķֱ��ʴ� dp �ĵ�λ ת��Ϊ px(����) 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dpValue"></param>
        /// <returns></returns>
        public static int Dip2Px(Context context, float dpValue)
        {
            float scale = context.Resources.DisplayMetrics.Density;
            return (int)(dpValue * scale + 0.5f);
        }
    }
}