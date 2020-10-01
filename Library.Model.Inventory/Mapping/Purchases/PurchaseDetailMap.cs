using Library.Model.Inventory.Purchases;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Purchases
{
    public class PurchaseDetailMap : EntityTypeConfiguration<PurchaseDetail>
    {
        public PurchaseDetailMap()
        {
            ToTable("PurchaseDetail", "dbo");
        }
    }
}
