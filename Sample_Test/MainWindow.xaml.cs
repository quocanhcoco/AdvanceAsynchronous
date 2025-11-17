using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sample_Test
{
	public sealed partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();
		}

		private async void TestAsyncFlow_Click(object sender, RoutedEventArgs e)
		{
			AsyncFlow asyncFlow = new AsyncFlow();
			var length = await asyncFlow.GetUrlContentLengthAsync();
			Debug.WriteLine($"length = {length}");
		}
	}

	public class AsyncFlow
	{
		public async Task<int> GetUrlContentLengthAsync()
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
	}
}
