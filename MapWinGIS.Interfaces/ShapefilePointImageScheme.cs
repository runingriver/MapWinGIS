using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace MapWinGIS.Interfaces
{
    public class ShapefilePointImageScheme
    {
        internal Hashtable m_Items = new Hashtable();
        internal Hashtable m_ItemVisibility = new Hashtable();
        private long m_FieldIndex;
        private long m_LastKnownLayerHandle;

        public ShapefilePointImageScheme(long lyrHandle)
        {
            m_FieldIndex = -1;
            m_LastKnownLayerHandle = lyrHandle;
        }

        public int NumberItems
        {
            get { return m_Items.Count; }
        }

        public Hashtable Items
        {
            get { return m_Items; }
        }

        public Hashtable ItemVisibility
        {
            get { return m_ItemVisibility; }
        }

        public bool GetVisible(string FieldValue)
        {
            if (!m_ItemVisibility.Contains(FieldValue)) return true;
            return (bool)m_ItemVisibility[FieldValue];
        }

        public void SetVisible(string FieldValue, bool value)
        {
            if (m_ItemVisibility.Contains(FieldValue))
            {
                m_ItemVisibility[FieldValue] = value;
            }
            else
            {
                m_ItemVisibility.Add(FieldValue, value);
            }
        }

        public int GetImageIndex(string FieldValue)
        {
            if (!m_Items.Contains(FieldValue))
            {
                return -1;
            }
            else
            {
                return Convert.ToInt32(m_Items[FieldValue]);
            }
        }

        public void SetImageIndex(string FieldValue, int value)
        {
            if (m_Items.Contains(FieldValue))
            {
                m_Items[FieldValue] = value;
            }
            else
            {
                m_Items.Add(FieldValue, value);
            }
        }

        public void Clear()
        {
            m_Items.Clear();
            m_FieldIndex = -1;
        }

        public long FieldIndex
        {
            get { return m_FieldIndex; }
            set { m_FieldIndex = value; }
        }

        public long LastKnownLayerHandle
        {
            get { return m_LastKnownLayerHandle; }
            set { m_LastKnownLayerHandle = value; }
        }


    }
}
