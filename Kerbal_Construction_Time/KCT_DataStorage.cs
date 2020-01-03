using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalConstructionTime
{
    public abstract class ConfigNodeStorage : IPersistenceLoad, IPersistenceSave
    {
        public ConfigNodeStorage() { }

        void IPersistenceLoad.PersistenceLoad()
        {
            OnDecodeFromConfigNode();
        }

        void IPersistenceSave.PersistenceSave()
        {
            OnEncodeToConfigNode();
        }

        public virtual void OnDecodeFromConfigNode() { }
        public virtual void OnEncodeToConfigNode() { }

        public ConfigNode AsConfigNode()
        {
            try
            {
                //Create a new Empty Node with the class name
                ConfigNode cnTemp = new ConfigNode(this.GetType().Name);
                //Load the current object in there
                cnTemp = ConfigNode.CreateConfigFromObject(this, cnTemp);
                return cnTemp;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                //Logging and return value?                    
                return new ConfigNode(this.GetType().Name);
            }
        }
    }

    public class FakePart : ConfigNodeStorage
    {
        [Persistent] public string part = "";
    }

    public class FakeTechNode : ConfigNodeStorage
    {
        [Persistent] public string id = "";
        [Persistent] public string state = "";

        public ProtoTechNode ToProtoTechNode()
        {
            ProtoTechNode ret = new ProtoTechNode();
            ret.techID = id;
            if (state == "Available")
                ret.state = RDTech.State.Available;
            else
                ret.state = RDTech.State.Unavailable;
            return ret;
        }

        public FakeTechNode FromProtoTechNode(ProtoTechNode node)
        {
            this.id = node.techID;
            this.state = node.state.ToString();
            return this;
        }
    }
    public class KCT_DataStorage : ConfigNodeStorage
    {
        [Persistent] bool enabledForSave = (HighLogic.CurrentGame.Mode == Game.Modes.CAREER || 
                                            HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX ||
                                            HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX);




        //[Persistent] bool firstStart = true;
        [Persistent] List<int> VABUpgrades = new List<int>() {0};
        [Persistent] List<int> SPHUpgrades = new List<int>() {0};
        [Persistent] List<int> RDUpgrades = new List<int>() {0,0};
        [Persistent] List<int> PurchasedUpgrades = new List<int>() {0,0};
        [Persistent] List<String> PartTracker = new List<String>();
        [Persistent] List<String> PartInventory = new List<String>();
        [Persistent] string activeKSC = "";
        [Persistent] float SalesFigures = 0, SciPoints = -1f;
        [Persistent] int UpgradesResetCounter = 0, TechUpgrades = 0, SavedUpgradePointsPreAPI = 0;


        public override void OnDecodeFromConfigNode()
        {

            KCT_GameStates.PurchasedUpgrades = PurchasedUpgrades;
            KCT_GameStates.activeKSCName = activeKSC;
            KCT_GameStates.UpgradesResetCounter = UpgradesResetCounter;
            KCT_GameStates.TechUpgradesTotal = TechUpgrades;
            KCT_GameStates.SciPointsTotal = SciPoints;
            KCT_GameStates.PermanentModAddedUpgradesButReallyWaitForTheAPI = SavedUpgradePointsPreAPI;

        }

        public override void OnEncodeToConfigNode()
        {
            TechUpgrades = KCT_GameStates.TechUpgradesTotal;
            PurchasedUpgrades = KCT_GameStates.PurchasedUpgrades;
            SciPoints = KCT_GameStates.SciPointsTotal;
            //firstStart = KCT_GameStates.firstStart;
            activeKSC = KCT_GameStates.ActiveKSC.KSCName;
            SalesFigures = KCT_GameStates.InventorySalesFigures;
            UpgradesResetCounter = KCT_GameStates.UpgradesResetCounter;
            SavedUpgradePointsPreAPI = KCT_GameStates.PermanentModAddedUpgradesButReallyWaitForTheAPI;
        }


        private bool VesselIsInWorld(Guid id)
        {
            for (int i = FlightGlobals.Vessels.Count - 1; i >= 0; i--)
            {
                Vessel vssl = FlightGlobals.Vessels[i];
            
            //foreach (Vessel vssl in FlightGlobals.Vessels)
            //{
                if (vssl.id == id)
                    return true;
            }
            return false;
        }
        public List<String> DictToList(Dictionary<String, int> dict)
        {
            List<String> list = new List<String>();
            foreach (string k in dict.Keys)
            {
                int val = dict[k];
                list.Add(k);
                list.Add(val.ToString());
            }
            return list;
        }
        public Dictionary<String, int> ListToDict(List<String> list)
        {
            Dictionary<String, int> dict = new Dictionary<String, int>();
            for (int i = 0; i < list.Count; i+=2 )
            {
                dict.Add(list[i], int.Parse(list[i + 1]));
            }
            return dict;
        }
    }
}
/*
Copyright (C) 2018  Michael Marvin, Zachary Eck

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
