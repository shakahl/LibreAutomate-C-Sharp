 /
function ^waitmax rgbcolor x y [hwnd]

 Obsolete. Use <help>wait</help>.


if(hwnd) wait waitmax C rgbcolor x y hwnd
else wait waitmax C rgbcolor x y
err+ end _error
