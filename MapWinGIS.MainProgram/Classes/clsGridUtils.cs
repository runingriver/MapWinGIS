using System;
using System.Threading;

namespace MapWinGIS.MainProgram
{
    class GridUtils
    {
        private bool m_Ready;
        private static MapWinGIS.Grid m_Grid;
        private Thread m_Thread;

        private void InitThread()
        {
            try
            {
                m_Grid = new MapWinGIS.Grid();
            }
            catch (Exception ex)
            {
                MapWinGIS.Utility.Logger.Message(ex.ToString());
            }
            finally
            {
                m_Ready = true;
            }
        }

        private void StartThread()
        {
            m_Ready = false;
            m_Thread = new Thread(new ThreadStart(InitThread));
            m_Thread.Start();

            while (m_Ready == false)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void StopThread()
        {
            m_Thread.Abort();
            m_Thread = null;
        }

        public MapWinGIS.Grid CreateSafeGrid()
        {
            try
            {
                StartThread();
                StopThread();
                MapWinGIS.Utility.Logger.Message(m_Thread.ThreadState.ToString());
            }
            catch (Exception ex)
            {
                Program.ShowError(ex);
            }
            return m_Grid;
        }

        public string GridCdlgFilter()
        {
            StartThread();
            StopThread();
            return m_Grid.CdlgFilter;
        }

    }
}
