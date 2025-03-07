﻿using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Panda.Tools.Web;

public class HttpContextLocal
{
    private static Func<object> _asyncLocalAccessor;
    private static Func<object, object> _holderAccessor;
    private static Func<object, HttpContext> _httpContextAccessor;

    public static HttpContext? GetCurrentHttpContext()
    {
        var asyncLocal = (_asyncLocalAccessor ??= CreateAsyncLocalAccessor())();

        var holder = (_holderAccessor ??= CreateHolderAccessor(asyncLocal))(asyncLocal);
        return holder switch
        {
            null => null,
            _ => (_httpContextAccessor ??= CreateHttpContextAccessor(holder))(holder)
        };

        static Func<object> CreateAsyncLocalAccessor()
        {
            var fieldInfo =
                typeof(HttpContextAccessor).GetField("_httpContextCurrent",
                    BindingFlags.Static | BindingFlags.NonPublic);
            var field = Expression.Field(null, fieldInfo!);
            return Expression.Lambda<Func<object>>(field).Compile();
        }

        static Func<object, object> CreateHolderAccessor(object asyncLocal)
        {
            var holderType = asyncLocal.GetType().GetGenericArguments()[0];
            var method = typeof(AsyncLocal<>).MakeGenericType(holderType).GetProperty("Value")?.GetGetMethod();
            var target = Expression.Parameter(typeof(object));
            var convert = Expression.Convert(target, asyncLocal.GetType());
            var getValue = Expression.Call(convert, method!);
            return Expression.Lambda<Func<object, object>>(getValue, target).Compile();
        }

        static Func<object, HttpContext> CreateHttpContextAccessor(object holder)
        {
            var target = Expression.Parameter(typeof(object));
            var convert = Expression.Convert(target, holder.GetType());
            var field = Expression.Field(convert, "Context");
            var convertAsResult = Expression.Convert(field, typeof(HttpContext));
            return Expression.Lambda<Func<object, HttpContext>>(convertAsResult, target).Compile();
        }
    }
}