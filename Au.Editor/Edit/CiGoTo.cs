using System.Linq;
using System.Net;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.CSharp;

class CiGoTo
{
	struct _SourceLocation
	{
		public _SourceLocation(string file, int line, int column) {
			this.file = file; this.line = line; this.column = column;
		}
		public string file;
		public int line, column;
	}

	bool _canGoTo, _inSource;
	//if in source
	List<_SourceLocation> _sourceLocations;
	//if in metadata
	string _assembly, _docId, _typeName;

	/// <summary>
	/// true if can go to the symbol source. Then caller eg can add link to HTML.
	/// May return true even if can't go to. Then on link click nothing happens. Next time will return false.
	/// </summary>
	public bool CanGoTo => _canGoTo;

	CiGoTo(bool inSource) { _canGoTo = true; _inSource = inSource; }

	/// <summary>
	/// Gets info required to go to symbol source file/line/position or website.
	/// This function is fast. The slow or async code is in the <b>GoTo</b> functions.
	/// </summary>
	public CiGoTo(ISymbol sym, bool onlyIfInSource = false) {
		if (_inSource = sym.IsFromSource()) {
			_sourceLocations = new List<_SourceLocation>();
			foreach (var loc in sym.Locations) {
				if (!loc.IsVisibleSourceLocation()) continue;
				var v = loc.GetLineSpan();
				_sourceLocations.Add(new _SourceLocation(v.Path, v.StartLinePosition.Line, v.StartLinePosition.Character));
			}
			_canGoTo = _sourceLocations.Count > 0;
		} else if (!onlyIfInSource) {
			_assembly = sym.ContainingAssembly?.Name; if (_assembly == null) return;
			if (_assembly == "Au" || _assembly.Starts("Au.")) return;
			if (s_sources.All(o => o.data != null) && _FindSourceSite(download: false) < 0) return;

			//If wrong symbol, the site shows an error page.
			//Look how SourceBrowser (github) gets correct symbol and its hash. We don't use everything from there.
			//Methods:
			//	private HtmlElementInfo ProcessReference(Classification.Range range, ISymbol symbol, ReferenceKind kind, bool isLargeFile = false)
			//	private static string GetDocumentationCommentId(ISymbol symbol)
			//	private ISymbol GetSymbol(SyntaxNode node)
			if (sym is IMethodSymbol ims) sym = ims.ReducedFrom ?? sym; //extension method
			if (!sym.IsDefinition) sym = sym.OriginalDefinition; //generic
			_docId = sym.GetDocumentationCommentId()?.Replace("#ctor", "ctor");
			if (_docId == null) return; //operator?

			//get type name to resolve forwarded type later. For generic must be like "Namespace.Type`1".
			for (var ct = sym.ContainingType; ct != null; ct = ct.ContainingType) sym = ct;
			if (sym != null) {
				_typeName = sym.QualifiedName();
				if (sym is INamedTypeSymbol nts && nts.IsGenericType) _typeName += "`" + nts.TypeParameters.Length.ToString();
			}
			//if(sym != null) _typeName = sym.GetDocumentationCommentId()[2..]; //ok too. ToXString etc with suffix "<T>".

			_canGoTo = true;
		}
	}

	int _FindSourceSite(bool download) {
		int R = -1;
		regexp rx = null;
		for (int i = 0; i < s_sources.Length; i++) {
			if (download && s_sources[i].data == null) {
				try {
					using var client = new WebClient { CachePolicy = new System.Net.Cache.RequestCachePolicy() }; //get from cache if available and not too old
					s_sources[i].data = client.DownloadString(s_sources[i].site + "/assemblies.txt");
				}
				catch (WebException) { }
			}
			if (R >= 0) continue;
			rx ??= new regexp($@"(?m)^{_assembly};\d");
			if (rx.IsMatch(s_sources[i].data)) {
				R = i;
				if (!download) break;
			}
		}
		return R;
	}

	static readonly (string site, string data)[] s_sources = {
		("https://source.dot.net", null), //.NET Core/5
		("https://referencesource.microsoft.com", null), //.NET Framework 4.x (in source.dot.net many classes are missing)
		("http://source.roslyn.io", null) //Roslyn
	};

