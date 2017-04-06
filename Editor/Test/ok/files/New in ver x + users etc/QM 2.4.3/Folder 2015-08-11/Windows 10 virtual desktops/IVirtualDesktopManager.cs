interface# IVirtualDesktopManager :IUnknown
	#IsWindowOnCurrentVirtualDesktop(topLevelWindow)
	GUID'GetWindowDesktopId(topLevelWindow)
	MoveWindowToDesktop(topLevelWindow GUID&desktopId)
	{a5cd92ff-29be-454c-8d04-d82879fb3f1b}

interface IVirtualDesktopManager_Raw :IUnknown
	#IsWindowOnCurrentVirtualDesktop(topLevelWindow &is)
	#GetWindowDesktopId(topLevelWindow GUID&desktopId)
	#MoveWindowToDesktop(topLevelWindow GUID&desktopId)
	{a5cd92ff-29be-454c-8d04-d82879fb3f1b}

