using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VChatCore.Dto;
using VChatCore.Model;
using VChatCore.Service;

namespace VChatCore.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private AuthService _authService;
        private IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;
        private MailService _mailService;
        private ConfirmEmail _confirmEmail;
        public AuthsController(AuthService authService, IWebHostEnvironment hostEnvironment, IHttpContextAccessor contextAccessor, MailService mailService, ConfirmEmail confirmEmail)
        {
            this._authService = authService;
            this._hostEnvironment = hostEnvironment;
            this._contextAccessor = contextAccessor;
            this._mailService = mailService;
            this._confirmEmail = confirmEmail;
        }

        [Route("auths/login")]
        [HttpPost]
        public IActionResult Login(User user)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            var u = _confirmEmail.GetUser(user.UserName);
            try
            {
                if (u !=null) {
                    if (u.IsConfirmEmail)
                    {
                        AccessToken accessToken = this._authService.Login(user);
                        responseAPI.Data = accessToken;
                        return Ok(responseAPI);
                    }
                    throw new ArgumentException("Vui lòng xác thực tài khoản!");

                }
                throw new ArgumentException("Tài khoản không tồn tại");


            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("auths/sign-up")]
        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._authService.SignUp(user);
                MailClass mailClass = getMailClass(user);
                await _mailService.SendMail(mailClass);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        public MailClass getMailClass(User user)
        {
            MailClass mailClass = new MailClass();
            mailClass.Subject = "Mail Confirm";
            mailClass.Body = _mailService.GetEmailBody(user);
            mailClass.ToEmailIds = new List<string>()
            {

                user.Email
        };
            return mailClass;

        }

        [Route("user/confirmMail/{username}")]
        [HttpPost]
        public string confirmMail(string username)
        {
           return _confirmEmail.ConfirmEmailInfo(username);
        }

        [HttpGet("img")]
        public IActionResult DownloadImage(string key)
        {
            try
            {
                string path = Path.Combine(this._hostEnvironment.ContentRootPath, key);
                var image = System.IO.File.OpenRead(path);
                return File(image, "image/*");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("file")]
        public IActionResult DownloadFile(string key)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                string pathTemplate = Path.Combine(this._hostEnvironment.ContentRootPath, key);
                Stream stream = new FileStream(pathTemplate, FileMode.Open);
                responseAPI.Data = "";
                return File(stream, "application/octet-stream", key);

            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("post-hubconnection")]
        [HttpPost]
        public IActionResult PutHubConnection(string key)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(this._contextAccessor);
                this._authService.PutHubConnection(userSession, key);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

    }
}
