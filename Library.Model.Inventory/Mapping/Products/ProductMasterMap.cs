using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class ProductMasterMap : EntityTypeConfiguration<ProductMaster>
    {
        public ProductMasterMap()
        {
            ToTable("ProductMaster", "dbo");
        }
    }
}
