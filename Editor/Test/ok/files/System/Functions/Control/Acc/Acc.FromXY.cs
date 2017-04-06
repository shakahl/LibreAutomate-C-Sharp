function `x `y [flags] ;;flags: 1 work area

 Gets object from point in screen, and initializes this variable.

 x, y - <help #IDP_PIXELS>coordinates</help>. Can be integer (pixels) or double (fraction, eg 0.5 is center).

 Added in: QM 2.3.3.


this=acc(x y 0 flags)

err+ end ERR_OBJECTGET
