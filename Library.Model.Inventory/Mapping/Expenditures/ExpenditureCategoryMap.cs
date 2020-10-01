using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Expenditures;

namespace Library.Model.Inventory.Mapping.Expenditures
{
    public class ExpenditureCategoryMap : EntityTypeConfiguration<ExpenditureCategory>
    {
        public ExpenditureCategoryMap()
        {
            ToTable("ExpenditureCategory", "dbo");
        }
    }
}
