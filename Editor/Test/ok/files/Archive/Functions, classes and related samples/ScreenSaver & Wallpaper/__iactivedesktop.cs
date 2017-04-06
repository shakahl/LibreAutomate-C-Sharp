def ADDURL_SILENT 0X0001
def AD_APPLY_ALL 0x00000007
def AD_APPLY_BUFFERED_REFRESH 0x00000010
def AD_APPLY_DYNAMICREFRESH 0x00000020
def AD_APPLY_FORCE 0x00000008
def AD_APPLY_HTMLGEN 0x00000002
def AD_APPLY_REFRESH 0x00000004
def AD_APPLY_SAVE 0x00000001
def AD_GETWP_BMP 0x00000000
def AD_GETWP_IMAGE 0x00000001
def AD_GETWP_LAST_APPLIED 0x00000002
type COMPONENT dwSize dwID iComponentType fChecked fDirty fNoScroll COMPPOS'cpPos @wszFriendlyName[260] @wszSource[2132] dwCurItemState COMPSTATEINFO'csiOriginal COMPSTATEINFO'csiRestored
type COMPONENTSOPT dwSize fEnableComponents fActiveDesktop
def COMPONENT_DEFAULT_LEFT 0xFFFF
def COMPONENT_DEFAULT_TOP 0xFFFF
def COMPONENT_TOP 0x3fffffff
type COMPPOS dwSize iLeft iTop dwWidth dwHeight izIndex fCanResize fCanResizeX fCanResizeY iPreferredLeftPercent iPreferredTopPercent
type COMPSTATEINFO dwSize iLeft iTop dwWidth dwHeight dwItemState
def COMP_ELEM_ALL 0x00007FFF
def COMP_ELEM_CHECKED 0x00000002
def COMP_ELEM_CURITEMSTATE 0x00004000
def COMP_ELEM_DIRTY 0x00000004
def COMP_ELEM_FRIENDLYNAME 0x00000400
def COMP_ELEM_NOSCROLL 0x00000008
def COMP_ELEM_ORIGINAL_CSI 0x00001000
def COMP_ELEM_POS_LEFT 0x00000010
def COMP_ELEM_POS_TOP 0x00000020
def COMP_ELEM_POS_ZINDEX 0x00000100
def COMP_ELEM_RESTORED_CSI 0x00002000
def COMP_ELEM_SIZE_HEIGHT 0x00000080
def COMP_ELEM_SIZE_WIDTH 0x00000040
def COMP_ELEM_SOURCE 0x00000200
def COMP_ELEM_SUBSCRIBEDURL 0x00000800
def COMP_ELEM_TYPE 0x00000001
def COMP_TYPE_CFHTML 4
def COMP_TYPE_CONTROL 3
def COMP_TYPE_HTMLDOC 0
def COMP_TYPE_MAX 4
def COMP_TYPE_PICTURE 1
def COMP_TYPE_WEBSITE 2
def DTI_ADDUI_DEFAULT 0x00000000
def DTI_ADDUI_DISPSUBWIZARD 0x00000001
def DTI_ADDUI_POSITIONITEM 0x00000002
def GADOF_DIRTY 0x00000001
interface# IADesktopP2 :IUnknown
	ReReadWallpaper()
	GetADObjectFlags(*lpdwFlags dwMask)
	UpdateAllDesktopSubscriptions()
	MakeDynamicChanges(IOleObject'pOleObj)
	{B22754E2-4574-11d1-9888-006097DEACF9}
interface# IActiveDesktop :IUnknown
	ApplyChanges(dwFlags)
	GetWallpaper(@*pwszWallpaper cchWallpaper dwFlags)
	SetWallpaper(@*pwszWallpaper dwReserved)
	GetWallpaperOptions(WALLPAPEROPT*pwpo dwReserved)
	SetWallpaperOptions(WALLPAPEROPT*pwpo dwReserved)
	GetPattern(@*pwszPattern cchPattern dwReserved)
	SetPattern(@*pwszPattern dwReserved)
	GetDesktopItemOptions(COMPONENTSOPT*pco dwReserved)
	SetDesktopItemOptions(COMPONENTSOPT*pco dwReserved)
	AddDesktopItem(COMPONENT*pcomp dwReserved)
	AddDesktopItemWithUI(hwnd COMPONENT*pcomp dwReserved)
	ModifyDesktopItem(COMPONENT*pcomp dwFlags)
	RemoveDesktopItem(COMPONENT*pcomp dwReserved)
	GetDesktopItemCount(*lpiCount dwReserved)
	GetDesktopItem(nComponent COMPONENT*pcomp dwReserved)
	GetDesktopItemByID(dwID COMPONENT*pcomp dwReserved)
	GenerateDesktopItemHtml(@*pwszFileName COMPONENT*pcomp dwReserved)
	AddUrl(hwnd @*pszSource COMPONENT*pcomp dwFlags)
	GetDesktopItemBySource(@*pwszSource COMPONENT*pcomp dwReserved)
	{f490eb00-1240-11d1-9888-006097deacf9}
interface# IActiveDesktopP :IUnknown
	SetSafeMode(dwFlags)
	EnsureUpdateHTML()
	SetScheme(@*pwszSchemeName dwFlags)
	GetScheme(@*pwszSchemeName *lpdwcchBuffer dwFlags)
	{52502EE0-EC80-11D0-89AB-00C04FC2972D}
type IE4COMPONENT dwSize dwID iComponentType fChecked fDirty fNoScroll COMPPOS'cpPos @wszFriendlyName[260] @wszSource[2132]
def IS_FULLSCREEN 0x00000002
def IS_NORMAL 0x00000001
def IS_SPLIT 0x00000004
def IS_VALIDSIZESTATEBITS 0x00000007
def IS_VALIDSTATEBITS 0xC0000007
def SCHEME_CREATE 0x0080
def SCHEME_DISPLAY 0x0001
def SCHEME_DONOTUSE 0x0040
def SCHEME_EDIT 0x0002
def SCHEME_GLOBAL 0x0008
def SCHEME_LOCAL 0x0004
def SCHEME_REFRESH 0x0010
def SCHEME_UPDATE 0x0020
def SSM_CLEAR 0x0000
def SSM_REFRESH 0x0002
def SSM_SET 0x0001
def SSM_UPDATE 0x0004
type WALLPAPEROPT dwSize dwStyle
def WPSTYLE_CENTER 0
def WPSTYLE_CROPTOFIT 4
def WPSTYLE_KEEPASPECT 3
def WPSTYLE_MAX 5
def WPSTYLE_STRETCH 2
def WPSTYLE_TILE 1
