These are actually not bugs but rather limitations.


When something is autodeclared through ref, error when compiling same macro. Don't compile ref macros. Maybe now works anyway (after fixing another bug).

Typeinfo: For dll functions, in status bar is displayed [truename]. It could be removed for A functions, but not recommended because sometimes UNICODE version is without W.

Error if multiline string (comments) assigned to member string. Difficult to implement. Documented.

type library: "type is undefined" if type1 refers to type2 and type2 refers to type1.

In QM you cannot define types referring to each other. Should support forward declarations.

act may fail when all these conditions are true:
   QM is running without uiAccess. It is when it is running as User or explicitly started as admin (ie not through qmserv.exe).
   Active window is IE8, clicked or typed recently.

When installing QM on a virtual PC, msgbox "cannot copy Main.qml".
   It is because the setup program is on the host PC, and tries to copy Main.qml if exists in its folder, but it is locked because open in QM on host PC.
