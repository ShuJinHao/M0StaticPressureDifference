using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _002JournalEncapsulation
{
        public class JournalInfo
        {
            /// <summary>
            /// 保存路径
            /// </summary>
            private string path_JournalSave { get; set; }
            /// <summary>
            /// 保存的名称
            /// </summary>
            private string name_JournalSave { get; set; }
            /// <summary>
            /// 日志队列
            /// </summary>
            public JournalQueue LogQueue;
            /// <summary>
        /// 构造函数
        /// </summary>
            protected internal JournalInfo(string path,string name)
                {
                    path_JournalSave = path;
                    name_JournalSave = name;
                    LogQueue = new JournalQueue();
                }
            /// <summary>
            /// 日志写入
            /// </summary>
            /// <param name="text"></param>
            public void ShowLog(string text)
                {
                    LogItem logitem = new LogItem();
                    logitem.text = text + "";
                    logitem.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    LogQueue.RunningLogQueue_AddOne(logitem);
                }
            /// <summary>
            /// 日志写入2
            /// </summary>
            /// <param name="text"></param>
            public void ShowLog(string title,string text)
            {
                LogItem logitem = new LogItem();
                logitem.text = text + "";
                logitem.title = title + "";
                logitem.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                LogQueue.RunningLogQueue_AddOne(logitem);
            }
            /// <summary>
            /// 日志保存
            /// </summary>
            /// <param name="log"></param>
            public void Save(LogItem log)
            {
                try
                {
                    if (!System.IO.Directory.Exists(path_JournalSave)) System.IO.Directory.CreateDirectory(path_JournalSave);
                    string df = DateTime.Now.ToLongDateString();
                    string addressLog = "";
                    addressLog = path_JournalSave + "\\" + name_JournalSave+".txt";
                    if (!File.Exists(addressLog)) File.Create(addressLog).Close();
                    StringBuilder strBuilderMessage = new StringBuilder();
                    strBuilderMessage.Append("日期:" + log.time + "\r\n");
                    strBuilderMessage.Append("标题:" + log.title + "\r\n");
                    strBuilderMessage.Append(log.text + "\r\n");
                    strBuilderMessage.Append("___________________________________________________\r\n");
                    using (FileStream fs = new FileStream(addressLog, FileMode.Append, FileAccess.Write))
                    {
                        StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                        sw.WriteLine(strBuilderMessage);
                        sw.Close();
                        fs.Close();
                    }
                }
                catch (Exception) { }
        }

        }
        public class LogItem
        {
            /// <summary>
            /// 时间
            /// </summary>
            public string time { get; set; }
            /// <summary>
            /// 标题
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string text { get; set; }

        }
}
