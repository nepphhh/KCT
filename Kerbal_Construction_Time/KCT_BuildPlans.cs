﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using ToolbarControl_NS;

namespace KerbalConstructionTime
{
    public static partial class KCT_GUI
    {
        static GUIStyle buildPlansbutton;
        static Texture2D background;
        static GUIContent upContent;
        static GUIContent hoverContent;
        static Rect rect;
        static float scale;
        static GUIContent content;
        //static bool buildPlansInitted = false;

        static SortedList<string, KCT_BuildListVessel> plansList = null;
        static int planToDelete;
        static Texture2D up;
        static Texture2D hover;

        internal static void InitBuildPlans()
        {
            //buildPlansInitted = true;
            buildPlansbutton = new GUIStyle(HighLogic.Skin.button);
            buildPlansbutton.margin = new RectOffset(0, 0, 0, 0);
            buildPlansbutton.padding = new RectOffset(0, 0, 0, 0);
            buildPlansbutton.border = new RectOffset(0, 0, 0, 0);
            buildPlansbutton.normal = buildPlansbutton.hover;
            buildPlansbutton.active = buildPlansbutton.hover;

            background = new Texture2D(2, 2);
            Color[] color = new Color[4];
            color[0] = new Color(1, 1, 1, 0);
            color[1] = color[0];
            color[2] = color[0];
            color[3] = color[0];
            background.SetPixels(color);

            buildPlansbutton.normal.background = background;
            buildPlansbutton.hover.background = background;
            buildPlansbutton.onHover.background = background;
            buildPlansbutton.active.background = background;
            buildPlansbutton.onActive.background = background;

            // rect = new Rect(Screen.width - 260, 0, 34, 34);

            //ALPresent = AssemblyLoader.loadedAssemblies.Any(a => a.assembly.GetName().Name == "AviationLights");

            up = new Texture2D(2, 2);
            hover = new Texture2D(2, 2);
            ToolbarControl.LoadImageFromFile(ref up, KSPUtil.ApplicationRootPath + "GameData/" + "KerbalConstructionTime/PluginData/Icons/KCT_add_normal");
            ToolbarControl.LoadImageFromFile(ref hover, KSPUtil.ApplicationRootPath + "GameData/" + "KerbalConstructionTime/PluginData/Icons/KCT_add_hover");
            //up = GameDatabase.Instance.GetTexture("KerbalConstructionTime/PluginData/Icons/KCT_add_normal", false);
            //hover = GameDatabase.Instance.GetTexture("KerbalConstructionTime/PluginData/Icons/KCT_add_hover", false);

            PositionAndSizeIcon();
        }

        static void PositionAndSizeIcon()
        {
            Texture2D upTex = Texture2D.Instantiate(up);
            Texture2D hoverTex = Texture2D.Instantiate(hover);

            int offset = 0;
            bool steamPresent = AssemblyLoader.loadedAssemblies.Any(a => a.assembly.GetName().Name == "KSPSteamCtrlr");
            bool mechjebPresent = AssemblyLoader.loadedAssemblies.Any(a => a.assembly.GetName().Name == "MechJeb2");
            if (steamPresent)
                offset = 46;
            if (mechjebPresent)
                offset = 140;
            scale = GameSettings.UI_SCALE;

            rect = new Rect(Screen.width - (260 + offset) * scale, 0, 42 * scale, 38 * scale);
            {
                TextureScale.Bilinear(upTex, (int)(up.width * scale), (int)(up.height * scale));
                TextureScale.Bilinear(hoverTex, (int)(hover.width * scale), (int)(hover.height * scale));
            }
            upContent = new GUIContent("", upTex, "");
            hoverContent = new GUIContent("", hoverTex, "");
        }

        private static void DoBuildPlansList()
        {
#if false
            if (!buildPlansInitted)
                InitBuildPlans();                
#endif
            
            if (rect.Contains(Mouse.screenPos))
                content = hoverContent;
            else
                content = upContent;
            if (scale != GameSettings.UI_SCALE)
            {
                PositionAndSizeIcon();
            }
            // When this is true, and the mouse is NOT over the toggle, the toggle code is making the toggle active
            // which is showing the corners of the button as unfilled
            showBuildPlansWindow = GUI.Toggle(rect, showBuildPlansWindow, content, buildPlansbutton);
        }

