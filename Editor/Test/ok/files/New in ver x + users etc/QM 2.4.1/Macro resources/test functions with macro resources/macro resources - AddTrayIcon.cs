 /exe

__Hicon hi=GetFileIcon("resource:output.ico")
out hi

AddTrayIcon ":5 q:\app\mouse.ico"; 2
AddTrayIcon "resource:<display images from macro resources - icon, cursor, jpg, png>test.ico"; 2
AddTrayIcon "resource:output.ico"; 2
Tray t.AddIcon("resource:output.ico[]mouse.ico[]resource:<display images from macro resources - icon, cursor, jpg, png>test.ico[]keyboard.ico"); for(_i 0 8) 0.5; t.Modify(_i+1&3+1)

 BEGIN PROJECT
 main_function  macro resources - AddTrayIcon
 exe_file  $my qm$\macro resources - AddTrayIcon.qmm
 flags  23
 guid  {FEE3898B-BC80-4B2C-AF96-A1180051FFA7}
 END PROJECT
