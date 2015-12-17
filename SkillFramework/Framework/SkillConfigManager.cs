using System;
using System.Collections.Generic;

namespace SkillFramework
{
    public class SkillConfigManager
    {
        public void LoadSkillIfNotExist(int id, string file)
        {
            if (!ExistSkill(id))
            {
                LoadSkill(file);
            }
        }
        public bool ExistSkill(int id)
        {
            return null != GetSkillInstanceResource(id);
        }
        public void LoadSkill(string file)
        {
            if (!string.IsNullOrEmpty(file))
            {
                ScriptableDataFile dataFile = new ScriptableDataFile();
#if DEBUG
                if (dataFile.Load(file))
                {
                    Load(dataFile);
                }
#else
                dataFile.LoadObfuscatedFile(file, GlobalVariables.Instance.DecodeTable);
                Load(dataFile);
#endif
            }
        }
        public void LoadSkillText(string text)
        {
            ScriptableDataFile dataFile = new ScriptableDataFile();
#if DEBUG
            if (dataFile.LoadFromString(text, "skill"))
            {
                Load(dataFile);
            }
#else
            dataFile.LoadObfuscatedCode(text, GlobalVariables.Instance.DecodeTable);
            Load(dataFile);
#endif
        }
        public SkillInstance NewSkillInstance(int id)
        {
            SkillInstance instance = null;
            SkillInstance temp = GetSkillInstanceResource(id);
            if (null != temp)
            {
                instance = temp.Clone();
            }
            return instance;
        }
        public void Clear()
        {
            lock (m_Lock)
            {
                m_SkillInstances.Clear();
            }
        }

        private void Load(ScriptableDataFile dataFile)
        {
            lock (m_Lock)
            {
                foreach (ScriptableDataInfo info in dataFile.ScriptableDatas)
                {
                    if (info.GetId() == "skill")
                    {
                        FunctionData funcData = info.First;
                        if (null != funcData)
                        {
                            CallData callData = funcData.Call;
                            if (null != callData && callData.HaveParam())
                            {
                                int id = int.Parse(callData.GetParamId(0));
                                if (!m_SkillInstances.ContainsKey(id))
                                {
                                    SkillInstance instance = new SkillInstance();
                                    instance.Init(info);
                                    m_SkillInstances.Add(id, instance);

                                    Logger.Info("ParseSkill {0}", id);
                                }
                                else
                                {
                                    //repeated skill config.
                                }
                            }
                        }
                    }
                }
            }
        }
        private SkillInstance GetSkillInstanceResource(int id)
        {
            SkillInstance instance = null;
            lock (m_Lock)
            {
                if (m_SkillInstances.ContainsKey(id))
                {
                    instance = m_SkillInstances[id];
                }
            }
            return instance;
        }

        private SkillConfigManager() { }

        private object m_Lock = new object();
        private Dictionary<int, SkillInstance> m_SkillInstances = new Dictionary<int, SkillInstance>();

        public static SkillConfigManager Instance
        {
            get { return s_Instance; }
        }
        private static SkillConfigManager s_Instance = new SkillConfigManager();
    }
}
