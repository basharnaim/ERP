using System.Data.Entity.ModelConfiguration;
using Library.Model.Inventory.Expenditures;

namespace Library.Model.Inventory.Mapping.Expenditures
{
    public class ExpenditureSubCategoryMap : EntityTypeConfiguration<ExpenditureSubCategory>
    {
        public ExpenditureSubCategoryMap()
        {
            ToTable("ExpenditureSubCategory", "dbo");
        }
    }
}
