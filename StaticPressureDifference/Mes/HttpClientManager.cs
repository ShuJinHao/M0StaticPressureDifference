using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StaticPressureDifference.Mes
{
    public class HttpClientManager
    {
        // 静态单例，防止 socket 耗尽（在 Framework 下非常重要）
        private static readonly HttpClient _client;

        static HttpClientManager()
        {
            // 1. ★★★ Win7 必须强制开启 TLS 1.2 ★★★
            // 否则会报错 "基础连接已经关闭: 发送时发生错误"
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                // 忽略 SSL 证书错误 (工厂内网常用，如果是公网建议去掉)
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }
            catch { /* 防止低版本 Framework 报错 */ }

            _client = new HttpClient();
            // 默认超时时间 30秒
            _client.Timeout = TimeSpan.FromSeconds(30);
            _client.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        /// <summary>
        /// 发送 POST 请求 (JSON)
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="jsonContent">JSON 字符串</param>
        /// <returns>服务器返回的字符串</returns>
        public async Task<string> PostJsonAsync(string url, string jsonContent)
        {
            try
            {
                // 创建 HttpContent
                using (var content = new StringContent(jsonContent, Encoding.UTF8, "application/json"))
                {
                    // 发送请求
                    var response = await _client.PostAsync(url, content);

                    // 确保状态码是 200-299，否则抛出异常
                    response.EnsureSuccessStatusCode();

                    // 读取返回内容
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                // 可以在这里记录底层日志
                throw new Exception("HTTP请求失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 发送 GET 请求
        /// </summary>
        public async Task<string> GetAsync(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}