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

namespace App.Entity
{
    public class PostionPoint
    {
        public float PointX { get; set; }

        public float PointY { get; set; }

        public int DrawableId { get; set; }

        public int MarkTempId { get; set; }

        public PostionPoint()
        {

        }

        public PostionPoint(float pointX, float pointY)
        {
            this.PointX = pointX;
            this.PointY = PointY;
        }


    }
}