	string _GetLinkData() {
		if (!_canGoTo) return null;
		if (_inSource) {
			var b = new StringBuilder();
			foreach (var v in _sourceLocations) b.AppendFormat("|{0}?{1},{2}", v.file, v.line, v.column);
			return b.ToString();
		} else {
			return $"||{_assembly}|{_docId}|{_typeName}";
		}
	}

	/// <summary>
	/// Gets link data string for <see cref="LinkGoTo(string, Control)"/>. Returns null if unavailable.
	/// This function is fast. The slow or async code is in the <b>GoTo</b> function.
	/// </summary>
	public static string GetLinkData(ISymbol sym) => new CiGoTo(sym)._GetLinkData();

	/// <summary>
	/// Opens symbol source file/line/position or website. Used on link click.
	/// If need to open website, runs async task.
	/// </summary>
	/// <param name="linkData">String returned by <see cref="GetLinkData"/>.</param>
	public static void LinkGoTo(string linkData) {
		if (linkData == null) return;
		bool inSource = !linkData.Starts("||");
		var g = new CiGoTo(inSource);
		var a = linkData.Split('|');
		if (inSource) {
			g._sourceLocations = new List<_SourceLocation>();
			for (int i = 1; i < a.Length; i++) {
				var s = a[i];
				int line = s.LastIndexOf('?'), column = s.LastIndexOf(',');
				g._sourceLocations.Add(new _SourceLocation(s.Remove(line), s.ToInt(line + 1), s.ToInt(column + 1)));
			}
		} else {
			g._assembly = a[2];
			g._docId = a[3];
			if (!a[4].NE()) g._typeName = a[4];
		}
		g.GoTo();
	}

	/// <summary>
	/// Opens symbol source file/line/position or website.
	/// If need to open website, runs async task.
	/// </summary>
	public void GoTo() {
		if (!_canGoTo) return;
		if (_inSource) {
			if (_sourceLocations.Count == 1) {
				_GoTo(_sourceLocations[0]);
			} else {
				int i = popupMenu.showSimple(_sourceLocations.Select(v => v.file + ", line " + v.line.ToString()).ToArray());
				if (i > 0) _GoTo(_sourceLocations[i - 1]);
			}

			static void _GoTo(_SourceLocation v) => App.Model.OpenAndGoTo(v.file, v.line, v.column);
		} else {
			Task.Run(() => {
				if (_typeName != null) { _GetAssemblyNameOfForwardedType(); _typeName = null; }

				int i = _FindSourceSite(download: true);
				if (i < 0) return;

				Hash.MD5Context md5 = default;
				md5.Add(_docId);
				var hash = md5.Hash.ToString().Remove(16);

				run.itSafe(s_sources[i].site + $"/{_assembly}/a.html#{hash}");
			});
		}
	}

	/// <summary>
	/// If _typeName or its ancestor type is forwarded to another assembly, replaces _assembly with the name of that assembly.
	/// For example initially we get that String is in System.Runtime. But actually it is in System.Private.CoreLib, and its name must be passed to https://source.dot.net.
	/// Speed: usually < 10 ms.
	/// </summary>
	void _GetAssemblyNameOfForwardedType() {
		var path = folders.NetRuntimeBS + _assembly + ".dll";
		if (!(filesystem.exists(path).isFile || filesystem.exists(path = folders.NetRuntimeDesktopBS + _assembly + ".dll").isFile)) return;

		var alc = new System.Runtime.Loader.AssemblyLoadContext(null, true);
		try {
			var asm = alc.LoadFromAssemblyPath(path);
			var ft = asm.GetForwardedTypes()?.FirstOrDefault(ty => ty.FullName == _typeName);
			if (ft != null) _assembly = ft.Assembly.GetName().Name;
		}
		catch (Exception ex) { Debug_.Print(ex); }
		finally { alc.Unload(); }
	}

	public static void GoToSymbolFromPos(bool onCtrlClick = false) {
		var (sym, _, helpKind, token) = CiUtil.GetSymbolEtcFromPos(out _);
		if (sym != null) {
			//print.it(sym);
			var g = new CiGoTo(sym, onlyIfInSource: onCtrlClick);
			if (g.CanGoTo) g.GoTo();
		} else if (helpKind == CiUtil.HelpKind.String && token.IsKind(SyntaxKind.StringLiteralToken)) {
			var s = token.ValueText;
			if (s.Ends(".cs", true)) App.Model.OpenAndGoTo(s);
		}
	}
}
