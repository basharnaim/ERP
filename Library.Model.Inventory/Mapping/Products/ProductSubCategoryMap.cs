using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class ProductSubCategoryMap : EntityTypeConfiguration<ProductSubCategory>
    {
        public ProductSubCategoryMap()
        {
            ToTable("ProductSubCategory", "dbo");
        }
    }
}
