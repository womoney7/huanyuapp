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
using Android.Util;
using Android.Graphics;

namespace App.CustomView
{
    public class AutoTextView : TextSwitcher, ViewSwitcher.IViewFactory
    {
        private float mHeight;
        private Context mContext;
        //mInUp,mOutUp分别构成向下翻页的进出动画
        private Rotate3dAnimation mInUp;
        private Rotate3dAnimation mOutUp;

        //mInDown,mOutDown分别构成向下翻页的进出动画
        private Rotate3dAnimation mInDown;
        private Rotate3dAnimation mOutDown;

        public AutoTextView(Context context)
            : this(context, null)
        {

        }

        public AutoTextView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Android.Content.Res.TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.auto3d);
            mHeight = a.GetDimension(Resource.Styleable.auto3d_textSize, 24);

            a.Recycle();
            mContext = context;
            Init();
        }

        private void Init()
        {
            // TODO Auto-generated method stub
            SetFactory(this);
            mInUp = CreateAnim(this, -90, 0, true, true);
            mOutUp = CreateAnim(this, 0, 90, false, true);
            mInDown = CreateAnim(this, 90, 0, true, false);
            mOutDown = CreateAnim(this, 0, -90, false, false);
            //TextSwitcher主要用于文件切换，比如 从文字A 切换到 文字 B，
            //setInAnimation()后，A将执行inAnimation，
            //setOutAnimation()后，B将执行OutAnimation
            InAnimation = mInUp;
            OutAnimation = mOutUp;
        }

        private Rotate3dAnimation CreateAnim(AutoTextView textView, float start, float end, bool turnIn, bool turnUp)
        {
            Rotate3dAnimation rotation = new Rotate3dAnimation(textView, start, end, turnIn, turnUp);
            rotation.Duration = 800;
            rotation.FillAfter = false;
            rotation.Interpolator = new Android.Views.Animations.AccelerateInterpolator();
            return rotation;
        }

        /// <summary>
        /// 这里返回的TextView，就是我们看到的View
        /// </summary>
        /// <returns></returns>
        public View MakeView()
        {
            TextView t = new TextView(mContext);
            t.LayoutParameters = new Android.Widget.FrameLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
            t.SetTextColor(Color.White);
            t.Gravity = GravityFlags.Center;
            t.TextSize = mHeight;
            t.SetMaxLines(2);
            return t;
        }

        //定义动作，向下滚动翻页
        public void Previous()
        {

            if (InAnimation != mInDown)
            {
                InAnimation = mInDown;

            }
            if (OutAnimation != mOutDown)
            {
                OutAnimation = mOutDown;
            }
        }
        //定义动作，向上滚动翻页
        public void Next()
        {
            if (InAnimation != mInUp)
            {
                InAnimation = mInUp;
            }
            if (OutAnimation != mOutUp)
            {
                OutAnimation = mOutUp;
            }
        }


    }
    public class Rotate3dAnimation : Android.Views.Animations.Animation
    {
        private float mFromDegrees;
        private float mToDegrees;
        private float mCenterX;
        private float mCenterY;
        private bool mTurnIn;
        private bool mTurnUp;
        private Android.Graphics.Camera mCamera;
        AutoTextView mTextView;

        public Rotate3dAnimation(AutoTextView textView, float fromDegrees, float toDegrees, bool turnIn, bool turnUp)
        {
            mTextView = textView;
            mFromDegrees = fromDegrees;
            mToDegrees = toDegrees;
            mTurnIn = turnIn;
            mTurnUp = turnUp;
        }

        public override void Initialize(int width, int height, int parentWidth, int parentHeight)
        {
            base.Initialize(width, height, parentWidth, parentHeight);
            mCamera = new Android.Graphics.Camera();
            mCenterY = this.mTextView.Height / 2;
            mCenterX = this.mTextView.Width / 2;
        }

        protected override void ApplyTransformation(float interpolatedTime, Android.Views.Animations.Transformation t)
        {
            float fromDegrees = mFromDegrees;
            float degrees = fromDegrees + ((mToDegrees - fromDegrees) * interpolatedTime);

            float centerX = mCenterX;
            float centerY = mCenterY;
            Camera camera = mCamera;
            int derection = mTurnUp ? 1 : -1;

            Matrix matrix = t.Matrix;

            camera.Save();
            if (mTurnIn)
            {
                camera.Translate(0.0f, derection * mCenterY * (interpolatedTime - 1.0f), 0.0f);
            }
            else
            {
                camera.Translate(0.0f, derection * mCenterY * (interpolatedTime), 0.0f);
            }
            camera.RotateX(degrees);
            camera.GetMatrix(matrix);
            camera.Restore();

            matrix.PreTranslate(-centerX, -centerY);
            matrix.PostTranslate(centerX, centerY);
        }
    }
}