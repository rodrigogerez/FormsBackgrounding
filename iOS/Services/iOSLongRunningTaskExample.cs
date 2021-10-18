using System;
using System.Threading;
using System.Threading.Tasks;
using FormsBackgrounding.Messages;
using FormsBackgrounding.Services;
using UIKit;
using Xamarin.Forms;

namespace FormsBackgrounding.iOS
{
	public class iOSLongRunningTaskExample
	{
		nint _taskId;
		CancellationTokenSource _cts;

		public async Task Start ()
		{
			Console.WriteLine("TASK STARTED ON iOS.");
			_cts = new CancellationTokenSource ();

			_taskId = UIApplication.SharedApplication.BeginBackgroundTask ("LongRunningTask", OnExpiration);

			try {
				//INVOKE THE SHARED CODE
				//var counter = new TaskCounter();
				//await counter.RunCounter(_cts.Token);
				await Task.Delay(5000);
				var networkService = new NetworkService();
				for(var i = 0; i<100; i++)
                {
					networkService.counter++;
					var result = await networkService.GetInfoFromAPI(_cts.Token);

					Console.WriteLine(result);
					await Task.Delay(2000);
				}
			} catch (OperationCanceledException) {
			} finally {
				if (_cts.IsCancellationRequested) {
					var message = new CancelledMessage();
					Console.WriteLine("TASK CANCELLED ON iOS.");
					Device.BeginInvokeOnMainThread (
						() => MessagingCenter.Send(message, "CancelledMessage")
					);
				}
			}

			UIApplication.SharedApplication.EndBackgroundTask (_taskId);
		}

		public void Stop ()
		{
			_cts.Cancel ();
		}

		void OnExpiration ()
		{
			_cts.Cancel ();
		}
	}
}