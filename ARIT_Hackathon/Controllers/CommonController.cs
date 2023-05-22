using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Entities;

using Microsoft.EntityFrameworkCore;

using Entities.ExtendedModels;
using Entities.Models;
using System;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;

namespace ARIT_Hackathon.Controllers
{
    //[BasicAuthorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repoWrapper;
        private RepositoryContext _context;
        private static readonly Random random = new Random();
        

        public CommonController(ILoggerManager logger, IRepositoryWrapper repoWrapper, RepositoryContext context)
        {
            _logger = logger;
            _repoWrapper = repoWrapper;
            _context = context;
        }
        [Route("GetUserLoginData")]
        [HttpGet]
        public IActionResult GetUserData()
        {
            string Pan = "AXGPT8163J";
            _logger.LogInfo($"Fetch all process start");
            var userData = _context.user_login.AsNoTracking().Where(x => Pan == Pan.ToUpper() && (x.expiry_dt == null || x.expiry_dt > DateTime.Now)).FirstOrDefault();
            return Ok(userData);
        }


        [Route("GetOtpLogin/{Pan}/{OTP}")]
        [HttpGet]
        public IActionResult GetOtpLogin(string Pan, string OTP)
        {
            if (string.IsNullOrWhiteSpace(Pan) || string.IsNullOrWhiteSpace(OTP) || Pan.Length != 10 || !IsRegexMatch(Pan, CodeValueContrant.PanRegex))
            {
                return BadRequest(new ApiCommonResponse<user_login>() { allowStatus = false, msg = "Invalid Pan or OTP!", showMsg = true });

            }
            Pan = Pan.ToUpper();
            var userDetail = _context.user_details.AsNoTracking().Where(x => x.pan == Pan).FirstOrDefault();
            if (userDetail == null)
            {
                return Ok(new ApiCommonResponse<user_login>() { allowStatus = false, msg = "You are not Registered!", showMsg = true });
            }
            var userLogin = _context.user_login.AsNoTracking().Where(x => x.pan == Pan && x.otp == OTP).FirstOrDefault();
            if (userLogin == null)
            {
                return Ok(new ApiCommonResponse<user_login>() { allowStatus = false, msg = "Invalid Pan or OTP!", showMsg = true });
            }
            else if (userLogin != null && userLogin.expiry_dt != null && userLogin.expiry_dt < DateTime.Now)
            {
                return Ok(new ApiCommonResponse<user_login>() { allowStatus = false, msg = "Password Expired, Kindly generate new OTP", showMsg = true });
            }
            return Ok(new ApiCommonResponse<user_login>() { allowStatus = true, msg = "You are successfully logged in", showMsg = false, contentData = userLogin });
        }

        [NonAction]
        public static string GenerateOTP()
        {
            int otp = random.Next(100000, 999999);
            return Convert.ToString(otp);
        }
        
        [NonAction]
        public bool sendOTPMailSms(string email, string mobile, string OTP)
        {
            return true;
        }

