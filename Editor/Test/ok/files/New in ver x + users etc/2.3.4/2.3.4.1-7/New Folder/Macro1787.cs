 ont- _hwndqm

 Grab SDK
P registered start
S WM_PAINT 0 0 0
S WM_NCPAINT ?
S WM_ERASEBKGND
S WM_PRINTCLIENT PRF_CLIENT

 SS Studio
P registered
P WM_PAINT 0 0 0
S WM_NCPAINT 1 1 0
S WM_ERASEBKGND
P registered stop
S WM_PRINTCLIENT PRF_CLIENT
P registered stop
P registered stop
