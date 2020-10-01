using Library.Context;
using Library.Model.Core.Core;
using Library.Model.Inventory.Sales;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;

namespace Library.Context.Repositories
{
    public class RawSqlService : IRawSqlService
    {
        #region Ctor
        private readonly ErpdbEntities _db;
        public RawSqlService(ErpdbEntities db)
        {
            _db = db;
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

        public IEnumerable<ProductVm> GetBranchwiseProductStockAll(string companyId, string branchId)
        {
            try
            {
                var sql = @"select 
                            max(itm.Id) as value,
                            max(itm.Name) as label,
                            max(itm.Id) as ProductId,
                            max(itm.Code) as ProductCode,
                            max(itm.Name) as ProductName,
                            max(isnull(itm.ReorderLevel,0)) as ReorderLevel,
                            max(isnull(itm.ShelfLife,0)) as ShelfLife,
                            max(itm.PurchasePrice) as PurchasePrice,
                            max(isnull(itm.RetailPrice,0)) as RetailPrice,
                            max(isnull(itm.WholeSalePrice,0)) as WholeSalePrice,
                            max(isnull(itm.MaxDiscount,0)) as MaxDiscount,
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
                            where itmMstr.CompanyId='" + companyId + @"' and itmMstr.BranchId='" + branchId + @"'
                            group by
                            itm.Id";
                return _db.Database.SqlQuery<ProductVm>(sql).ToArray().ToList();
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
                var sql = @"with itm as(
                            select 
                            max(itm.Id) as value,
                            max(itm.Code) as label,
                            max(itm.Id) as ProductId,
                            max(itm.Code) as ProductCode,
                            max(itm.Name) as ProductName,
                            max(isnull(itm.ReorderLevel,0)) as ReorderLevel,
                            max(isnull(itm.ShelfLife,0)) as ShelfLife,
                            max(itm.PurchasePrice) as PurchasePrice,
                            max(isnull(itm.RetailPrice,0)) as RetailPrice,
                            max(isnull(itm.WholeSalePrice,0)) as WholeSalePrice,
                            max(isnull(itm.MaxDiscount,0)) as MaxDiscount,
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
                            where itmMstr.CompanyId='" + companyId + @"' and itmMstr.BranchId='" + branchId + @"'
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
                            where 
                            itmMstr.CompanyId=" + companyId + @" and
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
                            where 
                            itmMstr.CompanyId=" + companyId + @" and
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
                            where " + paramSql + @"";
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

        public IEnumerable<ProductStockVm> GetAllProductStock(string companyId, string branchId, string supplierId, string productCategoryId, string productSubCategoryId, string ProductId, string productCode, string dateFrom, string dateTo)
        {
            try
            {
                string paramSql = "", sql = "";
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
                if (!string.IsNullOrEmpty(productCode))
                    paramSql = paramSql + " and itm.Code='" + productCode + "'";
                if (!string.IsNullOrEmpty(supplierId))
                    paramSql = paramSql + " and itm.SupplierId='" + supplierId + "'";

                if (string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
                {
                    sql = @"SELECT max(itm.Id) AS value
	                        ,max(itm.Code) AS label
	                        ,max(itm.Id) AS Id
                            ,max(R.Name) AS RackName
	                        ,max(itm.Code) AS ProductCode
	                        ,max(itm.Name) AS ProductName
	                        ,max(isnull(itm.ReorderLevel, 0)) AS ReorderLevel
	                        ,max(isnull(itm.ShelfLife, 0)) AS ShelfLife
	                        ,max(itm.PurchasePrice) AS PurchasePrice
	                        ,max(isnull(itm.RetailPrice, 0)) AS RetailPrice
	                        ,max(isnull(itm.WholeSalePrice, 0)) AS WholeSalePrice
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
			                        ), 0) AS VatRate
	                        ,max(isnull(itm.SupplierId, '')) AS SupplierId
	
	                        ,(
		                        (
			                        SELECT isnull(sum(prchDtl.Quantity), 0)
			                        FROM PurchaseDetail prchDtl
			                        WHERE prchDtl.ProductId = max(itm.Id)				
			                        ) + (
			                        SELECT isnull(sum(slRtnDtl.ReturnQuantity), 0)
			                        FROM SaleReturnDetail slRtnDtl
			                        WHERE slRtnDtl.ProductId = max(itm.Id)
				                        AND slRtnDtl.IsDamage = 0				
			                        )
		                        ) - (
		                        (
			                        SELECT isnull(sum(slDtl.Quantity), 0)
			                        FROM SaleDetail slDtl
			                        WHERE slDtl.ProductId = max(itm.Id)				
			                        ) + (
			                        SELECT isnull(sum(prchRtnDtl.ReturnQuantity), 0)
			                        FROM PurchaseReturnDetail prchRtnDtl
			                        WHERE prchRtnDtl.ProductId = max(itm.Id)
			                        )
		                        ) AS ProductStock

	                        ,isnull(max(PurchasePrice) * ((
			                        (
				                        SELECT isnull(sum(prchDtl.Quantity), 0)
				                        FROM PurchaseDetail prchDtl
				                        WHERE prchDtl.ProductId = max(itm.Id)
				                        ) + (
				                        SELECT isnull(sum(slRtnDtl.ReturnQuantity), 0)
				                        FROM SaleReturnDetail slRtnDtl
				                        WHERE slRtnDtl.ProductId = max(itm.Id)
					                        AND slRtnDtl.IsDamage = 0
				                        )
			                        ) - (
			                        (
				                        SELECT isnull(sum(slDtl.Quantity), 0)
				                        FROM SaleDetail slDtl
				                        WHERE slDtl.ProductId = max(itm.Id)
				                        ) + (
				                        SELECT isnull(sum(prchRtnDtl.ReturnQuantity), 0)
				                        FROM PurchaseReturnDetail prchRtnDtl
				                        WHERE prchRtnDtl.ProductId = max(itm.Id)
				                        )
			                        )), 0) AS ProductStockValue
	                        ,max(itmMstr.CompanyId) AS CompanyId
	                        ,max(itmMstr.BranchId) AS BranchId
                        FROM Product itm
                        JOIN ProductMaster itmMstr ON itm.Id = itmMstr.ProductId
                        LEFT JOIN Rack R on r.Id = itm.RackId
                        WHERE " + paramSql + @"
                        GROUP BY itm.Id";
                }
                else
                {
                    sql = @"select 
                            max(itm.Id) as value,
                            max(itm.Code) as label,
                            max(itm.Id) as Id,
                            max(R.Name) AS RackName,
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
                            isnull((select vatCtgr.VatRate from VatCategory vatCtgr where max(itm.VatCategoryId)=vatCtgr.Id),0) as VatRate,
                            max(isnull(itm.SupplierId,'')) as SupplierId,
                            ((select isnull(sum(prchDtl.Quantity),0)  from PurchaseDetail prchDtl where prchDtl.ProductId = max(itm.Id) and  convert(date,PurchaseDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')) +
                            (select isnull(sum(slRtnDtl.ReturnQuantity),0)  from SaleReturnDetail slRtnDtl where  slRtnDtl.ProductId =max(itm.Id) and slRtnDtl.IsDamage=0 and convert(date,SalesReturnDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')))-(
                            (select isnull(sum(slDtl.Quantity),0)  from SaleDetail slDtl where slDtl.ProductId =max(itm.Id) and convert(date,SaleDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')) +
                            (select isnull(sum(prchRtnDtl.ReturnQuantity),0)  from PurchaseReturnDetail prchRtnDtl where prchRtnDtl.ProductId =max(itm.Id) and convert(date,PurchaseReturnDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"'))) as ProductStock,
                            isnull(max(PurchasePrice)*((select isnull(sum(prchDtl.Quantity),0)  from PurchaseDetail prchDtl where prchDtl.ProductId = max(itm.Id) and  convert(date,PurchaseDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')) +
                            (select isnull(sum(slRtnDtl.ReturnQuantity),0)  from SaleReturnDetail slRtnDtl where  slRtnDtl.ProductId =max(itm.Id) and slRtnDtl.IsDamage=0 and convert(date,SalesReturnDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')))-(
                            (select isnull(sum(slDtl.Quantity),0)  from SaleDetail slDtl where slDtl.ProductId =max(itm.Id) and convert(date,SaleDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"')) +
                            (select isnull(sum(prchRtnDtl.ReturnQuantity),0)  from PurchaseReturnDetail prchRtnDtl where prchRtnDtl.ProductId =max(itm.Id) and convert(date,PurchaseReturnDate) between convert(date,'" + dateFrom + @"') and convert(date,'" + dateTo + @"'))),0) as ProductStockValue,
                            max(itmMstr.CompanyId) as CompanyId,
                            max(itmMstr.BranchId) as BranchId
                            from Product itm
                            join ProductMaster itmMstr on itm.Id=itmMstr.ProductId
                            Left JOIN Rack R on r.Id = itm.RackId
                            where " + paramSql + @"
                            group by itm.Id";
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
                                     Isnull(SUM(slsDtl.TotalAmount)+ Isnull(SUM(slsDtl.TotalVat),0),0)-Isnull(SUM(slsDtl.ProductDiscount+sls.OverAllDiscount+ sls.CustomerDiscountInAmount),0) as NetAmount,
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
                return _db.Database.SqlQuery<SaleVm>(sql).ToArray().ToList();
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
                    paramSql = " sls.CompanyId='" + companyId + "'";
                if (!string.IsNullOrEmpty(branchId))
                    paramSql = paramSql + " and sls.BranchId='" + branchId + "'";
                if (!string.IsNullOrEmpty(dateFrom))
                    paramSql = paramSql + " and convert(date,sls.SaleDate) between convert(date,'" + dateFrom + "') and convert(date,'" + dateTo + "')";
                if (!string.IsNullOrEmpty(saleById))
                    paramSql = paramSql + " and sls.AddedBy='" + saleById + "'";
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
                            sls.Archive,
                            sls.IsFullyReturned,
                            sls.IsFullyCancelled,
                            sls.AddedBy
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
                            slsDtl.ProductDiscount,
                            sls.AddedBy
                            )as sls
                            where " + paramSql + @"";
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
					                    AND convert(DATE, cstmrLdgr.TransactionDate) BETWEEN convert(DATE, '" + dateFrom + @"') AND convert(DATE, '" + dateTo + @"')
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
                    paramSql = paramSql + " and cstmr.Phone1 like '" + customerMobileNumber + @"%'";
                var sql = @"select
                            cstmr.Id as value,
                            cstmr.Phone1 as label,
                            cstmr.[Name] as CustomerName,
                            cstmr.Address1 as CustomerAddress1, 
                            cstmr.Address2 as CustomerAddress2, 
                            cstmr.DiscountRate as CustomerDiscountRate,
                            isNull((select (case when sum(DebitAmount)>sum(CreditAmount) then sum(DebitAmount)-sum(CreditAmount)end) from CustomerLedger  where CustomerId=cstmr.Id ),0) as CustomerAdvanceAmount,
                            isNull((select (case when sum(CreditAmount)>sum(DebitAmount) then sum(CreditAmount)-sum(DebitAmount)end) from CustomerLedger  where CustomerId=cstmr.Id ),0) as CustomerDueAmount,
                            isNull((select sum(EarningPoint)-sum(ExpensePoint) from CustomerLedger  where CustomerId=cstmr.Id ),0) as CustomerPoint,
                            isNull((select sum(EarningPointAmount)-sum(ExpensePointAmount) from CustomerLedger  where CustomerId=cstmr.Id ),0) as CustomerPointAmount
                            from Customer cstmr
                            where " + paramSql + @"";
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

        public DataSet GetPurchaseSummary(string companyId, string branchId, string supplierId)
        {
            try
            {
                SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ErpdbEntities"].ConnectionString);
                SqlCommand sqlcomm = new SqlCommand();

                DataSet ds = new DataSet();
                //using (SqlConnection conn = new SqlConnection("ERP_AmericanTMartConnectionString"))
                //{
                SqlCommand sqlComm = new SqlCommand("getCustomer", conn);
                sqlComm.Parameters.AddWithValue("@customerId", 1);

                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds, "Sale");
                //return ds;
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
