using UnityEngine;
using ToolbarControl_NS;

namespace KerbalConstructionTime
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(KCT_GameStates.MODID, KCT_GameStates.MODNAME);
        }
    }
}