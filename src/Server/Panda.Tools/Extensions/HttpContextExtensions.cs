﻿using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Panda.Tools.Exception;
using System.Web;

namespace Panda.Tools.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    ///     获取当前登录人Id
    /// </summary>
    /// <param name="http"></param>
    /// <returns></returns>
    public static Guid? CurrentAccountId(this HttpContext http)
    {
        var identity = http.User;
        var id = identity.Claims.Where(a => a.Type == "Id")
            .Select(a => a.Value).FirstOrDefault();
        return id != null ? Guid.Parse(id) : Guid.Empty;
    }

    /// <summary>
    /// 获取客户端的真实IP地址
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetClientIp(this HttpContext context)
    {
        var ip = context.Request.Headers["Cdn-Src-Ip"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ip))
            return IpReplace(ip);

        ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ip))
            return IpReplace(ip);

        ip = context.Connection.RemoteIpAddress?.ToString();

        return IpReplace(ip!);
    }

    static string IpReplace(string ip)
    {
        //::ffff:
        //::ffff:192.168.2.131 这种IP处理
        if (ip.Contains("::ffff:"))
        {
            ip = ip.Replace("::ffff:", "");
        }

        return ip;
    }


    /// <summary>
    /// 获取当前请求完整的Url地址
    /// </summary>
    /// <returns></returns>
    public static string GetFullUrl(this IHttpContextAccessor httpContextAccessor)
    {
        return new StringBuilder()
            .Append(httpContextAccessor.HttpContext?.Request.Scheme)
            .Append("://")
            .Append(httpContextAccessor.HttpContext?.Request.Host)
            .Append(httpContextAccessor.HttpContext?.Request.PathBase)
            .Append(httpContextAccessor.HttpContext?.Request.Path)
            .Append(httpContextAccessor.HttpContext?.Request.QueryString)
            .ToString();
    }

    /// <summary>
    ///     是否是Ajax请求
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public static bool IsAjax(this HttpRequest req)
    {
        var result = false;
        var xreq = req.Headers.ContainsKey("x-requested-with");
        if (xreq) result = req.Headers["x-requested-with"] == "XMLHttpRequest";
        return result;
    }

    /// <summary>
    /// 返回值是否是需要json
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public static bool IsResponseJson(this HttpRequest req)
    {
        if (req.Headers.TryGetValue("Content-Type", out var value))
        {
            return value.ToString().Contains("json");
        }

        return false;
    }

    public static Guid GetAccountId(this HttpContext Context)
    {
        var claimsIdentity = Context?.User.Identity as ClaimsIdentity;
        var accountId = claimsIdentity?.Claims.Where(a => a.Type == "Id").Select(a => a.Value).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new UserException("获取用户信息失败", HttpStatusCode.Unauthorized);
        }

        return Guid.Parse(accountId);
    }

    public static bool IsAdmin(this HttpContext Context)
    {
        var claimsIdentity = Context?.User.Identity as ClaimsIdentity;
        var isAdmin = claimsIdentity?.Claims.Where(a => a.Type == "IsAdmin").Select(a => a.Value).FirstOrDefault();
        if (isAdmin == null)
        {
            return false;
        }

        return isAdmin == "True";
    }

    public static string GetUserAgent(this HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("User-Agent", out var ua))
        {
            return ua.ToString();
        }

        return "";
    }
}