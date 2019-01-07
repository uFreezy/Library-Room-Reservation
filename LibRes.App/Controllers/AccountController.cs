using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LibRes.App.DbModels;
using LibRes.App.Models.Account;
using LibRes.App.Models.Calendar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibRes.App.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Controller that handles all User operations.
    /// </summary>
    public class AccountController : BaseController
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
            if (result.Succeeded) return RedirectToAction("Index", "Home");

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
            if (user == null)
            {
                ModelState.AddModelError("Email", "User with given email doesn't exist.");
                return View();
            }

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

            ViewBag.SecretQuestion = user.SecretQuestion;
            ViewBag.Email = user.Email;
            return View();
        }

        /// <summary>
        ///     Serves view confirming that the password has been changed.
        /// </summary>
        /// <returns>View confirming that the password has been changed.</returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        /// <summary>
        ///     Returns the profile page either for the current user (profile edit page)
        ///     or for given user based on userId (profile page).
        /// </summary>
        /// <param name="userId">User id for the profile to be fetched. Empty if it's current user.</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("/profile")]
        public IActionResult Profile(string userId = "")
        {
            if (userId == "" || userId == _userManager.GetUserId(HttpContext.User))
                return RedirectToAction("EditProfile");

            var evs = Context.ReservationModels
                .Include(r => r.EventDates)
                .Include(r => r.MeetingRoom)
                .Include(r => r.ReservationOwner)
                .Where(r => r.ReservationOwner.Id == userId)
                .Select(r => new EventSingleView
                {
                    Id = r.Id,
                    EventName = r.EventName,
                    InitialDate = r.EventDates.First().Occurence,
                    RepeatDates = r.EventDates
                        .Where(d => d.Id != r.EventDates.First().Id)
                        .Select(d => d.Occurence).ToList(),
                    BeginHour = r.EventDates.First().Occurence.ToString("HH:mm"),
                    EndHour = (r.EventDates.First().Occurence -
                               TimeSpan.FromMinutes(r.EventDates.First().DurationMinutes)).ToString("HH:mm"),
                    MeetingRoom = r.MeetingRoom.RoomName,
                    Department = r.Department,
                    ReservationOwner = r.ReservationOwner,
                    WantsMultimedia = r.WantsMultimedia,
                    IsOwner = r.ReservationOwner.Id == HttpContext.User
                                  .FindFirst(ClaimTypes.NameIdentifier).Value
                }).ToList();

            var profile = Context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Reservations)
                .Select(u => new ProfileViewModel
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Events = evs
                }).First();

            return View("_Profile", profile);
        }

        /// <summary>
        ///     Fetches the current profile data for the logged user
        ///     and returns the profile edit form.
        /// </summary>
        /// <returns>The profile edit view.</returns>
        [HttpGet]
        [Authorize]
        [Route("/profile/edit")]
        public IActionResult EditProfile()
        {
            var usr = _userManager.GetUserAsync(HttpContext.User).Result;

            var profile = new ProfileEditModel
            {
                Email = usr.Email,
                FirstName = usr.FirstName,
                LastName = usr.LastName,
                PhoneNumber = usr.PhoneNumber
            };

            return View(profile);
        }

        /// <summary>
        ///     Performs edit on user's profile data.
        /// </summary>
        /// <param name="model">The Data model user for editing.</param>
        /// <returns>View to the profile.</returns>
        [Authorize]
        [HttpPost("/profile/edit")]
        public IActionResult EditProfile(ProfileEditModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usr = _userManager.GetUserAsync(HttpContext.User).Result;
            var profile = Context.Users.First(u => u.Id == usr.Id);

            if (model.FirstName != null) profile.FirstName = model.FirstName;
            if (model.LastName != null) profile.LastName = model.LastName;
            if (model.PhoneNumber != null)
            {
                if (Regex.Matches(model.PhoneNumber, @"(\+359|359|0)\d{9}").Count > 0)
                {
                    profile.PhoneNumber = model.PhoneNumber;
                }
                else
                {
                    ModelState.AddModelError("PhoneNumber", "Invalid phone number format.");
                    return View(model);
                }
            }

            if (model.Password != null)
            {
                var isChangedSuccessfully = ChangePassword(model).Result;

                if (!isChangedSuccessfully)
                    ModelState
                        .AddModelError("Password", "Failed to update password. Please try again later");
            }

            if (profile.FirstName != model.FirstName ||
                profile.LastName != model.LastName ||
                profile.PhoneNumber != model.PhoneNumber)
            {
                Context.Users.Update(profile);

                Context.SaveChanges();
            }

            TempData["success"] = "Profile updated successfully! ";

            return View();
        }


        /// <summary>
        ///     Task which changes the user's password.
        /// </summary>
        /// <param name="model">Model to derive the password from.</param>
        /// <returns>True if the operation was successful. False otherwise.</returns>
        private async Task<bool> ChangePassword(ProfileEditModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);

            return result.Succeeded;
        }
    }
}