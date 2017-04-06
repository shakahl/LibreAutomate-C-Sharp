 /Macro350
function# $lib str&sout

def IMAGE_ARCHIVE_START_SIZE 8
def IMAGE_ARCHIVE_START "!<arch>[10]"
def IMAGE_ARCHIVE_END "`[10]"
def IMAGE_ARCHIVE_PAD "[10]"
def IMAGE_ARCHIVE_LINKER_MEMBER "/               "
def IMAGE_ARCHIVE_LONGNAMES_MEMBER "//              "
def IMAGE_SIZEOF_ARCHIVE_MEMBER_HDR 60
type IMAGE_ARCHIVE_MEMBER_HEADER !Name[16] !Date[12] !UserID[6] !GroupID[6] !Mode[8] !Size[10] !EndHeader[2]

str s.getfile(lib)
if(!s.beg(IMAGE_ARCHIVE_START)) ret
IMAGE_ARCHIVE_MEMBER_HEADER* hd
hd=s+IMAGE_ARCHIVE_START_SIZE
lpstr name=&hd.Name
if(strncmp(name IMAGE_ARCHIVE_LINKER_MEMBER 16)) ret
int* p=hd+IMAGE_SIZEOF_ARCHIVE_MEMBER_HDR
int n=RevInt(*p)
if(n<1) ret
int* po=p+4
lpstr fn=po+(n*4)
out fn

ret n
