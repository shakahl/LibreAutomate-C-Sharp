	 MIDI Triggers

 MIDI triggers is QM extension application that launches
 commands or macros when MIDI keyboard keys are pressed.
 
 To start, run MT_Main function. It creates hidden window (configuration
 dialog) and tray icon. MT_Main runs continously and exits automatically.
 To run automatically at startup, in function init2 add:

mac "MT_Main"

 Tray icon:
 Click to show configuration dialog.
 Ctrl+click to exit.
 Right click to enable/disable MIDI input monitoring.
 
 HOW TO ASSIGN TRIGGERS TO MACROS AND COMMANDS
 
 Create an usual QM popup menu and type its name in MIDI triggers dialog.
 Menu item syntax:

 notenumber :commands
 
 notenumber is number from 0 to 127. For example, 36 for C2, 38 for D2, etc.
 commands can be any commands. Look at the sample menu MT_Menu (it is the default menu).
