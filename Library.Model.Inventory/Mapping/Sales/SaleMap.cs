using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Sales;

namespace Library.Model.Inventory.Mapping.Sales
{
    public class SaleMap : EntityTypeConfiguration<Sale>
    {
        public SaleMap()
        {
            ToTable("Sale", "dbo");
        }
    }
}
