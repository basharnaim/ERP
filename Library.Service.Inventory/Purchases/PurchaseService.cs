using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Products;
using Library.Model.Inventory.Purchases;
using Library.Model.Inventory.Suppliers;
using Library.Service.Core.Core;
using Library.Context.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace Library.Service.Inventory.Purchases
{
    public class PurchaseService : Service<Purchase>, IPurchaseService
    {
        #region Ctor
        private readonly IRepository<Purchase> _purchaseRepository;
        private readonly IRepository<PurchaseDetail> _purchaseDetailRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<SupplierLedger> _supplierLedgerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRawSqlService _rawSqlService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SqlConnection conn;
        public PurchaseService(
            IRepository<Purchase> purchaseRepository,
            IRepository<PurchaseDetail> purchaseDetailRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<SupplierLedger> supplierLedgerRepository,
            IRepository<Product> productRepository,
            IRawSqlService rawSqlService,
            IUnitOfWork unitOfWork
            ) : base(purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
            _purchaseDetailRepository = purchaseDetailRepository;
            _supplierRepository = supplierRepository;
            _supplierLedgerRepository = supplierLedgerRepository;
            _productRepository = productRepository;
            _rawSqlService = rawSqlService;
            _unitOfWork = unitOfWork;
            conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ErpdbEntities"].ConnectionString);
        }
        #endregion
        public override string GenerateAutoId(string companyId, string branchId, string tableName)
        {
            try
            {
                return base.GenerateAutoId(companyId, branchId, tableName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(Purchase purchase)
        {
            try
            {
                var PurchaseId = "";
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var totalQuantity = 0m;
                var totalAmount = 0m;

                SqlCommand sqlComm = new SqlCommand("setPurchase", conn);
                #region purchase  
                sqlComm.Parameters.AddWithValue("@PurchaseId",'0');
                sqlComm.Parameters.AddWithValue("@CompanyId", identity.CompanyId);
                sqlComm.Parameters.AddWithValue("@BranchId", identity.BranchId);
                sqlComm.Parameters.AddWithValue("@MemoNo", purchase.MemoNo);

                sqlComm.Parameters.AddWithValue("@RefNo", purchase.RefNo);
                sqlComm.Parameters.AddWithValue("@SupplierId", purchase.SupplierId);
                sqlComm.Parameters.AddWithValue("@BrandId", purchase.BrandId);

                sqlComm.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                sqlComm.Parameters.AddWithValue("@MemoWiseDiscount", purchase.MemoWiseDiscount);
                sqlComm.Parameters.AddWithValue("@NetAmount", purchase.NetAmount);

                sqlComm.Parameters.AddWithValue("@PaidAmount", purchase.PaidAmount);
                sqlComm.Parameters.AddWithValue("@DueAmount", purchase.DueAmount);
                sqlComm.Parameters.AddWithValue("@FullyReturned", purchase.FullyReturned);

                sqlComm.Parameters.AddWithValue("@Active", purchase.Active);
                sqlComm.Parameters.AddWithValue("@Archive", purchase.Archive);
                sqlComm.Parameters.AddWithValue("@IsSynchronized", purchase.IsSynchronized);

                sqlComm.Parameters.AddWithValue("@IsUpdated", purchase.IsUpdated);
                sqlComm.Parameters.AddWithValue("@SynchronizationType", purchase.SynchronizationType);
                sqlComm.Parameters.AddWithValue("@Description", purchase.Description);

                sqlComm.Parameters.AddWithValue("@AddedBy", purchase.AddedBy);
                sqlComm.Parameters.AddWithValue("@AddedDate", DateTime.Now);
                sqlComm.Parameters.AddWithValue("@AddedFromIp", purchase.AddedFromIp);
                #endregion Purchase

                sqlComm.Parameters.Add("@pid", SqlDbType.VarChar, 100);
                sqlComm.Parameters["@pid"].Direction = ParameterDirection.Output;

                sqlComm.CommandType = CommandType.StoredProcedure;
                foreach (var purchaseDetail in purchase.PurchaseDetails)
                {
                    totalQuantity += purchaseDetail.Quantity;
                    totalAmount += purchaseDetail.TotalAmount;
                }

                sqlComm.Parameters.AddWithValue("@TotalQuantity", totalQuantity);
                sqlComm.Parameters.AddWithValue("@TotalAmount", totalAmount);
                sqlComm.Connection.Open();
                sqlComm.ExecuteNonQuery();
                PurchaseId = sqlComm.Parameters["@pid"].Value.ToString();
                sqlComm.Connection.Close();

                SqlCommand sqlCmd = new SqlCommand("setPurchaseDetail", conn); ;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection.Open();
                foreach (var purchaseDetail in purchase.PurchaseDetails)
                {
                    sqlCmd.Parameters.Clear();

                    sqlCmd.Parameters.AddWithValue("@PurchaseId", PurchaseId);
                    sqlCmd.Parameters.AddWithValue("@PurchaceDetailId", '0');
                    sqlCmd.Parameters.AddWithValue("@CompanyId", purchaseDetail.CompanyId);
                    sqlCmd.Parameters.AddWithValue("@BranchId", purchaseDetail.BranchId);
                    sqlCmd.Parameters.AddWithValue("@ProductId", purchaseDetail.ProductId);
                    sqlCmd.Parameters.AddWithValue("@ProductCode", purchaseDetail.ProductCode);
                    sqlCmd.Parameters.AddWithValue("@ProductName", purchaseDetail.ProductName);
                    sqlCmd.Parameters.AddWithValue("@ProductCategoryId", purchaseDetail.ProductCategoryId);
                    sqlCmd.Parameters.AddWithValue("@ProductSubCategoryId", purchaseDetail.ProductSubCategoryId);
                    sqlCmd.Parameters.AddWithValue("@ProductSubsidiaryCategoryId", purchaseDetail.ProductSubsidiaryCategoryId);

                    sqlCmd.Parameters.AddWithValue("@Quantity", purchaseDetail.Quantity);
                    sqlCmd.Parameters.AddWithValue("@ProductStock", purchaseDetail.ProductStock);
                    sqlCmd.Parameters.AddWithValue("@PurchasePrice", purchaseDetail.PurchasePrice);
                    sqlCmd.Parameters.AddWithValue("@SalePrice", purchaseDetail.SalePrice);
                    sqlCmd.Parameters.AddWithValue("@TotalAmount", purchaseDetail.TotalAmount);

                    sqlCmd.Parameters.AddWithValue("@UOMId", purchaseDetail.UOMId);
                    sqlCmd.Parameters.AddWithValue("@SizeId", purchaseDetail.SizeId); //purchaseDetail.SizeId);
                    sqlCmd.Parameters.AddWithValue("@SupplierId", purchase.SupplierId);

                    sqlCmd.Parameters.AddWithValue("@RAMId", 0); //purchaseDetail.RAMId);
                    sqlCmd.Parameters.AddWithValue("@ROMId", 0); //purchaseDetail.ROMId);
                    sqlCmd.Parameters.AddWithValue("@ColorId", 0); //purchaseDetail.ColorId);
                    sqlCmd.Parameters.AddWithValue("@StyleId", 0); //purchaseDetail.StyleId); 
                    sqlCmd.Parameters.AddWithValue("@GradeId", 0); //purchaseDetail.GradeId);                   
                    sqlCmd.Parameters.AddWithValue("@BrandId", 0); //purchaseDetail.BrandId);

                    sqlCmd.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                    sqlCmd.Parameters.AddWithValue("@PurchaseDetailDate", DateTime.Now);
                    sqlCmd.Parameters.AddWithValue("@ExpiryDate", purchaseDetail.ExpiryDate);
                    sqlCmd.Parameters.AddWithValue("@Description", purchaseDetail.Description);
                    sqlCmd.Parameters.AddWithValue("@Returned", false);
                    sqlCmd.Parameters.AddWithValue("@IsCancelled", false);
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
                //return SaleId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(Purchase purchase)
        {
            try
            {
                var PurchaseId = "";
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var totalQuantity = 0m;
                var totalAmount = 0m;

                SqlCommand sqlComm = new SqlCommand("setPurchase", conn);
                #region purchase  
                sqlComm.Parameters.AddWithValue("@PurchaseId", purchase.Id);
                sqlComm.Parameters.AddWithValue("@CompanyId", identity.CompanyId);
                sqlComm.Parameters.AddWithValue("@BranchId", identity.BranchId);
                sqlComm.Parameters.AddWithValue("@MemoNo", purchase.MemoNo);

                sqlComm.Parameters.AddWithValue("@RefNo", purchase.RefNo);
                sqlComm.Parameters.AddWithValue("@SupplierId", purchase.SupplierId);
                sqlComm.Parameters.AddWithValue("@BrandId", purchase.BrandId);

                sqlComm.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                sqlComm.Parameters.AddWithValue("@MemoWiseDiscount", purchase.MemoWiseDiscount);
                sqlComm.Parameters.AddWithValue("@NetAmount", purchase.NetAmount);

                sqlComm.Parameters.AddWithValue("@PaidAmount", purchase.PaidAmount);
                sqlComm.Parameters.AddWithValue("@DueAmount", purchase.DueAmount);
                sqlComm.Parameters.AddWithValue("@FullyReturned", purchase.FullyReturned);

                sqlComm.Parameters.AddWithValue("@Active", purchase.Active);
                sqlComm.Parameters.AddWithValue("@Archive", purchase.Archive);
                sqlComm.Parameters.AddWithValue("@IsSynchronized", purchase.IsSynchronized);

                sqlComm.Parameters.AddWithValue("@IsUpdated", purchase.IsUpdated);
                sqlComm.Parameters.AddWithValue("@SynchronizationType", purchase.SynchronizationType);
                sqlComm.Parameters.AddWithValue("@Description", purchase.Description);

                sqlComm.Parameters.AddWithValue("@AddedBy", purchase.AddedBy);
                sqlComm.Parameters.AddWithValue("@AddedDate", DateTime.Now);
                sqlComm.Parameters.AddWithValue("@AddedFromIp", purchase.AddedFromIp);
                #endregion Purchase

                sqlComm.Parameters.Add("@pid", SqlDbType.VarChar, 100);
                sqlComm.Parameters["@pid"].Direction = ParameterDirection.Output;

                sqlComm.CommandType = CommandType.StoredProcedure;
                foreach (var purchaseDetail in purchase.PurchaseDetails)
                {
                    totalQuantity += purchaseDetail.Quantity;
                    totalAmount += purchaseDetail.TotalAmount;
                }

                sqlComm.Parameters.AddWithValue("@TotalQuantity", totalQuantity);
                sqlComm.Parameters.AddWithValue("@TotalAmount", totalAmount);
                sqlComm.Connection.Open();
                sqlComm.ExecuteNonQuery();
                PurchaseId = sqlComm.Parameters["@pid"].Value.ToString();
                sqlComm.Connection.Close();

                SqlCommand sqlCmd = new SqlCommand("setPurchaseDetail", conn); ;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection.Open();
                foreach (var purchaseDetail in purchase.PurchaseDetails)
                {
                    sqlCmd.Parameters.Clear();

                    sqlCmd.Parameters.AddWithValue("@PurchaseId", PurchaseId);
                    sqlCmd.Parameters.AddWithValue("@PurchaceDetailId", purchaseDetail.Id);
                    sqlCmd.Parameters.AddWithValue("@CompanyId", purchaseDetail.CompanyId);
                    sqlCmd.Parameters.AddWithValue("@BranchId", purchaseDetail.BranchId);
                    sqlCmd.Parameters.AddWithValue("@ProductId", purchaseDetail.ProductId);
                    sqlCmd.Parameters.AddWithValue("@ProductCode", purchaseDetail.ProductCode);
                    sqlCmd.Parameters.AddWithValue("@ProductName", purchaseDetail.ProductName);
                    sqlCmd.Parameters.AddWithValue("@ProductCategoryId", purchaseDetail.ProductCategoryId);
                    sqlCmd.Parameters.AddWithValue("@ProductSubCategoryId", purchaseDetail.ProductSubCategoryId);
                    sqlCmd.Parameters.AddWithValue("@ProductSubsidiaryCategoryId", purchaseDetail.ProductSubsidiaryCategoryId);

                    sqlCmd.Parameters.AddWithValue("@Quantity", purchaseDetail.Quantity);
                    sqlCmd.Parameters.AddWithValue("@ProductStock", purchaseDetail.ProductStock);
                    sqlCmd.Parameters.AddWithValue("@PurchasePrice", purchaseDetail.PurchasePrice);
                    sqlCmd.Parameters.AddWithValue("@SalePrice", purchaseDetail.SalePrice);
                    sqlCmd.Parameters.AddWithValue("@TotalAmount", purchaseDetail.TotalAmount);

                    sqlCmd.Parameters.AddWithValue("@UOMId", purchaseDetail.UOMId);
                    sqlCmd.Parameters.AddWithValue("@SizeId", purchaseDetail.SizeId); //purchaseDetail.SizeId);
                    sqlCmd.Parameters.AddWithValue("@SupplierId", purchase.SupplierId);

                    sqlCmd.Parameters.AddWithValue("@RAMId", 0); //purchaseDetail.RAMId);
                    sqlCmd.Parameters.AddWithValue("@ROMId", 0); //purchaseDetail.ROMId);
                    sqlCmd.Parameters.AddWithValue("@ColorId", 0); //purchaseDetail.ColorId);
                    sqlCmd.Parameters.AddWithValue("@StyleId", 0); //purchaseDetail.StyleId); 
                    sqlCmd.Parameters.AddWithValue("@GradeId", 0); //purchaseDetail.GradeId);                   
                    sqlCmd.Parameters.AddWithValue("@BrandId", 0); //purchaseDetail.BrandId);

                    sqlCmd.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                    sqlCmd.Parameters.AddWithValue("@PurchaseDetailDate", DateTime.Now);
                    sqlCmd.Parameters.AddWithValue("@ExpiryDate", purchaseDetail.ExpiryDate);
                    sqlCmd.Parameters.AddWithValue("@Description", purchaseDetail.Description);
                    sqlCmd.Parameters.AddWithValue("@Returned", false);
                    sqlCmd.Parameters.AddWithValue("@IsCancelled", false);
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
                //return SaleId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //public void Add(Purchase purchase)
        //{
        //    var flag = false;
        //    try
        //    {
        //        _unitOfWork.BeginTransaction();
        //        flag = true;
        //        #region Purchase
        //        var totalQty = 0m;
        //        var totalAmt = 0m;
        //        var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
        //        purchase.Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "Purchase");
        //        purchase.Sequence = GetAutoSequence("Purchase");
        //        if (purchase.MemoWiseDiscount <= 0)
        //        {
        //            purchase.NetAmount = purchase.TotalAmount;
        //        }

        //        purchase.Active = true;
        //        purchase.Archive = false;
        //        purchase.SynchronizationType = SynchronizationType.Server.ToString();
        //        purchase.CompanyId = identity.CompanyId;
        //        purchase.BranchId = identity.BranchId;
        //        purchase.AddedBy = identity.Name;
        //        purchase.AddedDate = DateTime.Now;
        //        purchase.AddedFromIp = identity.IpAddress;

        //        #region Purchase Detail
        //        if (purchase.PurchaseDetails != null)
        //        {
        //            var sqnc = GetAutoSequence("PurchaseDetail");
        //            int detailId = Convert.ToInt32(GenerateAutoId(purchase.CompanyId, purchase.BranchId, "PurchaseDetail"));
        //            foreach (var purchaseDetail in purchase.PurchaseDetails)
        //            {
        //                var productDb = _productRepository.GetOne(x => x.Id == purchaseDetail.ProductId);
        //                if (!string.IsNullOrEmpty(productDb?.Id))
        //                {
        //                    totalQty += purchaseDetail.Quantity;
        //                    totalAmt += purchaseDetail.TotalAmount;
        //                    purchaseDetail.Id = detailId.ToString();
        //                    purchaseDetail.Sequence = sqnc;
        //                    purchaseDetail.PurchaseId = purchase.Id;
        //                    purchaseDetail.PurchaseDate = purchase.PurchaseDate;
        //                    purchaseDetail.PurchaseDetailDate = DateTime.Now;
        //                    purchaseDetail.CompanyId = identity.CompanyId;
        //                    purchaseDetail.BranchId = identity.BranchId;
        //                    purchaseDetail.ProductCode = productDb.Code;
        //                    purchaseDetail.ProductName = productDb.Name;
        //                    purchaseDetail.UOMId = productDb.UOMId;
        //                    purchaseDetail.RAMId = productDb.RAMId;
        //                    purchaseDetail.ROMId = productDb.ROMId;
        //                    purchaseDetail.SizeId = productDb.SizeId;
        //                    purchaseDetail.ColorId = productDb.ColorId;
        //                    purchaseDetail.StyleId = productDb.StyleId;
        //                    purchaseDetail.GradeId = productDb.GradeId;
        //                    purchaseDetail.SupplierId = purchase.SupplierId;
        //                    purchaseDetail.BrandId = productDb.BrandId;
        //                    purchaseDetail.SalePrice = productDb.RetailPrice;
        //                    purchaseDetail.ProductCategoryId = productDb.ProductCategoryId;
        //                    purchaseDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
        //                    purchaseDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;
        //                    purchaseDetail.Active = true;
        //                    purchaseDetail.SynchronizationType = SynchronizationType.Server.ToString();
        //                    purchaseDetail.AddedBy = identity.Name;
        //                    purchaseDetail.AddedDate = DateTime.Now;
        //                    purchaseDetail.AddedFromIp = identity.IpAddress;
        //                    _purchaseDetailRepository.Add(purchaseDetail);
        //                    detailId++;
        //                    sqnc++;
        //                }
        //            }
        //        }
        //        #endregion

        //        #region Supplier Ledger
        //        var supplier = _supplierRepository.GetOne(x => x.Id == purchase.SupplierId);
        //        if (!string.IsNullOrEmpty(supplier?.Id))
        //        {
        //            var supplierLedger = new SupplierLedger
        //            {
        //                Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "SupplierLedger"),
        //                Sequence = GetAutoSequence("SupplierLedger"),
        //                CompanyId = identity.CompanyId,
        //                BranchId = identity.BranchId,
        //                PurchaseId = purchase.Id,
        //                TrackingNo = GenerateTrackingNo(identity.CompanyId, identity.BranchId, "SupplierLedger"),
        //                SupplierId = supplier.Id,
        //                SupplierPhone = supplier.Phone1,
        //                Particulars = TransactionType.Purchase.ToString(),
        //                TransactionType = TransactionType.Purchase.ToString(),
        //                TransactionDate = DateTime.Now,
        //                DebitAmount = 0,
        //                CreditAmount = purchase.NetAmount,
        //                RunningBalance = purchase.NetAmount,
        //                Active = true,
        //                SynchronizationType = SynchronizationType.Server.ToString(),
        //                AddedBy = identity.Name,
        //                AddedDate = DateTime.Now,
        //                AddedFromIp = identity.IpAddress
        //            };
        //            _supplierLedgerRepository.Add(supplierLedger);
        //        }
        //        #endregion
        //        purchase.TotalQuantity = totalQty;
        //        purchase.TotalAmount = totalAmt;
        //        _purchaseRepository.Add(purchase);
        //        #endregion
        //        _unitOfWork.SaveChanges();
        //        flag = false;
        //        _unitOfWork.Commit();
        //        _rawSqlService.UpdateSupplierLedgerRunningBalance(supplier?.Id);
        //    }
        //    catch (DbEntityValidationException ex)
        //    {
        //        foreach (var errors in ex.EntityValidationErrors)
        //        {
        //            foreach (var validationError in errors.ValidationErrors)
        //            {
        //                // get the error message 
        //                string errorMessage = validationError.ErrorMessage;
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        if (flag)
        //            _unitOfWork.Rollback();
        //    }
        //}

        //public void Update(Purchase purchase)
        //{
        //    var flag = false;
        //    try
        //    {
        //        _unitOfWork.BeginTransaction();
        //        var supplier = _supplierRepository.GetOne(x => x.Id == purchase.SupplierId);
        //        flag = true;
        //        #region Purchase
        //        var totalQty = 0m;
        //        var totalAmt = 0m; 
        //        var archive = false;  
        //        var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
        //        var purchaseDb = _purchaseRepository.GetOne(x => x.Id == purchase.Id);
        //        var purchaseDetailsDb = _purchaseDetailRepository.GetAll(x => x.PurchaseId == purchase.Id).ToList();

        //        purchase.Active = purchaseDb.Active;
        //        purchase.Archive = purchaseDb.Archive;
        //        purchase.Sequence = purchaseDb.Sequence;
        //        purchase.SynchronizationType = purchaseDb.SynchronizationType;
        //        purchase.CompanyId = purchaseDb.CompanyId;
        //        purchase.BranchId = purchaseDb.BranchId;
        //        purchase.AddedBy = purchaseDb.AddedBy;
        //        purchase.AddedDate = purchaseDb.AddedDate;
        //        purchase.AddedFromIp = purchaseDb.AddedFromIp;

        //        if (purchase.MemoWiseDiscount <= 0)
        //        {
        //            purchase.NetAmount = purchase.TotalAmount;
        //        }
        //        purchase.UpdatedBy = identity.Name;
        //        purchase.UpdatedDate = DateTime.Now;
        //        purchase.UpdatedFromIp = identity.IpAddress;
        //        #endregion

        //        #region Purchase Products
        //        if (purchase.PurchaseDetails != null)
        //        {
        //            var sqnc = GetAutoSequence("PurchaseDetail");
        //            int detailId = Convert.ToInt32(GenerateAutoId(purchase.CompanyId, purchase.BranchId, "PurchaseDetail"));
        //            foreach (var purchaseDetail in purchase.PurchaseDetails)
        //            {
        //                var purchaseDetailDb = _purchaseDetailRepository.GetOne(x => x.Id == purchaseDetail.Id);
        //                var productDb = _productRepository.GetOne(x => x.Id == purchaseDetail.ProductId);
        //                if(purchaseDetail.Quantity == 0)
        //                {
        //                    archive = true;
        //                }
        //                if (purchaseDetailDb != null)
        //                {
        //                    #region Update
        //                    totalQty += purchaseDetail.Quantity;
        //                    totalAmt += purchaseDetail.TotalAmount;
                            
        //                    purchaseDetail.ProductCode = productDb.Code;
        //                    purchaseDetail.ProductName = productDb.Name;
        //                    purchaseDetail.UOMId = productDb.UOMId;
        //                    purchaseDetail.RAMId = productDb.RAMId;
        //                    purchaseDetail.ROMId = productDb.ROMId;
        //                    purchaseDetail.SizeId = productDb.SizeId;
        //                    purchaseDetail.GradeId = productDb.GradeId;
        //                    purchaseDetail.ColorId = productDb.ColorId;
        //                    purchaseDetail.StyleId = productDb.StyleId;
        //                    purchaseDetail.SupplierId = purchase.SupplierId; //productDb.SupplierId;
        //                    purchaseDetail.BrandId = productDb.BrandId;
        //                    purchaseDetail.SalePrice = productDb.RetailPrice;
        //                    purchaseDetail.ProductCategoryId = purchaseDetailDb.ProductCategoryId;
        //                    purchaseDetail.ProductSubCategoryId = purchaseDetailDb.ProductSubCategoryId;
        //                    purchaseDetail.ProductSubsidiaryCategoryId = purchaseDetailDb.ProductSubsidiaryCategoryId;
        //                    purchaseDetail.Sequence = purchaseDetailDb.Sequence;
        //                    purchaseDetail.PurchaseDate = purchaseDetailDb.PurchaseDate;
        //                    purchaseDetail.PurchaseDetailDate = purchaseDetailDb.PurchaseDetailDate;
        //                    purchaseDetail.Active = purchaseDetailDb.Active;
        //                    purchaseDetail.Archive = archive;
        //                    purchaseDetail.PurchaseId = purchaseDetailDb.PurchaseId;
        //                    purchaseDetail.CompanyId = purchaseDetailDb.CompanyId;
        //                    purchaseDetail.BranchId = purchaseDetailDb.BranchId;
        //                    purchaseDetail.SynchronizationType = purchaseDetailDb.SynchronizationType;
                           
        //                    purchaseDetail.AddedBy = purchaseDetailDb.AddedBy;
        //                    purchaseDetail.AddedDate = purchaseDetailDb.AddedDate;
        //                    purchaseDetail.AddedFromIp = purchaseDetailDb.AddedFromIp;
        //                    purchaseDetail.UpdatedBy = identity.Name;
        //                    purchaseDetail.UpdatedDate = DateTime.Now;
        //                    purchaseDetail.UpdatedFromIp = identity.IpAddress;
        //                    _purchaseDetailRepository.Update(purchaseDetail);
        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region Create
        //                    totalQty += purchaseDetail.Quantity;
        //                    totalAmt += purchaseDetail.TotalAmount;
        //                    purchaseDetail.Id = detailId.ToString();
        //                    purchaseDetail.Sequence = sqnc;
        //                    purchaseDetail.PurchaseId = purchase.Id;
        //                    purchaseDetail.PurchaseDate = purchase.PurchaseDate;
        //                    purchaseDetail.PurchaseDetailDate = DateTime.Now;
        //                    purchaseDetail.CompanyId = identity.CompanyId;
        //                    purchaseDetail.BranchId = identity.BranchId;
        //                    purchaseDetail.ProductCode = productDb.Code;
        //                    purchaseDetail.ProductName = productDb.Name;
        //                    purchaseDetail.UOMId = productDb.UOMId;
        //                    purchaseDetail.RAMId = productDb.RAMId;
        //                    purchaseDetail.ROMId = productDb.ROMId;
        //                    purchaseDetail.SizeId = productDb.SizeId;
        //                    purchaseDetail.ColorId = productDb.ColorId;
        //                    purchaseDetail.StyleId = productDb.StyleId;
        //                    purchaseDetail.GradeId = productDb.GradeId;
        //                    purchaseDetail.SupplierId = purchase.SupplierId;
        //                    purchaseDetail.BrandId = productDb.BrandId;
        //                    purchaseDetail.SalePrice = productDb.RetailPrice;
        //                    purchaseDetail.ProductCategoryId = productDb.ProductCategoryId;
        //                    purchaseDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
        //                    purchaseDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;
        //                    purchaseDetail.Active = true;
        //                    purchaseDetail.Archive = archive;
        //                    purchaseDetail.SynchronizationType = SynchronizationType.Server.ToString();
        //                    purchaseDetail.AddedBy = identity.Name;
        //                    purchaseDetail.AddedDate = DateTime.Now;
        //                    purchaseDetail.AddedFromIp = identity.IpAddress;
        //                    _purchaseDetailRepository.Add(purchaseDetail);
        //                    detailId++;
        //                    sqnc++;
        //                    #endregion
        //                }
        //            }
        //        }
        //        if (purchase.PurchaseDetails == null)
        //        {
        //            foreach (var purchaseDetail in purchaseDetailsDb)
        //            {
        //                _purchaseDetailRepository.Delete(purchaseDetail.Id);
        //            }
        //        }
        //        else
        //        {
        //            foreach (var purchaseDetail in purchaseDetailsDb)
        //            {
        //                if (purchase.PurchaseDetails.All(r => r.Id != purchaseDetail.Id))
        //                {
        //                    _purchaseDetailRepository.Delete(purchaseDetail.Id);
        //                }
        //            }
        //        }
        //        #endregion

        //        #region Supplier Ledger
        //        var supplierLedger = _supplierLedgerRepository.GetOne(x => x.SupplierId == purchase.SupplierId && x.PurchaseId == purchase.Id);
        //        if (supplierLedger != null)
        //        {
        //            supplierLedger.DebitAmount = 0;
        //            supplierLedger.CreditAmount = purchase.NetAmount;
        //            supplierLedger.RunningBalance = purchase.NetAmount;
        //            supplierLedger.UpdatedBy = identity.Name;
        //            supplierLedger.UpdatedDate = DateTime.Now;
        //            supplierLedger.UpdatedFromIp = identity.IpAddress;
        //            _supplierLedgerRepository.Update(supplierLedger);
        //        }
        //        #endregion
        //        purchase.TotalQuantity = totalQty;
        //        purchase.TotalAmount = totalAmt;
        //        _purchaseRepository.Update(purchase);
        //        _unitOfWork.SaveChanges();
        //        flag = false;
        //        _unitOfWork.Commit();
        //        _rawSqlService.UpdateSupplierLedgerRunningBalance(supplier?.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    finally
        //    {
        //        if (flag)
        //            _unitOfWork.Rollback();
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Purchase GetById(string id)
        {
            try
            {
                return _purchaseRepository.GetOne(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Purchase> GetAllForReport(string id)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && r.Id == id, "Supplier").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PurchaseDetail> GetAllPurchaseDetailbyMasterIdForReport(string purchaseId)
        {
            try
            {
                return _purchaseDetailRepository.GetAll(x => !x.Archive && x.PurchaseId == purchaseId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Purchase> GetAll()
        {
            try
            {
                return _purchaseRepository.GetAll(x => !x.Archive).OrderByDescending(x => x.PurchaseDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Purchase> GetAllBySupplier(string supplierId)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && !r.FullyReturned && r.SupplierId == supplierId, "Supplier").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public IEnumerable<Purchase> GetAllByCompany(string companyId)
        {
            try
            {
                return _purchaseRepository.GetAll(r => r.CompanyId == companyId && !r.Archive).OrderByDescending(x => x.PurchaseDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<Purchase> GetAll(string companyId, string branchId)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && !r.FullyReturned && r.CompanyId == companyId && r.BranchId == branchId).OrderByDescending(x => x.PurchaseDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Purchase> GetAll(string companyId, string branchId, string supplierId)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && !r.FullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.SupplierId == supplierId, "Supplier").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseId"></param>
        /// <returns></returns>
        public IEnumerable<PurchaseDetail> GetAllPurchaseDetailbyMasterId(string purchaseId)
        {
            try
            {
                return _purchaseDetailRepository.GetAll(x => !x.Archive && !x.Returned && x.PurchaseId == purchaseId, "UOM").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Purchase> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && !r.FullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.PurchaseDate >= dateFrom && r.PurchaseDate <= dateTo).OrderByDescending(x => x.PurchaseDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Purchase> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo, string supplierId)
        {
            try
            {
                return _purchaseRepository.GetAll(r => !r.Archive && !r.FullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.PurchaseDate >= dateFrom && r.PurchaseDate <= dateTo && r.SupplierId == supplierId, "Supplier").OrderByDescending(x => x.PurchaseDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
