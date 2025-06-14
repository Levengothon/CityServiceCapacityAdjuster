using System;
using System.Collections.Generic;
using Game;
using Game.Buildings;
using Game.Citizens;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Purpose = Colossal.Serialization.Entities.Purpose;

namespace CityServiceCapacityAdjuster
{
    public partial class PathfindingCostAdjusterSystem : GameSystemBase
    {
        private readonly Dictionary<Entity, PathfindPedestrianData> _CitizenPathfindingData =
            new Dictionary<Entity, PathfindPedestrianData>();

        private readonly Dictionary<Entity, PathfindCarData> _CarPathfindingData =
            new Dictionary<Entity, PathfindCarData>();

        //BuildingPropertyData x;

        private EntityQuery _queryA;
        private EntityQuery _queryB;

        protected override void OnCreate()
        {
            base.OnCreate();

            _queryA = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<PathfindPedestrianData>()
                }
            });

            _queryB = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<PathfindCarData>()
                }
            });

            RequireForUpdate(_queryA);
            RequireForUpdate(_queryB);
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            Mod.log.Info("On game loading - Pathfinding");
            base.OnGameLoadingComplete(purpose, mode);

            UpdateCitizendPathfindingCost();
            UpdateCarPathfindingCost();
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            if (Enabled)
            {
                UpdateCitizendPathfindingCost();
                UpdateCarPathfindingCost();

                Enabled = false;
            }
        }

        private void UpdateCitizendPathfindingCost()
        {
            var reduceCitizienCrosswalk = Mod.m_Setting.PedestrianCrosswalkReduction;
            var xA = _queryA.ToEntityArray(Allocator.Temp);
            float newCost;

            foreach (var x in xA)
            {

                if (!_CitizenPathfindingData.TryGetValue(x, out PathfindPedestrianData data))
                {
                    data = EntityManager.GetComponentData<PathfindPedestrianData>(x);
                    _CitizenPathfindingData.Add(x, data);
                }
                
                newCost = data.m_UnsafeCrosswalkCost.m_Value.y + reduceCitizienCrosswalk;

                Mod.log.Info($"Citizen old value: {data.m_UnsafeCrosswalkCost.m_Value} / new cost: {newCost}");

                data.m_UnsafeCrosswalkCost.m_Value.y = newCost;

                Mod.log.Info($"Citizen current value: {data.m_UnsafeCrosswalkCost.m_Value}");

                EntityManager.SetComponentData(x, data);
            }
        }

        private void UpdateCarPathfindingCost()
        {
            var xA = _queryB.ToEntityArray(Allocator.Temp);
            float newForbiddenCost, newLaneCrossCost, newUnsafeTurningCost, newUnsafeUTurnCost, newUTurnCost;

            foreach (var x in xA)
            {
                if (!_CarPathfindingData.TryGetValue(x, out PathfindCarData data))
                {
                    data = EntityManager.GetComponentData<PathfindCarData>(x);
                    _CarPathfindingData.Add(x, data);
                }
                
                newForbiddenCost = Math.Max(0, data.m_ForbiddenCost.m_Value.y + Mod.m_Setting.CarForbiddenCostSlider);
                newLaneCrossCost = Math.Max(0, data.m_LaneCrossCost.m_Value.y + Mod.m_Setting.CarLaneCrossCostSlider);;
                newUnsafeTurningCost = Math.Max(0, data.m_UnsafeTurningCost.m_Value.y + Mod.m_Setting.CarUnsafeTurningCostSlider);;
                newUnsafeUTurnCost = Math.Max(0, data.m_UnsafeUTurnCost.m_Value.y + Mod.m_Setting.CarUnsafeUTurnCostSlider);;
                newUTurnCost = Math.Max(0, data.m_UTurnCost.m_Value.y + Mod.m_Setting.CarUTurnCostSlider);;
               
                Mod.log.Info($"Car old m_ForbiddenCost value: {data.m_ForbiddenCost.m_Value}");
                Mod.log.Info($"Car old m_LaneCrossCost value: {data.m_LaneCrossCost.m_Value}");
                Mod.log.Info($"Car old m_UnsafeTurningCost value: {data.m_UnsafeTurningCost.m_Value}");
                Mod.log.Info($"Car old m_UnsafeUTurnCost value: {data.m_UnsafeUTurnCost.m_Value}");
                Mod.log.Info($"Car old m_UTurnCost value: {data.m_UTurnCost.m_Value}");

                data.m_ForbiddenCost.m_Value.y = newForbiddenCost;
                data.m_LaneCrossCost.m_Value.y = newLaneCrossCost;
                data.m_UnsafeTurningCost.m_Value.y = newUnsafeTurningCost;
                data.m_UnsafeUTurnCost.m_Value.y = newUnsafeUTurnCost;
                data.m_UTurnCost.m_Value.y = newUTurnCost;

                Mod.log.Info($"Car new m_ForbiddenCost value: {data.m_ForbiddenCost.m_Value}");
                Mod.log.Info($"Car new m_LaneCrossCost value: {data.m_LaneCrossCost.m_Value}");
                Mod.log.Info($"Car new m_UnsafeTurningCost value: {data.m_UnsafeTurningCost.m_Value}");
                Mod.log.Info($"Car new m_UnsafeUTurnCost value: {data.m_UnsafeUTurnCost.m_Value}");
                Mod.log.Info($"Car new m_UTurnCost value: {data.m_UTurnCost.m_Value}");

                EntityManager.SetComponentData(x, data);
            }

        }

    }
}
