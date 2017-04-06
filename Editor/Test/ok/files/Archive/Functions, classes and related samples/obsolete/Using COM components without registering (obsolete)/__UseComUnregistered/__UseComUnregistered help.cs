 Allows using COM components without registering.
 At first call __UseComUnregistered_CreateManifest to create manifest file. Do it once. Also after component version changes.
 Then call Activate each time before using the component.
 Does not work on Windows 2000 and XP SP0. Not tested on XP SP1.

 When creating manifest:
   The component file (usually dll or ocx) must be in qm folder or its subfolder.
   The manifest file will be created in qm folder.

 When using the component:
   The component file must be in the same folder relative to qm as when creating manifest.
   The manifest file must be in qm folder.

 A manifest file created on 1 computer can be usen on other computers.
 Can be used in exe too. Place the manifest in the same folder as the exe. Place the component in the same folder or subfolder.
 Can be used with COM dlls and ActiveX controls.

 A thread can use single active manifest at a time. Activating other manifest will deactivate previous manifest.
 If you use several such components in thread, create single manifest file for all: pass list of files to __UseComUnregistered_CreateManifest.

 EXAMPLES

 create manifest

__UseComUnregistered_CreateManifest "ComDll.dll" ;;or "ComDllFolder\ComDll.dll"
 __UseComUnregistered_CreateManifest "file1.dll[]file2.dll[]file3.dll" ;;use this if need to use several components in thread

 ________________________

 use component

 add this code somewhere at the beginning
 #if EXE ;;enable these #if/#endif if need only in exe
#compile "____UseComUnregistered"
__UseComUnregistered x.Activate("ComDll.X.manifest")
 #endif
