 Gets object from point in screen, and initializes this variable.

 PARAMETERS
 x, y - <help "::/Other/IDP_PIXELS.html">coordinates</help>. Can be integer (pixels) or double (fraction, eg 0.5 is center).

 ERRORS
 ERR_OBJECTGET - failed to get object.

 REMARKS
 bbbbb
 ccccc

 ddd

 Version: QM 2.3.3.

 EXAMPLE
this=acc(x y 0 flags)
err+ end ERR_OBJECTGET
