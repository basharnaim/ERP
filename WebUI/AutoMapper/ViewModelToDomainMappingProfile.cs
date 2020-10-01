#region

using AutoMapper;
using Library.Model.Core.Addresses;
using Library.Model.Core.Banks;
using Library.Model.Core.Core;
using Library.Model.Core.Doctors;
using Library.Model.Core.Organizations;
using Library.Model.Core.Securities;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Customers;
using Library.Model.Inventory.Expenditures;
using Library.Model.Inventory.Products;
using Library.Model.Inventory.Promotions;
using Library.Model.Inventory.Purchases;
using Library.Model.Inventory.Sales;
using Library.Model.Inventory.Suppliers;
using Library.ViewModel.Core.Addresses;
using Library.ViewModel.Core.Banks;
using Library.ViewModel.Core.Doctors;
using Library.ViewModel.Core.Organizations;
using Library.ViewModel.Core.Securities;
using Library.ViewModel.Inventory.Accounts;
using Library.ViewModel.Inventory.Customers;
using Library.ViewModel.Inventory.Expenditures;
using Library.ViewModel.Inventory.Products;
using Library.ViewModel.Inventory.Promotions;
using Library.ViewModel.Inventory.Purchases;
using Library.ViewModel.Inventory.Sales;
using Library.ViewModel.Inventory.Suppliers;

#endregion

namespace ERP.WebUI.AutoMapper
{
    /// <summary>
    /// DomainToViewModelMappingProfile.
    /// </summary>
    /// <remarks>Jahangir Hossain Sheikh</remarks>
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            #region Address
            CreateMap<CountryViewModel, Country>();
            CreateMap<DivisionViewModel, Division>();
            CreateMap<DistrictViewModel, District>();
            CreateMap<AreaViewModel, Area>();
            #endregion

            #region Organization
            CreateMap<CompanyViewModel, Company>();
            CreateMap<BranchViewModel, Branch>();
            CreateMap<WareHouseViewModel, Warehouse>();
            CreateMap<CourierViewModel, Courier>();
            #endregion

            #region User
            CreateMap<UserGroupViewModel, UserGroup>();
            CreateMap<UserViewModel, User>();
            CreateMap<UserRoleViewModel, UserRole>();
            CreateMap<AreaViewModel, Area>();
            #endregion

            #region Banks
            CreateMap<BankBranchViewModel, BankBranch>();
            CreateMap<BankViewModel, Bank>();
            CreateMap<BankMasterViewModel, BankMaster>();
            CreateMap<BankLedgerViewModel, BankLedgerVm>();
            #endregion

            #region Customer
            CreateMap<CustomerCategoryViewModel, CustomerCategory>();
            CreateMap<CustomerViewModel, Customer>();
            #endregion

            #region Supplier
            CreateMap<SupplierCategoryViewModel, SupplierCategory>();
            CreateMap<SupplierViewModel, Supplier>();
            CreateMap<BrandViewModel, Brand>();
            #endregion


            #region Doctors
            CreateMap<PatientViewModel, Patient>();
            #endregion

            #region Product
            CreateMap<ProductBasicConfigurationCategoryViewModel, ProductBasicConfigurationCategory>();
            CreateMap<ProductBasicConfigurationViewModel, ProductBasicConfiguration>();
            CreateMap<CurrencyViewModel, Currency>();
            CreateMap<StyleViewModel, Style>();
            CreateMap<ColorViewModel, Color>();
            CreateMap<FlavorViewModel, Flavor>();
            CreateMap<RAMViewModel, RAM>();
            CreateMap<ROMViewModel, ROM>();
            CreateMap<FloorViewModel, Floor>();
            CreateMap<RackViewModel, Rack>();
            CreateMap<GradeViewModel, Grade>();
            CreateMap<ManufacturerViewModel, Manufacturer>();
            CreateMap<SizeViewModel, Size>();
            CreateMap<UomViewModel, Uom>();
            CreateMap<VatCategoryViewModel, VatCategory>();
            CreateMap<ProductCategoryViewModel, ProductCategory>();
            CreateMap<ProductSubCategoryViewModel, ProductSubCategory>();
            CreateMap<ProductSubsidiaryCategoryViewModel, ProductSubsidiaryCategory>();
            CreateMap<ProductViewModel, Product>();
            CreateMap<ProductStockViewModel, ProductStockVm>();
            CreateMap<ProductStockViewModel, ProductVm>();
            CreateMap<ProductViewModelForReport, Product>();
            CreateMap<ProductMasterViewModel, ProductMaster>();
            #endregion

