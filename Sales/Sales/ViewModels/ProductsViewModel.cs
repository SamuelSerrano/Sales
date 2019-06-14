namespace Sales.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Windows.Input;
	using Common.Models;
	using GalaSoft.MvvmLight.Command;
	using Sales.Services;
	using Xamarin.Forms;

	public class ProductsViewModel : BaseViewModel
    {
		private ApiService apiService;


		private bool isRefreshing;
		

		private ObservableCollection<Product> products;
		public ObservableCollection<Product> Products {
			get { return this.products; }
			set { this.SetValue(ref this.products, value); }
		}

		public bool IsRefreshing
		{
			get { return this.isRefreshing; }
			set { this.SetValue(ref this.isRefreshing, value); }
		}

		public ProductsViewModel()
		{
			this.apiService = new ApiService();
			this.LoadProducts();
		}

		private async void LoadProducts()
		{
			this.IsRefreshing = true;
			var checkConnection = await this.apiService.CheckConnection();
			if (!checkConnection.IsSuccess)
			{
				this.IsRefreshing = false;
				await Application.Current.MainPage.DisplayAlert("Error", checkConnection.Message, "Accept");
				return;
			}
			var urlAPI = Application.Current.Resources["UrlAPI"].ToString();
			var response = await apiService.GetList<Product>(urlAPI, "/api", "/Products");
			if (!response.IsSuccess)
			{
				this.IsRefreshing = false;
				await Application.Current.MainPage.DisplayAlert("Error",response.Message,"Accept");
				return;
			}

			var list = (List<Product>)response.Result;
			this.Products = new ObservableCollection<Product>(list);
			this.IsRefreshing = false;
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
