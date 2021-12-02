using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using fyp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace fyp.Controllers
{

    public class AccountController : Controller
    {
        private const string AUTHSCHEME = "UserSecurity";
        private const string LOGIN_SQL =
           @"SELECT * FROM Account 
            WHERE AccountId = '{0}' 
              AND Password = HASHBYTES('SHA1', '{1}')";

        private const string LASTLOGIN_SQL =
           @"UPDATE Account SET LastLogin=GETDATE() 
                        WHERE AccountId='{0}'";

        private const string ROLE_COL = "Role";
        private const string NAME_COL = "Name";

        private const string REDIRECT_CNTR = "Home";
        private const string REDIRECT_ACTN = "Index";

        private const string LOGIN_VIEW = "Login";
        private AppDbContext _dbContext;

        public AccountController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View(LOGIN_VIEW);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(Account user)
        {
            if (!AuthenticateUser(user.AccountId, user.Password, out ClaimsPrincipal principal))
            {
                ViewData["Message"] = "Incorrect User Id or Password";
                ViewData["MsgType"] = "warning";
                return View(LOGIN_VIEW);
            }
            else
            {
                HttpContext.SignInAsync(
                   AUTHSCHEME,
                   principal,
               new AuthenticationProperties
               {
                   IsPersistent = false
               });

                // Update the Last Login Timestamp of the User
                DBUtl.ExecSQL(LASTLOGIN_SQL, user.AccountId);

                if (TempData["returnUrl"] != null)
                {
                    string returnUrl = TempData["returnUrl"].ToString();
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                }

                return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
            }
        }

        [Authorize]
        public IActionResult Logoff(string returnUrl = null)
        {
            HttpContext.SignOutAsync(AUTHSCHEME);
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
        }

        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }

        private bool AuthenticateUser(string aid, string pw, out ClaimsPrincipal principal)
        {
            principal = null;

            DataTable ds = DBUtl.GetTable(LOGIN_SQL, aid, pw);
            if (ds.Rows.Count == 1)
            {
                principal =
                   new ClaimsPrincipal(
                      new ClaimsIdentity(
                         new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, ds.Rows[0]["AccountId"].ToString()),
                        new Claim(ClaimTypes.Name, ds.Rows[0][NAME_COL].ToString()),
                        new Claim(ClaimTypes.Role, ds.Rows[0][ROLE_COL].ToString())
                         }, "Basic"
                      )
                   );
                return true;
            }
            return false;
        }
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(PasswordUpdate pu)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (_dbContext.Database.ExecuteSqlInterpolated(
                  $@"UPDATE Account 
                     SET Password = 
                         HASHBYTES('SHA1', CONVERT(VARCHAR, {pu.NewPassword})) 
                   WHERE AccountId = {accountId} 
                     AND Password = 
                         HASHBYTES('SHA1', CONVERT(VARCHAR, {pu.CurrentPassword}))"
               ) == 1)
                ViewData["Msg"] = "Password successfully updated!";
            else
                ViewData["Msg"] = "Failed to update password!";
            return View();
        }

        [Authorize]
        public JsonResult VerifyCurrentPassword(string CurrentPassword)
        {
            DbSet<Account> dbs = _dbContext.Account;
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Account user =
               dbs.FromSqlInterpolated(
                  $@"SELECT * FROM Account 
                   WHERE AccountId = {accountId} 
                     AND Password = 
                         HASHBYTES('SHA1', 
                                   CONVERT(VARCHAR, {CurrentPassword}))"
               ).FirstOrDefault();

            if (user != null)         // User's Password Found
                return Json(true);     // Current Password Valid
            else
                return Json(false);    // Current Password Invalid
        }

        [Authorize]
        public JsonResult VerifyNewPassword(string  NewPassword)
        {
            DbSet<Account> dbs = _dbContext.Account;
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Account user =
               dbs.FromSqlInterpolated(
                  $@"SELECT * FROM Account 
                   WHERE AccountId = {accountId} 
                     AND Password = 
                         HASHBYTES('SHA1', 
                                   CONVERT(VARCHAR, {NewPassword}))"
               ).FirstOrDefault();

            // New Password cannot be the same as Current Password
            if (user == null)       // User's Password Not Found
                return Json(true);   // New Password Valid
            else
                return Json(false);  // New Password Invalid
        }


    }
}