            #region Promotions
            CreateMap<PointPolicyViewModel, PointPolicy>();
            CreateMap<PointPolicyDetailViewModel, PointPolicyDetail>();
            #endregion

            #region Purchases
            CreateMap<MobileShopPurchaseViewModel, Purchase>();
            CreateMap<MobileShopPurchaseDetailViewModel, PurchaseDetail>();
            CreateMap<MobileShopPurchaseViewModel, PurchaseVm>();
            CreateMap<MobileShopPurchaseDetailViewModel, PurchaseDetailVm>();
            CreateMap<SuperShopPurchaseViewModel, Purchase>();
            CreateMap<PurchaseViewModelForReport, Purchase>();
            CreateMap<SuperShopPurchaseDetailViewModel, PurchaseDetail>();
            CreateMap<SuperShopPurchaseViewModel, PurchaseVm>();
            CreateMap<SuperShopPurchaseDetailViewModel, PurchaseDetailVm>();
            CreateMap<PurchaseDetailViewModelForReport, PurchaseDetail>();
            CreateMap<PurchaseReturnViewModel, PurchaseReturn>();
            CreateMap<PurchaseReturnDetailViewModel, PurchaseReturnDetail>();
            CreateMap<PurchaseReturnViewModel, PurchaseVm>();
            #endregion

            #region Sales
            CreateMap<MobileShopSaleViewModel, Sale>();
            CreateMap<MobileShopSaleDetailViewModel, SaleDetail>();
            CreateMap<MobileShopSaleViewModel, SaleVm>();
            CreateMap<MobileShopSaleDetailViewModel, SaleDetailVm>();
            CreateMap<SuperShopSaleViewModel, Sale>();
            CreateMap<SaleViewModelForReport, Sale>();
            CreateMap<SuperShopSaleDetailViewModel, SaleDetail>();
            CreateMap<SaleDetailViewModelForReport, SaleDetail>();
            CreateMap<SuperShopSaleViewModel, SaleVm>();
            CreateMap<SuperShopSaleDetailViewModel, SaleDetailVm>();
            CreateMap<SaleReturnViewModel, SaleReturn>();
            CreateMap<SaleReturnDetailViewModel, SaleReturnDetail>();
            CreateMap<SaleReturnViewModel, SaleVm>();
            #endregion

            #region Accounts
            CreateMap<BankLedgerViewModel, BankLedger>();
            CreateMap<SupplierLedgerViewModel, SupplierLedger>();
            CreateMap<SupplierLedgerViewModel, SupplierLedgerVm>();
            CreateMap<PaymentInfoViewModel, PaymentInfo>();
            CreateMap<CustomerLedgerViewModel, CustomerLedger>();
            CreateMap<CustomerLedgerViewModel, CustomerLedgerVm>();
            #endregion

            #region Expenditure
            CreateMap<ExpenditureCategoryViewModel, ExpenditureCategory>();
            CreateMap<ExpenditureSubCategoryViewModel, ExpenditureSubCategory>();
            CreateMap<ExpenditureSubsidiaryCategoryViewModel, ExpenditureSubsidiaryCategory>();
            CreateMap<ExpenditureViewModel, Expenditure>();
            #endregion
        }
    }
}