using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace OwinSelfHostWebAPI
{
    public class MailsController : ApiController
    {
        private List<Common.EMail> _mails = new List<Common.EMail>();

        public MailsController()
        {
            var mailP = new MailProcessor.Processor();
            _mails = mailP.GetAllItems().ToList();
        }

        // GET api/mails
        public IEnumerable<Common.EMail> Get()
        {
            return _mails.Take(10); //.Select(i => new { Sender = i.From, When = i.Date });
            //return new string[] { "value1", "value2" };
        }

        [Route("api/mails/from/{sender}")]
        [HttpGet]
        public IEnumerable<Common.EMail> From(string sender)
        {
            return _mails.Where(m => m.From == sender).Take(10);
        }

        [Route("api/mails/{year}")]
        [HttpGet]
        public IEnumerable<Common.EMail> GetWithYear(int year)
        {
            if (year < 2010)
                return Enumerable.Empty<Common.EMail>();

            // Fri, 18 Jun 2010 09:06:12 +0300
            // Lets skip all casting etc. and do search with a string
            string yr = year.ToString();
            return _mails.Where(m => m.Date.Contains(yr)).Take(10);
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}