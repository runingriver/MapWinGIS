using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapWinGIS.MainProgram
{
    public partial class PluginsForm : Form
    {
        private System.Resources.ResourceManager resources = 
            new System.Resources.ResourceManager("MapWinGIS.MainProgram.GlobalResource", System.Reflection.Assembly.GetExecutingAssembly());

        private string pluginfo;
        private Hashtable LookupTable;//key-index，value-PluginInfo.Key
        private bool[] CheckedList;
        //构造函数
        public PluginsForm()
        {
            InitializeComponent();
            this.Icon = MapWinGIS.MainProgram.Properties.Resources.MapWinGIS;
            LookupTable = new Hashtable();

        }

        private void DisplayPluginInfo(int pluginIndex)
        {
            PluginInfo plugin ;
            string key = (string)LookupTable[pluginIndex];
            plugin = (PluginInfo)(Program.frmMain.m_PluginManager.PluginsList[key]);
            pluginfo = "";
            DisplayPluginProperty(resources.GetString("PluginPropertyName_Text"), plugin.Name);
            DisplayPluginProperty(resources.GetString("PluginPropertyVersion_Text"), plugin.Version);
            DisplayPluginProperty(resources.GetString("PluginPropertyBuildDate_Text"), plugin.BuildDate);
            DisplayPluginProperty(resources.GetString("PluginPropertyAuthor_Text"), plugin.Author);
            DisplayPluginProperty(resources.GetString("PluginPropertyDescription_Text"), plugin.Description);

            lblPluginInfo.Text = pluginfo;
            plugin = null;
        }

        private void DisplayPluginProperty(string PropertyName, string PropertyValue)
        {
            //格式需要调整
            pluginfo = pluginfo + PropertyName + "  " + PropertyValue + "\r\n";
        }

        //LoadForm()方法暂时保留，若后期没有调用，则将LoadForm和LoadListBox一起删除
        public void LoadForm()
        {

            lblPluginInfo.Text = "";
            LoadListBox();

            if (lstPlugins.Items.Count > 0)
            {
                lstPlugins.SelectedIndex = 0;
                lstPlugins_SelectedIndexChanged(this, new System.EventArgs());
            }

            cmdApply.Enabled = false;
        }

        private void LoadListBox()
        {
            MapWinGIS.MainProgram.PluginInfo plugin;
            bool p_Checked;
            int p_Index;
            string objStr;

            try
            {
                lstPlugins.CreateControl();
                lstPlugins.Items.Clear();
                LookupTable.Clear();
                if (Program.frmMain.m_PluginManager.PluginsList.Count == 0)
                {
                    return;
                }

                CheckedList = new bool[Program.frmMain.m_PluginManager.PluginsList.Count];

                ArrayList sortedList;
                Hashtable sortMap;
                sortedList = new ArrayList();
                sortMap = new Hashtable();
                foreach (MapWinGIS.MainProgram.PluginInfo pInfo in Program.frmMain.m_PluginManager.PluginsList.Values)
                {
                    objStr = pInfo.Name;
                    sortedList.Add(objStr);
                    sortMap.Add(objStr, pInfo);
                }
                sortedList.Sort();
                foreach (string tempLoopVar_objStr in sortedList)
                {
                    plugin = (PluginInfo)(sortMap[tempLoopVar_objStr]);
                    p_Checked = Program.frmMain.m_PluginManager.ContainsKey(Program.frmMain.m_PluginManager.LoadedPlugins, plugin.Key);
                    p_Index = lstPlugins.Items.Add(plugin.Name, p_Checked);
                    LookupTable.Add(p_Index, plugin.Key);
                    CheckedList[p_Index] = p_Checked;
                }
            }
            catch (System.Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }
 
        }

        #region 窗体事件
        //窗体加载
        private void PluginsForm_Load(object sender, EventArgs e)
        {
            MapWinGIS.MainProgram.PluginInfo plugin;
            bool p_Checked ;
            int p_Index;
            string objStr;

            try
            {
                lstPlugins.CreateControl();
                lstPlugins.Items.Clear();
                LookupTable.Clear();
                if (Program.frmMain.m_PluginManager.PluginsList.Count == 0)
                {
                    return;
                }

                CheckedList = new bool[Program.frmMain.m_PluginManager.PluginsList.Count];

                ArrayList sortedList;
                Hashtable sortMap;
                sortedList = new ArrayList();
                sortMap = new Hashtable();
                foreach (MapWinGIS.MainProgram.PluginInfo pInfo in Program.frmMain.m_PluginManager.PluginsList.Values)
                {
                    objStr = pInfo.Name;
                    sortedList.Add(objStr);
                    sortMap.Add(objStr, pInfo);
                }
                sortedList.Sort();
                foreach (string tempLoopVar_objStr in sortedList)
                {
                    plugin = (PluginInfo)(sortMap[tempLoopVar_objStr]);
                    p_Checked = Program.frmMain.m_PluginManager.ContainsKey(Program.frmMain.m_PluginManager.LoadedPlugins, plugin.Key);
                    p_Index = lstPlugins.Items.Add(plugin.Name, p_Checked);
                    LookupTable.Add(p_Index, plugin.Key);
                    CheckedList[p_Index] = p_Checked;
                }
            }
            catch (System.Exception ex)
            {
                Program.g_error = ex.ToString();
                Program.ShowError(ex);
            }
        }
        //确定按钮
        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            if (cmdApply.Enabled)
            {
                cmdApply_Click(sender, new System.EventArgs());
            }

            Program.frmMain.SynchPluginMenu();
            this.Close();
        }
        //取消按钮
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        //应用按钮
        private void cmdApply_Click(object sender, EventArgs e)
        {
            int i;
            string key;

            for (i = 0; i < lstPlugins.Items.Count; i++)
            {
                key = LookupTable[i].ToString();
                if (CheckedList[i] == true)
                {
                    if (Program.frmMain.m_PluginManager.ContainsKey(Program.frmMain.m_PluginManager.LoadedPlugins, key) == false)
                    {
                        // 加载插件
                        Program.frmMain.m_PluginManager.StartPlugin(key);
                    }
                }
                else
                {
                    if (Program.frmMain.m_PluginManager.ContainsKey(Program.frmMain.m_PluginManager.LoadedPlugins, key) == true)
                    {
                        // 卸载插件
                        Program.frmMain.m_PluginManager.StopPlugin(key);
                    }
                }
            }

            Program.g_SyncPluginMenuDefer = false;
            Program.frmMain.SynchPluginMenu();
            cmdApply.Enabled = false;

            Program.frmMain.SetModified(true);

        }
        //选择索引改变
        private void lstPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.frmMain.m_PluginManager.PluginsList.Count == 0)
            {
                return;
            }
            DisplayPluginInfo(lstPlugins.SelectedIndex);
            if (CheckedList[lstPlugins.SelectedIndex])
            {
                lstPlugins.SetItemChecked(lstPlugins.SelectedIndex, false);
            }
            else
            {
                lstPlugins.SetItemChecked(lstPlugins.SelectedIndex, true);
            }
            cmdApply.Enabled = true;
            Program.frmMain.SetModified(true);

        }
        //选中
        private void lstPlugins_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedList[e.Index] = (e.NewValue == CheckState.Checked ? true : false);
        }
        //刷新按钮
        private void btnRefreshList_Click(object sender, EventArgs e)
        {
            Program.frmMain.m_PluginManager.LoadPlugins();

            //清空所有Item
            lstPlugins.Items.Clear();
            LookupTable.Clear();

            //刷新列表
            MapWinGIS.MainProgram.PluginInfo plugin;
            ArrayList sortedList;
            Hashtable sortMap; //key-Name,Value-PluginInfo
            CheckedList = new bool[Program.frmMain.m_PluginManager.PluginsList.Count];
            //插件排序
            sortedList = new ArrayList();
            sortMap = new Hashtable();

            string objStr;
            //将插件信息添加到集合中
            foreach (MapWinGIS.MainProgram.PluginInfo pInfo in Program.frmMain.m_PluginManager.PluginsList.Values)
            {
                objStr = pInfo.Name;
                sortedList.Add(objStr);
                sortMap.Add(objStr, pInfo);
            }
            sortedList.Sort();

            bool p_Checked;
            int p_Index;
            //将插件添加到LookupTable中，并设置checked状态
            foreach (string tempLoopVar_objStr in sortedList)
            {
                plugin = (PluginInfo)(sortMap[tempLoopVar_objStr]);
                p_Checked = Program.frmMain.m_PluginManager.ContainsKey(Program.frmMain.m_PluginManager.LoadedPlugins, plugin.Key);
                p_Index = lstPlugins.Items.Add(plugin.Name, p_Checked);
                LookupTable.Add(p_Index, plugin.Key);
                CheckedList[p_Index] = p_Checked;
            }
        }
        //反选按钮
        private void btnTurnAllOff_Click(object sender, EventArgs e)
        {
            cmdApply.Enabled = true; //卸载插件
            for (int i = 0; i < lstPlugins.Items.Count; i++)
            {
                lstPlugins.SetItemChecked(i, false);
                CheckedList[i] = false;
            }
        }
        //全选按钮
        private void btnTurnAllOn_Click(object sender, EventArgs e)
        {
            cmdApply.Enabled = true;//加载插件
            for (int i = 0; i < lstPlugins.Items.Count; i++)
            {
                lstPlugins.SetItemChecked(i, true);
                CheckedList[i] = true;
            }
        }

        #endregion

    }
}
