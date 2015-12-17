using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillFramework
{
    public sealed class TypedDataCollection
    {
        public void GetOrNewData<T>(out T t) where T : new()
        {
            t = GetData<T>();
            if (null == t)
            {
                t = new T();
                AddData(t);
            }
        }
        public void AddData<T>(T data)
        {
            Type t = typeof(T);
            if (null != data)
            {
                if (m_AiDatas.Contains(t))
                {
                    m_AiDatas[t] = data;
                }
                else
                {
                    m_AiDatas.Add(t, data);
                }
            }
        }
        public void RemoveData<T>(T t)
        {
            RemoveData<T>();
        }
        public void RemoveData<T>()
        {
            Type t = typeof(T);
            if (m_AiDatas.Contains(t))
            {
                m_AiDatas.Remove(t);
            }
        }
        public T GetData<T>()
        {
            T ret = default(T);
            Type t = typeof(T);
            if (m_AiDatas.Contains(t))
            {
                ret = (T)m_AiDatas[t];
            }
            return ret;
        }
        public void Clear()
        {
            m_AiDatas.Clear();
        }
        public void Visit(MyAction<object, object> visitor)
        {
            foreach (DictionaryEntry dict in m_AiDatas)
            {
                visitor(dict.Key, dict.Value);
            }
        }
        private Hashtable m_AiDatas = new Hashtable();
    }
}
