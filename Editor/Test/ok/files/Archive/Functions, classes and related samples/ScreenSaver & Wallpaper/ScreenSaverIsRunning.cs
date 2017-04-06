 Returns 1 if screen saver is running, 0 if not.
 Works well only if screen saver started naturally. If started by running a scr file, returns 0 even if running.

SystemParametersInfo(SPI_GETSCREENSAVERRUNNING 0 &_i 0)
ret _i
