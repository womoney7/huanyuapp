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
using Com.Baidu.Mapapi;
using Com.Baidu.Mapapi.Map;
using Android.Locations;
using Com.Baidu.Mapapi.Search.Geocode;
using Com.Baidu.Location;

namespace App
{
    [Activity(Label = "��������")]
    public class MapActivity : Activity, IBDLocationListener
    {
        MapView mapviw;
        LocationClient locaClient;
        bool isFirstLoc = true;// �Ƿ��״ζ�λ
        //Com.Baidu.Mapapi.Search.Geocode.GeoCoder mSearch = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SDKInitializer.Initialize(this.ApplicationContext);
            this.SetContentView(Resource.Layout.MapHome);
            // Create your application here
            mapviw = (MapView)this.FindViewById<MapView>(Resource.Id.bmapView);
            MapStatus mapstatus = new MapStatus.Builder()
                   .Target(new Com.Baidu.Mapapi.Model.LatLng(29.5446060000, 106.5306350000))
                   .Build();
            mapviw.Map.SetMapStatus(MapStatusUpdateFactory.NewMapStatus(mapstatus));
            mapviw.Map.MyLocationEnabled = true;
            locaClient = new LocationClient(this.ApplicationContext);
            locaClient.RegisterLocationListener(this);
            LocationClientOption option = new LocationClientOption();
            option.OpenGps = true;
            option.CoorType = "bd09ll";
            option.ScanSpan = 1000;
            
            option.SetIsNeedAddress(true);
            option.SetNeedDeviceDirect(true);
            option.SetLocationMode(LocationClientOption.LocationMode.HightAccuracy);
            locaClient.LocOption = option;
            locaClient.Start();


        }

        protected override void OnDestroy()
        {
            locaClient.Stop();
            mapviw.OnDestroy();
            base.OnDestroy();
           
            //mSearch.Destroy();
        }

        protected override void OnResume()
        {
            base.OnResume();
            mapviw.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            mapviw.OnResume();
        }

        public void OnReceiveLocation(BDLocation location)
        {
            // map view ���ٺ��ڴ����½��յ�λ��
            if (location == null || mapviw == null)
                return;
            MyLocationData locData = new MyLocationData.Builder().Accuracy(location.Radius)
                // �˴����ÿ����߻�ȡ���ķ�����Ϣ��˳ʱ��0-360
                    .Direction(100)
                    .Latitude(location.Latitude)
                    .Longitude(location.Longitude).Build();
            this.mapviw.Map.SetMyLocationData(locData);
            if (isFirstLoc)
            {
                isFirstLoc = false;
                Com.Baidu.Mapapi.Model.LatLng ll = new Com.Baidu.Mapapi.Model.LatLng(location.Latitude,
                        location.Longitude);
                MapStatusUpdate u = MapStatusUpdateFactory.NewLatLng(ll);
                this.mapviw.Map.AnimateMapStatus(u);
            }
        }
    }

}