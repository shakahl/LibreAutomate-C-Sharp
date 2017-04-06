No dock:
	no resources:  49023  tv=152668  86936  (288627)
	resources:  94158  tv=150342  86201  (330702)


Dock, resources:
	default:  83009  160690  tv=320980  134323  (699004)
	no hook:  137936  128519  157399  157965  (581821)
	no hook, ngen:  100738  74324  152081  112239  (439384)
	MEMORY:  13MB, GC same
	native TV:  137442  127617  100131  134331  (499522)
	native TV, ngen:  100539  78158  96315  109603  (384617)


Forms empty window:
	speed:  4629  16824  23172  (44627)
	MEMORY:  2.9MB


WPF empty window:
	speed:  47176  10221  99907  (157305)
	MEMORY:  11.9MB

WPF + AvalonDock, no data, no menu/toolbar:
	speed:  47055  169792  116470  (333317)
	ngen:  45976  137071  95367  (278415)
	+wfHost, Sci:  56012  141950  178756  (376719)
	+wfHost, Sci, TV:  57218  137934  276210  (471364), same ngened
	+items (full):  64621  180820  609  21631  tv=165957  321562  (755203)
	ngen:  54707  149397  408  18623  tv=149247  302693  (675077)
	MEMORY:  45MB, GC 37MB




















TODO if using WPF:
 Test WPF TreeView.
 Test WPF toolbar+menubar.
 Google for a WPF Scintilla wrapper or another code editor, maybe AvalonEdit (eg used in SharpDevelop).
   See:
     http://stackoverflow.com/questions/8577627/scintillanet-vs-avalonedit-for-providing-scripting-interface-for-a-wpf-applicati
     https://www.codeproject.com/articles/1025495/best-text-editor-component-for-net-wpf-developers
     https://www.codeproject.com/articles/161871/fast-colored-textbox-for-syntax-highlighting
 Try to use native controls instead of wfHost+.NET wrapper control.
