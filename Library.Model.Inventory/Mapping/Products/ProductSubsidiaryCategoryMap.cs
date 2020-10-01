using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class ProductSubsidiaryCategoryMap : EntityTypeConfiguration<ProductSubsidiaryCategory>
    {
        public ProductSubsidiaryCategoryMap()
        {
            ToTable("ProductSubsidiaryCategory", "dbo");
        }
    }
}
