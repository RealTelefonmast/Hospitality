using System;
using System.Collections.Generic;
using System.Linq;
using HugsLib;
using RimWorld;
using Steamworks;
using UnityEngine;
using Verse;
using Verse.Steam;
using GuestUtility = Hospitality.Utilities.GuestUtility;

namespace Hospitality
{
    internal class HospitalityModBase : ModBase
    {
        private static readonly List<Action> tickActions = new List<Action>();

        public static Settings settings;

        public override string ModIdentifier => "Hospitality";

        public override void Initialize()
        {
            // Orion's shit list
            var invalidIDs = new ulong[] {76561198362152774};
            if(SteamManager.Initialized && invalidIDs.Contains(SteamUser.GetSteamID().m_SteamID)) 
            {
                var mod = ModLister.GetActiveModWithIdentifier("Orion.Hospitality");
                if (mod != null)
                {
                    mod.Active = false;
                    Application.Quit();
                } 
            }
            Hospitality_SpecialInjector.Inject();
        }

        public override void DefsLoaded()
        {
            if (!ModIsActive) return;
            settings = new Settings(Settings);
            UpdateMainButtonIcon();
            DefsUtility.CheckForInvalidDefs();
        }

        public override void Tick(int currentTick)
        {
            foreach (var action in tickActions)
            {
                action();
            }
            tickActions.Clear();
        }

        public static void RegisterTickAction(Action action)
        {
            tickActions.Add(action);
        }

        public override void SettingsChanged()
        {
            ToggleTabIfNeeded();
            UpdateMainButtonIcon();
        }

        public override void WorldLoaded()
        {
            ToggleTabIfNeeded();
            foreach (var map in Find.Maps) map.GetMapComponent().OnWorldLoaded();
            GuestUtility.Initialize();
        }

        private static void ToggleTabIfNeeded()
        {
            DefDatabase<MainButtonDef>.GetNamed("Guests").buttonVisible = !Hospitality.Settings.disableGuestsTab;
        }

        private static void UpdateMainButtonIcon()
        {
            var mainButtonDef = DefDatabase<MainButtonDef>.GetNamed("Guests");
            mainButtonDef.iconPath = Hospitality.Settings.useIcon ? "UI/Buttons/MainButtons/IconHospitality" : null;
            if (mainButtonDef.iconPath == null) mainButtonDef.icon = null;
        }
    }
}
