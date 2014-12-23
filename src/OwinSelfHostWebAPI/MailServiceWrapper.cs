using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

            Task.Factory.StartNew(UpdateMails);

            _timer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
            _timer.Elapsed += (s, e) => UpdateMails();
            _timer.Start();
        }

        public DateTime LastUpdate { get; private set; }

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

        public IEnumerable<Tuple<string, IEnumerable<Tuple<int, int>>>> GetBySenderByYears(bool addMissingValues = true)
        {
            var data = _proc.TotalMailsBySenderByYears(_fsMails).ToList();

            if (!addMissingValues)
                return data;

            var allYears = data
                .SelectMany(d => d.Item2.Select(t => t.Item1))
                .Distinct().OrderBy(x => x);

            // Fill missing years with value 0
            var newData = data.Select(d =>
                Tuple.Create(d.Item1, allYears.Select(y =>
                    Tuple.Create(y, d.Item2.Any(i => i.Item1 == y)
                                        ? d.Item2.Single(i => i.Item1 == y).Item2 : 0))));

            return newData;
        }

        public List<Tuple<string, IEnumerable<Tuple<string, int>>>> GetBySenderByMonths()
        {
            return _proc.TotalMailsBySenderByMonths(_fsMails).ToList();
        }

        public List<Tuple<int, IEnumerable<Tuple<DayOfWeek, int>>>> GetWeekdayTotal()
        {
            return _proc.MailsByWeekdays(_fsMails).ToList();
        }

        public List<Tuple<int, IEnumerable<Tuple<DayOfWeek, double>>>> GetWeekdayPercentage()
        {
            return _proc.MailsByWeekdaysPercent(_fsMails).ToList();
        }

        private void UpdateMails()
        {
            LastUpdate = DateTime.Now;

            var lastDate = _proc.LastMailDate(_fsMails);

            FSharpList<Common.EMail> newMails;

            try
            {
                newMails = MailReader.downloadMailsAfterDate(lastDate);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} - Exception: {1}", DateTime.Now, ex.Message);
                return;
            }

            var updatedCollection = MailReader.updateAndWriteAfterLastDate(_fsMails, lastDate);

            LastUpdate = DateTime.Now;

            Debug.WriteLine("{0} - New items: {1}", LastUpdate, updatedCollection.Length - _fsMails.Length);

            _fsMails = updatedCollection;

            lock (_mails)
                _mails = _fsMails.ToList();
        }
    }
}