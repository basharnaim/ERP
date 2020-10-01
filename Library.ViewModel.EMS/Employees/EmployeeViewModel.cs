#region Using

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace Library.ViewModel.EMS.Employees
{
    public class EmployeeViewModel
    {
        #region Scalar
        /// <summary>
        /// Primary key.
        /// </summary>
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNumber1 { get; set; }
        public string ContactNumber2 { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string Email { get; set; }
        public string BloodGroup { get; set; }
        
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        #endregion

        #region Navigation
        public string DepartmentId { get; set; }
        public string DesignationId { get; set; }
        public string EmployeeTypeId { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string UnitId { get; set; }
        public string UserId { get; set; }
        #endregion
    }
}
