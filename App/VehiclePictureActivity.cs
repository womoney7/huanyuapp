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
using Android.Provider;
using App.Untity;
using Android.Graphics;

namespace App
{
    [Activity(Label = "��������")]
    public class VehiclePictureActivity : Activity
    {
        GridView pictureGridView;
        TextView btnPickPhoto;
        TextView btnTakePhoto;
        TextView totalpic;
        VehiclePictureAdapter picAdapter;
        IList<Entity.VehiclePictureItem> picList = new List<Entity.VehiclePictureItem>();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.SetContentView(Resource.Layout.VehiclePicture);
            pictureGridView = this.FindViewById<GridView>(Resource.Id.id_gridView);
            btnPickPhoto = this.FindViewById<TextView>(Resource.Id.btn_pick_photo);
            btnTakePhoto = this.FindViewById<TextView>(Resource.Id.btn_take_photo);
            totalpic = this.FindViewById<TextView>(Resource.Id.totalpic);
            picAdapter = new VehiclePictureAdapter(this.ApplicationContext, picList);
            pictureGridView.Adapter = picAdapter;
            btnPickPhoto.Click += (s, e) =>
            {
                //ѡ����Ƭ��ʱ��Ҳһ����������ActionΪIntent.ACTION_GET_CONTENT��
                //��Щ��ʹ��������Action���ҷ�������Щ�����л�����⣬��������ѡ�����
                Intent intent = new Intent();
                intent.SetType("image/*");
                intent.SetAction(Intent.ActionGetContent);
                this.StartActivityForResult(intent, 2);
            };
            btnTakePhoto.Click += (s, e) =>
            {
                //����������ActionΪMediaStore.ACTION_IMAGE_CAPTURE��
                //��Щ��ʹ��������Action���ҷ�������Щ�����л�����⣬��������ѡ�����
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                intent.SetAction(MediaStore.ActionImageCapture);
                this.StartActivityForResult(intent, 1);
            };

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode != Result.Ok)
            {
                return;
            }

            if (data.Extras != null)
            {
                Bitmap bitmap = data.Extras.GetParcelable("data").JavaCast<Bitmap>();
                if (bitmap != null)
                {
                    Entity.VehiclePictureItem picitem = new Entity.VehiclePictureItem();
                    picitem.PicData = BitmapUtil.GetBitmapByte(bitmap);
                    picList.Add(picitem);
                    picAdapter.Refresh(picList);

                    Intent intback = new Intent();
                    intback.PutParcelableArrayListExtra("PicData", picList.ToArray());
                    this.SetResult(Result.Ok, intback);
                }
            }
            if (data.Data != null)
            {
                //ȡ�÷��ص�Uri,������ѡ����Ƭ��ʱ�򷵻ص�����Uri��ʽ���������������еû�����Uri�ǿյģ�����Ҫ�ر�ע��
                var bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, data.Data);
                Entity.VehiclePictureItem picitem = new Entity.VehiclePictureItem();
                picitem.PicData = BitmapUtil.GetBitmapByte(bitmap);
                picitem.PicPath = data.Data.Path;
                picList.Add(picitem);
                picAdapter.Refresh(picList);

                Intent intback = new Intent();
                intback.PutParcelableArrayListExtra("PicData", picList.ToArray());
                this.SetResult(Result.Ok, intback);
            }
        }

    }
}