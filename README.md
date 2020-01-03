KCT
===

Kerbal Construction Time - An addon for Kerbal Space Program

####ABOUT

Kerbal Construction Time is an addon for Kerbal Space Program, a game developed by Squad, that makes rockets/planes/vessels take time to build before you can launch/fly them. The amount of time is based on the cost of all of the parts that make up the craft. This time is reduced when using parts that have been used before, or when using parts that have been recovered from other vessels (meaning there is an advantage to building reusable craft).

Check out the development forum for additional details and pre-built binaries: http://forum.kerbalspaceprogram.com/threads/92377-0-24-2-Kerbal-Construction-Time-Release-v1-0-2-%289-3-14%29

Please note:  Thre are 3 different projects here, only one should be enabled at any time:

KerbalConstructionTime-1.7.3		Builds for KSP 1.7.3
KerbalConstructinTime-1.8			Builds for KSP 1.8 and newer
KerbalConstructionTime-RO			Used by the RO people

####DEPENDENCIES FOR BUILDING
* Assembly-CSharp.dll (from KSP)
* UnityEngine.dll (from KSP)

I use Visual Studio Express 2017.



Expand Building Plans dialog.
Add menu items
		1. add to integration building list
		2. Load prebuilt assemblies
			(Load 1 from integration list, all others list listed in subassemblies)
			(all assemblies aftger first are loaded as a subassembly)
			(During integration, no parts are available)
			(Need to trigger subassembly view)
		Have whitelist of parts to be used during integration
			struts
			fuel lines
		Build times for integrated vessel will be 1/10 of the total time to build the assemblies, plus the time to build whatever new parts were added
		Check total number of parts (ignoring the whitelisted parts) to make sure no extra parts have been added (ie:  duplications, etc)