        [NonAction]
        public static void UpdateStringPropertiesToUppercase(object model)
        {
            if (model == null)
            {
                return;
            }

            var properties = model.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = (string)property.GetValue(model);

                    if (!string.IsNullOrEmpty(value))
                    {
                        property.SetValue(model, value.ToUpper());
                    }
                }
            }
        }

        [Route("GenerateNewOtpLogin/{Pan}/{DeviceName}")]
        [HttpGet]
        public IActionResult GenerateNewOtpLogin(string Pan, string DeviceName = "MOBILE")
        {
            if (string.IsNullOrWhiteSpace(Pan) || Pan.Length != 10 || !IsRegexMatch(Pan, CodeValueContrant.PanRegex))
            {
                return BadRequest(new ApiCommonResponse<string>() { allowStatus = false, msg = "Invalid Pan!", showMsg = true });

            }
            Pan = Pan.ToUpper();
            var userDetail = _context.user_details.AsNoTracking().Where(x => x.pan == Pan && x.isactive==true).FirstOrDefault();
            if (userDetail == null)
            {
                return Ok(new ApiCommonResponse<string>() { allowStatus = false, msg = "You are not Registered!", showMsg = true });
            }
            var userLogin = _context.user_login.AsNoTracking().Where(x => x.pan == Pan && x.device_name == DeviceName).FirstOrDefault();
            if (userLogin != null)
            {
                string newOTP = GenerateOTP();
                string ResMsg = "";
                if (sendOTPMailSms(userDetail.email, userDetail.mobile, newOTP))
                {
                    ResMsg = "OTP sent to registered email/mobile";
                }
                else
                {
                    return Ok(new ApiCommonResponse<string>() { allowStatus = false, msg = "Failed to Sent OTP on EMAIL/Mobile", showMsg = true });
                }
                userLogin.otp = newOTP;
                userLogin.modified_by = Pan;
                userLogin.modified_dt = DateTime.Now;
                userLogin.expiry_dt = DateTime.Now.AddMinutes(15);
                userLogin.ip_address = HttpContext.Connection.RemoteIpAddress?.ToString();
                _context.Entry(userLogin).State = EntityState.Modified;
                _context.SaveChanges();
                _context.Entry(userLogin).State = EntityState.Detached;
                return Ok(new ApiCommonResponse<string>() { allowStatus = true, msg = ResMsg, showMsg = true });

            }
            else
            {
                userLogin = new user_login();
                string newOTP = GenerateOTP();
                string ResMsg = "";
                if (sendOTPMailSms(userDetail.email, userDetail.mobile, newOTP))
                {
                    ResMsg = "OTP sent to registered email/mobile";
                }
                else
                {
                    return Ok(new ApiCommonResponse<string>() { allowStatus = false, msg = "Failed to Sent OTP on EMAIL/Mobile", showMsg = true });
                }
                userLogin.roletype = userDetail.roletype;
                userLogin.pan = userDetail.pan;
                userLogin.otp = newOTP;
                userLogin.created_by = Pan;
                userLogin.created_dt = DateTime.Now;
                userLogin.expiry_dt = DateTime.Now.AddMinutes(15);
                userLogin.device_name = DeviceName;
                userLogin.ip_address = HttpContext.Connection.RemoteIpAddress?.ToString();
                userLogin.user_id_ref = userDetail.transid;
                _context.Add(userLogin);
                _context.SaveChanges();
                _context.Entry(userLogin).State = EntityState.Detached;
                return Ok(new ApiCommonResponse<string>() { allowStatus = true, msg = ResMsg, showMsg = true });
            }
        }

        [Route("RegisterUserLoginData")]
        [HttpPost]        
        public ApiCommonResponse<user_details> RegisterUserLoginData([FromBody] Registration regPayload)
        {
            try
            {
                user_details user_Details = regPayload.userdetails;
                if (regPayload == null || user_Details == null)
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Invalid Data!", showMsg = true };

                }
                else if (string.IsNullOrWhiteSpace(user_Details.pan) || user_Details.pan.Length != 10 || !IsRegexMatch(user_Details.pan,CodeValueContrant.PanRegex))
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Invalid Pan!", showMsg = true };

                }
                else if (string.IsNullOrWhiteSpace(user_Details.ucc))
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "UCC Required!", showMsg = true };

                }
                else if (string.IsNullOrWhiteSpace(user_Details.fullname))
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "FullName Required!", showMsg = true };

                }
                else if (user_Details.dob == null)
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "DOB Required!", showMsg = true };

                }
                else if (string.IsNullOrWhiteSpace(user_Details.email) || !IsRegexMatch(user_Details.email,CodeValueContrant.EmailRegex))
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Email Required!", showMsg = true };

                }
                else if (string.IsNullOrWhiteSpace(user_Details.mobile))
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Mobile No Required!", showMsg = true };
                }
                else if (string.IsNullOrWhiteSpace(regPayload.device_name))
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Device Name is Required!", showMsg = true };
                }

                if (_context.user_details.AsNoTracking().Any(x => x.pan == user_Details.pan.ToUpper()))
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Already Registered!", showMsg = true };
                }
                UpdateStringPropertiesToUppercase(user_Details);

                user_Details.created_by = user_Details.pan;
                user_Details.roletype = "USER";

                user_login userLogin = new user_login();
                string newOTP = GenerateOTP();
                string ResMsg = "";
                if (sendOTPMailSms(user_Details.email, user_Details.mobile, newOTP))
                {
                    ResMsg = "OTP sent to registered email/mobile";
                    _context.Add(user_Details);
                    _context.SaveChanges();
                    _context.Entry(user_Details).State = EntityState.Detached;
                }
                else
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Registration Failed due to Sent OTP on EMAIL/Mobile", showMsg = true };
                }
                userLogin.roletype = user_Details.roletype;
                userLogin.pan = user_Details.pan;
                userLogin.otp = newOTP;
                userLogin.created_by = user_Details.pan;
                userLogin.created_dt = DateTime.Now;
                userLogin.expiry_dt = DateTime.Now.AddMinutes(15);
                userLogin.device_name = regPayload.device_name;
                userLogin.ip_address = HttpContext.Connection.RemoteIpAddress?.ToString();

                _context.Add(userLogin);
                _context.SaveChanges();
                _context.Entry(userLogin).State = EntityState.Detached;
                if (_context.user_details.AsNoTracking().Any(x => x.pan == user_Details.pan.ToUpper()))
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = true, msg = "Welcome to Grievance, OTP has been sent to your Registered Email/Mobile", showMsg = true };
                }
                else
                {
                    return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Something Went Wrong!", showMsg = true };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in RegisterUserLoginData " + JsonConvert.SerializeObject(ex));
                return new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Something Went Wrong!", showMsg = true };
            }
        }

        [Route("RegisterIssue")]
        [HttpPost]
        public IActionResult RegisterIssue([FromBody] issue_detail payload)
        {
            try
            {
                payload.pan = (payload.pan ?? "").ToUpper();
                if (payload == null)
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Invalid Data!", showMsg = true });
                }
                else if (string.IsNullOrWhiteSpace(payload.pan) || payload.pan.Length != 10 || !IsRegexMatch(payload.pan, CodeValueContrant.PanRegex))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Invalid Pan!", showMsg = true });
                }
                else if (string.IsNullOrWhiteSpace(payload.ucc))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "UCC Required!", showMsg = true });
                }
                else if (string.IsNullOrWhiteSpace(payload.fullname))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "FullName Required!", showMsg = true });
                }
                else if (string.IsNullOrWhiteSpace(payload.summary))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Summary Required!", showMsg = true });

                }
                else if (string.IsNullOrWhiteSpace(payload.exchange))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Exchange Required!", showMsg = true });

                }
                else if (string.IsNullOrWhiteSpace(payload.segment))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Segment Required!", showMsg = true });
                }
                else if (string.IsNullOrWhiteSpace(payload.category))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Category Required!", showMsg = true });
                }
                else if (string.IsNullOrWhiteSpace(payload.subcategory))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Sub Category Required!", showMsg = true });
                }
                else if (string.IsNullOrWhiteSpace(payload.details))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Details text Required!", showMsg = true });
                }
                else if (!string.IsNullOrWhiteSpace(payload.filename) && string.IsNullOrWhiteSpace(payload.filedata))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "File data is incorrect!", showMsg = true });
                }
                else if (string.IsNullOrWhiteSpace(payload.mode))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Mode Required!", showMsg = true });
                }


                if (payload.user_id_ref == 0 || !_context.user_details.AsNoTracking().Any(x => x.transid == payload.user_id_ref && x.pan.ToUpper() == payload.pan))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Invalid user_id_ref!", showMsg = true });
                }

                if (payload.issue_id != 0 && !_context.issue_detail.AsNoTracking().Any(x => x.issue_id == payload.issue_id && x.pan != payload.pan))
                {
                    return BadRequest(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "issue_id is already registered on another pan!", showMsg = true });
                }
                payload.targate_date = (payload.targate_date ?? payload.targate_date.Value.AddDays(5));
                payload.issue_by = payload.fullname;



                if (payload.issue_id == 0)
                {
                    _context.Add(payload);
                    _context.SaveChanges();
                    _context.Entry(payload).State = EntityState.Detached;
                }
                else
                {
                    _context.Entry(payload).State = EntityState.Modified;
                    _context.SaveChanges();
                    _context.Entry(payload).State = EntityState.Detached;
                }
                return Ok(new ApiCommonResponse<issue_detail>() { allowStatus = true, msg = "Successfully issue registered!", showMsg = true, contentData = payload });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in RegisterIssue " + JsonConvert.SerializeObject(ex));
                return Ok(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Something Went Wrong!", showMsg = true });
            }
        }

        [Route("GetIssueList")]
        [HttpPost]
        public IActionResult GetIssueList([FromBody] IssueListModel issueListPayLoad)
        {
            try
            {
                var data = _context.issue_detail.AsNoTracking().Where(x => (string.IsNullOrWhiteSpace(issueListPayLoad.Status) || x.status == issueListPayLoad.Status)
                && (string.IsNullOrWhiteSpace(issueListPayLoad.IssueCreatedBy) || x.issue_by == issueListPayLoad.IssueCreatedBy)
                && (string.IsNullOrWhiteSpace(issueListPayLoad.Summary) || x.summary == issueListPayLoad.Summary)
                && (issueListPayLoad.IssueId == null || x.issue_id == issueListPayLoad.IssueId)
                && (issueListPayLoad.UserId == null || x.issue_id == issueListPayLoad.UserId)).ToList();

                if (data == null || data.Count == 0)
                {
                    return Ok(new ApiCommonResponse<List<issue_detail>>() { allowStatus = false, msg = "No records found!", showMsg = true });
                }
                else
                {
                    return Ok(new ApiCommonResponse<List<issue_detail>>() { allowStatus = true, contentData = data, showMsg = false });
                }
            }
            catch (Exception)
            {

                return Ok(new ApiCommonResponse<List<issue_detail>>() { allowStatus = false, msg = "Something went wrong!", showMsg = true });
            }

        }

        [Route("getDropDownValue")]
        [HttpGet]
        public IActionResult getDropDownValue()
        {
            try
            {
                var data = _context.cfg_codevalue.ToList();

                if (data == null || data.Count == 0)
                {
                    return Ok(new ApiCommonResponse<List<cfg_codevalue>>() { allowStatus = false, msg = "No records found!", showMsg = true });
                }
                else
                {
                    return Ok(new ApiCommonResponse<List<cfg_codevalue>>() { allowStatus = true, contentData = data, showMsg = false });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiCommonResponse<List<cfg_codevalue>>() { allowStatus = false, msg = "Something went wrong!", showMsg = true });
                throw;
            }
        }

        [Route("AddNewNotes")]
        [HttpPost]
        public IActionResult AddnewNote([FromBody] issue_notes_detail issue_Notes)
        {
            try
            {
                if (issue_Notes == null)
                {
                    return BadRequest(new ApiCommonResponse<string>() { allowStatus = false, msg = "Invalid Data!", showMsg = true });
                }
                else if (issue_Notes.issue_id_ref == 0)
                {
                    return BadRequest(new ApiCommonResponse<string>() { allowStatus = false, msg = "issue id required!", showMsg = true });
                }
                else if (issue_Notes.user_id_ref == 0)
                {
                    return BadRequest(new ApiCommonResponse<string>() { allowStatus = false, msg = "user id required!", showMsg = true });
                }
                else if (!_context.issue_detail.AsNoTracking().Any(x => x.issue_id == issue_Notes.issue_id_ref))
                {
                    return BadRequest(new ApiCommonResponse<string>() { allowStatus = false, msg = "issue id is invalid!", showMsg = true });
                }
                else if (!_context.user_details.AsNoTracking().Any(x => x.transid == issue_Notes.user_id_ref))
                {
                    return BadRequest(new ApiCommonResponse<string>() { allowStatus = false, msg = "user id is invalid!", showMsg = true });
                }
                else if (string.IsNullOrWhiteSpace(issue_Notes.note) && string.IsNullOrWhiteSpace(issue_Notes.filename) && string.IsNullOrWhiteSpace(issue_Notes.filedata))
                {
                    return BadRequest(new ApiCommonResponse<string>() { allowStatus = false, msg = "atleast a one field is requird note attachment or note text !", showMsg = true });
                }
                issue_Notes.created_dt = System.DateTime.Now;
                issue_Notes.created_by = _context.user_details.AsNoTracking().Where(x => x.transid == issue_Notes.user_id_ref).Select(o => o.fullname).FirstOrDefault();


                _context.Add(issue_Notes);
                _context.SaveChanges();
                _context.Entry(issue_Notes).State = EntityState.Detached;

                return Ok(new ApiCommonResponse<string>() { allowStatus = true, msg = "Successfully issue registered!", showMsg = true });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in RegisterIssue " + JsonConvert.SerializeObject(ex));
                return Ok(new ApiCommonResponse<string>() { allowStatus = false, msg = "Something Went Wrong!", showMsg = true });
            }
        }


        [Route("GetNotes/{issueId}")]
        [HttpGet]
        public IActionResult GetNotesList(long issueId)
        {
            long id = issueId;
            try
            {// else if(!_context.issue_detail.AsNoTracking())
                if (id == 0)
                {
                    return BadRequest(new ApiCommonResponse<List<issue_notes_detail>>() { allowStatus = false, msg = "Invalid Data!", showMsg = true });
                }
                else if (!_context.issue_detail.AsNoTracking().Any(x => x.issue_id == id))
                {
                    return BadRequest(new ApiCommonResponse<List<issue_notes_detail>>() { allowStatus = false, msg = "Invalid issue id!", showMsg = true });
                }


                List<issue_notes_detail> data = _context.issue_notes_detail.AsNoTracking().Where(x => x.issue_id_ref == id).ToList();

                if (data == null || data.Count == 0)
                {
                    return BadRequest(new ApiCommonResponse<List<issue_notes_detail>>() { allowStatus = true, msg = "no data found!", showMsg = false });
                }
                else
                {
                    return Ok(new ApiCommonResponse<List<issue_notes_detail>>() { allowStatus = true, msg = "Successfully issue note Get!", showMsg = false, contentData = data });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in RegisterIssue " + JsonConvert.SerializeObject(ex));
                return Ok(new ApiCommonResponse<issue_detail>() { allowStatus = false, msg = "Something Went Wrong!", showMsg = true });
            }
        }


        [Route("GetUserDetailesByPan/{Pan}")]
        [HttpGet]
        public IActionResult GetUserDetailesByPan(string Pan)
        {
            try
            {
                if(Pan == null || !IsRegexMatch(Pan, CodeValueContrant.PanRegex))
                {
                    return BadRequest(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Invalid Data!", showMsg = true });
                }
                else
                {
                    var data= _context.user_details.AsNoTracking().Where(x=>x.pan == Pan && x.isactive==true).FirstOrDefault();

                    if(data == null)
                    {
                        return BadRequest(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Details not found!", showMsg = true });
                    }
                    else
                    {
                        return Ok(new ApiCommonResponse<user_details>() { allowStatus = true, msg = "User details Get Successful!", showMsg = true ,contentData=data});
                    }
                }

             }
            catch (Exception ex)
            {
                _logger.LogError("Error in RegisterIssue " + JsonConvert.SerializeObject(ex));
                return Ok(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Something Went Wrong!", showMsg = true });
                
            }

        }



        [Route("GetUserDetailesById/{userid}")]
        [HttpGet]
        public IActionResult GetUserDetailesById(long userid)
        {
            try
            {
                
                    var data = _context.user_details.AsNoTracking().Where(x => x.transid == userid && x.isactive == true).FirstOrDefault();

                    if (data == null)
                    {
                        return BadRequest(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Details not found!", showMsg = true });
                    }
                    else
                    {
                        return Ok(new ApiCommonResponse<user_details>() { allowStatus = true, msg = "User details Get Successful!", showMsg = true, contentData=data });
                    }
                

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in RegisterIssue " + JsonConvert.SerializeObject(ex));
                return Ok(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Something Went Wrong!", showMsg = true });

            }

        }



        [NonAction]
        public bool IsRegexMatch(string pan,string pattern) 
        {            
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(pan))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Route("catrgoryItem")]
        [HttpGet]
        public IActionResult GetListofCatrgoy()
        {

            var data = _context.category_master.ToList();

            if (data == null)
            {
                return Ok(new ApiCommonResponse<List<category_master>>() { allowStatus = false, msg = "No records found!", showMsg = true });
            }
            else
            {
                return Ok(new ApiCommonResponse<List<category_master>>() { allowStatus = true, contentData = data, showMsg = false });
            }

        }
    }
}
