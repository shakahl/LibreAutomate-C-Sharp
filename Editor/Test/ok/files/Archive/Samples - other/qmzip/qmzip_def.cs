#if 0

 zip

dll qmzip #CloseZip hz
dll qmzip #CreateZip !*z len flags
dll qmzip #FormatZipMessage code $buf len
type HZIP = #
def IMPORT
def XZIP_H
def ZIP_FILENAME 2
def ZIP_FOLDER 4
def ZIP_HANDLE 1
def ZIP_MEMORY 3
type ZRESULT = #
def ZR_ARGS 0x00010000
def ZR_BUGMASK 0xFF000000
def ZR_CALLERMASK 0x00FF0000
def ZR_CORRUPT 0x00000700
def ZR_ENDED 0x00050000
def ZR_FAILED 0x00040000
def ZR_FLATE 0x05000000
def ZR_GENMASK 0x0000FF00
def ZR_MEMSIZE 0x00030000
def ZR_MISSIZE 0x00060000
def ZR_MORE 0x00000600
def ZR_NOALLOC 0x00000300
def ZR_NOCHANGE 0x04000000
def ZR_NODUPH 0x00000100
def ZR_NOFILE 0x00000200
def ZR_NOTFOUND 0x00000500
def ZR_NOTINITED 0x01000000
def ZR_NOTMMAP 0x00020000
def ZR_OK 0x00000000
def ZR_PARTIALUNZ 0x00070000
def ZR_READ 0x00000800
def ZR_RECENT 0x00000001
def ZR_SEEK 0x02000000
def ZR_WRITE 0x00000400
def ZR_ZMODE 0x00080000
dll qmzip #ZipAdd hz $dstzn !*src len flags level

 unzip

dll qmzip #GetZipItem hz index ZIPENTRY*ze
type HZIP = #
def IMPORT
dll qmzip #OpenZip !*z len flags
dll qmzip #UnzipItem hz index !*dst len flags
def XUNZIP_H
type ZIPENTRY index !name[260] attr FILETIME'atime FILETIME'ctime FILETIME'mtime comp_size unc_size
def ZIP_FILENAME 2
def ZIP_HANDLE 1
def ZIP_MEMORY 3
type ZRESULT = #
def ZR_ARGS 0x00010000
def ZR_BUGMASK 0xFF000000
def ZR_CALLERMASK 0x00FF0000
def ZR_CORRUPT 0x00000700
def ZR_ENDED 0x00050000
def ZR_FAILED 0x00040000
def ZR_FLATE 0x05000000
def ZR_GENMASK 0x0000FF00
def ZR_MEMSIZE 0x00030000
def ZR_MISSIZE 0x00060000
def ZR_MORE 0x00000600
def ZR_NOALLOC 0x00000300
def ZR_NOCHANGE 0x04000000
def ZR_NODUPH 0x00000100
def ZR_NOFILE 0x00000200
def ZR_NOTFOUND 0x00000500
def ZR_NOTINITED 0x01000000
def ZR_NOTMMAP 0x00020000
def ZR_OK 0x00000000
def ZR_PARTIALUNZ 0x00070000
def ZR_READ 0x00000800
def ZR_RECENT 0x00000001
def ZR_SEEK 0x02000000
def ZR_WRITE 0x00000400
def ZR_ZMODE 0x00080000
