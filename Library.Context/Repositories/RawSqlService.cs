using Library.Context;
using Library.Crosscutting.Helper;
using Library.Crosscutting.Securities;
using Library.Model.Core.Core;
using Library.Model.Inventory.Customers;
using Library.Model.Inventory.Sales;
using Library.ViewModel.Inventory.Purchases;
using Library.ViewModel.Inventory.Sales;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Library.Context.Repositories
{
    public class RawSqlService : IRawSqlService
    {
        #region Ctor
        private readonly ErpdbEntities _db;
        private readonly SqlConnection conn;
        public RawSqlService(ErpdbEntities db)
        {
            _db = db;
            conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ErpdbEntities"].ConnectionString);
        }
        #endregion

        #region Action
        public void UpdateCustomerLedgerRunningBalance(string customerId)
        {
            try
            {
                var sql = @"update CustomerLedger 
                            set RunningBalance = t1.total
                            from (
                            select 
                            Id,
                            sum(cstmLdgr.DebitAmount-cstmLdgr.CreditAmount) over(order by cstmLdgr.Id rows unbounded preceding) as total 
                            from CustomerLedger cstmLdgr
                            where cstmLdgr.CustomerId='" + customerId + @"'
                            ) t1
                            where CustomerLedger.Id = t1.Id;";
                _db.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateSupplierLedgerRunningBalance(string supplierId)
        {
            try
            {
                var sql = @"update SupplierLedger 
                            set RunningBalance = t1.total
                            from (
                            select 
                            Id,
                            sum(splrLdgr.DebitAmount-splrLdgr.CreditAmount) over(order by splrLdgr.Id rows unbounded preceding) as total 
                            from SupplierLedger splrLdgr
                            where splrLdgr.SupplierId='" + supplierId + @"'
                            ) t1
                            where SupplierLedger.Id = t1.Id;";
                _db.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateBankLedgerRunningBalance(string accountNo)
        {
            try
            {
                var sql = @"update BankLedger 
                        set RunningBalance = t1.total
                        from (
                        select 
                        Id,
                        sum(bnkLdgr.DebitAmount-bnkLdgr.CreditAmount) over(order by bnkLdgr.Id rows unbounded preceding) as total 
                        from BankLedger bnkLdgr
                        where bnkLdgr.AccountNo='" + accountNo + @"'
                        ) t1
                        where BankLedger.Id = t1.Id";
                _db.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public decimal GetCustomerOpeningBalanceByCustomerId(string customerId, DateTime date)
        {
            try
            {
                var sql = @"select 
                            top 1
                            sum(cstmLdgr.DebitAmount-cstmLdgr.CreditAmount) over(order by cstmLdgr.Id rows unbounded preceding) as total 
                            from CustomerLedger cstmLdgr
                            where 
                            cstmLdgr.CustomerId='" + customerId + @"' and
                            convert(date,cstmLdgr.TransactionDate)<=convert(date,'" + date + @"') ";
                return _db.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetCustomerOpeningBalanceByCustomerPhone(string customerPhone, DateTime date)
        {
            try
            {
                var sql = @"select 
                            top 1
                            sum(cstmLdgr.DebitAmount-cstmLdgr.CreditAmount) over(order by cstmLdgr.Id rows unbounded preceding) as total 
                            from CustomerLedger cstmLdgr
                            where 
                            cstmLdgr.CustomerPhone='" + customerPhone + @"' and
                            convert(date,cstmLdgr.TransactionDate)<=convert(date,'" + date + @"')";
                return _db.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetBankLedgerOpeningBalance(string accountNo, DateTime date)
        {
            try
            {
                var sql = @"select 
                            top 1
                            sum(bnkLdgr.DebitAmount-bnkLdgr.CreditAmount) over(order by bnkLdgr.Id rows unbounded preceding) as total 
                            from BankLedger bnkLdgr
                            where 
                            bnkLdgr.AccountNo='" + accountNo + @"' and
                            convert(date,bnkLdgr.TransactionDate)<=convert(date,'" + date + @"') ";
                return _db.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetSupershopCurrentStock(string companyId, string branchId, string ProductId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(ProductId))
                    paramSql = paramSql + " and ProductId='" + ProductId + "'";
                var sql = @"select 
                            distinct
                            isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0) as ProductStock
                            FROM  Product as itm
                            join ProductStock itmStk
                            on itm.Id=itmStk.ProductId 
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetCeramicStockInSft(string companyId, string branchId, string ProductId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(ProductId))
                    paramSql = paramSql + " and ProductId='" + ProductId + "'";
                var sql = @"select 
                            distinct
                            isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) * (itm.OutputConversion/ConversionUnit) from ProductStock  where ProductId=itm.Id),0) as ProductStockInSft
                            FROM  Product as itm
                            join ProductStock itmStk
                            on itm.Id=itmStk.ProductId 
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetCeramicStockInBox(string companyId, string branchId, string ProductId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(ProductId))
                    paramSql = paramSql + " and ProductId='" + ProductId + "'";
                var sql = @"select 
                            distinct
                            isNull((select convert(int, floor( sum(case when StockStatus='StockIn' then Quantity else - Quantity end)/ ConversionUnit)) from ProductStock  where ProductId=itm.Id and (select uom.Name  from  UOM as uom where uom.Id=itmStk.UOMId) ='Sft'),0) as ProductStockBox
                            FROM  Product as itm
                            join ProductStock itmStk
                            on itm.Id=itmStk.ProductId 
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetCeramicStockInPcs(string companyId, string branchId, string ProductId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(ProductId))
                    paramSql = paramSql + " and ProductId='" + ProductId + "'";
                var sql = @"select 
                            distinct
                            isNull((select convert(int, round((sum(case when StockStatus='StockIn' then Quantity else - Quantity end)/ itm.ConversionUnit - (select convert(int,sum(case when StockStatus='StockIn' then Quantity else - Quantity end)/ itm.ConversionUnit))),2)*itm.ConversionUnit) from ProductStock  where ProductId=itm.Id and (select uom.Name  from  UOM as uom where uom.Id=itmStk.UOMId) ='Sft'),0) as ProductStockPcs
                            FROM  Product as itm
                            join ProductStock itmStk
                            on itm.Id=itmStk.ProductId 
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ProductVm GetProductDetailWithStock(string companyId, string branchId, string productId)
        {
            try
            {
                var sql = @"select 
                            max(itm.Id) as value,
                            max(itm.Name) as label,
                            max(itm.Code) as ProductCode,
                            max(itm.Name) as ProductName,
                            max(isnull(itm.ReorderLevel,0)) as ReorderLevel,
                            max(isnull(itm.ShelfLife,0)) as ShelfLife,
                            max(itm.PurchasePrice) as PurchasePrice,
                            max(isnull(itm.RetailPrice,0)) as RetailPrice,
                            max(isnull(itm.WholeSalePrice,0)) as WholeSalePrice,
                            max(itm.ProductCategoryId) as ProductCategoryId,
                            isnull((select itmCtgry.Name from ProductCategory itmCtgry where max(itm.ProductCategoryId)=itmCtgry.Id),'') as ProductCategoryName,
                            max(isnull(itm.ProductSubCategoryId,'')) as ProductSubCategoryId,
                            isnull((select itmSbCtgry.Name from ProductSubCategory itmSbCtgry where max(itm.ProductSubCategoryId)=itmSbCtgry.Id),'') as ProductSubCategoryName,
                            max(isnull(itm.ProductSubsidiaryCategoryId,'')) as ProductSubsidiaryCategoryId,
                            isnull((select itmSbsCtgry.Name from ProductSubsidiaryCategory itmSbsCtgry where max(itm.ProductSubsidiaryCategoryId)=itmSbsCtgry.Id),'') as ProductSubsidiaryCategoryName,
                            max(itm.UOMId) as UOMId,
                            isnull((select um.Name from UOM um where max(itm.UOMId)=um.Id),'') as UOMName,
                            max(isnull(itm.VatCategoryId,'')) as VatCategoryId,
                            isnull((select vatCtgr.VatRate from VatCategory vatCtgr where max(itm.VatCategoryId)=vatCtgr.Id),0) as TaxRate,
                            max(isnull(itm.SupplierId,'')) as SupplierId,
                            ((select isnull(sum(prchDtl.Quantity),0)  from PurchaseDetail prchDtl where prchDtl.ProductId = max(itm.Id) and  prchDtl.CompanyId=max(itmMstr.CompanyId) and prchDtl.BranchId=max(itmMstr.BranchId)) +
                            (select isnull(sum(slRtnDtl.ReturnQuantity),0)  from SaleReturnDetail slRtnDtl where  slRtnDtl.ProductId =max(itm.Id) and  slRtnDtl.CompanyId=max(itmMstr.CompanyId) and slRtnDtl.BranchId=max(itmMstr.BranchId)))-(
                            (select isnull(sum(slDtl.Quantity),0)  from SaleDetail slDtl where slDtl.ProductId =max(itm.Id) and  slDtl.CompanyId=max(itmMstr.CompanyId) and slDtl.BranchId=max(itmMstr.BranchId)) +
                            (select isnull(sum(prchRtnDtl.ReturnQuantity),0)  from PurchaseReturnDetail prchRtnDtl where prchRtnDtl.ProductId =max(itm.Id) and  prchRtnDtl.CompanyId=max(itmMstr.CompanyId) and prchRtnDtl.BranchId=max(itmMstr.BranchId))) as ProductStock,
                            max(itmMstr.CompanyId) as CompanyId,
                            max(itmMstr.BranchId) as BranchId
                            from Product itm
                            join ProductMaster itmMstr
                            on itm.Id=itmMstr.ProductId
                            where itmMstr.CompanyId='" + companyId + @"' and itmMstr.BranchId='" + branchId + @"' and itmMstr.ProductId='" + productId + @"' ";
                return _db.Database.SqlQuery<ProductVm>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<AutoCompleteForWarehouse> GetWareHouseList(string term)
        {
            try
            {
                var sql = @"select 
                            wrhs.Id as value,
                            wrhs.Name as label
                            from Warehouse as wrhs
                            where 
                            wrhs.IsArchive=0 and
                            wrhs.Name like '%" + term + @"%'
                            order by Id";
                return _db.Database.SqlQuery<AutoCompleteForWarehouse>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ProductVm> GetProductList(string companyId, string branchId)
        {
            try
            {
                var sql = @"select 
                            itm.Id as value,
                            itm.Name as label,
                            itm.Code as ProductCode,
                            isnull(itm.ReorderLevel,0) as ReorderLevel,
                            isnull(itm.ShelfLife,0) as ShelfLife,
                            itm.PurchasePrice,
                            isnull(itm.RetailPrice,0) as RetailPrice,
                            isnull(itm.WholeSalePrice,0) as WholeSalePrice,
                            itm.ProductCategoryId,
                            isnull((select itmCtgry.Name from ProductCategory itmCtgry where itm.ProductCategoryId=itmCtgry.Id),'') as ProductCategoryName,
                            isnull(itm.ProductSubCategoryId,'') as ProductSubCategoryId,
                            isnull((select itmSbCtgry.Name from ProductSubCategory itmSbCtgry where itm.ProductSubCategoryId=itmSbCtgry.Id),'') as ProductSubCategoryName,
                            isnull(itm.ProductSubsidiaryCategoryId,'') as ProductSubsidiaryCategoryId,
                            isnull((select itmSbsCtgry.Name from ProductSubsidiaryCategory itmSbsCtgry where itm.ProductSubsidiaryCategoryId=itmSbsCtgry.Id),'') as ProductSubsidiaryCategoryName,
                            itm.UOMId,
                            isnull((select um.Name from UOM um where itm.UOMId=um.Id),'') as UOMName,
                            isnull(itm.TaxCategoryId,'') as TaxCategoryId,
                            isnull((select vatCtgr.TaxRate from TaxCategory vatCtgr where itm.TaxCategoryId=vatCtgr.Id),0) as TaxRate,
                            isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0) as ProductStock
                            from Product as itm
                            join ProductMaster itmMstr
                            on itm.Id=itmMstr.ProductId
                            where 
                            itmMstr.CompanyId=" + companyId + @" and
                            itmMstr.BranchId=" + branchId + @" 
                            order by 
                            [value]";
                return _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ProductVm> GetPurchaseImeiList(string companyId, string branchId)
        {
            try
            {
                var sql = @"select 
                            max(itm.Id) as value,
                            max(itm.Code) as label,
                            max(itm.Name) as ProductName,
                            max(isnull(itm.ReorderLevel,0)) as ReorderLevel,
                            max(isnull(itm.ShelfLife,0)) as ShelfLife,
                            max(itm.PurchasePrice) as PurchasePrice,
                            max(isnull(itm.RetailPrice,0)) as RetailPrice,
                            max(isnull(itm.WholeSalePrice,0)) as WholeSalePrice,
                            max(isnull(itm.WholeSalePrice,0)) as WholeSalePrice,
                            max(itm.ProductCategoryId) as ProductCategoryId,
                            isnull((select itmCtgry.Name from ProductCategory itmCtgry where max(itm.ProductCategoryId)=itmCtgry.Id),'') as ProductCategoryName,
                            max(isnull(itm.ProductSubCategoryId,'')) as ProductSubCategoryId,
                            isnull((select itmSbCtgry.Name from ProductSubCategory itmSbCtgry where max(itm.ProductSubCategoryId)=itmSbCtgry.Id),'') as ProductSubCategoryName,
                            max(isnull(itm.ProductSubsidiaryCategoryId,'')) as ProductSubsidiaryCategoryId,
                            isnull((select itmSbsCtgry.Name from ProductSubsidiaryCategory itmSbsCtgry where max(itm.ProductSubsidiaryCategoryId)=itmSbsCtgry.Id),'') as ProductSubsidiaryCategoryName,
                            max(itm.UOMId) as UOMId,
                            isnull((select um.Name from UOM um where max(itm.UOMId)=um.Id),'') as UOMName,
                            max(isnull(itm.VatCategoryId,'')) as VatCategoryId,
                            isnull((select vatCtgr.VatRate from VatCategory vatCtgr where max(itm.VatCategoryId)=vatCtgr.Id),0) as VatRate,
                            max(isnull(itm.SupplierId,'')) as SupplierId,
                            ((select isnull(sum(prchDtl.Quantity),0)  from PurchaseDetail prchDtl where prchDtl.ProductId = max(itm.Id) and  prchDtl.CompanyId=max(itmMstr.CompanyId) and prchDtl.BranchId=max(itmMstr.BranchId)) +
                            (select isnull(sum(slRtnDtl.ReturnQuantity),0)  from SaleReturnDetail slRtnDtl where  slRtnDtl.ProductId =max(itm.Id) and slRtnDtl.IsDamage=0 and  slRtnDtl.CompanyId=max(itmMstr.CompanyId) and slRtnDtl.BranchId=max(itmMstr.BranchId)))-(
                            (select isnull(sum(slDtl.Quantity),0)  from SaleDetail slDtl where slDtl.ProductId =max(itm.Id) and  slDtl.CompanyId=max(itmMstr.CompanyId) and slDtl.BranchId=max(itmMstr.BranchId)) +
                            (select isnull(sum(prchRtnDtl.ReturnQuantity),0)  from PurchaseReturnDetail prchRtnDtl where prchRtnDtl.ProductId =max(itm.Id) and  prchRtnDtl.CompanyId=max(itmMstr.CompanyId) and prchRtnDtl.BranchId=max(itmMstr.BranchId))) as ProductStock,
                            max(itmMstr.CompanyId) as CompanyId,
                            max(itmMstr.BranchId) as BranchId
                            from Product itm
                            join ProductMaster itmMstr
                            on itm.Id=itmMstr.ProductId
                            where itmMstr.CompanyId=" + companyId + @" and itmMstr.BranchId=" + branchId + @"
                            group by
                            itm.Id";
                return _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ProductVm> GetReceiveProductForArtisti(string companyId, string branchId)
        {
            try
            {
                var sql = @"select 
                            itm.Id as value,
                            itm.Code as label,
                            itm.Name as ProductName,
                            isnull(itm.ReorderLevel,0) as ReorderLevel,
                            isnull(itm.ShelfLife,0) as ShelfLife,
                            isnull(itm.CostOfGoods,0) as CostOfGoods,
                            isnull(itm.MaximumRetailPrice,0) as MaximumRetailPrice,
                            itm.ProductCategoryId,
                            isnull((select itmCtgry.Name from ProductCategory itmCtgry where itm.ProductCategoryId=itmCtgry.Id),'') as ProductCategoryName,
                            isnull(itm.ProductSubCategoryId,'') as ProductSubCategoryId,
                            isnull((select itmSbCtgry.Name from ProductSubCategory itmSbCtgry where itm.ProductSubCategoryId=itmSbCtgry.Id),'') as ProductSubCategoryName,
                            isnull(itm.ProductSubsidiaryCategoryId,'') as ProductSubsidiaryCategoryId,
                            isnull((select itmSbsCtgry.Name from ProductSubsidiaryCategory itmSbsCtgry where itm.ProductSubsidiaryCategoryId=itmSbsCtgry.Id),'') as ProductSubsidiaryCategoryName,
                            itm.UOMId,
                            isnull((select um.Name from UOM um where itm.UOMId=um.Id),'') as UOMName,
                            isnull(itm.TaxCategoryId,'') as TaxCategoryId,
                            isnull((select vatCtgr.TaxRate from TaxCategory vatCtgr where itm.TaxCategoryId=vatCtgr.Id),0) as TaxRate,
                            isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0) as ProductStock
                            from Product as itm
                            join ProductMaster itmMstr
                            on itm.Id=itmMstr.ProductId
                            where 
                            itmMstr.CompanyId=" + companyId + @" and
                            itmMstr.BranchId=" + branchId + @" 
                            order by 
                            [value]";
                return _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PromotionalDiscountVm> GetPromotionalPointAndDiscount(DateTime today)
        {
            try
            {
                var sql = @"select
                            pntPlcyDtl.SaleAmount,
                            pntPlcyDtl.DiscountAmount,
                            pntPlcyDtl.EquivalantPoint
                            from PointPolicy pntPlcy
                            join PointPolicyDetail pntPlcyDtl
                            on pntPlcy.Id=pntPlcyDtl.PointPolicyId
                            where 
                            convert(date,'" + today + @"') between  convert(date,pntPlcy.DateFrom) and convert(date,pntPlcy.DateTo) and
                            pntPlcy.Active=1 and
                            pntPlcy.Archive=0 
                            order by 
                            pntPlcy.Id";
                return _db.Database.SqlQuery<PromotionalDiscountVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ProductVm> GetReceiveProductForArtisti()
        {
            try
            {
                var sql = @"select 
                            itm.Id as value,
                            itm.Code as label,
                            itm.Name as ProductName,
                            isnull(itm.ReorderLevel,0) as ReorderLevel,
                            isnull(itm.ShelfLife,0) as ShelfLife,
                            isnull(itm.CostOfGoods,0) as CostOfGoods,
                            isnull(itm.MaximumRetailPrice,0) as MaximumRetailPrice,
                            itm.ProductCategoryId,
                            isnull((select itmCtgry.Name from ProductCategory itmCtgry where itm.ProductCategoryId=itmCtgry.Id),'') as ProductCategoryName,
                            isnull(itm.ProductSubCategoryId,'') as ProductSubCategoryId,
                            isnull((select itmSbCtgry.Name from ProductSubCategory itmSbCtgry where itm.ProductSubCategoryId=itmSbCtgry.Id),'') as ProductSubCategoryName,
                            isnull(itm.ProductSubsidiaryCategoryId,'') as ProductSubsidiaryCategoryId,
                            isnull((select itmSbsCtgry.Name from ProductSubsidiaryCategory itmSbsCtgry where itm.ProductSubsidiaryCategoryId=itmSbsCtgry.Id),'') as ProductSubsidiaryCategoryName,
                            itm.UOMId,
                            isnull((select um.Name from UOM um where itm.UOMId=um.Id),'') as UOMName,
                            isnull(itm.TaxCategoryId,'') as TaxCategoryId,
                            isnull((select vatCtgr.TaxRate from TaxCategory vatCtgr where itm.TaxCategoryId=vatCtgr.Id),0) as TaxRate,
                            isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0) as ProductStock
                            from Product as itm
                            join ProductMaster itmMstr
                            on itm.Id=itmMstr.ProductId
                            order by 
                            [value]";
                return _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ProductVm> GetAllSalesProductListWithPositiveStockByCode(string companyId, string branchId)
        {
            try
            {
                var sql = @"with itm as( select 
                            max(itm.Id) as value,
                            max(itm.Code) as label,
                            max(itm.Name) as ProductName,
                            max(isnull(itm.ReorderLevel,0)) as ReorderLevel,
                            max(isnull(itm.ShelfLife,0)) as ShelfLife,
                            max(itm.PurchasePrice) as PurchasePrice,
                            max(isnull(itm.RetailPrice,0)) as RetailPrice,
                            max(isnull(itm.WholeSalePrice,0)) as WholeSalePrice,
                            max(itm.ProductCategoryId) as ProductCategoryId,
                            isnull((select itmCtgry.Name from ProductCategory itmCtgry where max(itm.ProductCategoryId)=itmCtgry.Id),'') as ProductCategoryName,
                            max(isnull(itm.ProductSubCategoryId,'')) as ProductSubCategoryId,
                            isnull((select itmSbCtgry.Name from ProductSubCategory itmSbCtgry where max(itm.ProductSubCategoryId)=itmSbCtgry.Id),'') as ProductSubCategoryName,
                            max(isnull(itm.ProductSubsidiaryCategoryId,'')) as ProductSubsidiaryCategoryId,
                            isnull((select itmSbsCtgry.Name from ProductSubsidiaryCategory itmSbsCtgry where max(itm.ProductSubsidiaryCategoryId)=itmSbsCtgry.Id),'') as ProductSubsidiaryCategoryName,
                            max(itm.UOMId) as UOMId,
                            isnull((select um.Name from UOM um where max(itm.UOMId)=um.Id),'') as UOMName,
                            max(isnull(itm.VatCategoryId,'')) as VatCategoryId,
                            isnull((select vatCtgr.VatRate from VatCategory vatCtgr where max(itm.VatCategoryId)=vatCtgr.Id),0) as VatRate,
                            max(isnull(itm.SupplierId,'')) as SupplierId,
                            ((select isnull(sum(prchDtl.Quantity),0)  from PurchaseDetail prchDtl where prchDtl.ProductId = max(itm.Id) and  prchDtl.CompanyId=max(itmMstr.CompanyId) and prchDtl.BranchId=max(itmMstr.BranchId)) +
                            (select isnull(sum(slRtnDtl.ReturnQuantity),0)  from SaleReturnDetail slRtnDtl where  slRtnDtl.ProductId =max(itm.Id) and slRtnDtl.IsDamage=0 and  slRtnDtl.CompanyId=max(itmMstr.CompanyId) and slRtnDtl.BranchId=max(itmMstr.BranchId)))-(
                            (select isnull(sum(slDtl.Quantity),0)  from SaleDetail slDtl where slDtl.ProductId =max(itm.Id) and  slDtl.CompanyId=max(itmMstr.CompanyId) and slDtl.BranchId=max(itmMstr.BranchId)) +
                            (select isnull(sum(prchRtnDtl.ReturnQuantity),0)  from PurchaseReturnDetail prchRtnDtl where prchRtnDtl.ProductId =max(itm.Id) and  prchRtnDtl.CompanyId=max(itmMstr.CompanyId) and prchRtnDtl.BranchId=max(itmMstr.BranchId))) as ProductStock,
                            max(itmMstr.CompanyId) as CompanyId,
                            max(itmMstr.BranchId) as BranchId
                            from Product itm
                            join ProductMaster itmMstr
                            on itm.Id=itmMstr.ProductId
                            where itmMstr.CompanyId=" + companyId + @" and itmMstr.BranchId=" + branchId + @"
                            group by
                            itm.Id
                            )
                            select * from itm where ProductStock>0";
                return _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ProductVm> GetAllSalesProductListWithPositiveStockByName(string companyId, string branchId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and itmMstr.branchId='" + branchId + "'";
                paramSql = paramSql + " and isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0)>0";

                var sql = @"select 
                            itm.Id as value,
                            itm.Name as label,
                            itm.Code as ProductCode,
                            isnull(itm.ReorderLevel,0) as ReorderLevel,
                            isnull(itm.ShelfLife,0) as ShelfLife,
                            itm.PurchasePrice,
                            isnull(itm.RetailPrice,0) as RetailPrice,
                            isnull(itm.WholeSalePrice,0) as WholeSalePrice,
                            max(isnull(itm.MaxDiscount,0)) as MaxDiscount,
                            itm.ProductCategoryId,
                            isnull((select itmCtgry.Name from ProductCategory itmCtgry where itm.ProductCategoryId=itmCtgry.Id),'') as ProductCategoryName,
                            isnull(itm.ProductSubCategoryId,'') as ProductSubCategoryId,
                            isnull((select itmSbCtgry.Name from ProductSubCategory itmSbCtgry where itm.ProductSubCategoryId=itmSbCtgry.Id),'') as ProductSubCategoryName,
                            isnull(itm.ProductSubsidiaryCategoryId,'') as ProductSubsidiaryCategoryId,
                            isnull((select itmSbsCtgry.Name from ProductSubsidiaryCategory itmSbsCtgry where itm.ProductSubsidiaryCategoryId=itmSbsCtgry.Id),'') as ProductSubsidiaryCategoryName,
                            itm.UOMId,
                            isnull((select um.Name from UOM um where itm.UOMId=um.Id),'') as UOMName,
                            isnull(itm.TaxCategoryId,'') as TaxCategoryId,
                            isnull((select vatCtgr.TaxRate from TaxCategory vatCtgr where itm.TaxCategoryId=vatCtgr.Id),0) as TaxRate,
                            isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0) as ProductStock
                            from Product as itm
                            join ProductMaster itmMstr
                            on itm.Id=itmMstr.ProductId
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ProductVm> GetBranchwiseProductCodeStockAll(string companyId, string branchId)
        {
            try
            {
                var sql = @"SELECT max(itm.Id) AS value
	                        ,max(itm.Code) AS label
	                        ,max(itm.Id) AS ProductId
	                        ,max(itm.Code) AS ProductCode
	                        ,max(itm.Name) AS ProductName
	                        ,max(isnull(itm.ReorderLevel, 0)) AS ReorderLevel
	                        ,max(isnull(itm.ShelfLife, 0)) AS ShelfLife
	                        ,max(itm.PurchasePrice) AS PurchasePrice
	                        ,max(isnull(itm.RetailPrice, 0)) AS RetailPrice
	                        ,max(isnull(itm.WholeSalePrice, 0)) AS WholeSalePrice
	                        ,max(isnull(itm.MaxDiscount, 0)) AS MaxDiscount
	                        ,max(itm.ProductCategoryId) AS ProductCategoryId
	                        ,isnull((
			                        SELECT itmCtgry.Name
			                        FROM ProductCategory itmCtgry
			                        WHERE max(itm.ProductCategoryId) = itmCtgry.Id
			                        ), '') AS ProductCategoryName
	                        ,max(isnull(itm.ProductSubCategoryId, '')) AS ProductSubCategoryId
	                        ,isnull((
			                        SELECT itmSbCtgry.Name
			                        FROM ProductSubCategory itmSbCtgry
			                        WHERE max(itm.ProductSubCategoryId) = itmSbCtgry.Id
			                        ), '') AS ProductSubCategoryName
	                        ,max(isnull(itm.ProductSubsidiaryCategoryId, '')) AS ProductSubsidiaryCategoryId
	                        ,isnull((
			                        SELECT itmSbsCtgry.Name
			                        FROM ProductSubsidiaryCategory itmSbsCtgry
			                        WHERE max(itm.ProductSubsidiaryCategoryId) = itmSbsCtgry.Id
			                        ), '') AS ProductSubsidiaryCategoryName
	                        ,max(itm.UOMId) AS UOMId
	                        ,isnull((
			                        SELECT um.Name
			                        FROM UOM um
			                        WHERE max(itm.UOMId) = um.Id
			                        ), '') AS UOMName
	                        ,max(isnull(itm.VatCategoryId, '')) AS VatCategoryId
	                        ,isnull((
			                        SELECT vatCtgr.VatRate
			                        FROM VatCategory vatCtgr
			                        WHERE max(itm.VatCategoryId) = vatCtgr.Id
			                        ), 0) AS TaxRate
	                        ,max(isnull(itm.SupplierId, '')) AS SupplierId
	                        ,isnull((Select Sum(ReceiveQty) from ProductStockMaser where ProductId = max(itm.Id) group by ProductId), 0) AS ProductStock
	                        ,max(itmMstr.CompanyId) AS CompanyId
	                        ,max(itmMstr.BranchId) AS BranchId
                        FROM Product itm
                        JOIN ProductMaster itmMstr ON itm.Id = itmMstr.ProductId
                        WHERE itm.Active= 1 and itm.Archive = 0 and itmMstr.CompanyId = '" + companyId + @"' AND itmMstr.BranchId = '" + branchId + @"'
                        GROUP BY itm.Id";

                var data = _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<ProductVm> GetBranchwiseProductStockAll(string companyId, string branchId)
        {
            try
            {
                var sql = @"SELECT max(itm.Id) AS value
	                        ,max(itm.Name) AS label
	                        ,max(itm.Id) AS ProductId
	                        ,max(itm.Code) AS ProductCode
	                        ,max(itm.Name) AS ProductName
	                        ,max(isnull(itm.ReorderLevel, 0)) AS ReorderLevel
	                        ,max(isnull(itm.ShelfLife, 0)) AS ShelfLife
	                        ,max(itm.PurchasePrice) AS PurchasePrice
	                        ,max(isnull(itm.RetailPrice, 0)) AS RetailPrice
	                        ,max(isnull(itm.WholeSalePrice, 0)) AS WholeSalePrice
	                        ,max(isnull(itm.MaxDiscount, 0)) AS MaxDiscount
	                        ,max(itm.ProductCategoryId) AS ProductCategoryId
	                        ,isnull((
			                        SELECT itmCtgry.Name
			                        FROM ProductCategory itmCtgry
			                        WHERE max(itm.ProductCategoryId) = itmCtgry.Id
			                        ), '') AS ProductCategoryName
	                        ,max(isnull(itm.ProductSubCategoryId, '')) AS ProductSubCategoryId
	                        ,isnull((
			                        SELECT itmSbCtgry.Name
			                        FROM ProductSubCategory itmSbCtgry
			                        WHERE max(itm.ProductSubCategoryId) = itmSbCtgry.Id
			                        ), '') AS ProductSubCategoryName
	                        ,max(isnull(itm.ProductSubsidiaryCategoryId, '')) AS ProductSubsidiaryCategoryId
	                        ,isnull((
			                        SELECT itmSbsCtgry.Name
			                        FROM ProductSubsidiaryCategory itmSbsCtgry
			                        WHERE max(itm.ProductSubsidiaryCategoryId) = itmSbsCtgry.Id
			                        ), '') AS ProductSubsidiaryCategoryName
	                        ,max(itm.UOMId) AS UOMId
	                        ,isnull((
			                        SELECT um.Name
			                        FROM UOM um
			                        WHERE max(itm.UOMId) = um.Id
			                        ), '') AS UOMName
	                        ,max(isnull(itm.VatCategoryId, '')) AS VatCategoryId
	                        ,isnull((
			                        SELECT vatCtgr.VatRate
			                        FROM VatCategory vatCtgr
			                        WHERE max(itm.VatCategoryId) = vatCtgr.Id
			                        ), 0) AS TaxRate
	                        ,max(isnull(itm.SupplierId, '')) AS SupplierId
	                        ,isnull((Select Sum(ReceiveQty) from ProductStockMaser where ProductId = max(itm.Id) group by ProductId), 0) AS ProductStock
	                        ,max(itmMstr.CompanyId) AS CompanyId
	                        ,max(itmMstr.BranchId) AS BranchId
                        FROM Product itm
                        JOIN ProductMaster itmMstr ON itm.Id = itmMstr.ProductId
                        WHERE itm.Active= 1 and itm.Archive = 0 and itmMstr.CompanyId = '" + companyId + @"' AND itmMstr.BranchId = '" + branchId + @"'
                        GROUP BY itm.Id";

                var data = _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<ProductVm> GetBranchwiseProductStockGreaterThanZero(string companyId, string branchId)
        {
            try
            {
                //var sql = @"with itm as(
                //            select 
                //            max(itm.Id) as value,
                //            max(itm.Code) as label,
                //            max(itm.Id) as ProductId,
                //            max(itm.Code) as ProductCode,
                //            max(itm.Name) as ProductName,
                //            max(isnull(itm.ReorderLevel,0)) as ReorderLevel,
                //            max(isnull(itm.ShelfLife,0)) as ShelfLife,
                //            max(itm.PurchasePrice) as PurchasePrice,
                //            max(isnull(itm.RetailPrice,0)) as RetailPrice,
                //            max(isnull(itm.WholeSalePrice,0)) as WholeSalePrice,
                //            max(isnull(itm.MaxDiscount,0)) as MaxDiscount,
                //            max(itm.ProductCategoryId) as ProductCategoryId,
                //            isnull((select itmCtgry.Name from ProductCategory itmCtgry where max(itm.ProductCategoryId)=itmCtgry.Id),'') as ProductCategoryName,
                //            max(isnull(itm.ProductSubCategoryId,'')) as ProductSubCategoryId,
                //            isnull((select itmSbCtgry.Name from ProductSubCategory itmSbCtgry where max(itm.ProductSubCategoryId)=itmSbCtgry.Id),'') as ProductSubCategoryName,
                //            max(isnull(itm.ProductSubsidiaryCategoryId,'')) as ProductSubsidiaryCategoryId,
                //            isnull((select itmSbsCtgry.Name from ProductSubsidiaryCategory itmSbsCtgry where max(itm.ProductSubsidiaryCategoryId)=itmSbsCtgry.Id),'') as ProductSubsidiaryCategoryName,
                //            max(itm.UOMId) as UOMId,
                //            isnull((select um.Name from UOM um where max(itm.UOMId)=um.Id),'') as UOMName,
                //            max(isnull(itm.VatCategoryId,'')) as VatCategoryId,
                //            isnull((select vatCtgr.VatRate from VatCategory vatCtgr where max(itm.VatCategoryId)=vatCtgr.Id),0) as TaxRate,
                //            max(isnull(itm.SupplierId,'')) as GradeId,
                //            max(itmMstr.CompanyId) as CompanyId,
                //            max(itmMstr.BranchId) as BranchId,
                //            max(sm.CurrentStock) ProductStock
                //         FROM Product itm
                //         join ProductStockMaser sm on sm.ProductID = itm.Id
                //            join ProductMaster itmMstr
                //            on itm.Id=itmMstr.ProductId
                //            where itm.Active= 1 and itm.Archive = 0 and itmMstr.CompanyId='" + companyId + @"' and itmMstr.BranchId='" + branchId + @"'
                //            group by
                //            itm.Id
                //             )
                //            select * from itm where ProductStock>0";
                var sql = @"WITH itm
                            AS (
	                            SELECT max(itm.Id) AS value
		                            ,max(itm.Code) AS label
		                            ,max(itm.Id) AS ProductId
		                            ,max(itm.Code) AS ProductCode
		                            ,max(itm.Name) AS ProductName
		                            ,max(isnull(itm.ReorderLevel, 0)) AS ReorderLevel
		                            ,max(isnull(itm.ShelfLife, 0)) AS ShelfLife
		                            ,max(itm.PurchasePrice) AS PurchasePrice
		                            ,max(isnull(itm.RetailPrice, 0)) AS RetailPrice
		                            ,max(isnull(itm.WholeSalePrice, 0)) AS WholeSalePrice
		                            ,max(isnull(itm.MaxDiscount, 0)) AS MaxDiscount
		                            ,max(itm.ProductCategoryId) AS ProductCategoryId
		                            ,isnull((
				                            SELECT itmCtgry.Name
				                            FROM ProductCategory itmCtgry
				                            WHERE max(itm.ProductCategoryId) = itmCtgry.Id
				                            ), '') AS ProductCategoryName
		                            ,max(isnull(itm.ProductSubCategoryId, '')) AS ProductSubCategoryId
		                            ,isnull((
				                            SELECT itmSbCtgry.Name
				                            FROM ProductSubCategory itmSbCtgry
				                            WHERE max(itm.ProductSubCategoryId) = itmSbCtgry.Id
				                            ), '') AS ProductSubCategoryName
		                            ,max(isnull(itm.ProductSubsidiaryCategoryId, '')) AS ProductSubsidiaryCategoryId
		                            ,isnull((
				                            SELECT itmSbsCtgry.Name
				                            FROM ProductSubsidiaryCategory itmSbsCtgry
				                            WHERE max(itm.ProductSubsidiaryCategoryId) = itmSbsCtgry.Id
				                            ), '') AS ProductSubsidiaryCategoryName
		                            ,max(itm.UOMId) AS UOMId
		                            ,isnull((
				                            SELECT um.Name
				                            FROM UOM um
				                            WHERE max(itm.UOMId) = um.Id
				                            ), '') AS UOMName
		                            ,max(isnull(itm.VatCategoryId, '')) AS VatCategoryId
		                            ,isnull((
				                            SELECT vatCtgr.VatRate
				                            FROM VatCategory vatCtgr
				                            WHERE max(itm.VatCategoryId) = vatCtgr.Id
				                            ), 0) AS TaxRate
		                            ,max(isnull(itm.SupplierId, '')) AS GradeId
		                            ,max(itmMstr.CompanyId) AS CompanyId
		                            ,max(itmMstr.BranchId) AS BranchId
		                            ,isnull((Select top 1 sm.CurrentStock from ProductStockMaser sm Where sm.ProductID = max(itm.id)),100) ProductStock
	                            FROM Product itm	
	                            JOIN ProductMaster itmMstr ON itm.Id = itmMstr.ProductId
	                            WHERE itm.Active = 1 AND itm.Archive = 0 AND itmMstr.CompanyId = '" + companyId + @"' AND itmMstr.BranchId = '" + branchId + @"'
	                            GROUP BY itm.Id )
                            SELECT * FROM itm Where
                            CASE WHEN (Select isNegative From Company) = 1 OR (Select isNegative From Company) IS NULL THEN 1 
                            WHEN itm.ProductStock > 0 THEN 1							
                            ELSE 0 END = 1";
                var data = _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<CeramicProductVm> GetCeramicProductListForPurchase(string companyId, string branchId)
        {
            try
            {
                var sql = @"select 
                            itm.Id as value,
                            itm.Code as label,
                            itm.Name as ProductName,
                            isnull(itm.InputSize,'') as InputSize,
                            isnull(itm.OutputSize,'') as OutputSize,
                            isnull(itm.ConversionUnit,0) as ConversionUnit,
                            isnull(itm.InputConversion,0) as InputConversion,
                            isnull(itm.OutputConversion,0) as OutputConversion,
                            isnull(itm.ReorderLevel,0) as ReorderLevel,
                            isnull(itm.ShelfLife,0) as ShelfLife,
                            itm.PurchasePrice,
                            isnull(itm.RetailPrice,0) as RetailPrice,
                            isnull(itm.WholeSalePrice,0) as WholeSalePrice,
                            isnull(itm.MaxDiscount,0) as MaxDiscount,
                            itm.ProductCategoryId,
                            isnull((select itmCtgry.Name from ProductCategory itmCtgry where itm.ProductCategoryId=itmCtgry.Id),'') as ProductCategoryName,
                            isnull(itm.ProductSubCategoryId,'') as ProductSubCategoryId,
                            isnull((select itmSbCtgry.Name from ProductSubCategory itmSbCtgry where itm.ProductSubCategoryId=itmSbCtgry.Id),'') as ProductSubCategoryName,
                            isnull(itm.ProductSubsidiaryCategoryId,'') as ProductSubsidiaryCategoryId,
                            isnull((select itmSbsCtgry.Name from ProductSubsidiaryCategory itmSbsCtgry where itm.ProductSubsidiaryCategoryId=itmSbsCtgry.Id),'') as ProductSubsidiaryCategoryName,
                            itm.UOMId,
                            isnull((select um.Name from UOM um where itm.UOMId=um.Id),'') as UOMName,
                            isnull(itm.TaxCategoryId,'') as TaxCategoryId,
                            isnull((select vatCtgr.TaxRate from TaxCategory vatCtgr where itm.TaxCategoryId=vatCtgr.Id),0) as TaxRate,
                            isNull((select case when uom.Name='Sft' then convert(int, floor( sum(case when StockStatus='StockIn' then Quantity else - Quantity end)/ ConversionUnit))  else sum(case when StockStatus='StockIn' then Quantity else - Quantity end) end from ProductStock  where ProductId=itm.Id ),0) as ProductStock
                            from Product as itm
                            join ProductMaster itmMstr
                            on itm.Id=itmMstr.ProductId
                            join UOM uom
                            on itm.UOMId=uom.Id
                            where itmMstr.CompanyId=" + companyId + @" and
                            itmMstr.BranchId=" + branchId + @" 
                            order by 
                            [value]";
                return _db.Database.SqlQuery<CeramicProductVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<CeramicProductVm> GetCeramicProductListForSale(string companyId, string branchId)
        {
            try
            {
                var sql = @"select 
                            itm.Id as value,
                            itm.Code as label,
                            itm.Name as ProductName,
                            isnull(itm.InputSize,'') as InputSize,
                            isnull(itm.OutputSize,'') as OutputSize,
                            isnull(itm.ConversionUnit,0) as ConversionUnit,
                            isnull(itm.InputConversion,0) as InputConversion,
                            isnull(itm.OutputConversion,0) as OutputConversion,
                            isnull(itm.ReorderLevel,0) as ReorderLevel,
                            isnull(itm.ShelfLife,0) as ShelfLife,
                            itm.PurchasePrice,
                            isnull(itm.RetailPrice,0) as RetailPrice,
                            isnull(itm.WholeSalePrice,0) as WholeSalePrice,
                            isnull(itm.MaxDiscount,0) as MaxDiscount,
                            itm.ProductCategoryId,
                            isnull((select itmCtgry.Name from ProductCategory itmCtgry where itm.ProductCategoryId=itmCtgry.Id),'') as ProductCategoryName,
                            isnull(itm.ProductSubCategoryId,'') as ProductSubCategoryId,
                            isnull((select itmSbCtgry.Name from ProductSubCategory itmSbCtgry where itm.ProductSubCategoryId=itmSbCtgry.Id),'') as ProductSubCategoryName,
                            isnull(itm.ProductSubsidiaryCategoryId,'') as ProductSubsidiaryCategoryId,
                            isnull((select itmSbsCtgry.Name from ProductSubsidiaryCategory itmSbsCtgry where itm.ProductSubsidiaryCategoryId=itmSbsCtgry.Id),'') as ProductSubsidiaryCategoryName,
                            itm.UOMId,
                            isnull((select um.Name from UOM um where itm.UOMId=um.Id),'') as UOMName,
                            isnull(itm.TaxCategoryId,'') as TaxCategoryId,
                            isnull((select vatCtgr.TaxRate from TaxCategory vatCtgr where itm.TaxCategoryId=vatCtgr.Id),0) as TaxRate,
                            isNull((select case when uom.Name='Sft' then convert(int, floor( sum(case when StockStatus='StockIn' then Quantity else - Quantity end) * (itm.OutputConversion/ConversionUnit)))  else sum(case when StockStatus='StockIn' then Quantity else - Quantity end) end from ProductStock  where ProductId=itm.Id ),0) as ProductStock
                            from Product as itm
                            join ProductMaster itmMstr
                            on itm.Id=itmMstr.ProductId
                            join UOM uom
                            on itm.UOMId=uom.Id
                            where itm.Active= 1 and itm.Archive = 0 and itmMstr.CompanyId=" + companyId + @" and
                            itmMstr.BranchId=" + branchId + @" 
                            order by 
                            [value]";
                return _db.Database.SqlQuery<CeramicProductVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<LowStockProductVm> GetAllLowStockProduct(string companyId, string branchId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    if (!string.IsNullOrEmpty(companyId))
                        paramSql = " CompanyId='" + companyId + "'";
                    if (!string.IsNullOrEmpty(branchId))
                        paramSql = paramSql + " and itmStk.branchId='" + branchId + "'";
                    paramSql = paramSql + " and isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0)<itm.ReorderLevel";
                }
                else
                {
                    paramSql = "isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0)<itm.ReorderLevel";
                }
                var sql = @"select 
                            distinct
                            itm.Code as ProductCode,
                            itm.[Name] as ProductName,
                            isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0) as ProductStock
                            FROM  Product as itm
                            join ProductStock itmStk
                            on itm.Id=itmStk.ProductId 
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<LowStockProductVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ProductVm> GetAllProductForBarcodeSale(string companyId, string branchId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    if (!string.IsNullOrEmpty(companyId))
                        paramSql = " CompanyId='" + companyId + "'";
                    if (!string.IsNullOrEmpty(branchId))
                        paramSql = paramSql + " and itmStk.branchId='" + branchId + "'";
                    paramSql = paramSql + " and isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0)>0";
                }
                else
                {
                    paramSql = "isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0)<itm.ReorderLevel";
                }
                var sql = @"select 
                            itm.Id as value,
                            itm.Code as label,
                            ISNULL(isnull(itm.RetailPrice,0) as RetailPrice,0) as RetailPrice,
                            itm.Name,
                            itm.UOMId,
                            isnull(itm.IdealPrice,0) as IdealPrice,
                            isNull((select sum(case when StockStatus='StockIn' then Quantity else - Quantity end) from ProductStock  where ProductId=itm.Id ),0) as ProductStock,
                            isnull((select top 1 tc.TaxRate from  TaxCategory as tc where tc.Id=itm.TaxCategoryId),0) as Tax, 
                            (select  uom.Name from  UOM as uom where uom.Id=itm.UOMId) as UOMName 
                            from Product as itm
                            join ProductStock itmStk
                            on itm.Id=itmStk.ProductId 
                            where itm.Active = 1 and itm.Archive = 0 and " + paramSql + @"";
                return _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetStockQty(string companyId, string branchId, string ProductId)
        {
            try
            {
                var sql = @"select 
                            ((select isnull(sum(prchDtl.Quantity),0)  from PurchaseDetail prchDtl where prchDtl.ProductId = max(itm.Id) and  prchDtl.CompanyId=max(itmMstr.CompanyId) and prchDtl.BranchId=max(itmMstr.BranchId)) +
                            (select isnull(sum(slRtnDtl.ReturnQuantity),0)  from SaleReturnDetail slRtnDtl where  slRtnDtl.ProductId =max(itm.Id) and slRtnDtl.IsDamage=0 and  slRtnDtl.CompanyId=max(itmMstr.CompanyId) and slRtnDtl.BranchId=max(itmMstr.BranchId)))-(
                            (select isnull(sum(slDtl.Quantity),0)  from SaleDetail slDtl where slDtl.ProductId =max(itm.Id) and  slDtl.CompanyId=max(itmMstr.CompanyId) and slDtl.BranchId=max(itmMstr.BranchId)) +
                            (select isnull(sum(prchRtnDtl.ReturnQuantity),0)  from PurchaseReturnDetail prchRtnDtl where prchRtnDtl.ProductId =max(itm.Id) and  prchRtnDtl.CompanyId=max(itmMstr.CompanyId) and prchRtnDtl.BranchId=max(itmMstr.BranchId))) as ProductStock
                            from Product itm
                            join ProductMaster itmMstr
                            on itm.Id=itmMstr.ProductId
                            where itmMstr.CompanyId='" + companyId + @"' and itmMstr.BranchId='" + branchId + @"' and  itmMstr.ProductId='" + ProductId + @"'";
                return _db.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //public IEnumerable<ProductStockVm> GetAllProductStockbySp(string companyId, string branchId, string supplierId, string productCategoryId, string productSubCategoryId, string ProductId, string productCode, string dateFrom, string dateTo)
        //{
        //    var result =  _db.Database.ExecuteSqlCommand("exec getProductStock ?, ?", ProductId, productCode);
        //    //var result = await _db.Database<ProductStockVms>($"getProductStock {Id}").AsNoTracking().ToListAsync();
        //    return result;
        //}
        public IEnumerable<ProductStockVm> GetAllProductStock(string companyId, string branchId, string supplierId, string productCategoryId, string productSubCategoryId, string ProductId, string productCode, string dateFrom, string dateTo)
        {
            try
            {
                string paramSql = "", sql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " pm.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and pm.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(productCategoryId))
                    paramSql = paramSql + " and p.ProductCategoryId='" + productCategoryId + "'";
                if (!string.IsNullOrEmpty(productSubCategoryId))
                    paramSql = paramSql + " and p.ProductSubCategoryId='" + productSubCategoryId + "'";
                if (!string.IsNullOrEmpty(ProductId))
                    paramSql = paramSql + " and p.Id='" + ProductId + "'";
                if (!string.IsNullOrEmpty(productCode))
                    paramSql = paramSql + " and p.Code='" + productCode + "'";
                if (!string.IsNullOrEmpty(supplierId))
                    paramSql = paramSql + " and p.SupplierId='" + supplierId + "'";
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    paramSql = paramSql + " and CAST(psm.LastReceiveDate as date) between CAST('" + dateFrom + "' as date) and CAST('" + dateTo + "' as date)";

                if (string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
                {
                    sql = @"SELECT max(p.Id) AS value
			                ,max(p.Code) AS label
			                ,max(p.Id) AS Id
			                ,max(p.Code) AS ProductCode
			                ,max(p.Name) AS ProductName
			                ,max(p.PurchasePrice) PurchasePrice
			                ,Isnull(max(psm.ReceiveQty),0) Qty
			               ,Isnull(max(psm.CurrentStock),0) ProductStock
			                ,Isnull(max(psm.CurrentStock) * max(p.PurchasePrice),0) ProductStockValue
			                ,Isnull(max(p.VatCategoryId), '0') VatCategoryId
			                ,Isnull(max(p.VatCategoryId), '0') VatCategoryId
			                ,Isnull((
					                SELECT vc.VatRate
					                FROM VatCategory vc
					                WHERE vc.Id = max(p.VatCategoryId)
					                ), 0) VatRate
			                ,max(p.ProductCategoryId) ProductCategoryId
			                ,isnull((
					                SELECT pc.Name
					                FROM ProductCategory pc
					                WHERE pc.Id = max(p.ProductCategoryId)
					                ), '') AS ProductCategoryName
			                ,isnull(max(p.ProductSubCategoryId), '') ProductSubCategoryId
			                ,isnull((
					                SELECT psc.Name
					                FROM ProductSubCategory psc
					                WHERE psc.Id = max(p.ProductSubCategoryId)
					                ), '') AS ProductSubCategoryName
			                ,max(isnull(p.ReorderLevel, 0)) AS ReorderLevel
			                ,max(isnull(p.ShelfLife, 0)) AS ShelfLife
			                ,max(p.PurchasePrice) AS PurchasePrice
			                ,max(isnull(p.RetailPrice, 0)) AS RetailPrice
			                ,max(isnull(p.WholeSalePrice, 0)) AS WholeSalePrice
			                ,max(p.ProductCategoryId) AS ProductCategoryId
			                ,max(pm.CompanyId) CompanyId
			                ,max(pm.BranchId) BranchId
			                ,max(p.UOMId) UOMId
			                ,max(u.Name) UOMName
		                FROM Product p
		                LEFT JOIN ProductStockMaser psm ON psm.ProductID = p.Id
		                JOIN ProductMaster pm ON pm.ProductId = p.Id
		                LEFT JOIN UOM u ON u.Id = p.UOMId
                        WHERE p.Active= 1 and p.Archive = 0 and " + paramSql + @"
                        GROUP BY p.Id";
                }
                else
                {
                    sql = @"SELECT max(p.Id) AS value
			                    ,max(p.Code) AS label
			                    ,max(p.Id) AS Id
			                    ,max(p.Code) AS ProductCode
			                    ,max(p.Name) AS ProductName
			                    ,max(p.PurchasePrice) PurchasePrice
			                    ,max(psm.ReceiveQty) Qty
			                    ,max(psm.ReceiveQty) Qty
			                    ,Isnull(max(psm.CurrentStock),0) ProductStock
			                    ,Isnull(max(psm.CurrentStock) * max(p.PurchasePrice),0) ProductStockValue
                                ,Isnull(max(p.VatCategoryId), '') VatCategoryId
                                ,Isnull(( SELECT vc.VatRate FROM VatCategory vc WHERE vc.Id = max(p.VatCategoryId) ), 0) VatRate,max(p.ProductCategoryId) ProductCategoryId
                                ,isnull(( SELECT pc.Name FROM ProductCategory pc WHERE pc.Id = max(p.ProductCategoryId) ), '') AS ProductCategoryName 
                                ,isnull(max(p.ProductSubCategoryId), '') ProductSubCategoryId 
                                ,isnull(( SELECT psc.Name FROM ProductSubCategory psc WHERE psc.Id = max(p.ProductSubCategoryId) ), '') AS ProductSubCategoryName  
                                ,max(isnull(p.ReorderLevel, 0)) AS ReorderLevel 
			                    ,max(isnull(p.ShelfLife, 0)) AS ShelfLife 
			                    ,max(p.PurchasePrice) AS PurchasePrice 
			                    ,max(isnull(p.RetailPrice, 0)) AS RetailPrice 
			                    ,max(isnull(p.WholeSalePrice, 0)) AS WholeSalePrice 
			                    ,max(p.ProductCategoryId) AS ProductCategoryId 
			                    ,max(pm.CompanyId) CompanyId 
			                    ,max(pm.BranchId) BranchId 
			                    ,max(p.UOMId) UOMId 
			                    ,max(u.Name) UOMName  
                            FROM Product p  
                            LEFT JOIN ProductStockMaser psm ON psm.ProductID = p.Id 
                            JOIN ProductMaster pm ON pm.ProductId = p.Id 
                            LEFT JOIN UOM u ON u.Id = p.UOMId 
                            where p.Active= 1 and p.Archive = 0 and " + paramSql + @" 
                            group by p.Id";
                }

                return _db.Database.SqlQuery<ProductStockVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ProductStockVm> GetAllProductStockDetail(string companyId, string branchId, string supplierId, string productCategoryId, string productSubCategoryId, string ProductId, string dateFrom, string dateTo)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " itmMstr.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and itmMstr.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(productCategoryId))
                    paramSql = paramSql + " and itm.ProductCategoryId='" + productCategoryId + "'";
                if (!string.IsNullOrEmpty(productSubCategoryId))
                    paramSql = paramSql + " and itm.ProductSubCategoryId='" + productSubCategoryId + "'";
                if (!string.IsNullOrEmpty(ProductId))
                    paramSql = paramSql + " and itm.Id='" + ProductId + "'";
                if (!string.IsNullOrEmpty(supplierId))
                    paramSql = paramSql + " and itm.SupplierId='" + supplierId + "'";
                var sql = @"select 
                                 max(itm.Id) as value,
                                 max(itm.Code) as label,
                                 max(itm.Id) as Id,
                                 max(itm.Code) as ProductCode,
                                  max(itm.Name) as ProductName,
                                 max(isnull(itm.ReorderLevel,0)) as ReorderLevel,
                                 max(isnull(itm.ShelfLife,0)) as ShelfLife,
                                 max(itm.PurchasePrice) as PurchasePrice,
                                 max(isnull(itm.RetailPrice,0)) as RetailPrice,
                                 max(isnull(itm.WholeSalePrice,0)) as WholeSalePrice,
                                 max(itm.ProductCategoryId) as ProductCategoryId,
                                 isnull((select itmCtgry.Name from ProductCategory itmCtgry where max(itm.ProductCategoryId)=itmCtgry.Id),'') as ProductCategoryName,
                                 max(isnull(itm.ProductSubCategoryId,'')) as ProductSubCategoryId,
                                 isnull((select itmSbCtgry.Name from ProductSubCategory itmSbCtgry where max(itm.ProductSubCategoryId)=itmSbCtgry.Id),'') as ProductSubCategoryName,
                                 max(isnull(itm.ProductSubsidiaryCategoryId,'')) as ProductSubsidiaryCategoryId,
                                 isnull((select itmSbsCtgry.Name from ProductSubsidiaryCategory itmSbsCtgry where max(itm.ProductSubsidiaryCategoryId)=itmSbsCtgry.Id),'') as ProductSubsidiaryCategoryName,
                                 max(itm.UOMId) as UOMId,
                                 isnull((select um.Name from UOM um where max(itm.UOMId)=um.Id),'') as UOMName,
                                 max(isnull(itm.TaxCategoryId,'')) as TaxCategoryId,
                                 isnull((select vatCtgr.TaxRate from TaxCategory vatCtgr where max(itm.TaxCategoryId)=vatCtgr.Id),0) as TaxRate,
                                 max(isnull(itm.SupplierId,'')) as SupplierId,
                                 (select isnull(sum(prchDtl.Quantity),0)  from PurchaseDetail prchDtl where prchDtl.ProductId = max(itm.Id) and  convert(date,prchDtl.PurchaseDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')) as PurchaseQuantity,
                                 (select isnull(sum(slRtnDtl.ReturnQuantity),0)  from SaleReturnDetail slRtnDtl where slRtnDtl.ProductId = max(itm.Id) and  convert(date,slRtnDtl.SalesReturnDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')) as SaleReturnQuantity,
                                 (select isnull(sum(slDtl.Quantity),0)  from SaleDetail slDtl where slDtl.ProductId = max(itm.Id) and  convert(date,slDtl.SaleDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')) as SaleQuantity,
                                 (select isnull(sum(prchRtnDtl.ReturnQuantity),0)  from PurchaseReturnDetail prchRtnDtl where prchRtnDtl.ProductId = max(itm.Id) and  convert(date,prchRtnDtl.PurchaseReturnDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')) as PurchaseReturnQuantity,
                                 max(itmMstr.CompanyId) as CompanyId,
                                 max(itmMstr.BranchId) as BranchId
                                 from Product itm
                                 join ProductMaster itmMstr
                                 on itm.Id=itmMstr.ProductId
                                 where " + paramSql + @"
                                 group by
                                 itm.Id";
                return _db.Database.SqlQuery<ProductStockVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ExpenditureVm> GetExpenses(string companyId, string branchId, string dateFrom, string dateTo, string expenditureCategoryId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,ExpenseDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                if (!string.IsNullOrEmpty(expenditureCategoryId))
                    paramSql = paramSql + " and expenditureCategoryId='" + expenditureCategoryId + "'";
                var sql = @"select
                            Id,
                            [Sequence],
                            Code,
                            ExpenseName,
                            ExpenseDate,
                            IsNull((select ctgry.Name  from  ExpenditureCategory as ctgry where ctgry.Id=expns.ExpenditureCategoryId),'') as ExpenditureCategoryName,
                            ExpenseAmount,
                            isnull(Comment,'') as Comment
                            from  
                            Expenditure as expns
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<ExpenditureVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<CustomerLedgerVm> GetAllCustomerLedger(string companyId, string branchId, string dateFrom, string dateTo, string customerType, string phone, string customerId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(customerType))
                    paramSql = paramSql + " and CustomerType='" + customerType + "'";
                if (!string.IsNullOrEmpty(customerId))
                    paramSql = paramSql + " and CustomerId='" + customerId + "'";
                if (!string.IsNullOrEmpty(phone))
                    paramSql = paramSql + " and CustomerMobileNumber='" + phone + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,TransactionDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";

                var sql = @"select
                            Id,
                            ISNULL(TrackingNo,'') as TrackingNo,
                            ISNULL(MoneyReceiveNo,'') as MoneyReceiveNo,
                            CustomerId,
                            CustomerMobileNumber,
                            Particulars,
                            TransactionDate,
                            TransactionType,
                            DebitAmount,
                            CreditAmount,
                            RunningBalance
                            from CustomerLedger
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<CustomerLedgerVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SupplierLedgerVm> GetAllSupplierLedger(string companyId, string branchId, string dateFrom, string dateTo, string supplierType, string phone, string supplierId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(supplierType))
                    paramSql = paramSql + " and SupplierType='" + supplierType + "'";
                if (!string.IsNullOrEmpty(supplierId))
                    paramSql = paramSql + " and SupplierId='" + supplierId + "'";
                if (!string.IsNullOrEmpty(phone))
                    paramSql = paramSql + " and SupplierPhone='" + phone + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,TransactionDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";

                var sql = @"select
                            Id,
                            ISNULL(TrackingNo,'') as TrackingNo,
                            ISNULL(MoneyReceiveNo,'') as MoneyReceiveNo,
                            SupplierId,
                            SupplierPhone,
                            Particulars,
                            TransactionDate,
                            TransactionType,
                            DebitAmount,
                            CreditAmount,
                            RunningBalance
                            from SupplierLedger
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<SupplierLedgerVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<BankLedgerVm> GetAllBankLedger(string companyId, string branchId, string accountNo, string dateFrom, string dateTo)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " bnkLdgr.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and bnkLdgr.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(accountNo))
                    paramSql = paramSql + " and bnkLdgr.AccountNo='" + accountNo + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,bnkLdgr.TransactionDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";

                var sql = @"select 
                            bnkLdgr.Id,
                            bnkLdgr.TrackingNo,
                            bnkLdgr.CompanyId,
                            bnkLdgr.BranchId,
                            bnkLdgr.BankId,
                            (select Name from Bank where Id=bnkLdgr.BankId) as BankName,
                            bnkLdgr.BankBranchId,
                            (select Name from BankBranch where Id=bnkLdgr.BankBranchId) as BankBranchName,
                            bnkLdgr.AccountNo,
                            bnkLdgr.Particulars,
                            bnkLdgr.TransactionType,
                            bnkLdgr.TransactionDate,
                            bnkLdgr.DebitAmount,
                            bnkLdgr.CreditAmount,
                            bnkLdgr.RunningBalance
                            from BankLedger bnkLdgr
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<BankLedgerVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleVm> GetAllSalesSummary(string companyId, string branchId, string dateFrom, string dateTo, string customerId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " sls.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and sls.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,sls.SaleDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                if (!string.IsNullOrEmpty(customerId))
                    paramSql = paramSql + " and sls.CustomerId='" + customerId + "'";
                var sql = @"select * from(
                                     select   
                                     sls.Id,
                                     sls.CompanyId,
                                     sls.BranchId,
                                     sls.CustomerId,
                                     sls.CustomerMobileNumber,
                                     sls.CustomerName,
                                     sls.SaleDate,
                                     SUM(slsDtl.TotalQuantity) AS TotalQuantity,
                                     SUM(slsDtl.TotalAmount) AS TotalAmount,
                                     Isnull(SUM(slsDtl.TotalVat),0) AS TotalVat,
                                     Isnull(SUM(slsDtl.ProductDiscount+sls.OverAllDiscount+ sls.CustomerDiscountInAmount),0) AS TotalDiscount,
                                     Isnull(SUM(slsDtl.TotalAmount) + Isnull(SUM(slsDtl.TotalVat), 0), 0) - Isnull(SUM(slsDtl.TotalDiscount), 0) AS NetAmount,
                                     --Isnull(SUM(slsDtl.TotalAmount)+ Isnull(SUM(slsDtl.TotalVat),0),0)-Isnull(SUM(slsDtl.ProductDiscount+sls.OverAllDiscount+ sls.CustomerDiscountInAmount),0) as NetAmount,
                                     sls.Archive,
                                     sls.IsFullyReturned,
                                     sls.IsFullyCancelled
                                     FROM  Sale sls
                                     INNER JOIN
                                     (select  
                                     SaleId,
                                     SUM(Quantity) AS TotalQuantity,
                                     SUM(TotalAmount) AS TotalAmount, 
                                     SUM(DiscountAmount) AS TotalDiscount,
                                     SUM(DiscountAmount+DiscountInAmount) AS ProductDiscount, 
                                     SUM(VatAmount) AS TotalVat
                                     FROM SaleDetail 
                                     where Archive=0 and 
                                     IsReturned=0 and 
                                     IsCancelled=0 
                                     GROUP BY SaleId) slsDtl
                                     ON sls.Id = slsDtl.SaleId
                                     where sls.Archive=0 and sls.IsFullyReturned=0 and sls.IsFullyCancelled=0
                                     GROUP BY
                                     sls.Id,
                                     sls.SaleDate,
                                     sls.CompanyId,
                                     sls.BranchId,
                                     sls.CustomerId,
                                     sls.CustomerMobileNumber,
                                     sls.CustomerName,
                                     sls.OverAllDiscount,
                                     sls.CustomerDiscountInAmount,
                                     sls.Archive,
                                     sls.IsFullyReturned,
                                     sls.IsFullyCancelled,
                                     slsDtl.ProductDiscount
                                     )as sls
                                    where " + paramSql + @"";
                return _db.Database.SqlQuery<SaleVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public DataSet ReportGetAllSaleClosing(string companyId, string branchId, string dateFrom, string dateTo, string saleById)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlCommand sqlComm = new SqlCommand("[getReportSaleClosing]", conn);
                sqlComm.Parameters.AddWithValue("@companyId", companyId);
                sqlComm.Parameters.AddWithValue("@branchId", branchId);
                sqlComm.Parameters.AddWithValue("@saleById", saleById);
                sqlComm.Parameters.AddWithValue("@dateFrom", dateFrom);
                sqlComm.Parameters.AddWithValue("@dateTo", dateTo);
                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<SaleVm> GetAllSaleClosing(string companyId, string branchId, string dateFrom, string dateTo, string saleById)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " sa.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " AND sa.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " AND convert(date,sa.SaleDate) between convert(date,'" + dateFrom + "') AND convert(date,'" + dateTo + "')";
                if (!string.IsNullOrEmpty(saleById) && saleById != "Admin")
                {
                    paramSql = paramSql + " AND sa.AddedBy='" + saleById + "'";
                }

                var @sql = @"SELECT * FROM (
                          SELECT s.Id
                           ,s.CompanyId
                           ,s.BranchId
                           ,s.CustomerId
                           ,s.CustomerMobileNumber
                           ,s.CustomerName
                           ,s.SaleDate
                           ,SUM(slsDtl.TotalQuantity) AS TotalQuantity
                           ,SUM(slsDtl.TotalAmount) AS TotalAmount
                           ,Isnull(SUM(slsDtl.TotalVat), 0) AS TotalVat
                           ,Isnull(SUM(slsDtl.ProductDiscount + s.OverAllDiscount + s.CustomerDiscountInAmount), 0) AS TotalDiscount
                           --,Isnull(SUM(slsDtl.TotalAmount) + Isnull(SUM(slsDtl.TotalVat), 0), 0) - Isnull(SUM(slsDtl.ProductDiscount + s.OverAllDiscount + s.CustomerDiscountInAmount), 0) AS NetAmount
                            ,Isnull(SUM(slsDtl.TotalAmount) + Isnull(SUM(slsDtl.TotalVat), 0), 0) - Isnull(SUM(slsDtl.DiscountAmount), 0) AS NetAmount                           
                            ,s.Archive
                           ,s.IsFullyReturned
                           ,s.IsFullyCancelled
                           ,s.AddedBy
                           ,s.PaymentType
                          FROM Sale s
                          INNER JOIN ( SELECT SaleId ,SUM(Quantity) AS TotalQuantity ,SUM(TotalAmount) AS TotalAmount
                             ,SUM(DiscountAmount + DiscountInAmount) AS ProductDiscount ,SUM(VatAmount) AS TotalVat, Sum(DiscountAmount) DiscountAmount
                             FROM SaleDetail
                             WHERE Archive = 0 AND IsReturned = 0 AND IsCancelled = 0 GROUP BY SaleId
                             ) slsDtl ON s.Id = slsDtl.SaleId
                          WHERE s.Archive = 0 AND s.IsFullyReturned = 0 AND s.IsFullyCancelled = 0
                          GROUP BY s.Id ,s.SaleDate ,s.CompanyId ,s.BranchId ,s.CustomerId ,s.CustomerMobileNumber
                           ,s.CustomerName ,s.OverAllDiscount ,s.CustomerDiscountInAmount ,s.Archive ,s.IsFullyReturned
                           ,s.IsFullyCancelled ,slsDtl.ProductDiscount ,s.AddedBy ,s.PaymentType
                          ) AS sa
                                WHERE " + paramSql + @"";

                return _db.Database.SqlQuery<SaleVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleVm> GetCategoryWiseSale(string companyId, string branchId, string supplierId, string dateFrom, string dateTo)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " sls.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and sls.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(supplierId))
                    paramSql = paramSql + " and p.supplierId='" + supplierId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,sls.SaleDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                var sql = @"SELECT 
		                     Max(slsDtl.SaleId) Id
		                    ,Max(c.Name) CustomerName
		                    ,slsDtl.ProductCategoryId		                   
		                    ,Max(pc.Name) ProductCategoryName
		                    ,sls.SaleDate
		                    ,sls.CompanyId
		                    ,sls.BranchId
		                    ,sls.PaymentType
                            ,Max(p.PurchasePrice)PurchasePrice
							,Max(p.RetailPrice)RetailPrice
		                    ,Max(p.Name) CourierName
		                    ,Max(slsDtl.SupplierId) SupplierId
		                    ,SUM(slsDtl.Quantity) AS TotalQuantity
		                    ,SUM(slsDtl.TotalAmount) AS TotalAmount
		                    ,Isnull(SUM(slsDtl.VatAmount), 0) AS TotalVat
		                    ,Isnull(SUM(slsDtl.DiscountAmount), 0) AS TotalDiscount
		                    ,Isnull((
				                    SELECT sum(isnull(cstmrLdgr.CreditAmount, 0))
				                    FROM CustomerLedger cstmrLdgr
				                    WHERE cstmrLdgr.Archive != 1
					                    AND cstmrLdgr.Discount != 1
					                    AND cstmrLdgr.TransactionType = 'CashReturn'
					                    AND convert(DATE, cstmrLdgr.TransactionDate) BETWEEN convert(DATE, '" + dateFrom + @"') AND convert(DATE, '" + dateTo + @"')
				                    ), 0) AS TotalCashReturnAmount
		                    ,Isnull(SUM(slsDtl.TotalAmount) + Isnull(SUM(slsdtl.VatAmount), 0), 0) - Isnull(SUM(slsDtl.DiscountAmount), 0) AS NetAmount
	                    FROM SaleDetail slsDtl
	                    JOIN Sale sls ON slsDtl.SaleId = sls.Id
	                    JOIN Product p ON p.Id = slsDtl.ProductId
                        JOIN ProductCategory pc on pc.Id = slsDtl.ProductCategoryId
	                    JOIN Customer c on c.Id= sls.CustomerId
	                    WHERE slsDtl.Archive = 0
		                    AND IsReturned = 0
		                    AND IsCancelled = 0
		                    AND " + paramSql + @"
                        GROUP BY slsDtl.ProductCategoryId
		                    ,sls.SaleDate
		                    ,sls.CompanyId
		                    ,sls.BranchId
		                    ,sls.PaymentType
		                    ,p.Id";

                //var sql = @"select * from(select 
                //            slsDtl.ProductCategoryId,
                //            (select Name from ProductCategory where Id=slsDtl.ProductCategoryId) as ProductCategoryName,
                //            sls.SaleDate,
                //            sls.CompanyId,
                //            sls.BranchId,
                //            sls.PaymentType,
                //            Max(p.Name) CourierName,
                //            Max(slsDtl.SupplierId) SupplierId,
                //            SUM(Quantity) AS TotalQuantity,
                //            SUM(slsDtl.TotalAmount) AS TotalAmount,
                //            Isnull(SUM(VatAmount),0) AS TotalVat,
                //            Isnull(SUM(slsDtl.DiscountAmount+slsDtl.DiscountInAmount+sls.OverAllDiscount+ sls.CustomerDiscountInAmount),0) AS TotalDiscount,
                //            Isnull((select sum(isnull(cstmrLdgr.CreditAmount,0)) from CustomerLedger cstmrLdgr  where cstmrLdgr.Archive!=1 and cstmrLdgr.Discount!=1 and cstmrLdgr.TransactionType='CashReturn'and
                //            convert(date, cstmrLdgr.TransactionDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')),0) AS TotalCashReturnAmount,
                //            Isnull(SUM(slsDtl.TotalAmount)+ Isnull(SUM(sls.TotalVat),0),0)-Isnull(SUM(sls.ProductDiscount+sls.OverAllDiscount+ sls.CustomerDiscountInAmount),0) as NetAmount
                //            FROM SaleDetail slsDtl
                //            join Sale sls on slsDtl.SaleId=sls.Id
                //            Join Product p on p.Id = slsDtl.ProductId
                //            where slsDtl.Archive=0 and 
                //            IsReturned=0 and 
                //            IsCancelled=0 
                //            GROUP BY 
                //            slsDtl.ProductCategoryId,
                //            sls.SaleDate,
                //            sls.CompanyId,
                //            sls.BranchId,
                //            sls.PaymentType,
                //            p.Id
                //            )as sls
                //            where " + paramSql + @"";
                return _db.Database.SqlQuery<SaleVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<SaleVm> GetCounterWiseSale(string companyId, string branchId, string dateFrom, string dateTo)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " sls.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and sls.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,sls.SaleDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                var sql = @"select * from(select
                            sls.AddedBy,
                            sls.SaleDate,
                            sls.CompanyId,
                            sls.BranchId,
                            SUM(Quantity) AS TotalQuantity,
                            SUM(slsDtl.TotalAmount) AS TotalAmount,
                            Isnull(SUM(VatAmount),0) AS TotalVat,
                            Isnull(SUM(slsDtl.DiscountAmount+slsDtl.DiscountInAmount+sls.OverAllDiscount+ sls.CustomerDiscountInAmount),0) AS TotalDiscount,
                            Isnull(SUM(slsDtl.TotalAmount)+ Isnull(SUM(sls.TotalVat),0),0)-Isnull(SUM(sls.ProductDiscount+sls.OverAllDiscount+ sls.CustomerDiscountInAmount),0) as NetAmount
                            FROM SaleDetail slsDtl
                            join Sale sls
                            on slsDtl.SaleId=sls.Id
                            where slsDtl.Archive=0 and 
                            IsReturned=0 and 
                            IsCancelled=0 
                            GROUP BY 
                            sls.AddedBy,
                            sls.SaleDate,
                            sls.CompanyId,
                            sls.BranchId
                            order by AddedBy
                            )as sls
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<SaleVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleVm> GetAllSaleReturnSummary(string companyId, string branchId, string dateFrom, string dateTo, string customerId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " slsRtn.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and slsRtn.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,slsRtn.SalesReturnDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                if (!string.IsNullOrEmpty(customerId))
                    paramSql = paramSql + " and slsRtn.CustomerId='" + customerId + "'";
                var sql = @"select 
                            slsRtn.Id,
                            slsRtn.SaleId,
                            slsRtn.SaleDate,
                            slsRtn.SalesReturnDate,
                            slsRtn.TotalQuantity,
                            slsRtn.TotalAmount,
                            slsRtn.CompanyId,
                            slsRtn.BranchId,
                            isnull(slsRtn.CustomerId,'') as CustomerId,
                            isnull((select cstmr.Name from Customer cstmr where cstmr.Id=slsRtn.CustomerId),'') as CustomerName 
                            from SaleReturn slsRtn
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<SaleVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleDetailVm> GetAllSaleDetail(string companyId, string branchId, string customerId, string saleNo, string productCategoryId, string dateFrom, string dateTo)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " slsDtl.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and slsDtl.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(customerId))
                    paramSql = paramSql + " and slsDtl.CustomerId='" + customerId + "'";
                if (!string.IsNullOrEmpty(saleNo))
                    paramSql = paramSql + " and slsDtl.SaleId='" + saleNo + "'";
                if (!string.IsNullOrEmpty(productCategoryId))
                    paramSql = paramSql + " and slsDtl.ProductCategoryId='" + productCategoryId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,slsDtl.SaleDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                var sql = @"select 
                            Id,
                            CompanyId,
                            BranchId,
                            SaleId,
                            ProductId,
                            ProductCode,
                            ProductName,
                            UOMId,
                            ProductCategoryId,
                            SaleDate,
                            SaleDetailDate,
                            Quantity,
                            PurchasePrice,
                            SalePrice,
                            VatAmount,
                            TotalAmount,
                            (SalePrice-PurchasePrice)*Quantity as ProfitAmount
                            FROM SaleDetail slsDtl
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<SaleDetailVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<PurchaseVm> GetAllPurchaseSummary(string companyId, string branchId, string dateFrom, string dateTo, string supplierId)
        {
            try
            {
                string paramSql = "prch.Archive=0 and prch.FullyReturned=0 and";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = paramSql + " prch.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and prch.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,prch.PurchaseDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                if (!string.IsNullOrEmpty(supplierId))
                    paramSql = paramSql + " and prch.SupplierId='" + supplierId + "'";
                var sql = @"select 
                            prch.Id,
                            prch.PurchaseDate,
                            prch.TotalQuantity,
                            prch.TotalAmount,
                            prch.NetAmount,
                            prch.CompanyId,
                            prch.BranchId,
                            isnull(prch.SupplierId,'') as SupplierId,
                            isnull((select splr.Name from Supplier splr where splr.Id=prch.SupplierId),'') as SupplierName 
                            from Purchase prch
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<PurchaseVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PurchaseVm> GetAllPurchaseReturnSummary(string companyId, string branchId, string dateFrom, string dateTo, string supplierId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " prchRtn.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and prchRtn.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,prchRtn.PurchaseDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                if (!string.IsNullOrEmpty(supplierId))
                    paramSql = paramSql + " and prchRtn.SupplierId='" + supplierId + "'";
                var sql = @"select 
                            prchRtn.Id,
                            prchRtn.PurchaseDate,
                            prchRtn.PurchaseReturnDate,
                            prchRtn.TotalQuantity,
                            prchRtn.TotalAmount,
                            prchRtn.CompanyId,
                            prchRtn.BranchId,
                            isnull(prchRtn.SupplierId,'') as SupplierId,
                            isnull((select splr.Name from Supplier splr where splr.Id=prchRtn.SupplierId),'') as SupplierName 
                            from PurchaseReturn prchRtn
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<PurchaseVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PurchaseDetailVm> GetAllPurchaseDetail(string companyId, string branchId, string dateFrom, string dateTo, string supplierId)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " prchDtl.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and prchDtl.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,prchDtl.PurchaseDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                if (!string.IsNullOrEmpty(supplierId))
                    paramSql = paramSql + " and prchDtl.SupplierId='" + supplierId + "'";
                var sql = @"select 
                            prchDtl.Id,
                            prchDtl.CompanyId,
                            prchDtl.BranchId,
                            prchDtl.PurchaseId,
                            prchDtl.ProductId,
                            prchDtl.ProductCode,
                            prchDtl.ProductName,
                            prchDtl.UOMId,
                            prchDtl.ProductCategoryId,
                            prchDtl.PurchaseDate,
                            prchDtl.PurchaseDetailDate,
                            prchDtl.Quantity,
                            prchDtl.PurchasePrice,
                            prchDtl.PurchasePrice,
                            prchDtl.SalePrice,
                            prchDtl.TotalAmount,
                            (prchDtl.SalePrice-prchDtl.PurchasePrice)*prchDtl.Quantity as ProfitAmount
                            FROM PurchaseDetail prchDtl
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<PurchaseDetailVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<CustomerVm> GetAllCustomerInformation(string companyId, string branchId, string customerMobileNumber)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " cstmr.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and cstmr.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(customerMobileNumber))
                    paramSql = paramSql + " and cstmr.Phone1 Like '%" + customerMobileNumber + @"%'";
                var sql = @"SELECT cstmr.Id AS value
	                        ,cstmr.Phone1 AS label
	                        ,cstmr.Id
	                        ,cstmr.Phone1
	                        ,isnull(cstmr.[Name], '') AS CustomerName	                        
	                        ,isnull(cstmr.Address1, '') AS CustomerAddress1	                       
							,isnull(cstmr.Email, '') AS CustomerEmail
							,isnull(cstmr.ContactPerson, '') as CustomerAddress2							
	                        ,isnull(CAST(cstmr.CustomerCategoryId AS DECIMAL(7,2)),0) AS CustomerDiscountRate
	                        ,isNull((
			                        SELECT (
					                        CASE 
						                        WHEN sum(DebitAmount) > sum(CreditAmount)
							                        THEN sum(DebitAmount) - sum(CreditAmount)
						                        END
					                        )
			                        FROM CustomerLedger
			                        WHERE CustomerId = cstmr.Id
			                        ), 0) AS CustomerAdvanceAmount
	                        ,isNull((
			                        SELECT (
					                        CASE 
						                        WHEN sum(CreditAmount) > sum(DebitAmount)
							                        THEN sum(CreditAmount) - sum(DebitAmount)
						                        END
					                        )
			                        FROM CustomerLedger
			                        WHERE CustomerId = cstmr.Id
			                        ), 0) AS CustomerDueAmount
	                        ,isNull((
			                        SELECT sum(EarningPoint) - sum(ExpensePoint)
			                        FROM CustomerLedger
			                        WHERE CustomerId = cstmr.Id
			                        ), 0) AS CustomerPoint
	                        ,isNull((
			                        SELECT sum(EarningPointAmount) - sum(ExpensePointAmount)
			                        FROM CustomerLedger
			                        WHERE CustomerId = cstmr.Id
			                        ), 0) AS CustomerPointAmount
                        FROM Customer cstmr
                        WHERE " + paramSql + @"";
                return _db.Database.SqlQuery<CustomerVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<CashBalanceVm> GetCashBalanceBetweenDate(string companyId, string branchId, string dateFrom, string dateTo)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " sls.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and sls.BranchId='" + branchId + "'";
                if (dateFrom != null)
                    paramSql = paramSql + " and convert(date,sls.SaleDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                var sql = @"select * from(
                            select   
                            sls.Id,
                            sls.CompanyId,
                            sls.BranchId,
                            sls.CustomerId,
                            sls.CustomerMobileNumber,
                            sls.CustomerName,
                            sls.SaleDate,
                            SUM(slsDtl.TotalQuantity) AS TotalQuantity,
                            SUM(slsDtl.TotalAmount) AS TotalAmount,
                            Isnull(SUM(slsDtl.TotalVat),0) AS TotalVat,
                            Isnull(SUM(slsDtl.ProductDiscount+sls.OverAllDiscount+ sls.CustomerDiscountInAmount),0) AS TotalDiscount,
                            Isnull(SUM(slsDtl.TotalAmount)+ Isnull(SUM(slsDtl.TotalVat),0),0)-Isnull(SUM(slsDtl.ProductDiscount+sls.OverAllDiscount+ sls.CustomerDiscountInAmount),0) as NetAmount,
                            isnull((select sum(isnull(cstmrLdgr.DebitAmount,0))  
                            from CustomerLedger cstmrLdgr 
                            where cstmrLdgr.Archive!=1 and cstmrLdgr.Discount!=1 and  cstmrLdgr.TransactionType!='SalesReturn' and  cstmrLdgr.TransactionType!='CashReturn' and
                            convert(date, cstmrLdgr.TransactionDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')),0) as TotalCollection,
                            isnull((select sum(isnull(splrLdgr.DebitAmount,0))   
                            from SupplierLedger splrLdgr 
                            where splrLdgr.Archive!=1 and splrLdgr.IsDiscount!=1 and
                            convert(date, splrLdgr.TransactionDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')),0) as TotalPayment,
                            isnull((select sum(isnull(cstmrLdgr.DebitAmount,0))  
                            from CustomerLedger cstmrLdgr 
                            where cstmrLdgr.Archive!=1 and cstmrLdgr.Discount!=1 and cstmrLdgr.TransactionType='SalesReturn' and 
                            convert(date, cstmrLdgr.TransactionDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')),0) as TotalSalesReturnAmount,
                            isnull((select sum(isnull(cstmrLdgr.CreditAmount,0))  
                            from CustomerLedger cstmrLdgr 
                            where cstmrLdgr.Archive!=1 and cstmrLdgr.Discount!=1 and cstmrLdgr.TransactionType='CashReturn' and 
                            convert(date, cstmrLdgr.TransactionDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')),0) as TotalCashReturnAmount,
                            isnull((select sum(isnull(expns.ExpenseAmount,0))   
                            from Expenditure expns 
                            where expns.Archive!=1 and 
                            convert(date, expns.ExpenseDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')),0) as TotalExpenses,
                            isnull((select sum(isnull(bnkLdgr.DebitAmount,0))  
                            from BankLedger bnkLdgr 
                            where bnkLdgr.Archive!=1 and  bnkLdgr.PaymentInfoId is not null and bnkLdgr.TransactionType='Deposit' and 
                            convert(date, bnkLdgr.TransactionDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')),0) as TotalBankDepositAmountByCustomer,
                            isnull((select sum(isnull(bnkLdgr.DebitAmount,0))  
                            from BankLedger bnkLdgr 
                            where bnkLdgr.Archive!=1 and bnkLdgr.PaymentInfoId is null and bnkLdgr.TransactionType='Deposit' and 
                            convert(date, bnkLdgr.TransactionDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')),0) as TotalBankDepositAmountByBranch,
                            isnull((select sum(isnull(bnkLdgr.CreditAmount,0))  
                            from BankLedger bnkLdgr 
                            where bnkLdgr.Archive!=1 and bnkLdgr.PaymentInfoId is null and bnkLdgr.TransactionType='Withdrawn' and 
                            convert(date, bnkLdgr.TransactionDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')),0) as TotalBankWithdrawnAmountByBranch, 
                            sls.Archive,
                            sls.IsFullyReturned,
                            sls.IsFullyCancelled
                            FROM  Sale sls
                            INNER JOIN
                            (select  
                            SaleId,
                            SUM(Quantity) AS TotalQuantity,
                            SUM(TotalAmount) AS TotalAmount, 
                            SUM(DiscountAmount+DiscountInAmount) AS ProductDiscount, 
                            SUM(VatAmount) AS TotalVat
                            FROM SaleDetail 
                            where Archive=0 and 
                            IsReturned=0 and 
                            IsCancelled=0 
                            GROUP BY SaleId) slsDtl
                            ON sls.Id = slsDtl.SaleId
                            where sls.Archive=0 and sls.IsFullyReturned=0 and sls.IsFullyCancelled=0
                            GROUP BY
                            sls.Id,
                            sls.SaleDate,
                            sls.CompanyId,
                            sls.BranchId,
                            sls.CustomerId,
                            sls.CustomerMobileNumber,
                            sls.CustomerName,
                            sls.OverAllDiscount,
                            sls.CustomerDiscountInAmount,
                            sls.Archive,
                            sls.IsFullyReturned,
                            sls.IsFullyCancelled,
                            slsDtl.ProductDiscount
                            )as sls
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<CashBalanceVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<AutoComplete> GetSupplierCategoryIdNameForAutoComplete()
        {
            try
            {
                return (from itmctgr in _db.Database.SqlQuery<AutoComplete>("exec [dbo].[GetSupplierCategoryIdNameForAutoComplete]").ToArray().ToList()
                        select new AutoComplete
                        {
                            value = itmctgr.value,
                            label = itmctgr.label
                        }).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<AutoComplete> GetCustomers(string companyId, string branchId)
        {
            try
            {
                var result = (from itmctgr in _db.Database.SqlQuery<AutoComplete>("exec [dbo].[GetCustomers] @companyId, @branchId",
                        new SqlParameter("@companyId", companyId),
                        new SqlParameter("@branchId", branchId)
                    ).ToArray().ToList()
                              select new AutoComplete
                              {
                                  value = itmctgr.value,
                                  label = itmctgr.label
                              }).AsEnumerable();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<AutoComplete> GetProductSubCategoryIdNameForAutoComplete(string ProductCategoryId)
        {
            try
            {
                return (from itm in _db.Database.SqlQuery<AutoComplete>("exec [GetProductSubCategoryIdNameForAutoComplete] @ProductCategoryId",
                        new SqlParameter("@ProductCategoryId", ProductCategoryId)
                    ).ToArray().ToList()
                        select new AutoComplete
                        {
                            value = itm.value,
                            label = itm.label
                        }).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<AutoComplete> GetSupplierIdNameForAutoComplete()
        {
            try
            {
                var result = (from itmctgr in _db.Database.SqlQuery<AutoComplete>("exec [dbo].[GetSupplierIdNameForAutoComplete]").ToArray().ToList()
                              select new AutoComplete
                              {
                                  value = itmctgr.value,
                                  label = itmctgr.label
                              }).AsEnumerable();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<AutoComplete> GetProductIdCodeForAutoComplete()
        {
            try
            {
                var result = (from itm in _db.Database.SqlQuery<AutoComplete>("exec [GetProductIdCodeForAutoComplete]").ToArray().ToList()
                              select new AutoComplete
                              {
                                  value = itm.value,
                                  label = itm.label
                              }).AsEnumerable();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<AutoComplete> GetProductIdNameForAutoComplete()
        {
            try
            {
                var result = (from itm in _db.Database.SqlQuery<AutoComplete>("exec [GetProductIdNameForAutoComplete]").ToArray().ToList()
                              select new AutoComplete
                              {
                                  value = itm.value,
                                  label = itm.label
                              }).AsEnumerable();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<AutoComplete> GetProductIdCodeForAutoCompleteByCompanyBranchId(string companyId, string branchId)
        {
            try
            {
                return (from itm in _db.Database.SqlQuery<AutoComplete>("exec [GetProductIdCodeForAutoCompleteByCompanyBranchId] @companyId, @branchId",
                                  new SqlParameter("@companyId", companyId),
                                  new SqlParameter("@branchId", branchId)
                                  ).ToArray().ToList()
                        select new AutoComplete
                        {
                            value = itm.value,
                            label = itm.label
                        }).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<AutoComplete> GetProductIdNameForAutoCompleteByCompanyBranchId(string companyId, string branchId)
        {
            try
            {
                return (from itm in _db.Database.SqlQuery<AutoComplete>("exec [GetProductIdNameForAutoCompleteByCompanyBranchId] @companyId, @branchId",
                                  new SqlParameter("@companyId", companyId),
                                  new SqlParameter("@branchId", branchId)
                                  ).ToArray().ToList()
                        select new AutoComplete
                        {
                            value = itm.value,
                            label = itm.label
                        }).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleVm> GetAllSalesForMultiplePrint(string saleIds)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(saleIds))
                    paramSql = " sl.Id in (" + saleIds + ")";
                var sql = @"select * 
                            from Sale sl
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<SaleVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleVm> GetAllSalesForMobileCover(string companyId, string branchId, string dateFrom, string dateTo, string customerId, string courierId, string orderStatus, string phone)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " sl.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and sl.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(customerId))
                    paramSql = " sl.CustomerId='" + customerId + "'";
                if (!string.IsNullOrEmpty(courierId))
                    paramSql = paramSql + " and sl.CourierId='" + courierId + "'";
                if (!string.IsNullOrEmpty(courierId))
                    paramSql = paramSql + " and sl.OrderStatus='" + orderStatus + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,sl.SaleDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                if (!string.IsNullOrEmpty(phone))
                    paramSql = paramSql + " and sl.CustomerPhone='" + phone + "'";
                var sql = @"select 
                            Id,
                            (select Name from Customer where Phone1=sl.CustomerPhone) as CustomerName,
                            (select Phone1 from Customer where Phone1=sl.CustomerPhone) as CustomerPhone,
                            (select Name from Courier where Id=sl.CourierId) as CourierName,
                            SaleDate,
                            TotalQuantity,
                            TotalAmount,
                            OrderNo,
                            OrderDate,
                            OrderStatus,
                            Isnull(OverAllDiscount,0) as OverAllDiscount,
                            NetAmount
                            from Sale sl
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<SaleVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<SaleDetailVm> GetAllSaleDetailsForMultiplePrint(string saleIds)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(saleIds))
                    paramSql = " slDtl.SaleId in (" + saleIds + ")";
                var sql = @"select * 
                            from SaleDetail slDtl
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<SaleDetailVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<MultipleInvoicePrint> GetAllForMultiplePrint(string saleIds)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(saleIds))
                    paramSql = " sl.Id in (" + saleIds + ")";
                var sql = @"select * 
                            from Sale sl
                            join SaleDetail slDtl
                            on sl.Id=slDtl.SaleId
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<MultipleInvoicePrint>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<MultipleInvoicePrint> GetAllForMultipleLabelPrint(string saleIds)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(saleIds))
                    paramSql = " sl.Id in (" + saleIds + ")";
                var sql = @"select 
                            sl.*,
                            (select Name from Courier where Id=sl.CourierId) as CourierName
                            from Sale sl
                            where " + paramSql + @"";
                return _db.Database.SqlQuery<MultipleInvoicePrint>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleVm> GetCategoryWiseItemSales(string companyId, string branchId, string supplierId, string dateFrom, string dateTo)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " sls.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and sls.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(supplierId))
                    paramSql = paramSql + " and p.supplierId='" + supplierId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,sls.SaleDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                var sql = @"SELECT slsDtl.ProductCategoryId
		                    ,(
			                    SELECT Name
			                    FROM ProductCategory
			                    WHERE Id = slsDtl.ProductCategoryId
			                    ) AS ProductCategoryName
		                    ,sls.SaleDate
		                    ,sls.CompanyId
		                    ,sls.BranchId
		                    ,sls.PaymentType
                            ,Max(p.PurchasePrice) PurchasePrice
                            ,(SUM(slsDtl.Quantity)*Max(p.PurchasePrice)) NetAmount
							,Max(p.RetailPrice) RetailPrice
		                    ,Max(p.Name) CourierName
		                    ,Max(slsDtl.SupplierId) SupplierId
		                    ,SUM(slsDtl.Quantity) AS TotalQuantity
		                    ,SUM(slsDtl.TotalAmount) AS NetAmount1
		                    ,Isnull(SUM(slsDtl.VatAmount), 0) AS TotalVat
		                    ,Isnull(SUM(slsDtl.DiscountAmount), 0) AS TotalDiscount
		                    ,Isnull((
				                    SELECT sum(isnull(cstmrLdgr.CreditAmount, 0))
				                    FROM CustomerLedger cstmrLdgr
				                    WHERE cstmrLdgr.Archive != 1
					                    AND cstmrLdgr.Discount != 1
					                    AND cstmrLdgr.TransactionType = 'CashReturn'
					                    AND convert(DATE, cstmrLdgr.TransactionDate) BETWEEN convert(DATE, '" + dateFrom + @"') AND convert(DATE, '" + dateTo + @"')
				                    ), 0) AS TotalCashReturnAmount
		                    ,Isnull(SUM(slsDtl.TotalAmount) + Isnull(SUM(slsdtl.VatAmount), 0), 0) - Isnull(SUM(slsDtl.DiscountAmount), 0) AS TotalAmount
	                    FROM SaleDetail slsDtl
	                    JOIN Sale sls ON slsDtl.SaleId = sls.Id
	                    JOIN Product p ON p.Id = slsDtl.ProductId
	                    WHERE slsDtl.Archive = 0
		                    AND IsReturned = 0
		                    AND IsCancelled = 0
		                    AND " + paramSql + @"
                        GROUP BY slsDtl.ProductCategoryId
		                    ,sls.SaleDate
		                    ,sls.CompanyId
		                    ,sls.BranchId
		                    ,sls.PaymentType
		                    ,p.Id";

                //var sql = @"SELECT *
                //            FROM (
                //             SELECT slsDtl.ProductCategoryId
                //              ,(
                //               SELECT Name
                //               FROM ProductCategory
                //               WHERE Id = slsDtl.ProductCategoryId
                //               ) AS ProductCategoryName
                //              ,Max(p.Name) CourierName
                //              ,sls.SaleDate
                //              ,sls.CompanyId
                //              ,sls.BranchId
                //              ,sls.PaymentType
                //              ,Max(slsDtl.SupplierId) SupplierId
                //              ,SUM(Quantity) AS TotalQuantity
                //              ,SUM(slsDtl.TotalAmount) AS TotalAmount
                //              ,Isnull(SUM(VatAmount), 0) AS TotalVat
                //              ,Isnull(SUM(slsDtl.DiscountAmount + slsDtl.DiscountInAmount + sls.OverAllDiscount + sls.CustomerDiscountInAmount), 0) AS TotalDiscount
                //              ,Isnull((
                //                SELECT sum(isnull(cstmrLdgr.CreditAmount, 0))
                //                FROM CustomerLedger cstmrLdgr
                //                WHERE cstmrLdgr.Archive != 1
                //                 AND cstmrLdgr.Discount != 1
                //                 AND cstmrLdgr.TransactionType = 'CashReturn'
                //                 AND convert(DATE, cstmrLdgr.TransactionDate) BETWEEN convert(DATE, '" + dateFrom + @"') AND convert(DATE, '" + dateFrom + @"')
                //                ), 0) AS TotalCashReturnAmount
                //              ,Isnull(SUM(slsDtl.TotalAmount) + Isnull(SUM(sls.TotalVat), 0), 0) - Isnull(SUM(sls.ProductDiscount + sls.OverAllDiscount + sls.CustomerDiscountInAmount), 0) AS NetAmount
                //             FROM SaleDetail slsDtl
                //             JOIN Sale sls ON slsDtl.SaleId = sls.Id
                //             Join Product p on p.Id = slsDtl.ProductId
                //             WHERE slsDtl.Archive = 0
                //              AND IsReturned = 0
                //              AND IsCancelled = 0
                //             GROUP BY slsDtl.ProductCategoryId
                //              ,sls.SaleDate
                //              ,sls.CompanyId
                //              ,sls.BranchId
                //              ,sls.PaymentType
                //              ,p.Id
                //             ) AS sls
                //            WHERE " + paramSql + @"";

                return _db.Database.SqlQuery<SaleVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<SaleVm> GetCategoryWiseDailySale(string companyId, string branchId, string supplierId, string dateFrom, string dateTo)
        {
            try
            {
                string paramSql = "";
                if (!string.IsNullOrEmpty(companyId))
                    paramSql = " sls.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and sls.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(supplierId))
                    paramSql = paramSql + " and p.supplierId='" + supplierId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,sls.SaleDate) = convert(date,'" + dateFrom + "') ";
                var sql = @"SELECT slsDtl.ProductCategoryId
		                    ,(
			                    SELECT Name
			                    FROM ProductCategory
			                    WHERE Id = slsDtl.ProductCategoryId
			                    ) AS ProductCategoryName
		                    ,sls.SaleDate
		                    ,sls.CompanyId
		                    ,sls.BranchId
		                    ,sls.PaymentType
                            ,Max(p.PurchasePrice)PurchasePrice
							,Max(p.RetailPrice)RetailPrice
		                    ,Max(p.Name) CourierName
		                    ,Max(slsDtl.SupplierId) SupplierId
		                    ,SUM(slsDtl.Quantity) AS TotalQuantity
		                    ,SUM(slsDtl.TotalAmount) AS TotalAmount
		                    ,Isnull(SUM(slsDtl.VatAmount), 0) AS TotalVat
		                    ,Isnull(SUM(slsDtl.DiscountAmount), 0) AS TotalDiscount
		                    ,Isnull((
				                    SELECT sum(isnull(cstmrLdgr.CreditAmount, 0))
				                    FROM CustomerLedger cstmrLdgr
				                    WHERE cstmrLdgr.Archive != 1
					                    AND cstmrLdgr.Discount != 1
					                    AND cstmrLdgr.TransactionType = 'CashReturn'
					                    AND convert(DATE, cstmrLdgr.TransactionDate) = convert(DATE, '" + dateFrom + @"') 
				                    ), 0) AS TotalCashReturnAmount
		                    ,Isnull(SUM(slsDtl.TotalAmount) + Isnull(SUM(slsdtl.VatAmount), 0), 0) - Isnull(SUM(slsDtl.DiscountAmount), 0) AS NetAmount
	                    FROM SaleDetail slsDtl
	                    JOIN Sale sls ON slsDtl.SaleId = sls.Id
	                    JOIN Product p ON p.Id = slsDtl.ProductId
	                    WHERE slsDtl.Archive = 0
		                    AND IsReturned = 0
		                    AND IsCancelled = 0
		                    AND " + paramSql + @"
                        GROUP BY slsDtl.ProductCategoryId
		                    ,sls.SaleDate
		                    ,sls.CompanyId
		                    ,sls.BranchId
		                    ,sls.PaymentType
		                    ,p.Id";


                return _db.Database.SqlQuery<SaleVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataSet GetPurchaseSummary(string companyId, string branchId, string supplierId, string fdate, string todate)
        {
            try
            {
                SqlCommand sqlcomm = new SqlCommand();
                DataSet ds = new DataSet();
                SqlCommand sqlComm = new SqlCommand("getPurchaseSummary", conn);
                sqlComm.Parameters.AddWithValue("@companyId", companyId);
                sqlComm.Parameters.AddWithValue("@branchId", branchId);
                sqlComm.Parameters.AddWithValue("@supplierId", supplierId);
                sqlComm.Parameters.AddWithValue("@fdate", fdate);
                sqlComm.Parameters.AddWithValue("@todate", todate);
                sqlComm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds, "Purchase");
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public IEnumerable<ProductVm> GetProductlistbySupplierId(string companyId, string branchId, string supplierId)
        {
            try
            {
                var sql = @"SELECT max(itm.Id) AS value
	                        ,max(itm.Name) AS label
	                        ,max(itm.Id) AS ProductId
	                        ,max(itm.Code) AS ProductCode
	                        ,max(itm.Name) AS ProductName
	                        ,max(isnull(itm.ReorderLevel, 0)) AS ReorderLevel
	                        ,max(isnull(itm.ShelfLife, 0)) AS ShelfLife
	                        ,max(itm.PurchasePrice) AS PurchasePrice
	                        ,max(isnull(itm.RetailPrice, 0)) AS RetailPrice
	                        ,max(isnull(itm.WholeSalePrice, 0)) AS WholeSalePrice
	                        ,max(isnull(itm.MaxDiscount, 0)) AS MaxDiscount
	                        ,max(itm.ProductCategoryId) AS ProductCategoryId
	                        ,isnull((
			                        SELECT itmCtgry.Name
			                        FROM ProductCategory itmCtgry
			                        WHERE max(itm.ProductCategoryId) = itmCtgry.Id
			                        ), '') AS ProductCategoryName
	                        ,max(isnull(itm.ProductSubCategoryId, '')) AS ProductSubCategoryId
	                        ,isnull((
			                        SELECT itmSbCtgry.Name
			                        FROM ProductSubCategory itmSbCtgry
			                        WHERE max(itm.ProductSubCategoryId) = itmSbCtgry.Id
			                        ), '') AS ProductSubCategoryName
	                        ,max(isnull(itm.ProductSubsidiaryCategoryId, '')) AS ProductSubsidiaryCategoryId
	                        ,isnull((
			                        SELECT itmSbsCtgry.Name
			                        FROM ProductSubsidiaryCategory itmSbsCtgry
			                        WHERE max(itm.ProductSubsidiaryCategoryId) = itmSbsCtgry.Id
			                        ), '') AS ProductSubsidiaryCategoryName
	                        ,max(itm.UOMId) AS UOMId
	                        ,isnull((
			                        SELECT um.Name
			                        FROM UOM um
			                        WHERE max(itm.UOMId) = um.Id
			                        ), '') AS UOMName
	                        ,max(isnull(itm.VatCategoryId, '')) AS VatCategoryId
	                        ,isnull((
			                        SELECT vatCtgr.VatRate
			                        FROM VatCategory vatCtgr
			                        WHERE max(itm.VatCategoryId) = vatCtgr.Id
			                        ), 0) AS TaxRate
	                        ,max(isnull(itm.SupplierId, '')) AS SupplierId
	                        ,isnull((Select Sum(ReceiveQty) from ProductStockMaser where ProductId = itm.Id), 0) AS ProductStock
	                        ,max(itmMstr.CompanyId) AS CompanyId
	                        ,max(itmMstr.BranchId) AS BranchId
                        FROM Product itm
                        JOIN ProductMaster itmMstr ON itm.Id = itmMstr.ProductId
                        WHERE itmMstr.CompanyId = '" + companyId + @"' AND itmMstr.BranchId = '" + branchId + @"' AND itm.SupplierId = '" + supplierId + @"'
                        GROUP BY itm.Id";

                return _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataSet GetProductReorder(string companyId, string branchId, string categoryId, string subCategoryId, string supplierId, string productId, string productCode)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand sqlComm = new SqlCommand("getReorderProducts", conn);
                sqlComm.Parameters.AddWithValue("@companyId", companyId);
                sqlComm.Parameters.AddWithValue("@branchId", branchId);
                sqlComm.Parameters.AddWithValue("@categoryId", categoryId);
                sqlComm.Parameters.AddWithValue("@subCategoryId", subCategoryId);
                sqlComm.Parameters.AddWithValue("@supplierId", supplierId);
                sqlComm.Parameters.AddWithValue("@productId", productId);
                sqlComm.Parameters.AddWithValue("@productCode", productCode);
                sqlComm.CommandType = CommandType.StoredProcedure;
                da.SelectCommand = sqlComm;
                da.Fill(ds, "ProductReorder");
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataSet GetProductExpire(string companyId, string branchId, string categoryId, string subCategoryId, string supplierId, string productId, string productCode, string expireDate, string isExpired)
        {
            try
            {
                SqlCommand sqlcomm = new SqlCommand();
                DataSet ds = new DataSet();
                SqlCommand sqlComm = new SqlCommand("getExpireProducts", conn);
                sqlComm.Parameters.AddWithValue("@companyId", companyId);
                sqlComm.Parameters.AddWithValue("@branchId", branchId);
                sqlComm.Parameters.AddWithValue("@categoryId", categoryId);
                sqlComm.Parameters.AddWithValue("@subCategoryId", subCategoryId);
                sqlComm.Parameters.AddWithValue("@supplierId", supplierId);
                sqlComm.Parameters.AddWithValue("@productId", productId);
                sqlComm.Parameters.AddWithValue("@productCode", productCode);
                sqlComm.Parameters.AddWithValue("@expireDate", expireDate);
                sqlComm.Parameters.AddWithValue("@isexpired", isExpired);
                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds, "ProductExpire");
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataSet GetProductExpired(string companyId, string branchId, string supplierId)
        {
            try
            {
                SqlCommand sqlcomm = new SqlCommand();
                DataSet ds = new DataSet();
                SqlCommand sqlComm = new SqlCommand("getExpireProducts", conn);
                sqlComm.Parameters.AddWithValue("@companyId", companyId);
                //sqlComm.Parameters.AddWithValue("@branchId", branchId);
                //sqlComm.Parameters.AddWithValue("@categoryId", categoryId);
                //sqlComm.Parameters.AddWithValue("@subCategoryId", subCategoryId);
                sqlComm.Parameters.AddWithValue("@supplierId", supplierId);
                //sqlComm.Parameters.AddWithValue("@expireDate", expireDate);
                //sqlComm.Parameters.AddWithValue("@isexpired", isExpired);
                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds, "ProductExpire");
                return ds;
                //var result = _db.Database.SqlQuery<ProductVm>("getExpireProducts").ToList();
                //return result; //_db.Database.SqlQuery<ProductVm>("getExpireProducts").ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataSet GetDashboard(string companyId, string branchId, string supplierId)
        {
            try
            {
                SqlCommand sqlcomm = new SqlCommand();
                DataSet ds = new DataSet();
                SqlCommand sqlComm = new SqlCommand("getDashboard", conn);
                sqlComm.Parameters.AddWithValue("@companyId", companyId);
                sqlComm.Parameters.AddWithValue("@branchId", branchId);
                sqlComm.Parameters.AddWithValue("@supplierId", supplierId);
                //sqlComm.Parameters.AddWithValue("@categoryId", categoryId);
                //sqlComm.Parameters.AddWithValue("@subCategoryId", subCategoryId);
                //sqlComm.Parameters.AddWithValue("@expireDate", expireDate);
                //sqlComm.Parameters.AddWithValue("@isexpired", isExpired);
                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds);
                return ds;
                //var result = _db.Database.SqlQuery<ProductVm>("getExpireProducts").ToList();
                //return result; //_db.Database.SqlQuery<ProductVm>("getExpireProducts").ToArray().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataSet GetStockInProductList(string companyId, string branchId, string categoryId, string subCategoryId, string supplierId, string productId, string productCode)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlCommand sqlComm = new SqlCommand("getStockInProducts", conn);
                sqlComm.Parameters.AddWithValue("@companyId", companyId);
                sqlComm.Parameters.AddWithValue("@branchId", branchId);
                sqlComm.Parameters.AddWithValue("@categoryId", categoryId);
                sqlComm.Parameters.AddWithValue("@subCategoryId", subCategoryId);
                sqlComm.Parameters.AddWithValue("@supplierId", supplierId);
                sqlComm.Parameters.AddWithValue("@productId", productId);
                sqlComm.Parameters.AddWithValue("@productCode", productCode);
                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds, "ProductInStock");
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataSet GetStockOutProductList(string companyId, string branchId, string categoryId, string subCategoryId, string supplierId, string productId, string productCode)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlCommand sqlComm = new SqlCommand("getStockOutProducts", conn);
                sqlComm.Parameters.AddWithValue("@companyId", companyId);
                sqlComm.Parameters.AddWithValue("@branchId", branchId);
                sqlComm.Parameters.AddWithValue("@categoryId", categoryId);
                sqlComm.Parameters.AddWithValue("@subCategoryId", subCategoryId);
                sqlComm.Parameters.AddWithValue("@supplierId", supplierId);
                sqlComm.Parameters.AddWithValue("@productId", productId);
                sqlComm.Parameters.AddWithValue("@productCode", productCode);
                //sqlComm.Parameters.AddWithValue("@expireDate", expireDate);
                //sqlComm.Parameters.AddWithValue("@isexpired", isExpired);
                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds, "ProductExpire");
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataSet GetCategoryWiseSaleProfitLoss(string companyId, string branchId, string supplierId, string categoryId, string subCategoryId, string productId, string productCode, string dateFrom, string dateTo)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlCommand sqlComm = new SqlCommand("getSaleSummaryProfitLoss", conn);
                sqlComm.Parameters.AddWithValue("@companyId", companyId);
                sqlComm.Parameters.AddWithValue("@branchId", branchId);
                sqlComm.Parameters.AddWithValue("@categoryId", categoryId);
                //sqlComm.Parameters.AddWithValue("@subCategoryId", subCategoryId);
                sqlComm.Parameters.AddWithValue("@supplierId", supplierId);
                sqlComm.Parameters.AddWithValue("@productId", productId);
                sqlComm.Parameters.AddWithValue("@productCode", productCode);
                sqlComm.Parameters.AddWithValue("@dateFrom", dateFrom);
                sqlComm.Parameters.AddWithValue("@dateTo", dateTo);
                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds, "SaleProfitLoss");
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string AddSaleItems(SuperShopSaleViewModel salevm)
        {
            try
            {
                var SaleId = "";
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var totalQuantity = 0m;
                var totalAmount = 0m;

                SqlCommand sqlComm = new SqlCommand("setCustomerSaleAndLedger", conn);
                #region sale      

                //sqlComm.Parameters.AddWithValue("@OrderNo", salevm.OrderNo);
                //sqlComm.Parameters.AddWithValue("@OrderDate", salevm.OrderDate);
                //sqlComm.Parameters.AddWithValue("@OrderStatus", salevm.OrderStatus);
                //sqlComm.Parameters.AddWithValue("@CourierId", salevm.CourierId);
                //sqlComm.Parameters.AddWithValue("@CustomerCode", salevm.CustomerCode);
                //sqlComm.Parameters.AddWithValue("@CustomerAddress2", salevm.CustomerAddress2);

                sqlComm.Parameters.AddWithValue("@CompanyId", identity.CompanyId);
                sqlComm.Parameters.AddWithValue("@BranchId", identity.BranchId);
                sqlComm.Parameters.AddWithValue("@CustomerId", salevm.CustomerId);
                sqlComm.Parameters.AddWithValue("@CustomerName", salevm.CustomerName);
                sqlComm.Parameters.AddWithValue("@CustomerMobileNumber", salevm.CustomerMobileNumber);
                sqlComm.Parameters.AddWithValue("@CustomerAddress1", salevm.CustomerAddress1);
                sqlComm.Parameters.AddWithValue("@CustomerEmail", salevm.CustomerEmail);
                sqlComm.Parameters.AddWithValue("@CustomerDueAmount", salevm.CustomerDueAmount);
                sqlComm.Parameters.AddWithValue("@CustomerAdvanceAmount", salevm.CustomerAdvanceAmount);
                sqlComm.Parameters.AddWithValue("@CustomerPoint", salevm.CustomerPoint);
                sqlComm.Parameters.AddWithValue("@CustomerPointAmount", salevm.CustomerPointAmount);
                sqlComm.Parameters.AddWithValue("@EarningPoint", salevm.EarningPoint);
                sqlComm.Parameters.AddWithValue("@EarningPointAmount", salevm.EarningPointAmount);
                sqlComm.Parameters.AddWithValue("@ExpensePoint", salevm.ExpensePoint);
                sqlComm.Parameters.AddWithValue("@ExpensePointAmount", salevm.ExpensePointAmount);
                sqlComm.Parameters.AddWithValue("@SaleDate", salevm.SaleDate);
                sqlComm.Parameters.AddWithValue("@LoadingCharge", salevm.LoadingCharge);
                sqlComm.Parameters.AddWithValue("@TransportCharge", salevm.TransportCharge);
                sqlComm.Parameters.AddWithValue("@OthersCharge", salevm.OthersCharge);
                sqlComm.Parameters.AddWithValue("@TotalBoxes", salevm.TotalBoxes);
                sqlComm.Parameters.AddWithValue("@TotalPcses", salevm.TotalPcses);

                sqlComm.Parameters.AddWithValue("@DiscountInPercentage", salevm.DiscountInPercentage);
                sqlComm.Parameters.AddWithValue("@DiscountInAmount", salevm.DiscountInAmount);
                sqlComm.Parameters.AddWithValue("@CustomerDiscountInPercentage", salevm.CustomerDiscountInPercentage);
                sqlComm.Parameters.AddWithValue("@CustomerDiscountInAmount", salevm.CustomerDiscountInAmount);
                sqlComm.Parameters.AddWithValue("@DiscountAmount", salevm.DiscountInAmount);
                sqlComm.Parameters.AddWithValue("@CustomerDiscountAmount", salevm.CustomerDiscountInAmount);
                sqlComm.Parameters.AddWithValue("@CustomerTotalDiscountAmount", salevm.CustomerDiscountInAmount);
                sqlComm.Parameters.AddWithValue("@ProductDiscount", salevm.ProductDiscount);
                sqlComm.Parameters.AddWithValue("@TotalProfit", salevm.TotalProfit);
                sqlComm.Parameters.AddWithValue("@TotalVat", salevm.TotalVat);
                sqlComm.Parameters.AddWithValue("@OverAllDiscount", salevm.OverAllDiscount);
                sqlComm.Parameters.AddWithValue("@NetAmount", salevm.NetAmount);
                sqlComm.Parameters.AddWithValue("@RetailTotal", salevm.TotalAmount);
                sqlComm.Parameters.AddWithValue("@PaidAmount", salevm.PaidAmount);
                sqlComm.Parameters.AddWithValue("@ChangeAmount", salevm.ChangeAmount);
                sqlComm.Parameters.AddWithValue("@DueAmount", salevm.DueAmount);
                sqlComm.Parameters.AddWithValue("@DiscountRate", salevm.CustomerDiscountInPercentage);
                sqlComm.Parameters.AddWithValue("@IsApprovedNeeded", salevm.IsApprovedNeeded);
                sqlComm.Parameters.AddWithValue("@IsApproved", salevm.IsApproved);
                sqlComm.Parameters.AddWithValue("@ApprovedBy", salevm.ApprovedBy);
                //sqlComm.Parameters.AddWithValue("@IsFullyDelivered", salevm.IsFullyDelivered);
                //sqlComm.Parameters.AddWithValue("@IsFullyCancelled", salevm.IsFullyCancelled);
                //sqlComm.Parameters.AddWithValue("@IsFullyReturned", salevm.IsFullyReturned);
                //sqlComm.Parameters.AddWithValue("@PaymentStatus", salevm.PaymentStatus);
                sqlComm.Parameters.AddWithValue("@PaymentType", salevm.PaymentType);
                sqlComm.Parameters.AddWithValue("@CardType", salevm.CardType);
                sqlComm.Parameters.AddWithValue("@CardNumber", salevm.CardNumber);
                sqlComm.Parameters.AddWithValue("@BankName", salevm.BankName);
                sqlComm.Parameters.AddWithValue("@ChequeNo", salevm.ChequeNo);
                sqlComm.Parameters.AddWithValue("@ChequeDate", salevm.ChequeDate);

                sqlComm.Parameters.AddWithValue("@Active", true);
                sqlComm.Parameters.AddWithValue("@Archive", salevm.Archive);
                sqlComm.Parameters.AddWithValue("@IsSynchronized", salevm.IsSynchronized);
                sqlComm.Parameters.AddWithValue("@IsUpdated", salevm.IsUpdated);
                sqlComm.Parameters.AddWithValue("@SynchronizationType", salevm.SynchronizationType);
                sqlComm.Parameters.AddWithValue("@AddedBy", identity.Name);
                sqlComm.Parameters.AddWithValue("@AddedDate", DateTime.Now);
                sqlComm.Parameters.AddWithValue("@AddedFromIp", identity.IpAddress);
                //sqlComm.Parameters.AddWithValue("@UpdatedBy", salevm.UpdatedBy);
                //sqlComm.Parameters.AddWithValue("@UpdatedDate", salevm.UpdatedDate);
                //sqlComm.Parameters.AddWithValue("@UpdatedFromIp", salevm.UpdatedFromIp);
                #endregion sale

                #region customerLeger
                //sqlComm.Parameters.AddWithValue("@SaleId", SaleId);
                sqlComm.Parameters.AddWithValue("@TransactionType", TransactionType.Sales.ToString());
                sqlComm.Parameters.AddWithValue("@Particulars", TransactionType.Sales.ToString());
                sqlComm.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                sqlComm.Parameters.AddWithValue("@DebitAmount", salevm.DueAmount > 0 ? salevm.PaidAmount : salevm.NetAmount);
                sqlComm.Parameters.AddWithValue("@CreditAmount", salevm.NetAmount);
                sqlComm.Parameters.AddWithValue("@RunningBalance", 0);
                //sqlComm.Parameters.AddWithValue("@EarningPoint", salevm.EarningPoint);
                //sqlComm.Parameters.AddWithValue("@EarningPointAmount", salevm.EarningPointAmount);
                //sqlComm.Parameters.AddWithValue("@ExpensePoint", salevm.ExpensePoint);
                //sqlComm.Parameters.AddWithValue("@ExpensePointAmount", salevm.ExpensePointAmount);
                //sqlComm.Parameters.AddWithValue("@CompanyId", identity.CompanyId);
                //sqlComm.Parameters.AddWithValue("@BranchId", identity.BranchId);
                #endregion customerLeger

                sqlComm.Parameters.Add("@SaleId", SqlDbType.VarChar, 100);
                sqlComm.Parameters["@SaleId"].Direction = ParameterDirection.Output;

                sqlComm.CommandType = CommandType.StoredProcedure;
                foreach (var saleDetail in salevm.SaleDetails)
                {
                    totalQuantity += saleDetail.Quantity;
                    totalAmount += saleDetail.TotalAmount;
                }

                sqlComm.Parameters.AddWithValue("@TotalQuantity", totalQuantity);
                sqlComm.Parameters.AddWithValue("@TotalAmount", totalAmount);
                sqlComm.Connection.Open();
                sqlComm.ExecuteNonQuery();
                SaleId = sqlComm.Parameters["@SaleId"].Value.ToString();
                sqlComm.Connection.Close();

                SqlCommand sqlCmd = new SqlCommand("setSaleDetail", conn); ;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection.Open();
                foreach (var saleDetail in salevm.SaleDetails)
                {
                    sqlCmd.Parameters.Clear();
                    sqlCmd.Parameters.AddWithValue("@SaleId", SaleId);
                    sqlCmd.Parameters.AddWithValue("@ProductId", saleDetail.ProductId);
                    sqlCmd.Parameters.AddWithValue("@ProductCode", saleDetail.ProductCode);
                    sqlCmd.Parameters.AddWithValue("@ProductName", saleDetail.ProductName);
                    sqlCmd.Parameters.AddWithValue("@ProductCategoryId", saleDetail.ProductCategoryId);
                    sqlCmd.Parameters.AddWithValue("@ProductSubCategoryId", saleDetail.ProductSubCategoryId);
                    sqlCmd.Parameters.AddWithValue("@ProductSubsidiaryCategoryId", saleDetail.ProductSubsidiaryCategoryId);

                    sqlCmd.Parameters.AddWithValue("@Quantity", saleDetail.Quantity);
                    sqlCmd.Parameters.AddWithValue("@ProductStock", saleDetail.ProductStock);
                    sqlCmd.Parameters.AddWithValue("@PurchasePrice", saleDetail.PurchasePrice);
                    sqlCmd.Parameters.AddWithValue("@SalePrice", saleDetail.SalePrice);
                    sqlCmd.Parameters.AddWithValue("@TotalAmount", saleDetail.TotalAmount);

                    sqlCmd.Parameters.AddWithValue("@UOMId", saleDetail.UOMId);
                    sqlCmd.Parameters.AddWithValue("@SizeId", saleDetail.SizeId); //saleDetail.SizeId);
                    sqlCmd.Parameters.AddWithValue("@SupplierId", saleDetail.SupplierId);

                    sqlCmd.Parameters.AddWithValue("@RAMId", 0); //saleDetail.RAMId);
                    sqlCmd.Parameters.AddWithValue("@ROMId", 0); //saleDetail.ROMId);
                    sqlCmd.Parameters.AddWithValue("@ColorId", 0); //saleDetail.ColorId);
                    sqlCmd.Parameters.AddWithValue("@StyleId", 0); //saleDetail.StyleId); 
                    sqlCmd.Parameters.AddWithValue("@GradeId", 0); //saleDetail.GradeId);                   
                    sqlCmd.Parameters.AddWithValue("@BrandId", 0); //saleDetail.BrandId);

                    sqlCmd.Parameters.AddWithValue("@MaxDiscount", saleDetail.MaxDiscount);
                    sqlCmd.Parameters.AddWithValue("@DiscountPerUnit", saleDetail.DiscountPerUnit);
                    sqlCmd.Parameters.AddWithValue("@DiscountAmount", saleDetail.DiscountAmount);
                    sqlCmd.Parameters.AddWithValue("@DiscountInPercentage", saleDetail.DiscountInPercentage);
                    sqlCmd.Parameters.AddWithValue("@DiscountInAmount", saleDetail.DiscountInAmount);
                    sqlCmd.Parameters.AddWithValue("@VatRate", saleDetail.VatRate);
                    sqlCmd.Parameters.AddWithValue("@VatAmount", saleDetail.VatAmount);

                    sqlCmd.Parameters.AddWithValue("@SaleDate", salevm.SaleDate);
                    sqlCmd.Parameters.AddWithValue("@SaleDetailDate", DateTime.Now);
                    sqlCmd.Parameters.AddWithValue("@CompanyId", identity.CompanyId);
                    sqlCmd.Parameters.AddWithValue("@BranchId", identity.BranchId);
                    sqlCmd.Parameters.AddWithValue("@Active", true);
                    sqlCmd.Parameters.AddWithValue("@IsReturned", false);
                    sqlCmd.Parameters.AddWithValue("@IsDelivered", false);
                    sqlCmd.Parameters.AddWithValue("@Archive", false);
                    sqlCmd.Parameters.AddWithValue("@IsSynchronized", true);
                    sqlCmd.Parameters.AddWithValue("@IsUpdated", false);

                    sqlCmd.Parameters.AddWithValue("@SynchronizationType", SynchronizationType.Server.ToString());
                    sqlCmd.Parameters.AddWithValue("@AddedBy", identity.Name);
                    sqlCmd.Parameters.AddWithValue("@AddedDate", DateTime.Now);
                    sqlCmd.Parameters.AddWithValue("@AddedFromIp", identity.IpAddress);
                    sqlCmd.ExecuteNonQuery();
                }
                sqlCmd.Connection.Close();
                return SaleId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //detailId++;
            //[Id] [varchar](15) NOT NULL,
            //[Sequence] [int] NOT NULL,
            //[CompanyId] [varchar](1) NOT NULL,
            //[BranchId] [varchar](3) NOT NULL,
            //[SaleId] [varchar](15) NOT NULL,
            //[ProductId] [varchar](15) NOT NULL,
            //[ProductCode] [nvarchar](50) NOT NULL,
            //[ProductName] [nvarchar](150) NOT NULL,
            //[UOMId] [varchar](15) NULL,
            //[RAMId] [varchar](15) NULL,
            //[ROMId] [varchar](15) NULL,
            //[SizeId] [varchar](15) NULL,
            //[ColorId] [varchar](15) NULL,
            //[StyleId] [varchar](15) NULL,
            //[GradeId] [varchar](15) NULL,
            //[SupplierId] [varchar](15) NULL,
            //[BrandId] [varchar](15) NULL,
            //[ProductCategoryId] [varchar](15) NOT NULL,
            //[ProductSubCategoryId] [varchar](15) NULL,
            //[ProductSubsidiaryCategoryId] [varchar](15) NULL,
            //[SaleDate] [datetime] NULL,
            //[SaleDetailDate] [datetime] NOT NULL,
            //[Quantity] [decimal](18, 4) NOT NULL,
            //[ProductStock] [decimal](18, 2) NOT NULL,
            //[PurchasePrice] [decimal](18, 2) NOT NULL,
            //[SalePrice] [decimal](18, 4) NOT NULL,
            //[MaxDiscount] [decimal](18, 4) NULL,
            //[DiscountPerUnit] [decimal](18, 2) NULL,
            //[DiscountAmount] [decimal](18, 4) NULL,
            //[DiscountInPercentage] [decimal](18, 2) NULL,
            //[DiscountInAmount] [decimal](18, 4) NULL,
            //[VatRate] [decimal](18, 2) NULL,
            //[VatAmount] [decimal](18, 4) NULL,
            //[TotalAmount] [decimal](18, 4) NOT NULL,
            //[IsCancelled] [bit] NULL,	
            //[IsReturned] [bit] NOT NULL,
            //[IsDelivered] [bit] NOT NULL,
            //[Active] [bit] NOT NULL,
            //[Archive] [bit] NOT NULL,
            //[IsSynchronized] [bit] NOT NULL,
            //[IsUpdated] [bit] NOT NULL,
            //[SynchronizationType] [nvarchar](8) NOT NULL,
            //[Description] [nvarchar](500) NULL,
            //[AddedBy] [nvarchar](20) NOT NULL,
            //[AddedDate] [datetime] NOT NULL,
            //[AddedFromIp] [nvarchar](15) NOT NULL,
            //[UpdatedBy] [nvarchar](20) NULL,
            //[UpdatedDate] [datetime] NULL,
            //[UpdatedFromIp] [nvarchar](15) NULL,
            /// Details
            //DataTable tbl = new DataTable();
            //tbl.Columns.Add(new DataColumn("Id", typeof(string)));
            //tbl.Columns.Add(new DataColumn("SaleId", typeof(string)));
            //tbl.Columns.Add(new DataColumn("Sequence", typeof(string)));
            //tbl.Columns.Add(new DataColumn("ProductId", typeof(string)));
            //tbl.Columns.Add(new DataColumn("ProductCode"));
            //tbl.Columns.Add(new DataColumn("ProductName"));
            //tbl.Columns.Add(new DataColumn("ProductCategoryId"));
            //tbl.Columns.Add(new DataColumn("ProductSubCategoryId"));
            //tbl.Columns.Add(new DataColumn("ProductSubsidiaryCategoryId"));

            //tbl.Columns.Add(new DataColumn("Quantity"));
            //tbl.Columns.Add(new DataColumn("ProductStock"));
            //tbl.Columns.Add(new DataColumn("PurchasePrice"));
            //tbl.Columns.Add(new DataColumn("SalePrice"));
            //tbl.Columns.Add(new DataColumn("TotalAmount"));

            //tbl.Columns.Add(new DataColumn("MaxDiscount"));
            //tbl.Columns.Add(new DataColumn("DiscountPerUnit"));
            //tbl.Columns.Add(new DataColumn("DiscountAmount"));
            //tbl.Columns.Add(new DataColumn("DiscountInPercentage"));
            //tbl.Columns.Add(new DataColumn("DiscountInAmount"));
            //tbl.Columns.Add(new DataColumn("VatRate"));
            //tbl.Columns.Add(new DataColumn("VatAmount"));

            //tbl.Columns.Add(new DataColumn("UOMId"));
            //tbl.Columns.Add(new DataColumn("SizeId"));
            //tbl.Columns.Add(new DataColumn("SupplierId"));

            //tbl.Columns.Add(new DataColumn("RAMId", typeof(Int32)));
            //tbl.Columns.Add(new DataColumn("ROMId", typeof(Int32)));
            //tbl.Columns.Add(new DataColumn("ColorId", typeof(Int32)));
            //tbl.Columns.Add(new DataColumn("StyleId", typeof(Int32)));
            //tbl.Columns.Add(new DataColumn("GradeId", typeof(Int32)));
            //tbl.Columns.Add(new DataColumn("BrandId", typeof(Int32)));

            //tbl.Columns.Add(new DataColumn("SaleDate", typeof(DateTime)));
            //tbl.Columns.Add(new DataColumn("SaleDetailDate", typeof(DateTime)));
            //tbl.Columns.Add(new DataColumn("CompanyId", typeof(Int32)));
            //tbl.Columns.Add(new DataColumn("BranchId", typeof(Int32)));

            //tbl.Columns.Add(new DataColumn("Active", typeof(bool)));
            //tbl.Columns.Add(new DataColumn("IsReturned", typeof(bool)));
            //tbl.Columns.Add(new DataColumn("IsDelivered", typeof(bool)));
            //tbl.Columns.Add(new DataColumn("Archive", typeof(bool))); 
            //tbl.Columns.Add(new DataColumn("IsSynchronized", typeof(bool)));
            //tbl.Columns.Add(new DataColumn("IsUpdated", typeof(bool)));

            //tbl.Columns.Add(new DataColumn("SynchronizationType", typeof(string)));
            //tbl.Columns.Add(new DataColumn("AddedBy", typeof(string)));
            //tbl.Columns.Add(new DataColumn("AddedDate", typeof(DateTime)));
            //tbl.Columns.Add(new DataColumn("AddedFromIp", typeof(string)));

            //int detailId = 10200000;
            //foreach (var saleDetail in salevm.SaleDetails)
            //{
            //    totalQuantity += saleDetail.Quantity;
            //    totalAmount += saleDetail.TotalAmount;

            //    DataRow dr = tbl.NewRow();
            //    dr["Id"] = detailId + 1;
            //    dr["SaleId"] = SaleId;
            //    dr["Sequence"] = saleDetail.Sequence;                    

            //    dr["ProductId"] = saleDetail.ProductId;
            //    dr["ProductCode"] = saleDetail.ProductCode;
            //    dr["ProductName"] = saleDetail.ProductName;
            //    dr["ProductCategoryId"] = saleDetail.ProductCategoryId;
            //    dr["ProductSubCategoryId"] = DBNull.Value; //saleDetail?.ProductSubCategoryId;
            //    dr["ProductSubsidiaryCategoryId"] = DBNull.Value; //saleDetail?.ProductSubsidiaryCategoryId;

            //    dr["Quantity"] = saleDetail.Quantity;                    
            //    dr["ProductStock"] = saleDetail.ProductStock;
            //    dr["PurchasePrice"] = saleDetail.PurchasePrice;
            //    dr["SalePrice"] = saleDetail.SalePrice;
            //    dr["TotalAmount"] = saleDetail.TotalAmount;

            //    dr["UOMId"] = saleDetail.UOMId;
            //    dr["SizeId"] = DBNull.Value; //saleDetail.SizeId;
            //    dr["SupplierId"] = salevm.SupplierId;

            //    dr["RAMId"] = 0; //saleDetail.RAMId;
            //    dr["ROMId"] = 0; //saleDetail.ROMId;
            //    dr["ColorId"] = 0; //saleDetail.ColorId;
            //    dr["StyleId"] = 0; //saleDetail.StyleId;
            //    dr["GradeId"] = 0; //saleDetail.GradeId;                   
            //    dr["BrandId"] = 0; //saleDetail.BrandId;

            //    dr["MaxDiscount"] = 0;
            //    dr["DiscountPerUnit"] = 0; 
            //    dr["DiscountAmount"] = 0;
            //    dr["DiscountInPercentage"] = 0;
            //    dr["DiscountInAmount"] = 0;
            //    dr["VatRate"] = 0;
            //    dr["VatAmount"] = 0;

            //    dr["PurchasePrice"] = saleDetail.PurchasePrice;
            //    dr["SaleDate"] = salevm.SaleDate;
            //    dr["SaleDetailDate"] = DateTime.Now;
            //    dr["CompanyId"] = identity.CompanyId;
            //    dr["BranchId"] = identity.BranchId;
            //    dr["Active"] = true;
            //    dr["IsReturned"] = false;
            //    dr["IsDelivered"] = false;
            //    dr["Archive"] = false;
            //    dr["IsSynchronized"] = true;
            //    dr["IsUpdated"] = false;

            //    dr["SynchronizationType"] = SynchronizationType.Server.ToString();
            //    dr["AddedBy"] = identity.Name;
            //    dr["AddedDate"] = DateTime.Now;
            //    dr["AddedFromIp"] = identity.IpAddress;
            //    tbl.Rows.Add(dr);
            //    detailId++;
            //}

            //SqlBulkCopy objbulk = new SqlBulkCopy(conn);
            //objbulk.DestinationTableName = "SaleDetail";
            //objbulk.ColumnMappings.Add("Id", "Id");
            //objbulk.ColumnMappings.Add("SaleId", "SaleId");
            //objbulk.ColumnMappings.Add("Sequence", "Sequence");               

            //objbulk.ColumnMappings.Add("ProductId", "ProductId");
            //objbulk.ColumnMappings.Add("ProductCode", "ProductCode");
            //objbulk.ColumnMappings.Add("ProductName", "ProductName");
            //objbulk.ColumnMappings.Add("ProductCategoryId", "ProductCategoryId");
            //objbulk.ColumnMappings.Add("ProductSubCategoryId", "ProductSubCategoryId");
            //objbulk.ColumnMappings.Add("ProductSubsidiaryCategoryId", "ProductSubsidiaryCategoryId");

            //objbulk.ColumnMappings.Add("Quantity", "Quantity");
            //objbulk.ColumnMappings.Add("ProductStock", "ProductStock");
            //objbulk.ColumnMappings.Add("PurchasePrice", "PurchasePrice");
            //objbulk.ColumnMappings.Add("SalePrice", "SalePrice");
            //objbulk.ColumnMappings.Add("TotalAmount", "TotalAmount");

            //objbulk.ColumnMappings.Add("UOMId", "UOMId");
            //objbulk.ColumnMappings.Add("SizeId", "SizeId");
            //objbulk.ColumnMappings.Add("RAMId", "RAMId");
            //objbulk.ColumnMappings.Add("ROMId", "ROMId");

            //objbulk.ColumnMappings.Add("ColorId", "ColorId");
            //objbulk.ColumnMappings.Add("StyleId", "StyleId");
            //objbulk.ColumnMappings.Add("SupplierId", "SupplierId");
            //objbulk.ColumnMappings.Add("BrandId", "BrandId");

            //objbulk.ColumnMappings.Add("MaxDiscount", "MaxDiscount");
            //objbulk.ColumnMappings.Add("DiscountPerUnit", "DiscountPerUnit");
            //objbulk.ColumnMappings.Add("DiscountAmount", "DiscountAmount");
            //objbulk.ColumnMappings.Add("DiscountInPercentage", "DiscountInPercentage");
            //objbulk.ColumnMappings.Add("DiscountInAmount", "DiscountInAmount");
            //objbulk.ColumnMappings.Add("VatRate", "VatRate");
            //objbulk.ColumnMappings.Add("VatAmount", "VatAmount");

            //objbulk.ColumnMappings.Add("PurchasePrice", "PurchasePrice");
            //objbulk.ColumnMappings.Add("SaleDate", "SaleDate");
            //objbulk.ColumnMappings.Add("SaleDetailDate", "SaleDetailDate");
            //objbulk.ColumnMappings.Add("CompanyId", "CompanyId");
            //objbulk.ColumnMappings.Add("BranchId", "BranchId");

            //objbulk.ColumnMappings.Add("Active", "Active");
            //objbulk.ColumnMappings.Add("IsReturned", "IsReturned");
            //objbulk.ColumnMappings.Add("IsDelivered", "IsDelivered"); 
            //objbulk.ColumnMappings.Add("Archive", "Archive");
            //objbulk.ColumnMappings.Add("IsSynchronized", "IsSynchronized");
            //objbulk.ColumnMappings.Add("IsUpdated", "IsUpdated");

            //objbulk.ColumnMappings.Add("SynchronizationType", "SynchronizationType");
            //objbulk.ColumnMappings.Add("AddedBy", "AddedBy");
            //objbulk.ColumnMappings.Add("AddedDate", "AddedDate");
            //objbulk.ColumnMappings.Add("AddedFromIp", "AddedFromIp");

            //conn.Open();
            //objbulk.WriteToServer(tbl);
            //conn.Close();

            //return SaleId;





            //    #region sale
            //    sale.Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "Sale");
            //    sale.Sequence = GetAutoSequence("Sale");
            //    sale.Barcode = GenerateBarcode(sale.Id);
            //    sale.Active = true;
            //    sale.SynchronizationType = SynchronizationType.Server.ToString();
            //    sale.CompanyId = identity.CompanyId;
            //    sale.BranchId = identity.BranchId;
            //    var branchWiseDiscount = _branchRepository.GetOne(x => x.Id == identity.BranchId).MaxDiscountRate;
            //    if (sale.DiscountInPercentage > branchWiseDiscount)
            //    {
            //        sale.IsApprovedNeeded = true;
            //    }
            //    sale.CustomerPoint = (sale.CustomerPoint + sale.EarningPoint) - sale.ExpensePoint;
            //    sale.CustomerPointAmount = (sale.CustomerPointAmount + sale.EarningPointAmount) - sale.ExpensePointAmount;
            //    //DateTime saleDate = sale.SaleDate;
            //    //DateTime now = DateTime.Now;

            //    //DateTime saleDate = DateTime.Parse(sale.SaleDate.ToShortDateString()+" "+ now.Hour + ":" + now.Minute + ":" + now.Second); //"24 May 2009 02:19:00"              
            //    //sale.SaleDate = saleDate;

            //    sale.AddedBy = identity.Name;
            //    sale.AddedDate = DateTime.Now;
            //    sale.AddedFromIp = identity.IpAddress;
            //    #endregion

            //    #region sale Products
            //    var detailId = Convert.ToInt32(GenerateAutoId(identity.CompanyId, identity.BranchId, "SaleDetail"));
            //    if (sale.SaleDetails != null)
            //    {
            //        var sqnc = GetAutoSequence("SaleDetail");
            //        foreach (var saleDetail in sale.SaleDetails)
            //        {
            //            Product productDb = _productRepository.GetOne(x => x.Id == saleDetail.ProductId);
            //            if (!string.IsNullOrEmpty(productDb?.Id))
            //            {
            //                totalQty += saleDetail.Quantity;
            //                totalAmt += saleDetail.TotalAmount;
            //                saleDetail.Id = detailId.ToString();
            //                saleDetail.SaleId = sale.Id;
            //                saleDetail.Sequence = sqnc;
            //                saleDetail.ProductCode = productDb.Code;
            //                saleDetail.ProductName = productDb.Name;
            //                saleDetail.ProductCategoryId = productDb.ProductCategoryId;
            //                saleDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
            //                saleDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;
            //                saleDetail.UOMId = productDb.UOMId;
            //                saleDetail.RAMId = productDb.RAMId;
            //                saleDetail.ROMId = productDb.ROMId;
            //                saleDetail.SizeId = productDb.SizeId;
            //                saleDetail.ColorId = productDb.ColorId;
            //                saleDetail.StyleId = productDb.StyleId;
            //                saleDetail.GradeId = productDb.GradeId;
            //                saleDetail.SupplierId = productDb.SupplierId;
            //                saleDetail.BrandId = productDb.BrandId;
            //                saleDetail.PurchasePrice = productDb.PurchasePrice;
            //                saleDetail.SaleDate = sale.SaleDate;
            //                saleDetail.SaleDetailDate = DateTime.Now;
            //                saleDetail.CompanyId = identity.CompanyId;
            //                saleDetail.BranchId = identity.BranchId;
            //                saleDetail.Active = true;
            //                saleDetail.SynchronizationType = SynchronizationType.Server.ToString();
            //                saleDetail.AddedBy = identity.Name;
            //                saleDetail.AddedDate = DateTime.Now;
            //                saleDetail.AddedFromIp = identity.IpAddress;
            //                _saleDetailRepository.Add(saleDetail);
            //                detailId++;
            //                sqnc++;
            //            }
            //        }
            //    }
            //    #endregion

            //    #region CustomerLedger
            //    if (!string.IsNullOrEmpty(customer.Id))
            //    {
            //        var customerLedger = new CustomerLedger
            //        {
            //            Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "CustomerLedger"),
            //            Sequence = GetAutoSequence("CustomerLedger"),
            //            TrackingNo = GenerateTrackingNo(identity.CompanyId, identity.BranchId, "CustomerLedger"),
            //            SaleId = sale.Id,
            //            CustomerId = customer.Id,
            //            CustomerMobileNumber = customer.Phone1,
            //            TransactionType = TransactionType.Sales.ToString(),
            //            Particulars = TransactionType.Sales.ToString(),
            //            TransactionDate = DateTime.Now,
            //            DebitAmount = sale.DueAmount > 0 ? sale.PaidAmount : sale.NetAmount,
            //            CreditAmount = sale.NetAmount,
            //            RunningBalance = 0,
            //            EarningPoint = sale.EarningPoint,
            //            EarningPointAmount = sale.EarningPointAmount,
            //            ExpensePoint = sale.ExpensePoint,
            //            ExpensePointAmount = sale.ExpensePointAmount,
            //            CompanyId = identity.CompanyId,
            //            BranchId = identity.BranchId,
            //            Active = true,
            //            SynchronizationType = SynchronizationType.Server.ToString(),
            //            AddedBy = identity.Name,
            //            AddedDate = DateTime.Now,
            //            AddedFromIp = identity.IpAddress
            //        };
            //        _customerLedgerRepository.Add(customerLedger);
            //    }
            //    #endregion
            //    sale.TotalQuantity = totalQty;
            //    sale.TotalAmount = totalAmt;
            //    _saleRepository.Add(sale);
            //    _unitOfWork.SaveChanges();
            //    flag = false;
            //    _unitOfWork.Commit();
            //    _rawSqlService.UpdateCustomerLedgerRunningBalance(customer.Id);
            //    var user = "";
            //    var cameraip = "";
            //    var userid = "";
            //    var password = "";
            //    if (sale.AddedBy != null)
            //    {
            //        user = sale.AddedBy.Substring(0, 5);
            //        cameraip = ConfigurationManager.AppSettings["cameraip"];
            //        userid = ConfigurationManager.AppSettings["userid"];
            //        password = ConfigurationManager.AppSettings["password"];
            //    }

            //    var msg = "U:" + user + ", Q:" + sale.TotalQuantity + ", Tk:" + Math.Round((sale.TotalAmount - sale.ProductDiscount), 0);
            //    var msgTwo = "User:" + sale.AddedBy + ", Inv:" + sale.Id + ", Qty:" + sale.TotalQuantity + ", Tk:" + Math.Round(sale.TotalAmount);

            //    SendToHikVision(userid, password, cameraip, msg, msgTwo);
            //    //SendToHikVision("admin", "admin123", "10.11.1.15", msg, msgTwo); 
            //    return sale.Id;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            //finally
            //{
            //    if (flag)
            //        _unitOfWork.Rollback();
            //}

        }

        //private string GenerateAutoId(string companyId, string branchId, string v)
        //{
        //    try
        //    {
        //        return base.GenerateAutoId(companyId, branchId, tableName);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public DataSet ReportLevelPrint(string saleId)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlCommand sqlComm = new SqlCommand("getSalesReportLevelPrint", conn);
                sqlComm.Parameters.AddWithValue("@saleId", saleId);
                sqlComm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter { SelectCommand = sqlComm };

                da.Fill(ds, "Sale");
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataSet getSalesList(string companyId, string branchId, string customerId, string dateFrom, string dateTo)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlCommand sqlComm = new SqlCommand("getSalesList", conn);
                sqlComm.Parameters.AddWithValue("@companyId", companyId);
                sqlComm.Parameters.AddWithValue("@branchId", branchId);
                sqlComm.Parameters.AddWithValue("@customerId", customerId);
                sqlComm.Parameters.AddWithValue("@dateFrom", dateFrom);
                sqlComm.Parameters.AddWithValue("@dateTo", dateTo);
                sqlComm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter { SelectCommand = sqlComm };

                da.Fill(ds, "Sale");
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string AddStockOuttems(SuperShopStockOutViewModel salevm)
        {
            try
            {
                var VoucherId = "";
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var totalQuantity = 0m;
                var totalAmount = 0m;

                SqlCommand sqlComm = new SqlCommand("setProductStockOut", conn);
                #region sale  

                sqlComm.Parameters.AddWithValue("@MemoNo", salevm.MemoNo);
                sqlComm.Parameters.AddWithValue("@RefNo", salevm.RefNo);
                sqlComm.Parameters.AddWithValue("@TypeId", salevm.TypeId);
                sqlComm.Parameters.AddWithValue("@CompanyId", identity.CompanyId);
                sqlComm.Parameters.AddWithValue("@BranchId", identity.BranchId);
                sqlComm.Parameters.AddWithValue("@SupplierId", salevm.SupplierId);
                sqlComm.Parameters.AddWithValue("@BrandId", salevm.BrandId);
                sqlComm.Parameters.AddWithValue("@StockOutDate", salevm.StockOutDate);
                sqlComm.Parameters.AddWithValue("@MemoWiseDiscount", salevm.MemoWiseDiscount);
                sqlComm.Parameters.AddWithValue("@NetAmount", salevm.NetAmount);
                sqlComm.Parameters.AddWithValue("@PaidAmount", salevm.PaidAmount);
                sqlComm.Parameters.AddWithValue("@DueAmount", salevm.DueAmount);

                sqlComm.Parameters.AddWithValue("@Active", true);
                sqlComm.Parameters.AddWithValue("@Archive", false);
                sqlComm.Parameters.AddWithValue("@IsSynchronized", false);
                sqlComm.Parameters.AddWithValue("@IsUpdated", false);
                sqlComm.Parameters.AddWithValue("@SynchronizationType", SynchronizationType.Server.ToString());
                sqlComm.Parameters.AddWithValue("@AddedBy", identity.Name);
                sqlComm.Parameters.AddWithValue("@AddedDate", DateTime.Now);
                sqlComm.Parameters.AddWithValue("@AddedFromIp", identity.IpAddress);

                #endregion customerLeger
                sqlComm.Parameters.Add("@VoucherId", SqlDbType.VarChar, 100);
                sqlComm.Parameters["@VoucherId"].Direction = ParameterDirection.Output;

                sqlComm.CommandType = CommandType.StoredProcedure;
                foreach (var saleDetail in salevm.StockOutDetails)
                {
                    totalQuantity += saleDetail.Quantity;
                    totalAmount += saleDetail.TotalAmount;
                }

                sqlComm.Parameters.AddWithValue("@TotalQuantity", totalQuantity);
                sqlComm.Parameters.AddWithValue("@TotalAmount", totalAmount);
                sqlComm.Connection.Open();
                sqlComm.ExecuteNonQuery();
                VoucherId = sqlComm.Parameters["@VoucherId"].Value.ToString();
                sqlComm.Connection.Close();

                SqlCommand sqlCmd = new SqlCommand("setProductStockOutDetail", conn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection.Open();

                foreach (var saleDetail in salevm.StockOutDetails)
                {
                    sqlCmd.Parameters.Clear();
                    sqlCmd.Parameters.AddWithValue("@StockOutId", VoucherId);
                    sqlCmd.Parameters.AddWithValue("@CompanyId", identity.CompanyId);
                    sqlCmd.Parameters.AddWithValue("@BranchId", identity.BranchId);
                    sqlCmd.Parameters.AddWithValue("@WarehouseId", saleDetail.WarehouseId);
                    sqlCmd.Parameters.AddWithValue("@ProductCategoryId", saleDetail.ProductCategoryId);
                    sqlCmd.Parameters.AddWithValue("@ProductSubCategoryId", saleDetail.ProductSubCategoryId);
                    sqlCmd.Parameters.AddWithValue("@ProductSubsidiaryCategoryId", saleDetail.ProductSubsidiaryCategoryId);
                    sqlCmd.Parameters.AddWithValue("@ProductId", saleDetail.ProductId);
                    sqlCmd.Parameters.AddWithValue("@ProductCode", saleDetail.ProductCode);
                    sqlCmd.Parameters.AddWithValue("@ProductName", saleDetail.ProductName);
                    sqlCmd.Parameters.AddWithValue("@UOMId", saleDetail.UOMId);
                    sqlCmd.Parameters.AddWithValue("@RAMId", saleDetail.RAMId);
                    sqlCmd.Parameters.AddWithValue("@ROMId", saleDetail.ROMId);
                    sqlCmd.Parameters.AddWithValue("@SizeId", saleDetail.SizeId);
                    sqlCmd.Parameters.AddWithValue("@ColorId", saleDetail.ColorId);
                    sqlCmd.Parameters.AddWithValue("@StyleId", saleDetail.StyleId);
                    sqlCmd.Parameters.AddWithValue("@GradeId", saleDetail.GradeId);
                    sqlCmd.Parameters.AddWithValue("@SupplierId", saleDetail.SupplierId);
                    sqlCmd.Parameters.AddWithValue("@BrandId", saleDetail.BrandId);
                    sqlCmd.Parameters.AddWithValue("@ExpiryDate", DateTime.Now);
                    sqlCmd.Parameters.AddWithValue("@StockOutDate", saleDetail.StockOutDate);
                    sqlCmd.Parameters.AddWithValue("@StockOutDetailDate", DateTime.Now);
                    sqlCmd.Parameters.AddWithValue("@ProductStock", saleDetail.ProductStock);
                    sqlCmd.Parameters.AddWithValue("@Quantity", saleDetail.Quantity);
                    sqlCmd.Parameters.AddWithValue("@StockOutPrice", saleDetail.StockOutPrice);
                    sqlCmd.Parameters.AddWithValue("@SalePrice", saleDetail.SalePrice);
                    sqlCmd.Parameters.AddWithValue("@TotalAmount", saleDetail.TotalAmount);

                    sqlCmd.Parameters.AddWithValue("@Active", true);
                    sqlCmd.Parameters.AddWithValue("@Archive", false);
                    sqlCmd.Parameters.AddWithValue("@IsSynchronized", false);
                    sqlCmd.Parameters.AddWithValue("@IsUpdated", false);
                    sqlCmd.Parameters.AddWithValue("@SynchronizationType", SynchronizationType.Server.ToString());
                    sqlCmd.Parameters.AddWithValue("@AddedBy", identity.Name);
                    sqlCmd.Parameters.AddWithValue("@AddedDate", DateTime.Now);
                    sqlCmd.Parameters.AddWithValue("@AddedFromIp", identity.IpAddress);

                    sqlCmd.ExecuteNonQuery();
                }
                sqlCmd.Connection.Close();
                return VoucherId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataSet GetProductStockAll(string companyId, string branchId, string dateFrom, string dateTo)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlCommand sqlComm = new SqlCommand("getProductStockAll", conn);
                sqlComm.Parameters.AddWithValue("@companyId", companyId);
                sqlComm.Parameters.AddWithValue("@branchId", branchId);
                sqlComm.Parameters.AddWithValue("@dateFrom", dateFrom);
                sqlComm.Parameters.AddWithValue("@dateTo", dateTo);
                sqlComm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter { SelectCommand = sqlComm };

                da.Fill(ds, "StockOut");
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
