using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace OwinSelfHostWebAPI
{
    public class MailsController : ApiController
    {
        private MailServiceWrapper _service;

        public MailsController()
        {
            _service = Startup.MailService.Value; 
        }

        [Route("api/mails/updatetime/"), HttpGet]
        public DateTime Updated()
        {
            return _service.LastUpdate;
        }

        [Route("api/mails/from/{sender}")]
        [HttpGet]
        public IEnumerable<Common.EMail> From(string sender)
        {
            return _service.GetMails().Where(m => m.From == sender).Take(10);
        }

        [Route("api/mails/year/{year}")]
        [HttpGet]
        public IEnumerable<Tuple<string,int>> GetWithYear(int year)
        {
            return _service.GetYearlyStats(year);
        }

        [Route("api/mails/yearly/all")]
        [HttpGet]
        public IEnumerable<Tuple<int,int>> GetYearly()
        {
            return _service.GetYearlyStats();
        }

        [Route("api/mails/yearly/allinone")]
        [HttpGet]
        public IEnumerable<int> GetYearlyInOne()
        {
            return _service.GetYearlyStats().Select(t => t.Item2).ToList();
        }

        [Route("api/mails/users/years")]
        [HttpGet]
        //[Authorize]
        public IEnumerable<Tuple<string, IEnumerable<Tuple<int, int>>>> GetBySenderByYears()
        {
            return _service.GetBySenderByYears();
        }

        [Route("api/mails/weekdays/percentage")]
        [HttpGet]
        public IEnumerable<Tuple<int, IEnumerable<Tuple<DayOfWeek, double>>>> GetWeekdayPercentage()
        {
            return _service.GetWeekdayPercentage();
        }

        [Route("api/mails/weekdays/total")]
        [HttpGet]
        public IEnumerable<Tuple<int, IEnumerable<Tuple<DayOfWeek, int>>>> GetWeekdayTotal()
        {
            return _service.GetWeekdayTotal();
        }
    }
}