using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
//using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel; //Win32Exception

//using System.Reflection;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;

namespace SdkConverter
{
	unsafe partial class Converter
	{

		void Test()
		{
			var x = new IntPtr(3);
			Out(x);
			//Out(x << (IntPtr)1);
		}
	}
}


class TestStructEtc
{

	//using Enum7 = tagEnum7;
	//using Struct7 = tagStruct7;
	//using PStruct7 = tagStruct7*; //error

	//public enum tagEnum7
	//{
	//	A7 = 1,
	//	B7 = 2
	//}
	//unsafe struct Enum7 { tagEnum7 p; }

	//public struct tagStruct7
	//{
	//	int x;
	//}
	//unsafe struct PStruct7 { tagStruct7* p; }

	//void TestEnum(Enum7 e, PStruct7 t)
	//{
	//}

}









public unsafe struct IntLong
{
	public void* x;
}
[DebuggerStepThrough]
[StructLayout(LayoutKind.Explicit)]
public struct PSAPI_WORKING_SET_BLOCK
{
	[FieldOffset(0)]
	public IntLong Flags;
	[DebuggerStepThrough]
	public struct TYPE_1
	{
		private long __bf_1;
		public long Protection { get { return (__bf_1 >> 0 & 0x1F); } set { __bf_1 = ((__bf_1 & ~0x1F) | ((value & 0x1F) << 0)); } }
		public long ShareCount { get { return (__bf_1 >> 5 & 0x7); } set { __bf_1 = ((__bf_1 & ~0xE0) | ((value & 0x7) << 5)); } }
		public long Shared { get { return (__bf_1 >> 8 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x100) | ((value & 0x1) << 8)); } }
		public long Reserved { get { return (__bf_1 >> 9 & 0x7); } set { __bf_1 = ((__bf_1 & ~0xE00) | ((value & 0x7) << 9)); } }
		public long VirtualPage { get { return (__bf_1 >> 12 & 0xFFFFFFFFFFFFF); } set { __bf_1 = ((__bf_1 & 0xFFF) | ((value & 0xFFFFFFFFFFFFF) << 12)); } }
	}
	[FieldOffset(0)]
	public TYPE_1 _2;
	[FieldOffset(0)]
	private long __bf_1;
	public long i1 { get { return (__bf_1 >> 0 & 0x7); } set { __bf_1 = ((__bf_1 & ~0x7) | ((value & 0x7) << 0)); } }
	public long i2 { get { return (__bf_1 >> 3 & 0x1F); } set { __bf_1 = ((__bf_1 & ~0xF8) | ((value & 0x1F) << 3)); } }
	public long i3 { get { return (__bf_1 >> 8 & 0xFFFFFFFFFF); } set { __bf_1 = ((__bf_1 & ~0xFFFFFFFFFF00) | ((value & 0xFFFFFFFFFF) << 8)); } }
}
[DebuggerStepThrough]
public struct IMAGE_ARCHITECTURE_HEADER
{
	private uint __bf_1;
	public uint AmaskValue { get { return (__bf_1 >> 0 & 0x1U); } set { __bf_1 = ((__bf_1 & ~0x1U) | ((value & 0x1U) << 0)); } }
	public uint AmaskShift { get { return (__bf_1 >> 8 & 0xFFU); } set { __bf_1 = ((__bf_1 & ~0xFF00U) | ((value & 0xFFU) << 8)); } }
	public int FirstEntryRVA;
}
public struct BITF
{
	[StructLayout(LayoutKind.Explicit)]
	public struct TYPE_2
	{
		[FieldOffset(0)]
		public int clear;
		[DebuggerStepThrough]
		public struct TYPE_1
		{
			private sbyte __bf_1;
			public sbyte a { get { return (sbyte)(__bf_1 >> 0 & 0x7F); } set { __bf_1 = (sbyte)((__bf_1 & ~0x7F) | ((value & 0x7F) << 0)); } }
			private sbyte __bf_2;
			public sbyte b { get { return (sbyte)(__bf_2 >> 0 & 0x3); } set { __bf_2 = (sbyte)((__bf_2 & ~0x3) | ((value & 0x3) << 0)); } }
		}
		[FieldOffset(0)]
		public TYPE_1 _2;
	}
	public TYPE_2 _1;
}
[DebuggerStepThrough]
public struct SHELLFLAGSTATE
{
	private int __bf_1;
	public int fShowAllObjects { get { return (__bf_1 >> 0 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x1) | ((value & 0x1) << 0)); } }
	public int fShowExtensions { get { return (__bf_1 >> 1 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x2) | ((value & 0x1) << 1)); } }
	public int fNoConfirmRecycle { get { return (__bf_1 >> 2 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x4) | ((value & 0x1) << 2)); } }
	public int fShowSysFiles { get { return (__bf_1 >> 3 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x8) | ((value & 0x1) << 3)); } }
	public int fShowCompColor { get { return (__bf_1 >> 4 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x10) | ((value & 0x1) << 4)); } }
	public int fDoubleClickInWebView { get { return (__bf_1 >> 5 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x20) | ((value & 0x1) << 5)); } }
	public int fDesktopHTML { get { return (__bf_1 >> 6 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x40) | ((value & 0x1) << 6)); } }
	public int fWin95Classic { get { return (__bf_1 >> 7 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x80) | ((value & 0x1) << 7)); } }
	public int fDontPrettyPath { get { return (__bf_1 >> 8 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x100) | ((value & 0x1) << 8)); } }
	public int fShowAttribCol { get { return (__bf_1 >> 9 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x200) | ((value & 0x1) << 9)); } }
	public int fMapNetDrvBtn { get { return (__bf_1 >> 10 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x400) | ((value & 0x1) << 10)); } }
	public int fShowInfoTip { get { return (__bf_1 >> 11 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x800) | ((value & 0x1) << 11)); } }
	public int fHideIcons { get { return (__bf_1 >> 12 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x1000) | ((value & 0x1) << 12)); } }
	public int fAutoCheckSelect { get { return (__bf_1 >> 13 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x2000) | ((value & 0x1) << 13)); } }
	public int fIconsOnly { get { return (__bf_1 >> 14 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x4000) | ((value & 0x1) << 14)); } }
	public int fRestFlags { get { return (__bf_1 >> 15 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x8000) | ((value & 0x1) << 15)); } }
	private int __bf_2;
	public int more { get { return (__bf_2 >> 0 & 0x1FFFFFF); } set { __bf_2 = ((__bf_2 & ~0x1FFFFFF) | ((value & 0x1FFFFFF) << 0)); } }
}
[DebuggerStepThrough]
public struct COMSTAT
{
	private int __bf_1;
	public int fCtsHold { get { return (__bf_1 >> 0 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x1) | ((value & 0x1) << 0)); } }
	public int fDsrHold { get { return (__bf_1 >> 1 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x2) | ((value & 0x1) << 1)); } }
	public int fRlsdHold { get { return (__bf_1 >> 2 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x4) | ((value & 0x1) << 2)); } }
	public int fXoffHold { get { return (__bf_1 >> 3 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x8) | ((value & 0x1) << 3)); } }
	public int fXoffSent { get { return (__bf_1 >> 4 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x10) | ((value & 0x1) << 4)); } }
	public int fEof { get { return (__bf_1 >> 5 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x20) | ((value & 0x1) << 5)); } }
	public int fTxim { get { return (__bf_1 >> 6 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x40) | ((value & 0x1) << 6)); } }
	public int fReserved { get { return (__bf_1 >> 7 & 0x1FFFFFF); } set { __bf_1 = ((__bf_1 & 0x7F) | ((value & 0x1FFFFFF) << 7)); } }
	public int cbInQue;
	public int cbOutQue;
}
[StructLayout(LayoutKind.Explicit)]
public struct RATE_QUOTA_LIMIT
{
	[FieldOffset(0)]
	public int RateData;
	[DebuggerStepThrough]
	public struct TYPE_1
	{
		private int __bf_1;
		public int RatePercent { get { return (__bf_1 >> 0 & 0x7F); } set { __bf_1 = ((__bf_1 & ~0x7F) | ((value & 0x7F) << 0)); } }
		public int Reserved0 { get { return (__bf_1 >> 7 & 0x1FFFFFF); } set { __bf_1 = ((__bf_1 & 0x7F) | ((value & 0x1FFFFFF) << 7)); } }
	}
	[FieldOffset(0)]
	public TYPE_1 _2;
}
public struct LDT_ENTRY
{
	public short LimitLow;
	public short BaseLow;
	[StructLayout(LayoutKind.Explicit)]
	public struct TYPEOF_HighWord
	{
		public struct TYPEOF_Bytes
		{
			public sbyte BaseMid;
			public sbyte Flags1;
			public sbyte Flags2;
			public sbyte BaseHi;
		}
		[FieldOffset(0)]
		public TYPEOF_Bytes Bytes;
		[DebuggerStepThrough]
		public struct TYPEOF_Bits
		{
			private int __bf_1;
			public int BaseMid { get { return (__bf_1 >> 0 & 0xFF); } set { __bf_1 = ((__bf_1 & ~0xFF) | ((value & 0xFF) << 0)); } }
			public int Type { get { return (__bf_1 >> 8 & 0x1F); } set { __bf_1 = ((__bf_1 & ~0x1F00) | ((value & 0x1F) << 8)); } }
			public int Dpl { get { return (__bf_1 >> 13 & 0x3); } set { __bf_1 = ((__bf_1 & ~0x6000) | ((value & 0x3) << 13)); } }
			public int Pres { get { return (__bf_1 >> 15 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x8000) | ((value & 0x1) << 15)); } }
			public int LimitHi { get { return (__bf_1 >> 16 & 0xF); } set { __bf_1 = ((__bf_1 & ~0xF0000) | ((value & 0xF) << 16)); } }
			public int Sys { get { return (__bf_1 >> 20 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x100000) | ((value & 0x1) << 20)); } }
			public int Reserved_0 { get { return (__bf_1 >> 21 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x200000) | ((value & 0x1) << 21)); } }
			public int Default_Big { get { return (__bf_1 >> 22 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x400000) | ((value & 0x1) << 22)); } }
			public int Granularity { get { return (__bf_1 >> 23 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x800000) | ((value & 0x1) << 23)); } }
			public int BaseHi { get { return (__bf_1 >> 24 & 0xFF); } set { __bf_1 = ((__bf_1 & 0xFFFFFF) | ((value & 0xFF) << 24)); } }
		}
		[FieldOffset(0)]
		public TYPEOF_Bits Bits;
	}
	public TYPEOF_HighWord HighWord;
}
[DebuggerStepThrough]
public struct XSTATE_CONFIGURATION
{
	public long EnabledFeatures;
	public long EnabledVolatileFeatures;
	public int Size;
	private int __bf_1;
	public int OptimizedSave { get { return (__bf_1 >> 0 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x1) | ((value & 0x1) << 0)); } }
	public int CompactionEnabled { get { return (__bf_1 >> 1 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x2) | ((value & 0x1) << 1)); } }
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = (64))]
	public int[] Features;
	public long EnabledSupervisorFeatures;
	public long AlignedFeatures;
	public int AllFeatureSize;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = (64))]
	public int[] AllFeatures;
}
public struct PROCESSOR_IDLESTATE_POLICY
{
	public short Revision;
	[StructLayout(LayoutKind.Explicit)]
	public struct TYPEOF_Flags
	{
		[FieldOffset(0)]
		public short AsWORD;
		[DebuggerStepThrough]
		public struct TYPE_1
		{
			private short __bf_1;
			public short AllowScaling { get { return (short)(__bf_1 >> 0 & 0x1); } set { __bf_1 = (short)((__bf_1 & ~0x1) | ((value & 0x1) << 0)); } }
			public short Disabled { get { return (short)(__bf_1 >> 1 & 0x1); } set { __bf_1 = (short)((__bf_1 & ~0x2) | ((value & 0x1) << 1)); } }
			public short Reserved { get { return (short)(__bf_1 >> 2 & 0x3FFF); } set { __bf_1 = (short)((__bf_1 & ~0xFFFC) | ((value & 0x3FFF) << 2)); } }
		}
		[FieldOffset(0)]
		public TYPE_1 _2;
	}
	public TYPEOF_Flags Flags;
	public int PolicyCount;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3)]
	public int[] Policy;
}
[DebuggerStepThrough]
public struct PROCESSOR_POWER_POLICY_INFO
{
	public int TimeCheck;
	public int DemoteLimit;
	public int PromoteLimit;
	public sbyte DemotePercent;
	public sbyte PromotePercent;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	public sbyte[] Spare;
	private int __bf_1;
	public int AllowDemotion { get { return (__bf_1 >> 0 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x1) | ((value & 0x1) << 0)); } }
	public int AllowPromotion { get { return (__bf_1 >> 1 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x2) | ((value & 0x1) << 1)); } }
	public int Reserved { get { return (__bf_1 >> 2 & 0x3FFFFFFF); } set { __bf_1 = ((__bf_1 & 0x3) | ((value & 0x3FFFFFFF) << 2)); } }
}
public struct PROCESSOR_PERFSTATE_POLICY
{
	public int Revision;
	public sbyte MaxThrottle;
	public sbyte MinThrottle;
	public sbyte BusyAdjThreshold;
	[StructLayout(LayoutKind.Explicit)]
	public struct TYPE_2
	{
		[FieldOffset(0)]
		public sbyte Spare;
		[StructLayout(LayoutKind.Explicit)]
		public struct TYPEOF_Flags
		{
			[FieldOffset(0)]
			public sbyte AsBYTE;
			[DebuggerStepThrough]
			public struct TYPE_1
			{
				private sbyte __bf_1;
				public sbyte NoDomainAccounting { get { return (sbyte)(__bf_1 >> 0 & 0x1); } set { __bf_1 = (sbyte)((__bf_1 & ~0x1) | ((value & 0x1) << 0)); } }
				public sbyte IncreasePolicy { get { return (sbyte)(__bf_1 >> 1 & 0x3); } set { __bf_1 = (sbyte)((__bf_1 & ~0x6) | ((value & 0x3) << 1)); } }
				public sbyte DecreasePolicy { get { return (sbyte)(__bf_1 >> 3 & 0x3); } set { __bf_1 = (sbyte)((__bf_1 & ~0x18) | ((value & 0x3) << 3)); } }
				public sbyte Reserved { get { return (sbyte)(__bf_1 >> 5 & 0x7); } set { __bf_1 = (sbyte)((__bf_1 & ~0xE0) | ((value & 0x7) << 5)); } }
			}
			[FieldOffset(0)]
			public TYPE_1 _2;
		}
		[FieldOffset(0)]
		public TYPEOF_Flags Flags;
	}
	public TYPE_2 _5;
	public int TimeCheck;
	public int IncreaseTime;
	public int DecreaseTime;
	public int IncreasePercent;
	public int DecreasePercent;
}
public struct IMAGE_TLS_DIRECTORY32
{
	public int StartAddressOfRawData;
	public int EndAddressOfRawData;
	public int AddressOfIndex;
	public int AddressOfCallBacks;
	public int SizeOfZeroFill;
	[StructLayout(LayoutKind.Explicit)]
	public struct TYPE_2
	{
		[FieldOffset(0)]
		public int Characteristics;
		[DebuggerStepThrough]
		public struct TYPE_1
		{
			private int __bf_1;
			public int Reserved0 { get { return (__bf_1 >> 0 & 0xFFFFF); } set { __bf_1 = ((__bf_1 & ~0xFFFFF) | ((value & 0xFFFFF) << 0)); } }
			public int Alignment { get { return (__bf_1 >> 20 & 0xF); } set { __bf_1 = ((__bf_1 & ~0xF00000) | ((value & 0xF) << 20)); } }
			public int Reserved1 { get { return (__bf_1 >> 24 & 0xFF); } set { __bf_1 = ((__bf_1 & 0xFFFFFF) | ((value & 0xFF) << 24)); } }
		}
		[FieldOffset(0)]
		public TYPE_1 _2;
	}
	public TYPE_2 _6;
}
public struct IMAGE_RESOURCE_DIRECTORY_ENTRY
{
	[StructLayout(LayoutKind.Explicit)]
	public struct TYPE_2
	{
		[DebuggerStepThrough]
		public struct TYPE_1
		{
			private int __bf_1;
			public int NameOffset { get { return (__bf_1 >> 0 & 0x7FFFFFFF); } set { __bf_1 = ((__bf_1 & ~0x7FFFFFFF) | ((value & 0x7FFFFFFF) << 0)); } }
			public int NameIsString { get { return (__bf_1 >> 31 & 0x1); } set { __bf_1 = ((__bf_1 & 0x7FFFFFFF) | ((value & 0x1) << 31)); } }
		}
		[FieldOffset(0)]
		public TYPE_1 _1;
		[FieldOffset(0)]
		public int Name;
		[FieldOffset(0)]
		public short Id;
	}
	public TYPE_2 _1;
	[StructLayout(LayoutKind.Explicit)]
	public struct TYPE_4
	{
		[FieldOffset(0)]
		public int OffsetToData;
		[DebuggerStepThrough]
		public struct TYPE_3
		{
			private int __bf_1;
			public int OffsetToDirectory { get { return (__bf_1 >> 0 & 0x7FFFFFFF); } set { __bf_1 = ((__bf_1 & ~0x7FFFFFFF) | ((value & 0x7FFFFFFF) << 0)); } }
			public int DataIsDirectory { get { return (__bf_1 >> 31 & 0x1); } set { __bf_1 = ((__bf_1 & 0x7FFFFFFF) | ((value & 0x1) << 31)); } }
		}
		[FieldOffset(0)]
		public TYPE_3 _2;
	}
	public TYPE_4 _2;
}
[DebuggerStepThrough]
public struct IMAGE_CE_RUNTIME_FUNCTION_ENTRY
{
	public int FuncStart;
	private int __bf_1;
	public int PrologLen { get { return (__bf_1 >> 0 & 0xFF); } set { __bf_1 = ((__bf_1 & ~0xFF) | ((value & 0xFF) << 0)); } }
	public int FuncLen { get { return (__bf_1 >> 8 & 0x3FFFFF); } set { __bf_1 = ((__bf_1 & ~0x3FFFFF00) | ((value & 0x3FFFFF) << 8)); } }
	public int ThirtyTwoBit { get { return (__bf_1 >> 30 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x40000000) | ((value & 0x1) << 30)); } }
	public int ExceptionFlag { get { return (__bf_1 >> 31 & 0x1); } set { __bf_1 = ((__bf_1 & 0x7FFFFFFF) | ((value & 0x1) << 31)); } }
}
[DebuggerStepThrough]
public struct FPO_DATA
{
	public int ulOffStart;
	public int cbProcSize;
	public int cdwLocals;
	public short cdwParams;
	private short __bf_1;
	public short cbProlog { get { return (short)(__bf_1 >> 0 & 0xFF); } set { __bf_1 = (short)((__bf_1 & ~0xFF) | ((value & 0xFF) << 0)); } }
	public short cbRegs { get { return (short)(__bf_1 >> 8 & 0x7); } set { __bf_1 = (short)((__bf_1 & ~0x700) | ((value & 0x7) << 8)); } }
	public short fHasSEH { get { return (short)(__bf_1 >> 11 & 0x1); } set { __bf_1 = (short)((__bf_1 & ~0x800) | ((value & 0x1) << 11)); } }
	public short fUseBP { get { return (short)(__bf_1 >> 12 & 0x1); } set { __bf_1 = (short)((__bf_1 & ~0x1000) | ((value & 0x1) << 12)); } }
	public short reserved { get { return (short)(__bf_1 >> 13 & 0x1); } set { __bf_1 = (short)((__bf_1 & ~0x2000) | ((value & 0x1) << 13)); } }
	public short cbFrame { get { return (short)(__bf_1 >> 14 & 0x3); } set { __bf_1 = (short)((__bf_1 & ~0xC000) | ((value & 0x3) << 14)); } }
}
[DebuggerStepThrough]
public struct IMPORT_OBJECT_HEADER
{
	public short Sig1;
	public short Sig2;
	public short Version;
	public short Machine;
	public int TimeDateStamp;
	public int SizeOfData;
	[StructLayout(LayoutKind.Explicit)]
	public struct TYPE_1
	{
		[FieldOffset(0)]
		public short Ordinal;
		[FieldOffset(0)]
		public short Hint;
	}
	public TYPE_1 _7;
	private short __bf_1;
	public short Type { get { return (short)(__bf_1 >> 0 & 0x3); } set { __bf_1 = (short)((__bf_1 & ~0x3) | ((value & 0x3) << 0)); } }
	public short NameType { get { return (short)(__bf_1 >> 2 & 0x7); } set { __bf_1 = (short)((__bf_1 & ~0x1C) | ((value & 0x7) << 2)); } }
	public short Reserved { get { return (short)(__bf_1 >> 5 & 0x7FF); } set { __bf_1 = (short)((__bf_1 & ~0xFFE0) | ((value & 0x7FF) << 5)); } }
}
[StructLayout(LayoutKind.Explicit)]
public struct SLIST_HEADER
{
	public struct TYPE_1
	{
		public long Alignment;
		public long Region;
	}
	[FieldOffset(0)]
	public TYPE_1 _1;
	[DebuggerStepThrough]
	public struct TYPEOF_HeaderX64
	{
		private long __bf_1;
		public long Depth { get { return (__bf_1 >> 0 & 0xFFFF); } set { __bf_1 = ((__bf_1 & ~0xFFFF) | ((value & 0xFFFF) << 0)); } }
		public long Sequence { get { return (__bf_1 >> 16 & 0xFFFFFFFFFFFF); } set { __bf_1 = ((__bf_1 & 0xFFFF) | ((value & 0xFFFFFFFFFFFF) << 16)); } }
		private long __bf_2;
		public long Reserved { get { return (__bf_2 >> 0 & 0xF); } set { __bf_2 = ((__bf_2 & ~0xF) | ((value & 0xF) << 0)); } }
		public long NextEntry { get { return (__bf_2 >> 4 & 0xFFFFFFFFFFFFFFF); } set { __bf_2 = ((__bf_2 & 0xF) | ((value & 0xFFFFFFFFFFFFFFF) << 4)); } }
	}
	[FieldOffset(0)]
	public TYPEOF_HeaderX64 HeaderX64;
}
[DebuggerStepThrough]
public struct SHELLSTATEA
{
	private int __bf_1;
	public int fShowAllObjects { get { return (__bf_1 >> 0 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x1) | ((value & 0x1) << 0)); } }
	public int fShowExtensions { get { return (__bf_1 >> 1 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x2) | ((value & 0x1) << 1)); } }
	public int fNoConfirmRecycle { get { return (__bf_1 >> 2 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x4) | ((value & 0x1) << 2)); } }
	public int fShowSysFiles { get { return (__bf_1 >> 3 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x8) | ((value & 0x1) << 3)); } }
	public int fShowCompColor { get { return (__bf_1 >> 4 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x10) | ((value & 0x1) << 4)); } }
	public int fDoubleClickInWebView { get { return (__bf_1 >> 5 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x20) | ((value & 0x1) << 5)); } }
	public int fDesktopHTML { get { return (__bf_1 >> 6 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x40) | ((value & 0x1) << 6)); } }
	public int fWin95Classic { get { return (__bf_1 >> 7 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x80) | ((value & 0x1) << 7)); } }
	public int fDontPrettyPath { get { return (__bf_1 >> 8 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x100) | ((value & 0x1) << 8)); } }
	public int fShowAttribCol { get { return (__bf_1 >> 9 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x200) | ((value & 0x1) << 9)); } }
	public int fMapNetDrvBtn { get { return (__bf_1 >> 10 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x400) | ((value & 0x1) << 10)); } }
	public int fShowInfoTip { get { return (__bf_1 >> 11 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x800) | ((value & 0x1) << 11)); } }
	public int fHideIcons { get { return (__bf_1 >> 12 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x1000) | ((value & 0x1) << 12)); } }
	public int fWebView { get { return (__bf_1 >> 13 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x2000) | ((value & 0x1) << 13)); } }
	public int fFilter { get { return (__bf_1 >> 14 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x4000) | ((value & 0x1) << 14)); } }
	public int fShowSuperHidden { get { return (__bf_1 >> 15 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x8000) | ((value & 0x1) << 15)); } }
	public int fNoNetCrawling { get { return (__bf_1 >> 16 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x10000) | ((value & 0x1) << 16)); } }
	public int dwWin95Unused;
	public int uWin95Unused;
	public int lParamSort;
	public int iSortDirection;
	public int version;
	public int uNotUsed;
	private int __bf_2;
	public int fSepProcess { get { return (__bf_2 >> 0 & 0x1); } set { __bf_2 = ((__bf_2 & ~0x1) | ((value & 0x1) << 0)); } }
	public int fStartPanelOn { get { return (__bf_2 >> 1 & 0x1); } set { __bf_2 = ((__bf_2 & ~0x2) | ((value & 0x1) << 1)); } }
	public int fShowStartPage { get { return (__bf_2 >> 2 & 0x1); } set { __bf_2 = ((__bf_2 & ~0x4) | ((value & 0x1) << 2)); } }
	public int fAutoCheckSelect { get { return (__bf_2 >> 3 & 0x1); } set { __bf_2 = ((__bf_2 & ~0x8) | ((value & 0x1) << 3)); } }
	public int fIconsOnly { get { return (__bf_2 >> 4 & 0x1); } set { __bf_2 = ((__bf_2 & ~0x10) | ((value & 0x1) << 4)); } }
	public int fShowTypeOverlay { get { return (__bf_2 >> 5 & 0x1); } set { __bf_2 = ((__bf_2 & ~0x20) | ((value & 0x1) << 5)); } }
	public int fShowStatusBar { get { return (__bf_2 >> 6 & 0x1); } set { __bf_2 = ((__bf_2 & ~0x40) | ((value & 0x1) << 6)); } }
	public int fSpareFlags { get { return (__bf_2 >> 7 & 0x1FF); } set { __bf_2 = ((__bf_2 & ~0xFF80) | ((value & 0x1FF) << 7)); } }
}
[DebuggerStepThrough]
public struct PDH_BROWSE_DLG_CONFIG_HW
{
	private int __bf_1;
	public int bIncludeInstanceIndex { get { return (__bf_1 >> 0 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x1) | ((value & 0x1) << 0)); } }
	public int bSingleCounterPerAdd { get { return (__bf_1 >> 1 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x2) | ((value & 0x1) << 1)); } }
	public int bSingleCounterPerDialog { get { return (__bf_1 >> 2 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x4) | ((value & 0x1) << 2)); } }
	public int bLocalCountersOnly { get { return (__bf_1 >> 3 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x8) | ((value & 0x1) << 3)); } }
	public int bWildCardInstances { get { return (__bf_1 >> 4 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x10) | ((value & 0x1) << 4)); } }
	public int bHideDetailBox { get { return (__bf_1 >> 5 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x20) | ((value & 0x1) << 5)); } }
	public int bInitializePath { get { return (__bf_1 >> 6 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x40) | ((value & 0x1) << 6)); } }
	public int bDisableMachineSelection { get { return (__bf_1 >> 7 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x80) | ((value & 0x1) << 7)); } }
	public int bIncludeCostlyObjects { get { return (__bf_1 >> 8 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x100) | ((value & 0x1) << 8)); } }
	public int bShowObjectBrowser { get { return (__bf_1 >> 9 & 0x1); } set { __bf_1 = ((__bf_1 & ~0x200) | ((value & 0x1) << 9)); } }
	public int bReserved { get { return (__bf_1 >> 10 & 0x3FFFFF); } set { __bf_1 = ((__bf_1 & 0x3FF) | ((value & 0x3FFFFF) << 10)); } }
	public int dwDefaultDetailLevel;
}
public struct PERSISTENT_RESERVE_COMMAND
{
	public int Version;
	public int Size;
	[StructLayout(LayoutKind.Explicit)]
	public struct TYPE_1
	{
		[DebuggerStepThrough]
		public struct TYPEOF_PR_IN
		{
			private sbyte __bf_1;
			public sbyte ServiceAction { get { return (sbyte)(__bf_1 >> 0 & 0x1F); } set { __bf_1 = (sbyte)((__bf_1 & ~0x1F) | ((value & 0x1F) << 0)); } }
			public sbyte Reserved1 { get { return (sbyte)(__bf_1 >> 5 & 0x7); } set { __bf_1 = (sbyte)((__bf_1 & ~0xE0) | ((value & 0x7) << 5)); } }
			public short AllocationLength;
		}
		[FieldOffset(0)]
		public TYPEOF_PR_IN PR_IN;
		[DebuggerStepThrough]
		public struct TYPEOF_PR_OUT
		{
			private sbyte __bf_1;
			public sbyte ServiceAction { get { return (sbyte)(__bf_1 >> 0 & 0x1F); } set { __bf_1 = (sbyte)((__bf_1 & ~0x1F) | ((value & 0x1F) << 0)); } }
			public sbyte Reserved1 { get { return (sbyte)(__bf_1 >> 5 & 0x7); } set { __bf_1 = (sbyte)((__bf_1 & ~0xE0) | ((value & 0x7) << 5)); } }
			private sbyte __bf_2;
			public sbyte Type { get { return (sbyte)(__bf_2 >> 0 & 0xF); } set { __bf_2 = (sbyte)((__bf_2 & ~0xF) | ((value & 0xF) << 0)); } }
			public sbyte Scope { get { return (sbyte)(__bf_2 >> 4 & 0xF); } set { __bf_2 = (sbyte)((__bf_2 & ~0xF0) | ((value & 0xF) << 4)); } }
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0)]
			public sbyte[] ParameterList;
		}
		[FieldOffset(0)]
		public TYPEOF_PR_OUT PR_OUT;
	}
	public TYPE_1 _3;
}

