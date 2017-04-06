#if false
		void _LoadCollection()
		{
			var createTvHandle = (Wnd)_tvPane;

			var p1 = new Perf.Inst(true);

			//_tv.Show(false);

			//Api.SetParent(_tv, Wnd.Misc.SpecHwnd.Message);
			//_tv.SetWindowLong(Api.GWL_HWNDPARENT, 0);
			//Api.SetWindowsHookEx(Api.WH_CALLWNDPROC, _hookProc, Zero, Api.GetCurrentThreadId());
			//Api.SetWindowsHookEx(Api.WH_CALLWNDPROCRET, _hookProc, Zero, Api.GetCurrentThreadId());

			var x = new CsvTable() { Separator = '|' };
			_collectionDir = Path.GetDirectoryName(_collectionCsvFile) + "\\";
			x.FromFile(_collectionCsvFile);
			//var dat = File.ReadAllText(_collectionCsvFile); x.FromString(dat + dat+dat+dat);
			//Perf.Next();
			_items = new List<CatItem>(x.RowCount + 20);
			_items.Add(new CatItem(0, "<file>", 0, null, null));
			int prevId = 0, selectId = 0;
			var stack = new Stack<int>(); stack.Push(0);
			//Print(x.RowCount);
			//Perf.Next();
			p1.Next();
			//var u = new Perf.Inst(); u.Incremental = true;
			for(int i = 0, n = x.RowCount; i < n; i++) {
				//var p = new Perf.Inst(true);
				n = 50; //TODO: remove
				var s = x[i, 1]; uint flags = 0;
				if(!Empty(s)) {
					flags = (uint)s.ToInt32_(0, out int numLen);
					if(s.Length > numLen) {
						switch(s[numLen]) {
						case '>':
							stack.Push(prevId);
							break;
						case '<':
							int k = s.ToInt32_(numLen + 1);
							do stack.Pop(); while(--k > 0);
							break;
						}
					}
				}

				int imageIndex = 0;
				bool isFolder = (flags & 1) != 0;
				if(isFolder) imageIndex = 1;

				int id = _items.Count, idParent = stack.Peek();
				var item = new CatItem(idParent, x[i, 0], flags, x[i, 2], x[i, 3]);
				_items.Add(item);
				//p.Next();
				item.Htvi = _tvPane.TvAdd(_items[idParent].Htvi, id, imageIndex);
				//p.Next();
				//if(p.TimeTotal >= 100)
				//	p.Write();

				prevId = id;
				if(selectId == 0 && !isFolder) selectId = id;
			}
			//Perf.Next();
			p1.Next();
			_tvPane.TvSend(_Api.TVM_SETIMAGELIST, _Api.TVSIL_NORMAL, EResources.ImageList_Files.Handle);
			//_tv.Send(_Api.TVM_SELECTITEM, _Api.TVGN_CARET, _items[selectId].Htvi);

			//Speed for ~13000 items in ~30 root folders: 40 ms.

			//Perf.NW();
			//p1.NW();
		}
#endif

#if true
#else
		public class CatFilesPane :Panel
		{
			//idea: when file clicked, open it and show CatMenu of its functions (if > 1).

			Wnd _w;
			Native.WNDPROC _tvWndproc;

			public CatFilesPane()
			{
				this.Text = this.Name = "Files";
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				return;

				uint style = Native.WS_CHILD | Native.WS_VISIBLE | _Api.TVS_INFOTIP | _Api.TVS_FULLROWSELECT | _Api.TVS_SHOWSELALWAYS;
				if(true) style |= _Api.TVS_HASBUTTONS | _Api.TVS_HASLINES | _Api.TVS_LINESATROOT | _Api.TVS_EDITLABELS;
				//else style |= _Api.TVS_SINGLEEXPAND | _Api.TVS_TRACKSELECT;
				_w = Wnd.Misc.CreateWindow(0, "SysTreeView32", "Files", style, 0, 0, 100, 100, (Wnd)Handle, 2202);
				_tvWndproc = (Native.WNDPROC)Marshal.GetDelegateForFunctionPointer(_w.GetWindowLong(Native.GWL_WNDPROC), typeof(Native.WNDPROC));

				base.OnHandleCreated(e);
			}

			/// <summary>
			/// Creates native treeview control and returns its handle.
			/// </summary>
			/// <returns></returns>
			public Wnd Init()
			{
				//OleInitialize(Zero);
				return _w;
			}

			protected override void OnClientSizeChanged(EventArgs e)
			{
				//Print("tree OnClientSizeChanged");
				if(!_w.Is0) {
					var z = this.ClientSize;
					_w.ResizeLL(z.Width, z.Height);
				}
				base.OnClientSizeChanged(e);
			}

			protected override unsafe void WndProc(ref Message m)
			{
				switch((uint)m.Msg) {
				//case Api.WM_SIZE:
				//	Print("tree WM_SIZE");
				//	break;
				case Api.WM_NOTIFY:
					if(!_w.Is0) {
						_Api.NMHDR* nh = (_Api.NMHDR*)m.LParam;
						//PrintList("notify", nh->hwndFrom, nh->code);
					}
					return;
				}
				base.WndProc(ref m);
			}

			internal LPARAM TvSend(uint message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM))
			{
				if(_w.Is0) { var t = (Wnd)Handle; } //create handles of this and _w
				return _w.Send(message, wParam, lParam);
				//return _tvWndproc(_w, message, wParam, lParam);
			}

			internal unsafe IntPtr TvAdd(IntPtr hparent, LPARAM param, int image)
			{
				var x = new _Api.TVINSERTSTRUCT();
				x.hParent = hparent;
				x.hInsertAfter = (IntPtr)_Api.TVI_LAST;
				x.item.mask = _Api.TVIF_TEXT | _Api.TVIF_PARAM | _Api.TVIF_IMAGE | _Api.TVIF_SELECTEDIMAGE;
				x.item.lParam = param;
				x.item.iImage = x.item.iSelectedImage = image;
				x.item.pszText = (IntPtr)_Api.LPSTR_TEXTCALLBACK;
				//return _tv.Send(_Api.TVM_INSERTITEM, 0, &x);
				return _tvWndproc(_w, _Api.TVM_INSERTITEM, 0, &x);
			}

			protected override void OnGotFocus(EventArgs e) { _w.FocusLL(); }
		}
#endif

	public class CatItem
	{
		public CatItem(int idParent, string name, uint flags, string guid, string etc)
		{
			IdParent = idParent; Name = name; _flags = flags; Guid = guid; _etc = etc;
		}

		public string Name { get; private set; }
		public string Guid { get; private set; }
		internal IntPtr Htvi { get; set; }
		string _etc;
		uint _flags;
		public int IdParent { get; private set; }

		public bool IsFolder { get { return (_flags & 7) == 1; } }
		public bool IsExternal { get { return (_flags & 7) == 2; } }
		public bool IsDisabled { get { return (_flags & 8) != 0; } }
		//public CatItem Parent { get { return } }

		public string FilePath
		{
			get
			{
				if(IsExternal) return _etc;
				return Name; //TODO
							 //var s = Node.FullPath;
							 //if(IsFolder) return _collectionDir + s;
							 //return _collectionDir + s + ".cs";
			}
		}
	}
