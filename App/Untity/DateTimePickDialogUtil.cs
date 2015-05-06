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
    public class DateTimePickDialogUtil : Java.Lang.Object, Android.Views.View.IOnClickListener, Android.Widget.DatePicker.IOnDateChangedListener, Android.Widget.TimePicker.IOnTimeChangedListener
    {
        private DatePicker datePicker;
        private TimePicker timePicker;
        private AlertDialog ad;
        private string dateTime;
        private DateTime initDateTime;
        private Activity activity;

        public DateTimePickDialogUtil(Activity activity, DateTime initDatetime)
        {
            this.activity = activity;
            this.initDateTime = initDatetime;
        }

        void init(DatePicker datePicker, TimePicker timePicker)
        {
            datePicker.Init(this.initDateTime.Year, this.initDateTime.Month, this.initDateTime.Day, this);
            timePicker.CurrentHour = new Java.Lang.Integer(this.initDateTime.Hour);
            timePicker.CurrentMinute = new Java.Lang.Integer(this.initDateTime.Minute);
        }

        public AlertDialog DateTimePicKDialog(EditText inputText)
        {

            LinearLayout dateTimeLayout = (LinearLayout)this.activity.LayoutInflater.Inflate(Resource.Layout.CommonDateTime, null);
            this.datePicker = dateTimeLayout.FindViewById<DatePicker>(Resource.Id.datepicker);
            this.timePicker = dateTimeLayout.FindViewById<TimePicker>(Resource.Id.timepicker);
            init(datePicker, timePicker);
            timePicker.SetIs24HourView(Java.Lang.Boolean.True);
            this.timePicker.SetOnTimeChangedListener(this);

            this.ad = new AlertDialog.Builder(this.activity)
               .SetTitle(inputText.Text)
               .SetView(dateTimeLayout)
               .SetPositiveButton("…Ë÷√", delegate
                {
                    inputText.Text = this.dateTime;
                })
               .SetNegativeButton("»°œ˚", delegate
               {
                   inputText.Text = "";
               })
           .Show();



            OnDateChanged(null, 0, 0, 0);
            return ad;

        }


        public void OnClick(View v)
        {

        }

        public void OnDateChanged(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            this.dateTime = new DateTime(this.datePicker.Year, this.datePicker.Month + 1, this.datePicker.DayOfMonth, this.timePicker.CurrentHour.IntValue(), this.timePicker.CurrentMinute.IntValue(), 0).ToString("yyyy-MM-dd HH:mm");

        }

        public void OnTimeChanged(TimePicker view, int hourOfDay, int minute)
        {
            OnDateChanged(null, 0, 0, 0);
        }
    }
}