using Library.Model.Inventory.Sales;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Sales
{
    public class SaleReturnMap : EntityTypeConfiguration<SaleReturn>
    {
        public SaleReturnMap()
        {
            ToTable("SaleReturn", "dbo");
        }
    }
}
