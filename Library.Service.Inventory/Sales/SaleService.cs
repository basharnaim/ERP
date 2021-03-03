using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Core.Core;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Customers;
using Library.Model.Inventory.Products;
using Library.Model.Inventory.Sales;
using Library.Service.Core.Core;
using Library.Context.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;
using System.Configuration;
using Library.Crosscutting.Helper;

namespace Library.Service.Inventory.Sales
{
    /// <summary>
    /// Class SaleService.
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public class SaleService : Service<Sale>, ISaleService
    {
        #region Ctor
        private readonly IRepository<Sale> _saleRepository;
        private readonly IRepository<SaleDetail> _saleDetailRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Branch> _branchRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerLedger> _customerLedgerRepository;
        private readonly IRawSqlService _rawSqlService;
        private readonly IUnitOfWork _unitOfWork;
        public SaleService(
            IRepository<Sale> saleRepository,
            IRepository<SaleDetail> saleDetailRepository,
            IRepository<Product> productRepository,
            IRepository<Branch> branchRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerLedger> customerLedgerRepository,
            IRawSqlService rawSqlService,
            IUnitOfWork unitOfWork
            ) : base(saleRepository)
        {
            _saleRepository = saleRepository;
            _saleDetailRepository = saleDetailRepository;
            _productRepository = productRepository;
            _branchRepository = branchRepository;
            _customerRepository = customerRepository;
            _customerLedgerRepository = customerLedgerRepository;
            _rawSqlService = rawSqlService;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Sale Service

        private DateTime MakeDateWithTime(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
        }

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

        public string Add(Sale sale)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var totalQty = 0m;
                var totalAmt = 0m;
                #region Customer
                var customer = _customerRepository.GetOne(x => x.Phone1 == sale.CustomerMobileNumber);
                if (string.IsNullOrEmpty(customer?.Id))
                {
                    customer = new Customer
                    {
                        Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "Customer"),
                        Sequence = GetAutoSequence("Customer"),
                        CompanyId = identity.CompanyId,
                        BranchId = identity.BranchId,
                        Name = sale.CustomerName,
                        DiscountRate = sale.CustomerDiscountInPercentage ?? 0,
                        Address1 = sale.CustomerAddress1,
                        Address2 = sale.CustomerAddress2,
                        Phone1 = sale.CustomerMobileNumber,
                        Active = true,
                        SynchronizationType = SynchronizationType.Server.ToString(),
                        AddedBy = identity.Name,
                        AddedDate = DateTime.Now,
                        AddedFromIp = identity.IpAddress
                    };
                    _customerRepository.Add(customer);
                }
                #endregion

                #region sale
                sale.Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "Sale");
                sale.Sequence = GetAutoSequence("Sale");
                sale.Barcode = GenerateBarcode(sale.Id);
                sale.Active = true;
                sale.SynchronizationType = SynchronizationType.Server.ToString();
                sale.CompanyId = identity.CompanyId;
                sale.BranchId = identity.BranchId;
                var branchWiseDiscount = _branchRepository.GetOne(x => x.Id == identity.BranchId).MaxDiscountRate;
                if (sale.DiscountInPercentage > branchWiseDiscount)
                {
                    sale.IsApprovedNeeded = true;
                }
                sale.CustomerPoint = (sale.CustomerPoint + sale.EarningPoint) - sale.ExpensePoint;
                sale.CustomerPointAmount = (sale.CustomerPointAmount + sale.EarningPointAmount) - sale.ExpensePointAmount;
                //DateTime saleDate = sale.SaleDate;
                //DateTime now = DateTime.Now;

                //DateTime saleDate = DateTime.Parse(sale.SaleDate.ToShortDateString()+" "+ now.Hour + ":" + now.Minute + ":" + now.Second); //"24 May 2009 02:19:00"              
                //sale.SaleDate = saleDate;

                sale.AddedBy = identity.Name;
                sale.AddedDate = DateTime.Now;
                sale.AddedFromIp = identity.IpAddress;
                #endregion

