using Library.Model.Core.Addresses;
using Library.Model.Core.Banks;
using Library.Model.Core.Doctors;
using Library.Model.Core.Logs;
using Library.Model.Core.Organizations;
using Library.Model.Core.Securities;
using Library.Model.EMS.Employees;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Customers;
using Library.Model.Inventory.Expenditures;
using Library.Model.Inventory.Products;
using Library.Model.Inventory.Promotions;
using Library.Model.Inventory.Purchases;
using Library.Model.Inventory.Sales;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;

namespace Library.Context
{
    public partial class ErpdbEntities : DbContext
    {
        #region Ctor
        static ErpdbEntities()
        {
            Database.SetInitializer<ErpdbEntities>(null);
        }

        public ErpdbEntities() : base("name=ErpdbEntities")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;

            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            // Sets the command timeout for all the commands
            objectContext.CommandTimeout = 300;
        }
        #endregion

        #region DbSet

        #region Securities
        public DbSet<ControlUser> ControlUser { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<AccessLog> AccessLog { get; set; }
        #endregion

        #region Organization
        public DbSet<Company> Company { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeGroup> EmployeeGroup { get; set; }
        public DbSet<EmployeeType> EmployeeType { get; set; }
        public DbSet<HolidayCalendar> HolidayCalendar { get; set; }
        public DbSet<OfficeCalendar> OfficeCalendar { get; set; }
        public DbSet<Courier> Courier { get; set; }
        #endregion

        #region Address
        public DbSet<Country> Country { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<Division> Division { get; set; }
        public DbSet<Area> Area { get; set; }
        #endregion

        #region Customer
        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerCategory> CustomerCategory { get; set; }
        #endregion

        #region Banks
        public DbSet<Bank> Bank { get; set; }
        public DbSet<BankBranch> BankBranch { get; set; }
        public DbSet<BankMaster> BankMaster { get; set; }
        #endregion

        #region Products
        public DbSet<ProductBasicConfigurationCategory> ConfigurationCategory { get; set; }
        public DbSet<ProductBasicConfiguration> ConfigurationSetting { get; set; }
        public DbSet<RAM> RAM { get; set; }
        public DbSet<ROM> ROM { get; set; }
        public DbSet<Manufacturer> Manufacturer { get; set; }
        public DbSet<Size> Size { get; set; }
        public DbSet<Uom> Uom { get; set; }
        public DbSet<VatCategory> VatCategory { get; set; }
        public DbSet<Style> Style { get; set; }
        public DbSet<Color> Color { get; set; }
        public DbSet<Flavor> Flavor { get; set; }
        public DbSet<Floor> Floor { get; set; }
        public DbSet<Rack> Rack { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<ProductSubCategory> ProductSubCategory { get; set; }
        public DbSet<ProductSubsidiaryCategory> ProductSubsidiaryCategory { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductMaster> ProductMaster { get; set; }
        #endregion

        #region Purchase        
        public DbSet<Purchase> Purchase { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetail { get; set; }
        public DbSet<PurchaseReturn> PurchaseReturn { get; set; }
        public DbSet<PurchaseReturnDetail> PurchaseReturnDetail { get; set; }
        #endregion

        #region Sales
        public DbSet<Sale> Sale { get; set; }
        public DbSet<SaleDetail> SaleDetail { get; set; }
        public DbSet<SaleReturn> SaleReturn { get; set; }
        public DbSet<SaleReturnDetail> SaleReturnDetail { get; set; }
        #endregion

        #region Accounts
        public DbSet<PaymentInfo> PaymentInfo { get; set; }
        public DbSet<BankLedger> BankLedger { get; set; }
        public DbSet<CustomerLedger> CustomerLedger { get; set; }
        public DbSet<SupplierLedger> SupplierLedger { get; set; }

        #endregion

        #region Expenditure
        public DbSet<ExpenditureCategory> ExpenditureCategory { get; set; }
        public DbSet<ExpenditureSubCategory> ExpenditureSubCategory { get; set; }
        public DbSet<ExpenditureSubsidiaryCategory> ExpenditureSubsidiaryCategory { get; set; }
        public DbSet<Expenditure> Expenditure { get; set; }
        #endregion

        #region Doctors
        public DbSet<Patient> Patient { get; set; }
        #endregion

        #region Promotions
        public DbSet<PointPolicy> PointPolicy { get; set; }
        public DbSet<PointPolicyDetail> PointPolicyDetail { get; set; }
        public DbSet<PromotionalDiscount> PromotionalDiscount { get; set; }
        public DbSet<PromotionalDiscountDetail> PromotionalDiscountDetail { get; set; }
        public DbSet<PromotionalDiscountMapping> PromotionalDiscountMapping { get; set; }
        public DbSet<PromotionalFreeItem> PromotionalFreeItem { get; set; }
        public DbSet<PromotionalFreeItemDetail> PromotionalFreeItemDetail { get; set; }
        public DbSet<PromotionalFreeItemMapping> PromotionalFreeItemMapping { get; set; }
        #endregion

        #endregion

        #region Action
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //throw new UnintentionalCodeFirstException();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Properties<decimal>().Configure(c => c.HasPrecision(38, 12));
            #region Register
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => !string.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null &&
                               type.BaseType.IsGenericType &&
                               (type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>) ||
                                type.BaseType.GetGenericTypeDefinition() == typeof(ComplexTypeConfiguration<>)));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            #endregion
        }
        #endregion
    }
}
