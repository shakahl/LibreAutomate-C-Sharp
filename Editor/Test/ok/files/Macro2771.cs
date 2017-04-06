int w=win("FindMe" "*.Window.*")
SendMessage w WM_APP 0 0
PostMessage w WM_APP 1 0