                #region sale Products
                var detailId = Convert.ToInt32(GenerateAutoId(identity.CompanyId, identity.BranchId, "SaleDetail"));
                if (sale.SaleDetails != null)
                {
                    var sqnc = GetAutoSequence("SaleDetail");
                    foreach (var saleDetail in sale.SaleDetails)
                    {
                        Product productDb = _productRepository.GetOne(x => x.Id == saleDetail.ProductId);
                        if (!string.IsNullOrEmpty(productDb?.Id))
                        {
                            totalQty += saleDetail.Quantity;
                            totalAmt += saleDetail.TotalAmount;
                            saleDetail.Id = detailId.ToString();
                            saleDetail.SaleId = sale.Id;
                            saleDetail.Sequence = sqnc;
                            saleDetail.ProductCode = productDb.Code;
                            saleDetail.ProductName = productDb.Name;
                            saleDetail.ProductCategoryId = productDb.ProductCategoryId;
                            saleDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
                            saleDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;
                            saleDetail.UOMId = productDb.UOMId;
                            saleDetail.RAMId = productDb.RAMId;
                            saleDetail.ROMId = productDb.ROMId;
                            saleDetail.SizeId = productDb.SizeId;
                            saleDetail.ColorId = productDb.ColorId;
                            saleDetail.StyleId = productDb.StyleId;
                            saleDetail.GradeId = productDb.GradeId;
                            saleDetail.SupplierId = productDb.SupplierId;
                            saleDetail.BrandId = productDb.BrandId;
                            saleDetail.PurchasePrice = productDb.PurchasePrice;
                            saleDetail.SaleDate = sale.SaleDate;
                            saleDetail.SaleDetailDate = DateTime.Now;
                            saleDetail.CompanyId = identity.CompanyId;
                            saleDetail.BranchId = identity.BranchId;
                            saleDetail.Active = true;
                            saleDetail.SynchronizationType = SynchronizationType.Server.ToString();
                            saleDetail.AddedBy = identity.Name;
                            saleDetail.AddedDate = DateTime.Now;
                            saleDetail.AddedFromIp = identity.IpAddress;
                            _saleDetailRepository.Add(saleDetail);
                            detailId++;
                            sqnc++;
                        }
                    }
                }
                #endregion

