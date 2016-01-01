using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TASDownloadService.Model;

namespace TASDownloadService.AttProcessDaily
{
    public static class PlaceTimeInOuts
    {
        #region -- Calculate Time In/Out --

        public static TimeSpan OpenShiftThresholdStart
        {
            get { return new TimeSpan(17, 00, 00); }
        }
        public static TimeSpan OpenShiftThresholdEnd
        {
            get { return new TimeSpan(11, 00, 00); }
        }

        public static void CalculateTimeINOUTOpenShift(AttData attendanceRecord, PollData up)
        {
            try
            {
                TAS2013Entities context = new TAS2013Entities();
                switch (up.RdrDuty)
                {
                    case 1: //IN
                        if (attendanceRecord.Tin0 == null)
                        {
                            if (up.EntTime.TimeOfDay < OpenShiftThresholdEnd)
                            {
                                DateTime dt = new DateTime();
                                dt = up.EntDate.Date.AddDays(-1);
                                var _attData = context.AttDatas.FirstOrDefault(aa => aa.AttDate == dt && aa.EmpID == up.EmpID);
                                if (_attData != null)
                                {

                                    if (_attData.TimeIn != null)
                                    {
                                        if (_attData.TimeIn.Value.TimeOfDay > OpenShiftThresholdStart)
                                        {
                                            //attdata - 1 . multipleTimeIn =  up.EntTime 

                                        }
                                        else
                                        {
                                            attendanceRecord.Tin0 = up.EntTime;
                                            attendanceRecord.TimeIn = up.EntTime;
                                            attendanceRecord.StatusAB = false;
                                            attendanceRecord.StatusP = true;
                                            attendanceRecord.Remarks = null;
                                            attendanceRecord.StatusIN = true;
                                        }
                                    }
                                    else
                                    {
                                        attendanceRecord.Tin0 = up.EntTime;
                                        attendanceRecord.TimeIn = up.EntTime;
                                        attendanceRecord.StatusAB = false;
                                        attendanceRecord.StatusP = true;
                                        attendanceRecord.Remarks = null;
                                        attendanceRecord.StatusIN = true;
                                    }
                                }
                                else
                                {
                                    attendanceRecord.Tin0 = up.EntTime;
                                    attendanceRecord.TimeIn = up.EntTime;
                                    attendanceRecord.StatusAB = false;
                                    attendanceRecord.StatusP = true;
                                    attendanceRecord.Remarks = null;
                                    attendanceRecord.StatusIN = true;
                                }
                            }
                            else
                            {
                                attendanceRecord.Tin0 = up.EntTime;
                                attendanceRecord.TimeIn = up.EntTime;
                                attendanceRecord.StatusAB = false;
                                attendanceRecord.StatusP = true;
                                attendanceRecord.Remarks = null;
                                attendanceRecord.StatusIN = true;
                            }

                        }
                        else if (attendanceRecord.Tin1 == null)
                        {
                            attendanceRecord.Tin1 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin2 == null)
                        {
                            attendanceRecord.Tin2 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin3 == null)
                        {
                            attendanceRecord.Tin3 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin4 == null)
                        {
                            attendanceRecord.Tin4 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin5 == null)
                        {
                            attendanceRecord.Tin5 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin6 == null)
                        {
                            attendanceRecord.Tin6 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin7 == null)
                        {
                            attendanceRecord.Tin7 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin8 == null)
                        {
                            attendanceRecord.Tin8 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin9 == null)
                        {
                            attendanceRecord.Tin9 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin10 == null)
                        {
                            attendanceRecord.Tin10 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else
                        {
                            attendanceRecord.Tin11 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        break;
                    case 5: //OUT
                        if (up.EntTime.TimeOfDay < OpenShiftThresholdEnd)
                        {
                            DateTime dt = up.EntDate.AddDays(-1);
                            if (context.AttDatas.Where(aa => aa.AttDate == dt && aa.EmpID == up.EmpID).Count() > 0)
                            {
                                AttData AttDataOfPreviousDay = context.AttDatas.FirstOrDefault(aa => aa.AttDate == dt && aa.EmpID == up.EmpID);
                                if (AttDataOfPreviousDay.TimeIn != null)
                                {
                                    if (AttDataOfPreviousDay.TimeIn.Value.TimeOfDay > OpenShiftThresholdStart)
                                    {
                                        //AttDate -1, Possible TimeOut = up.entryTime
                                        MarkOUTForOpenShift(up.EntTime, AttDataOfPreviousDay);
                                    }
                                    else
                                    {
                                        // Mark as out of that day
                                        MarkOUTForOpenShift(up.EntTime, attendanceRecord);
                                    }
                                }
                                else
                                    MarkOUTForOpenShift(up.EntTime, attendanceRecord);
                            }
                            else
                            {
                                // Mark as out of that day
                                MarkOUTForOpenShift(up.EntTime, attendanceRecord);
                            }


                        }
                        else
                        {
                            //Mark as out of that day
                            MarkOUTForOpenShift(up.EntTime, attendanceRecord);
                        }
                        //-------------------------------------------------------
                        context.SaveChanges();
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void MarkOUTForOpenShift(DateTime _pollTime, AttData _attendanceRecord)
        {
            if (_attendanceRecord.Tout0 == null)
            {
                _attendanceRecord.Tout0 = _pollTime;
                _attendanceRecord.TimeOut = _pollTime;
                SortingOutTime(_attendanceRecord);
            }
            else if (_attendanceRecord.Tout1 == null)
            {
                _attendanceRecord.Tout1 = _pollTime;
                _attendanceRecord.TimeOut = _pollTime;
                SortingOutTime(_attendanceRecord);
            }
            else if (_attendanceRecord.Tout2 == null)
            {
                _attendanceRecord.Tout2 = _pollTime;
                _attendanceRecord.TimeOut = _pollTime;
                SortingOutTime(_attendanceRecord);
            }
            else if (_attendanceRecord.Tout3 == null)
            {
                _attendanceRecord.Tout3 = _pollTime;
                _attendanceRecord.TimeOut = _pollTime;
                SortingOutTime(_attendanceRecord);
            }
            else
            {
                _attendanceRecord.Tout4 = _pollTime;
                _attendanceRecord.TimeOut = _pollTime;
                SortingOutTime(_attendanceRecord);
            }
            //else if (_attendanceRecord.Tout5 == null)
            //{
            //    _attendanceRecord.Tout5 = up.EntTime;
            //    _attendanceRecord.TimeOut = up.EntTime;
            //    SortingOutTime(_attendanceRecord);
            //}
            //else if (_attendanceRecord.Tout6 == null)
            //{
            //    _attendanceRecord.Tout6 = up.EntTime;
            //    _attendanceRecord.TimeOut = up.EntTime;
            //    SortingOutTime(_attendanceRecord);
            //}
            //else if (_attendanceRecord.Tout7 == null)
            //{
            //    _attendanceRecord.Tout7 = up.EntTime;
            //    _attendanceRecord.TimeOut = up.EntTime;
            //    SortingOutTime(_attendanceRecord);
            //}
            //else if (_attendanceRecord.Tout8 == null)
            //{
            //    _attendanceRecord.Tout8 = up.EntTime;
            //    _attendanceRecord.TimeOut = up.EntTime;
            //    SortingOutTime(_attendanceRecord);
            //}
            //else if (_attendanceRecord.Tout9 == null)
            //{
            //    _attendanceRecord.Tout9 = up.EntTime;
            //    _attendanceRecord.TimeOut = up.EntTime;
            //    SortingOutTime(_attendanceRecord);
            //}
            //else if (_attendanceRecord.Tout10 == null)
            //{
            //    _attendanceRecord.Tout10 = up.EntTime;
            //    _attendanceRecord.TimeOut = up.EntTime;
            //    SortingOutTime(_attendanceRecord);
            //}
            //else
            //{
            //    _attendanceRecord.Tout11 = up.EntTime;
            //    _attendanceRecord.TimeOut = up.EntTime;
            //    SortingOutTime(_attendanceRecord);
            //}
        }

        public static void CalculateTimeINOUT(AttData attendanceRecord, PollData up)
        {
            try
            {
                switch (up.RdrDuty)
                {
                    case 1: //IN
                        if (attendanceRecord.Tin0 == null)
                        {
                            attendanceRecord.Tin0 = up.EntTime;
                            attendanceRecord.TimeIn = up.EntTime;
                            attendanceRecord.StatusAB = false;
                            attendanceRecord.StatusP = true;
                            attendanceRecord.Remarks = null;
                        }
                        else if (attendanceRecord.Tin1 == null)
                        {
                            attendanceRecord.Tin1 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin2 == null)
                        {
                            attendanceRecord.Tin2 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin3 == null)
                        {
                            attendanceRecord.Tin3 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin4 == null)
                        {
                            attendanceRecord.Tin4 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin5 == null)
                        {
                            attendanceRecord.Tin5 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin6 == null)
                        {
                            attendanceRecord.Tin6 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin7 == null)
                        {
                            attendanceRecord.Tin7 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin8 == null)
                        {
                            attendanceRecord.Tin8 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin9 == null)
                        {
                            attendanceRecord.Tin9 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tin10 == null)
                        {
                            attendanceRecord.Tin10 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        else
                        {
                            attendanceRecord.Tin11 = up.EntTime;
                            SortingInTime(attendanceRecord);
                        }
                        break;
                    case 5: //OUT
                        if (attendanceRecord.Tout0 == null)
                        {
                            attendanceRecord.Tout0 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tout1 == null)
                        {
                            attendanceRecord.Tout1 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tout2 == null)
                        {
                            attendanceRecord.Tout2 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tout3 == null)
                        {
                            attendanceRecord.Tout3 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tout4 == null)
                        {
                            attendanceRecord.Tout4 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tout5 == null)
                        {
                            attendanceRecord.Tout5 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tout6 == null)
                        {
                            attendanceRecord.Tout6 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tout7 == null)
                        {
                            attendanceRecord.Tout7 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tout8 == null)
                        {
                            attendanceRecord.Tout8 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tout9 == null)
                        {
                            attendanceRecord.Tout9 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        else if (attendanceRecord.Tout10 == null)
                        {
                            attendanceRecord.Tout10 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        else
                        {
                            attendanceRecord.Tout11 = up.EntTime;
                            attendanceRecord.TimeOut = up.EntTime;
                            SortingOutTime(attendanceRecord);
                        }
                        break;
                    case 8: //DUTY
                        if (attendanceRecord.Tin0 != null)
                        {
                            if (attendanceRecord.Tout0 == null)
                            {
                                attendanceRecord.Tout0 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin1 == null)
                            {
                                attendanceRecord.Tin1 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout1 == null)
                            {
                                attendanceRecord.Tout1 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin2 == null)
                            {
                                attendanceRecord.Tin2 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout2 == null)
                            {
                                attendanceRecord.Tout2 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin3 == null)
                            {
                                attendanceRecord.Tin3 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout3 == null)
                            {
                                attendanceRecord.Tout3 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin4 == null)
                            {
                                attendanceRecord.Tin4 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout4 == null)
                            {
                                attendanceRecord.Tout4 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin5 == null)
                            {
                                attendanceRecord.Tin5 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout5 == null)
                            {
                                attendanceRecord.Tout5 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin6 == null)
                            {
                                attendanceRecord.Tin6 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout6 == null)
                            {
                                attendanceRecord.Tout6 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            //
                            else if (attendanceRecord.Tin7 == null)
                            {
                                attendanceRecord.Tin7 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout7 == null)
                            {
                                attendanceRecord.Tout7 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin8 == null)
                            {
                                attendanceRecord.Tin8 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout8 == null)
                            {
                                attendanceRecord.Tout8 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin9 == null)
                            {
                                attendanceRecord.Tin9 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout9 == null)
                            {
                                attendanceRecord.Tout9 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin10 == null)
                            {
                                attendanceRecord.Tin10 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout10 == null)
                            {
                                attendanceRecord.Tout10 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin11 == null)
                            {
                                attendanceRecord.Tin11 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout11 == null)
                            {
                                attendanceRecord.Tout11 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin12 == null)
                            {
                                attendanceRecord.Tin12 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tout12 == null)
                            {
                                attendanceRecord.Tout12 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else if (attendanceRecord.Tin13 == null)
                            {
                                attendanceRecord.Tin13 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                            else
                            {
                                attendanceRecord.Tout13 = up.EntTime;
                                attendanceRecord.TimeOut = up.EntTime;
                            }
                        }
                        else
                        {
                            attendanceRecord.Tin0 = up.EntTime;
                            attendanceRecord.TimeIn = up.EntTime;
                            attendanceRecord.StatusAB = false;
                            attendanceRecord.StatusP = true;
                            attendanceRecord.Remarks = null;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        // Sorting Time In
        public static void SortingInTime(AttData attendanceRecord)
        {
            List<DateTime> _InTimes = new List<DateTime>();

            if (attendanceRecord.Tin0 != null)
                _InTimes.Add((DateTime)attendanceRecord.Tin0);
            if (attendanceRecord.Tin1 != null)
                _InTimes.Add((DateTime)attendanceRecord.Tin1);
            if (attendanceRecord.Tin2 != null)
                _InTimes.Add((DateTime)attendanceRecord.Tin2);
            if (attendanceRecord.Tin3 != null)
                _InTimes.Add((DateTime)attendanceRecord.Tin3);
            if (attendanceRecord.Tin4 != null)
                _InTimes.Add((DateTime)attendanceRecord.Tin4);
            if (attendanceRecord.Tin5 != null)
                _InTimes.Add((DateTime)attendanceRecord.Tin5);
            if (attendanceRecord.Tin6 != null)
                _InTimes.Add((DateTime)attendanceRecord.Tin6);
            if (attendanceRecord.Tin7 != null)
                _InTimes.Add((DateTime)attendanceRecord.Tin7);
            if (attendanceRecord.Tin8 != null)
                _InTimes.Add((DateTime)attendanceRecord.Tin8);
            if (attendanceRecord.Tin9 != null)
                _InTimes.Add((DateTime)attendanceRecord.Tin9);
            if (attendanceRecord.Tin10 != null)
                _InTimes.Add((DateTime)attendanceRecord.Tin10);

            var list = _InTimes.OrderBy(x => x.TimeOfDay.Hours).ToList();
            PlacedSortedInTime(attendanceRecord, list);

        }

        public static void PlacedSortedInTime(AttData attendanceRecord, List<DateTime> _InTimes)
        {
            for (int i = 0; i < _InTimes.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        attendanceRecord.Tin0 = _InTimes[i];
                        attendanceRecord.TimeIn = _InTimes[i];
                        break;
                    case 1:
                        attendanceRecord.Tin1 = _InTimes[i];
                        break;
                    case 2:
                        attendanceRecord.Tin2 = _InTimes[i];
                        break;
                    case 3:
                        attendanceRecord.Tin3 = _InTimes[i];
                        break;
                    case 4:
                        attendanceRecord.Tin4 = _InTimes[i];
                        break;
                    case 5:
                        attendanceRecord.Tin5 = _InTimes[i];
                        break;
                    case 6:
                        attendanceRecord.Tin6 = _InTimes[i];
                        break;
                    case 7:
                        attendanceRecord.Tin7 = _InTimes[i];
                        break;
                    case 8:
                        attendanceRecord.Tin8 = _InTimes[i];
                        break;
                    case 9:
                        attendanceRecord.Tin9 = _InTimes[i];
                        break;
                }
            }
        }
        //Sorting Time Out
        public static void SortingOutTime(AttData attendanceRecord)
        {
            List<DateTime> _OutTimes = new List<DateTime>();

            if (attendanceRecord.Tout0 != null)
                _OutTimes.Add((DateTime)attendanceRecord.Tout0);
            if (attendanceRecord.Tout1 != null)
                _OutTimes.Add((DateTime)attendanceRecord.Tout1);
            if (attendanceRecord.Tout2 != null)
                _OutTimes.Add((DateTime)attendanceRecord.Tout2);
            if (attendanceRecord.Tout3 != null)
                _OutTimes.Add((DateTime)attendanceRecord.Tout3);
            if (attendanceRecord.Tout4 != null)
                _OutTimes.Add((DateTime)attendanceRecord.Tout4);
            if (attendanceRecord.Tout5 != null)
                _OutTimes.Add((DateTime)attendanceRecord.Tout5);
            if (attendanceRecord.Tout6 != null)
                _OutTimes.Add((DateTime)attendanceRecord.Tout6);
            if (attendanceRecord.Tout7 != null)
                _OutTimes.Add((DateTime)attendanceRecord.Tout7);
            if (attendanceRecord.Tout8 != null)
                _OutTimes.Add((DateTime)attendanceRecord.Tout8);
            if (attendanceRecord.Tout9 != null)
                _OutTimes.Add((DateTime)attendanceRecord.Tout9);
            if (attendanceRecord.Tout10 != null)
                _OutTimes.Add((DateTime)attendanceRecord.Tout10);

            var list = _OutTimes.OrderBy(x => x.TimeOfDay.Hours).ToList();
            PlacedSortedOutTime(attendanceRecord, list);


        }

        public static void PlacedSortedOutTime(AttData attendanceRecord, List<DateTime> _OutTimes)
        {
            for (int i = 0; i < _OutTimes.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        attendanceRecord.Tout0 = _OutTimes[i];
                        attendanceRecord.TimeOut = _OutTimes[i];
                        break;
                    case 1:
                        attendanceRecord.Tout1 = _OutTimes[i];
                        attendanceRecord.TimeOut = _OutTimes[i];
                        break;
                    case 2:
                        attendanceRecord.Tout2 = _OutTimes[i];
                        attendanceRecord.TimeOut = _OutTimes[i];
                        break;
                    case 3:
                        attendanceRecord.Tout3 = _OutTimes[i];
                        attendanceRecord.TimeOut = _OutTimes[i];
                        break;
                    case 4:
                        attendanceRecord.Tout4 = _OutTimes[i];
                        attendanceRecord.TimeOut = _OutTimes[i];
                        break;
                    case 5:
                        attendanceRecord.Tout5 = _OutTimes[i];
                        attendanceRecord.TimeOut = _OutTimes[i];
                        break;
                    case 6:
                        attendanceRecord.Tout6 = _OutTimes[i];
                        attendanceRecord.TimeOut = _OutTimes[i];
                        break;
                    case 7:
                        attendanceRecord.Tout7 = _OutTimes[i];
                        attendanceRecord.TimeOut = _OutTimes[i];
                        break;
                    case 8:
                        attendanceRecord.Tout8 = _OutTimes[i];
                        attendanceRecord.TimeOut = _OutTimes[i];
                        break;
                    case 9:
                        attendanceRecord.Tout9 = _OutTimes[i];
                        attendanceRecord.TimeOut = _OutTimes[i];
                        break;
                }
            }
        }
        #endregion
    }
}
