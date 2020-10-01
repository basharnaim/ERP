using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class RackMap : EntityTypeConfiguration<Rack>
    {
        public RackMap()
        {
            ToTable("Rack", "dbo");
        }
    }
}