                #region CustomerLedger
                if (!string.IsNullOrEmpty(customer.Id))
                {
                    var customerLedger = new CustomerLedger
                    {
                        Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "CustomerLedger"),
                        Sequence = GetAutoSequence("CustomerLedger"),
                        TrackingNo = GenerateTrackingNo(identity.CompanyId, identity.BranchId, "CustomerLedger"),
                        SaleId = sale.Id,
                        CustomerId = customer.Id,
                        CustomerMobileNumber = customer.Phone1,
                        TransactionType = TransactionType.Sales.ToString(),
                        Particulars = TransactionType.Sales.ToString(),
                        TransactionDate = DateTime.Now,
                        DebitAmount = sale.DueAmount > 0 ? sale.PaidAmount : sale.NetAmount,
                        CreditAmount = sale.NetAmount,
                        RunningBalance = 0,
                        EarningPoint = sale.EarningPoint,
                        EarningPointAmount = sale.EarningPointAmount,
                        ExpensePoint = sale.ExpensePoint,
                        ExpensePointAmount = sale.ExpensePointAmount,
                        CompanyId = identity.CompanyId,
                        BranchId = identity.BranchId,
                        Active = true,
                        SynchronizationType = SynchronizationType.Server.ToString(),
                        AddedBy = identity.Name,
                        AddedDate = DateTime.Now,
                        AddedFromIp = identity.IpAddress
                    };
                    _customerLedgerRepository.Add(customerLedger);
                }
                #endregion
                sale.TotalQuantity = totalQty;
                sale.TotalAmount = totalAmt;
                _saleRepository.Add(sale);
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateCustomerLedgerRunningBalance(customer.Id);
                var user = "";
                var cameraip = "";
                var userid = "";
                var password = "";
                if (sale.AddedBy != null)
                {
                    user = sale.AddedBy.Substring(0, 5);
                    cameraip = ConfigurationManager.AppSettings["cameraip"];
                    userid = ConfigurationManager.AppSettings["userid"];
                    password = ConfigurationManager.AppSettings["password"];
                }

                var msg = "U:" + user + ", Q:" + sale.TotalQuantity + ", Tk:" + Math.Round((sale.TotalAmount - sale.ProductDiscount), 0);
                var msgTwo = "User:" + sale.AddedBy + ", Inv:" + sale.Id + ", Qty:" + sale.TotalQuantity + ", Tk:" + Math.Round(sale.TotalAmount);

                SendToHikVision(userid, password, cameraip, msg, msgTwo);
                //SendToHikVision("admin", "admin123", "10.11.1.15", msg, msgTwo); 
                return sale.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        private string SendToHikVision(string userName, string password, string ipAddress, string msg, string msgTwo)
        {
            try
            {
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create("http://" + ipAddress + "//ISAPI/System/Video/inputs/channels/1/overlays/text");
                wr.Accept = "*/*";
                wr.Method = "PUT";
                wr.ReadWriteTimeout = 320000;
                wr.Credentials = new NetworkCredential(userName, password);
                //var msg = text; //GetMessageFromDataBase();
                byte[] pBytes = GetDHCPPost(msg, msgTwo);
                wr.ContentLength = pBytes.Length;

                using (Stream ms = wr.GetRequestStream())
                {
                    ms.Write(pBytes, 0, pBytes.Length);
                    ms.Close();
                }
                wr.BeginGetResponse(r => { var reponse = wr.EndGetResponse(r); }, null);
                return "Data send successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private byte[] GetDHCPPost(string msg, string msgTwo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<TextOverlayList  version=\"2.0\" xmlns=\"http://www.hikvision.com/ver10/XMLSchema\">");

            sb.AppendLine("<TextOverlay>");
            sb.AppendLine("<id>1</id>");
            sb.AppendLine("<enabled>true</enabled>");
            sb.AppendLine("<positionX>10</positionX>");
            sb.AppendLine("<positionY>500</positionY>");
            sb.AppendLine("<displayText>" + msg + "</displayText>");
            sb.AppendLine("</TextOverlay>");

            //sb.AppendLine("<TextOverlay>");
            //sb.AppendLine("<id>2</id>");
            //sb.AppendLine("<enabled>true</enabled>");
            //sb.AppendLine("<positionX>10</positionX>");
            //sb.AppendLine("<positionY>470</positionY>");
            //sb.AppendLine("<displayText>" + msgTwo + "</displayText>");
            //sb.AppendLine("</TextOverlay>");

            //sb.AppendLine("<TextOverlay>");
            //sb.AppendLine("<id>3</id>");
            //sb.AppendLine("<enabled>true</enabled>");
            //sb.AppendLine("<positionX>10</positionX>");
            //sb.AppendLine("<positionY>440</positionY>");
            //sb.AppendLine("<displayText>" + msg + "</displayText>");
            //sb.AppendLine("</TextOverlay>");

            //sb.AppendLine("<TextOverlay>");
            //sb.AppendLine("<id>4</id>");
            //sb.AppendLine("<enabled>true</enabled>");
            //sb.AppendLine("<positionX>10</positionX>");
            //sb.AppendLine("<positionY>410</positionY>");
            //sb.AppendLine("<displayText>" + msgTwo + "</displayText>");
            //sb.AppendLine("</TextOverlay>");

            sb.AppendLine("</TextOverlayList>");
            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        public string Update(Sale sale)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var totalQty = 0m;
                var totalAmt = 0m;
                #region Customer
                var customer = _customerRepository.GetOne(x => x.Phone1 == sale.CustomerMobileNumber);
                if (string.IsNullOrEmpty(customer?.Id))
                {
                    customer = new Customer
                    {
                        Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "Customer"),
                        Sequence = GetAutoSequence("Customer"),
                        CompanyId = identity.CompanyId,
                        BranchId = identity.BranchId,
                        Name = sale.CustomerName,
                        DiscountRate = sale.CustomerDiscountInPercentage ?? 0,
                        Address1 = sale.CustomerAddress1,
                        Address2 = sale.CustomerAddress2,
                        Phone1 = sale.CustomerMobileNumber,
                        Active = true,
                        SynchronizationType = SynchronizationType.Server.ToString(),
                        AddedBy = identity.Name,
                        AddedDate = DateTime.Now,
                        AddedFromIp = identity.IpAddress
                    };
                    _customerRepository.Add(customer);
                }
                #endregion
                #region sale
                Sale saleDb = _saleRepository.GetOne(x => x.Id == sale.Id);
                var saleDetailsDb = _saleDetailRepository.GetAll(x => x.SaleId == sale.Id).ToList();
                sale.Sequence = saleDb.Sequence;
                sale.SynchronizationType = saleDb.SynchronizationType;
                sale.SaleDate = saleDb.SaleDate;
                sale.CompanyId = saleDb.CompanyId;
                sale.BranchId = saleDb.BranchId;
                var branchWiseDiscount = _branchRepository.GetOne(x => x.Id == saleDb.BranchId).MaxDiscountRate;
                if (sale.DiscountInPercentage > branchWiseDiscount)
                {
                    sale.IsApprovedNeeded = true;
                }
                sale.AddedBy = saleDb.AddedBy;
                sale.AddedDate = saleDb.AddedDate;
                sale.AddedFromIp = saleDb.AddedFromIp;
                sale.Active = saleDb.Active;
                sale.ApprovedBy = saleDb.ApprovedBy;
                sale.UpdatedBy = identity.Name;
                sale.UpdatedDate = DateTime.Now;
                sale.UpdatedFromIp = identity.IpAddress;
                sale.Archive = saleDb.Archive;
                sale.IsFullyDelivered = true;

                #region sale Detail
                var detailId = Convert.ToInt32(GenerateAutoId(identity.CompanyId, identity.BranchId, "SaleDetail"));
                if (sale.SaleDetails != null)
                {
                    var sqnc = GetAutoSequence("SaleDetail");
                    foreach (var saleDetail in sale.SaleDetails)
                    {
                        Product productDb = _productRepository.GetOne(x => x.Id == saleDetail.ProductId);
                        var saleDetailDb = _saleDetailRepository.GetOne(x => x.Id == saleDetail.Id);
                        if (saleDetailDb != null)
                        {
                            #region Update
                            totalQty += saleDetail.Quantity;
                            totalAmt += saleDetail.TotalAmount;
                            saleDetail.ProductCode = productDb.Code;
                            saleDetail.ProductName = productDb.Name;
                            saleDetail.ProductCategoryId = productDb.ProductCategoryId;
                            saleDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
                            saleDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;
                            saleDetail.UOMId = productDb.UOMId;
                            saleDetail.RAMId = productDb.RAMId;
                            saleDetail.ROMId = productDb.ROMId;
                            saleDetail.SizeId = productDb.SizeId;
                            saleDetail.ColorId = productDb.ColorId;
                            saleDetail.StyleId = productDb.StyleId;
                            saleDetail.GradeId = productDb.GradeId;
                            saleDetail.SupplierId = productDb.SupplierId;
                            saleDetail.BrandId = productDb.BrandId;
                            saleDetail.PurchasePrice = productDb.PurchasePrice;
                            saleDetail.Sequence = saleDetailDb.Sequence;
                            saleDetail.SaleDate = saleDetailDb.SaleDate;
                            saleDetail.SaleDetailDate = saleDetailDb.SaleDetailDate;
                            saleDetail.SaleId = saleDetailDb.SaleId;
                            saleDetail.CompanyId = saleDetailDb.CompanyId;
                            saleDetail.BranchId = saleDetailDb.BranchId;
                            saleDetail.AddedBy = saleDetailDb.AddedBy;
                            saleDetail.AddedDate = saleDetailDb.AddedDate;
                            saleDetail.AddedFromIp = saleDetailDb.AddedFromIp;
                            saleDetail.SynchronizationType = saleDetailDb.SynchronizationType;
                            saleDetail.UpdatedBy = identity.Name;
                            saleDetail.UpdatedDate = DateTime.Now;
                            saleDetail.UpdatedFromIp = identity.IpAddress;
                            saleDetail.Active = saleDetailDb.Active;
                            _saleDetailRepository.Update(saleDetail);

                            #endregion
                        }
                        else
                        {
                            #region Create
                            totalQty += saleDetail.Quantity;
                            totalAmt += saleDetail.TotalAmount;
                            saleDetail.Id = detailId.ToString();
                            saleDetail.SaleId = sale.Id;
                            saleDetail.Sequence = sqnc;
                            saleDetail.ProductCode = productDb.Code;
                            saleDetail.ProductName = productDb.Name;
                            saleDetail.ProductCategoryId = productDb.ProductCategoryId;
                            saleDetail.ProductSubCategoryId = productDb.ProductSubCategoryId;
                            saleDetail.ProductSubsidiaryCategoryId = productDb.ProductSubsidiaryCategoryId;
                            saleDetail.UOMId = productDb.UOMId;
                            saleDetail.RAMId = productDb.RAMId;
                            saleDetail.ROMId = productDb.ROMId;
                            saleDetail.SizeId = productDb.SizeId;
                            saleDetail.ColorId = productDb.ColorId;
                            saleDetail.StyleId = productDb.StyleId;
                            saleDetail.GradeId = productDb.GradeId;
                            saleDetail.SupplierId = productDb.SupplierId;
                            saleDetail.BrandId = productDb.BrandId;
                            saleDetail.PurchasePrice = productDb.PurchasePrice;
                            saleDetail.SaleDate = sale.SaleDate;
                            saleDetail.SaleDetailDate = DateTime.Now;
                            saleDetail.CompanyId = identity.CompanyId;
                            saleDetail.BranchId = identity.BranchId;
                            saleDetail.Active = true;
                            saleDetail.SynchronizationType = SynchronizationType.Server.ToString();
                            saleDetail.AddedBy = identity.Name;
                            saleDetail.AddedDate = DateTime.Now;
                            saleDetail.AddedFromIp = identity.IpAddress;
                            _saleDetailRepository.Add(saleDetail);
                            detailId++;
                            sqnc++;

                            #endregion
                        }
                    }
                }
                if (sale.SaleDetails == null)
                {
                    foreach (var saleDetail in saleDetailsDb)
                    {
                        _saleDetailRepository.Delete(saleDetail.Id);
                    }
                }
                else
                {
                    foreach (var saleDetail in saleDetailsDb)
                    {
                        if (sale.SaleDetails.All(r => r.Id != saleDetail.Id))
                        {
                            _saleDetailRepository.Delete(saleDetail.Id);
                        }
                    }
                }
                #endregion

                #region CustomerLedger
                //if (!string.IsNullOrEmpty(customer.Id))
                //{
                //    var customerLedger = new CustomerLedger
                //    {
                //        CompanyId = identity.CompanyId,
                //        BranchId = identity.BranchId,
                //        Id = GenerateAutoId(identity.CompanyId, identity.BranchId, "CustomerLedger"),
                //        Sequence = GetAutoSequence("CustomerLedger"),
                //        SaleId = sale.Id,
                //        TrackingNo = GenerateTrackingNo(identity.CompanyId, identity.BranchId, "CustomerLedger"),
                //        CustomerId = customer.Id,
                //        CustomerMobileNumber = customer.Phone1,
                //        TransactionType = TransactionType.Sales.ToString(),
                //        Particulars = TransactionType.Sales.ToString(),
                //        TransactionDate = DateTime.Now,
                //        DebitAmount = sale.DueAmount > 0 ? sale.PaidAmount : sale.NetAmount,
                //        CreditAmount = sale.NetAmount,
                //        EarningPoint = sale.EarningPoint,
                //        EarningPointAmount = sale.EarningPointAmount,
                //        ExpensePoint = sale.ExpensePoint,
                //        ExpensePointAmount = sale.ExpensePointAmount,
                //        Active = true,
                //        SynchronizationType = SynchronizationType.Server.ToString(),
                //        AddedBy = identity.Name,
                //        AddedDate = DateTime.Now,
                //        AddedFromIp = identity.IpAddress
                //    };
                //    _customerLedgerRepository.Add(customerLedger);
                //}
                #endregion

                sale.TotalQuantity = totalQty;
                sale.TotalAmount = totalAmt;
                _saleRepository.Update(sale);

                #endregion
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateCustomerLedgerRunningBalance(customer.Id);
                return sale.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sale"></param>
        public void Approve(Sale sale)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                #region sale
                decimal totalprofit = 0m;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                Sale dbsale = _saleRepository.GetOne(x => x.Id == sale.Id);
                sale.SynchronizationType = dbsale.SynchronizationType;
                sale.Sequence = dbsale.Sequence;
                sale.SaleDate = dbsale.SaleDate;
                sale.CompanyId = dbsale.CompanyId;
                sale.BranchId = dbsale.BranchId;
                sale.IsApprovedNeeded = dbsale.IsApprovedNeeded;
                sale.IsApproved = true;
                sale.ApprovedBy = identity.Name;
                sale.AddedBy = dbsale.AddedBy;
                sale.AddedDate = dbsale.AddedDate;
                sale.AddedFromIp = dbsale.AddedFromIp;
                sale.Active = dbsale.Active;
                sale.UpdatedBy = identity.Name;
                sale.UpdatedDate = DateTime.Now;
                sale.UpdatedFromIp = identity.IpAddress;
                sale.Archive = dbsale.Archive;

                #endregion

                #region sale Products
                if (sale.SaleDetails != null)
                {
                    foreach (var product in sale.SaleDetails)
                    {
                        SaleDetail saleDetailDb = _saleDetailRepository.GetOne(x => x.Id == product.Id);
                        Product dbProduct = _productRepository.GetOne(x => x.Id == product.ProductId);
                        product.ProductCategoryId = dbProduct.ProductCategoryId;
                        product.ProductSubCategoryId = dbProduct.ProductSubCategoryId;
                        product.ProductSubsidiaryCategoryId = dbProduct.ProductSubsidiaryCategoryId;
                        product.Sequence = saleDetailDb.Sequence;
                        product.SaleDate = saleDetailDb.SaleDate;
                        product.SaleDetailDate = saleDetailDb.SaleDetailDate;
                        product.ProductId = dbProduct.Id;
                        product.UOMId = dbProduct.UOMId;
                        product.SaleId = saleDetailDb.SaleId;
                        product.CompanyId = saleDetailDb.CompanyId;
                        product.BranchId = saleDetailDb.BranchId;
                        product.AddedBy = saleDetailDb.AddedBy;
                        product.AddedDate = saleDetailDb.AddedDate;
                        product.AddedFromIp = saleDetailDb.AddedFromIp;
                        product.SynchronizationType = saleDetailDb.SynchronizationType;
                        product.UpdatedBy = identity.Name;
                        product.UpdatedDate = DateTime.Now;
                        product.UpdatedFromIp = identity.IpAddress;
                        product.Active = saleDetailDb.Active;
                        _saleDetailRepository.Update(product);
                    }
                }
                #endregion
                sale.TotalProfit = Math.Round(totalprofit, 2);
                _saleRepository.Update(sale);

                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir, 9-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Sale.</returns>
        public Sale GetById(string id)
        {
            try
            {
                return _saleRepository.GetOne(x => x.Id == id, "Customer");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 9-11-2015</remarks>
        /// <returns>IEnumerable&lt;Sale&gt;.</returns>
        public IEnumerable<Sale> GetAll()
        {

            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        public IEnumerable<Sale> GetAllForReport()
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Sale> GetAllForReport(string id)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.Id == id, "Customer").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public IEnumerable<Sale> GetAllByCustomer(string customerId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && !r.IsFullyDelivered && r.CustomerId == customerId, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Sale> GetAllByCompany(string companyId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && !r.IsFullyDelivered && r.CompanyId == companyId, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        public IEnumerable<Sale> GetAll(string companyId, string branchId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && !r.IsFullyDelivered && r.CompanyId == companyId && r.BranchId == branchId, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && !r.IsFullyDelivered && r.CompanyId == companyId && r.BranchId == branchId
                 && r.SaleDate >= dateFrom && r.SaleDate <= dateTo, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
                 //&& (r.SaleDate.Date >= dateFrom.Date && r.SaleDate.Date <= dateTo.Date), "Customer"); //.OrderByDescending(x => x.SaleDate.Date).AsEnumerable();
                 //r.SaleDate.Date >= dateFrom.Date
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Sale> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo, string customerId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && !r.IsFullyDelivered && r.CompanyId == companyId && r.BranchId == branchId && r.SaleDate >= dateFrom && r.SaleDate <= dateTo && r.CustomerId == customerId, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        public IEnumerable<Sale> GetAllForApprove()
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.IsApprovedNeeded && !r.IsApproved, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        public IEnumerable<Sale> GetAllForApprove(string companyId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.IsApprovedNeeded && !r.IsApproved, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        public IEnumerable<Sale> GetAllForApprove(string companyId, string branchId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.IsApprovedNeeded && !r.IsApproved, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllForApprove(string companyId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.IsApprovedNeeded && !r.IsApproved && r.SaleDate >= dateFrom && r.SaleDate <= dateTo, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllForApprove(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.IsApprovedNeeded && !r.IsApproved && r.SaleDate >= dateFrom && r.SaleDate <= dateTo, "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        public IEnumerable<SaleDetail> GetAllSaleDetailbyMasterId(string masterId)
        {
            try
            {
                return _saleDetailRepository.GetAll(x => !x.Archive && !x.IsReturned && !x.IsCancelled && x.SaleId == masterId, "UOM").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        public IEnumerable<SaleDetail> GetAllSaleDetailbyMasterIdForReport(string masterId)
        {
            try
            {
                return _saleDetailRepository.GetAll(x => !x.Archive && !x.IsReturned && !x.IsCancelled && x.SaleId == masterId, "Product, UOM").AsEnumerable();
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
        public IEnumerable<Sale> GetAllForDelivery()
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyDelivered && (r.IsApproved || !r.IsApprovedNeeded), "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        public IEnumerable<Sale> GetAllForDelivery(string companyId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && !r.IsFullyDelivered && (r.IsApproved || !r.IsApprovedNeeded), "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        public IEnumerable<Sale> GetAllForDelivery(string companyId, string branchId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && !r.IsFullyDelivered && (r.IsApproved || !r.IsApprovedNeeded), "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllForDelivery(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && !r.IsFullyDelivered && r.SaleDate >= dateFrom && r.SaleDate <= dateTo && (r.IsApproved || !r.IsApprovedNeeded), "Customer").OrderByDescending(x => x.SaleDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        public IEnumerable<SaleDetail> GetAllSaleDetailForDeliberybyMasterId(string masterId)
        {
            try
            {
                return _saleDetailRepository.GetAll(x => !x.Archive && !x.IsDelivered && x.SaleId == masterId);
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
        /// <param name="invoiceNo"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByInvoiceNoWithDate(string companyId, string branchId, string invoiceNo, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.Id == invoiceNo.Trim() && r.SaleDate >= dateFrom && r.SaleDate <= dateTo).OrderByDescending(x => x.SaleDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Sale> GetAllInvoiceByCustomerNameWithDate(string companyId, string branchId, string customerName, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.CustomerId == customerName && r.SaleDate >= dateFrom && r.SaleDate <= dateTo).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="customerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByCustomerIdWithDate(string companyId, string branchId, string customerId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.CustomerId == customerId && r.SaleDate >= dateFrom && r.SaleDate <= dateTo).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByCompanyBranchCustomerId(string companyId, string branchId, string customerId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.CustomerId == customerId.Trim()).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByCompanyCustomerId(string companyId, string customerId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.CustomerId == customerId.Trim()).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="customerPhone"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByCustomerPhoneWithDate(string companyId, string branchId, string customerPhone, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var customerId = _customerRepository.GetAll(x => x.Phone1 == customerPhone).FirstOrDefault().Id;
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.CustomerId == customerId && r.SaleDate >= dateFrom && r.SaleDate <= dateTo).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByInvoiceNo(string companyId, string branchId, string invoiceNo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.Id == invoiceNo).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="customerName"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByCustomerName(string companyId, string branchId, string customerName)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.CustomerId == customerName).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="customerPhone"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByCustomerPhone(string companyId, string branchId, string customerPhone)
        {
            try
            {
                var customerId = _customerRepository.GetAll(x => x.Phone1 == customerPhone.Trim()).FirstOrDefault().Id;
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.CustomerId == customerId).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        public IEnumerable<Sale> GetAllInvoiceByCompanyId(string companyId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.BranchId == branchId && r.SaleDate >= dateFrom && r.SaleDate <= dateTo).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByCompanyIdWithDateRange(string companyId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.SaleDate >= dateFrom && r.SaleDate <= dateTo).OrderByDescending(x => x.SaleDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByCustomerIdWithDateRange(string customerId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CustomerId == customerId && r.SaleDate >= dateFrom && r.SaleDate <= dateTo).OrderByDescending(x => x.SaleDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllInvoiceByCustomerId(string customerId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CustomerId == customerId).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        public IEnumerable<Sale> GetAllInvoiceByCompanyAndBranchId(string companyId, string branchId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && !r.IsFullyReturned && r.CompanyId == companyId && r.BranchId == branchId).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSalesIdsByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(x => !x.Archive && !x.IsFullyReturned && x.CompanyId == companyId && x.BranchId == branchId && x.SaleDate >= dateFrom && x.SaleDate <= dateTo).Select(x => x.Id).AsEnumerable();
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
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSalesIdsByCompanyBranchIdWithCustomerId(string companyId, string branchId, string customerId)
        {
            try
            {
                return _saleRepository.GetAll(x => !x.Archive && !x.IsFullyReturned && x.CompanyId == companyId && x.BranchId == branchId && x.CustomerId == customerId).Select(x => x.Id).AsEnumerable();
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
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSalesIdsByCompanyCustomerId(string companyId, string customerId)
        {
            try
            {
                return _saleRepository.GetAll(x => !x.Archive && !x.IsFullyReturned && x.CompanyId == companyId && x.CustomerId == customerId).Select(x => x.Id).AsEnumerable();
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
        public IEnumerable<string> GetSalesIdsByCompanyAndBranchId(string companyId, string branchId)
        {
            try
            {
                return _saleRepository.GetAll(x => !x.Archive && !x.IsFullyReturned && x.CompanyId == companyId && x.BranchId == branchId).Select(x => x.Id).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSalesIdsByCompanyIdWithDateRange(string companyId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(x => !x.Archive && !x.IsFullyReturned && x.CompanyId == companyId && x.SaleDate >= dateFrom && x.SaleDate <= dateTo).Select(x => x.Id).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSalesIdsByCustomerIdWithDateRange(string customerId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(x => !x.Archive && !x.IsFullyReturned && x.CustomerId == customerId && x.SaleDate >= dateFrom && x.SaleDate <= dateTo).Select(x => x.Id).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSalesIdsByCustomerId(string customerId)
        {
            try
            {
                return _saleRepository.GetAll(x => !x.Archive && !x.IsFullyReturned && x.CustomerId == customerId).Select(x => x.Id).AsEnumerable();
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
        /// <param name="customerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSalesIdsByCompanyBranchCustomerIdWithDate(string companyId, string branchId, string customerId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(x => !x.Archive && !x.IsFullyReturned && x.CompanyId == companyId && x.BranchId == branchId && x.SaleDate >= dateFrom && x.SaleDate <= dateTo).Select(x => x.Id).AsEnumerable();
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
        public IEnumerable<string> GetSalesIdsByCompanyId(string companyId)
        {
            try
            {
                return _saleRepository.GetAll(x => !x.Archive && !x.IsFullyReturned && x.CompanyId == companyId).Select(x => x.Id).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleId"></param>
        /// <returns></returns>
        public decimal GetNetProfit(string saleId)
        {
            try
            {
                var sale = _saleRepository.GetOne(saleId);
                var totalProfit = sale.TotalProfit.Value;
                var overallDiscount = sale.OverAllDiscount;
                decimal netProfit = totalProfit - overallDiscount.Value;
                return netProfit;
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
        public decimal GetAllPendingSales(string companyId, string branchId)
        {
            try
            {
                return _saleRepository.GetAll(x => !x.Archive && x.IsFullyDelivered == false && x.CompanyId == companyId && x.BranchId == branchId).Count();
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
        public IEnumerable<Sale> GetAllForReportDelivery()
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.IsFullyDelivered && (r.IsApproved || !r.IsApprovedNeeded)).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        public IEnumerable<Sale> GetAllForReportDelivery(string companyId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.IsFullyDelivered && r.CompanyId == companyId && (r.IsApproved || !r.IsApprovedNeeded)).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        public IEnumerable<Sale> GetAllForReportDelivery(string companyId, string branchId)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.IsFullyDelivered && r.CompanyId == companyId && r.BranchId == branchId && (r.IsApproved || !r.IsApprovedNeeded)).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetAllForReportDelivery(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _saleRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.SaleDate >= dateFrom && r.SaleDate <= dateTo && (r.IsApproved || !r.IsApprovedNeeded)).OrderByDescending(x => x.SaleDate).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Collectionvm> GetCollectionInformation(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var result = (from itm in _saleRepository.SqlQuery<Collectionvm>("exec [GetCollectionInformation] @companyId, @branchId, @fromDate, @toDate",
                                  new SqlParameter("@companyId", companyId),
                                  new SqlParameter("@branchId", branchId),
                                  new SqlParameter("@fromDate", dateFrom),
                                  new SqlParameter("@toDate", dateTo)
                                  ).ToArray().ToList()
                              select new Collectionvm
                              {
                                  SaleNo = itm.SaleNo ?? "",
                                  MoneyReceiveNo = itm.MoneyReceiveNo ?? "",
                                  Particulars = itm.Particulars ?? "Sales",
                                  Date = itm.SaleDate ?? itm.TransactionDate.Value,
                                  Amount = itm.ActualPaidAmount ?? itm.DebitAmount.Value
                              }).AsEnumerable();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}