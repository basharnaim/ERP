using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class ProductCategoryMap : EntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryMap()
        {
            ToTable("ProductCategory", "dbo");
        }
    }
}
