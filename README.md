## 介绍
开源桌面GIS软件MapWindow 4.x采用VB语音写的桌面。xxx原因,加上一腔热情,不眠不休，将MapWindow 用C#进行重构，并优化其中很多的细节。从开源中来，到开源中去！

Tip:重构这个开源软件时还未在开源在github上,且不知道如何反馈bug,提交分支,也没有做SEO!现在MapWindow5出来了,且用C#重构的!
遗憾的是MapWindow 4.x未在Github开源!

源软件链接：http://mapwindow4.codeplex.com/

## 使用
1. 本程序是一个桌面程序，底层依赖MapWinGIS.ocx。
2. 安装MapWinGIS.ocx。最简单的方法先安装MapWindow 4.x(即：MapWindowx86Full-v488SR-installer.exe)。
3. 运行代码中bin目录下的MapWinGIS.exe。

## 项目介绍
1. MapWinGIS.MainProgram 桌面程序主目录，源代码目录。
2. MapWinGIS.Utility，MapWinGIS.GeoProcess，MapWinGIS.Controls插件。
3. MapWinGIS.Interfaces插件接口。
5. IDE Visual Studio 10 下面开发。

## 目的
1. 对源软件进行了一次重构升级,修复若干bug,对UI进行升级优化，使得软件用起来更加顺手。
2. 可以作为学习插件式程序开发的一个参考,深入理解插件式程序开发的底层原理，且添加了比较详细的注释说明。
3. 可以作为学习用C#写桌面程序的一个参考。
4. 学习GIS桌面软件开发,学习基于GIS软件二次开发的原理!
