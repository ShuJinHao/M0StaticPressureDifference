using _002JournalEncapsulation;
using Microsoft.Win32;
using StaticPressureDifference.DataModel;
using StaticPressureDifference.Mes;

// using StaticPressureDifference.ExcelManange; // 如果你的 ExcelRead 在这个命名空间下，请取消注释
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SystemSet;

namespace StaticPressureDifference
{
    public partial class Form1 : Form
    {
        // 定时器
        private System.Windows.Forms.Timer _scanTimer;

        // 日志与工具类
        private LogItem log = null;

        private ExcelRead _excel;
        private JournalInfo JN;
        private SystemSetBll systemSetBll;

        // MES 服务类
        private MesService _mesService;

        // 记录已处理文件，避免重复读取
        private HashSet<string> _processedFiles = new HashSet<string>();

        // 缓存文件保存路径 (保存在软件运行目录下)
        private string _cacheFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProcessedHistory.txt");

        public Form1()
        {
            InitializeComponent();
            // 这行代码会移除右上角所有的按钮（最小化、最大化、关闭）
            this.ControlBox = false;
            // 1. 绑定窗体关闭事件 (用于保存记录)
            this.FormClosing += Form1_FormClosing;

            // 2. 软件启动时，先加载历史记录
            LoadCache();

            // 3. 初始化 Timer 和其他对象
            InitCustomTimer();
        }

        private void InitCustomTimer()
        {
            try
            {
                SetAutoStart(true);
                // 初始化读取帮助类
                _excel = new ExcelRead();

                // 初始化 MES 服务
                _mesService = new MesService();

                systemSetBll = SystemSetBll.CreateSsb("Config", "SystemConfig");
                _mesService._apiUrl = systemSetBll.GetValue<string>(PNameSystem.MES条码上传地址);
                // 初始化日志
                JN = JournalFactory.Greate(@"D:\日志记录", DateTime.Now.ToString("yyyy-MM-dd"));

                _scanTimer = new System.Windows.Forms.Timer();
                _scanTimer.Interval = 3000; // 3000毫秒 = 3秒

                // 绑定事件
                _scanTimer.Tick += OnScanTimerTick;

                _scanTimer.Start(); // 启动
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化失败: " + ex.Message);
            }
        }

