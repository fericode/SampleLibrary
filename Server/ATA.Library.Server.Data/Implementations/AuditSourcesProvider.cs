﻿using ATA.Library.Server.Model.Entities.AuditLogAssets;
using ATA.Library.Server.Model.Entities.Contracts.AuditLog;
using Microsoft.AspNetCore.Http;
using System;
using System.Reflection;

namespace ATA.Library.Server.Data.Implementations
{
    public class AuditSourcesProvider : IAuditSourcesProvider
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;

        public AuditSourcesProvider(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public virtual AuditSourceValues GetAuditSourceValues()
        {
            var httpContext = HttpContextAccessor.HttpContext;

            return new AuditSourceValues
            {
                HostName = GetHostName(httpContext),
                MachineName = GetComputerName(httpContext),
                LocalIpAddress = GetLocalIpAddress(httpContext),
                RemoteIpAddress = GetRemoteIpAddress(httpContext),
                UserAgent = GetUserAgent(httpContext),
                ApplicationName = GetApplicationName(httpContext),
                ClientName = GetClientName(httpContext),
                ClientVersion = GetClientVersion(httpContext),
                ApplicationVersion = GetApplicationVersion(httpContext),
                Other = GetOther(httpContext)
            };
        }

        protected virtual string? GetUserAgent(HttpContext httpContext)
        {
            return httpContext.Request?.Headers["User-Agent"].ToString();
        }

        protected virtual string? GetRemoteIpAddress(HttpContext httpContext)
        {
            return httpContext.Connection?.RemoteIpAddress?.ToString();
        }

        protected virtual string? GetLocalIpAddress(HttpContext httpContext)
        {
            return httpContext.Connection?.LocalIpAddress?.ToString();
        }

        protected virtual string GetHostName(HttpContext httpContext)
        {
            return httpContext.Request.Host.ToString();
        }

        protected virtual string GetComputerName(HttpContext httpContext)
        {
            return Environment.MachineName;
        }
        protected virtual string? GetApplicationName(HttpContext httpContext)
        {
            return Assembly.GetEntryAssembly()?.GetName().Name;
        }

        protected virtual string? GetApplicationVersion(HttpContext httpContext)
        {
            return Assembly.GetEntryAssembly()?.GetName().Version.ToString();
        }

        protected virtual string? GetClientVersion(HttpContext httpContext)
        {
            return httpContext.Request?.Headers["client-version"];
        }
        protected virtual string? GetClientName(HttpContext httpContext)
        {
            return httpContext.Request?.Headers["client-name"];
        }

        protected virtual string? GetOther(HttpContext httpContext)
        {
            return null;
        }
    }
}