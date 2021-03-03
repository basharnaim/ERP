#region

using AutoMapper;
using Library.Model.Core.Addresses;
using Library.Model.Core.Banks;
using Library.Model.Core.Core;
using Library.Model.Core.Doctors;
using Library.Model.Core.Menus;
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
    /// <remarks></remarks>
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            #region Address
            CreateMap<Country, CountryViewModel>();
            CreateMap<Division, DivisionViewModel>();
            CreateMap<District, DistrictViewModel>();
            CreateMap<Area, AreaViewModel>();
            #endregion

            #region Organization
            CreateMap<Company, CompanyViewModel>().ReverseMap();
            CreateMap<Branch, BranchViewModel>();
            CreateMap<Warehouse, WareHouseViewModel>();
            CreateMap<Courier, CourierViewModel>();
            #endregion

            #region User
            CreateMap<Menu, MenuViewModel>();
            CreateMap<UserGroup, UserGroupViewModel>();
            CreateMap<UserRole, MenuItemListViewModel>();
            CreateMap<User, UserViewModel>();
            #endregion

            #region Banks
            CreateMap<BankBranch, BankBranchViewModel>();
            CreateMap<Bank, BankViewModel>();
            CreateMap<BankMaster, BankMasterViewModel>();
            CreateMap< BankLedgerVm, BankLedgerViewModel>();
            #endregion

            #region Customer
            CreateMap<CustomerCategory, CustomerCategoryViewModel>();
            CreateMap<Customer, CustomerViewModel>();
            #endregion

            #region Supplier
            CreateMap<SupplierCategory, SupplierCategoryViewModel>();
            CreateMap<Supplier, SupplierViewModel>();
            CreateMap<Brand, BrandViewModel>();
            #endregion

            #region Doctors
            CreateMap<Patient, PatientViewModel>();
            #endregion

            #region Product
            CreateMap<ProductBasicConfigurationCategory, ProductBasicConfigurationCategoryViewModel>();
            CreateMap<ProductBasicConfiguration, ProductBasicConfigurationViewModel>();
            CreateMap<Currency, CurrencyViewModel>();
            CreateMap<Style, StyleViewModel>();
            CreateMap<Color, ColorViewModel>();
            CreateMap<Flavor, FlavorViewModel>();
            CreateMap<RAM, RAMViewModel>();
            CreateMap<ROM, ROMViewModel>();
            CreateMap<Floor, FloorViewModel>();
            CreateMap<Rack, RackViewModel>();
            CreateMap<Grade, GradeViewModel>();
            CreateMap<Manufacturer, ManufacturerViewModel>();
            CreateMap<Size, SizeViewModel>();
            CreateMap<Uom, UomViewModel>();
            CreateMap<VatCategory, VatCategoryViewModel>();
            CreateMap<ProductCategory, ProductCategoryViewModel>();
            CreateMap<ProductSubCategory, ProductSubCategoryViewModel>();
            CreateMap<ProductSubsidiaryCategory, ProductSubsidiaryCategoryViewModel>();
            CreateMap<Product, ProductViewModel>();
            CreateMap<ProductStockVm, ProductStockViewModel>();
            CreateMap<ProductVm, ProductStockViewModel>();
            CreateMap<Product, ProductViewModelForReport>();
            CreateMap<ProductMaster, ProductMasterViewModel>();

            #endregion

            #region Promotions
            CreateMap<PointPolicy, PointPolicyViewModel>();
            CreateMap<PointPolicyDetail, PointPolicyDetailViewModel>();
            #endregion

            #region Purchases
            CreateMap<Purchase, MobileShopPurchaseViewModel>();
            CreateMap<PurchaseDetail, MobileShopPurchaseDetailViewModel>();
            CreateMap<PurchaseVm, MobileShopPurchaseViewModel>();
            CreateMap<PurchaseDetailVm, MobileShopPurchaseDetailViewModel>();
            CreateMap<Purchase, SuperShopPurchaseViewModel>();
            CreateMap<Purchase, PurchaseViewModelForReport>();
            CreateMap<PurchaseDetail, SuperShopPurchaseDetailViewModel>();
            CreateMap<PurchaseDetail, PurchaseDetailViewModelForReport>();
            CreateMap<PurchaseVm, SuperShopPurchaseViewModel>();
            CreateMap<PurchaseDetailVm, SuperShopPurchaseDetailViewModel>();
            CreateMap<PurchaseReturn, PurchaseReturnViewModel>();
            CreateMap<PurchaseReturnDetail, PurchaseReturnDetailViewModel>();
            CreateMap<PurchaseVm, PurchaseReturnViewModel>();
            CreateMap<PurchaseReturn, PurchaseViewModelForReport>();
            CreateMap<PurchaseReturnDetail, PurchaseDetailViewModelForReport>();
            #endregion

            #region Sales
            CreateMap<Sale, MobileShopSaleViewModel>();
            CreateMap<SaleDetail, MobileShopSaleDetailViewModel>();
            CreateMap<SaleVm, MobileShopSaleViewModel>();
            CreateMap<SaleDetailVm, MobileShopSaleDetailViewModel>();
            CreateMap<Sale, SuperShopSaleViewModel>();
            CreateMap<Sale, SaleViewModelForReport>();
            CreateMap<SaleDetail, SuperShopSaleDetailViewModel>();
            CreateMap<SaleDetail, SaleDetailViewModelForReport>();
            CreateMap<SaleVm, SuperShopSaleViewModel>();
            CreateMap<SaleDetailVm, SuperShopSaleDetailViewModel>();
            CreateMap<SaleReturn, SaleReturnViewModel>();
            CreateMap<SaleReturnDetail, SaleReturnDetailViewModel>();
            CreateMap<SaleVm, SaleReturnViewModel>();
            CreateMap<SaleReturn, SaleViewModelForReport>();
            CreateMap<SaleReturnDetail, SaleDetailViewModelForReport>();
            #endregion

            #region Accounts
            CreateMap<BankLedger, BankLedgerViewModel>();
            CreateMap<SupplierLedger, SupplierLedgerViewModel>();
            CreateMap<SupplierLedgerVm, SupplierLedgerViewModel>();
            CreateMap<PaymentInfo, PaymentInfoViewModel>();
            CreateMap<CustomerLedger, CustomerLedgerViewModel>();
            CreateMap<CustomerLedgerVm, CustomerLedgerViewModel>();
            #endregion

            #region Expenditure
            CreateMap<ExpenditureCategory, ExpenditureCategoryViewModel>();
            CreateMap<ExpenditureSubCategory, ExpenditureSubCategoryViewModel>();
            CreateMap<ExpenditureSubsidiaryCategory, ExpenditureSubsidiaryCategoryViewModel>();
            CreateMap<Expenditure, ExpenditureViewModel>();
            CreateMap<ExpenditureVm, ExpenditureViewModel>();
            #endregion

            #region Reports
            CreateMap<CashBalanceVm, SuperShopSaleViewModel>();
            #endregion
        }
    }
}