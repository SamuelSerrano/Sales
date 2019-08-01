

namespace Sales.ViewModels
{
	using GalaSoft.MvvmLight.Command;
	using Sales.Common.Models;
	using Sales.Helpers;
    using Sales.Views;
    using Services;
	using System;
	using System.Linq;
	using System.Windows.Input;
	using Xamarin.Forms;

	public class ProductItemViewModel : Product
	{
		#region Attributes
		private ApiService apiService;
		#endregion

		#region Constructor
		public ProductItemViewModel()
		{
			this.apiService = new ApiService();
		}
		#endregion

		#region Commands

		public ICommand EditProductCommand {
			get { return new RelayCommand(EditProduct); }
		}		

		public ICommand DeleteProductCommand
		{
			get { return new RelayCommand(DeleteProduct); }
		}
		#endregion

		#region Methods
		private async void EditProduct()
		{
			MainViewModel.GetInstance().EditProduct = new EditProductViewModel(this);
			await Application.Current.MainPage.Navigation.PushAsync(new EditProductPage());
		}

		private async void DeleteProduct()
		{
			var answer = await Application.Current.MainPage.DisplayAlert(Languages.Confirm, Languages.DeleteConfirmation, Languages.Yes, Languages.No);
			if (!answer) { return; }

			var checkConnection = await this.apiService.CheckConnection();
			if (!checkConnection.IsSuccess)
			{
				await Application.Current.MainPage.DisplayAlert(Languages.Error, checkConnection.Message, Languages.Accept);
				return;
			}

			var urlAPI = Application.Current.Resources["UrlAPI"].ToString();
			var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
			var urlProductController = Application.Current.Resources["UrlProductController"].ToString();
			var response = await apiService.Delete(urlAPI, urlPrefix, urlProductController, this.ProductId);
			if (!response.IsSuccess)
			{
				await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
				return;
			}

			var productsViewModel = ProductsViewModel.GetInstance();
			var deleteProduct = productsViewModel.MyProducts.Where(p => p.ProductId == this.ProductId).FirstOrDefault();
			if (deleteProduct != null)
			{
				productsViewModel.MyProducts.Remove(deleteProduct);
			}

			productsViewModel.RefreshList();
		} 
		#endregion

	}
}
