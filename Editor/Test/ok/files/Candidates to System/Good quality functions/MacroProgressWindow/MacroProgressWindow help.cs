 Creates small window that displays macro name, time and user-defined text.
 Closes the window when macro ends.

 EXAMPLE

#compile "__MacroProgressWindow"
MacroProgressWindow x.Create(1 -1 -1 170 70)
x.SetText("macro[]info")
3
x.SetText("updated info" 0x0000ff)
3
