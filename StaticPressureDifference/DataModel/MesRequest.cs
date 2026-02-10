using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticPressureDifference.DataModel
{
    // 1. 发送给 MES 的单项数据结构
    public class MesRequestItem
    {
        public string cellCode { get; set; }     // 条码 (来自 ProductReport.Barcode)
        public string itemCode { get; set; }     // 项目名称 (来自 TestDetail.ItemName)
        public string minValue { get; set; }     // 最小值
        public string maxValue { get; set; }     // 最大值
        public string unit { get; set; }         // 单位
        public string actualValue { get; set; }  // 测试值 (注意：MES要求是字符串)
        public string result { get; set; }       // 结果 (OK/NG)
    }

    // 2. MES 返回的结构
    public class MesResponse
    {
        public int code { get; set; }
        public string data { get; set; }
        public string msg { get; set; }
        public bool success { get; set; }
    }
}