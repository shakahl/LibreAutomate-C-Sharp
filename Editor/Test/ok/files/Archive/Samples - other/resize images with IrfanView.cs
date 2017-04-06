str f1 f2
f1.expandpath("$my pictures$\screensaver2\*.jpg")
f2.expandpath("$my pictures$\screensaver2\converted\*.jpg")

str cl.format("''%s'' /resize=(500,300) /convert=''%s''" f1 f2)
 out cl
run "$program files$\IrfanView\i_view32.exe" cl
