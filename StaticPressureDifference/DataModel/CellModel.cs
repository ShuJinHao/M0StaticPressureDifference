using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticPressureDifference.DataModel
{
    // 1. 对应 Excel 下半部分的每一行详细测试数据
    public class TestDetail
    {
        public string ItemName { get; set; }   // 项目List (对应A列)
        public string MinValue { get; set; }   // 最小值 (对应B列，用string防空值报错)
        public string MaxValue { get; set; }   // 最大值 (对应C列)
        public string Unit { get; set; }       // 单位 (对应D列)
        public double TestValue { get; set; }  // 测试值 (对应E列)
        public string Result { get; set; }     // 判定 (对应F列)
    }

    // 2. 对应整个 Excel 文件（包含文件名里的信息）
    public class ProductReport
    {
        public string FilePath { get; set; }

        // --- 从文件名解析 ---
        public string Barcode { get; set; }    // 条码

        public string ModelType { get; set; }  // 规格/类型
        public DateTime TestTime { get; set; } // 测试时间

        // --- 从 Excel 内容解析 ---
        public List<TestDetail> Details { get; set; } = new List<TestDetail>();
    }
}