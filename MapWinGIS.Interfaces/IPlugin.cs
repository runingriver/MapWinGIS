using System;

//(MapWindow全部替换为MapWinGIS)
namespace MapWinGIS.Interfaces
{
    /// <summary>
    /// 基接口
    /// 所有插件必须实现此接口{6061F5C3-4157-4571-9A43-469BE2E25631}
    /// </summary>
    [System.Runtime.InteropServices.Guid("6061F5C3-4157-4571-9A43-469BE2E25631")]
    public interface IPlugin
    {
        #region 插件基本信息属性
        /// <summary>
        /// 插件作者
        /// </summary>
        string Author { get; }

        /// <summary>
        /// 插件简介
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 插件的连续号码，不再使用，保持向后兼容
        /// </summary>
        string SerialNumber { get; }

        /// <summary>
        /// 插件名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        string BuildDate { get; }

        /// <summary>
        /// 插件版本
        /// </summary>
        string Version { get; }
        #endregion

        #region 插件接口方法
        /// <summary>
        /// 项目加载时由宿主程序调用
        /// </summary>
        /// <param name="projectFile">项目文件名</param>
        /// <param name="settings">从项目文件中提取的与该插件相关的字符串（项目文件中保存了插件的信息）</param>
        void ProjectLoading(string projectFile, string settings);

        /// <summary>
        /// 项目保存时由宿主程序调用
        /// </summary>
        /// <param name="projectFile">项目文件名</param>
        /// <param name="settings">与该插件相关的字符串保存到项目文件中（项目文件中保存了插件的信息）</param>
        void ProjectSaving(string projectFile, ref string settings);

        /// <summary>
        /// 插件加载时由宿主程序调用
        /// </summary>
        /// <param name="mapWin">和宿主程序交互</param>
        /// <param name="parenthandle">MainProgram form 的handle</param>
        void Initialize(IMapWin mapWin, int parenthandle);

        /// <summary>
        /// 当插件卸载时注销插件，宿主程序调用
        /// </summary>
        void Terminate();

        /// <summary>
        /// 双击legend时调用的方法，宿主程序调用
        /// </summary>
        /// <param name="handle">双击的层或组的handle</param>
        /// <param name="location">点击的位置，要么是在层（layer）上要么是在一个组（group）上.</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息</param>
        void LegendDoubleClick(int handle, ClickLocation location, ref bool handled);

        /// <summary>
        /// 在legend的上的鼠标点击事件调用的方法，宿主程序调用
        /// </summary>
        /// <param name="handle">鼠标所点击的层或组的handle</param>
        /// <param name="button">按下的鼠标按钮.  可以用Button枚举决定是哪个按钮被按下</param>
        /// <param name="location">点击的位置，要么是在层（layer）上要么是在一个组（group）上.</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        void LegendMouseDown(int handle, int button, ClickLocation location, ref bool handled);

        /// <summary>
        /// 在legend上的MouseUp事件调用的方法，宿主程序调用
        /// </summary>
        /// <param name="handle">鼠标所点击的层或组的handle</param>
        /// <param name="button">MouseUP的鼠标按钮.  可以用Button枚举决定是哪个按钮被按下</param>
        /// <param name="location">点击的位置，要么是在层（layer）上要么是在一个组（group）上.</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        void LegendMouseUp(int handle, int button, ClickLocation location, ref bool handled);

        /// <summary>
        /// 当地图的范围（extents）发生改变时调用的方法，宿主程序调用
        /// </summary>
        void MapExtentsChanged();

        /// <summary>
        /// 鼠标在地图上按下的时候的方法，宿主程序调用
        /// </summary>
        /// <param name="button">MouseDown的鼠标按钮.  可以用Button枚举决定是哪个按钮被按下</param>
        /// <param name="shift">代表ctrl、alt、shift等按钮状态</param>
        /// <param name="x">x坐标点（像素）</param>
        /// <param name="y">y坐标点（像素）</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        void MapMouseDown(int button, int shift, int x, int y, ref bool handled);

        /// <summary>
        /// 当鼠标在地图上over的方法，宿主程序调用
        /// </summary>
        /// <param name="screenX">x坐标点（像素）</param>
        /// <param name="screenY">y坐标点（像素）</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        void MapMouseMove(int screenX, int screenY, ref bool handled);

        /// <summary>
        /// MouseUP事件发生调用的方法，由宿主程序调用.
        /// </summary>
        /// <param name="button">MouseUP的鼠标按钮.  可以用Button枚举决定是哪个按钮被按下</param>
        /// <param name="shift">代表ctrl、alt、shift等按钮状态</param>
        /// <param name="x">x坐标点（像素）</param>
        /// <param name="y">y坐标点（像素）</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        void MapMouseUp(int button, int shift, int x, int y, ref bool handled);

        /// <summary>
        /// 在地图上的拖拽操作的方法，宿主程序调用
        /// </summary>
        /// <param name="bounds">选择的区域, 像素为坐标.</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        void MapDragFinished(System.Drawing.Rectangle bounds, ref bool handled);

        /// <summary>
        /// 当一个或多个层（layer）被添加的方法，宿主程序调用
        /// </summary>
        /// <param name="layers">存储添加的layer的数组</param>
        void LayersAdded(Layer[] layers);

        /// <summary>
        /// 当一个层从map中移除时的方法，宿主调用
        /// </summary>
        /// <param name="handle">要移除的层的handle</param>
        void LayerRemoved(int handle);

        /// <summary>
        /// 当一个层被选中时的方法，宿主调用
        /// </summary>
        /// <param name="handle">选择的layer的handle</param>
        void LayerSelected(int handle);

        /// <summary>
        /// 当所有的层都被清空后的方法，宿主调用
        /// </summary>
        void LayersCleared();

        /// <summary>
        /// 用户选择shapes后方法，宿主调用
        /// </summary>
        /// <param name="handle">选择的shapefile类型的layer的handle</param>
        /// <param name="selectInfo">SelectInfo包含了选择的shapes的信息</param>
        void ShapesSelected(int handle, SelectInfo selectInfo);

        /// <summary>
        /// 当工具条或菜单项被点击的事件，宿主调用
        /// </summary>
        /// <param name="itemName">被点击项的名字</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        void ItemClicked(string itemName, ref bool handled);

        /// <summary>
        /// 当一个插件广播一个消息，有宿主程序转播 
        /// 消息可以在多个插件之间发送
        /// 若要实现多语言，则接受"Language:[区域代号]"格式的消息，并置handled为false
        /// 并记得将编译后的语言资源文件复制到响应目录
        /// </summary>
        /// <param name="msg">被send的消息</param>
        /// <param name="handled">若设为true，则表示除这个插件外的插件都不能接收这个消息.</param>
        void Message(string msg, ref bool handled);



        #endregion
    }
}
