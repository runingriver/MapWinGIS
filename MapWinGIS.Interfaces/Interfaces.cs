using System;
using System.Collections.Generic;
//(MapWindow全部替换为MapWinGIS)
namespace MapWinGIS.Interfaces
{
    /// <summary>
    /// panel关闭，委托
    /// </summary>
    /// <param name="caption">panel的名字</param>
    public delegate void OnPanelClose(string caption);

    #region 枚举常量
    /// <summary>
    /// 兼容按钮常量
    /// </summary>
    public enum Buttons
    {
        /// <summary>鼠标左键</summary>
        Left = 1,
        /// <summary>鼠标右键</summary>
        Right = 2,
        /// <summary>鼠标中间建</summary>
        Middle = 3
    }

    /// <summary>
    /// Defines the operation that will be used to update the existing selection
    /// 定义操作，可被用作更新那些存在的选项
    /// </summary>
    public enum SymbologyBehavior
    {
        RandomOptions = 0,      // 设置被ocx使用的选项
        DefaultOptions = 1,     // 从 .mwsymb or .mwsr 文件加载的选项
        UserPrompting = 2,      // 从列表中选择选项
    }

    /// <summary>
    /// 当加载的层的投影与之前的层的投影不匹配时，定义的行为的类型
    /// </summary>
    public enum ProjectionMismatchBehavior
    {
        IgnoreMismatch = 0,
        Reproject = 1,
        SkipFile = 2,
    }

    /// <summary>
    /// 当层没有添加任何投影，但是项目有投影时，定义的行为的类型
    /// </summary>
    public enum ProjectionAbsenceBehavior
    {
        AssignFromProject = 0,
        IgnoreAbsence = 1,
        SkipFile = 2,
    }

    /// <summary>
    /// geoprocessing工具箱的图标
    /// Defines indices of icons used inside geoprocessing toobox
    /// </summary>
    public enum ToolboxIcons
    {
        FolderClose = 0,
        FolderOpen = 1,
        Tool = 2,
    }

    /// <summary>
    /// 定义处理选择shape的模式
    /// </summary>
    public enum SelectionOperation
    {
        /// <summary>
        /// 原来选择的将丢失
        /// </summary>
        SelectNew = 0,          // old selection will be lost
        /// <summary>
        /// 新选择的shape将被添加到原来的选择集中
        /// </summary>
        SelectAdd = 1,          // new shapes will be added to the old selection
        /// <summary>
        /// 新选择的shapes将会在原来选择集中去掉
        /// </summary>
        SelectExclude = 2,      // new shapes will be excluded from old selection
        /// <summary>
        /// 新选择的shapes将会在原来选择集中的中倒置
        /// </summary>
        SelectInvert = 3        // new shapes will be inverted in case they are in the existing selection
    }

    /// <summary>
    /// 列举支持的层类型
    /// </summary>
    public enum eLayerType
    {
        /// <summary>无效层类型</summary>
        Invalid = -1,
        /// <summary>Image layer</summary>
        Image = 0,
        /// <summary>Point shapefile layer</summary>
        PointShapefile = 1,
        /// <summary>Line shapefile layer</summary>
        LineShapefile = 2,
        /// <summary>Polygon（多边形） shapefile layer</summary>
        PolygonShapefile = 3,
        /// <summary>Grid（格子） layer</summary>
        Grid = 4,
        /// <summary>Tiles（瓦片） layers</summary>
        Tiles = 5,
    }

    /// <summary>
    /// 枚举可能的preview map更新的类型
    /// </summary>
    public enum ePreviewUpdateExtents
    {
        /// <summary>
        /// Update using full exents.
        /// </summary>
        FullExtents = 0,
        /// <summary>
        /// Update using current map view.
        /// </summary>
        CurrentMapView = 1
    }

    /// <summary>
    /// Location of a click event within the legend.
    /// 定位在legend内 的点击事件
    /// </summary>
    public enum ClickLocation
    {
        /// <summary>用户在legend的空白处点击（不在层和组上）</summary>
        None = 0,
        /// <summary>用户在legend的层上点击</summary>
        Layer = 1,
        /// <summary>用户在legend的组上点击</summary>
        Group = 2
    }

    /// <summary>
    /// Scalebar支持的单位
    /// Supported units of measure when requesting a Scalebar
    /// </summary>
    public enum UnitOfMeasure
    {
        /// <summary>The units are in decimal degrees（小数位）.</summary>
        DecimalDegrees,
        /// <summary>The units are in millimeters（毫米）.</summary>
        Millimeters,
        /// <summary>The units are in centimeters（厘米）.</summary>
        Centimeters,
        /// <summary>The units are in inches（英寸）.</summary>
        Inches,
        /// <summary>The units are in feet（尺）.</summary>
        Feet,
        /// <summary>The units are in Yards（码）.</summary>
        Yards,
        /// <summary>The units are in meters（米）.</summary>
        Meters,
        /// <summary>The units are in miles（英里）.</summary>
        Miles,
        /// <summary>The units are in kilometers（千米，公里）.</summary>
        Kilometers,
        /// <summary>The units are in nautical miles（海里）.</summary>
        NauticalMiles,
        /// <summary>The units are in acres（英亩）.</summary>
        Acres,
        /// <summary>The units are in hectares（公顷）.</summary>
        Hectares,
        /// <summary>The units are unknown（未知）.</summary>
        Unknown
    }

    /// <summary>
    /// 文本对齐方式
    /// </summary>
    public enum eAlignment
    {
        /// <summary>Left align.</summary>
        Left = 0,
        /// <summary>Right align.</summary>
        Right = 1,
        /// <summary>Center align.</summary>
        Center = 2
    }

    /// <summary>
    /// 在宿主程序的上的停靠方式
    /// MapWindowDockStyle改为MapWinGISDockStyle(MapWindow全部替换为MapWinGIS)
    /// Docking styles for MapWinGIS UIPanels
    /// </summary>
    public enum MapWinGISDockStyle
    {
        /// <summary>Floating</summary>
        None = -1,
        /// <summary>Dock Left</summary>
        Left = 0,
        /// <summary>Dock Right</summary>
        Right = 1,
        /// <summary>Dock Top</summary>
        Top = 2,
        /// <summary>Dock Bottom</summary>
        Bottom = 3,
        /// <summary>Dock Left Autohidden</summary>
        LeftAutoHide = 4,
        /// <summary>Dock Right Autohidden</summary>
        RightAutoHide = 5,
        /// <summary>Dock Top Autohidden</summary>
        TopAutoHide = 6,
        /// <summary>Dock Bottom Autohidden</summary>
        BottomAutoHide = 7
    }

    #endregion

