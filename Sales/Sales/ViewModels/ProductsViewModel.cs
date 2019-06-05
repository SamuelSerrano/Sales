namespace Sales.ViewModels
{
	using System.Collections.ObjectModel;
	using Common.Models;
	public class ProductsViewModel : BaseViewModel
    {
		private ObservableCollection<Product> products;
		public ObservableCollection<Product> Products {
			get { return this.products; }
			set { this.SetValue(ref this.products, value); }
		}
	}
}
