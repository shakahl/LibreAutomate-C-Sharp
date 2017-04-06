function [iconindex] [$tooltip]

 Modifies tray icon that was added by AddIcon.

 iconindex - 1-based index of icon. If 0, does not change.
 tooltip - new tooltip text.

 EXAMPLE
 Tray t.AddIcon("mouse.ico[]keyboard.ico")
 1
 t.Modify(2)
 1


AddIcon(+iconindex tooltip)
