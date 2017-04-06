SystemParametersInfo SPI_SETDESKWALLPAPER 0 _s.expandpath("$commonpictures$\Sample Pictures\Forest.jpg") 3 ;;change
5
SystemParametersInfo SPI_SETDESKWALLPAPER 0 "" 3 ;;remove
5
SystemParametersInfo SPI_SETDESKWALLPAPER 0 SETWALLPAPER_DEFAULT 3 ;;should restore default, but does not work on my computer
