using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Drawing;

using Au.Types;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace UIA
{
	/// <summary>
	/// UI Automation client COM interfaces and related types.
	/// </summary>
	/// <remarks>
	/// Documented in MSDN Library. The most important interfaces are <msdn>IUIAutomation</msdn> and <msdn>IUIAutomationElement</msdn> (here IElement).
	/// 
	/// To make programming easier, changed these declarations:
	/// 1. Interface name prefix "IUIAutomation" replaced with "I". For example, IUIAutomationElement here is IElement.
	/// 2. Removed prefix "Current" from property names. For example, CurrentName here is Name.
	/// 3. All constants moved to enums, and removed prefixes and suffixes. For example, UIA_NamePropertyId here is PropertyId.Name. Function parameter types also changed from int to enum.
	/// </remarks>
	internal class NamespaceDoc
	{
		//SHFB uses this for namespace documentation.
	}

	/// <summary>
	/// This class is used to create COM objects of <see cref="IUIAutomation"/> type, like <c>var u = new UIA.CUIAutomation() as UIA.IUIAutomation;</c>
	/// </summary>
	[ComImport, Guid("ff48dba4-60ef-4201-aa87-54103eef594e"), ClassInterface(ClassInterfaceType.None)]
	public class CUIAutomation { }

	/// <summary>
	/// This class is used to create COM objects of IUIAutomationX types, like <c>var u = new UIA.CUIAutomation8() as UIA.IUIAutomation2;</c>
	/// </summary>
	/// <remarks>
	/// Unavailable on Windows 7. Use <see cref="CUIAutomation"/>/<see cref="IUIAutomation"/> instead.
	/// </remarks>
	[ComImport, Guid("e22ad333-b25f-460c-83d0-0581107395c9"), ClassInterface(ClassInterfaceType.None)]
	public class CUIAutomation8 { }

	/// <summary>
	/// Provides UI Automation services - gets <see cref="IElement"/> of root/window/point/focus, creates find conditions and other objects, registers event handlers, etc.
	/// </summary>
	/// <remarks>
	/// Use this code to create COM object: <c>var u = new UIA.CUIAutomation() as UIA.IUIAutomation;</c>
	/// There are several other IUIAutomationX interfaces, added in Windows 8 and later. To create them use <see cref="CUIAutomation8"/> class.
	/// </remarks>
	[Guid("30CBE57D-D9D0-452A-AB13-7AC5AC4825EE")]
	[ComConversionLoss]
	[InterfaceType(1)]
	[ComImport]
	public interface IUIAutomation
	{
		[return: MarshalAs(UnmanagedType.Bool)] bool CompareElements(IElement el1, IElement el2);

		[return: MarshalAs(UnmanagedType.Bool)] bool CompareRuntimeIds(int[] runtimeId1, int[] runtimeId2);

		IElement GetRootElement();

		IElement ElementFromHandle(Au.Wnd hwnd);

		IElement ElementFromPoint(Point pt);

		IElement GetFocusedElement();

		IElement GetRootElementBuildCache(ICacheRequest cacheRequest);

		IElement ElementFromHandleBuildCache(Au.Wnd hwnd, ICacheRequest cacheRequest);

		IElement ElementFromPointBuildCache(Point pt, ICacheRequest cacheRequest);

		IElement GetFocusedElementBuildCache(ICacheRequest cacheRequest);

		ITreeWalker CreateTreeWalker(ICondition pCondition);

		ITreeWalker ControlViewWalker { get; }

		ITreeWalker ContentViewWalker { get; }

		ITreeWalker RawViewWalker { get; }

		ICondition RawViewCondition { get; }

		ICondition ControlViewCondition { get; }

		ICondition ContentViewCondition { get; }

		ICacheRequest CreateCacheRequest();

		ICondition CreateTrueCondition();

		ICondition CreateFalseCondition();

		ICondition CreatePropertyCondition(PropertyId propertyId, object value);

		ICondition CreatePropertyConditionEx(PropertyId propertyId, object value, PropertyConditionFlags flags);

		ICondition CreateAndCondition(ICondition condition1, ICondition condition2);

		ICondition CreateAndConditionFromArray(ICondition[] conditions);

		ICondition CreateAndConditionFromNativeArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ICondition[] conditions, int conditionCount);

		ICondition CreateOrCondition(ICondition condition1, ICondition condition2);

		ICondition CreateOrConditionFromArray(ICondition[] conditions);

		ICondition CreateOrConditionFromNativeArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ICondition[] conditions, int conditionCount);

		ICondition CreateNotCondition(ICondition condition);

		void AddAutomationEventHandler(EventId eventId, IElement element, TreeScope scope, ICacheRequest cacheRequest, IEventHandler handler);

		void RemoveAutomationEventHandler(EventId eventId, IElement element, IEventHandler handler);

		void AddPropertyChangedEventHandlerNativeArray(IElement element, TreeScope scope, ICacheRequest cacheRequest, IPropertyChangedEventHandler handler, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] PropertyId[] propertyArray, int propertyCount);

		void AddPropertyChangedEventHandler(IElement element, TreeScope scope, ICacheRequest cacheRequest, IPropertyChangedEventHandler handler, PropertyId[] propertyArray);

		void RemovePropertyChangedEventHandler(IElement element, IPropertyChangedEventHandler handler);

		void AddStructureChangedEventHandler(IElement element, TreeScope scope, ICacheRequest cacheRequest, IStructureChangedEventHandler handler);

		void RemoveStructureChangedEventHandler(IElement element, IStructureChangedEventHandler handler);

		void AddFocusChangedEventHandler(ICacheRequest cacheRequest, IFocusChangedEventHandler handler);

		void RemoveFocusChangedEventHandler(IFocusChangedEventHandler handler);

		void RemoveAllEventHandlers();

		int[] IntNativeArrayToSafeArray(IntPtr array, int arrayCount);

		int IntSafeArrayToNativeArray(int[] intArray, [Out] IntPtr array);

		object RectToVariant(RECT rc);

		RECT VariantToRect(object var);

		int SafeArrayToRectNativeArray(double[] rects, [Out] IntPtr rectArray);

		IProxyFactoryEntry CreateProxyFactoryEntry(IProxyFactory factory);

		IProxyFactoryMapping ProxyFactoryMapping { get; }

		string GetPropertyProgrammaticName(PropertyId propertyId);

		string GetPatternProgrammaticName(PatternId patternId);

		void PollForPotentialSupportedPatterns(IElement pElement, out PatternId[] patternIds, out string[] patternNames);

		void PollForPotentialSupportedProperties(IElement pElement, out PropertyId[] propertyIds, out string[] propertyNames);

		[return: MarshalAs(UnmanagedType.Bool)] bool CheckNotSupported(object value);

		object ReservedNotSupportedValue { [return: MarshalAs(UnmanagedType.IUnknown)] get; }

		object ReservedMixedAttributeValue { [return: MarshalAs(UnmanagedType.IUnknown)] get; }

		IElement ElementFromIAccessible(IAccessible accessible, int childId);

		IElement ElementFromIAccessibleBuildCache(IAccessible accessible, int childId, ICacheRequest cacheRequest);
	}

	[Guid("34723AFF-0C9D-49D0-9896-7AB52DF8CD8A")]
	[InterfaceType(1)]
	[ComImport]
	public interface IUIAutomation2 :IUIAutomation
	{
		[return: MarshalAs(UnmanagedType.Bool)] new bool CompareElements(IElement el1, IElement el2);

		[return: MarshalAs(UnmanagedType.Bool)] new bool CompareRuntimeIds(int[] runtimeId1, int[] runtimeId2);

		new IElement GetRootElement();

		new IElement ElementFromHandle(Au.Wnd hwnd);

		new IElement ElementFromPoint(Point pt);

		new IElement GetFocusedElement();

		new IElement GetRootElementBuildCache(ICacheRequest cacheRequest);

		new IElement ElementFromHandleBuildCache(Au.Wnd hwnd, ICacheRequest cacheRequest);

		new IElement ElementFromPointBuildCache(Point pt, ICacheRequest cacheRequest);

		new IElement GetFocusedElementBuildCache(ICacheRequest cacheRequest);

		new ITreeWalker CreateTreeWalker(ICondition pCondition);

		new ITreeWalker ControlViewWalker { get; }

		new ITreeWalker ContentViewWalker { get; }

		new ITreeWalker RawViewWalker { get; }

		new ICondition RawViewCondition { get; }

		new ICondition ControlViewCondition { get; }

		new ICondition ContentViewCondition { get; }

		new ICacheRequest CreateCacheRequest();

		new ICondition CreateTrueCondition();

		new ICondition CreateFalseCondition();

		new ICondition CreatePropertyCondition(PropertyId propertyId, object value);

		new ICondition CreatePropertyConditionEx(PropertyId propertyId, object value, PropertyConditionFlags flags);

		new ICondition CreateAndCondition(ICondition condition1, ICondition condition2);

		new ICondition CreateAndConditionFromArray(ICondition[] conditions);

		new ICondition CreateAndConditionFromNativeArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ICondition[] conditions, int conditionCount);

		new ICondition CreateOrCondition(ICondition condition1, ICondition condition2);

		new ICondition CreateOrConditionFromArray(ICondition[] conditions);

		new ICondition CreateOrConditionFromNativeArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ICondition[] conditions, int conditionCount);

		new ICondition CreateNotCondition(ICondition condition);

		new void AddAutomationEventHandler(EventId eventId, IElement element, TreeScope scope, ICacheRequest cacheRequest, IEventHandler handler);

		new void RemoveAutomationEventHandler(EventId eventId, IElement element, IEventHandler handler);

		new void AddPropertyChangedEventHandlerNativeArray(IElement element, TreeScope scope, ICacheRequest cacheRequest, IPropertyChangedEventHandler handler, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] PropertyId[] propertyArray, int propertyCount);

		new void AddPropertyChangedEventHandler(IElement element, TreeScope scope, ICacheRequest cacheRequest, IPropertyChangedEventHandler handler, PropertyId[] propertyArray);

		new void RemovePropertyChangedEventHandler(IElement element, IPropertyChangedEventHandler handler);

		new void AddStructureChangedEventHandler(IElement element, TreeScope scope, ICacheRequest cacheRequest, IStructureChangedEventHandler handler);

		new void RemoveStructureChangedEventHandler(IElement element, IStructureChangedEventHandler handler);

		new void AddFocusChangedEventHandler(ICacheRequest cacheRequest, IFocusChangedEventHandler handler);

		new void RemoveFocusChangedEventHandler(IFocusChangedEventHandler handler);

		new void RemoveAllEventHandlers();

		new int[] IntNativeArrayToSafeArray(IntPtr array, int arrayCount);

		new int IntSafeArrayToNativeArray(int[] intArray, [Out] IntPtr array);

		new object RectToVariant(RECT rc);

		new RECT VariantToRect(object var);

		new int SafeArrayToRectNativeArray(double[] rects, [Out] IntPtr rectArray);

		new IProxyFactoryEntry CreateProxyFactoryEntry(IProxyFactory factory);

		new IProxyFactoryMapping ProxyFactoryMapping { get; }

		new string GetPropertyProgrammaticName(PropertyId propertyId);

		new string GetPatternProgrammaticName(PatternId patternId);

		new void PollForPotentialSupportedPatterns(IElement pElement, out PatternId[] patternIds, out string[] patternNames);

		new void PollForPotentialSupportedProperties(IElement pElement, out PropertyId[] propertyIds, out string[] propertyNames);

		[return: MarshalAs(UnmanagedType.Bool)] new bool CheckNotSupported(object value);

		new object ReservedNotSupportedValue { [return: MarshalAs(UnmanagedType.IUnknown)] get; }

		new object ReservedMixedAttributeValue { [return: MarshalAs(UnmanagedType.IUnknown)] get; }

		new IElement ElementFromIAccessible(IAccessible accessible, int childId);

		new IElement ElementFromIAccessibleBuildCache(IAccessible accessible, int childId, ICacheRequest cacheRequest);

		bool AutoSetFocus { [return: MarshalAs(UnmanagedType.Bool)] get; [param: MarshalAs(UnmanagedType.Bool)] set; }

		int ConnectionTimeout { get; set; }

		int TransactionTimeout { get; set; }
	}

	[Guid("73D768DA-9B51-4B89-936E-C209290973E7")]
	[InterfaceType(1)]
	[ComImport]
	public interface IUIAutomation3 :IUIAutomation2
	{
		[return: MarshalAs(UnmanagedType.Bool)] new bool CompareElements(IElement el1, IElement el2);

		[return: MarshalAs(UnmanagedType.Bool)] new bool CompareRuntimeIds(int[] runtimeId1, int[] runtimeId2);

		new IElement GetRootElement();

		new IElement ElementFromHandle(Au.Wnd hwnd);

		new IElement ElementFromPoint(Point pt);

		new IElement GetFocusedElement();

		new IElement GetRootElementBuildCache(ICacheRequest cacheRequest);

		new IElement ElementFromHandleBuildCache(Au.Wnd hwnd, ICacheRequest cacheRequest);

		new IElement ElementFromPointBuildCache(Point pt, ICacheRequest cacheRequest);

		new IElement GetFocusedElementBuildCache(ICacheRequest cacheRequest);

		new ITreeWalker CreateTreeWalker(ICondition pCondition);

		new ITreeWalker ControlViewWalker { get; }

		new ITreeWalker ContentViewWalker { get; }

		new ITreeWalker RawViewWalker { get; }

		new ICondition RawViewCondition { get; }

		new ICondition ControlViewCondition { get; }

		new ICondition ContentViewCondition { get; }

		new ICacheRequest CreateCacheRequest();

		new ICondition CreateTrueCondition();

		new ICondition CreateFalseCondition();

		new ICondition CreatePropertyCondition(PropertyId propertyId, object value);

		new ICondition CreatePropertyConditionEx(PropertyId propertyId, object value, PropertyConditionFlags flags);

		new ICondition CreateAndCondition(ICondition condition1, ICondition condition2);

		new ICondition CreateAndConditionFromArray(ICondition[] conditions);

		new ICondition CreateAndConditionFromNativeArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ICondition[] conditions, int conditionCount);

		new ICondition CreateOrCondition(ICondition condition1, ICondition condition2);

		new ICondition CreateOrConditionFromArray(ICondition[] conditions);

		new ICondition CreateOrConditionFromNativeArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ICondition[] conditions, int conditionCount);

		new ICondition CreateNotCondition(ICondition condition);

		new void AddAutomationEventHandler(EventId eventId, IElement element, TreeScope scope, ICacheRequest cacheRequest, IEventHandler handler);

		new void RemoveAutomationEventHandler(EventId eventId, IElement element, IEventHandler handler);

		new void AddPropertyChangedEventHandlerNativeArray(IElement element, TreeScope scope, ICacheRequest cacheRequest, IPropertyChangedEventHandler handler, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] PropertyId[] propertyArray, int propertyCount);

		new void AddPropertyChangedEventHandler(IElement element, TreeScope scope, ICacheRequest cacheRequest, IPropertyChangedEventHandler handler, PropertyId[] propertyArray);

		new void RemovePropertyChangedEventHandler(IElement element, IPropertyChangedEventHandler handler);

		new void AddStructureChangedEventHandler(IElement element, TreeScope scope, ICacheRequest cacheRequest, IStructureChangedEventHandler handler);

		new void RemoveStructureChangedEventHandler(IElement element, IStructureChangedEventHandler handler);

		new void AddFocusChangedEventHandler(ICacheRequest cacheRequest, IFocusChangedEventHandler handler);

		new void RemoveFocusChangedEventHandler(IFocusChangedEventHandler handler);

		new void RemoveAllEventHandlers();

		new int[] IntNativeArrayToSafeArray(IntPtr array, int arrayCount);

		new int IntSafeArrayToNativeArray(int[] intArray, [Out] IntPtr array);

		new object RectToVariant(RECT rc);

		new RECT VariantToRect(object var);

		new int SafeArrayToRectNativeArray(double[] rects, [Out] IntPtr rectArray);

		new IProxyFactoryEntry CreateProxyFactoryEntry(IProxyFactory factory);

		new IProxyFactoryMapping ProxyFactoryMapping { get; }

		new string GetPropertyProgrammaticName(PropertyId propertyId);

		new string GetPatternProgrammaticName(PatternId patternId);

		new void PollForPotentialSupportedPatterns(IElement pElement, out PatternId[] patternIds, out string[] patternNames);

		new void PollForPotentialSupportedProperties(IElement pElement, out PropertyId[] propertyIds, out string[] propertyNames);

		[return: MarshalAs(UnmanagedType.Bool)] new bool CheckNotSupported(object value);

		new object ReservedNotSupportedValue { [return: MarshalAs(UnmanagedType.IUnknown)] get; }

		new object ReservedMixedAttributeValue { [return: MarshalAs(UnmanagedType.IUnknown)] get; }

		new IElement ElementFromIAccessible(IAccessible accessible, int childId);

		new IElement ElementFromIAccessibleBuildCache(IAccessible accessible, int childId, ICacheRequest cacheRequest);

		new bool AutoSetFocus { [return: MarshalAs(UnmanagedType.Bool)] get; [param: MarshalAs(UnmanagedType.Bool)] set; }

		new int ConnectionTimeout { get; set; }

		new int TransactionTimeout { get; set; }

		void AddTextEditTextChangedEventHandler(IElement element, TreeScope scope, TextEditChangeType TextEditChangeType, ICacheRequest cacheRequest, ITextEditTextChangedEventHandler handler);

		void RemoveTextEditTextChangedEventHandler(IElement element, ITextEditTextChangedEventHandler handler);
	}

	[Guid("1189C02A-05F8-4319-8E21-E817E3DB2860")]
	[InterfaceType(1)]
	[ComImport]
	public interface IUIAutomation4 :IUIAutomation3
	{
		[return: MarshalAs(UnmanagedType.Bool)] new bool CompareElements(IElement el1, IElement el2);

		[return: MarshalAs(UnmanagedType.Bool)] new bool CompareRuntimeIds(int[] runtimeId1, int[] runtimeId2);

		new IElement GetRootElement();

		new IElement ElementFromHandle(Au.Wnd hwnd);

		new IElement ElementFromPoint(Point pt);

		new IElement GetFocusedElement();

		new IElement GetRootElementBuildCache(ICacheRequest cacheRequest);

		new IElement ElementFromHandleBuildCache(Au.Wnd hwnd, ICacheRequest cacheRequest);

		new IElement ElementFromPointBuildCache(Point pt, ICacheRequest cacheRequest);

		new IElement GetFocusedElementBuildCache(ICacheRequest cacheRequest);

		new ITreeWalker CreateTreeWalker(ICondition pCondition);

		new ITreeWalker ControlViewWalker { get; }

		new ITreeWalker ContentViewWalker { get; }

		new ITreeWalker RawViewWalker { get; }

		new ICondition RawViewCondition { get; }

		new ICondition ControlViewCondition { get; }

		new ICondition ContentViewCondition { get; }

		new ICacheRequest CreateCacheRequest();

		new ICondition CreateTrueCondition();

		new ICondition CreateFalseCondition();

		new ICondition CreatePropertyCondition(PropertyId propertyId, object value);

		new ICondition CreatePropertyConditionEx(PropertyId propertyId, object value, PropertyConditionFlags flags);

		new ICondition CreateAndCondition(ICondition condition1, ICondition condition2);

		new ICondition CreateAndConditionFromArray(ICondition[] conditions);

		new ICondition CreateAndConditionFromNativeArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ICondition[] conditions, int conditionCount);

		new ICondition CreateOrCondition(ICondition condition1, ICondition condition2);

		new ICondition CreateOrConditionFromArray(ICondition[] conditions);

		new ICondition CreateOrConditionFromNativeArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ICondition[] conditions, int conditionCount);

		new ICondition CreateNotCondition(ICondition condition);

		new void AddAutomationEventHandler(EventId eventId, IElement element, TreeScope scope, ICacheRequest cacheRequest, IEventHandler handler);

		new void RemoveAutomationEventHandler(EventId eventId, IElement element, IEventHandler handler);

		new void AddPropertyChangedEventHandlerNativeArray(IElement element, TreeScope scope, ICacheRequest cacheRequest, IPropertyChangedEventHandler handler, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] PropertyId[] propertyArray, int propertyCount);

		new void AddPropertyChangedEventHandler(IElement element, TreeScope scope, ICacheRequest cacheRequest, IPropertyChangedEventHandler handler, PropertyId[] propertyArray);

		new void RemovePropertyChangedEventHandler(IElement element, IPropertyChangedEventHandler handler);

		new void AddStructureChangedEventHandler(IElement element, TreeScope scope, ICacheRequest cacheRequest, IStructureChangedEventHandler handler);

		new void RemoveStructureChangedEventHandler(IElement element, IStructureChangedEventHandler handler);

		new void AddFocusChangedEventHandler(ICacheRequest cacheRequest, IFocusChangedEventHandler handler);

		new void RemoveFocusChangedEventHandler(IFocusChangedEventHandler handler);

		new void RemoveAllEventHandlers();

		new int[] IntNativeArrayToSafeArray(IntPtr array, int arrayCount);

		new int IntSafeArrayToNativeArray(int[] intArray, [Out] IntPtr array);

		new object RectToVariant(RECT rc);

		new RECT VariantToRect(object var);

		new int SafeArrayToRectNativeArray(double[] rects, [Out] IntPtr rectArray);

		new IProxyFactoryEntry CreateProxyFactoryEntry(IProxyFactory factory);

		new IProxyFactoryMapping ProxyFactoryMapping { get; }

		new string GetPropertyProgrammaticName(PropertyId propertyId);

		new string GetPatternProgrammaticName(PatternId patternId);

		new void PollForPotentialSupportedPatterns(IElement pElement, out PatternId[] patternIds, out string[] patternNames);

		new void PollForPotentialSupportedProperties(IElement pElement, out PropertyId[] propertyIds, out string[] propertyNames);

		[return: MarshalAs(UnmanagedType.Bool)] new bool CheckNotSupported(object value);

		new object ReservedNotSupportedValue { [return: MarshalAs(UnmanagedType.IUnknown)] get; }

		new object ReservedMixedAttributeValue { [return: MarshalAs(UnmanagedType.IUnknown)] get; }

		new IElement ElementFromIAccessible(IAccessible accessible, int childId);

		new IElement ElementFromIAccessibleBuildCache(IAccessible accessible, int childId, ICacheRequest cacheRequest);

		new bool AutoSetFocus { [return: MarshalAs(UnmanagedType.Bool)] get; [param: MarshalAs(UnmanagedType.Bool)] set; }

		new int ConnectionTimeout { get; set; }

		new int TransactionTimeout { get; set; }

		new void AddTextEditTextChangedEventHandler(IElement element, TreeScope scope, TextEditChangeType TextEditChangeType, ICacheRequest cacheRequest, ITextEditTextChangedEventHandler handler);

		new void RemoveTextEditTextChangedEventHandler(IElement element, ITextEditTextChangedEventHandler handler);

		void AddChangesEventHandler(IElement element, TreeScope scope, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] ChangeId[] changeTypes, int changesCount, ICacheRequest pCacheRequest, IChangesEventHandler handler);

		void RemoveChangesEventHandler(IElement element, IChangesEventHandler handler);
	}

	[Guid("25F700C8-D816-4057-A9DC-3CBDEE77E256")]
	[InterfaceType(1)]
	[ComImport]
	public interface IUIAutomation5 :IUIAutomation4
	{
		[return: MarshalAs(UnmanagedType.Bool)] new bool CompareElements(IElement el1, IElement el2);

		[return: MarshalAs(UnmanagedType.Bool)] new bool CompareRuntimeIds(int[] runtimeId1, int[] runtimeId2);

		new IElement GetRootElement();

		new IElement ElementFromHandle(Au.Wnd hwnd);

		new IElement ElementFromPoint(Point pt);

		new IElement GetFocusedElement();

		new IElement GetRootElementBuildCache(ICacheRequest cacheRequest);

		new IElement ElementFromHandleBuildCache(Au.Wnd hwnd, ICacheRequest cacheRequest);

		new IElement ElementFromPointBuildCache(Point pt, ICacheRequest cacheRequest);

		new IElement GetFocusedElementBuildCache(ICacheRequest cacheRequest);

		new ITreeWalker CreateTreeWalker(ICondition pCondition);

		new ITreeWalker ControlViewWalker { get; }

		new ITreeWalker ContentViewWalker { get; }

		new ITreeWalker RawViewWalker { get; }

		new ICondition RawViewCondition { get; }

		new ICondition ControlViewCondition { get; }

		new ICondition ContentViewCondition { get; }

		new ICacheRequest CreateCacheRequest();

		new ICondition CreateTrueCondition();

		new ICondition CreateFalseCondition();

		new ICondition CreatePropertyCondition(PropertyId propertyId, object value);

		new ICondition CreatePropertyConditionEx(PropertyId propertyId, object value, PropertyConditionFlags flags);

		new ICondition CreateAndCondition(ICondition condition1, ICondition condition2);

		new ICondition CreateAndConditionFromArray(ICondition[] conditions);

		new ICondition CreateAndConditionFromNativeArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ICondition[] conditions, int conditionCount);

		new ICondition CreateOrCondition(ICondition condition1, ICondition condition2);

		new ICondition CreateOrConditionFromArray(ICondition[] conditions);

		new ICondition CreateOrConditionFromNativeArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ICondition[] conditions, int conditionCount);

		new ICondition CreateNotCondition(ICondition condition);

		new void AddAutomationEventHandler(EventId eventId, IElement element, TreeScope scope, ICacheRequest cacheRequest, IEventHandler handler);

		new void RemoveAutomationEventHandler(EventId eventId, IElement element, IEventHandler handler);

		new void AddPropertyChangedEventHandlerNativeArray(IElement element, TreeScope scope, ICacheRequest cacheRequest, IPropertyChangedEventHandler handler, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] PropertyId[] propertyArray, int propertyCount);

		new void AddPropertyChangedEventHandler(IElement element, TreeScope scope, ICacheRequest cacheRequest, IPropertyChangedEventHandler handler, PropertyId[] propertyArray);

		new void RemovePropertyChangedEventHandler(IElement element, IPropertyChangedEventHandler handler);

		new void AddStructureChangedEventHandler(IElement element, TreeScope scope, ICacheRequest cacheRequest, IStructureChangedEventHandler handler);

		new void RemoveStructureChangedEventHandler(IElement element, IStructureChangedEventHandler handler);

		new void AddFocusChangedEventHandler(ICacheRequest cacheRequest, IFocusChangedEventHandler handler);

		new void RemoveFocusChangedEventHandler(IFocusChangedEventHandler handler);

		new void RemoveAllEventHandlers();

		new int[] IntNativeArrayToSafeArray(IntPtr array, int arrayCount);

		new int IntSafeArrayToNativeArray(int[] intArray, [Out] IntPtr array);

		new object RectToVariant(RECT rc);

		new RECT VariantToRect(object var);

		new int SafeArrayToRectNativeArray(double[] rects, [Out] IntPtr rectArray);

		new IProxyFactoryEntry CreateProxyFactoryEntry(IProxyFactory factory);

		new IProxyFactoryMapping ProxyFactoryMapping { get; }

		new string GetPropertyProgrammaticName(PropertyId propertyId);

		new string GetPatternProgrammaticName(PatternId patternId);

		new void PollForPotentialSupportedPatterns(IElement pElement, out PatternId[] patternIds, out string[] patternNames);

		new void PollForPotentialSupportedProperties(IElement pElement, out PropertyId[] propertyIds, out string[] propertyNames);

		[return: MarshalAs(UnmanagedType.Bool)] new bool CheckNotSupported(object value);

		new object ReservedNotSupportedValue { [return: MarshalAs(UnmanagedType.IUnknown)] get; }

		new object ReservedMixedAttributeValue { [return: MarshalAs(UnmanagedType.IUnknown)] get; }

		new IElement ElementFromIAccessible(IAccessible accessible, int childId);

		new IElement ElementFromIAccessibleBuildCache(IAccessible accessible, int childId, ICacheRequest cacheRequest);

		new bool AutoSetFocus { [return: MarshalAs(UnmanagedType.Bool)] get; [param: MarshalAs(UnmanagedType.Bool)] set; }

		new int ConnectionTimeout { get; set; }

		new int TransactionTimeout { get; set; }

		new void AddTextEditTextChangedEventHandler(IElement element, TreeScope scope, TextEditChangeType TextEditChangeType, ICacheRequest cacheRequest, ITextEditTextChangedEventHandler handler);

		new void RemoveTextEditTextChangedEventHandler(IElement element, ITextEditTextChangedEventHandler handler);

		new void AddChangesEventHandler(IElement element, TreeScope scope, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] ChangeId[] changeTypes, int changesCount, ICacheRequest pCacheRequest, IChangesEventHandler handler);

		new void RemoveChangesEventHandler(IElement element, IChangesEventHandler handler);

		void AddNotificationEventHandler(IElement element, TreeScope scope, ICacheRequest cacheRequest, INotificationEventHandler handler);

		void RemoveNotificationEventHandler(IElement element, INotificationEventHandler handler);
	}

	[InterfaceType(1)]
	[ComConversionLoss]
	[Guid("A7D0AF36-B912-45FE-9855-091DDC174AEC")]
	[ComImport]
	public interface IAndCondition :ICondition
	{
		int ChildCount { get; }

		void GetChildrenAsNativeArray([Out] IntPtr childArray, out int childArrayCount);

		ICondition[] GetChildren();
	}

	[InterfaceType(1)]
	[Guid("9A175B21-339E-41B1-8E8B-623F6B681098")]
	[ComImport]
	public interface IAnnotationPattern
	{
		AnnotationType AnnotationTypeId { get; }

		string AnnotationTypeName { get; }

		string Author { get; }

		string DateTime { get; }

		IElement Target { get; }

		AnnotationType CachedAnnotationTypeId { get; }

		string CachedAnnotationTypeName { get; }

		string CachedAuthor { get; }

		string CachedDateTime { get; }

		IElement CachedTarget { get; }
	}

	[InterfaceType(1)]
	[Guid("1B4E1F2E-75EB-4D0B-8952-5A69988E2307")]
	[ComImport]
	public interface IBoolCondition :ICondition
	{
		bool BooleanValue { [return: MarshalAs(UnmanagedType.Bool)] get; }
	}

	[Guid("B32A92B5-BC25-4078-9C08-D7EE95C48E03")]
	[InterfaceType(1)]
	[ComImport]
	public interface ICacheRequest
	{
		void AddProperty(PropertyId propertyId);

		void AddPattern(PatternId patternId);

		ICacheRequest Clone();

		TreeScope TreeScope { get; set; }

		ICondition TreeFilter { get; set; }

		AutomationElementMode AutomationElementMode { get; set; }
	}

	[Guid("58EDCA55-2C3E-4980-B1B9-56C17F27A2A0")]
	[InterfaceType(1)]
	[ComImport]
	public interface IChangesEventHandler
	{
		void HandleChangesEvent(IElement sender, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] UiaChangeInfo[] uiaChanges, int changesCount);
	}

	[InterfaceType(1)]
	[Guid("352FFBA8-0973-437C-A61F-F64CAFD81DF9")]
	[ComImport]
	public interface ICondition
	{
	}

	[Guid("01EA217A-1766-47ED-A6CC-ACF492854B1F")]
	[InterfaceType(1)]
	[ComImport]
	public interface ICustomNavigationPattern
	{
		IElement Navigate(NavigateDirection direction);
	}

	[InterfaceType(1)]
	[Guid("FDE5EF97-1464-48F6-90BF-43D0948E86EC")]
	[ComImport]
	public interface IDockPattern
	{
		void SetDockPosition(DockPosition dockPos);

		DockPosition DockPosition { get; }

		DockPosition CachedDockPosition { get; }
	}

	[InterfaceType(1)]
	[Guid("1DC7B570-1F54-4BAD-BCDA-D36A722FB7BD")]
	[ComImport]
	public interface IDragPattern
	{
		bool IsGrabbed { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedIsGrabbed { [return: MarshalAs(UnmanagedType.Bool)] get; }

		string DropEffect { get; }

		string CachedDropEffect { get; }

		string[] DropEffects { get; }

		string[] CachedDropEffects { get; }

		IElementArray GetCurrentGrabbedItems();

		IElementArray GetCachedGrabbedItems();
	}

	[Guid("69A095F7-EEE4-430E-A46B-FB73B1AE39A5")]
	[InterfaceType(1)]
	[ComImport]
	public interface IDropTargetPattern
	{
		string DropTargetEffect { get; }

		string CachedDropTargetEffect { get; }

		string[] DropTargetEffects { get; }

		string[] CachedDropTargetEffects { get; }
	}

	/// <summary>
	/// UI Automation element. The main interface of UI Automation.
	/// </summary>
	/// <remarks>
	/// COM objects are created using <see cref="IUIAutomation"/> or found using methods like FindFirst. See example.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// Wnd w = Wnd.Find("Options").OrThrow();
	/// var u = new UIA.CUIAutomation() as UIA.IUIAutomation;
	/// var ew = u.ElementFromHandle(w);
	/// Print(ew.Name);
	/// var cond = u.CreatePropertyCondition(UIA.PropertyId.Name, "Run at startup");
	/// cond = u.CreateAndCondition(cond, u.CreatePropertyCondition(UIA.PropertyId.IsOffscreen, false));
	/// var e = ew.FindFirst(UIA.TreeScope.Descendants, cond);
	/// Print(e?.Name);
	/// e?.SetFocus();
	/// ]]></code>
	/// </example>
	[Guid("D22108AA-8AC5-49A5-837B-37BBB3D7591E")]
	[InterfaceType(1)]
	[ComImport]
	public interface IElement
	{
		void SetFocus();

		int[] GetRuntimeId();

		IElement FindFirst(TreeScope scope, ICondition condition);

		IElementArray FindAll(TreeScope scope, ICondition condition);

		IElement FindFirstBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		IElementArray FindAllBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		IElement BuildUpdatedCache(ICacheRequest cacheRequest);

		object GetCurrentPropertyValue(PropertyId propertyId);

		object GetCurrentPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		object GetCachedPropertyValue(PropertyId propertyId);

		object GetCachedPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		IntPtr GetCurrentPatternAs(PatternId patternId, ref Guid riid);

		IntPtr GetCachedPatternAs(PatternId patternId, ref Guid riid);

		[return: MarshalAs(UnmanagedType.IUnknown)] object GetCurrentPattern(PatternId patternId);

		[return: MarshalAs(UnmanagedType.IUnknown)] object GetCachedPattern(PatternId patternId);

		IElement GetCachedParent();

		IElementArray GetCachedChildren();

		int ProcessId { get; }

		TypeId ControlType { get; }

		string LocalizedControlType { get; }

		string Name { get; }

		string AcceleratorKey { get; }

		string AccessKey { get; }

		bool HasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool IsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool IsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		string AutomationId { get; }

		string ClassName { get; }

		string HelpText { get; }

		int Culture { get; }

		bool IsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool IsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool IsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		Au.Wnd NativeWindowHandle { get; }

		string ItemType { get; }

		bool IsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		OrientationType Orientation { get; }

		string FrameworkId { get; }

		bool IsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		string ItemStatus { get; }

		RECT BoundingRectangle { get; }

		IElement LabeledBy { get; }

		string AriaRole { get; }

		string AriaProperties { get; }

		bool IsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		IElementArray ControllerFor { get; }

		IElementArray DescribedBy { get; }

		IElementArray FlowsTo { get; }

		string ProviderDescription { get; }

		int CachedProcessId { get; }

		TypeId CachedControlType { get; }

		string CachedLocalizedControlType { get; }

		string CachedName { get; }

		string CachedAcceleratorKey { get; }

		string CachedAccessKey { get; }

		bool CachedHasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedIsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedIsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		string CachedAutomationId { get; }

		string CachedClassName { get; }

		string CachedHelpText { get; }

		int CachedCulture { get; }

		bool CachedIsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedIsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedIsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		Au.Wnd CachedNativeWindowHandle { get; }

		string CachedItemType { get; }

		bool CachedIsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		OrientationType CachedOrientation { get; }

		string CachedFrameworkId { get; }

		bool CachedIsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		string CachedItemStatus { get; }

		RECT CachedBoundingRectangle { get; }

		IElement CachedLabeledBy { get; }

		string CachedAriaRole { get; }

		string CachedAriaProperties { get; }

		bool CachedIsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		IElementArray CachedControllerFor { get; }

		IElementArray CachedDescribedBy { get; }

		IElementArray CachedFlowsTo { get; }

		string CachedProviderDescription { get; }

		[return: MarshalAs(UnmanagedType.Bool)] bool GetClickablePoint(out Point clickable);
	}

	[Guid("6749C683-F70D-4487-A698-5F79D55290D6")]
	[InterfaceType(1)]
	[ComImport]
	public interface IElement2 :IElement
	{
		new void SetFocus();

		new int[] GetRuntimeId();

		new IElement FindFirst(TreeScope scope, ICondition condition);

		new IElementArray FindAll(TreeScope scope, ICondition condition);

		new IElement FindFirstBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElementArray FindAllBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElement BuildUpdatedCache(ICacheRequest cacheRequest);

		new object GetCurrentPropertyValue(PropertyId propertyId);

		new object GetCurrentPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new object GetCachedPropertyValue(PropertyId propertyId);

		new object GetCachedPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new IntPtr GetCurrentPatternAs(PatternId patternId, ref Guid riid);

		new IntPtr GetCachedPatternAs(PatternId patternId, ref Guid riid);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCurrentPattern(PatternId patternId);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCachedPattern(PatternId patternId);

		new IElement GetCachedParent();

		new IElementArray GetCachedChildren();

		new int ProcessId { get; }

		new TypeId ControlType { get; }

		new string LocalizedControlType { get; }

		new string Name { get; }

		new string AcceleratorKey { get; }

		new string AccessKey { get; }

		new bool HasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string AutomationId { get; }

		new string ClassName { get; }

		new string HelpText { get; }

		new int Culture { get; }

		new bool IsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd NativeWindowHandle { get; }

		new string ItemType { get; }

		new bool IsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType Orientation { get; }

		new string FrameworkId { get; }

		new bool IsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string ItemStatus { get; }

		new RECT BoundingRectangle { get; }

		new IElement LabeledBy { get; }

		new string AriaRole { get; }

		new string AriaProperties { get; }

		new bool IsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray ControllerFor { get; }

		new IElementArray DescribedBy { get; }

		new IElementArray FlowsTo { get; }

		new string ProviderDescription { get; }

		new int CachedProcessId { get; }

		new TypeId CachedControlType { get; }

		new string CachedLocalizedControlType { get; }

		new string CachedName { get; }

		new string CachedAcceleratorKey { get; }

		new string CachedAccessKey { get; }

		new bool CachedHasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedAutomationId { get; }

		new string CachedClassName { get; }

		new string CachedHelpText { get; }

		new int CachedCulture { get; }

		new bool CachedIsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd CachedNativeWindowHandle { get; }

		new string CachedItemType { get; }

		new bool CachedIsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType CachedOrientation { get; }

		new string CachedFrameworkId { get; }

		new bool CachedIsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedItemStatus { get; }

		new RECT CachedBoundingRectangle { get; }

		new IElement CachedLabeledBy { get; }

		new string CachedAriaRole { get; }

		new string CachedAriaProperties { get; }

		new bool CachedIsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray CachedControllerFor { get; }

		new IElementArray CachedDescribedBy { get; }

		new IElementArray CachedFlowsTo { get; }

		new string CachedProviderDescription { get; }

		[return: MarshalAs(UnmanagedType.Bool)] new bool GetClickablePoint(out Point clickable);

		bool OptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedOptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		LiveSetting LiveSetting { get; }

		LiveSetting CachedLiveSetting { get; }

		IElementArray FlowsFrom { get; }

		IElementArray CachedFlowsFrom { get; }
	}

	[Guid("8471DF34-AEE0-4A01-A7DE-7DB9AF12C296")]
	[InterfaceType(1)]
	[ComImport]
	public interface IElement3 :IElement2
	{
		new void SetFocus();

		new int[] GetRuntimeId();

		new IElement FindFirst(TreeScope scope, ICondition condition);

		new IElementArray FindAll(TreeScope scope, ICondition condition);

		new IElement FindFirstBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElementArray FindAllBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElement BuildUpdatedCache(ICacheRequest cacheRequest);

		new object GetCurrentPropertyValue(PropertyId propertyId);

		new object GetCurrentPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new object GetCachedPropertyValue(PropertyId propertyId);

		new object GetCachedPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new IntPtr GetCurrentPatternAs(PatternId patternId, ref Guid riid);

		new IntPtr GetCachedPatternAs(PatternId patternId, ref Guid riid);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCurrentPattern(PatternId patternId);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCachedPattern(PatternId patternId);

		new IElement GetCachedParent();

		new IElementArray GetCachedChildren();

		new int ProcessId { get; }

		new TypeId ControlType { get; }

		new string LocalizedControlType { get; }

		new string Name { get; }

		new string AcceleratorKey { get; }

		new string AccessKey { get; }

		new bool HasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string AutomationId { get; }

		new string ClassName { get; }

		new string HelpText { get; }

		new int Culture { get; }

		new bool IsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd NativeWindowHandle { get; }

		new string ItemType { get; }

		new bool IsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType Orientation { get; }

		new string FrameworkId { get; }

		new bool IsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string ItemStatus { get; }

		new RECT BoundingRectangle { get; }

		new IElement LabeledBy { get; }

		new string AriaRole { get; }

		new string AriaProperties { get; }

		new bool IsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray ControllerFor { get; }

		new IElementArray DescribedBy { get; }

		new IElementArray FlowsTo { get; }

		new string ProviderDescription { get; }

		new int CachedProcessId { get; }

		new TypeId CachedControlType { get; }

		new string CachedLocalizedControlType { get; }

		new string CachedName { get; }

		new string CachedAcceleratorKey { get; }

		new string CachedAccessKey { get; }

		new bool CachedHasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedAutomationId { get; }

		new string CachedClassName { get; }

		new string CachedHelpText { get; }

		new int CachedCulture { get; }

		new bool CachedIsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd CachedNativeWindowHandle { get; }

		new string CachedItemType { get; }

		new bool CachedIsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType CachedOrientation { get; }

		new string CachedFrameworkId { get; }

		new bool CachedIsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedItemStatus { get; }

		new RECT CachedBoundingRectangle { get; }

		new IElement CachedLabeledBy { get; }

		new string CachedAriaRole { get; }

		new string CachedAriaProperties { get; }

		new bool CachedIsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray CachedControllerFor { get; }

		new IElementArray CachedDescribedBy { get; }

		new IElementArray CachedFlowsTo { get; }

		new string CachedProviderDescription { get; }

		[return: MarshalAs(UnmanagedType.Bool)] new bool GetClickablePoint(out Point clickable);

		new bool OptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedOptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new LiveSetting LiveSetting { get; }

		new LiveSetting CachedLiveSetting { get; }

		new IElementArray FlowsFrom { get; }

		new IElementArray CachedFlowsFrom { get; }

		void ShowContextMenu();

		bool IsPeripheral { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedIsPeripheral { [return: MarshalAs(UnmanagedType.Bool)] get; }
	}

	[InterfaceType(1)]
	[Guid("3B6E233C-52FB-4063-A4C9-77C075C2A06B")]
	[ComImport]
	public interface IElement4 :IElement3
	{
		new void SetFocus();

		new int[] GetRuntimeId();

		new IElement FindFirst(TreeScope scope, ICondition condition);

		new IElementArray FindAll(TreeScope scope, ICondition condition);

		new IElement FindFirstBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElementArray FindAllBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElement BuildUpdatedCache(ICacheRequest cacheRequest);

		new object GetCurrentPropertyValue(PropertyId propertyId);

		new object GetCurrentPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new object GetCachedPropertyValue(PropertyId propertyId);

		new object GetCachedPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new IntPtr GetCurrentPatternAs(PatternId patternId, ref Guid riid);

		new IntPtr GetCachedPatternAs(PatternId patternId, ref Guid riid);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCurrentPattern(PatternId patternId);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCachedPattern(PatternId patternId);

		new IElement GetCachedParent();

		new IElementArray GetCachedChildren();

		new int ProcessId { get; }

		new TypeId ControlType { get; }

		new string LocalizedControlType { get; }

		new string Name { get; }

		new string AcceleratorKey { get; }

		new string AccessKey { get; }

		new bool HasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string AutomationId { get; }

		new string ClassName { get; }

		new string HelpText { get; }

		new int Culture { get; }

		new bool IsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd NativeWindowHandle { get; }

		new string ItemType { get; }

		new bool IsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType Orientation { get; }

		new string FrameworkId { get; }

		new bool IsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string ItemStatus { get; }

		new RECT BoundingRectangle { get; }

		new IElement LabeledBy { get; }

		new string AriaRole { get; }

		new string AriaProperties { get; }

		new bool IsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray ControllerFor { get; }

		new IElementArray DescribedBy { get; }

		new IElementArray FlowsTo { get; }

		new string ProviderDescription { get; }

		new int CachedProcessId { get; }

		new TypeId CachedControlType { get; }

		new string CachedLocalizedControlType { get; }

		new string CachedName { get; }

		new string CachedAcceleratorKey { get; }

		new string CachedAccessKey { get; }

		new bool CachedHasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedAutomationId { get; }

		new string CachedClassName { get; }

		new string CachedHelpText { get; }

		new int CachedCulture { get; }

		new bool CachedIsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd CachedNativeWindowHandle { get; }

		new string CachedItemType { get; }

		new bool CachedIsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType CachedOrientation { get; }

		new string CachedFrameworkId { get; }

		new bool CachedIsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedItemStatus { get; }

		new RECT CachedBoundingRectangle { get; }

		new IElement CachedLabeledBy { get; }

		new string CachedAriaRole { get; }

		new string CachedAriaProperties { get; }

		new bool CachedIsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray CachedControllerFor { get; }

		new IElementArray CachedDescribedBy { get; }

		new IElementArray CachedFlowsTo { get; }

		new string CachedProviderDescription { get; }

		[return: MarshalAs(UnmanagedType.Bool)] new bool GetClickablePoint(out Point clickable);

		new bool OptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedOptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new LiveSetting LiveSetting { get; }

		new LiveSetting CachedLiveSetting { get; }

		new IElementArray FlowsFrom { get; }

		new IElementArray CachedFlowsFrom { get; }

		new void ShowContextMenu();

		new bool IsPeripheral { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsPeripheral { [return: MarshalAs(UnmanagedType.Bool)] get; }

		int PositionInSet { get; }

		int SizeOfSet { get; }

		int Level { get; }

		AnnotationType[] AnnotationTypes { get; }

		IElementArray AnnotationObjects { get; }

		int CachedPositionInSet { get; }

		int CachedSizeOfSet { get; }

		int CachedLevel { get; }

		AnnotationType[] CachedAnnotationTypes { get; }

		IElementArray CachedAnnotationObjects { get; }
	}

	[Guid("98141C1D-0D0E-4175-BBE2-6BFF455842A7")]
	[InterfaceType(1)]
	[ComImport]
	public interface IElement5 :IElement4
	{
		new void SetFocus();

		new int[] GetRuntimeId();

		new IElement FindFirst(TreeScope scope, ICondition condition);

		new IElementArray FindAll(TreeScope scope, ICondition condition);

		new IElement FindFirstBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElementArray FindAllBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElement BuildUpdatedCache(ICacheRequest cacheRequest);

		new object GetCurrentPropertyValue(PropertyId propertyId);

		new object GetCurrentPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new object GetCachedPropertyValue(PropertyId propertyId);

		new object GetCachedPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new IntPtr GetCurrentPatternAs(PatternId patternId, ref Guid riid);

		new IntPtr GetCachedPatternAs(PatternId patternId, ref Guid riid);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCurrentPattern(PatternId patternId);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCachedPattern(PatternId patternId);

		new IElement GetCachedParent();

		new IElementArray GetCachedChildren();

		new int ProcessId { get; }

		new TypeId ControlType { get; }

		new string LocalizedControlType { get; }

		new string Name { get; }

		new string AcceleratorKey { get; }

		new string AccessKey { get; }

		new bool HasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string AutomationId { get; }

		new string ClassName { get; }

		new string HelpText { get; }

		new int Culture { get; }

		new bool IsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd NativeWindowHandle { get; }

		new string ItemType { get; }

		new bool IsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType Orientation { get; }

		new string FrameworkId { get; }

		new bool IsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string ItemStatus { get; }

		new RECT BoundingRectangle { get; }

		new IElement LabeledBy { get; }

		new string AriaRole { get; }

		new string AriaProperties { get; }

		new bool IsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray ControllerFor { get; }

		new IElementArray DescribedBy { get; }

		new IElementArray FlowsTo { get; }

		new string ProviderDescription { get; }

		new int CachedProcessId { get; }

		new TypeId CachedControlType { get; }

		new string CachedLocalizedControlType { get; }

		new string CachedName { get; }

		new string CachedAcceleratorKey { get; }

		new string CachedAccessKey { get; }

		new bool CachedHasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedAutomationId { get; }

		new string CachedClassName { get; }

		new string CachedHelpText { get; }

		new int CachedCulture { get; }

		new bool CachedIsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd CachedNativeWindowHandle { get; }

		new string CachedItemType { get; }

		new bool CachedIsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType CachedOrientation { get; }

		new string CachedFrameworkId { get; }

		new bool CachedIsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedItemStatus { get; }

		new RECT CachedBoundingRectangle { get; }

		new IElement CachedLabeledBy { get; }

		new string CachedAriaRole { get; }

		new string CachedAriaProperties { get; }

		new bool CachedIsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray CachedControllerFor { get; }

		new IElementArray CachedDescribedBy { get; }

		new IElementArray CachedFlowsTo { get; }

		new string CachedProviderDescription { get; }

		[return: MarshalAs(UnmanagedType.Bool)] new bool GetClickablePoint(out Point clickable);

		new bool OptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedOptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new LiveSetting LiveSetting { get; }

		new LiveSetting CachedLiveSetting { get; }

		new IElementArray FlowsFrom { get; }

		new IElementArray CachedFlowsFrom { get; }

		new void ShowContextMenu();

		new bool IsPeripheral { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsPeripheral { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new int PositionInSet { get; }

		new int SizeOfSet { get; }

		new int Level { get; }

		new AnnotationType[] AnnotationTypes { get; }

		new IElementArray AnnotationObjects { get; }

		new int CachedPositionInSet { get; }

		new int CachedSizeOfSet { get; }

		new int CachedLevel { get; }

		new AnnotationType[] CachedAnnotationTypes { get; }

		new IElementArray CachedAnnotationObjects { get; }

		LandmarkTypeId LandmarkType { get; }

		string LocalizedLandmarkType { get; }

		LandmarkTypeId CachedLandmarkType { get; }

		string CachedLocalizedLandmarkType { get; }
	}

	[InterfaceType(1)]
	[Guid("4780D450-8BCA-4977-AFA5-A4A517F555E3")]
	[ComImport]
	public interface IElement6 :IElement5
	{
		new void SetFocus();

		new int[] GetRuntimeId();

		new IElement FindFirst(TreeScope scope, ICondition condition);

		new IElementArray FindAll(TreeScope scope, ICondition condition);

		new IElement FindFirstBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElementArray FindAllBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElement BuildUpdatedCache(ICacheRequest cacheRequest);

		new object GetCurrentPropertyValue(PropertyId propertyId);

		new object GetCurrentPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new object GetCachedPropertyValue(PropertyId propertyId);

		new object GetCachedPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new IntPtr GetCurrentPatternAs(PatternId patternId, ref Guid riid);

		new IntPtr GetCachedPatternAs(PatternId patternId, ref Guid riid);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCurrentPattern(PatternId patternId);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCachedPattern(PatternId patternId);

		new IElement GetCachedParent();

		new IElementArray GetCachedChildren();

		new int ProcessId { get; }

		new TypeId ControlType { get; }

		new string LocalizedControlType { get; }

		new string Name { get; }

		new string AcceleratorKey { get; }

		new string AccessKey { get; }

		new bool HasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string AutomationId { get; }

		new string ClassName { get; }

		new string HelpText { get; }

		new int Culture { get; }

		new bool IsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd NativeWindowHandle { get; }

		new string ItemType { get; }

		new bool IsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType Orientation { get; }

		new string FrameworkId { get; }

		new bool IsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string ItemStatus { get; }

		new RECT BoundingRectangle { get; }

		new IElement LabeledBy { get; }

		new string AriaRole { get; }

		new string AriaProperties { get; }

		new bool IsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray ControllerFor { get; }

		new IElementArray DescribedBy { get; }

		new IElementArray FlowsTo { get; }

		new string ProviderDescription { get; }

		new int CachedProcessId { get; }

		new TypeId CachedControlType { get; }

		new string CachedLocalizedControlType { get; }

		new string CachedName { get; }

		new string CachedAcceleratorKey { get; }

		new string CachedAccessKey { get; }

		new bool CachedHasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedAutomationId { get; }

		new string CachedClassName { get; }

		new string CachedHelpText { get; }

		new int CachedCulture { get; }

		new bool CachedIsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd CachedNativeWindowHandle { get; }

		new string CachedItemType { get; }

		new bool CachedIsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType CachedOrientation { get; }

		new string CachedFrameworkId { get; }

		new bool CachedIsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedItemStatus { get; }

		new RECT CachedBoundingRectangle { get; }

		new IElement CachedLabeledBy { get; }

		new string CachedAriaRole { get; }

		new string CachedAriaProperties { get; }

		new bool CachedIsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray CachedControllerFor { get; }

		new IElementArray CachedDescribedBy { get; }

		new IElementArray CachedFlowsTo { get; }

		new string CachedProviderDescription { get; }

		[return: MarshalAs(UnmanagedType.Bool)] new bool GetClickablePoint(out Point clickable);

		new bool OptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedOptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new LiveSetting LiveSetting { get; }

		new LiveSetting CachedLiveSetting { get; }

		new IElementArray FlowsFrom { get; }

		new IElementArray CachedFlowsFrom { get; }

		new void ShowContextMenu();

		new bool IsPeripheral { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsPeripheral { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new int PositionInSet { get; }

		new int SizeOfSet { get; }

		new int Level { get; }

		new AnnotationType[] AnnotationTypes { get; }

		new IElementArray AnnotationObjects { get; }

		new int CachedPositionInSet { get; }

		new int CachedSizeOfSet { get; }

		new int CachedLevel { get; }

		new AnnotationType[] CachedAnnotationTypes { get; }

		new IElementArray CachedAnnotationObjects { get; }

		new LandmarkTypeId LandmarkType { get; }

		new string LocalizedLandmarkType { get; }

		new LandmarkTypeId CachedLandmarkType { get; }

		new string CachedLocalizedLandmarkType { get; }

		string FullDescription { get; }

		string CachedFullDescription { get; }
	}

	[Guid("204E8572-CFC3-4C11-B0C8-7DA7420750B7")]
	[InterfaceType(1)]
	[ComImport]
	public interface IElement7 :IElement6
	{
		new void SetFocus();

		new int[] GetRuntimeId();

		new IElement FindFirst(TreeScope scope, ICondition condition);

		new IElementArray FindAll(TreeScope scope, ICondition condition);

		new IElement FindFirstBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElementArray FindAllBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest);

		new IElement BuildUpdatedCache(ICacheRequest cacheRequest);

		new object GetCurrentPropertyValue(PropertyId propertyId);

		new object GetCurrentPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new object GetCachedPropertyValue(PropertyId propertyId);

		new object GetCachedPropertyValueEx(PropertyId propertyId, [MarshalAs(UnmanagedType.Bool)] bool ignoreDefaultValue);

		new IntPtr GetCurrentPatternAs(PatternId patternId, ref Guid riid);

		new IntPtr GetCachedPatternAs(PatternId patternId, ref Guid riid);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCurrentPattern(PatternId patternId);

		[return: MarshalAs(UnmanagedType.IUnknown)] new object GetCachedPattern(PatternId patternId);

		new IElement GetCachedParent();

		new IElementArray GetCachedChildren();

		new int ProcessId { get; }

		new TypeId ControlType { get; }

		new string LocalizedControlType { get; }

		new string Name { get; }

		new string AcceleratorKey { get; }

		new string AccessKey { get; }

		new bool HasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string AutomationId { get; }

		new string ClassName { get; }

		new string HelpText { get; }

		new int Culture { get; }

		new bool IsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd NativeWindowHandle { get; }

		new string ItemType { get; }

		new bool IsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType Orientation { get; }

		new string FrameworkId { get; }

		new bool IsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string ItemStatus { get; }

		new RECT BoundingRectangle { get; }

		new IElement LabeledBy { get; }

		new string AriaRole { get; }

		new string AriaProperties { get; }

		new bool IsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray ControllerFor { get; }

		new IElementArray DescribedBy { get; }

		new IElementArray FlowsTo { get; }

		new string ProviderDescription { get; }

		new int CachedProcessId { get; }

		new TypeId CachedControlType { get; }

		new string CachedLocalizedControlType { get; }

		new string CachedName { get; }

		new string CachedAcceleratorKey { get; }

		new string CachedAccessKey { get; }

		new bool CachedHasKeyboardFocus { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsKeyboardFocusable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsEnabled { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedAutomationId { get; }

		new string CachedClassName { get; }

		new string CachedHelpText { get; }

		new int CachedCulture { get; }

		new bool CachedIsControlElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsContentElement { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsPassword { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new Au.Wnd CachedNativeWindowHandle { get; }

		new string CachedItemType { get; }

		new bool CachedIsOffscreen { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new OrientationType CachedOrientation { get; }

		new string CachedFrameworkId { get; }

		new bool CachedIsRequiredForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new string CachedItemStatus { get; }

		new RECT CachedBoundingRectangle { get; }

		new IElement CachedLabeledBy { get; }

		new string CachedAriaRole { get; }

		new string CachedAriaProperties { get; }

		new bool CachedIsDataValidForForm { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray CachedControllerFor { get; }

		new IElementArray CachedDescribedBy { get; }

		new IElementArray CachedFlowsTo { get; }

		new string CachedProviderDescription { get; }

		[return: MarshalAs(UnmanagedType.Bool)] new bool GetClickablePoint(out Point clickable);

		new bool OptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedOptimizeForVisualContent { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new LiveSetting LiveSetting { get; }

		new LiveSetting CachedLiveSetting { get; }

		new IElementArray FlowsFrom { get; }

		new IElementArray CachedFlowsFrom { get; }

		new void ShowContextMenu();

		new bool IsPeripheral { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsPeripheral { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new int PositionInSet { get; }

		new int SizeOfSet { get; }

		new int Level { get; }

		new AnnotationType[] AnnotationTypes { get; }

		new IElementArray AnnotationObjects { get; }

		new int CachedPositionInSet { get; }

		new int CachedSizeOfSet { get; }

		new int CachedLevel { get; }

		new AnnotationType[] CachedAnnotationTypes { get; }

		new IElementArray CachedAnnotationObjects { get; }

		new LandmarkTypeId LandmarkType { get; }

		new string LocalizedLandmarkType { get; }

		new LandmarkTypeId CachedLandmarkType { get; }

		new string CachedLocalizedLandmarkType { get; }

		new string FullDescription { get; }

		new string CachedFullDescription { get; }

		IElement FindFirstWithOptions(TreeScope scope, ICondition condition, TreeTraversalOptions traversalOptions, IElement root);

		IElementArray FindAllWithOptions(TreeScope scope, ICondition condition, TreeTraversalOptions traversalOptions, IElement root);

		IElement FindFirstWithOptionsBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest, TreeTraversalOptions traversalOptions, IElement root);

		IElementArray FindAllWithOptionsBuildCache(TreeScope scope, ICondition condition, ICacheRequest cacheRequest, TreeTraversalOptions traversalOptions, IElement root);

		object GetCurrentMetadataValue(PropertyId targetId, MetadataId metadataId);
	}

	[Guid("14314595-B4BC-4055-95F2-58F2E42C9855")]
	[InterfaceType(1)]
	[ComImport]
	public interface IElementArray
	{
		int Length { get; }

		IElement GetElement(int index);
	}

	[Guid("146C3C17-F12E-4E22-8C27-F894B9B79C69")]
	[InterfaceType(1)]
	[ComImport]
	public interface IEventHandler
	{
		void HandleAutomationEvent(IElement sender, EventId eventId);
	}

	[InterfaceType(1)]
	[Guid("619BE086-1F4E-4EE4-BAFA-210128738730")]
	[ComImport]
	public interface IExpandCollapsePattern
	{
		void Expand();

		void Collapse();

		ExpandCollapseState ExpandCollapseState { get; }

		ExpandCollapseState CachedExpandCollapseState { get; }
	}

	[InterfaceType(1)]
	[Guid("C270F6B5-5C69-4290-9745-7A7F97169468")]
	[ComImport]
	public interface IFocusChangedEventHandler
	{
		void HandleFocusChangedEvent(IElement sender);
	}

	[Guid("78F8EF57-66C3-4E09-BD7C-E79B2004894D")]
	[InterfaceType(1)]
	[ComImport]
	public interface IGridItemPattern
	{
		IElement ContainingGrid { get; }

		int Row { get; }

		int Column { get; }

		int RowSpan { get; }

		int ColumnSpan { get; }

		IElement CachedContainingGrid { get; }

		int CachedRow { get; }

		int CachedColumn { get; }

		int CachedRowSpan { get; }

		int CachedColumnSpan { get; }
	}

	[InterfaceType(1)]
	[Guid("414C3CDC-856B-4F5B-8538-3131C6302550")]
	[ComImport]
	public interface IGridPattern
	{
		IElement GetItem(int row, int column);

		int RowCount { get; }

		int ColumnCount { get; }

		int CachedRowCount { get; }

		int CachedColumnCount { get; }
	}

	[Guid("FB377FBE-8EA6-46D5-9C73-6499642D3059")]
	[InterfaceType(1)]
	[ComImport]
	public interface IInvokePattern
	{
		void Invoke();
	}

	[Guid("C690FDB2-27A8-423C-812D-429773C9084E")]
	[InterfaceType(1)]
	[ComImport]
	public interface IItemContainerPattern
	{
		IElement FindItemByProperty(IElement pStartAfter, PropertyId propertyId, object value);
	}

	[Guid("828055AD-355B-4435-86D5-3B51C14A9B1B")]
	[InterfaceType(1)]
	[ComImport]
	public interface ILegacyIAccessiblePattern
	{
		void Select(AccSELFLAG flagsSelect);

		void DoDefaultAction();

		void SetValue([MarshalAs(UnmanagedType.LPWStr)] string szValue);

		int ChildId { get; }

		string Name { get; }

		string Value { get; }

		string Description { get; }

		AccROLE Role { get; }

		AccSTATE State { get; }

		string Help { get; }

		string KeyboardShortcut { get; }

		IElementArray GetCurrentSelection();

		string DefaultAction { get; }

		int CachedChildId { get; }

		string CachedName { get; }

		string CachedValue { get; }

		string CachedDescription { get; }

		AccROLE CachedRole { get; }

		AccSTATE CachedState { get; }

		string CachedHelp { get; }

		string CachedKeyboardShortcut { get; }

		IElementArray GetCachedSelection();

		string CachedDefaultAction { get; }

		IAccessible GetIAccessible();
	}

	[InterfaceType(1)]
	[Guid("8D253C91-1DC5-4BB5-B18F-ADE16FA495E8")]
	[ComImport]
	public interface IMultipleViewPattern
	{
		string GetViewName(int view);

		void SetCurrentView(int view);

		int CurrentView { get; }

		int[] GetCurrentSupportedViews();

		int CachedCurrentView { get; }

		int[] GetCachedSupportedViews();
	}

	[Guid("F528B657-847B-498C-8896-D52B565407A1")]
	[InterfaceType(1)]
	[ComImport]
	public interface INotCondition :ICondition
	{
		ICondition GetChild();
	}

	[Guid("C7CB2637-E6C2-4D0C-85DE-4948C02175C7")]
	[InterfaceType(1)]
	[ComImport]
	public interface INotificationEventHandler
	{
		void HandleNotificationEvent(IElement sender, NotificationKind NotificationKind, NotificationProcessing NotificationProcessing, string displayString, string activityId);
	}

	[InterfaceType(1)]
	[Guid("71C284B3-C14D-4D14-981E-19751B0D756D")]
	[ComImport]
	public interface IObjectModelPattern
	{
		[return: MarshalAs(UnmanagedType.IUnknown)] object GetUnderlyingObjectModel();
	}

	[ComConversionLoss]
	[Guid("8753F032-3DB1-47B5-A1FC-6E34A266C712")]
	[InterfaceType(1)]
	[ComImport]
	public interface IOrCondition :ICondition
	{
		int ChildCount { get; }

		void GetChildrenAsNativeArray([Out] IntPtr childArray, out int childArrayCount);

		ICondition[] GetChildren();
	}

	[Guid("40CD37D4-C756-4B0C-8C6F-BDDFEEB13B50")]
	[InterfaceType(1)]
	[ComImport]
	public interface IPropertyChangedEventHandler
	{
		void HandlePropertyChangedEvent(IElement sender, PropertyId propertyId, object newValue);
	}

	[InterfaceType(1)]
	[Guid("99EBF2CB-5578-4267-9AD4-AFD6EA77E94B")]
	[ComImport]
	public interface IPropertyCondition :ICondition
	{
		PropertyId propertyId { get; }

		object PropertyValue { get; }

		PropertyConditionFlags PropertyConditionFlags { get; }
	}

	[InterfaceType(1)]
	[Guid("85B94ECD-849D-42B6-B94D-D6DB23FDF5A4")]
	[ComImport]
	public interface IProxyFactory
	{
		IRawElementProviderSimple CreateProvider(Au.Wnd hwnd, AccOBJID idObject, int idChild);

		string ProxyFactoryId { get; }
	}

	[InterfaceType(1)]
	[Guid("D50E472E-B64B-490C-BCA1-D30696F9F289")]
	[ComImport]
	public interface IProxyFactoryEntry
	{
		//note: here cannot use { get; set; } because the 'set' functions are not after their 'get' functions.
		//	In other interfaces OK.

		IProxyFactory ProxyFactory { get; }

		string get_ClassName { get; }

		string get_ImageName { get; }

		bool get_AllowSubstringMatch { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool get_CanCheckBaseClass { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool get_NeedsAdviseEvents { [return: MarshalAs(UnmanagedType.Bool)]  get; }

		string set_ClassName { set; }

		string set_ImageName { set; }

		bool set_AllowSubstringMatch { [param: MarshalAs(UnmanagedType.Bool)] set; }

		bool set_CanCheckBaseClass { [param: MarshalAs(UnmanagedType.Bool)] set; }

		bool set_NeedsAdviseEvents { [param: MarshalAs(UnmanagedType.Bool)] set; }

		void SetWinEventsForAutomationEvent(EventId eventId, PropertyId propertyId, AccEVENT[] winEvents);

		AccEVENT[] GetWinEventsForAutomationEvent(EventId eventId, PropertyId propertyId);
	}

	[Guid("09E31E18-872D-4873-93D1-1E541EC133FD")]
	[InterfaceType(1)]
	[ComImport]
	public interface IProxyFactoryMapping
	{
		int Count { get; }

		[return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)] object[] GetTable();

		IProxyFactoryEntry GetEntry(int index);

		void SetTable([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)] object[] factoryList);

		void InsertEntries(int before, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)] object[] factoryList);

		void InsertEntry(int before, IProxyFactoryEntry factory);

		void RemoveEntry(int index);

		void ClearTable();

		void RestoreDefaultTable();
	}

	[InterfaceType(1)]
	[Guid("59213F4F-7346-49E5-B120-80555987A148")]
	[ComImport]
	public interface IRangeValuePattern
	{
		void SetValue(double val);

		double Value { get; }

		bool IsReadOnly { [return: MarshalAs(UnmanagedType.Bool)] get; }

		double Maximum { get; }

		double Minimum { get; }

		double LargeChange { get; }

		double SmallChange { get; }

		double CachedValue { get; }

		bool CachedIsReadOnly { [return: MarshalAs(UnmanagedType.Bool)] get; }

		double CachedMaximum { get; }

		double CachedMinimum { get; }

		double CachedLargeChange { get; }

		double CachedSmallChange { get; }
	}

	[Guid("B488300F-D015-4F19-9C29-BB595E3645EF")]
	[InterfaceType(1)]
	[ComImport]
	public interface IScrollItemPattern
	{
		void ScrollIntoView();
	}

	[InterfaceType(1)]
	[Guid("88F4D42A-E881-459D-A77C-73BBBB7E02DC")]
	[ComImport]
	public interface IScrollPattern
	{
		void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount);

		void SetScrollPercent(double horizontalPercent, double verticalPercent);

		double HorizontalScrollPercent { get; }

		double VerticalScrollPercent { get; }

		double HorizontalViewSize { get; }

		double VerticalViewSize { get; }

		bool HorizontallyScrollable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool VerticallyScrollable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		double CachedHorizontalScrollPercent { get; }

		double CachedVerticalScrollPercent { get; }

		double CachedHorizontalViewSize { get; }

		double CachedVerticalViewSize { get; }

		bool CachedHorizontallyScrollable { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedVerticallyScrollable { [return: MarshalAs(UnmanagedType.Bool)] get; }
	}

	[Guid("A8EFA66A-0FDA-421A-9194-38021F3578EA")]
	[InterfaceType(1)]
	[ComImport]
	public interface ISelectionItemPattern
	{
		void Select();

		void AddToSelection();

		void RemoveFromSelection();

		bool IsSelected { [return: MarshalAs(UnmanagedType.Bool)] get; }

		IElement SelectionContainer { get; }

		bool CachedIsSelected { [return: MarshalAs(UnmanagedType.Bool)] get; }

		IElement CachedSelectionContainer { get; }
	}

	[InterfaceType(1)]
	[Guid("5ED5202E-B2AC-47A6-B638-4B0BF140D78E")]
	[ComImport]
	public interface ISelectionPattern
	{
		IElementArray GetCurrentSelection();

		bool CanSelectMultiple { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool IsSelectionRequired { [return: MarshalAs(UnmanagedType.Bool)] get; }

		IElementArray GetCachedSelection();

		bool CachedCanSelectMultiple { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedIsSelectionRequired { [return: MarshalAs(UnmanagedType.Bool)] get; }
	}

	[Guid("0532BFAE-C011-4E32-A343-6D642D798555")]
	[InterfaceType(1)]
	[ComImport]
	public interface ISelectionPattern2 :ISelectionPattern
	{
		new IElementArray GetCurrentSelection();

		new bool CanSelectMultiple { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool IsSelectionRequired { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new IElementArray GetCachedSelection();

		new bool CachedCanSelectMultiple { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedIsSelectionRequired { [return: MarshalAs(UnmanagedType.Bool)] get; }

		IElement FirstSelectedItem { get; }

		IElement LastSelectedItem { get; }

		IElement CurrentSelectedItem { get; }

		int ItemCount { get; }

		IElement CachedFirstSelectedItem { get; }

		IElement CachedLastSelectedItem { get; }

		IElement CachedCurrentSelectedItem { get; }

		int CachedItemCount { get; }
	}

	[InterfaceType(1)]
	[Guid("7D4FB86C-8D34-40E1-8E83-62C15204E335")]
	[ComImport]
	public interface ISpreadsheetItemPattern
	{
		string Formula { get; }

		IElementArray GetCurrentAnnotationObjects();

		AnnotationType[] GetCurrentAnnotationTypes();

		string CachedFormula { get; }

		IElementArray GetCachedAnnotationObjects();

		AnnotationType[] GetCachedAnnotationTypes();
	}

	[InterfaceType(1)]
	[Guid("7517A7C8-FAAE-4DE9-9F08-29B91E8595C1")]
	[ComImport]
	public interface ISpreadsheetPattern
	{
		IElement GetItemByName(string name);
	}

	[InterfaceType(1)]
	[Guid("E81D1B4E-11C5-42F8-9754-E7036C79F054")]
	[ComImport]
	public interface IStructureChangedEventHandler
	{
		void HandleStructureChangedEvent(IElement sender, StructureChangeType changeType, int[] runtimeId);
	}

	[ComConversionLoss]
	[Guid("85B5F0A2-BD79-484A-AD2B-388C9838D5FB")]
	[InterfaceType(1)]
	[ComImport]
	public interface IStylesPattern
	{
		StyleId StyleId { get; }

		string StyleName { get; }

		int FillColor { get; }

		string FillPatternStyle { get; }

		string Shape { get; }

		int FillPatternColor { get; }

		string ExtendedProperties { get; }

		void GetCurrentExtendedPropertiesAsArray([Out] IntPtr propertyArray, out int propertyCount);

		StyleId CachedStyleId { get; }

		string CachedStyleName { get; }

		int CachedFillColor { get; }

		string CachedFillPatternStyle { get; }

		string CachedShape { get; }

		int CachedFillPatternColor { get; }

		string CachedExtendedProperties { get; }

		void GetCachedExtendedPropertiesAsArray([Out] IntPtr propertyArray, out int propertyCount);
	}

	[InterfaceType(1)]
	[Guid("2233BE0B-AFB7-448B-9FDA-3B378AA5EAE1")]
	[ComImport]
	public interface ISynchronizedInputPattern
	{
		void StartListening(SynchronizedInputType inputType);

		void Cancel();
	}

	[Guid("0B964EB3-EF2E-4464-9C79-61D61737A27E")]
	[InterfaceType(1)]
	[ComImport]
	public interface ITableItemPattern
	{
		IElementArray GetCurrentRowHeaderItems();

		IElementArray GetCurrentColumnHeaderItems();

		IElementArray GetCachedRowHeaderItems();

		IElementArray GetCachedColumnHeaderItems();
	}

	[InterfaceType(1)]
	[Guid("620E691C-EA96-4710-A850-754B24CE2417")]
	[ComImport]
	public interface ITablePattern
	{
		IElementArray GetCurrentRowHeaders();

		IElementArray GetCurrentColumnHeaders();

		RowOrColumnMajor RowOrColumnMajor { get; }

		IElementArray GetCachedRowHeaders();

		IElementArray GetCachedColumnHeaders();

		RowOrColumnMajor CachedRowOrColumnMajor { get; }
	}

	[InterfaceType(1)]
	[Guid("6552B038-AE05-40C8-ABFD-AA08352AAB86")]
	[ComImport]
	public interface ITextChildPattern
	{
		IElement TextContainer { get; }

		ITextRange TextRange { get; }
	}

	[InterfaceType(1)]
	[Guid("17E21576-996C-4870-99D9-BFF323380C06")]
	[ComImport]
	public interface ITextEditPattern :ITextPattern
	{
		new ITextRange RangeFromPoint(Point pt);

		new ITextRange RangeFromChild(IElement child);

		new ITextRangeArray GetSelection();

		new ITextRangeArray GetVisibleRanges();

		new ITextRange DocumentRange { get; }

		new SupportedTextSelection SupportedTextSelection { get; }

		ITextRange GetActiveComposition();

		ITextRange GetConversionTarget();
	}

	[Guid("92FAA680-E704-4156-931A-E32D5BB38F3F")]
	[InterfaceType(1)]
	[ComImport]
	public interface ITextEditTextChangedEventHandler
	{
		void HandleTextEditTextChangedEvent(IElement sender, TextEditChangeType TextEditChangeType, string[] eventStrings);
	}

	[InterfaceType(1)]
	[Guid("32EBA289-3583-42C9-9C59-3B6D9A1E9B6A")]
	[ComImport]
	public interface ITextPattern
	{
		ITextRange RangeFromPoint(Point pt);

		ITextRange RangeFromChild(IElement child);

		ITextRangeArray GetSelection();

		ITextRangeArray GetVisibleRanges();

		ITextRange DocumentRange { get; }

		SupportedTextSelection SupportedTextSelection { get; }
	}

	[InterfaceType(1)]
	[Guid("506A921A-FCC9-409F-B23B-37EB74106872")]
	[ComImport]
	public interface ITextPattern2 :ITextPattern
	{
		new ITextRange RangeFromPoint(Point pt);

		new ITextRange RangeFromChild(IElement child);

		new ITextRangeArray GetSelection();

		new ITextRangeArray GetVisibleRanges();

		new ITextRange DocumentRange { get; }

		new SupportedTextSelection SupportedTextSelection { get; }

		ITextRange RangeFromAnnotation(IElement annotation);

		ITextRange GetCaretRange([MarshalAs(UnmanagedType.Bool)] out bool isActive);
	}

	[InterfaceType(1)]
	[Guid("A543CC6A-F4AE-494B-8239-C814481187A8")]
	[ComImport]
	public interface ITextRange
	{
		ITextRange Clone();

		[return: MarshalAs(UnmanagedType.Bool)] bool Compare(ITextRange range);

		int CompareEndpoints(TextPatternRangeEndpoint srcEndPoint, ITextRange range, TextPatternRangeEndpoint targetEndPoint);

		void ExpandToEnclosingUnit(TextUnit TextUnit);

		ITextRange FindAttribute(TextAttributeId attr, object val, [MarshalAs(UnmanagedType.Bool)] bool backward);

		ITextRange FindText(string text, [MarshalAs(UnmanagedType.Bool)] bool backward, [MarshalAs(UnmanagedType.Bool)] bool ignoreCase);

		object GetAttributeValue(TextAttributeId attr);

		double[] GetBoundingRectangles();

		IElement GetEnclosingElement();

		string GetText(int maxLength);

		int Move(TextUnit unit, int count);

		int MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count);

		void MoveEndpointByRange(TextPatternRangeEndpoint srcEndPoint, ITextRange range, TextPatternRangeEndpoint targetEndPoint);

		void Select();

		void AddToSelection();

		void RemoveFromSelection();

		void ScrollIntoView([MarshalAs(UnmanagedType.Bool)] bool alignToTop);

		IElementArray GetChildren();
	}

	[Guid("BB9B40E0-5E04-46BD-9BE0-4B601B9AFAD4")]
	[InterfaceType(1)]
	[ComImport]
	public interface ITextRange2 :ITextRange
	{
		new ITextRange Clone();

		[return: MarshalAs(UnmanagedType.Bool)] new bool Compare(ITextRange range);

		new int CompareEndpoints(TextPatternRangeEndpoint srcEndPoint, ITextRange range, TextPatternRangeEndpoint targetEndPoint);

		new void ExpandToEnclosingUnit(TextUnit TextUnit);

		new ITextRange FindAttribute(TextAttributeId attr, object val, [MarshalAs(UnmanagedType.Bool)] bool backward);

		new ITextRange FindText(string text, [MarshalAs(UnmanagedType.Bool)] bool backward, [MarshalAs(UnmanagedType.Bool)] bool ignoreCase);

		new object GetAttributeValue(TextAttributeId attr);

		new double[] GetBoundingRectangles();

		new IElement GetEnclosingElement();

		new string GetText(int maxLength);

		new int Move(TextUnit unit, int count);

		new int MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count);

		new void MoveEndpointByRange(TextPatternRangeEndpoint srcEndPoint, ITextRange range, TextPatternRangeEndpoint targetEndPoint);

		new void Select();

		new void AddToSelection();

		new void RemoveFromSelection();

		new void ScrollIntoView([MarshalAs(UnmanagedType.Bool)] bool alignToTop);

		new IElementArray GetChildren();

		void ShowContextMenu();
	}

	[InterfaceType(1)]
	[Guid("6A315D69-5512-4C2E-85F0-53FCE6DD4BC2")]
	[ComImport]
	public interface ITextRange3 :ITextRange2
	{
		new ITextRange Clone();

		[return: MarshalAs(UnmanagedType.Bool)] new bool Compare(ITextRange range);

		new int CompareEndpoints(TextPatternRangeEndpoint srcEndPoint, ITextRange range, TextPatternRangeEndpoint targetEndPoint);

		new void ExpandToEnclosingUnit(TextUnit TextUnit);

		new ITextRange FindAttribute(TextAttributeId attr, object val, [MarshalAs(UnmanagedType.Bool)] bool backward);

		new ITextRange FindText(string text, [MarshalAs(UnmanagedType.Bool)] bool backward, [MarshalAs(UnmanagedType.Bool)] bool ignoreCase);

		new object GetAttributeValue(TextAttributeId attr);

		new double[] GetBoundingRectangles();

		new IElement GetEnclosingElement();

		new string GetText(int maxLength);

		new int Move(TextUnit unit, int count);

		new int MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count);

		new void MoveEndpointByRange(TextPatternRangeEndpoint srcEndPoint, ITextRange range, TextPatternRangeEndpoint targetEndPoint);

		new void Select();

		new void AddToSelection();

		new void RemoveFromSelection();

		new void ScrollIntoView([MarshalAs(UnmanagedType.Bool)] bool alignToTop);

		new IElementArray GetChildren();

		new void ShowContextMenu();

		IElement GetEnclosingElementBuildCache(ICacheRequest cacheRequest);

		IElementArray GetChildrenBuildCache(ICacheRequest cacheRequest);

		object[] GetAttributeValues([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] TextAttributeId[] attributeIds, int attributeIdCount);
	}

	[InterfaceType(1)]
	[Guid("CE4AE76A-E717-4C98-81EA-47371D028EB6")]
	[ComImport]
	public interface ITextRangeArray
	{
		int Length { get; }

		ITextRange GetElement(int index);
	}

	[Guid("94CF8058-9B8D-4AB9-8BFD-4CD0A33C8C70")]
	[InterfaceType(1)]
	[ComImport]
	public interface ITogglePattern
	{
		void Toggle();

		ToggleState ToggleState { get; }

		ToggleState CachedToggleState { get; }
	}

	[InterfaceType(1)]
	[Guid("A9B55844-A55D-4EF0-926D-569C16FF89BB")]
	[ComImport]
	public interface ITransformPattern
	{
		void Move(double x, double y);

		void Resize(double width, double height);

		void Rotate(double degrees);

		bool CanMove { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CanResize { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CanRotate { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedCanMove { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedCanResize { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedCanRotate { [return: MarshalAs(UnmanagedType.Bool)] get; }
	}

	[Guid("6D74D017-6ECB-4381-B38B-3C17A48FF1C2")]
	[InterfaceType(1)]
	[ComImport]
	public interface ITransformPattern2 :ITransformPattern
	{
		new void Move(double x, double y);

		new void Resize(double width, double height);

		new void Rotate(double degrees);

		new bool CanMove { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CanResize { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CanRotate { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedCanMove { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedCanResize { [return: MarshalAs(UnmanagedType.Bool)] get; }

		new bool CachedCanRotate { [return: MarshalAs(UnmanagedType.Bool)] get; }

		void Zoom(double zoomValue);

		void ZoomByUnit(ZoomUnit ZoomUnit);

		bool CanZoom { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedCanZoom { [return: MarshalAs(UnmanagedType.Bool)] get; }

		double ZoomLevel { get; }

		double CachedZoomLevel { get; }

		double ZoomMinimum { get; }

		double CachedZoomMinimum { get; }

		double ZoomMaximum { get; }

		double CachedZoomMaximum { get; }
	}

	[Guid("4042C624-389C-4AFC-A630-9DF854A541FC")]
	[InterfaceType(1)]
	[ComImport]
	public interface ITreeWalker
	{
		IElement GetParentElement(IElement element);

		IElement GetFirstChildElement(IElement element);

		IElement GetLastChildElement(IElement element);

		IElement GetNextSiblingElement(IElement element);

		IElement GetPreviousSiblingElement(IElement element);

		IElement NormalizeElement(IElement element);

		IElement GetParentElementBuildCache(IElement element, ICacheRequest cacheRequest);

		IElement GetFirstChildElementBuildCache(IElement element, ICacheRequest cacheRequest);

		IElement GetLastChildElementBuildCache(IElement element, ICacheRequest cacheRequest);

		IElement GetNextSiblingElementBuildCache(IElement element, ICacheRequest cacheRequest);

		IElement GetPreviousSiblingElementBuildCache(IElement element, ICacheRequest cacheRequest);

		IElement NormalizeElementBuildCache(IElement element, ICacheRequest cacheRequest);

		ICondition condition { get; }
	}

	[Guid("A94CD8B1-0844-4CD6-9D2D-640537AB39E9")]
	[InterfaceType(1)]
	[ComImport]
	public interface IValuePattern
	{
		void SetValue(string val);

		string Value { get; }

		bool IsReadOnly { [return: MarshalAs(UnmanagedType.Bool)] get; }

		string CachedValue { get; }

		bool CachedIsReadOnly { [return: MarshalAs(UnmanagedType.Bool)] get; }
	}

	[InterfaceType(1)]
	[Guid("6BA3D7A6-04CF-4F11-8793-A8D1CDE9969F")]
	[ComImport]
	public interface IVirtualizedItemPattern
	{
		void Realize();
	}

	[Guid("0FAEF453-9208-43EF-BBB2-3B485177864F")]
	[InterfaceType(1)]
	[ComImport]
	public interface IWindowPattern
	{
		void Close();

		[return: MarshalAs(UnmanagedType.Bool)] bool WaitForInputIdle(int milliseconds);

		void SetWindowVisualState(WindowVisualState state);

		bool CanMaximize { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CanMinimize { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool IsModal { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool IsTopmost { [return: MarshalAs(UnmanagedType.Bool)] get; }

		WindowVisualState WindowVisualState { get; }

		WindowInteractionState WindowInteractionState { get; }

		bool CachedCanMaximize { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedCanMinimize { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedIsModal { [return: MarshalAs(UnmanagedType.Bool)] get; }

		bool CachedIsTopmost { [return: MarshalAs(UnmanagedType.Bool)] get; }

		WindowVisualState CachedWindowVisualState { get; }

		WindowInteractionState CachedWindowInteractionState { get; }
	}

	[Guid("D6DD68D1-86FD-4332-8666-9ABEDEA2D24C")]
	[InterfaceType(1)]
	[ComImport]
	public interface IRawElementProviderSimple
	{
		ProviderOptions ProviderOptions { get; }

		[return: MarshalAs(UnmanagedType.IUnknown)] object GetPatternProvider(PatternId patternId);

		object GetPropertyValue(PropertyId propertyId);

		IRawElementProviderSimple HostRawElementProvider { get; }
	}

	[Guid("618736E0-3C3D-11CF-810C-00AA00389B71")]
	[ComImport]
	public interface IAccessible
	{
		object accParent { [return: MarshalAs(UnmanagedType.IDispatch)] get; }

		int accChildCount { get; }

		[return: MarshalAs(UnmanagedType.IDispatch)] object get_accChild(object varChild);

		string get_accName(object varChild);

		string get_accValue(object varChild);

		string get_accDescription(object varChild);

		object get_accRole(object varChild);

		object get_accState(object varChild);

		string get_accHelp(object varChild);

		int get_accHelpTopic(out string pszHelpFile, object varChild);

		string get_accKeyboardShortcut(object varChild);

		object accFocus { get; }

		object accSelection { get; }

		string get_accDefaultAction(object varChild);

		void accSelect(int flagsSelect, object varChild);

		void accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, object varChild);

		object accNavigate(int navDir, object varStart);

		object accHitTest(int xLeft, int yTop);

		void accDoDefaultAction(object varChild);

		void set_accName(object varChild, string pszName);

		void set_accValue(object varChild, string pszValue);
	}

}
