using System;
using System.Collections.Generic;
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

        private readonly Dictionary<Entity, SchoolData> _schoolData =
            new Dictionary<Entity, SchoolData>();

        private readonly Dictionary<Entity, DeathcareFacilityData> _crematoryData =
            new Dictionary<Entity, DeathcareFacilityData>();

        private Dictionary<Entity, PublicTransportVehicleData> _transportVehicleData = 
            new Dictionary<Entity, PublicTransportVehicleData>();

        private Dictionary<Entity, TransportDepotData> _transportDepotData =
            new Dictionary<Entity, TransportDepotData>();

        private Dictionary<Entity, GarbageTruckData> _garbageTruckData =
            new Dictionary<Entity, GarbageTruckData>();


        private EntityQuery _queryA;
        private EntityQuery _queryB;
        private EntityQuery _queryC;
        private EntityQuery _queryD;
        private EntityQuery _queryE;
        private EntityQuery _queryF;
        private EntityQuery _queryG;

        //PathfindCarData d;
        //PathfindPedestrianData e;

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

        private void UpdateGarbageTruckCapacity()
        {
            var xA = _queryG.ToEntityArray(Allocator.Temp);
            var TruckSliderFactor = Mod.m_Setting.GarbageTruckCapacitySlider;


            foreach (var x in xA)
            {
                GarbageTruckData data;

                if (!_garbageTruckData.TryGetValue(x, out data))
                {
                    data = EntityManager.GetComponentData<GarbageTruckData>(x);
                    _garbageTruckData.Add(x, data);
                }

                data.m_GarbageCapacity = (int)Math.Round((double)data.m_GarbageCapacity * TruckSliderFactor);

                Mod.log.Info($"Truck multiplier: {TruckSliderFactor}, final capacity: {data.m_GarbageCapacity}");

                EntityManager.SetComponentData(x, data);
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

        private void UpdateSchoolCapacity()
        {
            var xA = _queryC.ToEntityArray(Allocator.Temp);
            var TruckSliderFactor = Mod.m_Setting.CargoTruckSlider;

            foreach (var x in xA)
            {
                SchoolData data;

                if (!_schoolData.TryGetValue(x, out data))
                {
                    data = EntityManager.GetComponentData<SchoolData>(x);
                    _schoolData.Add(x, data);
                }

                var educationLevel = data.m_EducationLevel;
                float scalar = 0;
                switch (educationLevel)
                {
                    case (byte)SchoolLevel.Elementary: scalar = Mod.m_Setting.ElementrySchoolSlider; break;
                    case (byte)SchoolLevel.HighSchool: scalar = Mod.m_Setting.HighSchoolSlider; break;
                    case (byte)SchoolLevel.College: scalar = Mod.m_Setting.CollegeSlider; break;
                    case (byte)SchoolLevel.University: scalar = Mod.m_Setting.UniversitySlider; break;
                };

                if (scalar == 0) continue;

                int newCapacity = (int)Math.Round((decimal)(data.m_StudentCapacity * scalar));

                Mod.log.Info($"School Type: {educationLevel}, current capacity: {data.m_StudentCapacity}, sclar {scalar}, new {newCapacity}");

                data.m_StudentCapacity = newCapacity;
                EntityManager.SetComponentData(x, data);
            }
        }

        private void UpdateCrematoriumCapacity()
        {
            var xA = _queryD.ToEntityArray(Allocator.Temp);
            var CrematoriumSpeedSliderFactor = Mod.m_Setting.CrematoriumSpeedSlider;
            var CrematoriumCapcitySliderFactor = Mod.m_Setting.CrematoriumCapcitySlider;
            var CrematoriumHearseSliderFactor = Mod.m_Setting.CrematoriumHearseSlider;

            foreach (var x in xA)
            {
                DeathcareFacilityData data;

                if (!_crematoryData.TryGetValue(x, out data))
                {
                    data = EntityManager.GetComponentData<DeathcareFacilityData>(x);
                    _crematoryData.Add(x, data);
                }

                int newCapacity = (int)Math.Round((decimal)(data.m_StorageCapacity * CrematoriumCapcitySliderFactor));
                int newHerseCapacity = (int)Math.Round((decimal)(data.m_HearseCapacity * CrematoriumHearseSliderFactor));
                int newSpeed = (int)Math.Round((decimal)(data.m_ProcessingRate * CrematoriumSpeedSliderFactor));

                Mod.log.Info($"Crematory Capacity {data.m_StorageCapacity}, sclar {CrematoriumCapcitySliderFactor}, new {newCapacity}");
                Mod.log.Info($"Crematory Herse Capacity {data.m_HearseCapacity}, sclar {CrematoriumHearseSliderFactor}, new {newHerseCapacity}");
                Mod.log.Info($"Crematory Speed {data.m_ProcessingRate}, sclar {CrematoriumSpeedSliderFactor}, new {newSpeed}");

                data.m_StorageCapacity = newCapacity;
                data.m_HearseCapacity = newHerseCapacity;
                data.m_ProcessingRate = newSpeed;
                EntityManager.SetComponentData(x, data);
            }
        }

        private void UpdatePublicTransportPeopleCapacity()
        {

            var transports = _queryE.ToEntityArray(Allocator.Temp);

            foreach (var trans in transports)
            {
                PublicTransportVehicleData data;

                if (!_transportVehicleData.TryGetValue(trans, out data))
                {
                    data = EntityManager.GetComponentData<PublicTransportVehicleData>(trans);
                    _transportVehicleData.Add(trans, data);
                }

                float scalar = 0;
                switch (data.m_TransportType)
                {
                    case TransportType.Bus: scalar = Mod.m_Setting.PeopleCapacityBusSlider; break;
                    case TransportType.Taxi: scalar = Mod.m_Setting.PeopleCapacityTaxiSlider; break;
                    case TransportType.Subway: scalar = Mod.m_Setting.PeopleCapacitySubwaySlider; break;
                    case TransportType.Tram: scalar = Mod.m_Setting.PeopleCapacityTramSlider; break;
                    case TransportType.Train: scalar = Mod.m_Setting.PeopleCapacityTrainSlider; break;
                    case TransportType.Airplane: scalar = Mod.m_Setting.PeopleCapacityAirplaneSlider; break;
                    case TransportType.Ship: scalar = Mod.m_Setting.PeopleCapacityShipSlider; break;
                };

                if (scalar == 0) continue;

                int newCapacity = (int)Math.Round((decimal)(data.m_PassengerCapacity * scalar));

                Mod.log.Info($"Transport Type: {data.m_TransportType}, current capacity: {data.m_PassengerCapacity}, sclar {scalar}, new {newCapacity}");

                data.m_PassengerCapacity = newCapacity;
                EntityManager.SetComponentData<PublicTransportVehicleData>(trans, data);
            }

            Enabled = false;
        }

        private void UpdatePublicTransportDepotCapacity()
        {

            var transports = _queryF.ToEntityArray(Allocator.Temp);

            foreach (var trans in transports)
            {
                TransportDepotData data;

                if (!_transportDepotData.TryGetValue(trans, out data))
                {
                    data = EntityManager.GetComponentData<TransportDepotData>(trans);
                    _transportDepotData.Add(trans, data);
                }

                float scalar = 0;
                switch (data.m_TransportType)
                {
                    case TransportType.Bus: scalar = Mod.m_Setting.DepotCapacityBusSlider; break;
                    case TransportType.Taxi: scalar = Mod.m_Setting.DepotCapacityTaxiSlider; break;
                    case TransportType.Subway: scalar = Mod.m_Setting.DepotCapacitySubwaySlider; break;
                    case TransportType.Tram: scalar = Mod.m_Setting.DepotCapacityTramSlider; break;
                    case TransportType.Train: scalar = Mod.m_Setting.DepotCapacityTrainSlider; break;
                };

                if (scalar == 0) continue;

                int newCapacity = (int)Math.Round((decimal)(data.m_VehicleCapacity * scalar));

                Mod.log.Info($"Transport Type: {data.m_TransportType}, current capacity: {data.m_VehicleCapacity}, sclar {scalar}, new {newCapacity}");

                data.m_VehicleCapacity = newCapacity;
                EntityManager.SetComponentData<TransportDepotData>(trans, data);
            }

            Enabled = false;
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

            _queryC = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<SchoolData>()
                }
            });

            _queryD = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<DeathcareFacilityData>()
                }
            });

            _queryE = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] 
                {
                    ComponentType.ReadWrite<PublicTransportVehicleData>()
                }
            });

            _queryF = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<TransportDepotData>()
                }
            });

            _queryG = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
    {
                    ComponentType.ReadWrite<GarbageTruckData>()
                }
            });


            RequireForUpdate(_queryA);
            RequireForUpdate(_queryB);
            RequireForUpdate(_queryC);
            RequireForUpdate(_queryD);
            RequireForUpdate(_queryE);
            RequireForUpdate(_queryF);
            RequireForUpdate(_queryG);
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            Mod.log.Info($"On game loading {purpose.ToString()}, {mode.ToString()}");
            base.OnGameLoadingComplete(purpose, mode);

            UpdateCargoTransportStationTruckCapacity();
            UpdatePostFacilitySortingRate();
            UpdateSchoolCapacity();
            UpdateCrematoriumCapacity();
            UpdatePublicTransportPeopleCapacity();
            UpdatePublicTransportDepotCapacity();
            UpdateGarbageTruckCapacity();

            Enabled = false;
        }

        protected override void OnUpdate()
        {
            Mod.log.Info("On game update");
            if (Enabled)
            {
                UpdateCargoTransportStationTruckCapacity();
                UpdatePostFacilitySortingRate();
                UpdateSchoolCapacity();
                UpdateCrematoriumCapacity();
                UpdatePublicTransportPeopleCapacity();
                UpdatePublicTransportDepotCapacity();
                UpdateGarbageTruckCapacity();

                Enabled = false;
            }
        }
    }
}