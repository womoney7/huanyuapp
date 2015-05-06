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
using Java.Interop;

namespace App.Entity
{
    public class VehiclePictureItem : Java.Lang.Object, Android.OS.IParcelable
    {
        public string Name { get; set; }
        public string PicPath { get; set; }
        public byte[] PicData { get; set; }

        public ImageView ImageViewItem { get; set; }


        public VehiclePictureItem()
        {

        }

        // Create a new VehiclePictureItem populated with the values in parcel
        private VehiclePictureItem(Parcel parcel)
        {
            Name = parcel.ReadString();
            PicPath = parcel.ReadString();
            parcel.ReadByteArray(PicData);
        }

        // The creator creates an instance of the specified object
        private static readonly GenericParcelableCreator<VehiclePictureItem> _creator = new GenericParcelableCreator<VehiclePictureItem>(parcel => new VehiclePictureItem(parcel));
        [ExportField("CREATOR")]
        public static GenericParcelableCreator<VehiclePictureItem> GetCreator()
        {
            return _creator;
        }


        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            dest.WriteString(this.Name);
            dest.WriteString(this.PicPath);
            dest.WriteByteArray(this.PicData);

        }
    }


    /// <summary>
    /// Generic Parcelable creator that can be used to create objects from parcels
    /// </summary>
    public sealed class GenericParcelableCreator<T> : Java.Lang.Object, IParcelableCreator
        where T : Java.Lang.Object, new()
    {
        private readonly Func<Parcel, T> _createFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParcelableDemo.GenericParcelableCreator`1"/> class.
        /// </summary>
        /// <param name='createFromParcelFunc'>
        /// Func that creates an instance of T, populated with the values from the parcel parameter
        /// </param>
        public GenericParcelableCreator(Func<Parcel, T> createFromParcelFunc)
        {
            _createFunc = createFromParcelFunc;
        }

        #region IParcelableCreator Implementation

        public Java.Lang.Object CreateFromParcel(Parcel source)
        {
            return _createFunc(source);
        }

        public Java.Lang.Object[] NewArray(int size)
        {
            return new T[size];
        }

        #endregion
    }
}