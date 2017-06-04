using System;
using System.IO;
using Microsoft.VisualBasic;

namespace MapWinGIS.MainProgram
{
    public class SuperGrid :MapWinGIS.GridClass
    {
        ~SuperGrid()
        {
            try
            {
                base.Close();
            }
            catch
            {
            }
        }

        public struct sVertex
        {
            public double X;
            public double Y;
        }

        public struct sDEMData
        {
            public int HorizUnits;
            public int VertUnits;
            public string Notes;
            public sVertex[] Vertices;
            public double Min;
            public double Max;
            public int NumCols;
            public int[,] Values;
            public sVertex[] ColStarts;
            public int[] NumElevs;
            public double MaxY()
            {
                int i;
                double Y;
                Y = Vertices[0].Y;
                for (i = 0; i <= 3; i++)
                {
                    if (Vertices[i].Y > Y)
                    {
                        Y = Vertices[i].Y;
                    }
                }
                return Y;
            }
            public double MaxX()
            {
                int i;
                double X;
                X = Vertices[0].X;
                for (i = 0; i <= 3; i++)
                {
                    if (Vertices[i].X > X)
                    {
                        X = Vertices[i].X;
                    }
                }
                return X;
            }
            public double MinX()
            {
                int i;
                double X;
                X = Vertices[0].X;
                for (i = 0; i <= 3; i++)
                {
                    if (Vertices[i].X < X)
                    {
                        X = Vertices[i].X;
                    }
                }
                return X;
            }
            public double MinY()
            {
                int i;
                double Y;
                Y = Vertices[0].Y;
                for (i = 0; i <= 3; i++)
                {
                    if (Vertices[i].Y < Y)
                    {
                        Y = Vertices[i].Y;
                    }
                }
                return Y;
            }

        }

        public struct sFLTData
        {
            public int ncols; //2199
            public int nrows; //1861
            public double xllcenter; //-120.36583333496
            public double yllcenter; //46.992222223902
            public double cellsize; //0.0002777777778
            public int NODATA_value; //-9999
            public string byteorder; //LSBFIRST
            public float[,] Values;
        }

        public override bool SetInvalidValuesToNodata(double MinThresholdValue, double MaxThresholdValue)
        {
            return base.SetInvalidValuesToNodata(MinThresholdValue, MaxThresholdValue);
        }

        /// <summary>
        /// 导入.flt格式的文件
        /// </summary>
        private bool ImportFLTFormat(string InFile, ref MapWinGIS.ICallback Callback)
        {
            sFLTData FLTData = new sFLTData();
            try
            {
                ReadHDRFile(FLTData, InFile.Substring(0, InFile.LastIndexOf(".")) + ".hdr");
                ReadFLTData(FLTData, InFile, Callback);
                MakeGrid(FLTData, InFile, Callback);
                return true;
            }
            catch (System.Exception ex)
            {
                MapWinGIS.Utility.Logger.Message("导入.flt格式的文件出现错误," + InFile + "\r\n" + ex.Message);
                return false;
            }
        }

        public override bool Resource(string newSrcPath)
        {
            return base.Resource(newSrcPath);
        }

