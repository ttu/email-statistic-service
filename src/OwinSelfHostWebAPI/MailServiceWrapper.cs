using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace OwinSelfHostWebAPI
{
    public interface IMailService
    {
        List<Common.EMail> GetMails();

        List<Tuple<string, int>> GetYearlyStats(int year);

        List<Tuple<int, int>> GetYearlyStats();

        List<int> GetYears();
    }

    public class MailServiceWrapper : IMailService
    {
        private MailProcessor.Processor _proc;
        private FSharpList<Common.EMail> _fsMails;
        private List<Common.EMail> _mails;

        private Timer _timer = new Timer();

        public MailServiceWrapper()
        {
            _proc = new MailProcessor.Processor();
            _fsMails = _proc.GetAllItems();
            _mails = _fsMails.ToList();

            _timer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
            _timer.Elapsed += (s, e) => UpdateMails();
            _timer.Start();
        }

        public List<Common.EMail> GetMails()
        {
            lock (_mails)
                return _mails;
        }

        public List<int> GetYears()
        {
            return _proc.YearsThatHaveData(_fsMails).ToList();
        }

        public List<Tuple<int, int>> GetYearlyStats()
        {
            return _proc.TotalMailsByYear(_fsMails).ToList();
        }

        public List<Tuple<string, int>> GetYearlyStats(int year)
        {
            return _proc.TotalMailsBySenderByYear(_fsMails, year).ToList();
        }

        private void UpdateMails()
        {
            var lastDate = _proc.LastMailDate(_fsMails);

            var newMails = MailReader.downloadMailsAfterDate(lastDate);

            var updatedCollection = MailReader.updateAndWriteAfterLastDate(_fsMails, lastDate);

            Debug.WriteLine("{0} - New items: {1}", DateTime.Now, updatedCollection.Length - _fsMails.Length);

            _fsMails = updatedCollection;

            lock (_mails)
                _mails = _fsMails.ToList();
        }
    }
}