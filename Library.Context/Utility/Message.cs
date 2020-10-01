namespace Library.Context.Utility
{
    public class Message
    {
        public static string PoApprovalLimit = " Amount is exit Your Approval Limit, Please contact with Administrator";
        public static string PoBudgetAmountExistInRequisition = " Amount is crossed the budget Limit, Please contact with Administrator";
        public static string RequestInfo = " Request successfully updated";
        public static string NoAccessMsg = "You have no Access to this service";
        public static string AlreadyBookedMessage = "Seat already Booked !";
        public static string AlreadyPurchasedMessage = "Seat already Purchased !";
        public static string AlreadyExistMessage = "Already Exist !";
        public static string NotExistMessage = "Not Exist !";
        public static string AlreadyPosted = "Already Posted";

        #region requisition mail
        public static string RequisitionMailSubject = "Requisition is waiting for your approval";
        public static string RequisitionMailNoReceiver = "There have no approval person for approve this requisition.";
        public static string RequisitionNewInfo = "Requisition is waiting for your approval (level 1) .";
        public static string RequisitionApprove1Info = "Level 1 already approved. Waiting for next level approval.";
        public static string RequisitionApprove2Info = "Level 2 already approved. Waiting for next level approval."; 
        public static string RequisitionApprove3Info = "Level 3 already approved.";
        #endregion requisition mail

        #region quotation mail
        public static string QuotationMailSubject = "Quotation is waiting for your approval";
        public static string QuotationMailNoReceiver = "There have no approval person for approve this quotation.";
        public static string QuotationNewInfo = "Quotation is waiting for your approval (level 1) .";
        public static string QuotationApprove1Info = "Level 1 already approved. Waiting for next level approval.";
        public static string QuotationApprove2Info = "Level 2 already approved. Waiting for next level approval.";
        public static string QuotationApprove3Info = "Level 3 already approved.";
        #endregion quotation mail 

        #region po mail
        public static string PoMailSubject = "Purchase Order is waiting for your approval";
        public static string PoMailNoReceiver = "There have no approval person for approve this purchase order.";
        public static string PoNewInfo = "Purchase Order is waiting for your approval (level 1) .";
        public static string PoApprove1Info = "Level 1 already approved. Waiting for next level approval.";
        public static string PoApprove2Info = "Level 2 already approved. Waiting for next level approval.";
        public static string PoApprove3Info = "Level 3 already approved.";
        #endregion po mail

        #region pi mail
        public static string PiMailSubject = "Purchase Invoice is waiting for your approval";
        public static string PiMailNoReceiver = "There have no approval person for approve this purchase invoice.";
        public static string PiNewInfo = "Purchase Invoice is waiting for your approval (level 1) .";
        public static string PiApprove1Info = "Level 1 already approved. Waiting for next level approval.";
        public static string PiApprove2Info = "Level 2 already approved. Waiting for next level approval.";
        public static string PiApprove3Info = "Level 3 already approved.";
        #endregion pi mail 

        #region grn mail
        public static string GrnMailSubject = "Goods Receive is waiting for your approval";
        public static string GrnMailNoReceiver = "There have no approval person for approve this goods receive.";
        public static string GrnNewInfo = "Goods Receive is waiting for your approval (level 1) .";
        public static string GrnApprove1Info = "Level 1 already approved. Waiting for next level approval.";
        public static string GrnApprove2Info = "Level 2 already approved. Waiting for next level approval.";
        public static string GrnApprove3Info = "Level 3 already approved.";
        #endregion grn mail
    }
}
