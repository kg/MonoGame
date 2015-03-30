#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2014 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace Microsoft.Xna.Framework.Graphics
{	
	public abstract class GraphicsResource : IDisposable
	{
		#region Public Properties

		public GraphicsDevice GraphicsDevice
		{
			get;
			internal set;
		}

		public bool IsDisposed
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			set;
		}

		public Object Tag
		{
			get;
			set;
		}

		#endregion

		#region Private Variables

		#endregion

		#region Private Static Variables

		// Resources may be added to and removed from the list from many threads.
		private static object resourcesLock = new object();

		/* Use WeakReference for the global resources list as we do not know when a resource
		 * may be disposed and collected. We do not want to prevent a resource from being
		 * collected by holding a strong reference to it in this list.
         * 
         */

        /*
         * unfortunately, no weakrefs in JS! - mispy
		 */
		private static List<GraphicsResource> resources = new List<GraphicsResource>();

		#endregion

		#region Disposing Event

		public event EventHandler<EventArgs> Disposing;

		#endregion

		#region Internal Constructor and Deconstructor

		internal GraphicsResource()
		{
			lock (resourcesLock)
			{
                resources.Add(this);
			}
		}

		~GraphicsResource()
		{
			// Pass false so the managed objects are not released
			// FIXME: How, I say, how in the fuck was this supposed to work? -flibit
			// Dispose(false);
		}

		#endregion

		#region Public Dispose Method

		public void Dispose()
		{
			// Dispose of managed objects as well
			Dispose(true);
			// Since we have been manually disposed, do not call the finalizer on this object
			GC.SuppressFinalize(this);
		}

		#endregion

		#region Public Methods

		public override string ToString()
		{
			return string.IsNullOrEmpty(Name) ? base.ToString() : Name;
		}

		#endregion

		#region Internal Methods

		/// <summary>
		/// Called before the device is reset. Allows graphics resources to
		/// invalidate their state so they can be recreated after the device reset.
		/// Warning: This may be called after a call to Dispose() up until
		/// the resource is garbage collected.
		/// </summary>
		internal protected virtual void GraphicsDeviceResetting()
		{
		}

		#endregion

		#region Protected Dispose Method

		/// <summary>
		/// The method that derived classes should override to implement disposing of
		/// managed and native resources.
		/// </summary>
		/// <param name="disposing">True if managed objects should be disposed.</param>
		/// <remarks>
		/// Native resources should always be released regardless of the value of the
		/// disposing parameter.
		/// </remarks>
		protected virtual void Dispose(bool disposing)
		{
			// FIXME: What was this? No, really, what? -flibit
			//if (!IsDisposed)
			//{
			//if (disposing)
			//{
			// Release managed objects
			// ...
			//}

			// Release native objects
			// ...

			// Do not trigger the event if called from the finalizer
			if (disposing && Disposing != null)
			{
				Disposing(this, EventArgs.Empty);
			}

			// Remove from the global list of graphics resources
			lock (resourcesLock)
			{
				resources.Remove(this);
			}

			//selfReference = null;
			GraphicsDevice = null;
			IsDisposed = true;
			//}
		}

		#endregion

		#region Internal Static Methods

		internal static void DoGraphicsDeviceResetting()
		{
			lock (resourcesLock)
			{
				foreach (GraphicsResource resource in resources)
				{
                    resource.GraphicsDeviceResetting();
				}

				// Remove references to resources that have been garbage collected.
				//resources.RemoveAll(wr => !wr.IsAlive);
			}
		}

		/// <summary>
		/// Dispose all graphics resources remaining in the global resources list.
		/// </summary>
		internal static void DisposeAll()
		{
			lock (resourcesLock)
			{
				foreach (GraphicsResource resource in resources.ToArray())
				{
				    resource.Dispose();
				}
				resources.Clear();
			}
		}

		#endregion
	}
}
