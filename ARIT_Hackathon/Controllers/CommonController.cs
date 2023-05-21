using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Entities;

using Microsoft.EntityFrameworkCore;

using Entities.ExtendedModels;
using Entities.Models;
using System;
using Newtonsoft.Json;

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
            var userData = _context.user_login.AsNoTracking().Where(x => Pan == Pan.ToUpper() && (x.expirey_dt == null || x.expirey_dt > DateTime.Now)).FirstOrDefault();
            return Ok(userData);
        }


        [Route("GetOtpLogin/{Pan}/{OTP}")]
        [HttpGet]
        public IActionResult GetOtpLogin(string Pan, string OTP)
        {
            if (string.IsNullOrWhiteSpace(Pan) || string.IsNullOrWhiteSpace(OTP) || Pan.Length != 10)
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
            else if (userLogin != null && userLogin.expirey_dt != null && userLogin.expirey_dt < DateTime.Now)
            {
                return Ok(new ApiCommonResponse<user_login>() { allowStatus = false, msg = "Password Expired, Kindly generate new OTP", showMsg = true });
            }
            return Ok(new ApiCommonResponse<user_login>() { allowStatus = true, msg = "You are successfully logged in", showMsg = false, contentData = userLogin });
        }

        public static string GenerateOTP()
        {
            int otp = random.Next(100000, 999999);
            return Convert.ToString(otp);
        }
        public bool sendOTPMailSms(string email, string mobile, string OTP)
        {
            return true;
        }
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
            if (string.IsNullOrWhiteSpace(Pan) || Pan.Length != 10)
            {
                return BadRequest(new ApiCommonResponse<string>() { allowStatus = false, msg = "Invalid Pan!", showMsg = true });

            }
            Pan = Pan.ToUpper();
            var userDetail = _context.user_details.AsNoTracking().Where(x => x.pan == Pan).FirstOrDefault();
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
                userLogin.expirey_dt = DateTime.Now.AddMinutes(15);
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
                userLogin.expirey_dt = DateTime.Now.AddMinutes(15);
                userLogin.device_name = DeviceName;
                userLogin.ip_address = HttpContext.Connection.RemoteIpAddress?.ToString();

                _context.Add(userLogin);
                _context.SaveChanges();
                _context.Entry(userLogin).State = EntityState.Detached;
                return Ok(new ApiCommonResponse<string>() { allowStatus = true, msg = ResMsg, showMsg = true });
            }
        }

        [Route("RegisterUserLoginData")]
        [HttpPost]
        public IActionResult RegisterUserLoginData([FromBody] Registration regPayload)
        {
            try
            {
                user_details user_Details = regPayload.userdetails;
                if (regPayload == null || user_Details == null)
                {
                    return BadRequest(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Invalid Data!", showMsg = true });

                }
                else if (string.IsNullOrWhiteSpace(user_Details.pan) || user_Details.pan.Length != 10)
                {
                    return BadRequest(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Invalid Pan!", showMsg = true });

                }
                else if (string.IsNullOrWhiteSpace(user_Details.ucc))
                {
                    return BadRequest(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "UCC Required!", showMsg = true });

                }
                else if (string.IsNullOrWhiteSpace(user_Details.fullname))
                {
                    return BadRequest(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "FullName Required!", showMsg = true });

                }
                else if (user_Details.dob == null)
                {
                    return BadRequest(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "DOB Required!", showMsg = true });

                }
                else if (string.IsNullOrWhiteSpace(user_Details.email))
                {
                    return BadRequest(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Email Required!", showMsg = true });

                }
                else if (string.IsNullOrWhiteSpace(user_Details.mobile))
                {
                    return BadRequest(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Mobile No Required!", showMsg = true });
                }
                else if (string.IsNullOrWhiteSpace(regPayload.device_name))
                {
                    return BadRequest(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Device Name is Required!", showMsg = true });
                }

                if (_context.user_details.AsNoTracking().Any(x => x.pan == user_Details.pan.ToUpper()))
                {
                    return Ok(new ApiCommonResponse<user_details>() { allowStatus = false, msg = "Already Registered!", showMsg = true });
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
                    _context.Entry(user_Details).State = EntityState.Detached;
                }
                else
                {
                    return Ok(new ApiCommonResponse<string>() { allowStatus = false, msg = "Registration Failed due to Sent OTP on EMAIL/Mobile", showMsg = true });
                }
                userLogin.roletype = user_Details.roletype;
                userLogin.pan = user_Details.pan;
                userLogin.otp = newOTP;
                userLogin.created_by = user_Details.pan;
                userLogin.created_dt = DateTime.Now;
                userLogin.expirey_dt = DateTime.Now.AddMinutes(15);
                userLogin.device_name = regPayload.device_name;
                userLogin.ip_address = HttpContext.Connection.RemoteIpAddress?.ToString();

                _context.Add(userLogin);
                _context.SaveChanges();
                _context.Entry(userLogin).State = EntityState.Detached;
                if (_context.user_details.AsNoTracking().Any(x => x.pan == user_Details.pan.ToUpper()))
                {
                    return Ok(new ApiCommonResponse<user_details>() { allowStatus = true, msg = "Welcome to Grievance, OTP has been sent to your Registered Email/Mobile", showMsg = true });
                }
                else
                {
                    return Ok(new ApiCommonResponse<string>() { allowStatus = false, msg = "Something Went Wrong!", showMsg = true });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in RegisterUserLoginData " + JsonConvert.SerializeObject(ex));
                return Ok(new ApiCommonResponse<string>() { allowStatus = false, msg = "Something Went Wrong!", showMsg = true });
            }
        }
    }
}
