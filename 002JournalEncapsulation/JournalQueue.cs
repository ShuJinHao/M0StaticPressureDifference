using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _002JournalEncapsulation
{
    /// <summary>
    /// 日志队列
    /// </summary>
    public class JournalQueue
    {
        /// <summary>
        /// 日志队列    
        /// </summary>
        private ConcurrentQueue<LogItem> RunningLogQueue = new ConcurrentQueue<LogItem>();

        /// <summary>
        /// 向UI日志显示队列中添加一个对象
        /// </summary>
        /// <param name="log"></param>
        public void RunningLogQueue_AddOne(LogItem log)
        {
            RunningLogQueue.Enqueue(log);
        }

        /// <summary>
        /// 从UI日志队列中取出一个对象
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool RunningLogQueue_GetOne(out LogItem log)
        {
            return RunningLogQueue.TryDequeue(out log);
        }

        /// <summary>
        /// 判断UI日志队列中是否有对象
        /// </summary>
        /// <returns></returns>
        public bool RunningLogQueue_HaveElement()
        {
            if (RunningLogQueue.Count <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
