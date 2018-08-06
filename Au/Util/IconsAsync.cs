using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
//using System.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Gets file icons asynchronously.
	/// </summary>
	/// <remarks>
	/// Use to avoid waiting until all icons are extracted before displaying them in a UI (menu etc).
	/// Instead you show the UI without icons, and then asynchronously receive icons when they are extracted.
	/// At first call <see cref="Add"/> (for each file) or <see cref="AddRange"/>. Then call <see cref="GetAllAsync"/>.
	/// Create a callback function of type <see cref="Callback"/> and pass its delegate to <b>GetAllAsync</b>.
	/// </remarks>
	/// <seealso cref="Icons.ImageCache"/>
	public sealed class IconsAsync :IDisposable
	{
		//never mind:
		//	Could instead use Dictionary<string, List<object>>, to avoid extracting icon multiple times for the same path or even file type (difficult).
		//	But better is simpler code. Who wants max speed, in most cases can use a saved imagelist instead of getting multiple icons each time.

		List<Item> _files = new List<Item>();
		List<_AsyncWork> _works = new List<_AsyncWork>();

		/// <summary>
		/// Adds a file path to an internal collection.
		/// </summary>
		/// <param name="file">File path etc. See <see cref="Icons.GetFileIcon"/>.</param>
		/// <param name="obj">Something to pass to your callback function together with icon handle for this file.</param>
		public void Add(string file, object obj)
		{
			if(Empty(file)) return;
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
		/// <param name="iconSize">Icon width and height. Also can be enum <see cref="IconSize"/>, cast to int.</param>
		/// <param name="flags"><see cref="GIFlags"/></param>
		/// <param name="objCommon">Something to pass to callback functions.</param>
		/// <remarks>
		/// After this function returns, icons are asynchronously extracted with <see cref="Icons.GetFileIconHandle"/>, and callback called with icon handle (or default(IntPtr) if failed).
		/// The callback is called in this thread. This thread must have a message loop (eg Application.Run).
		/// If you'll need more icons, you can add more files and call this function again with the same <b>IconsAsync</b> instance, even if getting old icons if still not finished.
		/// </remarks>
		public void GetAllAsync(Callback callback, int iconSize, GIFlags flags = 0, object objCommon = null)
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
		//	But then client wants to get more icons with the same IconsAsync instance. Again calls AddX and GetAllAsync.
		//	For each GetAllAsync we add new _AsyncWork to the list and let it get the new icons.
		//	Using the same IconsAsync would be difficult because everything is executing async in multiple threads.
		class _AsyncWork
		{
			IconsAsync _host;
			Callback _callback;
			int _iconSize;
			GIFlags _iconFlags;
			object _objCommon;
			int _counter;
			volatile int _nPending;
			bool _canceled;

			internal void GetAllAsync(IconsAsync host, Callback callback, int iconSize, GIFlags flags, object objCommon)
			{
				Debug.Assert(_callback == null); //must be called once

				_host = host;
				_callback = callback;
				_iconSize = iconSize;
				_iconFlags = flags;
				_objCommon = objCommon;
				_counter = _host._files.Count;

				using(new Util.LibEnsureWindowsFormsSynchronizationContext()) {
					foreach(var v in _host._files) {
						if(!Empty(v.file)) _GetIconAsync(new Result(v.file, v.obj));
					}
				}
			}

#if true
			void _GetIconAsync(Result state)
			{
				Util.ThreadPoolSTA.SubmitCallback(state, d =>
				{ //this code runs in a thread pool thread
						if(_canceled) {
						d.completionCallback = null;
						return;
					}
						//Thread.Sleep(10);
						var k = d.state as Result;
					k.hIcon = Icons.GetFileIconHandle(k.file, _iconSize, _iconFlags);

						//var hi = GetFileIconHandle(k.file, _iconSize, _iconFlags);
						//if(0!=(_iconFlags&IconFlags.NeedImage) && _nPending>20) { /*Print(_nPending);*/ k.image = HandleToImage(hi); } else k.hIcon = hi;

						//Print("1");

						//Prevent overflowing the message queue and the number of icon handles.
						//Because bad things start when eg toolbar icon count is more than 3000 and they are extracted faster than consumed.
						//But don't make the threshold too low, because then may need to wait unnecessarily, and it makes slower.
						if(Interlocked.Increment(ref _nPending) >= 900) {
							//Print(_nPending);
							//var perf = Perf.StartNew();
							Thread.Sleep(10);
							//while(_nPending >= 900) Thread.Sleep(10);
							//perf.NW();
						}
				}, o =>
				{ //this code runs in the caller's thread
						Interlocked.Decrement(ref _nPending);

						//Print("2");

						var k = o as Result;
					if(_canceled) {
						Api.DestroyIcon(k.hIcon);
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
			//Print(_works.Count);
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
		~IconsAsync() { Cancel(); }

		/// <summary>
		/// For <see cref="AddRange(IEnumerable{Item})"/>. 
		/// </summary>
		/// <tocexclude />
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
		/// <tocexclude />
		public class Result
		{
			/// <summary>file passed to <see cref="Add(string, object)"/>.</summary>
			public string file;

			/// <summary>obj passed to <see cref="Add(string, object)"/>.</summary>
			public object obj;

			/// <summary>Icon handle. To get managed object from it, use <b>Icons.HandleToX</b> functions; else finally call <see cref="Icons.DestroyIconHandle"/>. Can be default(IntPtr).</summary>
			public IntPtr hIcon;

			/// <summary>Icon converted to Image object, if used IconFlags.NeedImage and the thread pool decided to convert handle to Image. You should call Dispose() when finished using it. Can be null.</summary>
			//public Image image;

			public Result(string file, object obj) { this.file = file; this.obj = obj; }
		}

		/// <summary>
		/// For <see cref="GetAllAsync"/>. 
		/// </summary>
		/// <param name="result">Contains icon, as well as the input parameters.</param>
		/// <param name="objCommon">objCommon passed to <see cref="GetAllAsync"/>.</param>
		/// <param name="nLeft">How many icons is still to get. Eg 0 if this is the last icon.</param>
		/// <tocexclude />
		public delegate void Callback(Result result, object objCommon, int nLeft);
	}
}
