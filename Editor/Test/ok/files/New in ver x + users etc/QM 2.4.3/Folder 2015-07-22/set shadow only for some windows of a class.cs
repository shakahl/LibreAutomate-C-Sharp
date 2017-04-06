int w=win("QM TOOLBAR" "QM_toolbar")
_i=SetClassLong(w GCL_STYLE GetClassLong(w GCL_STYLE)|CS_DROPSHADOW)
if(!_i) out _s.dllerror; ret ;;first time fails, error 0, need to call 2 times. Maybe would work better if same thread.
outx _i
SetWindowPos w 0 0 0 0 0 SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE|SWP_NOZORDER|SWP_FRAMECHANGED

 Cannot add/remove shadow for an existing window.
 Create a hidden window just to get/set class flags.
 Before creating a real window, set class flag CS_DROPSHADOW with that hidden window.
 After creating, remove CS_DROPSHADOW.
