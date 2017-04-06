If you can run the dialog in separate process as exe file, you can use a custom manifest file, where you remove the dpiAware tag or set its value to false. Then Windows will DPI-scale the dialog automatically. It must be exe file, not qmm.

In default manifest, dpiAware is true. It is like in QM. If true, Windows only scales fonts, but not images. Size of dialogs and their controls depends on font size, therefore they also are scaled. That is why your dialogs become wider at 125%. Non-dialog windows are not scaled; only fonts in them will be bigger.

If dpiAware is false, Windows scales everything. I'm not sure that everything will work well, because QM is designed to be DPI-aware, and not tested with dpiAware false.
