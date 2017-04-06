function hwnd [objId]

 Gets Acc from window or control, and initializes this variable.

 hwnd - window/control handle.
 objId - <google "site:microsoft.com object identifiers OBJID_WINDOW">object identifier</google>. Default: OBJID_WINDOW (0).

 Added in: QM 2.3.3.


a=0; elem=0
if(!IsWindow(hwnd)) end ERR_HWND
if(objId) if(AccessibleObjectFromWindow(hwnd objId uuidof(IAccessible) &a)) end ERR_OBJECTGET
else this=acc(hwnd); err end ERR_OBJECTGET ;;supports Java
