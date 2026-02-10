using ExcelDataReader;
using StaticPressureDifference.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ExcelRead
{
    public ProductReport ParseProductFile(string filePath)
    {
        var report = new ProductReport { FilePath = filePath };

        // 1. 解析文件名 (这部分保持不变)
        try
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string[] parts = fileName.Split('_');
            var validParts = parts.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (validParts.Length >= 3)
            {
                report.Barcode = validParts[0];
                report.ModelType = validParts[1];
                string timeStr = validParts[2];
                if (timeStr.Length == 14)
                {
                    if (DateTime.TryParseExact(timeStr, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
                    {
                        report.TestTime = dt;
                    }
                }
            }
        }
        catch { }

        // 2. 解析 Excel 内容
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // =======================================================
                // ★★★ 核心修改在这里 ★★★
                // 不要用 CreateBinaryReader，改用 CreateReader
                // 它会自动检测文件到底是 xls 还是 xlsx，无论后缀名是什么
                // =======================================================
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    bool foundHeader = false;

                    while (reader.Read())
                    {
                        // 获取第 0 列 (对应 A 列)
                        string colA = reader.GetValue(0)?.ToString()?.Trim() ?? "";

                        // 1. 寻找锚点 "项目List"
                        if (!foundHeader)
                        {
                            if (colA == "项目List")
                            {
                                foundHeader = true;
                            }
                            continue;
                        }

                        // 2. 读取数据行
                        if (string.IsNullOrWhiteSpace(colA)) continue;

                        var detail = new TestDetail();

                        // 映射 A=0, B=1, C=2...
                        detail.ItemName = colA;
                        detail.MinValue = reader.GetValue(1)?.ToString();
                        detail.MaxValue = reader.GetValue(2)?.ToString();
                        detail.Unit = reader.GetValue(3)?.ToString();

                        var valObj = reader.GetValue(4);
                        if (valObj != null)
                        {
                            if (double.TryParse(valObj.ToString(), out double val))
                            {
                                detail.TestValue = val;
                            }
                        }

                        detail.Result = reader.GetValue(5)?.ToString();

                        report.Details.Add(detail);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 可以在 Form1 里捕获这个异常并打印，方便调试
            throw;
        }

        return report;
    }
}