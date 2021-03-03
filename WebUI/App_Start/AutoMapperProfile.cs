
using AutoMapper;
using Library.Model.Core.Addresses;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Products;
using Library.ViewModel.Core.Addresses;
using Library.ViewModel.Core.Organizations;
using Library.ViewModel.Inventory.Products;
using Library.Model.Core.Banks;
using Library.Model.Core.Core;
using Library.Model.Core.Doctors;
using Library.Model.Core.Menus;
using Library.Model.Core.Securities;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Customers;
using Library.Model.Inventory.Expenditures;
using Library.Model.Inventory.Promotions;
using Library.Model.Inventory.Purchases;
using Library.Model.Inventory.Sales;
using Library.Model.Inventory.Suppliers;
using Library.ViewModel.Core.Banks;
using Library.ViewModel.Core.Doctors;
using Library.ViewModel.Core.Securities;
using Library.ViewModel.Inventory.Accounts;
using Library.ViewModel.Inventory.Customers;
using Library.ViewModel.Inventory.Expenditures;
using Library.ViewModel.Inventory.Promotions;
using Library.ViewModel.Inventory.Purchases;
using Library.ViewModel.Inventory.Sales;
using Library.ViewModel.Inventory.Suppliers;

namespace ERP.WebUI.App_Start
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Country, CountryViewModel>().ReverseMap();
            CreateMap<Division, DivisionViewModel>().ReverseMap();
            CreateMap<District, DistrictViewModel>().ReverseMap();
            CreateMap<Area, AreaViewModel>().ReverseMap();

            CreateMap<Company, CompanyViewModel>().ReverseMap();
            CreateMap<Branch, BranchViewModel>().ReverseMap();
            CreateMap<Warehouse, WareHouseViewModel>().ReverseMap();
            CreateMap<Courier, CourierViewModel>().ReverseMap();

            CreateMap<Menu, MenuViewModel>().ReverseMap();
            CreateMap<UserGroup, UserGroupViewModel>().ReverseMap();
            CreateMap<UserRole, MenuItemListViewModel>().ReverseMap();
            CreateMap<User, UserViewModel>().ReverseMap();


            CreateMap<BankBranch, BankBranchViewModel>().ReverseMap();
            CreateMap<Bank, BankViewModel>().ReverseMap();
            CreateMap<BankMaster, BankMasterViewModel>().ReverseMap();
            CreateMap<BankLedgerVm, BankLedgerViewModel>().ReverseMap();
          
            CreateMap<CustomerCategory, CustomerCategoryViewModel>().ReverseMap();
            CreateMap<Customer, CustomerViewModel>().ReverseMap();
         
            CreateMap<SupplierCategory, SupplierCategoryViewModel>().ReverseMap();
            CreateMap<Supplier, SupplierViewModel>().ReverseMap();
            CreateMap<Brand, BrandViewModel>().ReverseMap();
           
            CreateMap<Patient, PatientViewModel>().ReverseMap();
          
            CreateMap<ProductBasicConfigurationCategory, ProductBasicConfigurationCategoryViewModel>().ReverseMap();
            CreateMap<ProductBasicConfiguration, ProductBasicConfigurationViewModel>().ReverseMap();
            CreateMap<Currency, CurrencyViewModel>().ReverseMap();
            CreateMap<Style, StyleViewModel>().ReverseMap();
            CreateMap<Color, ColorViewModel>().ReverseMap();
            CreateMap<Flavor, FlavorViewModel>().ReverseMap();
            CreateMap<RAM, RAMViewModel>().ReverseMap();
            CreateMap<ROM, ROMViewModel>().ReverseMap();
            CreateMap<Floor, FloorViewModel>().ReverseMap();
            CreateMap<Rack, RackViewModel>().ReverseMap();
            CreateMap<Grade, GradeViewModel>().ReverseMap();
            CreateMap<Manufacturer, ManufacturerViewModel>().ReverseMap();
            CreateMap<Size, SizeViewModel>().ReverseMap();
            CreateMap<Uom, UomViewModel>().ReverseMap();
            CreateMap<VatCategory, VatCategoryViewModel>().ReverseMap();
            CreateMap<ProductCategory, ProductCategoryViewModel>().ReverseMap();
            CreateMap<ProductSubCategory, ProductSubCategoryViewModel>().ReverseMap();
            CreateMap<ProductSubsidiaryCategory, ProductSubsidiaryCategoryViewModel>().ReverseMap();
            CreateMap<Product, ProductViewModel>().ReverseMap();
            CreateMap<ProductStockVm, ProductStockViewModel>().ReverseMap();
            CreateMap<ProductVm, ProductStockViewModel>().ReverseMap();
            CreateMap<Product, ProductViewModelForReport>().ReverseMap();
            CreateMap<ProductMaster, ProductMasterViewModel>().ReverseMap();
 
            CreateMap<PointPolicy, PointPolicyViewModel>().ReverseMap();
            CreateMap<PointPolicyDetail, PointPolicyDetailViewModel>().ReverseMap();
          
            CreateMap<Purchase, MobileShopPurchaseViewModel>().ReverseMap();
            CreateMap<PurchaseDetail, MobileShopPurchaseDetailViewModel>().ReverseMap();
            CreateMap<PurchaseVm, MobileShopPurchaseViewModel>().ReverseMap();
            CreateMap<PurchaseDetailVm, MobileShopPurchaseDetailViewModel>().ReverseMap();
            CreateMap<Purchase, SuperShopPurchaseViewModel>().ReverseMap();
            CreateMap<Purchase, PurchaseViewModelForReport>().ReverseMap();
            CreateMap<PurchaseDetail, SuperShopPurchaseDetailViewModel>().ReverseMap();
            CreateMap<PurchaseDetail, PurchaseDetailViewModelForReport>().ReverseMap();
            CreateMap<PurchaseVm, SuperShopPurchaseViewModel>().ReverseMap();
            CreateMap<PurchaseDetailVm, SuperShopPurchaseDetailViewModel>().ReverseMap();
            CreateMap<PurchaseReturn, PurchaseReturnViewModel>().ReverseMap();
            CreateMap<PurchaseReturnDetail, PurchaseReturnDetailViewModel>().ReverseMap();
            CreateMap<PurchaseVm, PurchaseReturnViewModel>().ReverseMap();
            CreateMap<PurchaseReturn, PurchaseViewModelForReport>().ReverseMap();
            CreateMap<PurchaseReturnDetail, PurchaseDetailViewModelForReport>().ReverseMap();
           
            CreateMap<Sale, MobileShopSaleViewModel>().ReverseMap();
            CreateMap<SaleDetail, MobileShopSaleDetailViewModel>().ReverseMap();
            CreateMap<SaleVm, MobileShopSaleViewModel>().ReverseMap();
            CreateMap<SaleDetailVm, MobileShopSaleDetailViewModel>().ReverseMap();
            CreateMap<Sale, SuperShopSaleViewModel>().ReverseMap();
            CreateMap<Sale, SaleViewModelForReport>().ReverseMap();
            CreateMap<SaleDetail, SuperShopSaleDetailViewModel>().ReverseMap();
            CreateMap<SaleDetail, SaleDetailViewModelForReport>().ReverseMap();
            CreateMap<SaleVm, SuperShopSaleViewModel>().ReverseMap();
            CreateMap<SaleDetailVm, SuperShopSaleDetailViewModel>().ReverseMap();
            CreateMap<SaleReturn, SaleReturnViewModel>().ReverseMap();
            CreateMap<SaleReturnDetail, SaleReturnDetailViewModel>().ReverseMap();
            CreateMap<SaleVm, SaleReturnViewModel>().ReverseMap();
            CreateMap<SaleReturn, SaleViewModelForReport>().ReverseMap();
            CreateMap<SaleReturnDetail, SaleDetailViewModelForReport>().ReverseMap();
           
            CreateMap<BankLedger, BankLedgerViewModel>().ReverseMap();
            CreateMap<SupplierLedger, SupplierLedgerViewModel>().ReverseMap();
            CreateMap<SupplierLedgerVm, SupplierLedgerViewModel>().ReverseMap();
            CreateMap<PaymentInfo, PaymentInfoViewModel>().ReverseMap();
            CreateMap<CustomerLedger, CustomerLedgerViewModel>().ReverseMap();
            CreateMap<CustomerLedgerVm, CustomerLedgerViewModel>().ReverseMap();
           
            CreateMap<ExpenditureCategory, ExpenditureCategoryViewModel>().ReverseMap();
            CreateMap<ExpenditureSubCategory, ExpenditureSubCategoryViewModel>().ReverseMap();
            CreateMap<ExpenditureSubsidiaryCategory, ExpenditureSubsidiaryCategoryViewModel>().ReverseMap();
            CreateMap<Expenditure, ExpenditureViewModel>().ReverseMap();
            CreateMap<ExpenditureVm, ExpenditureViewModel>().ReverseMap();
           
            CreateMap<CashBalanceVm, SuperShopSaleViewModel>().ReverseMap();          
        }
    }
}