        private void ReadHDRFile(sFLTData FLTData, string HeaderFile)
        {
            int FileNum;
            string OneLine;
            bool isCorner = false;

            FileNum = FileSystem.FreeFile();
            FileSystem.FileOpen(FileNum, HeaderFile, OpenMode.Input, OpenAccess.Read);
            while (!(FileSystem.EOF(FileNum)))
            {
                OneLine = Strings.LCase((string)(FileSystem.LineInput(FileNum)));
                if (OneLine.IndexOf("ncols") + 1 > 0)
                {
                    int.TryParse((string)(OneLine.Replace("ncols", "").Replace("\t", "").Replace(" ", "")), out FLTData.ncols);
                }
                if (OneLine.IndexOf("nrows") + 1 > 0)
                {
                    int.TryParse((string)(OneLine.Replace("nrows", "").Replace("\t", "").Replace(" ", "")), out FLTData.nrows);
                }
                if (OneLine.IndexOf("xllcorner") + 1 > 0)
                {
                    isCorner = true;
                    double.TryParse(OneLine.Replace("xllcorner", "").Replace("\t", "").Replace(" ", ""), out FLTData.xllcenter);
                }
                if (OneLine.IndexOf("yllcorner") + 1 > 0)
                {
                    isCorner = true;
                    double.TryParse(OneLine.Replace("yllcorner", "").Replace("\t", "").Replace(" ", ""), out FLTData.yllcenter);
                }
                if (OneLine.IndexOf("xllcenter") + 1 > 0)
                {
                    double.TryParse(OneLine.Replace("xllcenter", "").Replace("\t", "").Replace(" ", ""), out FLTData.xllcenter);
                }
                if (OneLine.IndexOf("yllcenter") + 1 > 0)
                {
                    double.TryParse(OneLine.Replace("yllcenter", "").Replace("\t", "").Replace(" ", ""), out FLTData.yllcenter);
                }
                if (OneLine.IndexOf("cellsize") + 1 > 0)
                {
                    string tStr;
                    double tDbl;
                    tStr = OneLine.Substring(OneLine.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }));
                    double.TryParse(tStr, out tDbl);
                    tDbl = Math.Round(tDbl, 13);
                    FLTData.cellsize = tDbl;
                }
                if (OneLine.IndexOf("nodata_value") + 1 > 0)
                {
                    double t = (double)FLTData.NODATA_value;
                    double.TryParse(OneLine.Replace("nodata_value", "").Replace(" ", "").Replace("\t", ""), out t);
                    FLTData.NODATA_value = (int)t;
                }
                if (OneLine.IndexOf("byteorder") + 1 > 0)
                {
                    FLTData.byteorder = (string)(OneLine.Replace("byteorder", "").Replace(" ", "").Replace("\t", ""));
                }
            }

            if (isCorner)
            {
                FLTData.xllcenter += FLTData.cellsize / 2;
                FLTData.yllcenter += FLTData.cellsize / 2;
            }

