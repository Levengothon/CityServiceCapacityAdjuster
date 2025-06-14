using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using Unity.Entities;

//test

namespace CityServiceCapacityAdjuster
{

    [FileLocation(nameof(CityServiceCapacityAdjuster))]
    [SettingsUIGroupOrder(kCityServiceGroup, kSchoolGroup, kPublicTransportGroup, kCrematoryGroup, kPathfindingGroup)]
    [SettingsUIShowGroupName(kCityServiceGroup, kSchoolGroup, kPublicTransportGroup, kCrematoryGroup, kPathfindingGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kCityServiceGroup = "City Service Adjuster";
        public const string kPublicTransportGroup = "Public Transport Adjuster";
        public const string kCrematoryGroup = "Crematory Adjuster";
        public const string kPathfindingGroup = "Path Finding Adjuster";
        public const string kSchoolGroup = "School Capacity Adjuster";

        private bool otherChanged = false;
        private bool pathfindingChanged = false;
        private bool publicTransportChanged = false;
        private bool crematoryChanged = false;
        private bool schoolChanged = false;

        public Setting(IMod mod) : base(mod)
        {
            if (CargoTruckSlider == 0)
            {
                otherChanged = true;
                SetDefaults();
            }

            if (GarbageTruckCapacitySlider == 0)
            {
                GarbageTruckCapacitySlider = 1;
            }

            if (DepotCapacityBusSlider == 0 || PeopleCapacityBusSlider == 0)
            {
                publicTransportChanged = true;
                SetDefaults();
            }

            if (CrematoriumCapcitySlider == 0)
            {
                crematoryChanged = true;
                SetDefaults();
            }

            if (ElementrySchoolSlider == 0 || HighSchoolSlider == 0 || CollegeSlider == 0)
            {
                schoolChanged = true;
                SetDefaults();
            }
        }

        /* Other */
        [SettingsUISlider(min = 1, max = 5, step = 1, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kCityServiceGroup)]
        public int CargoTruckSlider { get; set; }

        [SettingsUISlider(min = 1, max = 5, step = 1, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kCityServiceGroup)]
        public int PostSortingSlider { get; set; }

        [SettingsUISection(kSection, kCityServiceGroup)]
        public bool FixMailOverflow { get; set; }

        [SettingsUISlider(min = 1, max = 5, step = 1, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kCityServiceGroup)]
        public int GarbageTruckCapacitySlider { get; set; }
        /*
        [SettingsUISection(kSection, kCityServiceGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ResetOtherSettings
        {
            set
            {
                otherChanged = true;
                SetDefaults();
                ApplyAndSave();
            }
        }
        */
        [SettingsUISection(kSection, kCityServiceGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ApplyOtherSettings
        {
            set
            {
                otherChanged = true;
                ApplyAndSave();
            }
        }

        //Public Transport
        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float DepotCapacityBusSlider { get; set; }

        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float DepotCapacityTaxiSlider { get; set; }

        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float DepotCapacityTramSlider { get; set; }

        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float DepotCapacityTrainSlider { get; set; }

        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float DepotCapacitySubwaySlider { get; set; }

        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float PeopleCapacityBusSlider { get; set; }

        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float PeopleCapacityTaxiSlider { get; set; }

        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float PeopleCapacityTramSlider { get; set; }

        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float PeopleCapacityTrainSlider { get; set; }

        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float PeopleCapacitySubwaySlider { get; set; }

        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float PeopleCapacityAirplaneSlider { get; set; }

        [SettingsUISlider(min = 1f, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kPublicTransportGroup)]
        public float PeopleCapacityShipSlider { get; set; }
        /*
        [SettingsUISection(kSection, kPublicTransportGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ResetPublicTransportSettings
        {
            set
            {
                publicTransportChanged = true;
                SetDefaults();
                ApplyAndSave();
            }
        }
        */
        [SettingsUISection(kSection, kPublicTransportGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ApplyPublicTransportSettings
        {
            set
            {
                publicTransportChanged = true;
                ApplyAndSave();
            }
        }

        //Crematory
        [SettingsUISlider(min = 1, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kCrematoryGroup)]
        public float CrematoriumSpeedSlider { get; set; }

        [SettingsUISlider(min = 1, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kCrematoryGroup)]
        public float CrematoriumCapcitySlider { get; set; }

        [SettingsUISlider(min = 1, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kCrematoryGroup)]
        public float CrematoriumHearseSlider { get; set; }
        /*
        [SettingsUISection(kSection, kCrematoryGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ResetCrematorySettings
        {
            set
            {
                crematoryChanged = true;
                SetDefaults();
                ApplyAndSave();
            }
        }
        */
        [SettingsUISection(kSection, kCrematoryGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ApplyCrematorySettings
        {
            set
            {
                crematoryChanged = true;
                ApplyAndSave();
            }
        }

        /* SCHOOL */

        [SettingsUISlider(min = 1, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kSchoolGroup)]
        public float ElementrySchoolSlider { get; set; }

        [SettingsUISlider(min = 1, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kSchoolGroup)]
        public float HighSchoolSlider { get; set; }

        [SettingsUISlider(min = 1, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kSchoolGroup)]
        public float CollegeSlider { get; set; }

        [SettingsUISlider(min = 1, max = 5f, step = 0.1f, scalarMultiplier = 1, unit = Unit.kFloatSingleFraction)]
        [SettingsUISection(kSection, kSchoolGroup)]
        public float UniversitySlider { get; set; }

        [SettingsUISection(kSection, kSchoolGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ApplySchoolSettings
        {
            set
            {
                schoolChanged = true;
                ApplyAndSave();
            }
        }

        /* TRAFFIC */

        [SettingsUISection(kSection, kPathfindingGroup)]
        [SettingsUISlider(min = -50, max = 1000, step = 50, scalarMultiplier = 1, unit = Unit.kInteger)]
        public int PedestrianCrosswalkReduction { get; set; }

        [SettingsUISection(kSection, kPathfindingGroup)]
        [SettingsUISlider(min = -100, max = 5000, step = 50, scalarMultiplier = 1, unit = Unit.kInteger)]
        public int CarForbiddenCostSlider { get; set; }
        
        [SettingsUISection(kSection, kPathfindingGroup)]
        [SettingsUISlider(min = -100, max = 5000, step = 50, scalarMultiplier = 1, unit = Unit.kInteger)]
        public int CarLaneCrossCostSlider { get; set; }
        
        [SettingsUISection(kSection, kPathfindingGroup)]
        [SettingsUISlider(min = -100, max = 5000, step = 50, scalarMultiplier = 1, unit = Unit.kInteger)]
        public int CarUnsafeTurningCostSlider { get; set; }
        
        [SettingsUISection(kSection, kPathfindingGroup)]
        [SettingsUISlider(min = -100, max = 5000, step = 50, scalarMultiplier = 1, unit = Unit.kInteger)]
        public int CarUnsafeUTurnCostSlider { get; set; }
        
        [SettingsUISection(kSection, kPathfindingGroup)]
        [SettingsUISlider(min = -100, max = 5000, step = 50, scalarMultiplier = 1, unit = Unit.kInteger)]
        public int CarUTurnCostSlider { get; set; }    
        
        [SettingsUISection(kSection, kPathfindingGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ApplyPathfindingSettings
        {
            set
            {
                pathfindingChanged = true;
                ApplyAndSave();
            }
        }


        public override void SetDefaults()
        {
            if (otherChanged)
            {
                CargoTruckSlider = 1;
                PostSortingSlider = 1;
                GarbageTruckCapacitySlider = 1;

                otherChanged = false;
            }

            if (publicTransportChanged)
            {
                DepotCapacityBusSlider = 1f;
                DepotCapacitySubwaySlider = 1f;
                DepotCapacityTaxiSlider = 1f;
                DepotCapacityTrainSlider = 1f;
                DepotCapacityTramSlider = 1f;

                PeopleCapacityBusSlider = 1f;
                PeopleCapacityShipSlider = 1f;
                PeopleCapacitySubwaySlider = 1f;
                PeopleCapacityTaxiSlider = 1f;
                PeopleCapacityTrainSlider = 1f;
                PeopleCapacityTramSlider = 1f;
                PeopleCapacityAirplaneSlider = 1f;

                publicTransportChanged = false;
            }

            if (crematoryChanged)
            {
                CrematoriumSpeedSlider = 1;
                CrematoriumCapcitySlider = 1;
                CrematoriumHearseSlider = 1;

                crematoryChanged = false;
            }

            if (schoolChanged)
            {
                ElementrySchoolSlider = 1;
                HighSchoolSlider = 1;
                CollegeSlider = 1;
                UniversitySlider = 1;

                schoolChanged = false;
            }
        }

        public override void Apply()
        {
            base.Apply();
            if (otherChanged || schoolChanged || publicTransportChanged || crematoryChanged)
            {
                var system = World.DefaultGameObjectInjectionWorld
                    .GetExistingSystemManaged<CityServiceCapacityAdjusterSystem>();
                system.Enabled = true;
                otherChanged = false;
                publicTransportChanged = false;
                crematoryChanged = false;
                schoolChanged = false;
            }

            if (pathfindingChanged)
            {
                var system = World.DefaultGameObjectInjectionWorld
                    .GetExistingSystemManaged<PathfindingCostAdjusterSystem>();
                system.Enabled = true;
                pathfindingChanged = false;
            }
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "City Service Capacity and Traffic Adjuster" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main Settings" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kCityServiceGroup), "Generic Capacity Sliders" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kSchoolGroup), "School Capacity Sliders" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kPathfindingGroup), "Pathfinding Reducer" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kPublicTransportGroup), "Public Transport Sliders" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCrematoryGroup), "Crematory Sliders" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CargoTruckSlider)), "[Cargo Station] Truck capacity multiplier" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CargoTruckSlider)),"Set multiplier for truck capacity in Cargo Station (base 16 * factor)" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PostSortingSlider)), "[Post Office] Sorting Rate" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PostSortingSlider)),"Set Multiplier for sorting speed in Sorting Facility (base 50k * factor)" },
                
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.FixMailOverflow)), "[Post Office] Fix Mail Overflow" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.FixMailOverflow)),"When the post office is full sometimes it stops sending new trucks. This options remove some of the mails to fix this issue." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.GarbageTruckCapacitySlider)), "[Garbage Truck] Vehicle Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.GarbageTruckCapacitySlider)),"Set Multiplier for capacity of grabage truck (base 2t * factor)" },


                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CrematoriumSpeedSlider)), "Processing Speed Rate" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CrematoriumSpeedSlider)),"Set Multiplier for processing speed rate in Crematorium" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CrematoriumCapcitySlider)), "Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CrematoriumCapcitySlider)),"Set Multiplier for capacity in Crematorium" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CrematoriumHearseSlider)), "Herse Multiplier" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CrematoriumHearseSlider)),"Set Multiplier for amount of herse in Crematorium" },


                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PeopleCapacityBusSlider)), "Bus Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PeopleCapacityBusSlider)),"Set Multiplier for bus capacity" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PeopleCapacityAirplaneSlider)), "Airplane Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PeopleCapacityAirplaneSlider)),"Set Multiplier for airplane capacity" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PeopleCapacityShipSlider)), "Ship Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PeopleCapacityShipSlider)),"Set Multiplier for ship capacity" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PeopleCapacitySubwaySlider)), "Subway Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PeopleCapacitySubwaySlider)),"Set Multiplier for subway capacity" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PeopleCapacityTaxiSlider)), "Taxi Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PeopleCapacityTaxiSlider)),"Set Multiplier for taxi capacity" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PeopleCapacityTrainSlider)), "Train Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PeopleCapacityTrainSlider)),"Set Multiplier for train capacity" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PeopleCapacityTramSlider)), "Tram Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PeopleCapacityTramSlider)),"Set Multiplier for tram capacity" },


                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DepotCapacityBusSlider)), "Bus Depot Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DepotCapacityBusSlider)),"Set Multiplier for bus depot capacity" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DepotCapacitySubwaySlider)), "Subway Depot Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DepotCapacitySubwaySlider)),"Set Multiplier for subway depot capacity" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DepotCapacityTaxiSlider)), "Taxi Depot Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DepotCapacityTaxiSlider)),"Set Multiplier for taxi depot capacity" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DepotCapacityTrainSlider)), "Train Depot Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DepotCapacityTrainSlider)),"Set Multiplier for train depot capacity" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DepotCapacityTramSlider)), "Tram Depot Capacity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DepotCapacityTramSlider)),"Set Multiplier for tram depot capacity" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ElementrySchoolSlider)), "Elementry School Capacity" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ElementrySchoolSlider)),
                    "Set Multiplier for school type capacity (base * factor)"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HighSchoolSlider)), "High School Capacity" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.HighSchoolSlider)),
                    "Set Multiplier for school type capacity (base * factor)"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CollegeSlider)), "College Capacity" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.CollegeSlider)),
                    "Set Multiplier for school type capacity (base * factor)"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UniversitySlider)), "University Capacity" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UniversitySlider)),
                    "Set Multiplier for school type capacity (base * factor)"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PedestrianCrosswalkReduction)), "Add or Reduce Pedestrian Crosswalk Cost" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PedestrianCrosswalkReduction)), "Set 0 for vanilla values" },
                
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CarForbiddenCostSlider)), "Add or Reduce Forbidden action Cost" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CarForbiddenCostSlider)), "Set 0 for vanilla values" },
                
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CarLaneCrossCostSlider)), "Add or Reduce Lane Cross Cost" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CarLaneCrossCostSlider)), "Set 0 for vanilla values" },
                
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CarUnsafeTurningCostSlider)), "Add or Reduce Unsafe Turning Cost" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CarUnsafeTurningCostSlider)), "Set 0 for vanilla values" },
                
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CarUnsafeUTurnCostSlider)), "Add or Reduce Unsafe U-Turn Cost" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CarUnsafeUTurnCostSlider)), "Set 0 for vanilla values" },
                
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CarUTurnCostSlider)), "Add or Reduce U-Turn Cost" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CarUTurnCostSlider)), "Set 0 for vanilla values" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApplyOtherSettings)), "Apply changes" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApplyPathfindingSettings)), "Apply changes" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApplySchoolSettings)), "Apply changes" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApplyCrematorySettings)), "Apply changes" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApplyPublicTransportSettings)), "Apply changes" },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ApplyOtherSettings)), "Accept new values" },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ApplyPathfindingSettings)), "Accept new values" },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ApplySchoolSettings)), "Accept new values" },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ApplyCrematorySettings)), "Accept new values" },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ApplyPublicTransportSettings)), "Accept new values" },

                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ApplyOtherSettings)), "Accept new values" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ApplyPathfindingSettings)), "Accept new values" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ApplySchoolSettings)), "Accept new values" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ApplyCrematorySettings)), "Accept new values" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ApplyPublicTransportSettings)), "Accept new values" },

            };
        }

        public void Unload()
        {
        }
    }
}