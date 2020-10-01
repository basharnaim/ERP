using Library.Model.Inventory.Products;
using System.Data.Entity.ModelConfiguration;

namespace Library.Model.Inventory.Mapping.Products
{
    public class GradeMap : EntityTypeConfiguration<Grade>
    {
        public GradeMap()
        {
            ToTable("Grade", "dbo");
        }
    }
}
