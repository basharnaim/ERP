using Library.Model.Inventory.Sales;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Sales
{
    public class SaleReturnDetailMap : EntityTypeConfiguration<SaleReturnDetail>
    {
        public SaleReturnDetailMap()
        {
            ToTable("SaleReturnDetail", "dbo");
        }
    }
}
