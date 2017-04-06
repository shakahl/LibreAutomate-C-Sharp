#ret
def ATTRIBUTE_PACKED
type BE_CONFIG dwConfig __BE_CONFIG1'format
def BE_CONFIG_LAME 256
def BE_CONFIG_MP3 0
type BE_ERR = #
def BE_ERR_BUFFER_TOO_SMALL 0x00000005
def BE_ERR_INVALID_FORMAT 0x00000001
def BE_ERR_INVALID_FORMAT_PARAMETERS 0x00000002
def BE_ERR_INVALID_HANDLE 0x00000004
def BE_ERR_NO_MORE_HANDLES 0x00000003
def BE_ERR_SUCCESSFUL 0x00000000
def BE_MAX_HOMEPAGE 128
def BE_MP3_MODE_DUALCHANNEL 2
def BE_MP3_MODE_JSTEREO 1
def BE_MP3_MODE_MONO 3
def BE_MP3_MODE_STEREO 0
type BE_VERSION !byDLLMajorVersion !byDLLMinorVersion !byMajorVersion !byMinorVersion !byDay !byMonth @wYear !zHomepage[129] !byAlphaLevel !byBetaLevel !byMMXEnabled !btReserved[125]
def CURRENT_STRUCT_SIZE sizeof(BE_CONFIG)
def CURRENT_STRUCT_VERSION 1
type HBE_STREAM = #
type LAME_QUALITY_PRESET = #
def LQP_ABR 11
def LQP_AM 3000
def LQP_CBR 12
def LQP_CD 9000
def LQP_EXTREME 8
def LQP_FAST_EXTREME 9
def LQP_FAST_MEDIUM 14
def LQP_FAST_STANDARD 7
def LQP_FM 4000
def LQP_HIFI 8000
def LQP_HIGH_QUALITY 2
def LQP_INSANE 10
def LQP_LOW_QUALITY 1
def LQP_MEDIUM 13
def LQP_NOPRESET 0xFFFFFFFF
def LQP_NORMAL_QUALITY 0
def LQP_PHONE 1000
def LQP_R3MIX 4
def LQP_RADIO 6000
def LQP_STANDARD 6
def LQP_STUDIO 10000
def LQP_SW 2000
def LQP_TAPE 7000
def LQP_VERYHIGH_QUALITY 5
def LQP_VOICE 5000
def LQP_VOICE_QUALITY 3
def MPEG1 1
def MPEG2 0
type PBE_CONFIG = BE_CONFIG*
type PBE_VERSION = BE_VERSION*
type PHBE_STREAM = #*
type VBRMETHOD = #
def VBR_METHOD_ABR 4
def VBR_METHOD_DEFAULT 0
def VBR_METHOD_MTRH 3
def VBR_METHOD_NEW 2
def VBR_METHOD_NONE 0xFFFFFFFF
def VBR_METHOD_OLD 1
def _BLADEDLL
type __BE_CONFIG1 ____BE_CONFIG11'mp3 []____BE_CONFIG12'LHV1 []____BE_CONFIG13'aac
def ___BLADEDLL_H_INCLUDED___
type ____BE_CONFIG11 dwSampleRate !byMode [+1]@wBitrate bPrivate bCRC bCopyright bOriginal
type ____BE_CONFIG12 dwStructVersion dwStructSize dwSampleRate dwReSampleRate nMode dwBitrate dwMaxBitrate nPreset dwMpegVersion dwPsyModel dwEmphasis bPrivate bCRC bCopyright bOriginal bWriteVBRHeader bEnableVBR nVBRQuality dwVbrAbr_bps nVbrMethod bNoRes bStrictIso @nQuality !btReserved[237]
type ____BE_CONFIG13 dwSampleRate !byMode [+1]@wBitrate !byEncodingMethod
dll lame_enc #beCloseStream hbeStream
dll lame_enc #beDeinitStream hbeStream !*pOutput *pdwOutput
dll lame_enc #beEncodeChunk hbeStream nSamples @*pSamples !*pOutput *pdwOutput
dll lame_enc #beEncodeChunkFloatS16NI hbeStream nSamples FLOAT*buffer_l FLOAT*buffer_r !*pOutput *pdwOutput
dll lame_enc #beFlushNoGap hbeStream !*pOutput *pdwOutput
dll lame_enc #beInitStream BE_CONFIG*pbeConfig *dwSamples *dwBufferSize *phbeStream
dll lame_enc beVersion BE_VERSION*pbeVersion
dll lame_enc #beWriteInfoTag hbeStream $lpszFileName
dll lame_enc #beWriteVBRHeader $lpszFileName
