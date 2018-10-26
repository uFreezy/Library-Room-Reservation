﻿using System;
using System.Threading.Tasks;
using LibRes.App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibRes.App.Controllers
{
    /// <summary>
    /// Controller that handles all User opperations
    /// </summary>
    public class AccountController : Controller
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Get action that checks if the user is logged and if not,
        ///  returns the login view.
        /// </summary>
        /// <returns>The login view.</returns>
        [Route("/login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Views/Account/Login.cshtml");
        }

        /// <summary>
        /// Post action that performs the login operation based on 
        /// the data entered in the form.
        /// </summary>
        /// <returns>Either home screen view or error if the login data is invalid</returns>
        /// <param name="model">Login view model containing the login form data</param>
        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager
                    .PasswordSignInAsync(model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                throw new Exception("Invalid login attempt");
            }
            return View(model);
        }

        /// <summary>
        /// Get action that destroys user session a.k.a logout
        /// </summary>
        /// <returns>Home screen view.</returns>
        [Route("/logout")]
        public ActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Get action that checks if the user is logged and if not,
        /// returns the register view,
        /// </summary>
        /// <returns>The register view.</returns>
        [Route("/register")]
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                RedirectToAction("Index", "Home");
            }

            return View();
        }

       /// <summary>
       /// Post action that performs the register operation based on the data
        /// entered in the form
       /// </summary>
       /// <returns>The login view if the registration is successful.</returns>
       /// <param name="model">Register view model that contains the register form data.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { 
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email 
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Login", "Account");
                }
            }

            return View(model);
        }

        /// <summary>
        /// Get action that returns the view for thepassword reset if the user isn't logged.
        /// </summary>
        /// <returns>The password reset view.</returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// Post action that generates the pass reset token, then passes it to the 'ResetPassword'
        /// get action. This token is neccessary for ASP.NET password to be resseted.
        /// </summary>
        /// <returns>Call to the ResetPassword GET action with embeded data.</returns>
        /// <param name="model">View model containing the form data.</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await this._userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    return View("ForgotPasswordConfirmation");
                }

                string Code = await this._userManager.GeneratePasswordResetTokenAsync(user);

                return RedirectToAction("ResetPassword", "Account", new {Code,user.Email});
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Get action that returns view with the second form for resseting the password.
        /// </summary>
        /// <returns>The second form for password reset</returns>
        /// <param name="code">The code required by ASP.NET for resseting passwords.</param>
        /// <param name="email">The email of the user.</param>
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string code, string email)
        {
            var user = await this._userManager.FindByNameAsync(email);
            ViewBag.SecretQuestion = user.SecretQuestion;
            ViewBag.Email = user.Email;
            return code == null ? View("Error") : View();
        }

       /// <summary>
       /// Resets the password.
       /// </summary>
       /// <returns>The password.</returns>
       /// <param name="model">Model.</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await this._userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            if(user.SecrectAnswerDecrypted == model.SecretAnswer){
                var result = await this._userManager.ResetPasswordAsync(user, model.Code, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
            }
           
            // TODO: Display error view here
            return View();
        }

       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
