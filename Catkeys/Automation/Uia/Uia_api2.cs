using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Drawing;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace UIA
{
	public enum AutomationElementMode
	{
		AutomationElementMode_None,
		AutomationElementMode_Full
	}

	public enum DockPosition
	{
		Top,
		Left,
		Bottom,
		Right,
		Fill,
		None,
	}

	public enum ExpandCollapseState
	{
		Collapsed,
		Expanded,
		PartiallyExpanded,
		LeafNode,
	}

	public struct ExtendedProperty
	{
		[MarshalAs(UnmanagedType.BStr)] public string PropertyName;
		[MarshalAs(UnmanagedType.BStr)] public string PropertyValue;
	}

	public enum LiveSetting
	{
		Off,
		Polite,
		Assertive,
	}

	public enum NavigateDirection
	{
		Parent,
		NextSibling,
		PreviousSibling,
		FirstChild,
		LastChild,
	}

	public enum NotificationKind
	{
		ItemAdded,
		ItemRemoved,
		ActionCompleted,
		ActionAborted,
		Other,
	}

	public enum NotificationProcessing
	{
		ImportantAll,
		ImportantMostRecent,
		All,
		MostRecent,
		CurrentThenMostRecent,
	}

	public enum OrientationType
	{
		None,
		Horizontal,
		Vertical,
	}

	[Flags]
	public enum PropertyConditionFlags
	{
		None,
		IgnoreCase
	}

	[Flags]
	public enum ProviderOptions
	{
		ClientSideProvider = 1,
		ServerSideProvider = 2,
		NonClientAreaProvider = 4,
		OverrideProvider = 8,
		ProviderOwnsSetFocus = 16,
		UseComThreading = 32,
		RefuseNonClientSupport = 64,
		HasNativeIAccessible = 128,
		UseClientCoordinates = 256,
	}

	public enum RowOrColumnMajor
	{
		RowMajor,
		ColumnMajor,
		Indeterminate,
	}

	public enum ScrollAmount
	{
		LargeDecrement,
		SmallDecrement,
		NoAmount,
		LargeIncrement,
		SmallIncrement,
	}

	public enum StructureChangeType
	{
		ChildAdded,
		ChildRemoved,
		ChildrenInvalidated,
		ChildrenBulkAdded,
		ChildrenBulkRemoved,
		ChildrenReordered,
	}

	public enum SupportedTextSelection
	{
		None,
		Single,
		Multiple,
	}

	[Flags]
	public enum SynchronizedInputType
	{
		KeyUp = 1,
		KeyDown = 2,
		LeftMouseUp = 4,
		LeftMouseDown = 8,
		RightMouseUp = 16,
		RightMouseDown = 32,
	}

	public enum TextEditChangeType
	{
		None,
		AutoCorrect,
		Composition,
		CompositionFinalized,
		AutoComplete,
	}

	public enum TextPatternRangeEndpoint
	{
		Start,
		End,
	}

	public enum TextUnit
	{
		Character,
		Format,
		Word,
		Line,
		Paragraph,
		Page,
		Document,
	}

	public enum ToggleState
	{
		Off,
		On,
		Indeterminate,
	}

	[Flags]
	public enum TreeScope
	{
		Element = 1,
		Children = 2,
		Descendants = 4,
		Parent = 8,
		Ancestors = 16,
		Subtree = 7
	}

	[Flags]
	public enum TreeTraversalOptions
	{
		Default,
		PostOrder,
		LastToFirstOrder,
	}

	public struct UiaChangeInfo
	{
		public int uiaId;
		[MarshalAs(UnmanagedType.Struct)] public object payload;
		[MarshalAs(UnmanagedType.Struct)] public object extraInfo;
	}

	public enum AnnotationType
	{
		Unknown = 60000,
		SpellingError = 60001,
		GrammarError = 60002,
		Comment = 60003,
		FormulaError = 60004,
		TrackChanges = 60005,
		Header = 60006,
		Footer = 60007,
		Highlighted = 60008,
		Endnote = 60009,
		Footnote = 60010,
		InsertionChange = 60011,
		DeletionChange = 60012,
		MoveChange = 60013,
		FormatChange = 60014,
		UnsyncedChange = 60015,
		EditingLockedChange = 60016,
		ExternalChange = 60017,
		ConflictingChange = 60018,
		Author = 60019,
		AdvancedProofingIssue = 60020,
		DataValidationError = 60021,
		CircularReferenceError = 60022,
		Mathematics = 60023,
	}

	public enum ChangeId
	{
		Summary = 90000,
	}

	public enum TypeId
	{
		Button = 50000,
		Calendar = 50001,
		CheckBox = 50002,
		ComboBox = 50003,
		Edit = 50004,
		Hyperlink = 50005,
		Image = 50006,
		ListItem = 50007,
		List = 50008,
		Menu = 50009,
		MenuBar = 50010,
		MenuItem = 50011,
		ProgressBar = 50012,
		RadioButton = 50013,
		ScrollBar = 50014,
		Slider = 50015,
		Spinner = 50016,
		StatusBar = 50017,
		Tab = 50018,
		TabItem = 50019,
		Text = 50020,
		ToolBar = 50021,
		ToolTip = 50022,
		Tree = 50023,
		TreeItem = 50024,
		Custom = 50025,
		Group = 50026,
		Thumb = 50027,
		DataGrid = 50028,
		DataItem = 50029,
		Document = 50030,
		SplitButton = 50031,
		Window = 50032,
		Pane = 50033,
		Header = 50034,
		HeaderItem = 50035,
		Table = 50036,
		TitleBar = 50037,
		Separator = 50038,
		SemanticZoom = 50039,
		AppBar = 50040,
	}

	public enum EventId
	{
		ToolTipOpened = 20000,
		ToolTipClosed = 20001,
		StructureChanged = 20002,
		MenuOpened = 20003,
		AutomationPropertyChanged = 20004,
		AutomationFocusChanged = 20005,
		AsyncContentLoaded = 20006,
		MenuClosed = 20007,
		LayoutInvalidated = 20008,
		Invoke_Invoked = 20009,
		SelectionItem_ElementAddedToSelection = 20010,
		SelectionItem_ElementRemovedFromSelection = 20011,
		SelectionItem_ElementSelected = 20012,
		Selection_Invalidated = 20013,
		Text_TextSelectionChanged = 20014,
		Text_TextChanged = 20015,
		Window_WindowOpened = 20016,
		Window_WindowClosed = 20017,
		MenuModeStart = 20018,
		MenuModeEnd = 20019,
		InputReachedTarget = 20020,
		InputReachedOtherElement = 20021,
		InputDiscarded = 20022,
		SystemAlert = 20023,
		LiveRegionChanged = 20024,
		HostedFragmentRootsInvalidated = 20025,
		Drag_DragStart = 20026,
		Drag_DragCancel = 20027,
		Drag_DragComplete = 20028,
		DropTarget_DragEnter = 20029,
		DropTarget_DragLeave = 20030,
		DropTarget_Dropped = 20031,
		TextEdit_TextChanged = 20032,
		TextEdit_ConversionTargetChanged = 20033,
		Changes = 20034,
		Notification = 20035,
	}

	public enum LandmarkTypeId
	{
		Custom = 80000,
		Form = 80001,
		Main = 80002,
		Navigation = 80003,
		Search = 80004,
	}

	public enum MetadataId
	{
		SayAsInterpretAs = 100000,
	}

	public enum PatternId
	{
		Invoke = 10000,
		Selection = 10001,
		Value = 10002,
		RangeValue = 10003,
		Scroll = 10004,
		ExpandCollapse = 10005,
		Grid = 10006,
		GridItem = 10007,
		MultipleView = 10008,
		Window = 10009,
		SelectionItem = 10010,
		Dock = 10011,
		Table = 10012,
		TableItem = 10013,
		Text = 10014,
		Toggle = 10015,
		Transform = 10016,
		ScrollItem = 10017,
		LegacyIAccessible = 10018,
		ItemContainer = 10019,
		VirtualizedItem = 10020,
		SynchronizedInput = 10021,
		ObjectModel = 10022,
		Annotation = 10023,
		Text2 = 10024,
		Styles = 10025,
		Spreadsheet = 10026,
		SpreadsheetItem = 10027,
		Transform2 = 10028,
		TextChild = 10029,
		Drag = 10030,
		DropTarget = 10031,
		TextEdit = 10032,
		CustomNavigation = 10033,
		Selection2 = 10034,
	}

	public enum PropertyId
	{
		RuntimeId = 30000,
		BoundingRectangle = 30001,
		ProcessId = 30002,
		ControlType = 30003,
		LocalizedControlType = 30004,
		Name = 30005,
		AcceleratorKey = 30006,
		AccessKey = 30007,
		HasKeyboardFocus = 30008,
		IsKeyboardFocusable = 30009,
		IsEnabled = 30010,
		AutomationId = 30011,
		ClassName = 30012,
		HelpText = 30013,
		ClickablePoint = 30014,
		Culture = 30015,
		IsControlElement = 30016,
		IsContentElement = 30017,
		LabeledBy = 30018,
		IsPassword = 30019,
		NativeWindowHandle = 30020,
		ItemType = 30021,
		IsOffscreen = 30022,
		Orientation = 30023,
		FrameworkId = 30024,
		IsRequiredForForm = 30025,
		ItemStatus = 30026,
		IsDockPatternAvailable = 30027,
		IsExpandCollapsePatternAvailable = 30028,
		IsGridItemPatternAvailable = 30029,
		IsGridPatternAvailable = 30030,
		IsInvokePatternAvailable = 30031,
		IsMultipleViewPatternAvailable = 30032,
		IsRangeValuePatternAvailable = 30033,
		IsScrollPatternAvailable = 30034,
		IsScrollItemPatternAvailable = 30035,
		IsSelectionItemPatternAvailable = 30036,
		IsSelectionPatternAvailable = 30037,
		IsTablePatternAvailable = 30038,
		IsTableItemPatternAvailable = 30039,
		IsTextPatternAvailable = 30040,
		IsTogglePatternAvailable = 30041,
		IsTransformPatternAvailable = 30042,
		IsValuePatternAvailable = 30043,
		IsWindowPatternAvailable = 30044,
		ValueValue = 30045,
		ValueIsReadOnly = 30046,
		RangeValueValue = 30047,
		RangeValueIsReadOnly = 30048,
		RangeValueMinimum = 30049,
		RangeValueMaximum = 30050,
		RangeValueLargeChange = 30051,
		RangeValueSmallChange = 30052,
		ScrollHorizontalScrollPercent = 30053,
		ScrollHorizontalViewSize = 30054,
		ScrollVerticalScrollPercent = 30055,
		ScrollVerticalViewSize = 30056,
		ScrollHorizontallyScrollable = 30057,
		ScrollVerticallyScrollable = 30058,
		SelectionSelection = 30059,
		SelectionCanSelectMultiple = 30060,
		SelectionIsSelectionRequired = 30061,
		GridRowCount = 30062,
		GridColumnCount = 30063,
		GridItemRow = 30064,
		GridItemColumn = 30065,
		GridItemRowSpan = 30066,
		GridItemColumnSpan = 30067,
		GridItemContainingGrid = 30068,
		DockDockPosition = 30069,
		ExpandCollapseExpandCollapseState = 30070,
		MultipleViewCurrentView = 30071,
		MultipleViewSupportedViews = 30072,
		WindowCanMaximize = 30073,
		WindowCanMinimize = 30074,
		WindowWindowVisualState = 30075,
		WindowWindowInteractionState = 30076,
		WindowIsModal = 30077,
		WindowIsTopmost = 30078,
		SelectionItemIsSelected = 30079,
		SelectionItemSelectionContainer = 30080,
		TableRowHeaders = 30081,
		TableColumnHeaders = 30082,
		TableRowOrColumnMajor = 30083,
		TableItemRowHeaderItems = 30084,
		TableItemColumnHeaderItems = 30085,
		ToggleToggleState = 30086,
		TransformCanMove = 30087,
		TransformCanResize = 30088,
		TransformCanRotate = 30089,
		IsLegacyIAccessiblePatternAvailable = 30090,
		LegacyIAccessibleChildId = 30091,
		LegacyIAccessibleName = 30092,
		LegacyIAccessibleValue = 30093,
		LegacyIAccessibleDescription = 30094,
		LegacyIAccessibleRole = 30095,
		LegacyIAccessibleState = 30096,
		LegacyIAccessibleHelp = 30097,
		LegacyIAccessibleKeyboardShortcut = 30098,
		LegacyIAccessibleSelection = 30099,
		LegacyIAccessibleDefaultAction = 30100,
		AriaRole = 30101,
		AriaProperties = 30102,
		IsDataValidForForm = 30103,
		ControllerFor = 30104,
		DescribedBy = 30105,
		FlowsTo = 30106,
		ProviderDescription = 30107,
		IsItemContainerPatternAvailable = 30108,
		IsVirtualizedItemPatternAvailable = 30109,
		IsSynchronizedInputPatternAvailable = 30110,
		OptimizeForVisualContent = 30111,
		IsObjectModelPatternAvailable = 30112,
		AnnotationAnnotationTypeId = 30113,
		AnnotationAnnotationTypeName = 30114,
		AnnotationAuthor = 30115,
		AnnotationDateTime = 30116,
		AnnotationTarget = 30117,
		IsAnnotationPatternAvailable = 30118,
		IsTextPattern2Available = 30119,
		StylesStyleId = 30120,
		StylesStyleName = 30121,
		StylesFillColor = 30122,
		StylesFillPatternStyle = 30123,
		StylesShape = 30124,
		StylesFillPatternColor = 30125,
		StylesExtendedProperties = 30126,
		IsStylesPatternAvailable = 30127,
		IsSpreadsheetPatternAvailable = 30128,
		SpreadsheetItemFormula = 30129,
		SpreadsheetItemAnnotationObjects = 30130,
		SpreadsheetItemAnnotationTypes = 30131,
		IsSpreadsheetItemPatternAvailable = 30132,
		Transform2CanZoom = 30133,
		IsTransformPattern2Available = 30134,
		LiveSetting = 30135,
		IsTextChildPatternAvailable = 30136,
		IsDragPatternAvailable = 30137,
		DragIsGrabbed = 30138,
		DragDropEffect = 30139,
		DragDropEffects = 30140,
		IsDropTargetPatternAvailable = 30141,
		DropTargetDropTargetEffect = 30142,
		DropTargetDropTargetEffects = 30143,
		DragGrabbedItems = 30144,
		Transform2ZoomLevel = 30145,
		Transform2ZoomMinimum = 30146,
		Transform2ZoomMaximum = 30147,
		FlowsFrom = 30148,
		IsTextEditPatternAvailable = 30149,
		IsPeripheral = 30150,
		IsCustomNavigationPatternAvailable = 30151,
		PositionInSet = 30152,
		SizeOfSet = 30153,
		Level = 30154,
		AnnotationTypes = 30155,
		AnnotationObjects = 30156,
		LandmarkType = 30157,
		LocalizedLandmarkType = 30158,
		FullDescription = 30159,
		FillColor = 30160,
		OutlineColor = 30161,
		FillType = 30162,
		VisualEffects = 30163,
		OutlineThickness = 30164,
		CenterPoint = 30165,
		Rotation = 30166,
		Size = 30167,
		IsSelectionPattern2Available = 30168,
		Selection2FirstSelectedItem = 30169,
		Selection2LastSelectedItem = 30170,
		Selection2CurrentSelectedItem = 30171,
		Selection2ItemCount = 30172,
	}

	public enum StyleId
	{
		Custom = 70000,
		Heading1 = 70001,
		Heading2 = 70002,
		Heading3 = 70003,
		Heading4 = 70004,
		Heading5 = 70005,
		Heading6 = 70006,
		Heading7 = 70007,
		Heading8 = 70008,
		Heading9 = 70009,
		Title = 70010,
		Subtitle = 70011,
		Normal = 70012,
		Emphasis = 70013,
		Quote = 70014,
		BulletedList = 70015,
		NumberedList = 70016,
	}

	public enum TextAttributeId
	{
		AnimationStyle = 40000,
		BackgroundColor = 40001,
		BulletStyle = 40002,
		CapStyle = 40003,
		Culture = 40004,
		FontName = 40005,
		FontSize = 40006,
		FontWeight = 40007,
		ForegroundColor = 40008,
		HorizontalTextAlignment = 40009,
		IndentationFirstLine = 40010,
		IndentationLeading = 40011,
		IndentationTrailing = 40012,
		IsHidden = 40013,
		IsItalic = 40014,
		IsReadOnly = 40015,
		IsSubscript = 40016,
		IsSuperscript = 40017,
		MarginBottom = 40018,
		MarginLeading = 40019,
		MarginTop = 40020,
		MarginTrailing = 40021,
		OutlineStyles = 40022,
		OverlineColor = 40023,
		OverlineStyle = 40024,
		StrikethroughColor = 40025,
		StrikethroughStyle = 40026,
		Tabs = 40027,
		TextFlowDirections = 40028,
		UnderlineColor = 40029,
		UnderlineStyle = 40030,
		AnnotationTypes = 40031,
		AnnotationObjects = 40032,
		StyleName = 40033,
		StyleId = 40034,
		Link = 40035,
		IsActive = 40036,
		SelectionActiveEnd = 40037,
		CaretPosition = 40038,
		CaretBidiMode = 40039,
		LineSpacing = 40040,
		BeforeParagraphSpacing = 40041,
		AfterParagraphSpacing = 40042,
		SayAsInterpretAs = 40043,
	}

	public enum WindowInteractionState
	{
		Running,
		Closing,
		ReadyForUserInteraction,
		BlockedByModalWindow,
		NotResponding,
	}

	public enum WindowVisualState
	{
		Normal,
		Maximized,
		Minimized,
	}

	public enum ZoomUnit
	{
		NoAmount,
		LargeDecrement,
		SmallDecrement,
		LargeIncrement,
		SmallIncrement,
	}

}
