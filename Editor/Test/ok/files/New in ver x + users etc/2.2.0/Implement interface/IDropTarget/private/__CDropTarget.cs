type __IDropTarget QueryInterface AddRef Release DragEnter DragOver DragLeave Drop
__IDropTarget+ __g_CDropTarget

if(!__g_CDropTarget.QueryInterface)
	__g_CDropTarget.QueryInterface=&CDropTarget_QueryInterface
	__g_CDropTarget.AddRef=&CDropTarget_AddRef
	__g_CDropTarget.Release=&CDropTarget_Release
	__g_CDropTarget.DragEnter=&CDropTarget_DragEnter
	__g_CDropTarget.DragOver=&CDropTarget_DragOver
	__g_CDropTarget.DragLeave=&CDropTarget_DragLeave
	__g_CDropTarget.Drop=&CDropTarget_Drop

vtbl=&__g_CDropTarget
