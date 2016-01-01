using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSFFService
{
    //public class CopyingLeaves
    //{
    //    //Copy Leave Applications

    //    //Copy Short Leaves
    //    TAS_FF2000Entities oldDB = new TAS_FF2000Entities();
    //    TAS2013Entities newDB = new TAS2013Entities();
    //    MyCustomFunctions _myHelperClass = new MyCustomFunctions();
    //    public void copyLeaveApplications()
    //    {
    //        int totalcount = 0;
    //        int convertedCount = 0;
    //        int notConvertedCount = 0;
    //        DateTime _dt = GlobalSettings._dateTime;
    //        List<OldDatabase.OldModel.LvAppl> _OldLeaves = new List<OldDatabase.OldModel.LvAppl>();
    //        //_OldLeaves = oldDB.LvAppls.Where(aa=>aa.EmpNo == "000180").ToList();
    //        /////////Second Time and Ownwards
    //        //_OldLeaves = oldDB.LvAppls.Where(aa => aa.Version == 0).ToList();
    //        _OldLeaves = oldDB.LvAppls.Where(aa=>aa.LvID > 5530).ToList();
    //        /////First Time
    //        //_OldLeaves = oldDB.LvAppls.ToList();
    //        totalcount = _OldLeaves.Count;
    //        foreach (var leavesToBeConverted in _OldLeaves)
    //        {
    //            try
    //            {
    //                NewDatabase.LvApplication convertedLeave = new NewDatabase.LvApplication();
    //                convertedLeave.LvID = leavesToBeConverted.LvID;
    //                convertedLeave.LvDate = (DateTime)leavesToBeConverted.LvDate;
    //                if (leavesToBeConverted.LvType == "N")
    //                    convertedLeave.LvType = "B";
    //                else if (leavesToBeConverted.LvType == "S")
    //                    convertedLeave.LvType = "C";
    //                else if (leavesToBeConverted.LvType == "C")
    //                    convertedLeave.LvType = "A";
    //                else
    //                    convertedLeave.LvType = "A";
    //                convertedLeave.EmpID = Convert.ToInt32(leavesToBeConverted.EmpNo);
    //                convertedLeave.FromDate = (DateTime)leavesToBeConverted.FromDate;
    //                convertedLeave.ToDate = (DateTime)leavesToBeConverted.ToDate;
    //                convertedLeave.NoOfDays = (float)leavesToBeConverted.NoDays;

    //                if (leavesToBeConverted.HalfAbsent == 1)
    //                    convertedLeave.HalfAbsent = true;
    //                else
    //                    convertedLeave.HalfAbsent = false;

    //                if (leavesToBeConverted.HalfLeave == 1)
    //                    convertedLeave.IsHalf = true;
    //                else
    //                    convertedLeave.IsHalf = false;

    //                convertedLeave.LvReason = leavesToBeConverted.LvReason;
    //                convertedLeave.CreatedBy = 3;
    //                convertedLeave.LvStatus = "P";
    //                newDB.LvApplications.AddObject(convertedLeave);
    //                if (newDB.SaveChanges() > 0)
    //                {
    //                    convertedCount++;
    //                }
    //                else
    //                    notConvertedCount++;
    //            }
    //            catch (Exception ex)
    //            {
    //                notConvertedCount++;
    //                string _error = "";
    //                if (ex.InnerException.Message != null)
    //                    _error = ex.InnerException.Message;
    //                else
    //                    _error = ex.Message;
    //                _myHelperClass.WriteToLogFile("Exception at Copying Leaves Function: Total/Converted " + totalcount.ToString() + "/" + convertedCount.ToString() + _error);
    //            }
    //        }
    //        _myHelperClass.WriteToLogFile("Copying LeavesApplication Completed: Total/Converted " + totalcount.ToString() + "/" + convertedCount.ToString());

    //    }

    //    //Copy LeaveData
    //    public void CopyLeavesData()
    //    {
    //        int totalcount = 0;
    //        int convertedCount = 0;
    //        int notConvertedCount = 0;
    //        List<OldDatabase.OldModel.LvData> _OldLeaves = new List<OldDatabase.OldModel.LvData>();
    //        DateTime _st = GlobalSettings._dateTime;
    //        //_OldLeaves = oldDB.LvDatas.Where(aa => aa.DutyDate >= _st && aa.Version != 1).ToList();
    //        _OldLeaves = oldDB.LvDatas.Where(aa => aa.LvID > 5530).ToList();
    //        /////////First Time
    //        //_OldLeaves = oldDB.LvDatas.ToList();
    //        totalcount = _OldLeaves.Count;
    //        foreach (var leavesToBeConverted in _OldLeaves)
    //        {
    //            try
    //            {
    //                NewDatabase.LvData convertedLeave = new NewDatabase.LvData();
    //                convertedLeave.EmpID = Convert.ToInt32(leavesToBeConverted.EmpNo);
    //                convertedLeave.LvID = leavesToBeConverted.LvID;
    //                convertedLeave.Remarks = leavesToBeConverted.Remarks;
    //                convertedLeave.EmpDate = Convert.ToInt64(leavesToBeConverted.EmpDate).ToString();
    //                convertedLeave.AttDate = (DateTime)leavesToBeConverted.DutyDate;
    //                convertedLeave.LvCode = leavesToBeConverted.DutyCode;
    //                if (leavesToBeConverted.DutyCode == "N")
    //                    convertedLeave.LvCode = "B";
    //                else if (leavesToBeConverted.DutyCode == "S")
    //                    convertedLeave.LvCode = "C";
    //                else if (leavesToBeConverted.DutyCode == "C")
    //                    convertedLeave.LvCode = "A";
    //                else
    //                    convertedLeave.LvCode = "A";
    //                convertedLeave.LvStatus = 1;
    //                newDB.LvDatas.AddObject(convertedLeave);
    //                if (newDB.SaveChanges() > 0)
    //                {
    //                    convertedCount++;
    //                    leavesToBeConverted.Version = 1;
    //                    oldDB.SaveChanges();
    //                }
    //                else
    //                    notConvertedCount++;
    //            }
    //            catch (Exception ex)
    //            {
    //                notConvertedCount++;
    //                string _error = "";
    //                if (ex.InnerException.Message != null)
    //                    _error = ex.InnerException.Message;
    //                else
    //                    _error = ex.Message;
    //                _myHelperClass.WriteToLogFile("Exception at CopyingPoll Function: Total/Converted " + totalcount.ToString() + "/" + convertedCount.ToString() + _error);
    //            }

    //        }
    //        _myHelperClass.WriteToLogFile("Copying Leave Data Completed: Total/Converted " + totalcount.ToString() + "/" + convertedCount.ToString());
    //    }

    //    List<WMSFFService.NewDatabase.LvApplication> _NewLvApp = new List<NewDatabase.LvApplication>();
    //    //public void MakeLvData()
    //    //{
    //    //    _NewLvApp = newDB.LvApplications.Where(aa => aa.LvID > 3661).ToList();
    //    //    foreach (var lvapp in _NewLvApp)
    //    //    {
    //    //        AddLeaveToLeaveData(lvapp);
    //    //    }
    //    //}
    //    //private void AddLeaveToLeaveData(LvApplication lvappl)
    //    //{
    //    //    try
    //    //    {
    //    //        DateTime datetime = new DateTime();
    //    //        datetime = lvappl.FromDate;
    //    //        for (int i = 0; i < lvappl.NoOfDays; i++)
    //    //        {
    //    //            string _EmpDate = lvappl.EmpID + datetime.Date.ToString("yymmdd"); ;
    //    //            WMSFFService.NewDatabase.LvData _LVData = new WMSFFService.NewDatabase.LvData();
    //    //            _LVData.EmpID = lvappl.EmpID;
    //    //            _LVData.EmpDate = _EmpDate;
    //    //            _LVData.Remarks = lvappl.LvReason;
    //    //            _LVData.LvID = lvappl.LvID;
    //    //            _LVData.AttDate = datetime.Date;
    //    //            _LVData.LvCode = lvappl.LvType;
    //    //            newDB.LvDatas.AddObject(_LVData);
    //    //            datetime = datetime.AddDays(1);
    //    //            newDB.SaveChanges();
    //    //        }
    //    //    }
    //    //    catch (Exception ex)
    //    //    {

    //    //    }

    //    //}
    //}
}
