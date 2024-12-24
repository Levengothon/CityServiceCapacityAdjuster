using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Unity.Entities;

//test

namespace CityServiceCapacityAdjuster
{
    [FileLocation(nameof(CityServiceCapacityAdjuster))]
    [SettingsUIGroupOrder(kSliderGroup)]
    [SettingsUIShowGroupName(kSliderGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kSliderGroup = "Slider";

        public Setting(IMod mod) : base(mod)
        {
            if (CargoTruckSlider == 0) SetDefaults();
        }

        [SettingsUISlider(min = 1, max = 5, step = 1, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kSliderGroup)]
        public int CargoTruckSlider { get; set; }

        [SettingsUISlider(min = 1, max = 5, step = 1, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kSliderGroup)]
        public int PostSortingSlider { get; set; }

        [SettingsUISection(kSection, kSliderGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ResetSettings
        {
            set
            {
                SetDefaults();
                ApplyAndSave();
            }
        }

        [SettingsUISection(kSection, kSliderGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ApplySettings
        {
            set
            {
                ApplyAndSave();
            }
        }


        public override void SetDefaults()
        {
            CargoTruckSlider = 1;
            PostSortingSlider = 1;
        }

        public override void Apply()
        {
            base.Apply();
            var system = World.DefaultGameObjectInjectionWorld
                .GetExistingSystemManaged<CityServiceCapacityAdjusterSystem>();
            system.Enabled = true;
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
                { m_Setting.GetSettingsLocaleID(), "City Service Capacity Adjuster" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main Settings" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSliderGroup), "Capacity Sliders" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CargoTruckSlider)), "Cargo Station Truck Capacity" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.CargoTruckSlider)),
                    "Set multiplier for truck capacity in Cargo Station (base 16 * factor)"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PostSortingSlider)), "Post Office Sorting Rate" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.PostSortingSlider)),
                    "Set Multiplier for sorting speed in Sorting Facility (base 50k * factor)"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetSettings)), "Reset to defaults" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApplySettings)), "Apply changes" },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ResetSettings)), "Accept Reset" },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ApplySettings)), "Accept new values" }
            };
        }

        public void Unload()
        {
        }
    }
}