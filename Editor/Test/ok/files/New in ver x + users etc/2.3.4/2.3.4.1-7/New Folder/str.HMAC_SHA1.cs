 /
function str&data str'password [flags] ;;flags: 1 hex-encode

 Creates a hashed message authentication checksum (HMAC) using SHA1 algorithm.
 Stores the HMAC (160-bit length binary data) in this variable. Error if fails.

 data - variable containing message data.
 password - key.

 REMARKS
 A hashed message authentication checksum (HMAC) is typically used to verify that a message has not been changed during transit. Both parties to the message must have a shared secret key. The sender combines the key and the message into a string, creates a digest of the string by using an algorithm such as SHA-1 or MD5, and transmits the message and the digest. The receiver combines the shared key with the message, applies the appropriate algorithm, and compares the digest thus obtained with that transmitted by the sender. If the digests are exactly the same, the message was not tampered with.

 EXAMPLE
 str data="message"
 str password="password"
 str s
 s.HMAC_SHA1(data password 1)
 out s


type HMAC_INFO HashAlgid !*pbInnerString cbInnerString !*pbOuterString cbOuterString
def CALG_SHA1 0x00008004
dll advapi32 [CryptAcquireContextA]#CryptAcquireContext *phProv $szContainer $szProvider dwProvType dwFlags
def PROV_RSA_FULL 1
def CRYPT_VERIFYCONTEXT 0xF0000000
dll advapi32 #CryptCreateHash hProv Algid hKey dwFlags *phHash
dll advapi32 #CryptHashData hHash !*pbData dwDataLen dwFlags
type BLOBHEADER !bType !bVersion @reserved aiKeyAlg
def PLAINTEXTKEYBLOB 0x8
def CUR_BLOB_VERSION 2
def CALG_RC2 0x00006602
dll advapi32 #CryptImportKey hProv !*pbData dwDataLen hPubKey dwFlags *phKey
def CRYPT_IPSEC_HMAC_KEY 0x00000100
def CALG_HMAC 0x00008009
dll advapi32 #CryptSetHashParam hHash dwParam !*pbData dwFlags
def HP_HMAC_INFO 0x0005
dll advapi32 #CryptGetHashParam hHash dwParam !*pbData *pdwDataLen dwFlags
def HP_HASHVAL 0x0002
dll advapi32 #CryptDestroyHash hHash
dll advapi32 #CryptDestroyKey hKey
dll advapi32 #CryptReleaseContext hProv dwFlags


if(password.len>1024) end "password too long (max 1024)"

int hProv ;;HCRYPTPROV
int hHash ;;HCRYPTHASH
int hKey ;;HCRYPTKEY
int hHmacHash ;;HCRYPTHASH
int dwDataLen
HMAC_INFO HmacInfo
str es ;;error string

HmacInfo.HashAlgid = CALG_SHA1

if(!CryptAcquireContext(&hProv 0 0 PROV_RSA_FULL CRYPT_VERIFYCONTEXT)) es.format(" Error in AcquireContext 0x%08x \n", GetLastError()); goto ErrorExit
if(!CryptCreateHash(hProv CALG_SHA1 0 0 &hHash)) es.format("Error in CryptCreateHash 0x%08x \n", GetLastError()); goto ErrorExit
if(!CryptHashData(hHash password password.len 0)) es.format("Error in CryptHashData 0x%08x \n", GetLastError()); goto ErrorExit

type __KEYBLOB BLOBHEADER'hdr len key[1024]
__KEYBLOB key_blob
key_blob.hdr.bType = PLAINTEXTKEYBLOB
key_blob.hdr.bVersion = CUR_BLOB_VERSION
key_blob.hdr.aiKeyAlg = CALG_RC2
key_blob.len = password.len
memcpy &key_blob.key password password.len
if(!CryptImportKey(hProv &key_blob sizeof(key_blob) 0 CRYPT_IPSEC_HMAC_KEY &hKey)) es.format("Error in CryptImportKey 0x%08x \n", GetLastError()); goto ErrorExit

if(!CryptCreateHash(hProv CALG_HMAC hKey 0 &hHmacHash)) es.format("Error in CryptCreateHash 0x%08x \n", GetLastError()); goto ErrorExit
if(!CryptSetHashParam(hHmacHash HP_HMAC_INFO &HmacInfo 0)) es.format("Error in CryptSetHashParam 0x%08x \n", GetLastError()); goto ErrorExit
if(!CryptHashData(hHmacHash data data.len 0)) es.format("Error in CryptHashData 0x%08x \n", GetLastError()); goto ErrorExit

if(!CryptGetHashParam(hHmacHash HP_HASHVAL 0 &dwDataLen 0)) es.format("Error in CryptGetHashParam 0x%08x \n", GetLastError()); goto ErrorExit
this.all(dwDataLen 2)
if(!CryptGetHashParam(hHmacHash HP_HASHVAL this &dwDataLen 0)) es.format("Error in CryptGetHashParam 0x%08x \n", GetLastError()); goto ErrorExit

int ok=1

 ErrorExit
if(hHmacHash) CryptDestroyHash(hHmacHash)
if(hKey) CryptDestroyKey(hKey)
if(hHash) CryptDestroyHash(hHash)
if(hProv) CryptReleaseContext(hProv 0)

if(!ok) end "failed. %s" 0 es

if(flags&1) this.encrypt(8)
