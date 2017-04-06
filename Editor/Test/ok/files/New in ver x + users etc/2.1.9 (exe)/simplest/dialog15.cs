type HWVAR hwnd hwndedit ;;add more members if needed
HWVAR- v ;;also add this line in other functions of this thread where you want to use v
MainWindow "Hello World" "QM_HW_Class" &HW_WndProc 200 200 200 200

 BASIC CONCEPTS OF A WINDOWS APPLICATION

 Typical Windows application registers main window's class, creates
 main window and enters a message loop, which runs until the main
 window is closed. Function MainWindow performs all this.
 MainWindow arguments: window name, window class name, address
 of window procedure, left, top, width, height and optionally
 style, owner window handle, pointer to some data, and extended style.
 Other required function is window procedure. It is called
 whenever Windows, the application or other applications send or
 post a message to a window that belongs to that class. Window
 procedure processes some messages, and/or calls DefWindowProc,
 which does default processing.
