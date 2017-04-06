A QM item has a "debug mode" setting.
It can be Normal, Debug and Auto.
In Debug mode allows debugging, breakpoints, optional "Debug" window, assertions etc. The compiled script code contains a debug code. It is much slower.
In Normal mode the script is fast etc, and does not allow debugging.
In Auto mode, the script runs in debug mode the first 24 hours after its creation or last modification. Later runs in normal mode.
The Run button compiles/launches the script in current mode, which can be selected eg from a drop-down menu by the Run button.
The "Step into" command by default skips functions that have normal mode.
Also could have a third option - run in separate process.
