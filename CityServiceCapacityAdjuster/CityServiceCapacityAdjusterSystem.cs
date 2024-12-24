using System.Collections.Generic;
using System.Xml.Serialization;
using Colossal.IO.AssetDatabase.Internal;
using Colossal.Serialization.Entities;
using Game;
using Game.Companies;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace CityServiceCapacityAdjuster
{
    public partial class CityServiceCapacityAdjusterSystem : GameSystemBase
    {
        private readonly Dictionary<Entity, CargoTransportStationData> _cargoStationData =
            new Dictionary<Entity, CargoTransportStationData>();

        private readonly Dictionary<Entity, PostFacilityData> _postSortingData =
            new Dictionary<Entity, PostFacilityData>();

        private EntityQuery _queryA;
        private EntityQuery _queryB;

        private void UpdateCargoTransportStationTruckCapacity()
        {
            var xA = _queryA.ToEntityArray(Allocator.Temp);
            var TruckSliderFactor = Mod.m_Setting.CargoTruckSlider;

            var componentData2 = default(TransportCompanyData);
            componentData2.m_MaxTransports = TruckSliderFactor * 16;

            foreach (var x in xA)
            {
                CargoTransportStationData data;

                if (!_cargoStationData.TryGetValue(x, out data))
                {
                    data = EntityManager.GetComponentData<CargoTransportStationData>(x);
                    _cargoStationData.Add(x, data);
                }

                Mod.log.Info($"Truck multiplier: {TruckSliderFactor}, final capacity: {TruckSliderFactor * 16}");

                EntityManager.SetComponentData(x, componentData2);
            }
        }

        private void UpdatePostFacilitySortingRate()
        {
            var xB = _queryB.ToEntityArray(Allocator.Temp);
            var PostSortingSliderFactor = Mod.m_Setting.PostSortingSlider;

            foreach (var x in xB)
            {
                PostFacilityData data;

                if (!_postSortingData.TryGetValue(x, out data))
                {
                    data = EntityManager.GetComponentData<PostFacilityData>(x);
                    _postSortingData.Add(x, data);
                }

                Mod.log.Info($"Factor Post {PostSortingSliderFactor},{PostSortingSliderFactor * data.m_SortingRate}");

                data.m_SortingRate = PostSortingSliderFactor * data.m_SortingRate;
                EntityManager.SetComponentData(x, data);
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _queryA = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<CargoTransportStationData>()
                }
            });
          
            _queryB = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<PostFacilityData>()
                }
            });
           

            RequireForUpdate(_queryA);
            RequireForUpdate(_queryB);
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            Mod.log.Info("On game loading");
            base.OnGameLoadingComplete(purpose, mode);

            UpdateCargoTransportStationTruckCapacity();
            UpdatePostFacilitySortingRate();

            Enabled = false;
        }

        protected override void OnUpdate()
        {
            Mod.log.Info("On game update");
            if (Enabled)
            {
                UpdateCargoTransportStationTruckCapacity();
                UpdatePostFacilitySortingRate();

                Enabled = false;
            }
        }
    }
}