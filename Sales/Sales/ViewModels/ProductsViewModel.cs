namespace Sales.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
	using Common.Models;
	using GalaSoft.MvvmLight.Command;
	using Sales.Helpers;
	using Sales.Services;
	using Xamarin.Forms;

	public class ProductsViewModel : BaseViewModel
    {
		#region Atributes
		private ApiService apiService;
		private bool isRefreshing;		
		private ObservableCollection<ProductItemViewModel> products;
		#endregion

		public List<Product> MyProducts { get; set; }
		public ObservableCollection<ProductItemViewModel> Products {
			get { return this.products; }
			set { this.SetValue(ref this.products, value); }
		}

		public bool IsRefreshing
		{
			get { return this.isRefreshing; }
			set { this.SetValue(ref this.isRefreshing, value); }
		}
		#region Constructor
		public ProductsViewModel()
		{
			instance = this;
			this.apiService = new ApiService();
			this.LoadProducts();
		}
		#endregion

		#region Singleton
		private static ProductsViewModel instance;

		public static ProductsViewModel GetInstance()
		{
			if (instance == null)
			{
				return new ProductsViewModel();
			}
			return instance;
		}
		#endregion

		private async void LoadProducts()
		{
			this.IsRefreshing = true;			
			var checkConnection = await this.apiService.CheckConnection();
			if (!checkConnection.IsSuccess)
			{
				this.IsRefreshing = false;
				this.Products = new ObservableCollection<ProductItemViewModel>(new List<ProductItemViewModel>());
				await Application.Current.MainPage.DisplayAlert(Languages.Error, checkConnection.Message, Languages.Accept);				
				return;
			}
			var urlAPI = Application.Current.Resources["UrlAPI"].ToString();
			var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
			var urlProductController = Application.Current.Resources["UrlProductController"].ToString();
			var response = await apiService.GetList<Product>(urlAPI, urlPrefix, urlProductController);
			if (!response.IsSuccess)
			{
				this.IsRefreshing = false;
				await Application.Current.MainPage.DisplayAlert(Languages.Error,response.Message,Languages.Accept);
				return;
			}

			this.MyProducts = (List<Product>)response.Result;
			this.RefreshList();			
			this.IsRefreshing = false;
		}

		public void RefreshList()
		{
			var mylistProductItemViewModel = MyProducts.Select(p => new ProductItemViewModel
			{
				Description = p.Description,
				ImageArray = p.ImageArray,
				ImagePath = p.ImagePath,
				IsAvailable = p.IsAvailable,
				Price = p.Price,
				ProductId = p.ProductId,
				PublishOn = p.PublishOn,
				Remarks = p.Remarks,
			});
			this.Products = new ObservableCollection<ProductItemViewModel>(mylistProductItemViewModel.OrderBy(p => p.Description));
		}

		public ICommand RefreshCommand
		{
			get
			{
				return new RelayCommand(LoadProducts);
			}
		}
	}
}
