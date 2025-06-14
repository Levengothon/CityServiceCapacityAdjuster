using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colossal.Entities;
using Game;
using Game.Economy;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace CityServiceCapacityAdjuster
{
    public partial class PostalOfficeFixSystem : GameSystemBase
    {
        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return 262144 / 32; // 32 updates per day is once per 45 in-game minutes
        }
        public override int GetUpdateOffset(SystemUpdatePhase phase)
        {
            return 48; // PostFacilityAISystem has 176
        }

        private EntityQuery m_PostFacilitiesQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_PostFacilitiesQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[3]
                {
                    ComponentType.ReadOnly<Game.Prefabs.PrefabRef>(),
                    ComponentType.ReadOnly<Game.Buildings.PostFacility>(),
                    ComponentType.ReadWrite<Game.Economy.Resources>(),
                },
                None = new ComponentType[3]
                {
                    ComponentType.ReadOnly<Game.Common.Destroyed>(),
                    ComponentType.ReadOnly<Game.Common.Deleted>(),
                    ComponentType.ReadOnly<Game.Tools.Temp>(),
                },
            });
            RequireForUpdate(m_PostFacilitiesQuery);
            Mod.log.Info("PostalOfficeFixSystem has been created.");
        }

        protected override void OnUpdate()
        {
            if(Mod.m_Setting.FixMailOverflow == true)
            {
                Mod.log.Info("Update in progress");

                foreach (Entity postEntity in m_PostFacilitiesQuery.ToEntityArray(Allocator.Temp))
                {
                    if (!EntityManager.TryGetComponent<PrefabRef>(postEntity, out PrefabRef prefab))
                    {
                        Mod.log.Warn($"Failed to retrieve PrefabRef for {postEntity}.");
                        continue;
                    }
                    if (!EntityManager.TryGetComponent<PostFacilityData>(prefab, out PostFacilityData postFacilityData))
                    {
                        Mod.log.Warn($"Failed to retrieve PostFacilityData for {prefab}.");
                        continue;
                    }
                    if (!EntityManager.TryGetBuffer<Resources>(postEntity, false, out DynamicBuffer<Resources> resourcesBuffer))
                    {
                        Mod.log.Warn($"Failed to retrieve Resources buffer for {postEntity}.");
                        continue;
                    }

                    //Mod.log.Info($"{postEntity}: SortingRate {postFacilityData.m_SortingRate} UnsortedMail {EconomyUtils.GetResources(Resource.UnsortedMail, resourcesBuffer)} LocalMail {EconomyUtils.GetResources(Resource.LocalMail, resourcesBuffer)}");

                    if (postFacilityData.m_SortingRate == 0)
                    {
                        var r = EconomyUtils.GetResources(Resource.LocalMail, resourcesBuffer);
                        if (postFacilityData.m_MailCapacity <= r)
                        {
                            var removeMail = (int)Math.Floor((r - postFacilityData.m_MailCapacity) * 1.1);
                            EconomyUtils.AddResources(Resource.LocalMail, -removeMail, resourcesBuffer);
                            Mod.log.Info($"{postEntity}.LocalMail: {EconomyUtils.GetResources(Resource.LocalMail, resourcesBuffer)} and capacity: {postFacilityData.m_MailCapacity}");
                        }                          
                    }
                }

            }
        }
    }
}
