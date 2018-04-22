using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARTC_PilotBot.Dialogs
{
    public class transformResponse
    {
        public static string processScrapValue(status1Object JSON, string calculation, DateTime duration)
        {
            int count = JSON.value.Count();
            List<double> readings = new List<double>();

            //generate the value needed
            for(int i = 0; i < count; i++)
            {
                if(JSON.value[i].Timestamp.AddHours(8).Date.Equals(duration.Date))
                {
                    readings.Add(Convert.ToDouble(JSON.value[i].Value));
                }
            }
            double total = 0;
            if(calculation == "total")
            {
                total = readings.Sum();
            }
            return $"Total value is {total.ToString()}.";
        }

        public static string processTotalAndGoodParts(status1Object JSON, DateTime duration, string indicator)
        {
            int count = JSON.value.Count();
            List<double> readings = new List<double>();
            for (int i = 0; i < count; i++)
            {
                if (JSON.value[i].Timestamp.AddHours(8).Date.Equals(duration.Date))
                {
                    readings.Add(Convert.ToDouble(JSON.value[i].Value));
                }
            }
            double total = readings.Sum();
            return $"{indicator} for {duration.ToString("dd-MM-yyyy")} is {total.ToString()}";
        }

        public static string processWorkingTime(status1Object JSON, DateTime duration, string station)
        {
            int count = JSON.value.Count();
            List<double> readings = new List<double>();
            for (int i = 0; i < count; i++)
            {
                if (JSON.value[i].Timestamp.AddHours(8).Date.Equals(duration.Date))
                {
                    readings.Add(Convert.ToDouble(JSON.value[i].Value));
                }
            }
            double total = readings.Sum();
            return $"Operators are spending {total.ToString()} time at {station}";
        }

        public static string processDefect(status1Object JSON, string station)
        {
            int count = JSON.value.Count();
            List<string> readings = new List<string>();
            List<DateTime> dates = new List<DateTime>();

            for(int i = 0; i < count; i++)
            {
                readings.Add(JSON.value[i].Value);
                dates.Add(JSON.value[i].Timestamp);
            }

            int latest = dates.IndexOf(dates.Max());

            return $"Latest defect for {station} is {readings[latest]}.";
        }
    }
}