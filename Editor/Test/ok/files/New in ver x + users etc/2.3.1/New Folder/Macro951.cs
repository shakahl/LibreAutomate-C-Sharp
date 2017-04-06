__Handle h=run("notepad")

PostQuitMessage 0

out WaitForMultipleObjects(1 &h 0 5000)
