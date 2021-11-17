﻿using System.ComponentModel;
using System.Configuration;

namespace DerekWare.Iris.Properties
{
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    sealed partial class Settings
    {
        #region Event Handlers

        void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
        {
            // AddType code to handle the SettingChangingEvent event here.
        }

        void SettingsSavingEventHandler(object sender, CancelEventArgs e)
        {
            // AddType code to handle the SettingsSaving event here.
        }

        #endregion
    }
}
