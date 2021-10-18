using Android.App;
using Android.Content;
using System.Threading.Tasks;
using Android.OS;
using System.Threading;
using Xamarin.Forms;
using FormsBackgrounding.Messages;
using FormsBackgrounding.Services;
using System;

namespace FormsBackgrounding.Droid
{
	[Service]
	public class LongRunningTaskService : Service
	{
		CancellationTokenSource _cts;

		public override IBinder OnBind (Intent intent)
		{
			return null;
		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			_cts = new CancellationTokenSource ();

			Task.Run (async() => {
				try {
					//INVOKE THE SHARED CODE
					//var counter = new TaskCounter();
					//counter.RunCounter(_cts.Token).Wait();
					await Task.Delay(3000);
					var networkService = new NetworkService();
					for (var i = 0; i < 100; i++)
					{
						networkService.counter++;
						var result = await networkService.GetInfoFromAPI(_cts.Token);

						Console.WriteLine(result);
						await Task.Delay(2000);
					}
				}
				catch (Exception e) {
					Console.WriteLine(e.Message);
				}
				finally {
					if (_cts.IsCancellationRequested) {
						var message = new CancelledMessage();
						Device.BeginInvokeOnMainThread (
							() => MessagingCenter.Send(message, "CancelledMessage")
						);
					}
				}

			}, _cts.Token);

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy ()
		{
			if (_cts != null) {
				_cts.Token.ThrowIfCancellationRequested ();

				_cts.Cancel ();
			}
			base.OnDestroy ();
		}
	}
}