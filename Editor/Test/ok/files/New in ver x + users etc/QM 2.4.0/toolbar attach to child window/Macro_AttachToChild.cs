 Open QM Options dialog and run this macro.
 Then select other tab. It hides current tab, and then ToolbarHook_AttachToChild hides the toolbar.

int w=win("Options" "#32770")
int c=child("" "#32770" w) ;;property page
mac("Toolbar_AttachToChild" c)
