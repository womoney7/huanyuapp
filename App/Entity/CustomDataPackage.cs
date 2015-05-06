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
using fastBinaryJSON;

namespace App.Entity
{
    public class CustomDataPackage
    {
        public string Id { get; set; }//Ψһ��ʶ
        public string Command { get; set; }//��������
        public bool IsCompressed { get; set; }//�Ƿ�ѹ��
        public bool IsEncrypted { get; set; } //�Ƿ����
        public object Value { get; set; } //��������
        public string ValueType { get; set; }//��������
        public string ErrorMessage { get; set; } //������Ϣ


        public CustomDataPackage()
        {

        }

        public CustomDataPackage(object parameter)
        {
            this.Value = parameter;
        }


        public byte[] ToBytes()
        {
            var data = fastBinaryJSON.BJSON.Instance.ToJSON(this, true, true);
            data = QuickLZ.Compress(data, 1);
            return data;
        }

        public static CustomDataPackage ToDataPackage(byte[] data)
        {
            data = QuickLZ.Decompress(data);
            return BJSON.Instance.ToObject<CustomDataPackage>(data);
        }

        public static object ToObject(byte[] data)
        {
            return ToDataPackage(data).Value;
        }

        public static T ToObject<T>(byte[] data)
        {
            return (T)ToObject(data);
        }

    }
}