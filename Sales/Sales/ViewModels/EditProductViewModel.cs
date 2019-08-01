


namespace Sales.ViewModels
{
	using Common.Models;
	using GalaSoft.MvvmLight.Command;
	using Plugin.Media;
	using Plugin.Media.Abstractions;
	using Sales.Helpers;
	using Sales.Services;
	using System;
	using System.Linq;
	using System.Windows.Input;
	using Xamarin.Forms;

	public class EditProductViewModel : BaseViewModel
	{
		#region Attributes
		private Product product;
		private bool isRunning;
		private bool isEnabled;
		private ApiService apiService;
		public ImageSource imageSource;
		public MediaFile file; // Atributo necesario para tomar foto.
		#endregion

		#region Properties
		public Product Product
		{
			get { return this.product; }
			set { this.SetValue(ref this.product, value); }
		}

		public bool IsRunning
		{
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

		#region Constructor
		public EditProductViewModel(Product product)
		{
			this.product = product;
			this.apiService = new ApiService();
			this.IsEnabled = true;
			this.ImageSource = product.ImageFullPath;
		}
		#endregion

		#region Commands
		public ICommand SaveCommand
		{
			get
			{
				return new RelayCommand(Save);
			}
		}

		public ICommand DeleteCommand
		{
			get
			{
				return new RelayCommand(DeleteProduct);
			}
		}

		private async void DeleteProduct()
		{
			var answer = await Application.Current.MainPage.DisplayAlert(Languages.Confirm, Languages.DeleteConfirmation, Languages.Yes, Languages.No);
			if (!answer) { return; }

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

			

			var urlAPI = Application.Current.Resources["UrlAPI"].ToString();
			var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
			var urlProductController = Application.Current.Resources["UrlProductController"].ToString();
			var response = await apiService.Delete(urlAPI, urlPrefix, urlProductController,this.Product.ProductId);
			if (!response.IsSuccess)
			{
				await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
				return;
			}

			var productsViewModel = ProductsViewModel.GetInstance();
			var deleteProduct = productsViewModel.MyProducts.Where(p => p.ProductId == this.Product.ProductId).FirstOrDefault();
			if (deleteProduct != null)
			{
				productsViewModel.MyProducts.Remove(deleteProduct);
			}

			productsViewModel.RefreshList();
			this.IsRunning = false;
			this.IsEnabled = true;
			await Application.Current.MainPage.Navigation.PopAsync();
		}

		private async void Save()
		{
			if (string.IsNullOrEmpty(this.Product.Description))
			{
				await Application.Current.MainPage.DisplayAlert(
					Languages.Error,
					Languages.DescriptionError,
					Languages.Accept);
			}

			
			if (this.Product.Price < 0)
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
			if (this.file != null)
			{
				imageArray = FilesHelper.ReadFully(this.file.GetStream());
				this.Product.ImageArray = imageArray;
			}
			
			var urlAPI = Application.Current.Resources["UrlAPI"].ToString();
			var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
			var urlProductController = Application.Current.Resources["UrlProductController"].ToString();
			var response = await apiService.Put(urlAPI, urlPrefix, urlProductController, this.product,this.product.ProductId);

			if (!response.IsSuccess)
			{
				this.IsRunning = false;
				this.IsEnabled = true;
				await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
				return;
			}
			// Add product no refresh using singleton.
			var newProduct = (Product)response.Result;
			var productsViewModel = ProductsViewModel.GetInstance();

			var oldProduct = productsViewModel.MyProducts.Where(p => p.ProductId == this.Product.ProductId).FirstOrDefault();
			if (oldProduct != null)
			{
				productsViewModel.MyProducts.Remove(oldProduct);
			}

			productsViewModel.MyProducts.Add(newProduct);
			productsViewModel.RefreshList();

			this.IsRunning = false;
			this.IsEnabled = true;
			await Application.Current.MainPage.Navigation.PopAsync();
		}

		public ICommand ChangeImageCommand
		{
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
