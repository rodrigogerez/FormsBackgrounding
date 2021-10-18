using System.Net.Http;
using System.Threading.Tasks;
using FormsBackgrounding.Messages;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace FormsBackgrounding.Services
{
    public class NetworkService
    {
        static readonly HttpClient client = new HttpClient();
        public int counter = 0;

        public async Task<string> GetInfoFromAPI()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.publicapis.org/entries");
            var result = await client.SendAsync(request);
            if (result.IsSuccessStatusCode)
            {
                var message = new TickedMessage
                {
                    Message = counter.ToString()
                };

                Device.BeginInvokeOnMainThread(() => {
                    MessagingCenter.Send<TickedMessage>(message, "TickedMessage");
                });
                return result.Content.ToString();
            }
            return null;
        }
    }
}
