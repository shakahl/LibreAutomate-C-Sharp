using Au.Types;
using Au.Util;
using Au.Triggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Au
{

	public partial class AToolbar
	{
		AToolbar _satellite;
		AToolbar _satPlanet;
		bool _satVisible;
		int _satAnimation;
		ATimer _satTimer;
		ATimer _satAnimationTimer;
		AScreen _screenAHSE;

		/// <summary>
		/// A toolbar attached to this toolbar. Can be null.
		/// </summary>
		/// <exception cref="InvalidOperationException">The 'set' function throws if the satellite toolbar was attached to another toolbar or was shown as non-satellite toolbar.</exception>
		/// <remarks>
		/// The satellite toolbar is shown when mouse enters its owner toolbar and hidden when mouse leaves it and its owner. Like an "auto hide" feature.
		/// A toolbar can have multiple satellite toolbars at different times. A satellite toolbar can be attached/detached multiple times to the same toolbar.
		/// </remarks>
		public AToolbar Satellite {
			get => _satellite;
			set {
				_ThreadTrap();
				if (value != _satellite) {
					if (value == null) {
						_SatHide();
						_satellite = null;
						//and don't clear _satPlanet etc
					} else {
						if (_closed || value._closed) throw new ObjectDisposedException(nameof(AToolbar));
						var p = value._satPlanet; if (p != this) { if (p != null || value._created) throw new InvalidOperationException(); }
						_satellite = value;
						_satellite._satPlanet = this;
					}
				}
			}
		}

		/// <summary>
		/// If this is a sattellite toolbar (<see cref="Satellite"/>), gets its owner toolbar. Else null.
		/// </summary>
		public AToolbar SatelliteOwner => _satPlanet;

		bool _IsSatellite => _satPlanet != null;

		AToolbar _SatPlanetOrThis => _satPlanet ?? this;

		void _SatMouse() {
			if (_satellite == null || _satVisible) return;
			_satVisible = true;

			//AOutput.Write("show");
			if (!_satellite._created) {
				var owner = _w;
				_satellite._CreateWindow(true, owner);
				_satellite._ow = new _OwnerWindow(owner);
				_satellite._ow.a.Add(_satellite);
				var w1 = _satellite._w; w1.OwnerWindow = owner; //let OS keep Z order and close/hide when owner toolbar closed/minimized
			}
			_SatFollow();
			_SatShowHide(true, animate: true);

			_satTimer ??= new ATimer(_SatTimer);
			_satTimer.Every(100);
		}

		void _SatTimer(ATimer _) {
			Debug.Assert(IsOpen);
			if (_inMoveSize || _satellite._inMoveSize) return;

			POINT p = AMouse.XY;
			int dist = ADpi.Scale(30, _dpi);
			var wa = AWnd.Active;

			RECT ru = default;
			if (_MouseIsIn(this) || _MouseIsIn(_satellite)) return;
			if (ru.Contains(p)) return;
			if (AInputInfo.GetGUIThreadInfo(out var g, AThread.Id) && g.flags.Has(GTIFlags.INMENUMODE)) return;

			bool _MouseIsIn(AToolbar tb) {
				var w = tb._w;
				if (w == wa) return true;
				var r = w.Rect;
				r.Inflate(dist, dist);
				if (r.Contains(p.x, p.y)) return true;
				ru.Union(r);
				return false;
			}

			_SatHide(animate: true);
		}

		void _SatDestroying() {
			if (_IsSatellite) _satPlanet.Satellite = null;
			ADebug_.PrintIf(_satellite != null, "_satellite");
			//When destroying planet, OS at first destroys satellites (owned windows).
		}

		//Hides _satellite and stops _satTimer.
		void _SatHide(bool animate = false/*, [CallerMemberName] string cmn=null*/) {
			if (_satellite == null) return;
			//AOutput.Write("hide", cmn, _satVisible);
			if (_satVisible) {
				_satVisible = false;
				_satTimer.Stop();
				_SatShowHide(false, animate);
			} else if (!animate && (_satAnimationTimer?.IsRunning ?? false)) {
				_SatShowHide(false, false);
			}
		}

		//Shows or hides _satellite and manages animation.
		void _SatShowHide(bool show, bool animate) {
			if (!animate || _satellite._transparency != default) {
				var w = _satellite._w;
				if (show != w.IsVisible) _satellite._SetVisibleL(show);
				if (_satellite._transparency == default) w.SetTransparency(false);
				_satAnimationTimer?.Stop();
				_satAnimation = 0;
				return;
			}

			_satAnimationTimer ??= new ATimer(_ => {
				_satAnimation += _satVisible ? 64 : -32;
				bool stop; if (_satVisible) { if (stop = _satAnimation >= 255) _satAnimation = 255; } else { if (stop = _satAnimation <= 0) _satAnimation = 0; }
				if (stop) {
					_satAnimationTimer.Stop();
					if (_satAnimation == 0) _satellite._SetVisibleL(false);
				}
				if (_satellite._transparency == default) _satellite._w.SetTransparency(!stop, _satAnimation);
			});
			_satAnimationTimer.Now();
			_satAnimationTimer.Every(30);

			if (show) _satellite._SetVisibleL(true);
		}

		void _SatFollow() {
			if (!_satVisible) return;
			if (!_satellite._ow.UpdateRect(out bool changed) || !changed) return;
			_satellite._FollowRect(onFollowOwner: true);
		}

		#region auto-hide owner toolbars

		/// <summary>
		/// Creates new toolbar and sets its <see cref="Satellite"/> = this.
		/// Returns the new toolbar.
		/// </summary>
		/// <param name="ctorFlags">See <see cref="AToolbar(string, TBCtor, string, int)"/>.</param>
		/// <param name="f_">[](xref:caller_info)</param>
		/// <param name="l_">[](xref:caller_info)</param>
		/// <exception cref="InvalidOperationException">This toolbar was attached to another toolbar or was shown as non-satellite toolbar.</exception>
		/// <remarks>
		/// Sets toolbar name = <c>this.Name + "^"</c>.
		/// If this already is a satellite toolbar, just returns its owner.
		/// </remarks>
		public AToolbar AutoHide(TBCtor ctorFlags = 0, [CallerFilePath] string f_ = null, [CallerLineNumber] int l_ = 0) {
			_ThreadTrap();
			if (_satPlanet == null) {
				_satPlanet = new AToolbar(this.Name + "^", ctorFlags, f_, l_) { Satellite = this };
				if (_satPlanet.FirstTime) {
					_satPlanet.Size = new(20, 20);
				}
				_satPlanet.AutoSize = false;
			}
			return _satPlanet;
		}

		/// <summary>
		/// Creates new toolbar and sets its <see cref="Satellite"/> = this. Sets properties for showing at a screen edge.
		/// Returns the new toolbar.
		/// </summary>
		/// <param name="mta">Mouse edge trigger arguments.</param>
		/// <param name="rangeStart"><i>rangeStart</i> and <i>rangeEnd</i> can be used to specify a smaller range of the edge part. For example, you can create 2 toolbars there: one with 0, 0.5f, other with 0.5f, 1f.</param>
		/// <param name="rangeEnd"></param>
		/// <param name="thickness">The visible thickness. Pixels.</param>
		/// <param name="ctorFlags">See <see cref="AToolbar(string, TBCtor, string, int)"/>.</param>
		/// <param name="f_">[](xref:caller_info)</param>
		/// <param name="l_">[](xref:caller_info)</param>
		public AToolbar AutoHideScreenEdge(MouseTriggerArgs mta, Coord rangeStart = default, Coord rangeEnd = default, int thickness = 1, TBCtor ctorFlags = 0, [CallerFilePath] string f_ = null, [CallerLineNumber] int l_ = 0) {
			if (mta == null) throw new ArgumentNullException();
			if (mta.Trigger.Kind != TMKind.Edge) throw new ArgumentException("Not an edge trigger.");
			return AutoHideScreenEdge(mta.Trigger.Edge, mta.Trigger.ScreenIndex, rangeStart, rangeEnd, thickness, ctorFlags, f_, l_);
		}

		/// <summary>
		/// Creates new toolbar and sets its <see cref="Satellite"/> = this. Sets properties for showing at a screen edge.
		/// Returns the new toolbar.
		/// </summary>
		/// <param name="edge">Screen edge/part.</param>
		/// <param name="screen">Screen index. Default: primary.</param>
		/// <param name="rangeStart"><i>rangeStart</i> and <i>rangeEnd</i> can be used to specify a smaller range of the edge part. For example, you can create 2 toolbars there: one with 0, 0.5f, other with 0.5f, 1f.</param>
		/// <param name="rangeEnd"></param>
		/// <param name="thickness">The visible thickness. Pixels.</param>
		/// <param name="ctorFlags">See <see cref="AToolbar(string, TBCtor, string, int)"/>.</param>
		/// <param name="f_">[](xref:caller_info)</param>
		/// <param name="l_">[](xref:caller_info)</param>
		public AToolbar AutoHideScreenEdge(TMEdge edge, TMScreen screen = TMScreen.Primary, Coord rangeStart = default, Coord rangeEnd = default, int thickness = 1, TBCtor ctorFlags = 0, [CallerFilePath] string f_ = null, [CallerLineNumber] int l_ = 0) {
			_ThreadTrap();
			if (screen < 0) throw new NotSupportedException("screen");
			var sh = AScreen.Index((int)screen);
			var rs = sh.Rect;

			var se = edge.ToString(); char se0 = se[0];
			bool vertical = se0 == 'L' || se0 == 'R';

			TBAnchor anchor = TBAnchor.TopLeft;
			RECT k = default;
			if (thickness <= 0) thickness = 1;
			int offscreen = thickness == 1 ? 1 : 0;
			switch (se0) {
			case 'T': k.top = -offscreen; break;
			case 'R': k.right = -offscreen; anchor = TBAnchor.TopRight; break;
			case 'B': k.bottom = -offscreen; anchor = TBAnchor.BottomLeft; break;
			case 'L': k.left = -offscreen; break;
			}
			int x25 = rs.Width / 4, y25 = rs.Height / 4;
			bool reverse = false;
			switch (edge) {
			case TMEdge.TopInCenter50: k.left = x25; break;
			case TMEdge.TopInRight25: anchor = TBAnchor.TopRight; reverse = true; break;
			case TMEdge.RightInCenter50: k.top = y25; break;
			case TMEdge.RightInBottom25: anchor = TBAnchor.BottomRight; reverse = true; break;
			case TMEdge.BottomInCenter50: k.left = x25; break;
			case TMEdge.BottomInRight25: anchor = TBAnchor.BottomRight; reverse = true; break;
			case TMEdge.LeftInCenter50: k.top = y25; break;
			case TMEdge.LeftInBottom25: anchor = TBAnchor.BottomLeft; reverse = true; break;
			}

			int edgeLength = vertical ? rs.Height : rs.Width; if (se.Contains("25")) edgeLength /= 4; else if (se.Contains("50")) edgeLength /= 2;
			int move = rangeStart.IsEmpty ? 0 : rangeStart.NormalizeInRange(0, edgeLength);
			int length = rangeEnd.IsEmpty ? edgeLength : Math.Max(0, rangeEnd.NormalizeInRange(0, edgeLength) - move);
			if (vertical) {
				if (reverse) k.bottom = edgeLength - length - move; else k.top += move;
			} else {
				if (reverse) k.right = edgeLength - length - move; else k.left += move;
			}

			var planet = AutoHide(ctorFlags, f_, l_);
			planet._screenAHSE = sh;
			if (planet.FirstTime) {
				planet.Size = ADpi.Unscale(vertical ? new SIZE(thickness + offscreen, length) : new SIZE(length, thickness + offscreen), sh.Handle);
				planet.Sizable = false;
			}
			planet.Anchor = anchor;
			planet.Offsets = new(k.left, k.top, k.right, k.bottom);
			planet.Border = TBBorder.Width1;
			planet.NoContextMenu = TBNoMenu.Anchor | TBNoMenu.Border | TBNoMenu.Layout | TBNoMenu.AutoSize;

			this.Anchor = anchor | (vertical ? TBAnchor.OppositeEdgeX : TBAnchor.OppositeEdgeY);
			this.NoContextMenu = TBNoMenu.Anchor;

			return planet;
		}

		#endregion

	}
}
