function$ !*buffer bufferSize [flags] ;;flags: 1 Hex (0 binary)

 Computes SHA256 checksum for input buffer.
 Returns self.
 Error if fails.

 buffer - input buffer. Can be a string or other memory.
 bufferSize - input buffer size. If <0, calculates length of buffer string.

 REMARKS
 From <link>http://nagareshwar.securityxploded.com/2010/10/22/cryptocode-generate-sha1sha256-hash-using-windows-cryptography-library/</link>.

 EXAMPLE
 str s
 s.HashSHA256("The quick brown fox jumps over the lazy dog" -1 1)
 out s


 ref WINAPI2
dll advapi32 [CryptAcquireContextA]#CryptAcquireContext *phProv $szContainer $szProvider dwProvType dwFlags
def CRYPT_VERIFYCONTEXT 0xF0000000
def PROV_RSA_AES 24
dll advapi32 #CryptCreateHash hProv Algid hKey dwFlags *phHash
dll advapi32 #CryptHashData hHash !*pbData dwDataLen dwFlags
dll advapi32 #CryptGetHashParam hHash dwParam !*pbData *pdwDataLen dwFlags
def HP_HASHSIZE 0x0004
def HP_HASHVAL 0x0002
dll advapi32 #CryptDestroyHash hHash
dll advapi32 #CryptReleaseContext hProv dwFlags
def CALG_SHA256 0x0000800C

int dwStatus bResult hProv hHash cbHashSize
if(bufferSize<0) lpstr _k=buffer; bufferSize=len(_k)

if(!CryptAcquireContext(&hProv, 0, 0, PROV_RSA_AES, CRYPT_VERIFYCONTEXT)) ret
if(!CryptCreateHash(hProv, CALG_SHA256, 0, 0, &hHash)) goto EndHash
if(!CryptHashData(hHash, buffer, bufferSize, 0)) goto EndHash
int dwCount = sizeof(int);
if(!CryptGetHashParam(hHash, HP_HASHSIZE, &cbHashSize, &dwCount, 0)) goto EndHash
this.all(cbHashSize 2)
if(!CryptGetHashParam(hHash, HP_HASHVAL, this, &cbHashSize, 0)) goto EndHash
bResult = 1;
 EndHash
if(hHash) CryptDestroyHash(hHash);
if(hProv) CryptReleaseContext(hProv, 0);
if(!bResult) end F"{ERR_FAILED}. {_s.dllerror}"

if(flags&1) this.encrypt(8)
