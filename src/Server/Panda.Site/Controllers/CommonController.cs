﻿using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panda.Admin.Services.Account;
using Panda.Site.Models;
using Panda.Tools.Auth.Models;
using Panda.Tools.Exception;
using Panda.Tools.FileStorage;
using Panda.Tools.Models;
using Panda.Tools.Security;
using Panda.Tools.Web;

namespace Panda.Controllers;

/// <summary>
/// 公共接口
/// </summary>
[ApiController]
public class CommonController : Controller
{
    private readonly IAccountService _accountService;

    private readonly IFileStorage _fileStorage;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CommonController(IFileStorage fileStorage, IWebHostEnvironment webHostEnvironment,
        IAccountService accountService)
    {
        _fileStorage = fileStorage;
        _webHostEnvironment = webHostEnvironment;
        _accountService = accountService;
    }

    /// <summary>
    /// 图片文件上传
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    /// <exception cref="UserException"></exception>
    [HttpPost("/admin/upload")]
    public async Task<UploadResult> Upload(IFormCollection form)
    {
        var files = form.Files;
        if (files.Count == 0) throw new UserException("服务器没有收到文件");

        if (files[0].Length >= 1024 * 1024 * 8) //图片不能大于4M
            throw new UserException("图片不能大于4M");

        var list = new List<string> { "jpg", "png", "jpeg", "gif" };
        var regex = Regex.Match(files[0].FileName, @"[^\.]\w*$");
        if (regex.Success == false || list.Contains(regex.Value) == false) throw new UserException("文件格式不支持！");

        await using var stream = files[0].OpenReadStream();
        var buffer = new byte[stream.Length];
        var readAsync = await stream.ReadAsync(buffer);
        // todo 图片水印功能
        var file = await _fileStorage.SaveAsync(buffer, $"{Md5Helper.ComputeHash(buffer)[..10]}.{regex.Value}");
        if (file.Success == false) return new UploadResult { Code = 1, Message = file.Message };

        return new UploadResult
        {
            Code = 0,
            Url = file.Url
        };
    }

    /// <summary>
    /// base64图片上传
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [IgnoreAntiforgeryToken]
    [HttpPost("admin/upload/base64")]
    public async Task<UploadResult> UploadBase64([FromForm] UploadBase64Model request)
    {
        var bytes = Convert.FromBase64String(Base64Utils.GetBase64byImage(request.Base64));
        request.Base64 = ImageUtils.GetPicThumbnail(bytes, request.W, request.H);
        var mime = Base64Utils.GetBase64Mime(request.Base64);
        var extName = MimeUtils.GetMimeExtName(mime);
        ;
        var file = await _fileStorage.SaveAsync(bytes, $"{Md5Helper.ComputeHash(bytes)[..10]}.{extName}");
        if (file.Success == false) return new UploadResult { Code = 1, Message = file.Message };

        return new UploadResult
        {
            Code = 0,
            Url = file.Url
        };
    }

    [AllowAnonymous]
    [HttpGet("/img/{day}/{file}")]
    public ActionResult Img(string day, string file)
    {
        // todo 防盗链处理
        var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Content", "Upload", day, file);
        if (System.IO.File.Exists(path) == false) return new NotFoundResult();

        var ext = new FileInfo(path).Extension;
        return PhysicalFile(path, $"image/{ext[1..]}");
    }

    // [HttpGet("/sitemap.xml")]
    // public IActionResult SiteMap()
    // {
    //     var query = _context.Set<Posts>().OrderByDescending(a => a.UpdateTime);
    //     var sitemapConfig = new PostSiteMapConfiguration(query, 2);
    //
    //     return new DynamicSitemapIndexProvider().CreateSitemapIndex(
    //         new SitemapProvider(new BaseUrlProvider(_dicDataService)), sitemapConfig);
    // }
}