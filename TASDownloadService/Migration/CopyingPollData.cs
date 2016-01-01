using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMSFFService;
using OldDatabase.OldModel;
using TASDownloadService.Model;

namespace WMSFFService
{
    public class CopyingPollData
    {
        TAS_FF2000Entities oldDB = new TAS_FF2000Entities();
        TAS2013Entities newDB = new TAS2013Entities();

        public void CopyingPolls()
        {
            int totalcount = 0;
            int convertedCount = 0;
            int notConvertedCount = 0;
            List<OldDatabase.OldModel.PollData> _OldPolls = new List<OldDatabase.OldModel.PollData>();
            DateTime _dt = new DateTime();
            DateTime _st1 = new DateTime();
            _dt = GlobalSettings._dateTime;
            _st1 = GlobalSettings._dateTime.AddDays(1);
            oldDB.CommandTimeout = 0;
            //_OldPolls = oldDB.PollDatas.Where(aa => aa.EntryDate >= _dt && aa.EmpNo == "000180" ).ToList();
            /////////Second Time and Ownwards
            _OldPolls = oldDB.PollDatas.Where(p => p.Version == -1 && p.EntryDate >= _dt.Date).ToList();
            /////////First Time
            //_OldPolls = oldDB.PollDatas.Where(aa => aa.EntryDate >= _dt && aa.EntryDate <= _st1).ToList();
            totalcount = _OldPolls.Count;
            MyCustomFunctions _myHelperClass = new MyCustomFunctions();
            foreach (var pollsToBeConverted in _OldPolls)
            {
                try
                {
                    if (pollsToBeConverted.EmpNo != "<ERR>")
                    {
                        TASDownloadService.Model.PollData convertedPolls = new TASDownloadService.Model.PollData();
                        //convertedPolls.PollID = Convert.ToInt64(pollsToBeConverted.TranNo);
                        convertedPolls.CardNo = pollsToBeConverted.CardNo;
                        convertedPolls.EmpID = Convert.ToInt16(pollsToBeConverted.EmpNo);
                        convertedPolls.RdrDuty = Convert.ToByte(pollsToBeConverted.DutyCode);
                        convertedPolls.EntDate = pollsToBeConverted.EntryDate.Value;
                        convertedPolls.EntTime = pollsToBeConverted.EntryDate.Value + GlobalSettings.ConvertTime(pollsToBeConverted.EntryTime);
                        convertedPolls.RdrID = (short)pollsToBeConverted.ReaderID;
                        convertedPolls.EmpDate = Convert.ToInt64(pollsToBeConverted.EmpDate).ToString();
                        convertedPolls.Process = false;
                        newDB.PollDatas.AddObject(convertedPolls);
                        if (newDB.SaveChanges() > 0)
                        {
                            convertedCount++;

                            pollsToBeConverted.Version = 1;
                            oldDB.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    notConvertedCount++;
                    string _error = "";
                    if (ex.InnerException.Message != null)
                        _error = ex.InnerException.Message;
                    else
                        _error = ex.Message;
                    _myHelperClass.WriteToLogFile("Exception at CopyingPoll Function: Total/Converted " + totalcount.ToString() + "/" + convertedCount.ToString() + _error);
                }
            }
            _myHelperClass.WriteToLogFile("Copying Poll Completed: Total/Converted " + totalcount.ToString() + "/" + convertedCount.ToString());
        }

    }
}
