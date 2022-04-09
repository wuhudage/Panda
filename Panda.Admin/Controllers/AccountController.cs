using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panda.Admin.Models;
using Panda.Admin.Models.Request;
using Panda.Admin.Services.Account;
using Panda.Entity.Responses;
using Panda.Tools.Auth.Models;
using Panda.Tools.Exception;

namespace Panda.Admin.Controllers;

public class AccountController : AdminController
{
    private readonly IAccountService<Accounts> _accountService;


    public AccountController(IAccountService<Accounts> accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    ///     登录
    /// </summary>
    /// <param name="request"></param>
    /// <exception cref="UserException"></exception>
    [AllowAnonymous]
    [HttpPost]
    public async Task Login(AccountLoginRequest request)
    {
        var res = await _accountService.LoginAsync(request.UserName, request.Password);
        if (res.IsSuccess == false) throw new UserException(res.Message);
    }

    [HttpGet]
    public async Task LoginOutAsync()
    {
        await HttpContext.SignOutAsync();
    }

    [AllowAnonymous]
    [HttpPost("/admin/createAdminAccount")]
    public async Task<IActionResult> CreateAdminAccount(CreateAdminAccountRequest request)
    {
        await _accountService.CreateAdminAccount(request);
        return Content("初始化账号成功");
    }


    /// <summary>
    /// 检查当前状态
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    public async Task<LoginStatusResult> IsLogin()
    {
        var isLogin = HttpContext.User.Identity is {IsAuthenticated: true};
        return new LoginStatusResult()
        {
            IsLogin = isLogin,
            IsInit = await _accountService.CheckAdminAccountExistAsync()
        };
    }

    [HttpPost]
    public async Task ChangePwd(ChangePwdRequest request)
    {
        await _accountService.ChangePwdAsync(request.OldPwd, request.NewPwd);
    }
}