function $sa

out _command
out sa
 run _command "/xxxxxxxxxxx"
run "qmmacro.exe" F"''{_command}'' {sa}"
 ret 1
