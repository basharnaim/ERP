using Library.Model.EMS.Employees;
using System;
using System.Collections.Generic;

namespace Library.Service.EMS.Employees
{
    /// <summary>
    /// IHoliday Service
    /// </summary>
    public interface IOfficeCalendarService
    {
        /// <summary>
        /// Add 
        /// </summary>
        /// <param name="companyId">string</param>
        /// <param name="branchId">string</param>
        /// <param name="month">int</param>
        /// <param name="year">int</param>
        void Add(string companyId, string branchId, string year, string month);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="holiday"></param>
        void Update(OfficeCalendar holiday);

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OfficeCalendar Get(string id);

        IEnumerable<object> GetAll(int month, int year);
        /// <summary>
        /// GetAll
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        IEnumerable<object> GetAll(string companyId, string branchId, int year, int month);
        /// <summary>
        /// IsSaved
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        bool IsSaved(string companyId, string branchId, int year, int month);

        /// <summary>
        /// GetHolidays
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        List<OfficeCalendar> GetHolidays(DateTime fromDate, DateTime toDate);

        OfficeCalendar GetTodayStatus(string companyId, string branchId, int year, int month, DateTime date);
    }
}
