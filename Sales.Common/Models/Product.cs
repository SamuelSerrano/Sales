namespace Sales.Common.Models
{
	using System;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Product
	{
		[Key]
		public int ProductId { get; set; }
		[Required]
		[StringLength(50)]
		public String Description { get; set; }
		[DataType(DataType.MultilineText)]
		public String Remarks { get; set; }
		[Display(Name ="Image")]
		public String ImagePath { get; set; }
		[DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode =false)]
		public Decimal Price { get; set; }
		[Display(Name = "Is Available")]
		public bool IsAvailable { get; set; }
		[Display(Name = "Publish On")]
		[DataType(DataType.Date)]
		public DateTime PublishOn { get; set; }
		[NotMapped]
		public byte[] ImageArray { get; set; }

		public string ImageFullPath { get
			{
				if (string.IsNullOrEmpty(this.ImagePath)) return "noimage_256";
				return $"http://201.244.122.43/WebSales/{this.ImagePath.Substring(1)}";
			}
		}

		public override string ToString()
		{
			return this.Description;
		}
	}
}
