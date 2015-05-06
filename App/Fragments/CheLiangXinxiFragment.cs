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
using Android.Provider;

namespace App.Fragments
{
    public class CheLiangXinxiFragment : Android.Support.V4.App.Fragment
    {
        EditText jiluTimeEdit;
        Button btnviewpic;
        Button btnpaizhao;

        IList<Entity.VehiclePictureItem> picList;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string tabname = this.Arguments.GetString("TabName");
            View v = inflater.Inflate(Resource.Layout.CheliangXinxiFragment, container, false);
            jiluTimeEdit = v.FindViewById<EditText>(Resource.Id.cheliangxinxi_jiulushijian);
            btnviewpic = v.FindViewById<Button>(Resource.Id.btnviewpic);
            btnpaizhao = v.FindViewById<Button>(Resource.Id.btnpaizhao);
            //v.FindViewById<TextView>(Resource.Id.fragment_text).Text = tabname;
            return v;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            jiluTimeEdit.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            jiluTimeEdit.Click += (s, e) =>
            {
                App.Untity.DateTimePickDialogUtil dateTimePicKDialog = new App.Untity.DateTimePickDialogUtil(this.Activity, DateTime.Now);
                dateTimePicKDialog.DateTimePicKDialog(jiluTimeEdit);
            };

            btnviewpic.Click += (s, e) =>
            {
                Intent intent = new Intent(this.Activity, typeof(VehiclePictureActivity));
                this.StartActivityForResult(intent, 1);
            };

            btnpaizhao.Click += (s, e) =>
            {
                //拍照我们用Action为MediaStore.ACTION_IMAGE_CAPTURE，
                //有些人使用其他的Action但我发现在有些机子中会出问题，所以优先选择这个
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                intent.SetAction(MediaStore.ActionImageCapture);
                this.StartActivityForResult(intent, 2);
            };

        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == 2)
            {
                if (resultCode == (int)Result.Ok)
                {
                    Intent intent = new Intent(this.Activity, typeof(VehiclePictureActivity));
                    this.StartActivityForResult(intent, 1);
                }

            }
            else if (requestCode == 1)
            {
                picList = data.GetParcelableArrayListExtra("PicData").Cast<Entity.VehiclePictureItem>().ToList();
                if (picList != null)
                {

                }
            }
        }
    }
}