using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Core.Organizations;
using Library.Model.EMS.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Library.Service.EMS.Employees
{
    public class OfficeCalendarService : IOfficeCalendarService
    {
        private readonly IRepository<OfficeCalendar> _holidayRepository;
        private readonly IRepository<Branch> _branchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OfficeCalendarService(
            IRepository<OfficeCalendar> holidayRepository,
            IRepository<Branch> branchRepository,
            IUnitOfWork unitOfWork
            )
        {
            _holidayRepository = holidayRepository;
            _branchRepository = branchRepository;
            _unitOfWork = unitOfWork;
        }
        private int GetAutoId()
        {
            var idList = _holidayRepository.GetAll().Select(x => x.Id).ToList();
            return idList.Any() ? idList.Max(x => Convert.ToInt32(x) + 1) : 1;
        }
        public void Add(string companyId, string branchId, string year, string month)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(month))
                {
                    var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                    DateTime firstDay = new DateTime(Convert.ToInt16(year), Convert.ToInt16(month), 1);
                    int day = DateTime.DaysInMonth(Convert.ToInt16(year), Convert.ToInt16(month));
                    int holidayId = GetAutoId();
                    for (int i = 1; i <= day; i++)
                    {
                        OfficeCalendar officeCalendar = new OfficeCalendar
                        {
                            Id = holidayId.ToString()
                        };
                        holidayId++;
                        officeCalendar.Day = i;
                        officeCalendar.Month = Convert.ToInt16(month);
                        officeCalendar.Year = Convert.ToInt16(year);
                        officeCalendar.Date = firstDay;
                        firstDay = firstDay.AddDays(1);
                        officeCalendar.IsWeekday = true;
                        officeCalendar.CompanyId = companyId;
                        officeCalendar.BranchId = branchId;
                        officeCalendar.AddedDate = DateTime.Now;
                        officeCalendar.AddedBy = identity.Name;
                        officeCalendar.AddedFromIP = identity.IpAddress;
                        if (!_holidayRepository.Any(x => x.Year == officeCalendar.Year && x.Month == officeCalendar.Month && x.Date == officeCalendar.Date && x.CompanyId == officeCalendar.CompanyId && x.BranchId == officeCalendar.BranchId))
                        {
                            _holidayRepository.Add(officeCalendar);
                        }
                    }
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Update(OfficeCalendar holiday)
        {
            try
            {
                if (!string.IsNullOrEmpty(holiday.CompanyId) && !string.IsNullOrEmpty(holiday.BranchId) && !string.IsNullOrEmpty(holiday.Year.ToString()) && !string.IsNullOrEmpty(holiday.Month.ToString()))
                {
                    var holidayOld = _holidayRepository.GetOne(x => x.Day == holiday.Day && x.Month == holiday.Month && x.Year == holiday.Year && x.CompanyId == holiday.CompanyId && x.BranchId == holiday.BranchId);
                    holidayOld.Description = holiday.Description;
                    _holidayRepository.Update(holidayOld);
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public OfficeCalendar Get(string id)
        {
            try
            {
                var holiday = _holidayRepository.GetOne(x => !x.Archive && x.Id == id );
                return new OfficeCalendar
                {
                    Id = holiday.Id,
                    Code = holiday.Code,
                    Active = holiday.Active,
                    Archive = holiday.Archive,
                    Description = holiday.Description,
                    CompanyId = holiday.CompanyId,
                    BranchId = holiday.BranchId,
                    Date = holiday.Date,
                    Day = holiday.Day,
                    IsHoliday = holiday.IsHoliday,
                    IsWeekday = holiday.IsWeekday,
                    IsWeekoff = holiday.IsWeekoff,
                    Month = holiday.Month,
                    Year = holiday.Year
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<object> GetAll(int month, int year)
        {
            try
            {
                var holiday = (from h in _holidayRepository.GetAll(x => !x.Archive && x.Month == month && x.Year == year)
                               select new
                               {
                                   h.Id,
                                   h.Code,
                                   h.Active,
                                   h.Archive,
                                   h.Description,
                                   h.CompanyId,
                                   h.BranchId,
                                   h.Date,
                                   h.Day,
                                   h.IsHoliday,
                                   h.IsWeekday,
                                   h.IsWeekoff,
                                   h.Month,
                                   h.Year,
                                   Status = h.IsWeekday ? "Weekday" : (h.IsWeekoff ? "Weekoff" : "Holiday")
                               }).ToList();
                return holiday;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<object> GetAll(string companyId, string branchId, int year, int month)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(year.ToString()) && !string.IsNullOrEmpty(month.ToString()))
                {
                    var holiday = (from h in _holidayRepository.GetAll(x => !x.Archive && x.CompanyId == companyId && x.BranchId == branchId && x.Year == year && x.Month == month)
                                   select new
                                   {
                                       h.Id,
                                       h.Code,
                                       h.Active,
                                       h.Archive,
                                       h.Description,
                                       h.CompanyId,
                                       h.BranchId,
                                       h.Date,
                                       h.Day,
                                       h.IsHoliday,
                                       h.IsWeekday,
                                       h.IsWeekoff,
                                       h.Month,
                                       h.Year,
                                       Status = h.IsWeekday ? "Weekday" : (h.IsWeekoff ? "Weekoff" : "Holiday")
                                   }).ToList();
                    return holiday;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool IsSaved(string companyId, string branchId, int year, int month)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(year.ToString()) && !string.IsNullOrEmpty(month.ToString()))
                {
                    return _holidayRepository.Any(x => x.CompanyId == companyId && x.BranchId == branchId && x.Month == month && x.Year == year);
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<OfficeCalendar> GetHolidays(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var holiday = (from h in _holidayRepository.GetAll(x => !x.Archive && x.Date >= fromDate && x.Date <= toDate)
                               select new OfficeCalendar
                               {
                                   Id = h.Id,
                                   Code = h.Code,
                                   Active = h.Active,
                                   Archive = h.Archive,
                                   Description = h.Description,
                                   CompanyId = h.CompanyId,
                                   BranchId = h.BranchId,
                                   Date = h.Date,
                                   Day = h.Day,
                                   IsHoliday = h.IsHoliday,
                                   IsWeekday = h.IsWeekday,
                                   IsWeekoff = h.IsWeekoff,
                                   Month = h.Month,
                                   Year = h.Year
                               }).ToList();
                return holiday;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public OfficeCalendar GetTodayStatus(string companyId, string branchId, int year, int month, DateTime date)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(year.ToString()) && !string.IsNullOrEmpty(month.ToString()) && !string.IsNullOrEmpty(date.ToString()))
                {
                    var holiday = _holidayRepository.GetOne(x => x.CompanyId == companyId && x.BranchId == branchId && x.Year == year && x.Month == month && x.Date == date);
                    if (holiday != null)
                    {
                        return new OfficeCalendar
                        {
                            Id = holiday.Id,
                            Code = holiday.Code,
                            Active = holiday.Active,
                            Archive = holiday.Archive,
                            Description = holiday.Description,
                            CompanyId = holiday.CompanyId,
                            BranchId = holiday.BranchId,
                            Date = holiday.Date,
                            Day = holiday.Day,
                            IsHoliday = holiday.IsHoliday,
                            IsWeekday = holiday.IsWeekday,
                            IsWeekoff = holiday.IsWeekoff,
                            Month = holiday.Month,
                            Year = holiday.Year,
                            Status = holiday.IsWeekday ? "Weekday" : (holiday.IsWeekoff ? "Weekoff" : "Holiday")
                        };
                    }

                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
