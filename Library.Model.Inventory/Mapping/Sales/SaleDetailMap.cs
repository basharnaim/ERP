using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Sales;

namespace Library.Model.Inventory.Mapping.Sales
{
    public class SaleDetailMap : EntityTypeConfiguration<SaleDetail>
    {
        public SaleDetailMap()
        {
            ToTable("SaleDetail", "dbo");
        }
    }
}
