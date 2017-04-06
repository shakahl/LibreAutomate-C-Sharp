 int w=win("Open" "#32770")
int w=win("Save As" "#32770")
str sFolder sFilename
GetPathInOpenSaveDialog w sFolder sFilename
out F"{sFolder}\{sFilename}"
