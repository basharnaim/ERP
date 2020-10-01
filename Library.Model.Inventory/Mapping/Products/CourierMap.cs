using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class CourierMap : EntityTypeConfiguration<Courier>
    {
        public CourierMap()
        {
            ToTable("Courier", "dbo");
        }
    }
}
