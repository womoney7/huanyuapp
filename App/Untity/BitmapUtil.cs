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

namespace App.Untity
{
    public class BitmapUtil
    {
        public static byte[] GetBitmapByte(Android.Graphics.Bitmap bitmap)
        {
            byte[] data = null;
            using (System.IO.MemoryStream memstream = new System.IO.MemoryStream())
            {
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, memstream);
                if (memstream.CanRead)
                {
                    data = new byte[memstream.Length];
                    memstream.Read(data, 0, data.Length);
                }
            }

            return data;

        }
        public static Android.Graphics.Bitmap GetBitmapFromByte(byte[] temp)
        {
            if (temp != null)
            {
                Android.Graphics.Bitmap bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(temp, 0, temp.Length);
                return bitmap;
            }
            else
            {
                return null;
            }
        }
    }
}