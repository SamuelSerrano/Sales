namespace Sales.ViewModels
{
	using System.Windows.Input;
	using GalaSoft.MvvmLight.Command;
	using Helpers;
	using Sales.Common.Models;
	using Services;
	using Xamarin.Forms;
	public class AddProductViewModel : BaseViewModel
    {
		#region Attributes
		private bool isRunning;
		private bool isEnabled;
		private ApiService apiService;
		#endregion

		#region Properties
		public string EntryDescription { get; set; }
		public string EntryPrice { get; set; }
		public string EntryRemarks { get; set; }
		public bool IsRunning {
			get { return this.isRunning; }
			set { this.SetValue(ref this.isRunning, value); }
		}

		public bool IsEnabled
		{
			get { return this.isEnabled; }
			set { this.SetValue(ref this.isEnabled, value); }
		}
		#endregion

		#region Constructors
		public AddProductViewModel()
		{
			this.apiService = new ApiService();
			this.IsEnabled = true;
		}
		#endregion

		#region Commands
		public ICommand SaveCommand {
			get {
				return new RelayCommand(Save);
			}
		}

		private async void Save()
		{
			if (string.IsNullOrEmpty(this.EntryDescription))
			{
				await Application.Current.MainPage.DisplayAlert(
					Languages.Error,
					Languages.DescriptionError,
					Languages.Accept);
			}

			if (string.IsNullOrEmpty(this.EntryPrice))
			{
				await Application.Current.MainPage.DisplayAlert(
					Languages.Error,
					Languages.PriceError,
					Languages.Accept);
			}

			var price = decimal.Parse(this.EntryPrice);
			if (price<0)
			{
				await Application.Current.MainPage.DisplayAlert(
					Languages.Error,
					Languages.PriceError,
					Languages.Accept);
			}

			this.IsRunning = true;
			this.IsEnabled = false;

			var checkConnection = await this.apiService.CheckConnection();
			if (!checkConnection.IsSuccess)
			{
				this.IsRunning = false;
				this.IsEnabled = true;
				await Application.Current.MainPage.DisplayAlert(Languages.Error, checkConnection.Message, Languages.Accept);
				return;
			}
			var product = new Product
			{
				Description = this.EntryDescription,
				Price = price,
				Remarks = this.EntryRemarks
			};
			var urlAPI = Application.Current.Resources["UrlAPI"].ToString();
			var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
			var urlProductController = Application.Current.Resources["UrlProductController"].ToString();
			var response = await apiService.Post(urlAPI, urlPrefix, urlProductController,product);

			if (!response.IsSuccess)
			{
				this.IsRunning = false;
				this.IsEnabled = true;
				await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
				return;
			}

			this.IsRunning = false;
			this.IsEnabled = true;
			await Application.Current.MainPage.Navigation.PopAsync();
		}
		#endregion
	}
}
