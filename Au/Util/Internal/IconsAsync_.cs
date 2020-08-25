using Au.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
//using System.Linq;

namespace Au.Util
{
	//This class could be public, but probably nobody will use. Possibly will be rejected in the future.

	/// <summary>
	/// Gets file icons asynchronously.
	/// </summary>
	/// <remarks>
	/// Use to avoid waiting until all icons are extracted before displaying them in a UI (menu etc).
	/// Instead you show the UI without icons, and then asynchronously receive icons when they are extracted.
	/// At first call <see cref="Add"/> (for each file) or <see cref="AddRange"/>. Then call <see cref="GetAllAsync"/>.
	/// Create a callback function of type <see cref="Callback"/> and pass its delegate to <b>GetAllAsync</b>.
	/// </remarks>
	/// <seealso cref="AIconCache_"/>
	internal sealed class IconsAsync_ : IDisposable
	{
		//never mind:
		//	Could instead use Dictionary<string, List<object>>, to avoid extracting icon multiple times for the same path or even file type (difficult).
		//	But better is simpler code. Who wants max speed, in most cases can use a saved imagelist instead of getting multiple icons each time.

		List<Item> _files = new List<Item>();
		List<_AsyncWork> _works = new List<_AsyncWork>();

		/// <summary>
		/// Adds a file path to an internal collection.
		/// </summary>
		/// <param name="file">File path etc. See <see cref="AIcon.OfFile"/>.</param>
		/// <param name="obj">Something to pass to your callback function together with icon handle for this file.</param>
		public void Add(string file, object obj)
		{
			if(file.NE()) return;
			_files.Add(new Item(file, obj));
		}

		/// <summary>
		/// Adds multiple file paths to an internal collection.
		/// The same as calling <see cref="Add"/> multiple times.
		/// This function copies the list.
		/// </summary>
		public void AddRange(IEnumerable<Item> files)
		{
			if(files != null) _files.AddRange(files);
		}

		/// <summary>
		/// Gets the number of added items.
		/// <see cref="GetAllAsync"/> sets it = 0.
		/// </summary>
		public int Count => _files.Count;

		/// <summary>
		/// Starts getting icons of added files.
		/// </summary>
		/// <param name="callback">A callback function delegate.</param>
		/// <param name="iconSize">Icon width and height.</param>
		/// <param name="flags"></param>
		/// <param name="objCommon">Something to pass to callback functions.</param>
		/// <remarks>
		/// After this function returns, icons are asynchronously extracted with <see cref="AIcon.OfFile"/>, and callback called with icon handle (or default(IntPtr) if failed).
		/// The callback is called in this thread. This thread must have a message loop (eg Application.Run).
		/// If you'll need more icons, you can add more files and call this function again with the same <b>IconsAsync_</b> instance, even if getting old icons if still not finished.
		/// </remarks>
		public void GetAllAsync(Callback callback, int iconSize, IconGetFlags flags = 0, object objCommon = null)
		{
			if(_files.Count == 0) return;
			var work = new _AsyncWork();
			_works.Add(work);
			work.GetAllAsync(this, callback, iconSize, flags, objCommon);
			_files.Clear();
		}

		//We use List<_AsyncWork> to support getting additional icons after GetAllAsync was already called and possibly still executing.
		//Example:
		//	Client adds icons with AddX and calls GetAllAsync.
		//	But then client wants to get more icons with the same IconsAsync_ instance. Again calls AddX and GetAllAsync.
		//	For each GetAllAsync we add new _AsyncWork to the list and let it get the new icons.
		//	Using the same IconsAsync_ would be difficult because everything is executing async in multiple threads.
		class _AsyncWork
		{
			IconsAsync_ _host;
			Callback _callback;
			int _iconSize;
			IconGetFlags _iconFlags;
			object _objCommon;
			int _counter;
			volatile int _nPending;
			bool _canceled;

