 Several classes that make easier to use GDI+.
 http://www.quickmacros.com/forum/viewtopic.php?f=2&t=4321

 __________________________________________________


 ABOUT GDI+

 GDI+ is a Windows graphics library.
 Available on Windows XP and later. For Windows 2000 need to add gdiplus.dll to the system folder. Download from Microsoft or take from XP or 7.

 GDI+ is documented in MSDN Library.
 The documentation is for GDI+ C++ classes. We cannot use them in QM. But we can use GDI+ flat API. The C++ classes internally use the flat API too, and are open-source.
 The flat API are not documented, but you can understand almost everything from the documentation of the C++ classes. What is possible with C++ classes, also is possible with flat API.

 GDI+ can load files of these formats: BMP, GIF, JPEG, PNG, TIFF, ICON, WMF, EMF, EXIF.
 Can save in these formats: BMP, GIF, JPEG, PNG, TIFF.

 __________________________________________________


 ABOUT THE QM CLASSES

 Each class holds a pointer to a GDI+ native object of corresponding type (GpGraphics*, GpPen*, etc). See declarations in macro "__Gdip".
 The GDI+ native object is automatically deleted when the class variable dies or a creation function called again. Or you can call Delete.

 Most of the classes have only object creation functions. Some classes can load/save image files.
 To draw, etc, use GDI+ flat API. It is easy when using these classes. Look in "examples" folder.
 A class variable can be passed to GDI+ flat API functions where need pointer to GDI+ native object of corresponding type.

 To initialize variables of these classes, use the member functions.
 Or you can create GDI+ native object using GDI+ flat API and do: classVar.Delete; classVar=nativePointer.
 Copying of variables is not supported. Never do classVar=classVar. To copy bitmap: b2.FromBitmapArea(b1 0 0 b1.width b1.height).

 All the creation functions return pointer to the GDI+ native object. Don't delete the object. The variable will automatically delete it.
 Other functions return 1 or other value.
 If a function fails, it returns 0. There are no functions throwing errors.
 If a function fails, it assigns a GDI+ error code to _hresult (it's a special variable having thread scope, also used with COM functions). On success _hresult will be 0.
 GDI+ error codes are documented in MSDN Library, in "Status Enumeration" topic.
 _hresult will be -1 if gdiplus.dll in unavailable (Windows 2000).

 You don't have to initialize GDI+ when using these classes. The creation functions initialize it automatically. GDI+ version 1.0.
 If you use GDI+ flat API without these classes, call GdipInit before.

 The GDI+ flat API declarations are in reference file GDIP (gdiplus.txt). The file is installed with QM.
 It's scope is not global, so you have to always prepend "GDIP." to the GDI+ flat API functions, types and constants.
 The file contains everything from GDI+ C++ headers (Windows 7 SDK).
 The file contains even such classes as Bitmap, Graphics etc that are not useful because don't have member functions in QM.
 Some other classes, such as RectF, also don't have member functions in QM, but can be used with GDI+ flat API as simple types.

 Also there are several related global functions in Functions folder.

 This can be used in exe too. Just remember that Windows 2000 doesn't have gdiplus.dll. You can take it together with exe.
