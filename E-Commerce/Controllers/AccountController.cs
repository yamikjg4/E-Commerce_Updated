using E_Commerce.Models;
using E_Commerce.repositry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountrepostry _accountRepository;

        public AccountController(IAccountrepostry accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [Route("Account/Register")]
        public IActionResult singup()
        {
            return View();
        }
        [Route("Account/Register")]
        [HttpPost]
        public async Task<IActionResult> singup(singupmodel singup)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.createuserAsync(singup);
                if (!result.Succeeded)
                {
                    foreach (var errorMessage in result.Errors)
                    {
                        ModelState.AddModelError("", errorMessage.Description);
                    }
                    return View(singup);
                }
                ModelState.Clear();
                return RedirectToAction("ConfirmEmails", new { email = singup.email});
            }
            return View(singup);
        }
        [Route("Account/Login")]
        public IActionResult login()
        {
            return View();
        }
        [Route("Account/Login")]
        [HttpPost]
        public async Task<IActionResult> login(singinmodel signInModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.PasswordSignInAsync(signInModel);
                if (result.Succeeded)
                {
                   
                        return RedirectToAction("Index", "Home");
                 }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("","Account is Block Try after 2Hr");
                }
                else { ModelState.AddModelError("", "Invalid Credetnial"); }
                
                     
            }

            return View(signInModel);
        }
        public async Task<ActionResult> logout()
        {
            await _accountRepository.logoutsync();
            return RedirectToAction("Index","Home");
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmails(string uid, string token, string email)
        {
            EmailCofirmModel model = new EmailCofirmModel
            {
                Email = email
            };

            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                token = token.Replace(' ', '+');
                var result = await _accountRepository.ConfirmEmailAsync(uid, token);
                if (result.Succeeded)
                {
                    model.EmailVerified = true;
                }
            }

            return View(model);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmails(EmailCofirmModel model)
        {
            var user = await _accountRepository.GetUserByEmailAsync(model.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    model.EmailVerified = true;
                    return View(model);
                }

                await _accountRepository.GenerateEmailConfirmationTokenAsync(user);
                model.EmailSent = true;
                ModelState.Clear();
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong.");
            }
            return View(model);
        }
        [AllowAnonymous, HttpGet("fotgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [AllowAnonymous, HttpPost("fotgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswodModel forgot)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountRepository.GetUserByEmailAsync(forgot.Email);
                if (user != null)
                {
                    await _accountRepository.GenerateForgotPasswordTokenAsync(user);
                }
                ModelState.Clear();
                forgot.EmailSent = true;
            }
            return View(forgot);
        }
        [AllowAnonymous, HttpGet("Reset-password")]
        public IActionResult ResetPassword(string uid,string token)
        {
            ResetPasswordModel reset = new ResetPasswordModel
            {
                Token = token,
                UserId=uid
            };
            return View();
        }

        [AllowAnonymous, HttpPost("Reset-password")]
        public async Task<IActionResult> Resetpassword(ResetPasswordModel reset)
        {
            if (ModelState.IsValid)
            {
                reset.Token = reset.Token.Replace(' ','+');
                var res = await _accountRepository.ResetPasswordAsync(reset);
                if (res.Succeeded)
                {
                    ModelState.Clear();
                    reset.IsSuccess = true;
                    return View(reset);
                }
                foreach (var errorMessage in res.Errors)
                {
                    ModelState.AddModelError("", errorMessage.Description);
                }
            }
            return View(reset);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

