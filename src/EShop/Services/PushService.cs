using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;

namespace EShop.Services
{
    public interface IPushService
    {
        bool SendAll(string message);
        bool SendTo(IEnumerable<int> to, string message);
    }

    public class PushService : IPushService
    {
        private readonly DataContext _context;
		private readonly string _url = "http://push.dbogatov.org/api/push/send";

        public PushService(DataContext context)
        {
            _context = context;
        }

        public bool SendAll(string message)
        {
            return this.SendTo(_context.PushPairs.Select(pp => pp.UserId), message);
        }

        public bool SendTo(IEnumerable<int> to, string message)
        {
            var tokens =
                from pushPair in _context.PushPairs
                join userId in to on pushPair.UserId equals userId
                select pushPair.DeviceToken;

            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
				{
					{ "appname", "shevastream" },
					{ "message", message },
					{ "tokens", String.Join(",", tokens) },
					{ "production", "false" }
				};

                
                var content = new FormUrlEncodedContent(values);
                
				//Console.WriteLine($"{message} : {String.Join(",", tokens)}");
				
                var response = client.PostAsync(_url, content);
            }
			
			return true;
        }
    }


}
