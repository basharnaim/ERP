using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Promotions;

namespace Library.Model.Inventory.Mapping.Promotions
{
    public class PointPolicyMap : EntityTypeConfiguration<PointPolicy>
    {
        public PointPolicyMap()
        {
            ToTable("PointPolicy", "dbo");
        }
    }
}
