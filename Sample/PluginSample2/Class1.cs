using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapWinGIS.Interfaces;

namespace PluginSample2
{
    public class Class1 : IPlugin
    {
        #region 插件基本信息属性
        /// <summary>
        /// 插件作者
        /// </summary>
        public string Author { get { return "1111huzongzhe"; } }

        /// <summary>
        /// 插件简介
        /// </summary>
        public string Description { get { return "aaawdasdasaaaaaaaaaaa"; } }

        /// <summary>
        /// 插件的连续号码，不再使用，保持向后兼容
        /// </summary>
        public string SerialNumber { get { return "dagdgasdasdasdg"; } }

        /// <summary>
        /// 插件名
        /// </summary>
        public string Name { get { return "PluginSample2"; } }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string BuildDate { get { return "2014-07-16"; } }

        /// <summary>
        /// 插件版本
        /// </summary>
        public string Version { get { return "1.0.0.0"; } }
        #endregion

        #region 插件接口方法
        /// <summary>
        /// 项目加载时由宿主程序调用
        /// </summary>
        /// <param name="projectFile">项目文件名</param>
        /// <param name="settings">插件路径</param>
        public void ProjectLoading(string projectFile, string settings)
        { }

        /// <summary>
        /// 项目保存时由宿主程序调用
        /// </summary>
        /// <param name="projectFile">项目文件名</param>
        /// <param name="settings">保存插件的设置</param>
        public void ProjectSaving(string projectFile, ref string settings)
        { }

        /// <summary>
        /// 插件加载时由宿主程序调用
        /// </summary>
        /// <param name="mapWin">和宿主程序交互</param>
        /// <param name="parenthandle">MainProgram form 的handle</param>
        public void Initialize(IMapWin mapWin, int parenthandle)
        { }

        /// <summary>
        /// 当插件卸载时注销插件，宿主程序调用
        /// </summary>
        public void Terminate()
        { }

        /// <summary>
        /// 双击legend时调用的方法，宿主程序调用
        /// </summary>
        /// <param name="handle">双击的层或组的handle</param>
        /// <param name="location">点击的位置，要么是在层（layer）上要么是在一个组（group）上.</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息</param>
        public void LegendDoubleClick(int handle, ClickLocation location, ref bool handled)
        { }

        /// <summary>
        /// 在legend的上的鼠标点击事件调用的方法，宿主程序调用
        /// </summary>
        /// <param name="handle">鼠标所点击的层或组的handle</param>
        /// <param name="button">按下的鼠标按钮.  可以用Button枚举决定是哪个按钮被按下</param>
        /// <param name="location">点击的位置，要么是在层（layer）上要么是在一个组（group）上.</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        public void LegendMouseDown(int handle, int button, ClickLocation location, ref bool handled)
        { }

        /// <summary>
        /// 在legend上的MouseUp事件调用的方法，宿主程序调用
        /// </summary>
        /// <param name="handle">鼠标所点击的层或组的handle</param>
        /// <param name="button">MouseUP的鼠标按钮.  可以用Button枚举决定是哪个按钮被按下</param>
        /// <param name="location">点击的位置，要么是在层（layer）上要么是在一个组（group）上.</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        public void LegendMouseUp(int handle, int button, ClickLocation location, ref bool handled)
        { }

        /// <summary>
        /// 当地图的范围（extents）发生改变时调用的方法，宿主程序调用
        /// </summary>
        public void MapExtentsChanged()
        { }

        /// <summary>
        /// 鼠标在地图上按下的时候的方法，宿主程序调用
        /// </summary>
        /// <param name="button">MouseDown的鼠标按钮.  可以用Button枚举决定是哪个按钮被按下</param>
        /// <param name="shift">代表ctrl、alt、shift等按钮状态</param>
        /// <param name="x">x坐标点（像素）</param>
        /// <param name="y">y坐标点（像素）</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        public void MapMouseDown(int button, int shift, int x, int y, ref bool handled)
        { }

        /// <summary>
        /// 当鼠标在地图上over的方法，宿主程序调用
        /// </summary>
        /// <param name="screenX">x坐标点（像素）</param>
        /// <param name="screenY">y坐标点（像素）</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        public void MapMouseMove(int screenX, int screenY, ref bool handled)
        { }

        /// <summary>
        /// MouseUP事件发生调用的方法，由宿主程序调用.
        /// </summary>
        /// <param name="button">MouseUP的鼠标按钮.  可以用Button枚举决定是哪个按钮被按下</param>
        /// <param name="shift">代表ctrl、alt、shift等按钮状态</param>
        /// <param name="x">x坐标点（像素）</param>
        /// <param name="y">y坐标点（像素）</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        public void MapMouseUp(int button, int shift, int x, int y, ref bool handled)
        { }

        /// <summary>
        /// 在地图上的拖拽操作的方法，宿主程序调用
        /// </summary>
        /// <param name="bounds">选择的区域, 像素为坐标.</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        public void MapDragFinished(System.Drawing.Rectangle bounds, ref bool handled)
        { }

        /// <summary>
        /// 当一个或多个层（layer）被添加的方法，宿主程序调用
        /// </summary>
        /// <param name="layers">存储添加的layer的数组</param>
        public void LayersAdded(Layer[] layers)
        { }

        /// <summary>
        /// 当一个层从map中移除时的方法，宿主调用
        /// </summary>
        /// <param name="handle">要移除的层的handle</param>
        public void LayerRemoved(int handle)
        { }

        /// <summary>
        /// 当一个层被选中时的方法，宿主调用
        /// </summary>
        /// <param name="handle">选择的layer的handle</param>
        public void LayerSelected(int handle)
        { }

        /// <summary>
        /// 当所有的层都被清空后的方法，宿主调用
        /// </summary>
        public void LayersCleared()
        { }

        /// <summary>
        /// 用户选择shapes后方法，宿主调用
        /// </summary>
        /// <param name="handle">选择的shapefile类型的layer的handle</param>
        /// <param name="selectInfo">SelectInfo包含了选择的shapes的信息</param>
        public void ShapesSelected(int handle, SelectInfo selectInfo)
        { }

        /// <summary>
        /// 当工具条或菜单项被点击的事件，宿主调用
        /// </summary>
        /// <param name="itemName">被点击项的名字</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        public void ItemClicked(string itemName, ref bool handled)
        { }

        /// <summary>
        /// 当一个插件广播一个消息，有宿主程序转播 
        /// 消息可以在两个插件之间发送
        /// </summary>
        /// <param name="msg">被send的消息</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        public void Message(string msg, ref bool handled)
        { }



        #endregion

    }
}
