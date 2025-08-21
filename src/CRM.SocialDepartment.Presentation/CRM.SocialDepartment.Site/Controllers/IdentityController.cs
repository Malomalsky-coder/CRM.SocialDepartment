using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.SocialDepartment.Site.Controllers;

[AllowAnonymous]
public class IdentityController : Controller
{
    [HttpGet]
    [Route("Identity/Account/Register")]
    public IActionResult Register()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Register")]
    public IActionResult RegisterPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/RegisterConfirmation")]
    public IActionResult RegisterConfirmation()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/ForgotPassword")]
    public IActionResult ForgotPassword()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/ForgotPassword")]
    public IActionResult ForgotPasswordPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/ForgotPasswordConfirmation")]
    public IActionResult ForgotPasswordConfirmation()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/ResetPassword")]
    [Route("Identity/Account/ResetPassword/{code}")]
    public IActionResult ResetPassword(string code = null)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/ResetPassword")]
    [Route("Identity/Account/ResetPassword/{code}")]
    public IActionResult ResetPasswordPost(string code = null)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/ResetPasswordConfirmation")]
    public IActionResult ResetPasswordConfirmation()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/ConfirmEmail")]
    [Route("Identity/Account/ConfirmEmail/{userId}/{code}")]
    public IActionResult ConfirmEmail(string userId = null, string code = null)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/ConfirmEmailChange")]
    [Route("Identity/Account/ConfirmEmailChange/{userId}/{email}/{code}")]
    public IActionResult ConfirmEmailChange(string userId = null, string email = null, string code = null)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/ResendEmailConfirmation")]
    public IActionResult ResendEmailConfirmation()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/ResendEmailConfirmation")]
    public IActionResult ResendEmailConfirmationPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/ExternalLogin")]
    [Route("Identity/Account/ExternalLogin/{provider}")]
    public IActionResult ExternalLogin(string provider = null)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/ExternalLogin")]
    [Route("Identity/Account/ExternalLogin/{provider}")]
    public IActionResult ExternalLoginPost(string provider = null)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/ExternalLoginCallback")]
    public IActionResult ExternalLoginCallback()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Lockout")]
    public IActionResult Lockout()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/LoginWith2fa")]
    public IActionResult LoginWith2fa()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/LoginWith2fa")]
    public IActionResult LoginWith2faPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/LoginWithRecoveryCode")]
    public IActionResult LoginWithRecoveryCode()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/LoginWithRecoveryCode")]
    public IActionResult LoginWithRecoveryCodePost()
    {
        return RedirectToAction("Index", "Home");
    }

    // Перенаправление страниц управления аккаунтом
    [HttpGet]
    [Route("Identity/Account/Manage")]
    [Route("Identity/Account/Manage/Index")]
    public IActionResult Manage()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage")]
    [Route("Identity/Account/Manage/Index")]
    public IActionResult ManagePost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/Email")]
    public IActionResult Email()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/Email")]
    public IActionResult EmailPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/Email/{userId}")]
    public IActionResult EmailWithUserId(string userId)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/Email/{userId}")]
    public IActionResult EmailWithUserIdPost(string userId)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/ChangePassword")]
    public IActionResult ChangePassword()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/ChangePassword")]
    public IActionResult ChangePasswordPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/SetPassword")]
    public IActionResult SetPassword()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/SetPassword")]
    public IActionResult SetPasswordPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/TwoFactorAuthentication")]
    public IActionResult TwoFactorAuthentication()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/TwoFactorAuthentication")]
    public IActionResult TwoFactorAuthenticationPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/EnableAuthenticator")]
    public IActionResult EnableAuthenticator()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/EnableAuthenticator")]
    public IActionResult EnableAuthenticatorPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/Disable2fa")]
    public IActionResult Disable2fa()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/Disable2fa")]
    public IActionResult Disable2faPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/GenerateRecoveryCodes")]
    public IActionResult GenerateRecoveryCodes()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/GenerateRecoveryCodes")]
    public IActionResult GenerateRecoveryCodesPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/ResetAuthenticator")]
    public IActionResult ResetAuthenticator()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/ResetAuthenticator")]
    public IActionResult ResetAuthenticatorPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/ExternalLogins")]
    public IActionResult ExternalLogins()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/ExternalLogins")]
    public IActionResult ExternalLoginsPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/RemoveLogin")]
    [Route("Identity/Account/Manage/RemoveLogin/{loginProvider}/{providerKey}")]
    public IActionResult RemoveLogin(string loginProvider = null, string providerKey = null)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/RemoveLogin")]
    [Route("Identity/Account/Manage/RemoveLogin/{loginProvider}/{providerKey}")]
    public IActionResult RemoveLoginPost(string loginProvider = null, string providerKey = null)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/LinkLogin")]
    [Route("Identity/Account/Manage/LinkLogin/{provider}")]
    public IActionResult LinkLogin(string provider = null)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/LinkLogin")]
    [Route("Identity/Account/Manage/LinkLogin/{provider}")]
    public IActionResult LinkLoginPost(string provider = null)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/LinkLoginCallback")]
    public IActionResult LinkLoginCallback()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/PersonalData")]
    public IActionResult PersonalData()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/DownloadPersonalData")]
    public IActionResult DownloadPersonalData()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/DownloadPersonalData")]
    public IActionResult DownloadPersonalDataPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/Manage/DeletePersonalData")]
    public IActionResult DeletePersonalData()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/DeletePersonalData")]
    public IActionResult DeletePersonalDataPost()
    {
        return RedirectToAction("Index", "Home");
    }

    // Обработка маршрутов с параметрами в URL
    [HttpGet]
    [Route("Identity/Account/Manage/{*catchAll}")]
    public IActionResult ManageCatchAll()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/Account/Manage/{*catchAll}")]
    public IActionResult ManageCatchAllPost()
    {
        return RedirectToAction("Index", "Home");
    }

    // Обработка всех остальных маршрутов Identity
    [HttpGet]
    [Route("Identity/{*catchAll}")]
    public IActionResult IdentityCatchAll()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Route("Identity/{*catchAll}")]
    public IActionResult IdentityCatchAllPost()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("Identity/Account/ShowRecoveryCodes")]
    public IActionResult ShowRecoveryCodes()
    {
        return RedirectToAction("Index", "Home");
    }
} 