﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


namespace BlueFramework.Common.Http
{
    public class HttpWebRequestUtil
    {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        /// <summary>   
        /// 创建GET方式的HTTP请求   
        /// </summary>   
        /// <param name="url">请求的URL</param>   
        /// <param name="timeout">请求的超时时间</param>   
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>   
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>   
        /// <returns></returns>   
        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return request.GetResponse() as HttpWebResponse;
        }
        /// <summary>   
        /// 创建POST方式的HTTP请求   
        /// </summary>   
        /// <param name="url">请求的URL</param>   
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>   
        /// <param name="timeout">请求的超时时间</param>   
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>   
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>   
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>   
        /// <returns></returns>   
        public static string CreatePostHttpResponse(string url, IDictionary<string, object> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //如果是发送HTTPS请求   
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                CookieContainer cc = new CookieContainer();
                request.CookieContainer = cc;
            }
            //如果需要POST数据   
            if (!(parameters == null || parameters.Count == 0))
            {
                string postData = "";
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        postData += String.Format("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        postData = String.Format("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] postBytes = Encoding.GetEncoding("utf-8").GetBytes(postData);
                request.ContentLength = postBytes.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postBytes, 0, postBytes.Length);
                }
            }
            HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受   
        }   
    }
}
