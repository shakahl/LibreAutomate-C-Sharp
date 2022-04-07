/// To create code to find and click an image or color in a window, use tool "Find image"; it's in the Code menu.

{
var w = wnd.find(1, "Character Map", "#32770");
var c = w.Child(1, id: 108); // CharGridWClass
string image = @"image:iVBORw0KGgoAAAANSUhEUgAAAAkAAAAKCAYAAABmBXS+AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABGSURBVChTlY7BDQAgCAM7bUdkvUpUIiZG8X4lVwAq8CuZCIgWkQLo0yQZISxjK3SpC7MVwirEprR6xJPkPM8NCo/fKEhSAyJHN5L2gSDoAAAAAElFTkSuQmCC";
var im = uiimage.find(1, c, image);
im.MouseClick();
}

/// The tool embeds the captured image into the script as a string (Base64). In the code editor the string is hidden.

/// Wait max 60 s for a color. Then print its coordinates.

{
var w = wnd.find(1, "Test");
var im = uiimage.find(60, w, 0xF2C90E);
print.it(im.Rect);
}

/// Example of "if exists". Also, the window can be in the background.

{
var w = wnd.find(1, "Character Map", "#32770");
string image = @"image:iVBORw0KGgoAAAANSUhEUgAAAAkAAAAKCAYAAABmBXS+AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABGSURBVChTlY7BDQAgCAM7bUdkvUpUIiZG8X4lVwAq8CuZCIgWkQLo0yQZISxjK3SpC7MVwirEprR6xJPkPM8NCo/fKEhSAyJHN5L2gSDoAAAAAElFTkSuQmCC";
var im = uiimage.find(w, image, IFFlags.WindowDC);
if (im != null) {
	print.it("exists");
} else {
	print.it("doesn't exist");
}
}
