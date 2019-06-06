﻿namespace Sales.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using Common.Models;
	using Sales.Services;
	using Xamarin.Forms;

	public class ProductsViewModel : BaseViewModel
    {
		private ApiService apiService;
		private ObservableCollection<Product> products;
		public ObservableCollection<Product> Products {
			get { return this.products; }
			set { this.SetValue(ref this.products, value); }
		}

		public ProductsViewModel()
		{
			this.apiService = new ApiService();
			this.LoadProducts();
		}

		private async void LoadProducts()
		{
			var response = await apiService.GetList<Product>("https://salesapiservices.azurewebsites.net", "/api", "/Product");
			if (!response.IsSuccess)
			{
				await Application.Current.MainPage.DisplayAlert("Error",response.Message,"Accept");
				return;
			}

			var list = (List<Product>)response.Result;
			this.Products = new ObservableCollection<Product>(list);
		}
	}
}
