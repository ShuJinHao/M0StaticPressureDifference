using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SystemSet
{
    public class SystemSetBll
    {
        /// <summary>
        /// 系统参数对象
        /// </summary>
        private ConcurrentDictionary<PNameSystem, PItem> CDSys;
        /// <summary>
        /// 保存路径
        /// </summary>
        string path_System = String.Empty;
        /// <summary>
        /// 保存的文件夹路径
        /// </summary>
        string path_folder= String.Empty;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">文件名称</param>
        /// <param name="path1">文件夹路径</param>
        private SystemSetBll(string path,string path1)
        {
            CDSys = new ConcurrentDictionary<PNameSystem, PItem>();
            path_System = path+ "\\"+ path1;
            path_folder = path;
            LoadSystem();
        }
        private static SystemSetBll ssbList;
        /// <summary>
        /// 单例模式
        /// </summary>
        /// <param name="path">文件名称</param>
        /// <param name="path1">文件夹路径</param>
        /// <returns></returns>
        public static SystemSetBll CreateSsb(string path,string path1)
        {
            if (ssbList == null)
            {
                ssbList = new SystemSetBll(path, path1);
            }
            return ssbList;
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="desc"></param>
        public void SystemSet(PNameSystem key, object value, string desc)
        {
            if (CDSys.Keys.Contains(key))
            {
                CDSys[key].Value = value;
                CDSys[key].Marking = desc;
            }
            else
            {
                PItem pitem = new PItem()
                {
                    Value = value,
                    Marking = desc
                };
                CDSys.TryAdd(key, pitem);
            }
            SaveSystem();
        }
        /// <summary>
        /// 返回枚举的string集合
        /// </summary>
        /// <typeparam name="PNameSystem"></typeparam>
        /// <returns></returns>
        public List<string> EnumToList<PNameSystem>()
        {
            List<string> list = new List<string>();
            foreach (var item in Enum.GetValues(typeof(PNameSystem)))
            {
                list.Add(item.ToString());
            }
            return list;
        }
        /// <summary>
        /// 返回当前的系统对象
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<PNameSystem, PItem> CurrentCDSys()
        {
            return CDSys;
        }
        /// <summary>
        /// 保存系统对象
        /// </summary>
        private void SaveSystem()
        {
            string js = Newtonsoft.Json.JsonConvert.SerializeObject(CDSys);
            System.IO.File.WriteAllText(path_System, js);
        }
        /// <summary>
        /// 加载系统对象
        /// </summary>
        private void LoadSystem()
        {
            if (!Directory.Exists(path_folder))
            {
                Directory.CreateDirectory(path_folder);
            }
            if (File.Exists(path_System))
            {
                string js = System.IO.File.ReadAllText(path_System);
                CDSys = Newtonsoft.Json.JsonConvert.DeserializeObject<ConcurrentDictionary<PNameSystem, PItem>>(js);
            }
            else
            {

            }
        }
        /// <summary>
        /// 返回枚举集合
        /// </summary>
        /// <typeparam name="PNameSystem"></typeparam>
        /// <returns></returns>
        public List<T> TEnumToList<T>()
        {
            List<T> list = new List<T>();
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                list.Add((T)item);
            }
            return list;
        }
        /// <summary>
        /// 返回参数内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public T GetValue<T>(PNameSystem paramName)
        {

            if (CDSys.ContainsKey(paramName))
            {
                return (T)Convert.ChangeType(CDSys[paramName].Value, typeof(T));
            }
            else
            {
                return default(T);
            }
        }
    }
}