        // 放在 Form1 类里面
        public void SetAutoStart(bool enable)
        {
            string appName = "MyIndustrialApp"; // 你的程序名，必须唯一
            string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

            try
            {
                // OpenSubKey 的第二个参数 true 表示我们需要"写"权限
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, true))
                {
                    if (key == null) return; // 防御性编程

                    if (enable)
                    {
                        // 加上引号，防止路径带空格导致启动失败
                        key.SetValue(appName, "\"" + Application.ExecutablePath + "\"");
                    }
                    else
                    {
                        // 如果本来就没设置过，删除时可能会报错，所以要判断一下
                        if (key.GetValue(appName) != null)
                        {
                            key.DeleteValue(appName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("设置开机自启失败，请检查杀毒软件拦截。\n" + ex.Message);
            }
        }

        // ==========================================
        // 核心逻辑：定时扫描 (包含跨天处理)
        // ==========================================
        private async void OnScanTimerTick(object sender, EventArgs e)
        {
            // 暂停 Timer 防止重入
            _scanTimer.Stop();

            try
            {
                // 优化策略：同时扫描 [今天] 和 [昨天]
                var daysToCheck = new DateTime[]
                {
                    DateTime.Now,            // 今天
                    DateTime.Now.AddDays(-1) // 昨天
                };

                foreach (var day in daysToCheck)
                {
                    string dateFolder = day.ToString("yyyyMMdd");
                    //string targetPath = Path.Combine(@"D:\新建文件夹", dateFolder);
                    string targetPath = Path.Combine(systemSetBll.GetValue<string>(PNameSystem.采集文件夹), dateFolder);

                    // 如果文件夹不存在，跳过
                    if (!Directory.Exists(targetPath)) continue;

                    // 找该文件夹下所有的 xls 文件 (兼容 .xls)
                    var files = Directory.GetFiles(targetPath, "*.xls");

                    foreach (var file in files)
                    {
                        // ★去重检查★
                        if (_processedFiles.Contains(file)) continue;

                        // 尝试读取并上传 (异步等待)
                        bool isHandled = await TryReadAndUpload(file);

                        if (isHandled)
                        {
                            _processedFiles.Add(file); // 读取且上传成功才加入记录
                            SaveCache();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"轮询出错: {ex.Message}");
            }
            finally
            {
                // 恢复 Timer
                if (!_scanTimer.Enabled) _scanTimer.Start();
            }
        }

        // ==========================================
        // 核心逻辑：读取 Excel 并上传
        // ==========================================
        private async Task<bool> TryReadAndUpload(string filePath)
        {
            try
            {
                // 1. 解析 Excel
                ProductReport report = _excel.ParseProductFile(filePath);

                Log($"解析成功: {Path.GetFileName(filePath)} | 条码: {report.Barcode}");

                // 2. 上传给 MES (异步)
                // 传入 Log 方法的委托，让 Service 也能打印日志到界面
                bool uploadSuccess = await _mesService.UploadReportAsync(report, (msg) => Log(msg));

                if (uploadSuccess)
                {
                    return true; // 解析且上传成功 -> 加入已处理列表
                }
                else
                {
                    // 上传失败，返回 false，以便下次轮询重试
                    Log("等待下次轮询重试上传...");
                    return false;
                }
            }
            catch (IOException)
            {
                // 文件被占用，返回 false 以便下次重试
                return false;
            }
            catch (Exception ex)
            {
                Log($"文件处理异常: {ex.Message}");
                return true; // 烂文件标记为已处理，防止死循环
            }
        }

        // ==========================================
        // 之前遗漏的完整代码：历史记录加载
        // ==========================================
        private void LoadCache()
        {
            try
            {
                // 如果文件存在，读取所有行
                if (File.Exists(_cacheFilePath))
                {
                    var lines = File.ReadAllLines(_cacheFilePath);
                    // 只有非空行才加入
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            _processedFiles.Add(line);
                        }
                    }
                    Log($"系统恢复：已加载 {lines.Length} 条历史上传记录。");
                }
                else
                {
                    // 文件不存在，创建个空的防止报错
                    File.Create(_cacheFilePath).Close();
                }
            }
            catch (Exception ex)
            {
                Log($"读取历史记录失败: {ex.Message}");
            }
        }

        // ==========================================
        // 之前遗漏的完整代码：保存并清理缓存
        // ==========================================
        private void SaveCache()
        {
            try
            {
                // 1. 定义保留范围：只保留 [今天] 和 [昨天] 的文件夹数据
                string todayStr = DateTime.Now.ToString("yyyyMMdd");
                string yesterdayStr = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

                // 2. 筛选有效记录 (清理掉前天及更早的数据，防止文件无限膨胀)
                var validRecords = _processedFiles.Where(filePath =>
                {
                    try
                    {
                        // 获取文件所在的文件夹名 (即日期)
                        string dirName = Path.GetFileName(Path.GetDirectoryName(filePath));
                        return dirName == todayStr || dirName == yesterdayStr;
                    }
                    catch
                    {
                        return false; // 路径非法的直接丢弃
                    }
                }).ToList();

                // 3. 写入文件 (覆盖写入)
                File.WriteAllLines(_cacheFilePath, validRecords);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存历史记录失败: {ex.Message}");
            }
        }

        // ==========================================
        // 窗体关闭事件
        // ==========================================
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 停止定时器
            if (_scanTimer != null) _scanTimer.Stop();

            // 释放资源
            _mesService?.Dispose();

            // ★关键：保存历史记录★
            SaveCache();
        }

        // ==========================================
        // 日志逻辑 (保持原样)
        // ==========================================
        private void Log(string msg)
        {
            if (JN == null) return;

            JN.ShowLog(msg);
            if (JN.LogQueue.RunningLogQueue_HaveElement())
            {
                log = null;
                JN.LogQueue.RunningLogQueue_GetOne(out log);
                if (log != null)
                {
                    string t = log.time + " " + log.text;
                    AddLog(t);
                    JN.Save(log);
                }
            }
        }

        public void AddLog(string txt)
        {
            if (this.IsDisposed) return;
            this.Invoke(new Action(() =>
            {
                if (this.txtLog.Lines.Length > 1000) this.txtLog.Clear();
                this.txtLog.AppendText(txt + "\r\n");
                this.txtLog.Select(this.txtLog.TextLength, 0);
                this.txtLog.ScrollToCaret();
            }));
        }

        private void 系统配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SystemSet.SystemSetFrom("Config", "SystemConfig").ShowDialog();
        }
    }
}