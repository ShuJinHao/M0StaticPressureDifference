using Newtonsoft.Json;
using StaticPressureDifference.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticPressureDifference.Mes
{
    public class MesService
    {
        private HttpClientManager _http;
        public string _apiUrl = "http://172.30.1.100:9999/api/Pressure/Upload";

        public MesService()
        {
            // 直接 new，不需要传参了，参数我都写死在静态构造函数里了
            _http = new HttpClientManager();
        }

        public async Task<bool> UploadReportAsync(ProductReport report, Action<string> logAction)
        {
            try
            {
                // 1. 数据转换 (保持不变)
                var postData = new List<MesRequestItem>();
                foreach (var detail in report.Details)
                {
                    postData.Add(new MesRequestItem
                    {
                        cellCode = report.Barcode,
                        itemCode = detail.ItemName,
                        minValue = detail.MinValue,
                        maxValue = detail.MaxValue,
                        unit = detail.Unit,
                        actualValue = detail.TestValue.ToString(),
                        result = detail.Result
                    });
                }

                // 2. 序列化 (保持不变)
                string jsonContent = JsonConvert.SerializeObject(postData);

                // 3. ★★★ 修改这里：调用新的 PostJsonAsync 方法 ★★★
                string responseStr = await _http.PostJsonAsync(_apiUrl, jsonContent);

                // 4. 解析结果 (保持不变)
                var res = JsonConvert.DeserializeObject<MesResponse>(responseStr);

                if (res != null && res.code == 200 && res.success)
                {
                    logAction($"MES上传成功: {report.Barcode}");
                    return true;
                }
                else
                {
                    logAction($"MES上传失败: {report.Barcode}, Msg: {res?.msg}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logAction($"MES通讯异常: {ex.Message}");
                return false;
            }
        }

        // Dispose 可以去掉了，因为 HttpClient 变成了静态单例，不需要释放
        public void Dispose() { }
    }
}