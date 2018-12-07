using System.Threading.Tasks;
using LibRes.App.DbModels;
using LibRes.App.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibRes.App.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Controller that handles all User operations
    /// </summary>
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     Get action that checks if the user is logged and if not,
        ///     returns the login view.
        /// </summary>
        /// <returns>The login view.</returns>
        [Route("/login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            return View("Login");
        }

        /// <summary>
        ///     Post action that performs the login operation based on
        ///     the data entered in the form.
        /// </summary>
        /// <returns>Either home screen view or error if the login data is invalid</returns>
        /// <param name="model">Login view model containing the login form data</param>
        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var result = await _signInManager
                .PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded) {return RedirectToAction("Index", "Home");}

            ModelState.AddModelError("Email", "Wrong credentials");

            return View(model);
        }

        /// <summary>
        ///     Get action that destroys user session a.k.a logout
        /// </summary>
        /// <returns>Home screen view.</returns>
        [Route("/logout")]
        public ActionResult Logout()
        {
            _signInManager?.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        ///     Get action that checks if the user is logged and if not,
        ///     returns the register view,
        /// </summary>
        /// <returns>The register view.</returns>
        [Route("/register")]
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        ///     Post action that performs the register operation based on the data
        ///     entered in the form
        /// </summary>
        /// <returns>The login view if the registration is successful.</returns>
        /// <param name="model">Register view model that contains the register form data.</param>
        [HttpPost("/register")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                PhoneNumber = model.PhoneNumber,
                SecretQuestion = model.SecretQuestion,
                SecretAnswer = model.SecretAnswer
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return View(model);
            
            
            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        ///     Get action that returns the view for the password reset if the user isn't logged.
        /// </summary>
        /// <returns>The password reset view.</returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            if (HttpContext.User.Identity.IsAuthenticated) RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        ///     Post action that generates the pass reset token, then passes it to the 'ResetPassword'
        ///     get action. This token is necessary for ASP.NET password to be renewed.
        /// </summary>
        /// <returns>Call to the ResetPassword GET action with embedded data.</returns>
        /// <param name="model">View model containing the form data.</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByNameAsync(model.Email);
            // ReSharper disable once Mvc.ViewNotResolved
            if (user == null) return View("ForgotPasswordConfirmation");

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            return RedirectToAction("ResetPassword", "Account", new {Code = code, user.Email});
        }

        /// <summary>
        ///     Get action that returns view with the second form for resetting the password.
        /// </summary>
        /// <returns>The second form for password reset</returns>
        /// <param name="code">The code required by ASP.NET for resetting passwords.</param>
        /// <param name="email">The email of the user.</param>
        [Route("/reset")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string code, string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            ViewBag.SecretQuestion = user.SecretQuestion;
            ViewBag.Email = user.Email;
            return code == null ? View("Error") : View();
        }

        /// <summary>
        ///     Resets the password.
        /// </summary>
        /// <returns>The password.</returns>
        /// <param name="model">Model.</param>
        [HttpPost("/reset")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return RedirectToAction("ResetPasswordConfirmation", "Account");
            if (user.SecretAnswerDecrypted != model.SecretAnswer)
            {
                ViewBag.SecretQuestion = user.SecretQuestion;
                ViewBag.Email = user.Email;
                
                return View(model);
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded) return RedirectToAction("ResetPasswordConfirmation", "Account");

            // TODO: Display error view here
            return View();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}