using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace ContactSample
{
    public static class Common
    {
        public static string ResolveIPAddress(HttpContext context)
        {
            return context?.Connection?.RemoteIpAddress?.ToString();
        }

        public static string GetJsonFromException(Exception ex)
        {
            if (ex == null) return null;
            return JsonConvert.SerializeObject(ex);
        }

        
    }
}
