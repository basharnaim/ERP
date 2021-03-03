using System;
using Library.Context;
using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Model.Core.Addresses;
using Library.Model.Core.Banks;
using Library.Model.Core.Doctors;
using Library.Model.Core.Logs;
using Library.Model.Core.Menus;
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
using Library.Model.Inventory.Suppliers;
using Library.Service.Core.Addresses;
using Library.Service.Core.Banks;
using Library.Service.Core.Core;
using Library.Service.Core.Doctors;
using Library.Service.Core.Enums;
using Library.Service.Core.Logs;
using Library.Service.Core.Menus;
using Library.Service.Core.Organizations;
using Library.Context.Repositories;
using Library.Service.Core.Securities;
using Library.Service.EMS.Employees;
using Library.Service.Inventory.Accounts;
using Library.Service.Inventory.Customers;
using Library.Service.Inventory.Expenditures;
using Library.Service.Inventory.Products;
using Library.Service.Inventory.Promotions;
using Library.Service.Inventory.Purchases;
using Library.Service.Inventory.Sales;
using Library.Service.Inventory.Suppliers;

using Unity;
using Unity.AspNet.Mvc;

namespace ERP.WebUI
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {

            #region Service Only
            container
                .RegisterType<ErpdbEntities, ErpdbEntities>(new PerRequestLifetimeManager())
                .RegisterType<IUnitOfWork, UnitOfWork>(new PerRequestLifetimeManager())
                .RegisterType(typeof(IService<>), typeof(Service<>))
                .RegisterType<IEnumService, EnumService>()
                .RegisterType<IRawSqlService, RawSqlService>()
                .RegisterType<IBarcodeService, BarcodeService>()
            #endregion

            #region Address
                //Country
                .RegisterType<IRepository<Country>, Repository<Country>>()
                .RegisterType<ICountryService, CountryService>()

                //Division
                .RegisterType<IRepository<Division>, Repository<Division>>()
                .RegisterType<IDivisionService, DivisionService>()

                //District
                .RegisterType<IRepository<District>, Repository<District>>()
                .RegisterType<IDistrictService, DistrictService>()
                .RegisterType<IRepository<Area>, Repository<Area>>()
                .RegisterType<IAreaService, AreaService>()
            #endregion

            #region Organization
                //Company
                .RegisterType<IRepository<Company>, Repository<Company>>()
                .RegisterType<ICompanyService, CompanyService>()

                //Branch
                .RegisterType<IRepository<Branch>, Repository<Branch>>()
                .RegisterType<IBranchService, BranchService>()

                //Warehouse
                .RegisterType<IRepository<Warehouse>, Repository<Warehouse>>()
                .RegisterType<IWarehouseService, WarehouseService>()

                //Menu
                .RegisterType<IRepository<Menu>, Repository<Menu>>()
                .RegisterType<IMenuService, MenuService>()

                .RegisterType<IRepository<Courier>, Repository<Courier>>()
                .RegisterType<ICourierService, CourierService>()
            #endregion

            #region Users
                //User
                .RegisterType<IRepository<User>, Repository<User>>()
                .RegisterType<IRepository<ControlUser>, Repository<ControlUser>>()
                .RegisterType<IUserService, UserService>()

                //UserRole
                .RegisterType<IRepository<UserRole>, Repository<UserRole>>()
                .RegisterType<IUserRoleService, UserRoleService>()

                //UserGroup
                .RegisterType<IRepository<UserGroup>, Repository<UserGroup>>()
                .RegisterType<IUserGroupService, UserGroupService>()
                //Menu
                .RegisterType<IRepository<Menu>, Repository<Menu>>()
                .RegisterType<IMenuService, MenuService>()
                //Access Log
                .RegisterType<IRepository<AccessLog>, Repository<AccessLog>>()
                .RegisterType<IAccessLogService, AccessLogService>()
            #endregion

            #region Accounts
             .RegisterType<IRepository<BankLedger>, Repository<BankLedger>>()
             .RegisterType<IBankLedgerService, BankLedgerService>()
             .RegisterType<IRepository<CustomerLedger>, Repository<CustomerLedger>>()
             .RegisterType<ICustomerLedgerService, CustomerLedgerService>()
             .RegisterType<IRepository<SupplierLedger>, Repository<SupplierLedger>>()
             .RegisterType<ISupplierLedgerService, SupplierLedgerService>()
             .RegisterType<IRepository<PaymentInfo>, Repository<PaymentInfo>>()
             .RegisterType<IPaymentInfoService, PaymentInfoService>()
            #endregion

            #region Banks
             .RegisterType<IRepository<Bank>, Repository<Bank>>()
             .RegisterType<IBankService, BankService>()
             .RegisterType<IRepository<BankBranch>, Repository<BankBranch>>()
             .RegisterType<IBankBranchService, BankBranchService>()
             .RegisterType<IRepository<BankMaster>, Repository<BankMaster>>()
             .RegisterType<IBankMasterService, BankMasterService>()
            #endregion

            #region Customer
             .RegisterType<IRepository<CustomerCategory>, Repository<CustomerCategory>>()
             .RegisterType<ICustomerCategoryService, CustomerCategoryService>()
             .RegisterType<IRepository<Customer>, Repository<Customer>>()
             .RegisterType<ICustomerService, CustomerService>()
            #endregion

            #region Supplier
             .RegisterType<IRepository<SupplierCategory>, Repository<SupplierCategory>>()
             .RegisterType<ISupplierCategoryService, SupplierCategoryService>()
             .RegisterType<IRepository<Supplier>, Repository<Supplier>>()
             .RegisterType<ISupplierService, SupplierService>()
             .RegisterType<IRepository<Brand>, Repository<Brand>>()
             .RegisterType<IBrandService, BrandService>()
            #endregion

            #region Doctor
             .RegisterType<IRepository<Patient>, Repository<Patient>>()
             .RegisterType<IPatientService, PatientService>()
            #endregion

            #region Employee
                .RegisterType<IRepository<EmployeeType>, Repository<EmployeeType>>()
                .RegisterType<IEmployeeTypeService, EmployeeTypeService>()
                .RegisterType<IRepository<EmployeeGroup>, Repository<EmployeeGroup>>()
                .RegisterType<IEmployeeGroupService, EmployeeGroupService>()
                .RegisterType<IRepository<Department>, Repository<Department>>()
                .RegisterType<IDepartmentService, DepartmentService>()
                .RegisterType<IRepository<Designation>, Repository<Designation>>()
                .RegisterType<IDesignationService, DesignationService>()
                .RegisterType<IRepository<Employee>, Repository<Employee>>()
                .RegisterType<IEmployeeService, EmployeeService>()
                .RegisterType<IRepository<OfficeCalendar>, Repository<OfficeCalendar>>()
                .RegisterType<IOfficeCalendarService, OfficeCalendarService>()
            #endregion

            #region Product
                .RegisterType<IRepository<Style>, Repository<Style>>()
                .RegisterType<IStyleService, StyleService>()
                .RegisterType<IRepository<Color>, Repository<Color>>()
                .RegisterType<IColorService, ColorService>()
                .RegisterType<IRepository<Flavor>, Repository<Flavor>>()
                .RegisterType<IFlavorService, FlavorService>()
                .RegisterType<IRepository<RAM>, Repository<RAM>>()
                .RegisterType<IRAMService, RAMService>()
                .RegisterType<IRepository<ROM>, Repository<ROM>>()
                .RegisterType<IROMService, ROMService>()
                .RegisterType<IRepository<Floor>, Repository<Floor>>()
                .RegisterType<IFloorService, FloorService>()
                .RegisterType<IRepository<Rack>, Repository<Rack>>()
                .RegisterType<IRackService, RackService>()
                .RegisterType<IRepository<Grade>, Repository<Grade>>()
                .RegisterType<IGradeService, GradeService>()
                .RegisterType<IRepository<Manufacturer>, Repository<Manufacturer>>()
                .RegisterType<IManufacturerService, ManufacturerService>()
                .RegisterType<IRepository<Size>, Repository<Size>>()
                .RegisterType<ISizeService, SizeService>()
                .RegisterType<IRepository<VatCategory>, Repository<VatCategory>>()
                .RegisterType<IVatCategoryService, VatCategoryService>()
                .RegisterType<IRepository<Uom>, Repository<Uom>>()
                .RegisterType<IUomService, UomService>()
                .RegisterType<IRepository<Currency>, Repository<Currency>>()
                .RegisterType<ICurrencyService, CurrencyService>()
                .RegisterType<IRepository<ProductBasicConfigurationCategory>, Repository<ProductBasicConfigurationCategory>>()
                .RegisterType<IProductBasicConfigurationCategoryService, ProductBasicConfigurationCategoryService>()
                .RegisterType<IRepository<ProductBasicConfiguration>, Repository<ProductBasicConfiguration>>()
                .RegisterType<IProductBasicConfigurationService, ProductBasicConfigurationService>()
                .RegisterType<IRepository<ProductCategory>, Repository<ProductCategory>>()
                .RegisterType<IProductCategoryService, ProductCategoryService>()
                .RegisterType<IRepository<ProductSubCategory>, Repository<ProductSubCategory>>()
                .RegisterType<IProductSubCategoryService, ProductSubCategoryService>()
                .RegisterType<IRepository<ProductSubsidiaryCategory>, Repository<ProductSubsidiaryCategory>>()
                .RegisterType<IProductSubsidiaryCategoryService, ProductSubsidiaryCategoryService>()
                .RegisterType<IRepository<Product>, Repository<Product>>()
                .RegisterType<IProductService, ProductService>()
                .RegisterType<IRepository<ProductMaster>, Repository<ProductMaster>>()
                .RegisterType<IProductMasterService, ProductMasterService>()
            #endregion

            #region Purchases
                .RegisterType<IRepository<Purchase>, Repository<Purchase>>()
                .RegisterType<IRepository<PurchaseDetail>, Repository<PurchaseDetail>>()
                .RegisterType<IPurchaseService, PurchaseService>()
                .RegisterType<IRepository<PurchaseReturn>, Repository<PurchaseReturn>>()
                .RegisterType<IRepository<PurchaseReturnDetail>, Repository<PurchaseReturnDetail>>()
                .RegisterType<IPurchaseReturnService, PurchaseReturnService>()
            #endregion

            #region Sales
                .RegisterType<IRepository<Sale>, Repository<Sale>>()
                .RegisterType<IRepository<SaleDetail>, Repository<SaleDetail>>()
                .RegisterType<ISaleService, SaleService>()
                .RegisterType<IRepository<SaleReturn>, Repository<SaleReturn>>()
                .RegisterType<IRepository<SaleReturnDetail>, Repository<SaleReturnDetail>>()
                .RegisterType<ISaleReturnService, SaleReturnService>()
            #endregion

            #region Account Management
                .RegisterType<IRepository<PaymentInfo>, Repository<PaymentInfo>>()
                .RegisterType<IPaymentInfoService, PaymentInfoService>()

                .RegisterType<IRepository<CustomerLedger>, Repository<CustomerLedger>>()
                .RegisterType<ICustomerLedgerService, CustomerLedgerService>()

                .RegisterType<IRepository<SupplierLedger>, Repository<SupplierLedger>>()
                .RegisterType<ISupplierLedgerService, SupplierLedgerService>()

                .RegisterType<IRepository<BankLedger>, Repository<BankLedger>>()
                .RegisterType<IBankLedgerService, BankLedgerService>()
            #endregion

            #region Expenditure
                .RegisterType<IRepository<ExpenditureCategory>, Repository<ExpenditureCategory>>()
                .RegisterType<IExpenditureCategoryService, ExpenditureCategoryService>()

                .RegisterType<IRepository<ExpenditureSubCategory>, Repository<ExpenditureSubCategory>>()
                .RegisterType<IExpenditureSubCategoryService, ExpenditureSubCategoryService>()

                .RegisterType<IRepository<ExpenditureSubsidiaryCategory>, Repository<ExpenditureSubsidiaryCategory>>()
                .RegisterType<IExpenditureSubsidiaryCategoryService, ExpenditureSubsidiaryCategoryService>()

                .RegisterType<IRepository<Expenditure>, Repository<Expenditure>>()
                .RegisterType<IExpenditureService, ExpenditureService>()
            #endregion

            #region Promotions
                .RegisterType<IRepository<PointPolicy>, Repository<PointPolicy>>()
                .RegisterType<IRepository<PointPolicyDetail>, Repository<PointPolicyDetail>>()
                .RegisterType<IPointPolicyService, PointPolicyService>()
                .RegisterType<IRepository<PromotionalDiscount>, Repository<PromotionalDiscount>>()
                .RegisterType<IRepository<PromotionalDiscountDetail>, Repository<PromotionalDiscountDetail>>()
                .RegisterType<IRepository<PromotionalDiscountMapping>, Repository<PromotionalDiscountMapping>>()
                .RegisterType<IPromotionalDiscountMappingService, PromotionalDiscountMappingService>()
                .RegisterType<IPromotionalDiscountService, PromotionalDiscountService>()
                .RegisterType<IRepository<PromotionalFreeItem>, Repository<PromotionalFreeItem>>()
                .RegisterType<IRepository<PromotionalFreeItemDetail>, Repository<PromotionalFreeItemDetail>>()
                .RegisterType<IPromotionalFreeItemService, PromotionalFreeItemService>()
                .RegisterType<IRepository<PromotionalFreeItemMapping>, Repository<PromotionalFreeItemMapping>>()
                .RegisterType<IPromotionalFreeItemMappingService, PromotionalFreeItemMappingService>();
            #endregion
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
        }
    }
}