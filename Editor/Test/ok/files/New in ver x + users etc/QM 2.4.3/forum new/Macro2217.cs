int w=win("Untitled - Notepad" "Notepad")
wait(30 -WC w)
err ;;handle error, it is probably timeout
	mes "the window still exists after 30 s"
