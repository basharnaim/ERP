using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.WebUI.Models.ViewModels
{
    public class VmProductStockMaster
    {
        public string StockID { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int? SupplierID { get; set; }
        public int? CategoryID { get; set; }
        public string ProductID { get; set; }
        
        public int UOMID { get; set; }
        [DisplayName("Receive Rate")]
        public decimal? ReceiveRate { get; set; } 
        [DisplayName("Receive Qty")]
        public decimal? ReceiveQty { get; set; }
        [DisplayName("Receive Value")]
        public decimal? ReceiveValue { get; set; }
        [DataType(DataType.Date)]
        public DateTime LastReceiveDate { get; set; }
        [DisplayName("Branch Name")]
        public decimal IssueQty { get; set; }
        [DisplayName("Issue Value")]
        public decimal IssueValue { get; set; }
        [DataType(DataType.Date)]
        public DateTime LastIssueDate { get; set; }
        [DisplayName("Current Rate")]
        public decimal CurrentRate { get; set; }
        [DisplayName("Current Stock")]
        public decimal? CurrentStock { get; set; }
        [DisplayName("Current Value")]
        public decimal CurrentValue { get; set; }
        [DisplayName("Sales Rate")]
        public decimal SalesRate { get; set; }
        [DisplayName("Sales Value")]
        public decimal SalesValue { get; set; }
        [DataType(DataType.Date)]
        public DateTime LastSalesDate { get; set; }
        public bool IsActive { get; set; }
        public int CreateBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreateOn { get; set; }
        public string CreatePc { get; set; }
        public int UpdateBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime UpdateOn { get; set; }
        public string UpdatePc { get; set; }
        public bool IsDeleted { get; set; }
        public int DeleteBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime DeleteOn { get; set; }
        public string DeletePc { get; set; }

        [DisplayName("Product Code")]
        public string ProductCode { get; set; }
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        [DisplayName("Description")]
        public string ProdcutDescription { get; set; }

        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("Supplier Name")]
        public string SupplierName { get; set; }
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("UOM Name")]
        public string UOMName { get; set; }
        public List<VmProductStockMaster> ProductStockMasters { get; set; }

    }
}