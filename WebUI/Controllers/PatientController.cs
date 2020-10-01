using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Library.Model.Core.Doctors;
using Library.Service.Core.Doctors;
using Library.ViewModel.Core.Doctors;

namespace ERP.WebUI.Controllers
{
    public class PatientController : Controller
    {
        #region Ctor
        private readonly IPatientService _patientService;
        public PatientController(
            IPatientService patientService
            )
        {
            _patientService = patientService;
        }
        #endregion
        public ActionResult Index(string companyId, string branchId)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<PatientViewModel>>(_patientService.GetAll(companyId, branchId)));
                if (!string.IsNullOrEmpty(companyId))
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<PatientViewModel>>(_patientService.GetAll(companyId)));
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpGet]
        public ActionResult Create(string patientId = "")
        {
            try
            {
                return View(new PatientViewModel { Active = true, Id = patientId });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Create(PatientViewModel vm, string patientId = "")
        {
            try
            {
                _patientService.Add(AutoMapperConfiguration.mapper.Map<Patient>(vm));
                return JavaScript("ShowResult('Data saved successfully.','success')");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Address(string patientId = "")
        {
            try
            {
                return PartialView(new PatientViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Address(PatientViewModel vm, string patientId = "")
        {
            try
            {
                _patientService.Add(AutoMapperConfiguration.mapper.Map<Patient>(vm));
                return JavaScript("ShowResult('Data saved successfully.','success')");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult InsuranceInformation(string patientId = "")
        {
            try
            {
                return PartialView(new PatientViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult InsuranceInformation(PatientViewModel vm, string patientId = "")
        {
            try
            {
                _patientService.Add(AutoMapperConfiguration.mapper.Map<Patient>(vm));
                return JavaScript("ShowResult('Data saved successfully.','success')");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}