			internal void GetAllAsync(IconsAsync_ host, Callback callback, int iconSize, IconGetFlags flags, object objCommon)
			{
				Debug.Assert(_callback == null); //must be called once

				_host = host;
				_callback = callback;
				_iconSize = iconSize;
				_iconFlags = flags;
				_objCommon = objCommon;
				_counter = _host._files.Count;

				using(new EnsureWindowsFormsSynchronizationContext_()) {
					foreach(var v in _host._files) {
						if(!v.file.NE()) _GetIconAsync(new Result(v.file, v.obj));
					}
				}
			}

#if true
			void _GetIconAsync(Result state)
			{
				ThreadPoolSTA_.SubmitCallback(state, d => { //this code runs in a thread pool thread
					if(_canceled) {
						d.completionCallback = null;
						return;
					}
					//Thread.Sleep(10);
					var k = d.state as Result;
					k.icon = AIcon.OfFile(k.file, _iconSize, _iconFlags);

					//Prevent overflowing the message queue and the number of icon handles.
					//Because bad things start when eg toolbar icon count is more than 3000 and they are extracted faster than consumed.
					//But don't make the threshold too low, because then may need to wait unnecessarily, and it makes slower.
					if(Interlocked.Increment(ref _nPending) >= 900) {
						//AOutput.Write(_nPending);
						//var perf = APerf.Create();
						Thread.Sleep(10);
						//while(_nPending >= 900) Thread.Sleep(10);
						//perf.NW();
					}
				}, o => { //this code runs in the caller's thread
					Interlocked.Decrement(ref _nPending);

					//AOutput.Write("2");

					var k = o as Result;
					if(_canceled) {
						k.icon.Dispose();
					} else {
						_callback(k, _objCommon, --_counter); //even if hIcon == default, it can be useful
						if(_counter == 0) {
							_host._works.Remove(this);
							_host = null;
							Debug.Assert(_nPending == 0);
						}
					}
				});
			}
#elif false
				async void _GetIconAsync(AsyncResult state)
				{
					var task = Task.Factory.StartNew(() =>
					{
						if(_canceled) return;
						//Thread.Sleep(500);
						var k = state;
						k.hIcon = GetFileIconHandle(k.file, _iconSize, _iconFlags);
					}, CancellationToken.None, TaskCreationOptions.None, _staTaskScheduler);
					await task;

					//async continuation

					if(_canceled) {
						Api.DestroyIcon(state.hIcon);
					} else {
						_callback(state, _objCommon); //even if hi == default, it can be useful
						if(--_counter == 0) {
							if(_onFinished != null) _onFinished(_objCommon);
							_host._works.Remove(this);
							_host = null;
						}
					}
				}

				static readonly System.Threading.Tasks.Schedulers.StaTaskScheduler _staTaskScheduler = new System.Threading.Tasks.Schedulers.StaTaskScheduler(4); //tested: without StaTaskScheduler would be 4 threads. With 3 the UI thread is slightly faster.
#else
				async void _GetIconAsync(AsyncResult state)
				{
					var task = Task.Run(() =>
					{
						if(_canceled) return;
						//Thread.Sleep(500);
						var k = state;
						k.hIcon = GetFileIconHandle(k.file, _iconSize, _iconFlags);
					});
					await task;

					//async continuation

					if(_canceled) {
						Api.DestroyIcon(state.hIcon);
					} else {
						_callback(state, _objCommon); //even if hi == default, it can be useful
						if(--_counter == 0) {
							if(_onFinished != null) _onFinished(_objCommon);
							_host._works.Remove(this);
							_host = null;
						}
					}
				}
#endif

			internal void Cancel()
			{
				_canceled = true;
			}
		}

		/// <summary>
		/// Clears the internal collection of file paths added with Add().
		/// </summary>
		public void Clear()
		{
			_files.Clear();
		}

		/// <summary>
		/// Stops getting icons and calling callback functions.
		/// </summary>
		public void Cancel()
		{
			//AOutput.Write(_works.Count);
			if(_works.Count == 0) return;
			foreach(var work in _works) work.Cancel();
			_works.Clear();
		}

		/// <summary>
		/// Calls <see cref="Cancel"/>.
		/// </summary>
		public void Dispose()
		{
			Cancel();
			GC.SuppressFinalize(this);
		}

		///
		~IconsAsync_() { Cancel(); }

		/// <summary>
		/// For <see cref="AddRange(IEnumerable{Item})"/>. 
		/// </summary>
		public struct Item
		{
#pragma warning disable 1591 //XML doc
			public string file;
			public object obj;

			public Item(string file, object obj) { this.file = file; this.obj = obj; }
#pragma warning restore 1591 //XML doc
		}

		/// <summary>
		/// For <see cref="Callback"/>. 
		/// </summary>
		public class Result
		{
			/// <summary>file passed to <see cref="Add(string, object)"/>.</summary>
			public string file;

			/// <summary>obj passed to <see cref="Add(string, object)"/>.</summary>
			public object obj;

			/// <summary>Icon handle. Will need to dispose. Can be empty.</summary>
			public AIcon icon;

			/// <summary>Icon converted to Image object, if used IconGetFlags.NeedImage and the thread pool decided to convert handle to Image. You should call Dispose() when finished using it. Can be null.</summary>
			//public Image image;

			public Result(string file, object obj) { this.file = file; this.obj = obj; }
		}

		/// <summary>
		/// For <see cref="GetAllAsync"/>. 
		/// </summary>
		/// <param name="result">Contains icon, as well as the input parameters.</param>
		/// <param name="objCommon">objCommon passed to <see cref="GetAllAsync"/>.</param>
		/// <param name="nLeft">How many icons is still to get. Eg 0 if this is the last icon.</param>
		public delegate void Callback(Result result, object objCommon, int nLeft);
	}
}
