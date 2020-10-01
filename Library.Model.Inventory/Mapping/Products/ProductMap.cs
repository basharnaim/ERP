using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Products;

namespace Library.Model.Inventory.Mapping.Products
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            ToTable("Product", "dbo");
        }
    }
}
