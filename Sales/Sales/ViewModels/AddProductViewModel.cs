namespace Sales.ViewModels
{
	using System;
	using System.Linq;
	using System.Windows.Input;
	using GalaSoft.MvvmLight.Command;
	using Helpers;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
	using Sales.Common.Models;
	using Services;
	using Xamarin.Forms;
	public class AddProductViewModel : BaseViewModel
    {
		#region Attributes
		private bool isRunning;
		private bool isEnabled;
		private ApiService apiService;
		public ImageSource imageSource;
		public MediaFile file; // Atributo necesario para tomar foto.
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

		public ImageSource ImageSource
		{
			get { return this.imageSource; }
			set { this.SetValue(ref this.imageSource, value); }
		}

		#endregion

		#region Constructors
		public AddProductViewModel()
		{
			this.apiService = new ApiService();
			this.IsEnabled = true;
			this.ImageSource = "noimage_256";
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

			byte[] imageArray = null;
			if(this.file!=null)
			{
				imageArray = FilesHelper.ReadFully(this.file.GetStream());
			}

			var product = new Product
			{
				Description = this.EntryDescription,
				Price = price,
				Remarks = this.EntryRemarks,
				ImageArray = imageArray
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
			// Add product no refresh using singleton.
			var newProduct = (Product)response.Result;
			var viewModel = ProductsViewModel.GetInstance();
			viewModel.Products.Add(newProduct);
			this.IsRunning = false;
			this.IsEnabled = true;
			await Application.Current.MainPage.Navigation.PopAsync();
		}

		public ICommand ChangeImageCommand {
			get
			{
				return new RelayCommand(ChangeImage);
			}
		}

		private async void ChangeImage()
		{
			await CrossMedia.Current.Initialize();

			var source = await Application.Current.MainPage.DisplayActionSheet(
				Languages.ImageSource,
				Languages.Cancel,
				null,
				Languages.FromGallery,
				Languages.NewPicture);

			if (source == Languages.Cancel)
			{
				this.file = null;
				return;
			}

			if (source == Languages.NewPicture)
			{
				this.file = await CrossMedia.Current.TakePhotoAsync(
					new StoreCameraMediaOptions
					{
						Directory = "Sample",
						Name = "test.jpg",
						PhotoSize = PhotoSize.Small,
					}
				);
			}
			else
			{
				this.file = await CrossMedia.Current.PickPhotoAsync();
			}

			if (this.file != null)
			{
				this.ImageSource = ImageSource.FromStream(() =>
				{
					var stream = this.file.GetStream();
					return stream;
				});
			}
		}
		#endregion
	}
}
