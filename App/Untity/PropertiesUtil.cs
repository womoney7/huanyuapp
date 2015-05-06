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
using Java.Util;

namespace App.Untity
{
    public class PropertiesUtil
    {

        private static PropertiesUtil instance;
        public static PropertiesUtil Instance
        {
            get
            {
                if (instance == null)
                    instance = new PropertiesUtil();
                return instance;
            }
        }

        private Properties properties;

        private PropertiesUtil()
        {
            properties = new Properties();
            properties.Load(Application.Context.Assets.Open("property.properties"));
        }


        public string GetProperty(string key)
        {
            return properties.GetProperty(key);
        }
    }
}