using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemSet
{
    public partial class SystemSetFrom : Form
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">文件名称</param>
        /// <param name="path1">文件夹路径</param>
        public SystemSetFrom(string path,string path1)
        {
            InitializeComponent();
            ssb = SystemSetBll.CreateSsb(path, path1);
            Init();
        }
        SystemSetBll ssb;
        private void SystemSetFrom_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 初始化界面
        /// </summary>
        private void Init()
        {
            dgv_SystemSet.Columns["Column1"].ReadOnly = true;
            //判断字典是否为null
            if (ssb.CurrentCDSys().Count != 0)
            {
                foreach (var item in ssb.CurrentCDSys())
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell tbx_name = new DataGridViewTextBoxCell();
                    tbx_name.Value = item.Key;//名称
                    row.Cells.Add(tbx_name);

                    DataGridViewTextBoxCell tbx_value = new DataGridViewTextBoxCell();
                    tbx_value.Value = item.Value.Value;//数值
                    row.Cells.Add(tbx_value);

                    DataGridViewTextBoxCell tbx_port = new DataGridViewTextBoxCell();
                    tbx_port.Value = item.Value.Marking;//数值 
                    row.Cells.Add(tbx_port);

                    this.dgv_SystemSet.AllowUserToAddRows = false;
                    this.dgv_SystemSet.Rows.Add(row);
                }
                //判断当前字典是否和枚举里的数量一致
                if (ssb.CurrentCDSys().Count != ssb.EnumToList<PNameSystem>().Count)
                {
                    List<PNameSystem> list = ssb.TEnumToList<PNameSystem>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (!ssb.CurrentCDSys().ContainsKey(list[i]))
                        {
                            DataGridViewRow row = new DataGridViewRow();
                            DataGridViewTextBoxCell tbx_name = new DataGridViewTextBoxCell();
                            tbx_name.Value = list[i].ToString(); ;//名称
                            row.Cells.Add(tbx_name);

                            DataGridViewTextBoxCell tbx_value = new DataGridViewTextBoxCell();
                            tbx_value.Value = "";//数值
                            row.Cells.Add(tbx_value);

                            DataGridViewTextBoxCell tbx_marking = new DataGridViewTextBoxCell();
                            tbx_marking.Value = "";//数值
                            row.Cells.Add(tbx_marking);

                            this.dgv_SystemSet.Rows.Add(row);
                        }
                    }
                }

            }
            else
            {
                List<string> list = ssb.EnumToList<PNameSystem>();
                for (int i = 0; i < list.Count; i++)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell tbx_name = new DataGridViewTextBoxCell();
                    tbx_name.Value = list[i].ToString(); ;//名称
                    row.Cells.Add(tbx_name);

                    DataGridViewTextBoxCell tbx_value = new DataGridViewTextBoxCell();
                    tbx_value.Value = "";//数值
                    row.Cells.Add(tbx_value);

                    DataGridViewTextBoxCell tbx_marking = new DataGridViewTextBoxCell();
                    tbx_marking.Value = "";//数值 

                    row.Cells.Add(tbx_marking);
                    this.dgv_SystemSet.Rows.Add(row);
                }
            }
        }

        private void btn_SaveSystemSet_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < this.dgv_SystemSet.Rows.Count; i++)
                {
                    if (this.dgv_SystemSet.Rows[i].Cells[0].Value == null)
                    {
                        break;
                    }

                    PNameSystem key = (PNameSystem)Enum.Parse(typeof(PNameSystem), this.dgv_SystemSet.Rows[i].Cells[0].Value.ToString());
                    object value = this.dgv_SystemSet.Rows[i].Cells[1].Value;
                    string desc = Convert.ToString(this.dgv_SystemSet.Rows[i].Cells[2].Value);
                    ssb.SystemSet(key, value, desc);
                }

                MessageBox.Show("参数保存OK");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
