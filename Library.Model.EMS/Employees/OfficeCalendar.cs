using Library.Crosscutting.Helper;
using System;

namespace Library.Model.EMS.Employees
{
    /// <summary>
    /// Office calendar settings entity.
    /// </summary>
    /// <remarks> Author: Jahangir Hossain Sheikh. </remarks>
    public class OfficeCalendar : BaseModel
    {
        #region Scalar
        /// <summary>
        /// Primary key.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Record active status.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Record delete status.
        /// Default is false.
        /// </summary>
        public bool Archive { get; set; }

        /// <summary>
        /// Code name.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Day of month.
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Month of year.
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Year
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Calendar date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Week/Working day flag.
        /// </summary>
        public bool IsWeekday { get; set; }
        /// <summary>
        /// Weeken/Weekoff day flag.
        /// </summary>
        public bool IsWeekoff { get; set; }

        /// <summary>
        /// Holiday flag.
        /// </summary>
        public bool IsHoliday { get; set; }

        /// <summary>
        /// Comments/Note if holiday.
        /// </summary>
        public string Description { get; set; }

        public string Status { get; set; }

        #endregion

        #region Navigation
        /// <summary>
        /// FK(Plain).
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// FK(Plain).
        /// </summary>
        public string BranchId { get; set; }
        #endregion

        #region Audit
        
        public string AddedBy { get; set; }
        
        public DateTime AddedDate { get; set; }
        
        public string AddedFromIP { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedFromIP { get; set; }
        #endregion
    }
}