        private static void DrawBuildPlansWindow(int id)
        {
            int butW = 20;

            GUILayout.BeginVertical();
            if (HighLogic.LoadedSceneIsEditor)
            {
                if (EditorLogic.fetch.ship != null && EditorLogic.fetch.ship.Parts != null && EditorLogic.fetch.ship.Parts.Count > 0)
                {
                    if (EditorLogic.fetch.ship.shipName == "Untitled Space Craft" || EditorLogic.fetch.ship.shipName == "")
                    {
                        
                        if (GUILayout.Button("Cannot Add a Plan Without a Valid Name", GUILayout.Height(2 * 22)))
                        {
                            if (EditorLogic.fetch.ship.shipName == "Untitled Space Craft")
                            {
                                var message = new ScreenMessage("[KCT] Vessel must have a name other than 'Untitled Space Craft'.", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                                ScreenMessages.PostScreenMessage(message);
                            } else
                            {
                                var message = new ScreenMessage("[KCT] Vessel must have a name", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                                ScreenMessages.PostScreenMessage(message);
                            }
                        }
                        
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add To Building Plans", GUILayout.Height(2 * 22)))
                        {
                            AddVesselToPlansList();
                        }
                        GUILayout.EndHorizontal();
                        //if (!KCT_GameStates.settings.OverrideLaunchButton)
                        {
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Build", GUILayout.Height(2 * 22)))
                            {
                                KCT_Utilities.AddVesselToBuildList();
                                //SwitchCurrentPartCategory();
                                KCT_Utilities.RecalculateEditorBuildTime(EditorLogic.fetch.ship);
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                else
                {
                    GUILayout.Button("No vessel available", GUILayout.Height(2 * 22));
                }
            }
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();            
            GUILayout.FlexibleSpace();
            GUILayout.Label("Available Building Plans");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            bool VABSelectedNew = GUILayout.Toggle(VABSelected, "VAB", GUI.skin.button);
            bool SPHSelectedNew = GUILayout.Toggle(SPHSelected, "SPH", GUI.skin.button);

            if (VABSelectedNew != VABSelected)
                SelectList("VAB");
            else if (SPHSelectedNew != SPHSelected)
                SelectList("SPH");

            GUILayout.EndHorizontal();
            {
                switch (listWindow)
                {
                    case 0:
                         plansList = KCT_GameStates.ActiveKSC.VABPlans;
                        break;
                    case 1:
                         plansList = KCT_GameStates.ActiveKSC.SPHPlans;
                        break;
                }
                if (listWindow >= 0 && plansList != null)
                {
                    GUILayout.BeginHorizontal();
                    //  GUILayout.Space((butW + 4) * 3);
                    GUILayout.Label("Name:");
                    GUILayout.EndHorizontal();
                    scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(250));

                    if (plansList.Count == 0)
                    {
                        GUILayout.Label("No vessels in plans.");
                    }
                    for (int i = 0; i < plansList.Count; i++)
                    {
                        KCT_BuildListVessel b = plansList.Values[i];
                        if (!b.allPartsValid)
                            continue;
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("X", redButton,  GUILayout.Width(butW)))
                            {
                                planToDelete = i;
                                InputLockManager.SetControlLock(ControlTypes.EDITOR_SOFT_LOCK, "KCTPopupLock");
                                IDSelected = b.id;
                                DialogGUIBase[] options = new DialogGUIBase[2];
                                options[0] = new DialogGUIButton("Yes", RemoveVesselFromPlans);
                                options[1] = new DialogGUIButton("No", DummyVoid);
                                MultiOptionDialog diag = new MultiOptionDialog("scrapVesselPopup", "Are you sure you want to remove this vessel from the plans?", "Delete plan", null, options: options);
                                PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), diag, false, HighLogic.UISkin);
                            }
       
                            if (GUILayout.Button(b.shipName))
                            {
                                KCT_Utilities.AddVesselToBuildList(b.NewCopy(true));
                            }
                        }
                     
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close"))
                {
                    showBuildPlansWindow = false;
                }
            }

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        // Following is mostly duplicating the AddVesselToBuildList set of methods
        public static KCT_BuildListVessel AddVesselToPlansList()
        {
            return AddVesselToPlansList(EditorLogic.fetch.launchSiteName);
        }

        public static KCT_BuildListVessel AddVesselToPlansList(string launchSite)
        {
            if (string.IsNullOrEmpty(launchSite))
            {
                launchSite = EditorLogic.fetch.launchSiteName;
            }
            double effCost = KCT_Utilities.GetEffectiveCost(EditorLogic.fetch.ship.Parts);
            double bp = KCT_Utilities.GetBuildTime(effCost);
            KCT_BuildListVessel blv = new KCT_BuildListVessel(EditorLogic.fetch.ship, launchSite, effCost, bp, EditorLogic.FlagURL);
            blv.shipName = EditorLogic.fetch.shipNameField.text;
            return AddVesselToPlansList(blv);
        }

        public static KCT_BuildListVessel AddVesselToPlansList(KCT_BuildListVessel blv)
        {
            ScreenMessage message;
            if (KCT_Utilities.CurrentGameIsCareer())
            {
                //Check upgrades
                //First, mass limit
                List<string> facilityChecks = blv.MeetsFacilityRequirements(true);
                if (facilityChecks.Count != 0)
                {
                    PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), "editorChecksFailedPopup", "Failed editor checks!",
                        "Warning! This vessel did not pass the editor checks! It will still be added to the plans, but you will not be able to launch it without upgrading. Listed below are the failed checks:\n"
                        + string.Join("\n", facilityChecks.ToArray()), "Acknowledged", false, HighLogic.UISkin);
                }
            }
            string type = "";
            if (blv.type == KCT_BuildListVessel.ListType.VAB)
            {
                if (KCT_GameStates.ActiveKSC.VABPlans.ContainsKey(blv.shipName))
                {
                    KCT_GameStates.ActiveKSC.VABPlans.Remove(blv.shipName);
                    message = new ScreenMessage("[KCT] Replacing previous plan for " + blv.shipName +" in the VAB Building Plans list.", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                    ScreenMessages.PostScreenMessage(message);
                }
                KCT_GameStates.ActiveKSC.VABPlans.Add(blv.shipName, blv);
                type = "VAB";
            }
            else if (blv.type == KCT_BuildListVessel.ListType.SPH)
            {
                if (KCT_GameStates.ActiveKSC.SPHPlans.ContainsKey(blv.shipName))
                {
                    KCT_GameStates.ActiveKSC.SPHPlans.Remove(blv.shipName);
                    message = new ScreenMessage("[KCT] Replacing previous plan for " + blv.shipName + " in the SPH Building Plans list.", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                    ScreenMessages.PostScreenMessage(message);
                }
                    KCT_GameStates.ActiveKSC.SPHPlans.Add(blv.shipName, blv);
                type = "SPH";
            }

            ScrapYardWrapper.ProcessVessel(blv.ExtractedPartNodes);

            KCTDebug.Log("Added " + blv.shipName + " to " + type + " build list at KSC " + KCT_GameStates.ActiveKSC.KSCName + ". Cost: " + blv.cost);
            KCTDebug.Log("Launch site is " + blv.launchSite);
            //KCTDebug.Log("Cost Breakdown (total, parts, fuel): " + blv.totalCost + ", " + blv.dryCost + ", " + blv.fuelCost);
            message = new ScreenMessage("[KCT] Added " + blv.shipName + " to " + type + " build list.", 4.0f, ScreenMessageStyle.UPPER_CENTER);
            ScreenMessages.PostScreenMessage(message);
            return blv;
        }

        private static void RemoveVesselFromPlans()
        {
            InputLockManager.RemoveControlLock("KCTPopupLock");
            
            plansList.RemoveAt(planToDelete);
        }
    }
}