            FileSystem.FileClose(FileNum);
        }

        private void ReadFLTData(sFLTData FLTData, string DataFile, MapWinGIS.ICallback Callback)
        {
            //读取.flt网格图层的数据文件
            int i, j;
            FileStream fs = null;
            BinaryReader r;
            try
            {
                FLTData.Values = new float[FLTData.ncols - 1 + 1, FLTData.nrows - 1 + 1];

                fs = File.Open(DataFile, FileMode.Open);
                r = new BinaryReader(fs);
                fs.Seek(0, SeekOrigin.Begin);
                for (j = 0; j < FLTData.nrows; j++)
                {
                    for (i = 0; i < FLTData.ncols; i++)
                    {
                        FLTData.Values[i, j] = r.ReadSingle();
                    }
                    if (Callback != null)
                    {
                        Callback.Progress(base.Key, j / FLTData.nrows * 50, "读取 FLT 数据");
                    }

                }
            }
            catch (System.Exception ex)
            {
                MapWinGIS.Utility.Logger.Message("读取.flt网格图层的数据文件出错: " + "\r\n" + ex.Message);
            }

            try
            {
                fs.Close();
            }
            catch
            {
            }
        }

        // 将二进制的Surfer7转换成ASCII形式的网格文件
        public bool grd2asc(string sGridfile, string sAscfile, ref string errMsg)
        {
            bool bResult = false;
            System.IO.TextWriter lAscGridWriter;
            double[] arr = null;
            long x, y;
            int lFileBinaryRead;
            int nRows = 0, nCols = 0;
            long lDataLength = 0;
            double xLL = 0, yLL = 0;
            double xSize = 0, ySize = 0;
            double blankValue = 0;
            const string cNoDataValue = "-99";
            long lMaxPoints;
            long lMinPoints;
            string sRegel;
            string sCellWaarde;
            double dblTempValue;
            try
            {
                bResult = false;

                lFileBinaryRead = FileSystem.FreeFile();
                FileSystem.FileOpen(lFileBinaryRead, sGridfile, OpenMode.Binary);

                FileSystem.FileGet(lFileBinaryRead, ref nRows, 21);
                FileSystem.FileGet(lFileBinaryRead, ref nCols, 25);
                FileSystem.FileGet(lFileBinaryRead, ref xLL, 29);
                FileSystem.FileGet(lFileBinaryRead, ref yLL, 37);
                FileSystem.FileGet(lFileBinaryRead, ref xSize, 45);
                FileSystem.FileGet(lFileBinaryRead, ref ySize, 53);
                FileSystem.FileGet(lFileBinaryRead, ref blankValue, 85);

                lDataLength = (nRows * nCols) - 1;
                arr = new double[lDataLength + 1];
                Array tArr = arr;
                long tl = (long)101;
                FileSystem.FileGet(lFileBinaryRead, ref tArr, tl);

                FileSystem.FileClose(lFileBinaryRead);

                try
                {
                    FileSystem.Kill(sAscfile);
                }
                catch
                {
                }

                lAscGridWriter = new System.IO.StreamWriter(sAscfile);

                //Write header:
                //NCOLS  672
                lAscGridWriter.WriteLine("NCOLS" + "\t" + (nCols).ToString());
                //NROWS  464
                lAscGridWriter.WriteLine("NROWS" + "\t" + (nRows).ToString());
                //XLLCENTER  500010
                lAscGridWriter.WriteLine("XLLCENTER" + "\t" + (xLL).ToString());
                //YLLCENTER  4830030
                lAscGridWriter.WriteLine("YLLCENTER" + "\t" + (yLL).ToString());
                lAscGridWriter.WriteLine("DX" + "\t" + (xSize).ToString());
                lAscGridWriter.WriteLine("DY" + "\t" + (ySize).ToString());
                //NODATA_VALUE -99
                lAscGridWriter.WriteLine("NODATA_VALUE" + "\t" + cNoDataValue);

                //The data:
                lMaxPoints = arr.Length - 1;
                lMinPoints = 0;
                //Read from bottom to top:
                //Per row:
                for (x = lMaxPoints - nCols; x >= lMinPoints - nCols; x += -1 * nCols)
                {
                    sRegel = "";
                    for (y = 1; y <= nCols; y++)
                    {
                        //x is start of row:
                        dblTempValue = System.Convert.ToDouble(arr[x + y]);
                        if (dblTempValue < blankValue)
                        {
                            sCellWaarde = Math.Round(dblTempValue, 4).ToString();
                        }
                        else
                        {
                            sCellWaarde = cNoDataValue;
                        }
                        if (sRegel == "")
                        {
                            sRegel = sCellWaarde;
                        }
                        else
                        {
                            sRegel = sRegel + "\t" + sCellWaarde;
                        }
                    }
                    //Compler row, so write it down:
                    lAscGridWriter.WriteLine(sRegel);
                }

                lAscGridWriter.Close();

                bResult = true;
            }
            catch (Exception ex)
            {
                if (errMsg != null)
                {
                    errMsg = ex.ToString();
                }
            }
            finally
            {
                //Clean up arrays:
                arr = null;
                arr = new double[1];
            }
            return bResult;
        }

        //导入.dem格式的文件
        private bool ImportDEMFormat(string InFile, ref MapWinGIS.ICallback Callback)
        {
            Stream FileStream;
            sDEMData DEMData = new sDEMData();
            try
            {
                FileStream = System.IO.File.Open(InFile, FileMode.Open, FileAccess.Read);
                if (!(FileStream == null))
                {
                    DEMData = ReadDEMData(FileStream, Callback);
                    FileStream.Close();
                    FileStream.Dispose();
                    MakeGrid(DEMData, Callback);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                MapWinGIS.Utility.Logger.Message("导入.dem格式的文件出现错误 " + InFile + "\r\n" + ex.Message);
                return false;
            }
        }

        private bool ImportSURFERFormat(string InFile, MapWinGIS.ICallback Callback)
        {
            string tempPath = System.IO.Path.ChangeExtension(Program.GetMWGTempFile(), "");
            System.IO.Directory.CreateDirectory(tempPath);
            string tempAsc = tempPath + "\\" + System.IO.Path.GetFileNameWithoutExtension(InFile) + ".asc";

            string null_string = null;
            if (!grd2asc(InFile, tempAsc, ref null_string))
            {
                return false;
            }

            base.Close();
            if (!base.Open(tempAsc))
            {
                return false;
            }

            return true;
        }

        private void MakeGrid(sDEMData DEMData, MapWinGIS.ICallback Callback)
        {
            //根据.dem文件，生成一个网格文件对象
            MapWinGIS.GridHeader h = new MapWinGIS.GridHeader();
            int i ;
            int j ;
            int m ;
            int n ;
            h.dX = 30;
            h.dY = 30;
            h.Notes = "DEMData.Notes";
            h.NodataValue = -1;
            h.NumberCols = DEMData.NumCols;
            h.NumberRows = System.Convert.ToInt32((DEMData.MaxY() - DEMData.MinY() / 30)) + 1;
            h.XllCenter = DEMData.MinX();
            h.YllCenter = DEMData.MinY();

            MapWinGIS.Utility.Logger.Dbg("根据.dem文件和其数据文件，生成一个网格文件对象");
            base.Close();
            MapWinGIS.Utility.Logger.Dbg("开始创建");
            base.CreateNew("", h, MapWinGIS.GridDataType.ShortDataType, h.NodataValue, true, MapWinGIS.GridFileType.Binary);
            MapWinGIS.Utility.Logger.Dbg("只带header的新的grid网格文件创建了");

            for (i = 0; i < DEMData.NumCols; i++)
            {
                for (j = 0; j < DEMData.NumElevs[i]; j++)
                {
                    base.ProjToCell(DEMData.ColStarts[i].X, System.Convert.ToInt32(DEMData.ColStarts[i].Y) + (30 * j), out m, out n);
                    base.Value[m, n] = DEMData.Values[i, j];
                }
                MapWinGIS.Utility.Logger.Dbg("创建 MapWinGIS grid 对象. numCols: " + i.ToString());
                if (Callback != null)
                {
                    Callback.Progress(base.Key, 50 + i / DEMData.NumCols * 50, "创建 MapWinGIS grid 对象");
                }
            }
            MapWinGIS.Utility.Logger.Dbg("生成一个网格文件对象完成");
        }

        private void MakeGrid(sFLTData FLTData, string InitialFile, MapWinGIS.ICallback Callback)
        {
            //根据.flt文件及其数据文件，创建一个grid网格对象
            MapWinGIS.GridHeader h = new MapWinGIS.GridHeader();
            int i, j;
            h.dX = FLTData.cellsize;
            h.dY = FLTData.cellsize;
            h.Notes = "Grid文件导入路径 " + System.IO.Path.GetFileName(InitialFile);
            h.NodataValue = FLTData.NODATA_value;
            h.NumberCols = FLTData.ncols;
            h.NumberRows = FLTData.nrows;
            h.XllCenter = FLTData.xllcenter;
            h.YllCenter = FLTData.yllcenter;

            base.Close();
            base.CreateNew("", h, MapWinGIS.GridDataType.FloatDataType, -1, true, MapWinGIS.GridFileType.Binary);

            for (i = 0; i < FLTData.ncols; i++)
            {
                for (j = 0; j < FLTData.nrows; j++)
                {
                    base.Value[i, j] = FLTData.Values[i, j];
                }
                if (Callback != null)
                {
                    Callback.Progress(base.Key, 50 + i / FLTData.ncols * 50, "创建 MapWinGIS grid");
                }
            }
        }
        private sDEMData ReadDEMData(Stream fileStream, MapWinGIS.ICallback Callback)
        {
            // 从USGS读取dem格式的数据文件(ASCII 文本)
            int i, j;

            int numElevs;
            int Elev = 0;
            int Off2;
            int Off3;
            int Inserts;
            sDEMData DEMData = new sDEMData();
            string chunckResult;
            try
            {
                //获取header数据
                DEMData.Notes = GetChunk(fileStream, 1, 144);
                DEMData.HorizUnits = System.Convert.ToInt32(GetChunk(fileStream, 529, 6));
                DEMData.VertUnits = System.Convert.ToInt32(GetChunk(fileStream, 535, 6));
                DEMData.Vertices = new sVertex[4];
                for (i = 0; i <= 3; i++)
                {
                    DEMData.Vertices[i].X = Fdbl(GetChunk(fileStream, 547 + (48 * i), 24));
                    DEMData.Vertices[i].Y = Fdbl(GetChunk(fileStream, 571 + (48 * i), 24));
                }
                DEMData.Min = System.Convert.ToDouble(Fdbl(GetChunk(fileStream, 739, 24)));
                DEMData.Max = System.Convert.ToDouble(Fdbl(GetChunk(fileStream, 763, 24)));
                DEMData.NumCols = System.Convert.ToInt32((GetChunk(fileStream, 859, 6)));
                DEMData.Values = new int[DEMData.NumCols - 1 + 1, 1];
                DEMData.NumElevs = new int[DEMData.NumCols - 1 + 1];
                MapWinGIS.Utility.Logger.Dbg("DEM 的头(Header)信息:");
                MapWinGIS.Utility.Logger.Dbg("DEMData.Notes: " + DEMData.Notes);
                MapWinGIS.Utility.Logger.Dbg("DEMData.HorizUnits: " + DEMData.HorizUnits.ToString());
                MapWinGIS.Utility.Logger.Dbg("DEMData.VertUnits: " + DEMData.VertUnits.ToString());
                MapWinGIS.Utility.Logger.Dbg("DEMData.Min: " + DEMData.Min.ToString());
                MapWinGIS.Utility.Logger.Dbg("DEMData.Max: " + DEMData.Max.ToString());
                MapWinGIS.Utility.Logger.Dbg("DEMData.NumCols: " + DEMData.NumCols.ToString());

                //获取海拔(elevation)数据
                Off2 = 1024;
                Inserts = 0;
                DEMData.ColStarts = new sVertex[DEMData.NumCols - 1 + 1];
                MapWinGIS.Utility.Logger.Dbg("DEM Data");
                for (i = 0; i < DEMData.NumCols; i++)
                {
                    //读取elevs的数量, starting x 和 starting y
                    numElevs = Convert.ToInt32(GetChunk(fileStream, Off2 + 13, 6));
                    DEMData.NumElevs[i] = numElevs;
                    DEMData.ColStarts[i].X = Fdbl(GetChunk(fileStream, Off2 + 25, 24));
                    DEMData.ColStarts[i].Y = Fdbl(GetChunk(fileStream, Off2 + 49, 24));
                    //Information.UBound(DEMData.Values, 2)
                    if (numElevs - 1 > DEMData.Values.GetLength(1))
                    {
                        DEMData.Values = (int[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray((Array)DEMData.Values, new int[DEMData.NumCols, numElevs]);
                    }
                    for (j = 0; j < numElevs; j++)
                    {
                        Inserts = 0;
                        if (j > 145)
                        {
                            Inserts = (int)((j - 145) / 171 + 1);
                        }
                        Off3 = Off2 + 145 + (j * 6) + Inserts * 4; //4 spaces between each extended group
                        chunckResult = GetChunk(fileStream, Off3, 6);
                        if (chunckResult.Trim() != string.Empty)
                        {
                            if (int.TryParse(chunckResult, out Elev) == false)
                            {
                                Elev = -1;
                                MapWinGIS.Utility.Logger.Dbg("ReadDEMData Error. GetChunk returned non-integer:" + chunckResult);
                            }
                        }
                        DEMData.Values[i, j] = Elev;
                    }
                    if (numElevs <= 146)
                    {
                        Off2 = Off2 + 1024;
                    }
                    else
                    {
                        Inserts = (int)((numElevs - 146) / 170 + 1);
                        Off2 = Off2 + 1024 + Inserts * 1024;
                    }
                    if (Callback != null)
                    {
                        Callback.Progress(base.Key, i / DEMData.NumCols * 50, "读取 DEM 数据");
                    }
                }
                MapWinGIS.Utility.Logger.Dbg("读取 DEM 数据完成");
                return DEMData;
            }
            catch (System.Exception ex)
            {
                MapWinGIS.Utility.Logger.Msg("读取DEM数据出错 : " + ex.Message);
            }
            sDEMData a = new sDEMData();
            return a;
        }

        private string GetChunk(Stream St, int Offset, int Count)
        {
            byte[] b = null;
            b = new byte[Count - 1 + 1];
            St.Seek(Offset - 1, SeekOrigin.Begin);
            St.Read(b, 0, Count);

            return System.Text.Encoding.ASCII.GetString(b).Trim();
        }

        private double Fdbl(string s)
        {
            double d ;
            s = Strings.Replace(s, "D+", "e", 1, -1, CompareMethod.Text);
            d = double.Parse(s);
            return d;
        }

        public override string CdlgFilter
        {
            get
            {
                string[] arr = null;
                string newFormats = default(string);
                int oldLength = default(int);

                arr = base.CdlgFilter.Split('|');
                if (arr.Length > 1)
                {
                    newFormats = arr[1];
                    newFormats += ";*.dem;*.flt";
                    oldLength = arr.Length;

                    Array.Resize(ref arr, arr.Length + 3 + 1); // 添加两个新的格式
                    arr[1] = newFormats;
                    arr[oldLength] = "USGS NED Grid Float (*.flt)";
                    arr[oldLength + 1] = "*.flt";
                    arr[oldLength + 2] = "USGS DEM (*.dem)";
                    arr[oldLength + 3] = "*.dem";
                    return string.Join("|", arr);
                }
                else
                {
                    return base.CdlgFilter;
                }
            }
        }

        public override void CellToProj(int Column, int Row, out double x, out double y)
        {
            base.CellToProj(Column, Row, out x, out y);
        }

        public override bool Clear(object ClearValue)
        {
           return base.Clear(ClearValue);
        }

        public override bool Close()
        {
           return base.Close();
        }

        public override MapWinGIS.GridDataType DataType
        {
            get
            {
                return base.DataType;
            }
        }

        public override string get_ErrorMsg(int ErrorCode)
        {
            return base.ErrorMsg[ErrorCode];
        }

        public override string Filename
        {
            get
            {
                return base.Filename;
            }
        }

        public override MapWinGIS.ICallback GlobalCallback
        {
            get
            {
                return base.GlobalCallback;
            }
            set
            {
                base.GlobalCallback = value;
            }
        }

        public override MapWinGIS.GridHeader Header
        {
            get
            {
                return base.Header;
            }
        }

        public override bool InRam
        {
            get
            {
                return base.InRam;
            }
        }

        public override string Key
        {
            get
            {
                return base.Key;
            }
            set
            {
                base.Key = value;
            }
        }

        public override int LastErrorCode
        {
            get
            {
                return base.LastErrorCode;
            }
        }

        public override object Maximum
        {
            get
            {
                return base.Maximum;
            }
        }

        public override object Minimum
        {
            get
            {
                return base.Minimum;
            }
        }

        public override void ProjToCell(double x, double y, out int Column, out int Row)
        {
            base.ProjToCell(x, y, out Column, out Row);
        }

        public override bool Save(string Filename = "", MapWinGIS.GridFileType GridFileType = MapWinGIS.GridFileType.UseExtension, MapWinGIS.ICallback cBack = null)
        {
            return base.Save(Filename, GridFileType, cBack);
        }

        public override object get_Value(int Column, int Row)
        {
            return base.Value[Column, Row];
        }
        public override void set_Value(int Column, int Row, object Value)
        {
            base.Value[Column, Row] = Value;
        }

        public override bool CreateNew(string Filename, MapWinGIS.GridHeader Header, MapWinGIS.GridDataType DataType, object InitialValue, bool InRam = true, MapWinGIS.GridFileType FileType = MapWinGIS.GridFileType.UseExtension, MapWinGIS.ICallback cBack = null)
        {
           return base.CreateNew(Filename, Header, DataType, InitialValue, InRam, FileType, cBack);
        }

        public override bool Open(string Filename, MapWinGIS.GridDataType DataType = MapWinGIS.GridDataType.UnknownDataType, bool InRam = true, MapWinGIS.GridFileType FileType = MapWinGIS.GridFileType.UseExtension, MapWinGIS.ICallback cBack = null)
        {
            string bgdequiv = (string)(System.IO.Path.ChangeExtension(Filename, ".bgd"));
            if (Filename.ToLower().EndsWith(".flt"))
            {
                if (File.Exists(bgdequiv) && MapWinGIS.Utility.DataManagement.CheckFile2Newest(Filename, bgdequiv))
                {
                    return base.Open(bgdequiv, MapWinGIS.GridDataType.UnknownDataType, InRam, MapWinGIS.GridFileType.Binary, cBack);
                }
                else
                {
                    if (ImportFLTFormat(Filename, ref cBack))
                    {
                        base.Save(bgdequiv, MapWinGIS.GridFileType.Binary, cBack);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (Filename.ToLower().EndsWith(".dem"))
            {
                if (File.Exists(bgdequiv) && MapWinGIS.Utility.DataManagement.CheckFile2Newest(Filename, bgdequiv))
                {
                    return base.Open(bgdequiv, MapWinGIS.GridDataType.UnknownDataType, InRam, MapWinGIS.GridFileType.Binary, cBack);
                }
                else
                {
                    if (ImportDEMFormat(Filename, ref cBack))
                    {
                        MapWinGIS.Utility.Logger.Dbg("Save grid with name: " + bgdequiv);
                        base.Save(bgdequiv, MapWinGIS.GridFileType.Binary, cBack);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (Filename.ToLower().EndsWith(".grd"))
            {
                if (File.Exists(bgdequiv) && MapWinGIS.Utility.DataManagement.CheckFile2Newest(Filename, bgdequiv))
                {
                    return base.Open(bgdequiv, MapWinGIS.GridDataType.UnknownDataType, InRam, MapWinGIS.GridFileType.Binary, cBack);
                }
                else
                {
                    if (ImportSURFERFormat(Filename, cBack))
                    {
                        base.Save(bgdequiv, MapWinGIS.GridFileType.Binary, cBack);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return base.Open(Filename, DataType, InRam, FileType, cBack);
            }
        }

        public override bool AssignNewProjection(string Projection)
        {
           return base.AssignNewProjection(Projection);
        }

        public override MapWinGIS.GridColorScheme RasterColorTableColoringScheme
        {
            get
            {
                return base.RasterColorTableColoringScheme;
            }
        }

        public override bool GetRow(int Row, ref float Vals)
        {
            return base.GetRow(Row, Vals);
        }

        public override bool PutRow(int Row, ref float Vals)
        {
            return base.PutRow(Row, Vals);
        }

        public override bool GetFloatWindow(int StartRow, int EndRow, int StartCol, int EndCol, ref float Vals)
        {
            return base.GetFloatWindow(StartRow, EndRow, StartCol, EndCol, Vals);
        }

        public override bool PutFloatWindow(int StartRow, int EndRow, int StartCol, int EndCol, ref float Vals)
        {
            return base.PutFloatWindow(StartRow, EndRow, StartCol, EndCol, Vals);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
