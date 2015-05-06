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

namespace App.Adapter
{
    public class VehiclePictureAdapter : BaseAdapter
    {
        private LayoutInflater mInflater;
        private Context mContext;
        private IList<VehiclePictureItem> imgList;
        public VehiclePictureAdapter(Context context, IList<VehiclePictureItem> imageList)
        {
            this.mContext = context;
            this.mInflater = LayoutInflater.From(this.mContext);
            this.imgList = imageList;
        }

        public void Refresh(IList<VehiclePictureItem> imageList)
        {
            this.imgList = imageList;
            this.NotifyDataSetChanged();
        }

        public override int Count
        {
            get { return imgList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return imgList[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = this.imgList[position];
            Android.Graphics.Bitmap bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(item.PicData, 0, item.PicData.Length);
            if (convertView == null)
            {
                convertView = this.mInflater.Inflate(Resource.Layout.VehiclePictureItem, null);
                item.ImageViewItem  = convertView.FindViewById<ImageView>(Resource.Id.id_item_image);
            }
            item.ImageViewItem.SetImageBitmap(bitmap);

            return convertView;

        }
    }
}