 int w=win("Quick Macros - ok - [Macro2571]" "QM_Editor")
 int c=id(2216 w) ;;editable text
 int w=win("Keyboard Layout Creator 1.4 - 'Layout01 Description'" "WindowsForms10.Window.8.app.0.378734a")
 int c=child("" "*.BUTTON.*" w 0x0 "wfName=ckCapsLock") ;;check box 'Show the Caps Lock'
 int w=win("Profile Setup: app copy to notebook - Simple" "TfrmSetupMain!dx")
 int c=child("Source" "TButton!dx" w) ;;push button 'Source'
out _s.GetWinBaseClass(c)

 __MapIntStr m.AddList("0 ListBox[]2 Button[]3 Static[]4 Edit[]5 ComboBox[]10 ScrollBar[]11 StatusBar[]12 ToolBar[]13 Progress[]14 Animate[]15 Tab[]16 HotKey[]17 Header[]18 TrackBar[]19 ListView[]22 UpDown[]24 ToolTips[]25 TreeView[]28 RichEdit")
 int ct=SendMessage(c WM_GETOBJECT 0 OBJID_QUERYCLASSNAMEIDX)-65536
 ct=m.FindInt(ct)
 out ct
