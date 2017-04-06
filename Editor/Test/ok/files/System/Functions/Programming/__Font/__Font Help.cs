 A variable of this class represents a font.
 Can be used in dialog procedures, to set font of controls.
 To set font for one or more controls: On WM_INITDIALOG, call Create or CreateFromString, then call SetDialogFont.

 A __Font variable that is used in a dialog procedure must exist while the dialog is open. Declare it as thread-local, not as local. Example: __Font-- font1.Create("Tahoma" 10).
 Use different __Font variables for different fonts.

 QM 2.3.2. Supports Unicode in font names.

 See also: <DT_SetTextColor>.
