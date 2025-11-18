using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Sample_Test
{
	public sealed partial class MainWindow : Window
	{
		static readonly CancellationTokenSource s_cts = new CancellationTokenSource();

		public MainWindow()
		{
			this.InitializeComponent();

			//TestInitDeadlock();
		}

		#region Recap

		#region Test_1a_Async_and_Sync
		private void Test_1a_Async_and_Sync_Click(object sender, RoutedEventArgs e)
		{
			DoWorkSync();    // dong bo

			//_ = DoWorkAsync();   // bat dong bo
		}

		private void DoWorkSync()
		{
			Debug.WriteLine("Start");

			Thread.Sleep(2000);
			Debug.WriteLine("Work 1 done");

			Thread.Sleep(2000);
			Debug.WriteLine("Work 2 done");

			Debug.WriteLine("End");
		}

		private async Task DoWorkAsync()
		{
			Debug.WriteLine("Start");

			await Task.Delay(2000);
			Debug.WriteLine("Work 1 done");

			await Task.Delay(2000);
			Debug.WriteLine("Work 2 done");

			Debug.WriteLine("End");
		}
		#endregion

		#region Test_1b_Deadlock
		//private void TestInitDeadlock()
		//{
		//	string result = GetDataAsync().Result;
		//	Debug.WriteLine(result);
		//}

		private void Test_1b_Deadlock_Click(object sender, RoutedEventArgs e)
		{
			string result = GetDataAsync().Result;
			Debug.WriteLine(result);
		}

		private async Task<string> GetDataAsync()
		{
			await Task.Delay(2000);
			return "Hello";
		}
		#endregion

		#region Test_1d_Cancel_Task_Click
		private async void Test_1d_Cancel_Task_Click(object sender, RoutedEventArgs e)
		{
			// Khoi chya Task va truyen token
			var task = DoWorkDownloadAsync(s_cts.Token);

			// Huy Task sau 3s
			_ = Task.Run(() =>
			{
				Thread.Sleep(3000);
				Debug.WriteLine("Requesting cancellation...");
				s_cts.Cancel();
			});

			try
			{
				//s_cts.CancelAfter(3500);	// set time out

				await task;
				Debug.WriteLine("Task completed successfully.");
			}
			catch (OperationCanceledException)
			{
				Debug.WriteLine("Task was canceled!");
			}
			finally
			{
				s_cts.Dispose();
			}
		}

		private static async Task DoWorkDownloadAsync(CancellationToken token)
		{
			for (int i = 0; i < 10; i++)
			{
				token.ThrowIfCancellationRequested(); // Throw exception neu s_cts.Cancel   ||   co the dung CancellationToken.IsCancellationRequested de kiem tra
				Debug.WriteLine($"Working {i}...");
				await Task.Delay(1000); // gia lap cong viec download du lieu
			}
		}
		#endregion

		#region Test_1e_Async_Flow
		private async void Test_1e_Async_Flow_Click(object sender, RoutedEventArgs e)
		{
			var length = await GetUrlContentLengthAsync();
			Debug.WriteLine($"length = {length}");
		}

		private async Task<int> GetUrlContentLengthAsync()
		{
			var client = new HttpClient();

			Task<string> getStringTask = client.GetStringAsync("https://docs.microsoft.com/dotnet");

			DoIndependentWork();

			string contents = await getStringTask;
			return contents.Length;
		}

		private void DoIndependentWork()
		{
			Debug.WriteLine("Working...");
		}
		#endregion

		#endregion

		#region StateMachine



		#endregion
	}
}
