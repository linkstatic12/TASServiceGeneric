using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSFFService
{
    //public class CopyingRoster
    //{
    //    TAS_FF2000Entities oldDB = new TAS_FF2000Entities();
    //    TAS2013Entities newDB = new TAS2013Entities();
    //    public void CopyRoster()
    //    {
            
    //        int totalcount = 0;
    //        int convertedCount = 0;
    //        int notConvertedCount = 0;
    //        DateTime dt = new DateTime(2015,02,01);
    //        //dt = GlobalSettings._dateTime;
    //        List<OldDatabase.OldModel.Roster> _OldRoster = new List<OldDatabase.OldModel.Roster>();
    //        ////////First Time
    //        //_OldRoster = oldDB.Rosters.Where(aa => aa.DutyDate >= dt).ToList();
    //        ////////Second Time
    //        _OldRoster = oldDB.Rosters.Where(aa => aa.DutyDate >= dt).ToList();
    //        //_OldRoster = oldDB.Rosters.Where(aa => aa.DutyDate >= dt && aa.EmpNo == "000180").ToList();
            
    //        totalcount = _OldRoster.Count;
    //        MyCustomFunctions _myHelperClass = new MyCustomFunctions();

    //        foreach (var rosterToBeConverted in _OldRoster)
    //        {
    //            try
    //            {
    //                if (rosterToBeConverted.EmpNo != "<ERR>")
    //                {
    //                    NewDatabase.Roster convertedRoster = new NewDatabase.Roster();
    //                    convertedRoster.EmpID = Convert.ToInt32(rosterToBeConverted.EmpNo);
    //                    convertedRoster.RosterDate = rosterToBeConverted.DutyDate.Value;
    //                    convertedRoster.EmpDate = Convert.ToInt64(rosterToBeConverted.EmpDate).ToString();
    //                    if (rosterToBeConverted.DutyCode == "DUTY")
    //                        convertedRoster.DutyCode = "D";
    //                    else if (rosterToBeConverted.DutyCode == "R")
    //                        convertedRoster.DutyCode = "R";
    //                    if (rosterToBeConverted.DutyTime == "R")
    //                        convertedRoster.DutyTime = new TimeSpan(07,45,0);
    //                    else
    //                        convertedRoster.DutyTime = GlobalSettings.ConvertTime(rosterToBeConverted.DutyTime);
    //                    convertedRoster.Remarks = rosterToBeConverted.Remarks;
    //                    convertedRoster.WorkMin = (short)rosterToBeConverted.WrkMin;
    //                    newDB.Rosters.AddObject(convertedRoster);
    //                    if (newDB.SaveChanges() > 0)
    //                    {
    //                        convertedCount++;
    //                        rosterToBeConverted.Version = 1;
    //                        oldDB.SaveChanges();
    //                    }
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                string _error = "";
    //                if (ex.InnerException.Message != null)
    //                    _error = ex.InnerException.Message;
    //                else
    //                    _error = ex.Message;
    //                _myHelperClass.WriteToLogFile("Exception at CopyingPoll Function: Total/Converted " + totalcount.ToString() + "/" + convertedCount.ToString() + _error);
    //            }

    //        }
    //        _myHelperClass.WriteToLogFile("Copying Roster Completed: Total/Converted " + totalcount.ToString() + "/" + convertedCount.ToString());
    //    }
    //}
}
