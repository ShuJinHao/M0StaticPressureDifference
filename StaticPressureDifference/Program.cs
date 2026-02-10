using StaticPressureDifference;
using System;
using System.Threading; // 必须引用
using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        bool createdNew;

        // 1. 创建互斥体
        // "Global\\YourAppName_UUID" 是互斥体的名称。
        // 为了确保系统唯一性，建议加上 "Global\" 前缀，后面跟一个 GUID 或项目名称。
        using (Mutex mutex = new Mutex(true, "Global\\MyUniqueApp_123456", out createdNew))
        {
            // 2. 检查是否是第一个实例
            if (createdNew)
            {
                // 如果是第一个实例，正常启动
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                // 3. 如果不是第一个实例（createdNew 为 false），说明已经有一个在跑了
                // 你可以选择弹窗提示，或者直接静默退出
                MessageBox.Show("程序已经运行中，请勿重复启动。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // 退出当前尝试启动的实例
                return;
            }
        }
    }
}