    #region 接口
    /// <summary>
    /// 接口，可编辑下拉列表框
    /// 添加到工具条
    /// </summary>
    public interface ComboBoxItem
    {
        /// <summary>
        /// 鼠标
        /// </summary>
        System.Windows.Forms.Cursor Cursor { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 下拉列表框的下拉的样式
        /// </summary>
        System.Windows.Forms.ComboBoxStyle DropDownStyle { get; set; }

        /// <summary>
        /// 获取或设置下拉列表框是否启用
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 返回下拉列表框的子项的集合
        /// </summary>
        System.Windows.Forms.ComboBox.ObjectCollection Items();

        /// <summary>
        /// 下拉列表框的名字
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 选择的以零开始的索引的条目
        /// </summary>
        int SelectedIndex { get; set; }

        /// <summary>
        /// 选择的对象
        /// </summary>
        object SelectedItem { get; set; }

        /// <summary>
        /// 选择的文本
        /// </summary>
        string SelectedText { get; set; }

        /// <summary>
        /// 选择的高亮文本的长度
        /// </summary>
        int SelectionLength { get; set; }

        /// <summary>
        /// 高亮文本的开始索引
        /// </summary>
        int SelectionStart { get; set; }

        /// <summary>
        /// 这个对象的文本
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// 提示文本
        /// </summary>
        string Tooltip { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        int Width { get; set; }
    }

    /// <summary>
    /// 接口，工具条按钮
    /// 当按钮添加到工具条上返回的对象  这个对象能够被用作操纵改变属性
    /// </summary>
    public interface ToolbarButton
    {
        /// <summary>
        /// 是否按下
        /// </summary>
        bool Pressed { get; set; }

        /// <summary>
        /// 按钮的文本
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// 按钮图片
        /// </summary>
        object Picture { get; set; }

        /// <summary>
        /// 按钮类别
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// 按钮提示文本
        /// </summary>
        string Tooltip { get; set; }

        /// <summary>
        /// 是否在前面加一竖线，分组
        /// </summary>
        bool BeginsGroup { get; set; }

        /// <summary>
        /// 鼠标
        /// </summary>
        System.Windows.Forms.Cursor Cursor { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        bool Displayed { get; set; }

        /// <summary>
        /// Gets/Sets the enabled state
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 按钮名字
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 按钮的可见性
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// 子条目的编号
        /// </summary>
        int NumSubItems { get; }

        /// <summary>
        /// 指定的以零开始的索引的子条目
        /// 返回指定索引的工具条按钮，null表示索引越界
        /// </summary>
        Interfaces.ToolbarButton SubItem(int index);

        /// <summary>
        /// 返回指定Name的工具条按钮，null表示不存在
        /// </summary>
        Interfaces.ToolbarButton SubItem(string name);
    }

    /// <summary>
    /// 接口,工具条
    /// </summary>
    public interface Toolbar
    {
        /// <summary>
        /// 添加一个工具条到主工具条
        /// Adds a Toolbar group to the Main Toolbar
        /// </summary>
        /// <param name="name">工具条的名字</param>
        /// <returns>是否添加成功</returns>
        bool AddToolbar(string name);

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">指定新添加的工具条按钮的名字</param>
        Interfaces.ToolbarButton AddButton(string name);

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">指定的新添加的工具条按钮的名字</param>
        /// <param name="isDropDown">按钮是否支持DropDown</param>
        Interfaces.ToolbarButton AddButton(string name, bool isDropDown);

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">指定的新添加的工具条按钮的名字</param>
        /// <param name="toolbar">按钮所属于的工具条的名字  (if null or empty, then the default Toolbar will be used</param>
        /// <param name="isDropDown">按钮是否支持DropDown</param>
        Interfaces.ToolbarButton AddButton(string name, string toolbar, bool isDropDown);

        /// <summary>
        /// 在可下拉的按钮中的条目中添加一个分隔符
        /// </summary>
        /// <param name="name">指定的新添加的工具条按钮的名字</param>
        /// <param name="toolbar">按钮所依附的工具条的名字</param>
        /// <param name="parentButton">工具条按钮的名字 ，以子条目的方式添加分割线</param>
        void AddButtonDropDownSeparator(string name, string toolbar, string parentButton);

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">给工具条按钮的名字</param>
        /// <param name="picture">一图片的方式改变按钮的显示样式</param>
        Interfaces.ToolbarButton AddButton(string name, object picture);

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">给工具条按钮的名字</param>
        /// <param name="picture">按钮的图片样式</param>
        /// <param name="text">可以显示在toolbar上名字</param>
        Interfaces.ToolbarButton AddButton(string name, object picture, string text);

        /// <summary>
        /// 添加一个按钮到指定的默认人工具条上
        /// </summary>
        /// <param name="name">给工具条按钮的名字</param>
        /// <param name="after">新添加的按钮的后面按钮的名字</param>
        /// <param name="parentButton">以子按钮的形式添加，父按钮的名字</param>
        /// <param name="toolbar">工具条的名字 (if null or empty, then the default Toolbar will be used</param>
        Interfaces.ToolbarButton AddButton(string name, string toolbar, string parentButton, string after);

        /// <summary>
        /// 添加一个下拉列表框插件到工具条上
        /// </summary>
        /// <param name="name">新建下拉列表框的名字</param>
        /// <param name="after">新的下拉列表框后面的插件的名字</param>
        /// <param name="toolbar">这个插件所依附的工具条的名字</param>
        Interfaces.ComboBoxItem AddComboBox(string name, string toolbar, string after);

        /// <summary>
        /// 返回指定工具条按钮 (null on failure)
        /// </summary>
        /// <param name="name">要寻找的工具条名字</param>
        Interfaces.ToolbarButton ButtonItem(string name);

        /// <summary>
        /// 返回指定的下拉列表框
        /// </summary>
        /// <param name="name">要查找条目的名字</param>
        Interfaces.ComboBoxItem ComboBoxItem(string name);

        /// <summary>
        /// 移除工具条，以及工具条上所有的按钮的下拉列表框
        /// </summary>
        /// <param name="name">要移除的工具条的名字</param>
        /// <returns>是否成功</returns>
        bool RemoveToolbar(string name);

        /// <summary>
        /// 移除所有已经加载的工具条
        /// </summary>
        /// <returns>是否成功</returns>
        bool RemoveAllToolbars();

        /// <summary>
        /// 获取所有的当前已经加载的工具条名字
        /// 存在泛型集合IList中
        /// </summary>
        /// <returns>返回名字列表</returns>
        System.Collections.Generic.IList<string> ToolbarNames();

        /// <summary>
        /// 移除指定的按钮
        /// </summary>
        /// <param name="name">要移除的按钮的名字</param>
        /// <returns>是否成功</returns>
        bool RemoveButton(string name);

        /// <summary>
        /// 移除指定的下拉列表框
        /// </summary>
        /// <param name="name">要移除的下拉列表框的名字</param>
        /// <returns>是否成功</returns>
        bool RemoveComboBox(string name);

        /// <summary>
        /// 返回指定工具条上按钮的数量
        /// </summary>
        /// <param name="toolbarName">工具条的名字</param>
        /// <returns>0代表工具条没有找到</returns>
        int NumToolbarButtons(string toolbarName);

        /// <summary>
        /// 是否按下一个指定的工具条按钮
        /// </summary>
        /// <param name="name">被按下的按钮的名字</param>
        /// <returns>true，按下，false，没按</returns>
        bool PressToolbarButton(string name);

        /// <summary>
        /// 是否按下一个指定的工具条按钮
        /// </summary>
        /// <param name="toolbarName">工具条的名字</param>
        /// <param name="buttonName">被按下的按钮的名字</param>
        /// <returns>true，成功 false，失败</returns>
        bool PressToolbarButton(string toolbarName, string buttonName);
    }

    /// <summary>
    /// 宿主程序底部的状态栏
    /// </summary>
    public interface StatusBar
    {
        /// <summary>
        /// 是否可用
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 状态栏的进展条是否显示
        /// </summary>
        bool ShowProgressBar { get; set; }

        /// <summary>
        /// 进度条的值
        /// </summary>
        int ProgressBarValue { get; set; }

        /// <summary>
        /// 添加一个panel到状态栏，但是这个功能被去掉了。
        /// 可以用AddPanel（）覆盖
        /// </summary>
        /// <returns>返回添加的panel条目</returns>
        Interfaces.StatusBarItem AddPanel();

        /// <summary>
        /// 添加一个panel到状态栏，但是这个功能被去掉了。
        /// 可以用AddPanel（）覆盖
        /// </summary>
        /// <param name="insertAt">插入位置</param>
        /// <returns> 被添加的panel</returns>
        Interfaces.StatusBarItem AddPanel(int insertAt);

        /// <summary>
        /// 添加一个panel的首选方法
        /// </summary>
        /// <param name="text">显示在panel上的文字</param>
        /// <param name="position">插入位置</param>
        /// <param name="width">宽度</param>
        /// <param name="autoSize">是否AutoSize</param>
        /// <returns>StatusBarPanel对象.</returns>
        System.Windows.Forms.StatusBarPanel AddPanel(string text, int position, int width, System.Windows.Forms.StatusBarPanelAutoSize autoSize);

        /// <summary>
        /// 按照索引的方式移除panel，但是必须存在一个panel
        /// </summary>
        /// <param name="index">以零为索引</param>
        void RemovePanel(int index);

        /// <summary>
        /// 以指定panel对象的方式移除.  但是必须存在一个panel
        /// </summary>
        /// <param name="panel"><c>StatusBarPanel</c> to remove.</param>
        void RemovePanel(ref System.Windows.Forms.StatusBarPanel panel);

        /// <summary>
        /// 移除指定的状态栏中的条目
        /// </summary>
        void RemovePanel(ref Interfaces.StatusBarItem panel);

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">Index of the StatusBarItem to retrieve</param>
        Interfaces.StatusBarItem this[int index] { get; }

        /// <summary>
        /// 状态栏上panel的数量
        /// </summary>
        /// <returns>Number of panels in the <c>StatusBar</c>.</returns>
        int NumPanels { get; }

        /// <summary>
        /// 使得进度条合适的显示在状态栏中.
        /// 在任何你要改变状态栏傻瓜任何panel尺寸的地方调用.
        /// 但是不需要再AddPanel和RemovePanel的地方调用.
        /// </summary>
        void ResizeProgressBar();

        /// <summary>
        /// 刷新状态栏
        /// </summary>
        void Refresh();
    }

    /// <summary>
    /// 状态栏的子项
    /// Statuslabel、ProgressBar、DropDownButton
    /// </summary>
    public interface StatusBarItem
    {
        /// <summary>
        /// 文本对齐
        /// </summary>
        Interfaces.eAlignment Alignment { get; set; }

        /// <summary>
        /// 自动调整大小
        /// </summary>
        bool AutoSize { get; set; }

        /// <summary>
        /// 设置最小宽度
        /// </summary>
        int MinWidth { get; set; }

        /// <summary>
        /// 子条目的文本
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// 设置宽度
        /// </summary>
        int Width { get; set; }
    }

    /// <summary>
    /// 接口，概览地图
    /// 可操纵鸟瞰地图 PreviewMap
    /// </summary>
    public interface PreviewMap
    {
        /// <summary>
        /// 背景颜色
        /// </summary>
        System.Drawing.Color BackColor { get; set; }

        /// <summary>
        /// 显示的图片
        /// </summary>
        System.Drawing.Image Picture { get; set; }

        /// <summary>
        /// LocatorBox的颜色
        /// </summary>
        System.Drawing.Color LocatorBoxColor { get; set; }

        /// <summary>
        /// 按照传过来的地图重新显示地图
        /// </summary>
        void GetPictureFromMap();

        /// <summary>
        /// 让PreviewMap重绘地图，根据主地图中当前范围传过来的数据 (current extents).
        /// </summary>
        void Update();

        /// <summary>
        /// 更新自己的显示的地图.
        /// <param name="updateExtents">从当前范围还是从全图范围更新Preview的视图</param>
        /// </summary>
        void Update(Interfaces.ePreviewUpdateExtents updateExtents);

        /// <summary>
        /// 从指定的目录加载一张图片到PreviewMap中
        /// </summary>
        /// <param name="filename">图片的路径</param>
        /// <returns>true 加载成功，false，失败</returns>
        bool GetPictureFromFile(string filename);

        /// <summary>
        /// 关闭预览地图控件
        /// </summary>
        void Close();

        /// <summary>
        /// 浮动停靠预览地图控件
        /// </summary>
        /// <param name="dockStyle"> The dock style</param>
        void DockTo(Interfaces.MapWinGISDockStyle dockStyle);
    }

    /// <summary>
    /// legend容器
    /// 操作Legend panel
    /// </summary>
    public interface LegendPanel
    {
        /// <summary>
        /// 关闭LegendPanel
        /// </summary>
        void Close();

        /// <summary>
        /// 浮动，停靠LegendPanel
        /// </summary>
        /// <param name="dockStyle"> The dock style</param>
        void DockTo(MapWinGISDockStyle dockStyle);
    }

    /// <summary>
    /// 插件信息
    /// 存储可用插件信息的集合
    /// </summary>
    public interface PluginInfo
    {
        /// <summary>
        /// 作者信息
        /// </summary>
        string Author { get; }

        /// <summary>
        /// 制作时间
        /// </summary>
        string BuildDate { get; }

        /// <summary>
        /// 插件描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 插件名字
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 插件版本
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 如果插件设置了GUID，就获取
        /// </summary>
        string GUID { get; }

        /// <summary>
        /// Key识别插件，区别于其他其他可用插件的标识
        /// </summary>
        string Key { get; }
    }

    /// <summary>
    /// 操控IPlugin type类型的插件
    /// 不同于CondensedPlugins
    /// </summary>
    public interface Plugins : System.Collections.IEnumerable
    {
        /// <summary>
        /// 从可用插件列表清空所有插件，不清空没有加载的插件
        /// </summary>
        void Clear();

        /// <summary>
        /// 从指定包含文件名的路径添加插件
        /// </summary>
        /// <param name="path">包含文件名的路径</param>
        /// <returns>true成功，false，失败</returns>
        bool AddFromFile(string path);

        /// <summary>
        /// 从目录，添加任何可兼容的插件
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <returns>true on success, false otherwise</returns>
        bool AddFromDir(string path);

        /// <summary>
        /// 从一个实例对象中加载一个插件
        /// </summary>
        /// <param name="plugin">要加载的插件对象</param>
        /// <param name="pluginKey">插件唯一标识Key</param>
        /// <param name="settingsString">当插件加载后传给插件的设置</param>
        /// <returns>true on success, false otherwise</returns>
        bool LoadFromObject(IPlugin plugin, string pluginKey, string settingsString);

        /// <summary>
        /// 从实例对象中加载一个插件
        /// </summary>
        /// <param name="plugin">要加载的插件对象</param>
        /// <param name="pluginKey">插件唯一标识Key</param>
        /// <returns>true on success, false otherwise</returns>
        bool LoadFromObject(IPlugin plugin, string pluginKey);

        /// <summary>
        /// 加载一个指定的插件
        /// </summary>
        /// <param name="key">要加载插件的Key</param>
        /// <returns>true on success, false otherwise</returns>
        bool StartPlugin(string key);

        /// <summary>
        /// 卸载一个指定的插件
        /// </summary>
        /// <param name="key">插件的key</param>
        void StopPlugin(string key);

        /// <summary>
        /// 可用插件的数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 索引，获取一个IPlugin对象，从加载插件列表中
        /// <param name="index">0-based index into the list of plugins</param>
        /// </summary>
        IPlugin this[int index] { get; }

        /// <summary>
        /// 从可用插件列表，移除一个插件，如果插件已经加载，卸载这个插件
        /// </summary>
        /// <param name="indexOrKey">以零开始的索引或者字符串关键字</param>
        void Remove(string key);

        /// <summary>
        /// 设置插件的默认加载文件夹
        /// </summary>
        string PluginFolder { get; set; }

        /// <summary>
        /// 检查插件当前是否加载
        /// </summary>
        /// <param name="key">插件的唯一识别关键字key</param>
        /// <returns>true if loaded, false otherwise</returns>
        bool PluginIsLoaded(string key);

        /// <summary>
        /// 显示插件对话框
        /// Shows the dialog for loading/starting/stopping plugins
        /// </summary>
        void ShowPluginDialog();

        /// <summary>
        /// 以广播的方式发送消息给所有已经加载的插件
        /// </summary>
        /// <param name="message">要发送的消息</param>
        void BroadcastMessage(string message);

        /// <summary>
        /// 根据插件的名称，返回插件的关键字key
        /// </summary>
        /// <param name="pluginName">插件名</param>
        /// <returns>返回空字符串代表该插件没有找到</returns>
        string GetPluginKey(string pluginName);
    }

    /// <summary>
    /// 子菜单
    /// 在主菜单中代表一个子菜单项menu item
    /// </summary>
    public interface MenuItem
    {
        /// <summary>
        /// MenuItem的显示文本
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// 菜单项的icon
        /// </summary>
        object Picture { get; set; }

        /// <summary>
        /// 从item中获取设置菜单的种类
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// item的选择状态
        /// </summary>
        bool Checked { get; set; }

        /// <summary>
        /// 当鼠标over过该item显示的文本提示
        /// 鼠标over事件发生
        /// </summary>
        string Tooltip { get; set; }

        /// <summary>
        /// 是否在该item前面画一条分割线
        /// </summary>
        bool BeginsGroup { get; set; }

        /// <summary>
        /// 获取设置鼠标
        /// </summary>
        System.Windows.Forms.Cursor Cursor { get; set; }

        /// <summary>
        /// 获取设置菜单项的描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 获取设置显示状态
        /// </summary>
        bool Displayed { get; set; }

        /// <summary>
        /// 获取设置菜单项的可用（enabled）的状态
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 获取菜单项名字
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取设置菜单项的可见性
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// 获取包含在这个菜单项下面的子菜单项的数目
        /// </summary>
        int NumSubItems { get; }

        /// <summary>
        /// 返回这个菜单项的子菜单项，通过以零开始的索引
        /// </summary>
        Interfaces.MenuItem SubItem(int index);

        /// <summary>
        /// 返回这个菜单项的子菜单项，通过子菜单的名字
        /// </summary>
        Interfaces.MenuItem SubItem(string name);

        /// <summary>
        /// 获取，这个菜单项是否是第一个可见的子菜单项
        /// </summary>
        bool IsFirstVisibleSubmenuItem { get; }
    }

    /// <summary>
    /// 主菜单
    /// 用作为这个应用操作菜单系统
    /// </summary>
    public interface Menus
    {
        /// <summary>
        /// 用指定的名字，添加一个菜单
        /// </summary>
        /// <param name="name">指定菜单的名字</param>
        /// <returns>菜单对象</returns>
        MenuItem AddMenu(string name);

        /// <summary>
        /// 用指定的名字，图片，添加一个菜单
        /// </summary>
        /// <param name="name"></param>
        /// <param name="picture"></param>
        /// <returns></returns>
        MenuItem AddMenu(string name, object picture);

        /// <summary>
        /// 用指定的名字，图片，文本。添加一个菜单
        /// </summary>
        /// <param name="name"></param>
        /// <param name="picture"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        MenuItem AddMenu(string name, object picture, string text);

        /// <summary>
        /// 用指定的名字添加一个菜单到父菜单上
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentMenu"></param>
        /// <returns></returns>
        MenuItem AddMenu(string name, string parentMenu);

        /// <summary>
        /// 用指定的名字，图片，添加一个菜单到父菜单栏上
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentMenu"></param>
        /// <param name="picture"></param>
        /// <returns></returns>
        MenuItem AddMenu(string name, string parentMenu, object picture);

        /// <summary>
        /// 用指定的名字，图片，文本。添加一个菜单到父菜单上
        /// </summary>
        MenuItem AddMenu(string name, string parentMenu, object picture, string text);

        /// <summary>
        /// 指定名字，图片，文本、添加一个菜单到父菜单上，并且在指定菜单项的后面添加
        /// </summary>
        MenuItem AddMenu(string name, string parentMenu, object picture, string text, string after);

        /// <summary>
        /// 指定名字，图片，文本、添加一个菜单到父菜单上，并且在指定菜单项的前面添加
        /// </summary>
        MenuItem AddMenu(string name, string parentMenu, string text, string before);

        /// <summary>
        /// 索引，通过名字，获取该菜单项
        /// </summary>
        MenuItem this[string menuName] { get; }

        /// <summary>
        /// 移除一个菜单项
        /// </summary>
        /// <param name="name">要移除菜单项的名字</param>
        /// <returns>true on success, false otherwise</returns>
        bool Remove(string name);

        /// <summary>
        /// 当鼠标点击，激活才菜单项
        /// </summary>
        bool PerformClick(string name);

        /// <summary>
        /// 泛型集合，当前已经加载的菜单的名字
        /// </summary>
        /// <returns>菜单名列表</returns>
        System.Collections.Generic.IList<string> MenuNames();

        /// <summary>
        /// 移除所有已经加载的菜单
        /// </summary>
        /// <returns>True移除成功</returns>
        bool RemoveAllMenus();
    }

    /// <summary>
    /// 获得在地图上选择的shape的信息
    /// </summary>
    public interface SelectedShape
    {
        /// <summary>
        /// 初始化所有信息，然后高亮显示shape
        /// </summary>
        /// <param name="ShapeIndex">Index of the shape in the shapefile.</param>
        /// <param name="SelectColor">Color to use when highlighting the shape.</param>
        void Add(int ShapeIndex, System.Drawing.Color SelectColor);

        /// <summary>
        /// 返回选择的shape的范围
        /// </summary>
        MapWinGIS.Extents Extents { get; }

        /// <summary>
        /// 返回所选择的shape的索引
        /// </summary>
        int ShapeIndex { get; }
    }

    /// <summary>
    /// 接口，用作管理所有选择的shape
    /// 所有的选择只能在已选择的层中。可以用LayerHandle属性得到选择的层
    /// </summary>
    public interface SelectInfo : System.Collections.IEnumerable
    {
        /// <summary>
        /// 添加一个已选择的shape到指定的存储选择shape的集合中
        /// </summary>
        /// <param name="newShape">The <c>SelectedShape</c> object to add.</param>
        void AddSelectedShape(SelectedShape newShape);

        /// <summary>
        /// 根据提供的shape的索引，添加到存储以选择shape的集合中
        /// </summary>
        /// <param name="ShapeIndex">要添加的shape的索引.</param>
        /// <param name="SelectColor">不用的参数.</param>
        void AddByIndex(int ShapeIndex, System.Drawing.Color SelectColor);

        /// <summary>
        /// 清空所有已选择的shape的列表
        /// </summary>
        void ClearSelectedShapes();

        /// <summary>
        /// 从集合中移除指定索引的shape
        /// </summary>
        /// <param name="ListIndex">要移除的shape的集中对应的索引</param>
        void RemoveSelectedShape(int ListIndex);

        /// <summary>
        /// 从集合中移除以shapeIndex索引的shape
        /// </summary>
        /// <param name="ShapeIndex">shape的索引</param>
        void RemoveByShapeIndex(int ShapeIndex);

        /// <summary>
        /// 返回选择的层的layerHandle
        /// </summary>
        int LayerHandle { get; }

        /// <summary>
        /// 当前选择的shape的数量
        /// </summary>
        int NumSelected { get; }

        /// <summary>
        /// 返回所有选择shape的全图
        /// </summary>
        MapWinGIS.Extents SelectBounds { get; }

        /// <summary>
        /// 索引，获得SelectedShape
        /// </summary>
        Interfaces.SelectedShape this[int Index] { get; }
    }

    /// <summary>
    /// 添加常用的Drawing到宿主程序
    /// The draw interface is used to add custom drawing to the MapWinGIS view.
    /// </summary>
    public interface Draw
    {
        /// <summary>
        /// 在指定的层上清空所有drawing
        /// 只清空单一的drawing，比清空所有的drawing要快
        /// </summary>
        /// <param name="drawHandle">要清空的层的DrawHandle</param>
        void ClearDrawing(int drawHandle);

        /// <summary>
        /// 在所有正在绘制的层上清空所有自定义绘制的对象
        /// </summary>
        void ClearDrawings();

        /// <summary>
        /// 在当前绘制的层上绘制一个圆
        /// </summary>
        /// <param name="x">圆点x点坐标</param>
        /// <param name="y">圆点y坐标</param>
        /// <param name="PixelRadius">半径，单位：像素</param>
        /// <param name="Color">园的颜色</param>
        /// <param name="FillCircle">是否填充</param>
        void DrawCircle(double x, double y, double pixelRadius, System.Drawing.Color color, bool fillCircle);

        /// <summary>
        ///在当前绘制的层上绘制一条线
        /// </summary>
        /// <param name="X1">起点x坐标</param>
        /// <param name="Y1">起点y坐标</param>
        /// <param name="X2">终点x坐标</param>
        /// <param name="Y2">终点y坐标.</param>
        /// <param name="PixelWidth">线宽，单位：像素</param>
        /// <param name="Color">线的颜色</param>
        void DrawLine(double X1, double Y1, double X2, double Y2, int PixelWidth, System.Drawing.Color color);

        /// <summary>
        /// 在当前绘制的层上，绘制一个点
        /// </summary>
        /// <param name="x">点的x坐标</param>
        /// <param name="y">点的y坐标</param>
        /// <param name="PixelSize">点的大小，单位：像素.</param>
        /// <param name="Color">点的颜色</param>
        void DrawPoint(double x, double y, int pixelSize, System.Drawing.Color color);

        /// <summary>
        /// 在当前绘制的层上，绘制一个多边形
        /// </summary>
        /// <param name="x">多边形x坐标数组</param>
        /// <param name="y">多边形y坐标数组</param>
        /// <param name="Color">绘制多边形的颜色</param>
        /// <param name="FillPolygon">是否填充多边形.</param>
        /// <remarks>点要以顺时针存放，并且如果要填充颜色的话要求没有交叉，起点要与终点相同</remarks>
        void DrawPolygon(double[] x, double[] y, System.Drawing.Color color, bool fillPolygon);

        /// <summary>
        /// 创建一个新的绘制层
        /// Creates a new drawing layer.
        /// </summary>
        /// <param name="Projection">指定是用屏幕坐标还是使用地图坐标</param>
        /// <returns>返回绘制的句柄handle.  如果以后要清除这个绘制的层，应该保存这个handle</returns>
        /// <remarks>图层绘制，在这个版本只有部分功能实现了</remarks>
        int NewDrawing(MapWinGIS.tkDrawReferenceList projection);

        /// <summary>
        /// 是否使用双缓冲，这样可以使得绘制的对象显得圆滑
        /// </summary>
        bool DoubleBuffer { get; set; }

        /// <summary>
        /// 给当前绘制的层添加一个标签
        /// </summary>
        /// <param name="DrawHandle">Handle of the drawing layer</param>
        /// <param name="Text">标签文本</param>
        /// <param name="Color">文本颜色</param>
        /// <param name="x">在地图上的x坐标</param>
        /// <param name="y">在地图上的y坐标</param>
        /// <param name="hJustification">文本对齐方式.</param>
        void AddDrawingLabel(int drawHandle, string Text, System.Drawing.Color color, double x, double y, MapWinGIS.tkHJustification hJustification);

        /// <summary>
        /// 给当前新绘制的层添加一个标签
        /// </summary>
        /// <param name="DrawHandle">Handle of the drawing layer</param>
        /// <param name="Text">标签文本</param>
        /// <param name="Color">文本颜色t</param>
        /// <param name="x">在地图上的x坐标</param>
        /// <param name="y">在地图上的y坐标</param>
        /// <param name="hJustification">文本对齐方式</param>
        /// <param name="Rotation">标签的旋转角度</param>
        void AddDrawingLabelEx(int drawHandle, string Text, System.Drawing.Color color, double x, double y, MapWinGIS.tkHJustification hJustification, double rotation);

        /// <summary>
        /// 在当前新绘制的层上清除所有的标签
        /// </summary>
        /// <param name="DrawHandle">Handle of the drawing layer</param>
        void ClearDrawingLabels(int drawHandle);

        /// <summary>
        /// 在当前新绘制的层上绘制一个圆
        /// </summary>
        /// <param name="DrawHandle">Handle of the drawing layer</param>
        /// <param name="x">圆点x坐标</param>
        /// <param name="y">圆点y坐标</param>
        /// <param name="pixelRadius">半径，单位：像素</param>
        /// <param name="Color">颜色</param>
        /// <param name="FillCircle">是否填充/param>
        void DrawCircleEx(int DrawHandle, double x, double y, double pixelRadius, System.Drawing.Color color, bool fillCircle);

        /// <summary>
        /// 在新绘制的层上，绘制条线
        /// </summary>
        void DrawLineEx(int DrawHandle, double x1, double y1, double x2, double y2, int PixelWidth, System.Drawing.Color color);

        /// <summary>
        /// 在新绘制的层上，绘制一个点
        /// </summary>
        void DrawPointEx(int drawHandle, double x, double y, int pixelSize, System.Drawing.Color color);

        /// <summary>
        /// 在新绘制的层上，绘制一个多边形
        /// </summary>
        void DrawPolygonEx(int drawHandle, double[] x, double[] y, System.Drawing.Color color, bool fillPolygon);

        /// <summary>
        /// 在当前绘制的层上，绘制一个指定线宽的圆
        /// </summary>
        void DrawWideCircle(double x, double y, double pixelRadius, System.Drawing.Color color, bool fillCircle, Int16 pixelWidth);

        /// <summary>
        /// 在当前绘制的层上，绘制一个指定线宽的多边形
        /// </summary>
        void DrawWidePolygon(double[] x, double[] y, System.Drawing.Color color, bool fill, Int16 pixelWidth);

        /// <summary>
        /// 在当前绘制的层上设置字体
        /// </summary>
        /// <param name="drawHandle">The handle of the drawing layer</param>
        /// <param name="fontName">字体的名字</param>
        /// <param name="fontSize">字体的大小</param>
        void DrawingFont(int drawHandle, string fontName, int fontSize);

        /// <summary>
        /// 设置是否显示这个图层
        /// </summary>
        /// <param name="drawHandle">The handle of the drawing layer</param>
        /// <param name="visible">Visible or not</param>
        void SetDrawingLayerVisible(int drawHandle, bool visible);

        /// <summary>
        /// 设置在当前图层上，标签是否显示
        /// </summary>
        /// <param name="drawHandle">The handle of the drawing layer</param>
        /// <param name="visible">Visible or not</param>
        void SetDrawingLabelsVisible(int drawHandle, bool visible);

        /// <summary>
        /// 在新绘制的层上，绘制一个指定线宽的多边形
        /// </summary>
        void DrawWidePolygonEx(int DrawHandle, double[] x, double[] y, System.Drawing.Color Color, bool fill, Int16 pixelWidth);

        /// <summary>
        /// 在新绘制的层上，绘制一个指定线宽的原
        /// Draws a circle on the current drawing layer and specify the width of the line
        /// </summary>
        void DrawWideCircleEx(int drawHandle, double x, double y, double pixelRadius, System.Drawing.Color color, bool FillCircle, Int16 pixelWidth);
    }

    /// <summary>
    /// 从存储shapes 的列表中，提取信息
    /// This interface is used to access the list of shapes that were found during an Identify function call.
    /// </summary>
    public interface IdentifiedShapes
    {
        /// <summary>
        /// 被选中的shapes的数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 根据存储在ArrayList索引返回shape的索引
        /// </summary>
        int this[int index] { get; }
    }

    /// <summary>
    /// 从存储layers的列表中，提取层查询所必须的信息
    /// IdentifiedLayers is used to access the list of layers that contained any
    /// information gathered during an Identify function call.
    /// </summary>
    public interface IdentifiedLayers
    {
        /// <summary>
        /// 返回从Identify方法调用的并存在相关信息的图层的数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 返回一个IdentifiedShapes对象
        /// </summary>
        Interfaces.IdentifiedShapes this[int layerHandle] { get; }
    }

    /// <summary>
    /// 视图接口，操作或者协作宿主程序的显示
    /// The View interface is used to manipulate or work with the main MapWinGIS display.
    /// </summary>
    public interface View
    {
        /// <summary>
        /// 在所有的层上清空所有的shapes
        /// </summary>
        void ClearSelectedShapes();

        /// <summary>
        /// 在给定的点下，查询所有的活动的shapefile层
        /// Queries all of the active shapefile layers for any within the specified tolerance of the given point.
        /// </summary>
        /// <param name="ProjX">要查询的相对于地图的x点坐标</param>
        /// <param name="ProjY">要查询的相对于地图的y点坐标</param>
        /// <param name="Tolerance">在所查询点坐标周围，允许的距离</param>
        /// <returns>Returns an <c>IdentifiedLayers</c> object containing query results.</returns>
        Interfaces.IdentifiedLayers Identify(double ProjX, double ProjY, double Tolerance);

        /// <summary>
        /// 在没有解锁之前，阻止legend中的任何操作。
        /// 并且，保持所有锁定前的数据
        /// </summary>
        void LockLegend();

        /// <summary>
        /// 解锁legend，允许通过它重绘和更新视图，并反应出我们所做的任何改变
        /// </summary>
        void UnlockLegend();

        /// <summary>
        /// 锁地图，在没有解锁地图（Unlocked）前，阻止在层上做的任何改变，更新显示在地图上
        /// 并保持锁前所有的数据
        /// </summary>
        void LockMap();

        /// <summary>
        /// 决定PreviewMap控件是否可见
        /// </summary>
        bool PreviewVisible { get; set; }

        /// <summary>
        /// 决定legend插件是否可见
        /// </summary>
        bool LegendVisible { get; set; }

        /// <summary>
        /// 解锁，当做出改变时，允许地图重绘
        /// </summary>
        void UnlockMap();

        /// <summary>
        /// 将屏幕上的坐标点（像素）转换成地图上的坐标点
        /// </summary>
        /// <param name="PixelX">ref 屏幕坐标点的x坐标</param>
        /// <param name="PixelY">ref 屏幕坐标点的y坐标</param>
        /// <param name="ProjX">ref 地图上的x坐标</param>
        /// <param name="ProjY">ref 地图上的y坐标</param>
        void PixelToProj(double PixelX, double PixelY, ref double ProjX, ref double ProjY);

        /// <summary>
        /// 将地图上的坐标点转换成屏幕上的坐标点
        /// </summary>
        /// <param name="ProjX">ref 地图上的x坐标</param>
        /// <param name="ProjY">ref 地图上的y坐标</param>
        /// <param name="PixelX">ref 屏幕上的x坐标</param>
        /// <param name="PixelY">ref 屏幕上的y坐标</param>
        void ProjToPixel(double ProjX, double ProjY, ref double PixelX, ref double PixelY);

        /// <summary>
        /// 让地图重绘，但是单地图锁住时，无法执行该功能
        /// </summary>
        void Redraw();

        /// <summary>
        /// 显示一个提示在地图上的鼠标下面
        /// </summary>
        /// <param name="Text">要显示的文本</param>
        /// <param name="Milliseconds">显示时间</param>
        void ShowToolTip(string Text, int Milliseconds);

        /// <summary>
        /// 将所有的加载的可见的层Zoom为全图
        /// </summary>
        void ZoomToMaxExtents();

        /// <summary>
        /// 根据给定的百分比，缩小地图显示
        /// </summary>
        /// <param name="Percent">缩小的比例</param>
        void ZoomIn(double Percent);

        /// <summary>
        /// 根据给定的百分比，放大地图显示
        /// </summary>
        /// <param name="Percent">放大的比例</param>
        void ZoomOut(double Percent);

        /// <summary>
        /// 返回到前一次显示样式
        /// </summary>
        void ZoomToPrev();

        /// <summary>
        /// 获取一个活动的legend
        /// </summary>
        LegendControl.Legend LegendControl { get; }

        /// <summary>
        /// 获取设置存储地图放大缩小等操作历史的数组
        /// </summary>
        System.Collections.ArrayList ExtentHistory { get; set; }

        /// <summary>
        /// 在指定的范围内，当前可见的层中，指定的范围内截图Takes a snapshot
        /// </summary>
        /// <param name="Bounds">要截图的范围</param>
        MapWinGIS.Image Snapshot(MapWinGIS.Extents Bounds);

        /// <summary>
        /// 获取设置地图的背景色
        /// </summary>
        System.Drawing.Color BackColor { get; set; }

        /// <summary>
        /// 获取设置当前鼠标的样式. 可用样式：
        /// <list type="bullet">
        /// <item>cmNone</item>
        /// <item>cmPan</item>
        /// <item>cmSelection</item>
        /// <item>cmZoomIn</item>
        /// <item>cmZoomOut</item>
        /// </list>
        /// </summary>
        MapWinGIS.tkCursorMode CursorMode { get; set; }

        /// <summary>
        /// 获取常用的Drawing
        /// </summary>
        Interfaces.Draw Draw { get; }

        /// <summary>
        /// Gets or sets the amount to pad around the extents when calling <c>ZoomToMaxExtents</c>,
        /// <c>ZoomToLayer</c>, and <c>ZoomToShape</c>.
        /// </summary>
        double ExtentPad { get; set; }

        /// <summary>
        /// Gets or sets the map's current extents.
        /// </summary>
        MapWinGIS.Extents Extents { get; set; }

        /// <summary>
        /// 作用在地图上鼠标样式.  可用样式如下:
        /// <list type="bullet">
        /// <item>crsrAppStarting</item>
        /// <item>crsrArrow</item>
        /// <item>crsrCross</item>
        /// <item>crsrHelp</item>
        /// <item>crsrIBeam</item>
        /// <item>crsrMapDefault</item>
        /// <item>crsrNo</item>
        /// <item>crsrSizeAll</item>
        /// <item>crsrSizeNESW</item>
        /// <item>crsrSizeNS</item>
        /// <item>crsrSizeNWSE</item>
        /// <item>crsrSizeWE</item>
        /// <item>crsrUpArrow</item>
        /// <item>crsrUserDefined</item>
        /// <item>crsrWait</item>
        /// </list>
        /// </summary>
        MapWinGIS.tkCursor MapCursor { get; set; }

        /// <summary>
        /// Indicates that the map should handle file drag-drop events (as opposed to firing a message indicating file(s) were dropped).
        /// </summary>
        bool HandleFileDrop { get; set; }

        /// <summary>
        /// MapState是描述整个地图状态的字符串，包括：层和配色
        /// </summary>
        string MapState { get; set; }

        /// <summary>
        /// 获取SelectInfo对象所包含的关于当前层的所有shapes的信息
        /// </summary>
        Interfaces.SelectInfo SelectedShapes { get; }

        /// <summary>
        /// 决定标签是否从project-specific或shapefile-specific文件夹保存和加载 
        /// Using a project-level label file will create a new subdirectory with the project's name.
        /// </summary>
        bool LabelsUseProjectLevel { get; set; }

        /// <summary>
        /// 在给定的层中引发重新加载field values
        /// 如果层（shapefile、labels）的handle是无效的，任何事件不会发生
        /// </summary>
        /// <param name="LayerHandle"></param>
        void LabelsRelabel(int LayerHandle);

        /// <summary>
        /// 显示label编辑器
        /// 如果LayerHandle是无效的或没有shapefile，不会发生任何事件
        /// </summary>
        /// <param name="LayerHandle"></param>
        void LabelsEdit(int LayerHandle);

        /// <summary>
        /// 是否保持选择。默认为false
        /// 是，那么之前的选择在没有选择新想shape之前不会清空。
        /// 否，那么所有的选择将清空
        /// </summary>
        bool SelectionPersistence { get; set; }

        /// <summary>
        /// 在地图中可容忍选择的公差
        /// </summary>
        double SelectionTolerance { get; set; }

        /// <summary>
        /// 获取设置 选择模式：包含、交叉
        /// <list type="bullet">
        /// <item>Inclusion</item>
        /// <item>Intersection</item>
        /// </summary>
        MapWinGIS.SelectMode SelectMethod { get; set; }

        /// <summary>
        /// 获取设置被选择的shape的颜色
        /// </summary>
        System.Drawing.Color SelectColor { get; set; }

        /// <summary>
        /// 与地图相关的提示语
        /// </summary>
        string Tag { get; set; }

        /// <summary>
        /// 当CursorMode是cmUserDefined时，获取设置鼠标句柄
        /// </summary>
        int UserCursorHandle { get; set; }

        /// <summary>
        /// 当在地图上用鼠标Zoom时，设置默认Zoom比例
        /// </summary>
        double ZoomPercent { get; set; }

        /// <summary>
        /// S在指定的点上选择shape. 误差在:View.SelectionTolerance中设置
        /// </summary>
        /// <param name="ScreenX">相对于屏幕的x坐标</param>
        /// <param name="ScreenY">相对于屏幕的y坐标</param>
        /// <param name="ClearOldSelection">是否清空所有之前选择的shape</param>
        Interfaces.SelectInfo Select(int ScreenX, int ScreenY, bool ClearOldSelection);

        /// <summary>
        /// 选择用户用鼠标指定的矩形内的shapes
        /// </summary>
        /// <param name="ScreenBounds">相对于屏幕的矩形的坐标点</param>
        /// <param name="ClearOldSelection">是否清空所有之前选择的shape</param>
        Interfaces.SelectInfo Select(System.Drawing.Rectangle ScreenBounds, bool ClearOldSelection);

        /// <summary>获取设置shapes的绘制方法</summary>
        MapWinGIS.tkShapeDrawingMethod ShapeDrawingMethod { get; set; }

        /// <summary>获取map control的高度</summary>
        /// <remarks>Added by Paul Meems on May 26 2010</remarks>
        int MapHeight { get; }

        /// <summary>获取map control的宽度</summary>
        int MapWidth { get; }

        /// <summary>获取设置当前视图的scale</summary>
        double Scale { get; set; }

        /// <summary>获取地图是否被锁</summary>
        bool IsMapLocked { get; }

        /// <summary>
        /// 是否能用CanUseImageGrouping属性
        /// 提高Image加载速度
        /// </summary>
        bool CanUseImageGrouping { get; set; }

        /// <summary>
        /// 在指定的层上更新选择项
        /// 别的层上的选择将保持
        /// </summary>
        /// <param name="sf">要更新的Shapefile</param>
        /// <param name="Indices">所有相关的Shape</param>
        /// <param name="Mode">选择操作</param>
        /// <returns></returns>
        Interfaces.SelectInfo UpdateSelection(int LayerHandle, ref int[] Indices, Interfaces.SelectionOperation Mode);

        /// <summary>
        /// 返回最大可见范围
        /// 所有可见的关联的层的范围
        /// </summary>
        MapWinGIS.Extents MaxVisibleExtents { get; }

        /// <summary>
        /// 重绘map和legend
        /// </summary>
        void ForceFullRedraw();

        /// <summary>
        /// 在legend和toolbox中转换，显示legend
        /// </summary>
        void ShowLegend();

        /// <summary>
        /// 在legend和toolbox中转换，显示toolbox
        /// </summary>
        void ShowToolbox();
    }

    /// <summary>
    /// 包含在shapefile中的shape
    /// </summary>
    public interface Shape
    {
        /// <summary>
        /// 移动shape
        /// View.ExtentPad将增加一个小的空间，以便容易看见这个shape
        /// </summary>
        void ZoomTo();

        /// <summary>
        /// 获取设置这个shape的颜色
        /// </summary>
        System.Drawing.Color Color { get; set; }

        /// <summary>
        /// 是否填充这个shape
        /// 只能应用于多边形的shape
        /// </summary>
        bool DrawFill { get; set; }

        /// <summary>
        /// 填充方式
        /// 只能应用于多边形的shape
        /// 可用 的方式如下：
        /// <list type="bullet">
        /// <item>fsCustom</item>
        /// <item>fsDiagonalDownLeft</item>
        /// <item>fsDiagonalDownRight</item>
        /// <item>fsHorizontalBars</item>
        /// <item>fsNone</item>
        /// <item>fsPolkaDot</item>
        /// <item>fsVerticalBars</item>
        /// </list>
        /// </summary>
        MapWinGIS.tkFillStipple FillStipple { get; set; }

        /// <summary>
        /// 获取设置点或线的尺寸.  
        /// 如果PointType是ptUserDefined类型，点或线的尺寸将和用户定义的并联
        /// </summary>
        float LineOrPointSize { get; set; }

        /// <summary>
        /// 绘制shape的line的类型
        /// 可用类型如下：
        /// <list type="bullet">
        /// <item>lsCustom</item>
        /// <item>lsDashDotDash</item>
        /// <item>lsDashed</item>
        /// <item>lsDotted</item>
        /// <item>lsNone</item>
        /// </list>
        /// </summary>
        MapWinGIS.tkLineStipple LineStipple { get; set; }

        /// <summary>
        /// outline（轮廓） color for this shape.  
        /// Only applies to polygon shapes.
        /// </summary>
        System.Drawing.Color OutlineColor { get; set; }

        /// <summary>
        /// 点的类型
        /// 可用类型:
        /// <list type="bullet">
        /// <item>ptCircle</item>
        /// <item>ptDiamond</item>
        /// <item>ptImageList</item>
        /// <item>ptSquare</item>
        /// <item>ptTriangleDown</item>
        /// <item>ptTriangleLeft</item>
        /// <item>ptTriangleRight</item>
        /// <item>ptTriangleUp</item>
        /// <item>ptUserDefined</item>
        /// </list>
        /// </summary>
        MapWinGIS.tkPointType PointType { get; set; }

        /// <summary>
        /// shape是否可见
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// 显示顶点
        /// 顶点能够被隐藏
        /// </summary>
        /// <param name="color">顶点颜色</param>
        /// <param name="vertexSize">顶点大小（像素）</param>
        void ShowVertices(System.Drawing.Color color, int vertexSize);

        /// <summary>
        /// 隐藏顶点
        /// </summary>
        void HideVertices();

        /// <summary>
        /// 从images列表中获取设置图片索引
        /// </summary>
        long ShapePointImageListID { get; set; }

        /// <summary>
        /// 多边形shape的透明百分比
        /// </summary>
        float ShapeFillTransparency { get; set; }

        /// <summary>
        /// shape填充颜色
        /// </summary>
        System.Drawing.Color ShapeFillColor { get; set; }

        /// <summary>
        /// shape的line的颜色
        /// </summary>
        System.Drawing.Color ShapeLineColor { get; set; }

        float ShapeLineWidth { get; set; }
    }

    /// <summary>
    /// 从shapefile中取出特定的shape
    /// </summary>
    public interface Shapes : System.Collections.IEnumerable
    {
        /// <summary>
        /// 在shapefile中的shape的数量
        /// </summary>
        int NumShapes { get; }

        /// <summary>
        /// 索引，返回一个shape，通过在shapefile中的索引
        /// </summary>
        Interfaces.Shape this[int Index] { get; }
    }

    /*--------------Group和Groups是新增接口---------------------*/
    public interface Group
    {
        /// <summary>
        /// 获取设置显示在legend上组的文本
        /// Gets or sets the Text that appears in the legend for this group
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// 获取和设置显示在该组下面的图标
        /// 设置该值null，则移除该图标
        /// </summary>
        object Icon { get; set; }

        /// <summary>
        /// 获取在该组中层的数量
        /// Gets the number of layers within this group
        /// </summary>
        int LayerCount { get; }

        /// <summary>
        /// 获取该组的句柄（handle），该标志是唯一的
        /// </summary>
        int Handle { get; }

        /// <summary>
        /// 根据该层在该组中的位置获取层的句柄
        /// </summary>
        int LayerHandle(int PositionInGroup);

        /// <summary>
        /// 获取和设置该组是否是展开的
        /// 这将显示或隐藏在该组中的层
        /// </summary>
        bool Expanded { get; set; }

        /// <summary>
        /// 获取和设置该组中所有层的可见性
        /// </summary>
        bool LayersVisible { get; set; }

        /// <summary>
        /// 获取和设置锁定状态的属性，这将阻止用户改变该可见性状态
        /// </summary>
        bool StateLocked { get; set; }

        /// <summary>
        /// 返回该组的闪照图像
        /// </summary>
        /// <param name="imgWidth">返回的图像的宽度（pix），高度根据该组中层的个数</param>
        /// <returns>该组及其子层（若展开了）一个Bitmap对象</returns>
        System.Drawing.Bitmap Snapshot(int imgWidth);


    }

    /// <summary>
    /// legend 中的Group（组）接口
    /// </summary>
    public interface Groups
    {
        /// <summary>
        /// 添加一个新的组到legend中的最上面
        /// </summary>
        int Add();

        /// <summary>
        /// 用指定名称（Caption）在legend的最上面添加一个新的组
        /// </summary>
        int Add(string Name);

        /// <summary>
        /// 指定名称、位置在legend上添加一个新的组
        /// </summary>
        int Add(string Name, int Position);

        /// <summary>
        /// 移除一个组及其所有的层
        /// </summary>
        bool Remove(int Handle);

        /// <summary>
        /// 获取在当前legend中的组的数量
        /// </summary>
        int Count { get; }

        Interfaces.Group this[int Position] { get; }

        /// <summary>
        /// 清空所有的组和层
        /// </summary>
        void Clear();

        /// <summary>
        /// 可以通过组的位置获取一个组对象
        /// </summary>
        Interfaces.Group ItemByPosition(int Position);

        /// <summary>
        /// 通过组的句柄查找一个组
        /// </summary>
        /// <param name="Handle">代表该组的唯一的号码Handle</param>
        /// <returns>返回一个Group对象，以便读取或者该变该Group的属性,null表示获取失败</returns>
        Interfaces.Group ItemByHandle(int Handle);

        /// <summary>
        /// 在组列表中查找照组的位置（index）
        /// </summary>
        /// <param name="GroupHandle">代表该组的唯一的号码Handle</param>
        int PositionOf(int GroupHandle);

        /// <summary>
        /// 检查指定handle的组是否仍存在于组列表中
        /// </summary>
        /// <param name="Handle">Group handle</param>
        /// <returns>True存在, False 其他</returns>
        bool IsValidHandle(int Handle);

        /// <summary>
        /// 将指定的组移动到新的位置
        /// </summary>
        /// <param name="GroupHandle">要移动组的handle</param>
        /// <param name="NewPos">组要放置的位置（从0开始）</param>
        /// <returns>True 移动成功, False 其他</returns>
        bool MoveGroup(int GroupHandle, int NewPos);

        /// <summary>
        /// 折叠所有的组
        /// </summary>
        void CollapseAll();

        /// <summary>
        /// 展开所有的组
        /// </summary>
        void ExpandAll();
 
    }

    /// <summary>
    /// 接口，包含层的属性和方法
    /// </summary>
    public interface Layer
    {
        /// <summary>
        /// 舍弃的属性
        /// </summary>
        int LineSeparationFactor { get; set; }

        /// <summary>
        /// 添加一个label到当前层
        /// </summary>
        /// <param name="Text">The text of the label.</param>
        /// <param name="TextColor">The color of the label text.</param>
        /// <param name="xPos">X position in projected map units.</param>
        /// <param name="yPos">Y position in projected map units.</param>
        /// <param name="Justification">Text justification.  Can be hjCenter, hjLeft or hjRight.</param>
        void AddLabel(string Text, System.Drawing.Color TextColor, double xPos, double yPos, MapWinGIS.tkHJustification Justification);

        /// <summary>
        /// 添加一个扩展的label到当前层
        /// </summary>
        /// <param name="Text">The text of the label.</param>
        /// <param name="TextColor">The color of the label text.</param>
        /// <param name="xPos">X position in projected map units.</param>
        /// <param name="yPos">Y position in projected map units.</param>
        /// <param name="Justification">Text justification.  Can be hjCenter, hjLeft or hjRight.</param>
        /// <param name="Rotation">The rotation angle for the label.</param>
        void AddLabelEx(string Text, System.Drawing.Color TextColor, double xPos, double yPos, MapWinGIS.tkHJustification Justification, double Rotation);

        /// <summary>
        /// 更新存储label的文件夹信息
        /// Updates the label information file stored for this layer.
        /// </summary>
        void UpdateLabelInfo();

        /// <summary>
        /// 清空这个层上的所有label
        /// </summary>
        void ClearLabels();

        /// <summary>
        /// 设置当前层上所有label的字体
        /// </summary>
        /// <param name="FontName">Name of the font or font family.  Example:  "Arial"</param>
        /// <param name="FontSize">Size of the font.</param>
        void Font(string FontName, int FontSize);

        /// <summary>
        /// 设置当前层上所有label的字体
        /// </summary>
        /// <param name="FontName">Name of the font or font family.  Example:  "Arial"</param>
        /// <param name="FontSize">Size of the font.</param>
        /// <param name="FontStyle">Style of the font: Bold, Italic, Underline</param>
        void Font(string FontName, int FontSize, System.Drawing.FontStyle FontStyle);

        /// <summary>
        /// 移动这个层的显示
        /// 把View.ExtentPad考虑在内
        /// </summary>
        void ZoomTo();

        /// <summary>
        /// 获取shapefile、Image对象
        /// 如果是Grid对象，则用GetGridObject方法重新检索
        /// </summary>
        object GetObject();

        /// <summary>
        /// 在layer列表中将当前层移动到一个新的位置
        /// </summary>
        /// <param name="NewPosition">The new position.</param>
        /// <param name="TargetGroup">The group to put this layer in.</param>
        void MoveTo(int NewPosition, int TargetGroup);

        /// <summary>
        /// shapefile的颜色，只应用与shapefile
        /// 设置shapefile的颜色将清空所有选择的shapes并且重置每个shape为相同的颜色
        /// 同样颜色配置也会被覆盖
        /// </summary>
        System.Drawing.Color Color { get; set; }

        /// <summary>
        /// 是否绘制填充多边形的shapefile
        /// </summary>
        bool DrawFill { get; set; }

        /// <summary>
        /// 是否层的颜色配置也显示在legend中
        /// </summary>
        bool Expanded { get; set; }

        /// <summary>
        /// 全图
        /// </summary>
        MapWinGIS.Extents Extents { get; }

        /// <summary>
        /// 获得包含路径的层的文件名.  
        /// 如果是memory-based only，也可能没有文件名
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// 获得这个层的投影（projection）
        /// projections必须是PROJ4格式
        /// 没有则null
        /// 如果提供的是一个无效的projection，会保存不成功
        /// </summary>
        string Projection { get; set; }

        /// <summary>
        /// 获取设置填充整个shapefile的样式
        ///
        /// 样式如下：
        /// <list type="bullet">
        /// <item>lsCustom</item>
        /// <item>lsDashDotDash</item>
        /// <item>lsDashed</item>
        /// <item>lsDotted</item>
        /// <item>lsNone</item>
        /// </list>
        /// </summary>
        MapWinGIS.tkFillStipple FillStipple { get; set; }

        /// <summary>
        /// 返回层的Grid类型对象，如果没有grid layer则返回Nothing
        /// </summary>
        MapWinGIS.Grid GetGridObject { get; }

        /// <summary>
        /// 层的handle，MapWin自动为层设置一个LayerHandle，并且不能被重置清空
        /// </summary>
        int Handle { get; set; }

        /// <summary>
        /// 设置文本起点到标签的距离（像素）
        /// </summary>
        int LabelsOffset { get; set; }

        /// <summary>
        /// 决定标签是否可以按比例该变
        /// </summary>
        bool LabelsScale { get; set; }

        /// <summary>
        /// label的阴影
        /// </summary>
        System.Drawing.Color LabelsShadowColor { get; set; }

        /// <summary>
        /// Deprecated.舍弃的方法
        /// </summary>
        void HatchingRecalculate();

        /// <summary>
        /// 标签是否被层所遮盖
        /// </summary>
        bool LabelsShadow { get; set; }

        /// <summary>
        /// label的可见性
        /// </summary>
        bool LabelsVisible { get; set; }

        /// <summary>
        /// 当与已存在的标签冲突时，是否让MapWinGIS ocx隐藏标签
        /// </summary>
        bool UseLabelCollision { get; set; }

        /// <summary>
        /// 层的类型，可选如下：
        /// <list type="bullet">
        /// <item>Grid</item>
        /// <item>Image</item>
        /// <item>Invalid</item>
        /// <item>LineShapefile</item>
        /// <item>PointShapefile</item>
        /// <item>PolygonShapefile</item>
        /// </list>
        /// </summary>
        Interfaces.eLayerType LayerType { get; }

        /// <summary>
        /// 颜色配置，shapefile和grid有各自的配色方案，
        /// </summary>
        object ColoringScheme { get; set; }

        /// <summary>
        /// 舍弃的属性，用Shapefile.DefaultDrawingOptions替代
        /// </summary>
        Interfaces.ShapefileFillStippleScheme FillStippleScheme { get; set; }

        /// <summary>
        /// 画线颜色设置，用于定义多边形shap
        /// </summary>
        System.Drawing.Color FillStippleLineColor { get; set; }

        /// <summary>
        /// hatching的透明度，用于多边形的shap
        /// </summary>
        bool FillStippleTransparency { get; set; }

        /// <summary>
        /// 设置在legend中层的icon
        /// </summary>
        object Icon { get; set; }

        /// <summary>
        /// 线和点的大小，如果PiontType是ptUserDefined类型，则由LineOrPointSize整合（像素）
        /// </summary>
        float LineOrPointSize { get; set; }

        /// <summary>
        /// expansion box设置是否显示
        /// </summary>
        bool ExpansionBoxForceAllowed { get; set; }

        /// <summary>
        /// 为层扩充一个区域，当ExpansionBoxForceAllowed为true是可用
        /// 并且，要用这个功能，必须设置ExpansionBoxCustomHeight
        /// </summary>
        LegendControl.ExpansionBoxCustomRenderer ExpansionBoxCustomRenderFunction { get; set; }

        /// <summary>
        /// 告诉legend你要设置的高度
        /// </summary>
        LegendControl.ExpansionBoxCustomHeight ExpansionBoxCustomHeightFunction { get; set; }

        /// <summary>
        /// 获取设置整个shapefile的绘制方案
        /// 可用属性如下：
        /// <list type="bullet">
        /// <item>lsCustom</item>
        /// <item>lsDashDotDash</item>
        /// <item>lsDashed</item>
        /// <item>lsDotted</item>
        /// <item>lsNone</item>
        /// </list>
        /// </summary>
        MapWinGIS.tkLineStipple LineStipple { get; set; }

        /// <summary>
        /// 层的名字
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 设置这个层的多边形的shapefile的轮廓线条颜色
        /// </summary>
        System.Drawing.Color OutlineColor { get; set; }

        /// <summary>
        /// 设置shapefile点的类型
        /// 可选类型如下：
        /// <list type="bullet">
        /// <item>ptCircle</item>
        /// <item>ptDiamond</item>
        /// <item>ptSquare</item>
        /// <item>ptTriangleDown</item>
        /// <item>ptTriangleLeft</item>
        /// <item>ptTriangleRight</item>
        /// <item>ptTriangleUp</item>
        /// <item>ptUserDefined</item>
        /// </list>
        /// </summary>
        MapWinGIS.tkPointType PointType { get; set; }

        /// <summary>
        /// 全局位置
        /// 获取设置不依赖与任何组的层的位置
        /// </summary>
        int GlobalPosition { get; set; }

        /// <summary>
        /// 在组中的层的位置
        /// </summary>
        int GroupPosition { get; set; }

        /// <summary>
        /// 获取设置层所属于的组的handle
        /// </summary>
        int GroupHandle { get; set; }

        /// <summary>
        /// 获取在一个层中的所有shapes，但是只应用与shapefile类型的层
        /// </summary>
        Interfaces.Shapes Shapes { get; }

        /// <summary>
        /// 让用户可以选择这个层的标准视图宽度
        /// </summary>
        double StandardViewWidth { get; set; }

        /// <summary>
        /// 获取设置这个层的tag（标签，一个可以存储任何信息的字符串）
        /// </summary>
        string Tag { get; set; }

        /// <summary>
        /// 获取设置Image类型的层的透明色
        /// </summary>
        System.Drawing.Color ImageTransparentColor { get; set; }

        /// <summary>
        /// 获取设置Image类型层的透明色2
        /// </summary>
        System.Drawing.Color ImageTransparentColor2 { get; set; }

        /// <summary>
        /// 获取设置是否在Image类型层中用透明色
        /// </summary>
        bool UseTransparentColor { get; set; }

        /// <summary>
        /// 用户自定义线条
        /// </summary>
        int UserLineStipple { get; set; }

        /// <summary>
        /// 在用户自定义线条中获取单一行
        /// 在fill stipple中有32行，每行都用同样的UserLineStipple
        /// </summary>
        /// <param name="Row">在0-31中选择一个要选择的行的索引</param>
        /// <returns>A single stipple row.</returns>
        int GetUserFillStipple(int Row);

        /// <summary>
        /// 舍弃的方法，设置用户自定义中的一个单行
        /// </summary>
        /// <param name="Row">The index of the row to set.  Must be between 0 and 31 inclusive.</param>
        /// <param name="Value">The row value to set in the fill stipple.</param>
        void SetUserFillStipple(int Row, int Value);

        /// <summary>
        /// 在这个层中获取或设置图层上点的图像，该图像是由用户自定义的图像
        /// 要显示用户定义的点图像，PointType必须设置为ptUserDefined
        /// </summary>
        MapWinGIS.Image UserPointType { get; set; }

        /// <summary>
        /// 获取设置层是否可见
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// 为整个shapefile设置所有的顶点，只应用于line和polygon类型的shapefile
        /// </summary>
        /// <param name="color">顶点颜色</param>
        /// <param name="vertexSize">顶点大小</param>
        void ShowVertices(System.Drawing.Color color, int vertexSize);

        /// <summary>
        /// line和polygon的顶点是否显示
        /// (Doesn't apply to line shapefiles)
        bool VerticesVisible { get; set; }

        /// <summary>
        /// 隐藏所有的顶点，只应用于line和polygon类型的shapefile
        /// </summary>
        void HideVertices();

        /// <summary>
        /// 获取设置正在改变的scale的可见性
        /// 当这个scale在别的scales上面的时候，可见，直到移动到下面时才不可见
        /// </summary>
        double DynamicVisibilityScale { get; set; }

        /// <summary>
        /// 获取设置正在改变的exten的可见性
        /// 当这个extent在别的extents的上面，可见，直到移动到下面才会不可见
        /// </summary>
        MapWinGIS.Extents DynamicVisibilityExtents { get; set; }

        /// <summary>
        /// 指定是否使用DynamicVisibility
        /// </summary>
        bool UseDynamicVisibility { get; set; }

        /// <summary>
        /// 当DynamicVisibilityScale使用时，获取设置最小的scale
        /// </summary>
        double MinVisibleScale { get; set; }

        /// <summary>
        /// 当DynamicVisibilityScale使用时，获取设置最大的scale
        /// </summary>
        double MaxVisibleScale { get; set; }

        /// <summary>
        /// 重新加载在.lbl文件中的指定的label
        /// </summary>
        /// <param name="lblFilename">要为这个层加载的Label file</param>
        void ReloadLabels(string lblFilename);

        /// <summary>
        /// Deprecated.
        /// </summary>
        /// <param name="newValue">The new image to add.</param>
        /// <returns>The index for this image, to be passed to ShapePointImageListID or other functions.</returns>
        long UserPointImageListAdd(MapWinGIS.Image newValue);

        /// <summary>
        /// Deprecated.舍弃的方法
        /// </summary>
        /// <param name="ImageIndex">The image index to retrieve.</param>
        /// <returns>The index associated with this index; or null/nothing if nonexistant.</returns>
        MapWinGIS.Image UserPointImageListItem(long ImageIndex);

        /// <summary>
        /// Deprecated.舍弃的方法
        /// </summary>
        void ClearUDPointImageList();

        /// <summary>
        /// Deprecated.舍弃的方法
        /// </summary>
        /// <returns>image list的数量.</returns>
        long UserPointImageListCount();

        /// <summary>
        /// 指示是否在保存一个项目时略过这个层
        /// </summary>
        bool SkipOverDuringSave { get; set; }

        /// <summary>
        /// 是否略过这个层，当绘制legend时。
        /// </summary>
        bool HideFromLegend { get; set; }

        /// <summary>
        /// 为给定的polygon shapefile层，设置透明百分比 
        /// </summary>
        float ShapeLayerFillTransparency { get; set; }

        /// <summary>
        /// 为给定的image layer，设置透明百分比.
        /// </summary>
        float ImageLayerFillTransparency { get; set; }

        /// <summary>
        /// Deprecated.
        /// </summary>
        Interfaces.ShapefilePointImageScheme PointImageScheme { get; set; }

        /// <summary>
        /// 保存shapefile的rendering properties到指定目录
        /// 如果这个层是grid类型，则忽视
        /// </summary>
        /// <param name="saveToFilename">The filename.</param>
        /// <returns>True on success</returns>
        bool SaveShapeLayerProps(string saveToFilename);

        /// <summary>
        /// 保存shapefile的rendering properties到.mwsr文件中（与shapefile匹配的filename）
        /// 如果层是grid类型，则忽视并返回错误
        /// </summary>
        /// <returns>True on success</returns>
        bool SaveShapeLayerProps();

        /// <summary>
        /// 从指定的文件中加载shapefile rendering properties
        /// 层不是能是grid类型
        /// </summary>
        /// <param name="loadFromFilename">The filename.</param>
        /// <returns>True on success</returns>
        bool LoadShapeLayerProps(string loadFromFilename);

        /// <summary>
        /// 从.mwsr中加载shapefile rendering properties
        /// 如果文件没有找到，返回false
        /// 如果层是grid类型，则忽视并返回错误
        /// </summary>
        /// <returns>True on success</returns>
        bool LoadShapeLayerProps();

        /// <summary>
        /// 从指定的文件中加载shapefile目录
        /// Supports files created by Categories dialog (.mwleg extention).
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>True on success</returns>
        bool LoadShapefileCategories(string filename);

        /// <summary>
        /// 将shapefile保存到指定目录
        /// 指定shapefile层能用
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>True on success</returns>
        bool SaveShapefileCategories(string filename);

        /// <summary>
        /// Create or overwrite .mwsymb file
        /// </summary>
        /// <returns>True on success</returns>
        bool SaveOptions();

        /// <summary>Create or overwrite .mwsymb file</summary>
        /// <returns>True on success</returns>
        /// <param name="filename">The filename.</param>
        bool SaveOptions(string filename);

        /// <summary>Load .mwsymb file</summary>
        /// <returns>True on success</returns>
        bool LoadOptions();

        /// <summary>Load .mwsymb file</summary>
        /// <returns>True on success</returns>
        /// <param name="filename">The filename.</param>
        bool LoadOptions(string filename);

        /// <summary>
        /// 重写ToString方法，返回给定图层的基本信息
        /// </summary>
        string ToString();

        System.Drawing.Color ShapeLayerFillColor { get; set; }

        System.Drawing.Color ShapeLayerLineColor { get; set; }

        System.Drawing.Color ShapeLayerPointColor { get; set; }

        float ShapeLayerPointSize { get; set; }

        /// <summary>
        /// 获取层的选择项，如果没有shapefile层，则返回空。
        /// </summary>
        Interfaces.SelectInfo SelectedShapes { get; }

        /// <summary>
        /// 清空shapefile上选择的shape
        /// </summary>
        void ClearSelection();

        /// <summary>
        /// 获取与layer相关的对象，以便插件开发者存储可扩展类
        /// </summary>
        /// <param name="key">A key of object to return</param>
        /// <returns>对于指定的key不存在，返回null</returns>
        object GetCustomObject(string key);

        /// <summary>
        /// 设置与layer相关的对象，以便插件开发者存储可扩展类
        /// </summary>
        /// <param name="obj">Custom object</param>
        /// <param name="key">A key of object to access it later</param>
        void SetCustomObject(object obj, string key);
    }

    /// <summary>
    /// 在MainProgram中管理层的接口
    /// </summary>
    public interface Layers : IEnumerable<Interfaces.Layer>
    {
        /// <summary>
        /// 从地图中移除所有的层
        /// </summary>
        void Clear();

        /// <summary>
        /// 将一个层移动到另外一个位置或者组中
        /// </summary>
        /// <param name="Handle">要移动层的Handle</param>
        /// <param name="NewPosition">New position in the target group.</param>
        /// <param name="TargetGroup">Group to move the layer to.</param>
        void MoveLayer(int Handle, int NewPosition, int TargetGroup);

        /// <summary>
        /// 从MainProgram中移除指定的层
        /// </summary>
        /// <param name="LayerHandle"></param>
        void Remove(int LayerHandle);

        /// <summary>
        /// 如果指定层的handle属于一个有效层，true
        /// </summary>
        /// <param name="LayerHandle">Handle of the layer to check.</param>
        /// <returns>True if the layer handle is valid.  False otherwise.</returns>
        bool IsValidHandle(int LayerHandle);

        #region 添加层到MainProgram
        /// <summary>
        /// 默认加载层，显示一个文件对话框
        /// </summary>
        /// <returns>An array of <c>MapWinGIS.Interfaces.Layer</c> objects.</returns>
        Interfaces.Layer[] Add();

        /// <summary>
        /// 从指定文件名中加载一个层
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(string Filename);

        /// <summary>
        /// 从指定的文件中，添加一个指定名字的层
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(string Filename, string LayerName);

        /// <summary>
        /// 从指定的文件中，加载一个指定名字和投影的层
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <param name="layerName">The layer Name</param>
        /// <param name="geoProjection">The projection</param>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(string filename, string layerName, ref MapWinGIS.GeoProjection geoProjection);

        /// <summary>
        /// 添加一个Image类型的层到MainProgram中
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(ref MapWinGIS.Image ImageObject);

        /// <summary>
        /// 添加一个指定层名的Image类型层到MainProgram中
        /// </summary>
        /// <param name="ImageObject">Image类型的层对象</param>
        /// <param name="LayerName">加载的层的名字</param>
        /// <returns>Layer对象</returns>
        Interfaces.Layer Add(ref MapWinGIS.Image ImageObject, string LayerName);

        /// <summary>
        /// 添加一个Image类型的层到MainProgram中
        /// </summary>
        /// <param name="ImageObject">The image object</param>
        /// <param name="LayerName">Tha name of the layer</param>
        /// <param name="Visible">Visibility of the layer</param>
        /// <param name="TargetGroup">Add to which group, -1 means top group</param>
        /// <param name="LayerPosition">On what layer position, -1 means top position</param>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(ref MapWinGIS.Image ImageObject, string LayerName, bool Visible, int TargetGroup, int LayerPosition);


        /// <summary>
        /// Adds a <c>Shapefile</c> layer to the MapWinGIS.
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(ref MapWinGIS.Shapefile ShapefileObject);

        /// <summary>
        /// Adds a <c>Shapefile</c> layer to the MapWinGIS with the specified layer name.
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(ref MapWinGIS.Shapefile ShapefileObject, string LayerName);

        /// <summary>
        /// Adds a <c>Shapefile</c> layer to the MapWinGIS with the specified properties.
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(ref MapWinGIS.Shapefile ShapefileObject, string LayerName, int Color, int OutlineColor);

        /// <summary>
        /// Adds a <c>Shapefile</c> layer to the MapWinGIS with the specified properties.
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(ref MapWinGIS.Shapefile ShapefileObject, string LayerName, int Color, int OutlineColor, int LineOrPointSize);

        /// <summary>
        /// Adds a <c>Grid</c> layer to the MapWinGIS.
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(ref MapWinGIS.Grid GridObject);

        /// <summary>
        /// Adds a <c>Grid</c> layer to the MapWinGIS, with the specified coloring scheme.
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(ref MapWinGIS.Grid GridObject, MapWinGIS.GridColorScheme ColorScheme);

        /// <summary>
        /// Adds a <c>Grid</c> layer to the MapWinGIS with the specified layer name.
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(ref MapWinGIS.Grid GridObject, string LayerName);

        /// <summary>
        /// Adds a <c>Grid</c> object to the MapWinGIS with the specified properties.
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(ref MapWinGIS.Grid GridObject, MapWinGIS.GridColorScheme ColorScheme, string LayerName);

        #endregion

        /// <summary>
        /// Gets or sets the current layer handle.
        /// </summary>
        int CurrentLayer { get; set; }

        /// <summary>
        /// 根据图层在图层列表中的位置获取一个图层的句柄
        /// </summary>
        /// <param name="GlobalPosition">Position in the layers list, disregarding groups.</param>
        /// <returns>The handle of the layer at the specified position, or -1 if no layer is found there.</returns>
        int GetHandle(int GlobalPosition);

        /// <summary>
        /// 索引，根据LayerHandle获取指定的Layer
        /// </summary>
        Interfaces.Layer this[int LayerHandle] { get; }

        /// <summary>
        /// 获得所有已经加载到MainProgram中的层的数量，正在绘制的不算
        /// </summary>
        int NumLayers { get; }

        /// <summary>
        /// 用指定的GridColorScheme新建一个grid layer
        /// </summary>
        /// <param name="LayerHandle">Handle of the grid layer.</param>
        /// <param name="GridObject">Grid object corresponding to that layer handle.</param>
        /// <param name="ColorScheme">Coloring scheme to use when rebuilding.</param>
        /// <returns></returns>
        bool RebuildGridLayer(int LayerHandle, MapWinGIS.Grid GridObject, MapWinGIS.GridColorScheme ColorScheme);

        /// <summary>
        /// 获取在legend中的组的属性
        /// </summary>
        Interfaces.Groups Groups { get; }

        /// <summary>
        /// 通过filename添加一个层，名字指定，legend可见
        /// </summary>
        /// <returns>Returns a <c>Layer</c> object.</returns>
        Interfaces.Layer Add(string Filename, string LayerName, bool VisibleInLegend);

        /// <summary>
        /// 添加一个层到map中，层的位置是当前选择的层的上面，否则就是层列表的顶部
        /// </summary>
        /// <param name="Visible">添加的层是否可见</param>
        /// <param name="PlaceAboveCurrentlySelected">是将层放在当前选择的上面，还是顶部</param>
        /// <param name="Filename">The name of the file to add.</param>
        /// <param name="LayerName">层的名字，将显示在legend中</param>
        Interfaces.Layer Add(string Filename, string LayerName, bool Visible, bool PlaceAboveCurrentlySelected);

        /// <summary>
        /// 开始添加层对话框（Session）
        /// 在此期间，Projection MisMatch会检查，添加的层是否匹配。并且map,、legend 、buttons不会更新
        /// </summary>
        void StartAddingSession();

        /// <summary>
        /// 停止添加对话框（Session）
        /// </summary>
        void StopAddingSession();

        /// <summary>
        /// 停止添加对话框（Session）
        /// </summary>
        /// <param name="zoomToExtents">是否显示全图</param>
        void StopAddingSession(bool zoomToExtents);
    }

    /// <summary>
    /// 与MainProgram相关的数据
    /// </summary>
    public interface AppInfo
    {
        /// <summary>
        /// 显示在帮助菜单中的帮助路径
        /// </summary>
        string HelpFilePath { get; set; }

        /// <summary>
        /// 在应用开始时，是否显示一个splash screen（启动界面）
        /// </summary>
        bool UseSplashScreen { get; set; }

        /// <summary>
        /// 欢迎界面的名字
        /// </summary>
        string WelcomePlugin { get; set; }

        /// <summary>
        /// 显示在启动界面上的图像
        /// </summary>
        System.Drawing.Image SplashPicture { get; set; }

        /// <summary>
        /// 默认窗体的图标
        /// </summary>
        System.Drawing.Icon FormIcon { get; set; }

        /// <summary>
        /// 启动界面的显示时间
        /// </summary>
        double SplashTime { get; set; }

        /// <summary>
        /// 文件对话框的默认目录
        /// </summary>
        string DefaultDir { get; set; }

        /// <summary>
        /// 在帮助->关于 对话框的URL
        /// </summary>
        string URL { get; set; }

        /// <summary>
        /// 主窗体的名字
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// 是否显示启动界面
        /// </summary>
        bool ShowWelcomeScreen { get; set; }

        /// <summary>
        /// 是否显示MainProgram的版本
        /// 之前是ShowMapWindowVersion
        /// </summary>
        bool ShowMapWinGISVersion { get; set; }

        /// <summary>
        /// 是否显示FloatingScalebar（浮动比例尺）
        /// </summary>
        bool ShowFloatingScalebar { get; set; }

        /// <summary>
        /// 是否显示redraw speed标签
        /// </summary>
        bool ShowRedrawSpeed { get; set; }

        /// <summary>
        /// 在加载层数据时，定义加载可视化选项的行为
        /// Defines behavior for loading visualazation options while loading data layer
        /// </summary>
        SymbologyBehavior SymbologyLoadingBehavior { get; set; }

        /// <summary>
        /// List of EPSG codes for favorite projections
        /// </summary>
        List<int> FavoriteProjections { get; }

        /// <summary>
        /// 当添加的层没有投影时，定义应用程序的行为
        /// </summary>
        ProjectionAbsenceBehavior ProjectionAbsenceBehavior { get; set; }

        /// <summary>
        /// 当添加的层的投影不匹配于已添加的层的投影时，定义应该程序行为
        /// Defines application behavior when projection of the layer being added is different from project one
        /// </summary>
        ProjectionMismatchBehavior ProjectionMismatchBehavior { get; set; }

        /// <summary>
        /// 获取设置一个值，指示在投影不匹配的情况下，是否显示投影对话框
        /// </summary>
        bool NeverShowProjectionDialog { get; set; }

        /// <summary>
        /// 获取设置一个值，指示在加载一个层之后，投影不匹配的情况下是否显示加载报告
        /// </summary>
        bool ShowLoadingReport { get; set; }
    }

    /// <summary>
    /// 项目改变事件发生时的委托
    /// </summary>
    public delegate void ProjectionChangedDelegate(MapWinGIS.GeoProjection oldProjection, MapWinGIS.GeoProjection newProjection);

    public delegate void OnUpdateTableJoinDelegate(String filename, String fieldNames, String joinOptions, MapWinGIS.Table tableToFill);

    /// <summary>
    /// 管理在MainProgram中的项目和配置目录
    /// </summary>
    public interface Project
    {
        /// <summary>
        /// 项目投影改变时发生的事件
        /// </summary>
        event OnUpdateTableJoinDelegate OnUpdateTableJoin;

        /// <summary>
        /// 项目投影改变时发生的事件
        /// </summary>
        event ProjectionChangedDelegate ProjectionChanged;

        /// <summary>
        /// 获取包含当前MainProgram项目的配置文件的文件名完整路径
        /// </summary>
        string ConfigFileName { get; }

        /// <summary>
        /// 当前项目的文件名
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// 复制当前项目并保存
        /// </summary>
        bool Save(string Filename);

        /// <summary>
        /// 复制当前项目并保存
        /// </summary>
        /// <param name="Filename">要保存的项目文件路径</param>
        bool SaveCopy(string Filename);

        /// <summary>
        /// 从项目文件中加载一个符号（symbology），到指定的层
        /// </summary>
        /// <param name="filename">包含项目文件名的完整路径</param>
        /// <param name="handle">要加载symbology的图层的handle</param>
        bool LoadLayerSymbologyFromProjectFile(string Filename, int Handle);

        /// <summary>
        /// 加载一个项目文件
        /// </summary>
        /// <param name="Filename">Filename of the project to load.</param>
        /// <returns>Returns true if successful.</returns>
        bool Load(string Filename);

        /// <summary>
        /// 加载一个项目文件到当前项目中，作为一个子group
        /// </summary>
        /// <param name="filename">要加载的完整项目路径</param>
        void LoadIntoCurrentProject(string Filename);

        /// <summary>
        /// 保存当前配置到配置文件中
        /// </summary>
        /// <param name="Filename">保存配置文件的文件路径</param>
        bool SaveConfig(string Filename);

        /// <summary>
        /// 获取或设置项目文件是否修改
        /// </summary>
        bool Modified { get; set; }

        /// <summary>
        /// 返回或者设置当前项目投影
        /// 格式：+proj=tmerc +ellps=WGS84  +datum=WGS84
        /// </summary>
        string ProjectProjection { get; set; }

        /// <summary>
        /// 此处触发ProjectionChanged事件
        /// 获取或设置项目投影
        /// </summary>
        MapWinGIS.GeoProjection GeoProjection { get; set; }

        /// <summary>
        /// 该项目的指定的配置文件是否已经加载了
        /// </summary>
        bool ConfigLoaded { get; }

        /// <summary>
        /// 返回ArrayList数组，存储最近的项目(完整路径)
        /// </summary>
        System.Collections.ArrayList RecentProjects { get; }

        /// <summary>
        /// 当前项目的单位，格式："Meters", "Feet"
        /// </summary>
        string MapUnits { get; set; }

        /// <summary>
        /// 获取或设置当前更替的显示单位，格式："Meters", "Feet"
        /// </summary>
        string MapUnitsAlternate { get; set; }

        /// <summary>
        /// 保存shape的设置
        /// </summary>
        bool SaveShapeSettings { get; set; }
    }
/************************************************************************************************/
    /// <summary>
    /// 所有接口的根接口
    /// 其他接口都是为此接口服务的
    /// </summary>
    public interface IMapWin
    {
        /// <summary>
        /// 刷新宿主程序地图的显示
        /// </summary>
        void Refresh();

        /// <summary>
        /// 获取最后一条错误信息. 
        /// 任何时候都能设置这个错误信息
        /// </summary>
        string LastError { get; }

        /// <summary>
        /// 返回层对象，管理
        /// </summary>
        Interfaces.Layers Layers { get; }

        /// <summary>
        /// 返回视图（View）对象，处理地图视图
        /// </summary>
        Interfaces.View View { get; }

        /// <summary>
        /// 返回菜单对象，管理菜单
        /// </summary>
        Interfaces.Menus Menus { get; }

        /// <summary>
        /// 返回插件对象，管理插件
        /// </summary>
        Interfaces.Plugins Plugins { get; }

        /// <summary>
        /// 放回地图预览(PreviewMap)对象，管理预览地图
        /// </summary>
        Interfaces.PreviewMap PreviewMap { get; }

        /// <summary>
        /// 返回legendPanel对象，管理legend panel
        /// </summary>
        Interfaces.LegendPanel LegendPanel { get; }

        /// <summary>
        /// 返回状态栏对象，管理状态栏
        /// </summary>
        Interfaces.StatusBar StatusBar { get; }

        /// <summary>
        /// 返回工具条，管理工具条
        /// </summary>
        Interfaces.Toolbar Toolbar { get; }

        /// <summary>
        /// 为方法和属性提供入口，得到一个报告
        /// Provides access to report generation methods and properties. 
        /// </summary>
        Interfaces.Reports Reports { get; }

        /// <summary>
        /// 为项目和配置文件，提供管理
        /// Provides control over project and configuration files.
        /// </summary>
        Interfaces.Project Project { get; }

        /// <summary>
        /// 为App ，提供控制
        /// Provides control over application-level settings like the app name.
        /// </summary>
        Interfaces.AppInfo ApplicationInfo { get; }

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="ex">抛出的异常</param>
        void ShowErrorDialog(System.Exception ex);

        /// <summary>
        /// 显示错误对话框，并发送到指定的地址
        /// </summary>
        /// <param name="ex">抛出的异常</param>
        /// <param name="sendEmailTo">要发送的地址</param>
        void ShowErrorDialog(System.Exception ex, string sendEmailTo);

        /// <summary>
        /// 显示在App名字后面的，对话框标题，覆盖默认项目名标题
        /// Sets the dialog title to be displayed after the "AppInfo" name for the main window.
        /// </summary>
        void SetCustomWindowTitle(string newTitleText);

        /// <summary>
        /// 显示默认项目名标题
        /// Returns dialog title for the main window to the default "project name" title.
        /// </summary>
        void ClearCustomWindowTitle();

        /// <summary>
        /// 指定是否用绝对文件路径
        /// Specify whether the full project path should be specified rather than just filename, in title bar for main window.
        /// </summary>
        bool DisplayFullProjectPath { set; }

        /// <summary>
        /// 让用户选择一个投影，并且返回PROJ4格式的代替这个投影
        /// 指定对话框的标题和一个可选择的的默认投影
        /// </summary>
        /// <param name="dialogCaption">The text to be displayed on the dialog, e.g. "Please select a projection."</param>
        /// <param name="defaultProjection">The PROJ4 projection string of the projection to default to, "" for none.</param>
        /// <returns></returns>
        string GetProjectionFromUser(string dialogCaption, string defaultProjection);

        /// <summary>
        /// 在宿主程序的右下角为用户提供panel
        /// </summary>
        Interfaces.UIPanel UIPanel { get; }

        /// <summary>
        /// 用户交互，促使用输入内容
        /// </summary>
        Interfaces.UserInteraction UserInteraction { get; }

        /// <summary>
        /// MapWinGIS用户控件的高级操作
        /// Returns the underlying MapWinGIS activex control for advanced operations.
        /// </summary>
        object GetOCX { get; }

        void RefreshDynamicVisibility();

        /// <summary>
        /// 返回指向工具盒的对象
        /// </summary>
        Interfaces.IGisToolBox GisToolbox { get; }

        /// <summary>
        /// 获取投影数据库
        /// </summary>
        Interfaces.IProjectionDatabase ProjectionDatabase { get; }

        /// <summary>
        /// 与层关联的自定义对象从项目加载时触发的事件
        /// Event raised when state of custom object associated with layer is read from project
        /// </summary>
        event CustomObjectLoadedDelegate CustomObjectLoaded;

        /// <summary>
        /// shapefile层对象的选择发生改变时触发的事件
        /// </summary>
        event LayerSelectionChangedDelegate LayerSelectionChanged;

        /// <summary>
        /// 当一个项目有MainProgram加载完毕后触发的事件
        /// </summary>
        event ProjectLoadedDelegate ProjectLoaded;
    }
/************************************************************************************************/

    /// <summary>
    /// CustomObjectLoaded事件的委托
    /// </summary>
    /// <param name="layerHandle">加载层的handle</param>
    /// <param name="key">？</param>
    /// <param name="state">？？</param>
    /// <param name="handled">？</param>
    public delegate void CustomObjectLoadedDelegate(int layerHandle, string key, string state, ref bool handled);

    /// <summary>
    /// 层选择改变事件触发事件的委托
    /// </summary>
    public delegate void LayerSelectionChangedDelegate(int layerHandle, ref bool handled);

    /// <summary>
    /// 项目加载事件的委托
    /// </summary>
    /// <param name="projectName">The name of the project loaded</param>
    /// <param name="errors">Are there errors during loading</param>
    public delegate void ProjectLoadedDelegate(string projectName, bool errors);

    /// <summary>
    /// 用户交互功能，让用户输入内容,选择投影，和颜色
    /// </summary>
    public interface UserInteraction
    {
        /// <summary>
        /// 让用户选择一个项目，并且返回一个PROJ4类型文件替代这个加载项目
        /// 指定对话框的Caption和项目默认选项
        /// </summary>
        /// <param name="dialogCaption">选择投影对话框的标题</param>
        /// <param name="defaultProjection">默认投影，""表示无</param>
        /// <returns></returns>
        string GetProjectionFromUser(string dialogCaption, string defaultProjection);

        /// <summary>
        /// 由用户定义的开始和结束色
        /// </summary>
        /// <param name="suggestedStart">初始化默认选择的开始颜色</param>
        /// <param name="suggestedEnd">初始化默认选择的结束颜色</param>
        /// <param name="selectedEnd">用户选择的结束色</param>
        /// <param name="selectedStart">用户选择的开始色</param>
        /// <returns></returns>
        bool GetColorRamp(System.Drawing.Color suggestedStart, System.Drawing.Color suggestedEnd, out System.Drawing.Color selectedStart, out System.Drawing.Color selectedEnd);
    }

    /// <summary>
    /// 当要在MainProgram的数据中产生一个报告时用
    /// The <c>Reports</c> contains tools that are useful when generating a report from the data in the MapWinGIS.
    /// </summary>
    public interface Reports
    {
        /// <summary>
        /// 返回一个（north arrow）指北针的图像
        /// </summary>
        System.Drawing.Image GetNorthArrow();

        /// <summary>
        /// 在指定的范围返回一个MapWinGIS.Image类型的对象的视角
        /// </summary>
        /// <param name="boundBox">The area that you wish to take the picture of.  Uses projected map units.</param>
        MapWinGIS.Image GetScreenPicture(MapWinGIS.Extents boundBox);

        /// <summary>
        /// 当legend中不只有一个层时，返回一张高质量的闪照图片
        /// </summary>
        /// <param name="layerHandle">Handle of the layer to take a snapshot of.</param>
        /// <param name="width">Maximum width of the image.  The height of the image depends on the coloring scheme of the layer.</param>
        /// <param name="columns">The number of columns to generate</param>
        /// <param name="fontFamily">Font family</param>
        /// <param name="minFontSize">Minimum font size</param>
        /// <param name="maxFontSize">Maximum Font size</param>
        /// <param name="underlineLayerTitles"></param>
        /// <param name="boldLayerTitles"></param>
        /// <returns></returns>
        System.Drawing.Image GetLegendSnapshotHQ(int layerHandle, int width, int columns, string fontFamily, int minFontSize, int maxFontSize, bool underlineLayerTitles, bool boldLayerTitles);

        /// <summary>
        /// 返回legend中的一个指定的层和其中的color breaks的一张高质量的闪照图片
        /// </summary>
        /// <param name="layerHandle">Handle of the layer to take a snapshot of.</param>
        /// <param name="category">The color break to use</param>
        /// <param name="width">Width in pixels of the box to create</param>
        /// <param name="height">Height in pixels of the box to create</param>
        /// <returns></returns>
        System.Drawing.Image GetLegendSnapshotBreakHQ(int layerHandle, int category, int width, int height);

        /// <summary>
        /// 返回legend中的一张图片
        /// </summary>
        /// <param name="VisibleLayersOnly">指定只截取可见的层的部分</param>
        /// <param name="imgWidth">图片最大宽度，高度取决于已加载层的数量</param>
        System.Drawing.Image GetLegendSnapshot(bool visibleLayersOnly, int imgWidth);

        /// <summary>
        /// 类似于GetLegendSnapshot，此时不应只只考虑一个layer在内
        /// </summary>
        /// <param name="layerHandle">Handle of the layer to take a snapshot of.</param>
        /// <param name="imgWidth">图片最大宽度，高度取决于已加载层的颜色配置</param>
        System.Drawing.Image GetLegendLayerSnapshot(int layerHandle, int imgWidth);

        /// <summary>
        /// 包含精确比例尺的图片
        /// </summary>
        /// <param name="mapUnits">采用的地图的单位</param>
        /// <param name="scalebarUnits">显示在比例尺上的尺量单位，能够转换成地图的任何单位</param>
        /// <param name="maxWidth">含比例尺照片最大宽度</param>
        System.Drawing.Image GetScaleBar(UnitOfMeasure mapUnits, UnitOfMeasure scalebarUnits, int maxWidth);

        /// <summary>
        /// 包含精确比例尺的图片
        /// </summary>
        /// <param name="mapUnits">采用的地图的单位</param>
        /// <param name="scalebarUnits">显示在比例尺上的尺量单位，能够转换成地图的任何单位</param>
        /// <param name="maxWidth">含比例尺图片最大宽度</param>
        System.Drawing.Image GetScaleBar(string mapUnits, string scalebarUnits, int maxWidth);
    }

    /// <summary>
    /// 允许用户插件使用在MainProgram底部的容器（panel）
    /// </summary>
    public interface UIPanel
    {
        /// <summary>
        /// 返回一个能够用作添加可停靠内容（dockable content）到MainProgram的系统panel。
        /// 先创建一个可停靠Form，再在Form里面添加一个Panel，返回这个Panel给调用者。
        /// </summary>
        System.Windows.Forms.Panel CreatePanel(string caption, MapWinGISDockStyle dockStyle);

        /// <summary>
        /// 返回一个能够用作添加可停靠内容（dockable content）到MainProgram的系统panel。
        /// 先创建一个可停靠Form，再在Form里面添加一个Panel，返回这个Panel给调用者。
        /// </summary>
        System.Windows.Forms.Panel CreatePanel(string caption, System.Windows.Forms.DockStyle dockStyle);

        /// <summary>
        /// 删除指定的panel
        /// </summary>
        void DeletePanel(string caption);

        /// <summary>
        /// 如果一个panel没有必要删除，可以设置它的可见性
        /// visible参数不再起作用
        /// </summary>
        void SetPanelVisible(string caption, bool visible);

        /// <summary>
        /// 将指定Panel中要注销的句柄添加到m_OnCloseHandlers中
        /// 一般在往Panel里面添加句柄时，就应该调用该函数添加
        /// </summary>
        void AddOnCloseHandler(string caption, Interfaces.OnPanelClose onCloseFunction);
    }


    #endregion


    #region GIS工具盒

    /// <summary>
    /// 工具选择和工具点击事件的委托
    /// </summary>
    /// <param name="tool">指向被选择的工具的引用</param>
    /// <param name="handled">通知调用的程序，这个事件已经处理</param>
    public delegate void ToolSelectedDelegate(IGisTool tool, ref bool handled);

    /// <summary>
    /// 组选择事件的委托
    /// </summary>
    /// <param name="tool">指向被选择的组的引用</param>
    /// <param name="handled">通知调用的程序，这个事件已经处理</param>
    public delegate void GroupSelectedDelegate(IGisToolboxGroup group, ref bool handled);

    /// <summary>
    /// 保存从geoprocessing 工具盒中得到的工具信息
    /// </summary>
    public interface IGisTool
    {
        /// <summary>
        /// 工具名
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 工具描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 工具关键字
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// 存储与工具相关的额外信息
        /// </summary>
        object Tag { get; set; }
    }

    /// <summary>
    /// 保存从geoprocessing 工具盒中得到的工具信息，但是必须由GisToolbox.CreateGroup创建的
    /// </summary>
    public interface IGisToolboxGroup
    {
        /// <summary>
        /// 工具名
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 工具描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 存储与工具相关的额外信息
        /// </summary>
        object Tag { get; set; }

        /// <summary>
        /// 在groups中的工具列表
        /// </summary>
        IGisTools Tools { get; }

        /// <summary>
        /// 在组中的子组的列表
        /// </summary>
        IGisToolboxGroups SubGroups { get; }

        /// <summary>
        /// 组的扩展状态
        /// </summary>
        bool Expanded { get; set; }
    }

    /// <summary>
    /// 有MainProgram工具盒提供的方法和属性
    /// </summary>
    public interface IGisToolBox
    {
        /// <summary>
        /// 由toolbox提供的组的列表
        /// </summary>
        IGisToolboxGroups Groups { get; }

        /// <summary>
        /// 将所有tool存储在IEnumerable中
        /// </summary>
        IEnumerable<IGisTool> Tools { get; }

        /// <summary>
        /// 创建一个新的GisTool类的实例对象
        /// </summary>
        IGisTool CreateTool(string name, string key);

        /// <summary>
        /// 创建一个新的GisToolboxGroup类的实例对象
        /// </summary>
        IGisToolboxGroup CreateGroup(string name, string key);

        /// <summary>
        /// 事件，当tool被选择时触发
        /// </summary>
        event ToolSelectedDelegate ToolSelected;

        /// <summary>
        /// 事件，当用户想去执行该tool时触发
        /// </summary>
        event ToolSelectedDelegate ToolClicked;

        /// <summary>
        /// 事件，当组被选择时触发
        /// </summary>
        event GroupSelectedDelegate GroupSelected;

        /// <summary>
        /// 展开所有的组
        /// </summary>
        void ExpandGroups(int level);
    }

    /// <summary>
    /// 存储groups的容器
    /// 没有成员
    /// </summary>
    public interface IGisToolboxGroups : ICollection<IGisToolboxGroup>
    {
        // no members
    }

    /// <summary>
    /// 存储tools的容器
    /// 没有成员
    /// </summary>
    public interface IGisTools : ICollection<IGisTool>
    {
        // no members
    }

    /// <summary>
    /// 为投影数据库类提供入口的接口
    /// 没有成员
    /// </summary>
    public interface IProjectionDatabase
    {
        // no members, is needed to pass reference only
    }
    #endregion

}
