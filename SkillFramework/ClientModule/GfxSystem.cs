using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillFramework
{
    public delegate void GfxSkillStartHandler(GameObject obj, int skillid);

    public sealed class GfxSkillSystem
    {
        public static GfxSkillStartHandler OnGfxSkillStart = null;

        private List<SkillLogicInfo> m_SkillLogicInfos = new List<SkillLogicInfo>();
        private Dictionary<int, List<SkillInstanceInfo>> m_SkillInstancePool = new Dictionary<int, List<SkillInstanceInfo>>();

        public static GfxSkillSystem Instance
        {
            get
            {
                return s_Instance;
            }
        }
        private static GfxSkillSystem s_Instance = new GfxSkillSystem();

        private class SkillInstanceInfo
        {
            public int m_SkillId;
            public SkillInstance m_SkillInstance;
            public bool m_IsUsed;
        }
        private class SkillLogicInfo
        {
            public GameObject Sender
            {
                get
                {
                    return m_Sender;
                }
            }
            public int SkillId
            {
                get
                {
                    return m_SkillInfo.m_SkillId;
                }
            }
            public SkillInstance SkillInst
            {
                get
                {
                    return m_SkillInfo.m_SkillInstance;
                }
            }
            public SkillInstanceInfo Info
            {
                get
                {
                    return m_SkillInfo;
                }
            }

            public SkillLogicInfo(GameObject obj, SkillInstanceInfo info)
            {
                m_Sender = obj;
                m_SkillInfo = info;
            }

            private GameObject m_Sender;
            private SkillInstanceInfo m_SkillInfo;
        }
        public void Init()
        {
            //注册技能触发器
            //SkillTrigerManager.Instance.RegisterTrigerFactory("movecontrol", new SkillTrigerFactoryHelper<Trigers.MoveControlTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("superarmor", new SkillTrigerFactoryHelper<Trigers.SuperArmorTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("animation", new SkillTrigerFactoryHelper<Trigers.AnimationTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("areadamage", new SkillTrigerFactoryHelper<Trigers.AreaDamageTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("colliderdamage", new SkillTrigerFactoryHelper<Trigers.ColliderDamageTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("deliverdamage", new SkillTrigerFactoryHelper<Trigers.DeliverDamageTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("playsound", new SkillTrigerFactoryHelper<Trigers.PlaySoundTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("stopsound", new SkillTrigerFactoryHelper<Trigers.StopSoundTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("charactereffect", new SkillTrigerFactoryHelper<Trigers.CharacterEffectTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("sceneeffect", new SkillTrigerFactoryHelper<Trigers.SceneEffectTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("uieffect", new SkillTrigerFactoryHelper<Trigers.UIEffectTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("charge", new SkillTrigerFactoryHelper<Trigers.ChargeTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("jump", new SkillTrigerFactoryHelper<Trigers.JumpTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("timescale", new SkillTrigerFactoryHelper<Trigers.TimeScaleTriger>());

            //SkillTrigerManager.Instance.RegisterTrigerFactory("addimpacttoself", new SkillTrigerFactoryHelper<Trigers.AddImpactToSelfTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("lockframe", new SkillTrigerFactoryHelper<Trigers.LockFrameTriger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("movechild", new SkillTrigerFactoryHelper<Trigers.MoveChildTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("addbreaksection", new SkillTrigerFactoryHelper<Trigers.BreakSectionTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("startcurvemove", new SkillTrigerFactoryHelper<Trigers.CurveMovementTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("startframemove", new SkillTrigerFactoryHelper<Trigers.FrameMovementTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("shakecamera2", new SkillTrigerFactoryHelper<Trigers.ShakeCamera2Trigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("setanimspeed", new SkillTrigerFactoryHelper<Trigers.AnimationSpeedTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("setcross2othertime", new SkillTrigerFactoryHelper<Trigers.SetCrossFadeTimeTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("findmovetarget", new SkillTrigerFactoryHelper<Trigers.ChooseTargetTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("addlockinputtime", new SkillTrigerFactoryHelper<Trigers.AddLockInputTimeTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("summonnpc", new SkillTrigerFactoryHelper<Trigers.SummonObjectTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("summonainpc", new SkillTrigerFactoryHelper<Trigers.SummonAIObjectTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("settransform", new SkillTrigerFactoryHelper<Trigers.SetTransformTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("destroyself", new SkillTrigerFactoryHelper<Trigers.DestroySelfTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("destroysummonnpc", new SkillTrigerFactoryHelper<Trigers.DestroySummonObjectTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("setchildvisible", new SkillTrigerFactoryHelper<Trigers.SetChildVisibleTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("setchildactive", new SkillTrigerFactoryHelper<Trigers.SetChildActiveTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("rotate", new SkillTrigerFactoryHelper<Trigers.RotateTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("setenable", new SkillTrigerFactoryHelper<Trigers.SetEnableTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("gotosection", new SkillTrigerFactoryHelper<Trigers.GotoSectionTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("createshadow", new SkillTrigerFactoryHelper<Trigers.CreateShadowTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("cleardamagepool", new SkillTrigerFactoryHelper<Trigers.ClearDamagePoolTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("cleardamagestate", new SkillTrigerFactoryHelper<Trigers.ClearDamageStateTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("checkonground", new SkillTrigerFactoryHelper<Trigers.CheckOnGroundTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("stopcursection", new SkillTrigerFactoryHelper<Trigers.StopCurSectionTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("enablechangedir", new SkillTrigerFactoryHelper<Trigers.EnableChangeDirTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("stopeffect", new SkillTrigerFactoryHelper<Trigers.StopEffectTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("setcamerafollowspeed", new SkillTrigerFactoryHelper<Trigers.SetCameraFollowSpeed>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("resetcamerafollowspeed", new SkillTrigerFactoryHelper<Trigers.ResetCameraFollowSpeed>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("movecamera", new SkillTrigerFactoryHelper<Trigers.MoveCameraTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("cameratrack", new SkillTrigerFactoryHelper<Trigers.CameraTrack>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("facetotarget", new SkillTrigerFactoryHelper<Trigers.FaceToTargetTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("rotatecamera", new SkillTrigerFactoryHelper<Trigers.RotateCameraTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("setlifetime", new SkillTrigerFactoryHelper<Trigers.SetlifeTimeTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("simulatemove", new SkillTrigerFactoryHelper<Trigers.SimulateMoveTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("exchangeposition", new SkillTrigerFactoryHelper<Trigers.ExchangePositionTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("fruitninjia", new SkillTrigerFactoryHelper<Trigers.FruitNinjiaTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("oninput", new SkillTrigerFactoryHelper<Trigers.OnInputTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("move2targetpos", new SkillTrigerFactoryHelper<Trigers.Move2TargetPosTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("crosssummonmove", new SkillTrigerFactoryHelper<Trigers.CrossSummonMoveTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("storepos", new SkillTrigerFactoryHelper<Trigers.StorePosTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("restorepos", new SkillTrigerFactoryHelper<Trigers.RestorePosTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("blackscene", new SkillTrigerFactoryHelper<Trigers.BlackSceneTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("setuivisible", new SkillTrigerFactoryHelper<Trigers.SetUIVisibleTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("addimpacttotarget", new SkillTrigerFactoryHelper<Trigers.AddImpactToTargetTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("grabtarget", new SkillTrigerFactoryHelper<Trigers.GrabTargetTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("oncross", new SkillTrigerFactoryHelper<Trigers.OnCrossTrigger>());
            //SkillTrigerManager.Instance.RegisterTrigerFactory("fieldofview", new SkillTrigerFactoryHelper<Trigers.FieldOfViewTrigger>());
        }
        public void Reset()
        {
            int count = m_SkillLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index)
            {
                SkillLogicInfo info = m_SkillLogicInfos[index];
                if (null != info)
                {
                    info.SkillInst.OnSkillStop(info.Sender, 0);
                    StopSkillInstance(info);
                    m_SkillLogicInfos.RemoveAt(index);
                }
            }
            m_SkillLogicInfos.Clear();
        }
        public void PreloadSkillInstance(int skillId)
        {
            SkillInstanceInfo info = NewSkillInstance(skillId);
            if (null != info)
            {
                RecycleSkillInstance(info);
            }
        }
        public void ClearSkillInstancePool()
        {
            m_SkillInstancePool.Clear();
        }
        //用于上层逻辑检查通过后调用
        public void StartSkill(int actorId, int skillId)
        {
            GameObject obj = GetGameObjectById(actorId);
            if (null != obj)
            {
                SkillLogicInfo logicInfo = m_SkillLogicInfos.Find(info => info.Sender == obj && info.SkillId == skillId);
                if (logicInfo != null)
                {
                    return;
                }
                SkillInstanceInfo inst = NewSkillInstance(skillId);
                if (null != inst)
                {
                    m_SkillLogicInfos.Add(new SkillLogicInfo(obj, inst));
                }
                else
                {
                    //DashFire.LogicSystem.NotifyGfxStopSkill(obj, skillId);
                    return;
                }

                logicInfo = m_SkillLogicInfos.Find(info => info.Sender == obj && info.SkillId == skillId);
                if (null != logicInfo)
                {
                    if (OnGfxSkillStart != null)
                    {
                        OnGfxSkillStart(obj, skillId);
                    }
                    logicInfo.SkillInst.Start(logicInfo.Sender);
                }
            }
        }
        //在技能未开始时取消技能（用于上层逻辑检查失败时）
        public void CancelSkill(int actorId, int skillId)
        {
            GameObject obj = GetGameObjectById(actorId);
            if (null != obj)
            {
                SkillLogicInfo logicInfo = m_SkillLogicInfos.Find(info => info.Sender == obj && info.SkillId == skillId);
                if (null != logicInfo)
                {
                    if (logicInfo.SkillInst.IsControlMove)
                    {
                        //DashFire.LogicSystem.NotifyGfxMoveControlFinish(obj, skillId, true);
                        logicInfo.SkillInst.IsControlMove = false;
                    }
                    //DashFire.LogicSystem.NotifyGfxAnimationFinish(obj, true);
                    RecycleSkillInstance(logicInfo.Info);
                    m_SkillLogicInfos.Remove(logicInfo);
                }
            }
        }
        public void StopSkill(int actorId, bool isinterrupt)
        {
            GameObject obj = GetGameObjectById(actorId);
            if (null == obj)
            {
                return;
            }
            int count = m_SkillLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index)
            {
                SkillLogicInfo info = m_SkillLogicInfos[index];
                if (info.Sender == obj)
                {
                    if (isinterrupt)
                    {
                        info.SkillInst.OnInterrupt(obj, 0);
                    }
                    else
                    {
                        info.SkillInst.OnSkillStop(obj, 0);
                    }
                    StopSkillInstance(info, isinterrupt);
                    m_SkillLogicInfos.RemoveAt(index);
                }
            }
        }
        public void SendMessage(int actorId, int skillId, string msgId)
        {
            GameObject obj = GetGameObjectById(actorId);
            if (null != obj)
            {
                SkillLogicInfo logicInfo = m_SkillLogicInfos.Find(info => info.Sender == obj && info.SkillId == skillId);
                if (null != logicInfo && null != logicInfo.SkillInst)
                {
                    logicInfo.SkillInst.SendMessage(msgId);
                }
            }
        }
        public void Tick()
        {
            int ct = m_SkillLogicInfos.Count;
            long delta = (long)(Time.deltaTime * 1000 * 1000);
            for (int ix = ct - 1; ix >= 0; --ix)
            {
                SkillLogicInfo info = m_SkillLogicInfos[ix];
                bool exist = IsExistGameObject(info.Sender);
                if (exist)
                {
                    info.SkillInst.Tick(info.Sender, delta);
                }
                if (!exist || info.SkillInst.IsFinished)
                {
                    if (!exist)
                    {
                        info.SkillInst.OnSkillStop(info.Sender, 0);
                    }
                    StopSkillInstance(info);
                    m_SkillLogicInfos.RemoveAt(ix);
                }
            }
        }

        private void StopSkillInstance(SkillLogicInfo info)
        {
            StopSkillInstance(info, false);
        }

        private void StopSkillInstance(SkillLogicInfo info, bool isInterrupt)
        {
            //DashFire.LogSystem.Debug("Skill {0} finished.", info.SkillId);
            if (!isInterrupt)
            {
                if (info.SkillInst.IsControlMove)
                {
                    //DashFire.LogicSystem.NotifyGfxMoveControlFinish(info.Sender, info.SkillId, true);
                    info.SkillInst.IsControlMove = false;
                }
                //DashFire.LogicSystem.NotifyGfxAnimationFinish(info.Sender, true);
                //DashFire.LogicSystem.NotifyGfxStopSkill(info.Sender, info.SkillId);
            }
            else
            {
                if (info.SkillInst.IsControlMove)
                {
                    info.SkillInst.IsControlMove = false;
                }
            }
            RecycleSkillInstance(info.Info);
        }

        public void OnCollider(GameObject one, GameObject two)
        {

        }
        private GameObject GetGameObjectById(int actorId)
        {
            //TODO:
            return null;
        }
        private bool IsExistGameObject(GameObject go)
        {
            //TODO:
            return false;
        }
        private string GetSkillFilePath(int skillId)
        {
            //TODO:
            return string.Empty;
        }
        private SkillInstanceInfo NewSkillInstance(int skillId)
        {
            SkillInstanceInfo instInfo = GetUnusedSkillInstanceInfoFromPool(skillId);
            if (null == instInfo)
            {
                string filePath = GetSkillFilePath(skillId);
                if (string.IsNullOrEmpty(filePath))
                {
                    SkillConfigManager.Instance.LoadSkillIfNotExist(skillId, filePath);
                    SkillInstance inst = SkillConfigManager.Instance.NewSkillInstance(skillId);

                    if (inst == null)
                    {
                        Logger.Error("Can't load skill config, skill:{0} !", skillId);
                        return null;
                    }
                    SkillInstanceInfo res = new SkillInstanceInfo();
                    res.m_SkillId = skillId;
                    res.m_SkillInstance = inst;
                    res.m_IsUsed = true;

                    AddSkillInstanceInfoToPool(skillId, res);
                    return res;
                }
                else
                {
                    Logger.Error("Can't find skill config, skill:{0} !", skillId);
                    return null;
                }
            }
            else
            {
                instInfo.m_IsUsed = true;
                return instInfo;
            }
        }
        private void RecycleSkillInstance(SkillInstanceInfo info)
        {
            info.m_SkillInstance.Reset();
            info.m_IsUsed = false;
        }
        private void AddSkillInstanceInfoToPool(int skillId, SkillInstanceInfo info)
        {
            if (m_SkillInstancePool.ContainsKey(skillId))
            {
                List<SkillInstanceInfo> infos = m_SkillInstancePool[skillId];
                infos.Add(info);
            }
            else
            {
                List<SkillInstanceInfo> infos = new List<SkillInstanceInfo>();
                infos.Add(info);
                m_SkillInstancePool.Add(skillId, infos);
            }
        }
        private SkillInstanceInfo GetUnusedSkillInstanceInfoFromPool(int skillId)
        {
            SkillInstanceInfo info = null;
            if (m_SkillInstancePool.ContainsKey(skillId))
            {
                List<SkillInstanceInfo> infos = m_SkillInstancePool[skillId];
                int ct = infos.Count;
                for (int ix = 0; ix < ct; ++ix)
                {
                    if (!infos[ix].m_IsUsed)
                    {
                        info = infos[ix];
                        break;
                    }
                }
            }
            return info;
        }

        
    }
}
