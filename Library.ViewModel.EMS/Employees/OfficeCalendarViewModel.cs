using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.EMS.Employees
{
    /// <summary>
    /// HolidayViewModel
    /// </summary>
    public class OfficeCalendarViewModel
    {
        #region Scalar
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        [Display(Name = "Day Type")]
        public string DayType { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public bool IsWeekday { get; set; }
        public bool IsWeekoff { get; set; }
        public bool IsHoliday { get; set; }
        public string Description { get; set; }
        #endregion

        #region Others
        public string Status { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        [Display(Name = "SBU")]
        public string BranchId { get; set; }
        [Display(Name = "SBU")]
        public string BranchName { get; set; }
        #endregion
    }
}
