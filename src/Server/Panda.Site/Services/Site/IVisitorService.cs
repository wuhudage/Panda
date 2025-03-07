﻿namespace Panda.Site.Services.Site;

public interface IVisitorService
{
    public const string VisitorCookieNickName = "VisitorCookieNickName";
    public const string VisitorCookieNickKey = "VisitorCookieNickKey";
    public const string VisitorCookieNickEmail = "VisitorCookieNickEmail";

    /// <summary>
    /// 发送验证码
    /// </summary>
    /// <param name="nickName"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    Task SendVerificationCode(string nickName, string email);

    /// <summary>
    /// 确认验证码
    /// </summary>
    /// <param name="email"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    Task ConfirmEmail(string email, string code);

    /// <summary>
    /// 检查当前访客身份
    /// </summary>
    /// <returns></returns>
    Task CheckVisitorIdentity();
}