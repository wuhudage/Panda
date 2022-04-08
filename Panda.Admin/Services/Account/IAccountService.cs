using Panda.Entity.Responses;
using Panda.Tools.Auth;
using Panda.Tools.Auth.Models;
using Panda.Tools.Auth.Request;
using Panda.Tools.Auth.Response;

namespace Panda.Admin.Services.Account;

public interface IAccountService<TU> where TU : Accounts
{
    // Task InitAsync();

    /// <summary>
    ///     登录
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<AuthResult> LoginAsync(string email, string password);

    /// <summary>
    ///     修改密码
    /// </summary>
    /// <param name="oldPwd"></param>
    /// <param name="newPwd"></param>
    /// <returns></returns>
    Task ChangePwdAsync(string oldPwd, string newPwd);


    /// <summary>
    ///     初始化管理员密码
    /// </summary>
    /// <returns></returns>
    Task InitAdminPassword();

    /// <summary>
    ///     初始化一个后台账号
    /// </summary>
    /// <returns></returns>
    Task InitAccount();


    /// <summary>
    ///     禁用账号
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    Task Disable(Guid accountId, bool status);


    /// <summary>
    ///     获取当前用户
    /// </summary>
    /// <returns></returns>
    Task<TU?> GetCurrentAccount();

    /// <summary>
    ///     获取用户列表
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    Task<PageDto<AccountResp>> GetAccountList(AccountReq req);
}