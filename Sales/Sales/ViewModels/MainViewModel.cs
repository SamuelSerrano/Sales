﻿

namespace Sales.ViewModels
{
	using GalaSoft.MvvmLight.Command;
	using Sales.Views;
	using System.Windows.Input;
	using Xamarin.Forms;
	public class MainViewModel
	{
		#region Properties
		public ProductsViewModel Products { get; set; }
		public EditProductViewModel EditProduct { get; set; }
		public AddProductViewModel AddProduct { get; set; }
		#endregion

		#region Constructor
		public MainViewModel()
		{
			instance = this;
			this.Products = new ProductsViewModel();
		}
		#endregion

		#region Commands
		public ICommand AddProductCommand
		{
			get { return new RelayCommand(GoToAddProduct); }
		}

		private async void GoToAddProduct()
		{
			this.AddProduct = new AddProductViewModel();
			await Application.Current.MainPage.Navigation.PushAsync(new AddProductPage());
		}
		#endregion

		#region Singleton
		private static MainViewModel instance;

		public static MainViewModel GetInstance()
		{
			if (instance == null)
			{
				return new MainViewModel();
			}
			return instance;
		}
		#endregion

	}
}
