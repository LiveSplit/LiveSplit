// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimedBroadcasterPlugin.cs" company="Starboard">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <author> William Eddins </author>
// <summary>
//   Represents a BroadcasterPlugin object that will keep track of a Visual
//   object, and render updates based on a timer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XSplit.Wpf
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows.Forms;
    using VHMediaCOMLib;
    using Timer = System.Timers.Timer;

    /// <summary>
    /// Represents a BroadcasterPlugin object that will keep track of a Visual 
    ///   object, and render updates based on a timer.
    /// </summary>
    public class TimedBroadcasterPlugin : BroadcasterPlugin, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   Timer instance
        /// </summary>
        private readonly Timer timer;

        /// <summary>
        ///   Whether this object has been disposed yet, to prevent duplicate calls.
        /// </summary>
        private bool disposed;

        /// <summary>
        ///   TaskScheduler defining the UI thread. This assumes this object is created properly on the UI thread.
        /// </summary>
        private TaskScheduler taskScheduler;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedBroadcasterPlugin"/> class.
        /// </summary>
        /// <param name="xsplit">
        /// The xsplit COM instance. 
        /// </param>
        protected TimedBroadcasterPlugin(VHCOMRenderEngineExtSrc2 xsplit)
            : base(xsplit)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedBroadcasterPlugin"/> class.
        /// </summary>
        /// <param name="xsplit">
        /// The xsplit COM instance. 
        /// </param>
        /// <param name="timeInterval">
        /// The time interval between updates for the timer. 
        /// </param>
        protected TimedBroadcasterPlugin(VHCOMRenderEngineExtSrc2 xsplit, int timeInterval)
            : this(xsplit)
        {
            this.timer = new Timer(timeInterval);
            this.timer.Elapsed += this.RenderTimerElapsed;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TimedBroadcasterPlugin"/> class. Simply implements the IDisposable pattern.
        /// </summary>
        ~TimedBroadcasterPlugin()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the interval, in milliseconds, that an updated frame is rendered and sent to the broadcaster.
        /// </summary>
        public double Interval
        {
            get
            {
                return this.timer.Interval;
            }

            set
            {
                this.timer.Interval = value;
            }
        }

        /// <summary>
        /// Gets the visual object attached that is being rendered to XSplit.
        /// </summary>
        public Bitmap Image
        {
            get; private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates an instance of the TimedBroadcasterPlugin class, attempting to make a connection to XSplit.
        /// </summary>
        /// <param name="connectionUID">
        /// A unique ID for this application, to be matched in the accompanying .xbs file. 
        /// </param>
        /// <param name="image">
        /// The visual to be rendered. 
        /// </param>
        /// <param name="timeInterval">
        /// The time interval between updates, in milliseconds. 
        /// </param>
        /// <returns>
        /// Returns an instance of a TimedBroadcasterPlugin if the connection to XSplit was successful, otherwise null is returned. 
        /// </returns>
        public static TimedBroadcasterPlugin CreateInstance(
            string connectionUID, Bitmap image, int timeInterval)
        {
            TimedBroadcasterPlugin plugin = null;

            try
            {
                var extsrc = new VHCOMRenderEngineExtSrc2 { ConnectionUID = connectionUID };
                plugin = new TimedBroadcasterPlugin(extsrc, timeInterval)
                    {
                        Image = image, 
                        taskScheduler = TaskScheduler.FromCurrentSynchronizationContext()
                    };
            }
            catch (COMException)
            {
                // Do nothing, the plugin failed to load so null will be returned.
            }

            return plugin;
        }

        /// <summary>
        /// Releases all resources used by the TimedBroadcasterPlugin.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Begins the update timer, triggering frames to be rendered to XSplit.
        /// </summary>
        public void StartTimer()
        {
            this.timer.Start();
        }

        /// <summary>
        /// Stops the update timer, stopping all updates to XSplit until restarted.
        /// </summary>
        public void StopTimer()
        {
            this.timer.Stop();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Stops and releases the timer when disposed.
        /// </summary>
        /// <param name="disposing">
        /// Whether this was calling through the Dispose() method. 
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.timer.Stop();

                if (disposing)
                {
                    this.timer.Dispose();
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// Begins a UI thread to render the visual object when the render timer has elapsed.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void RenderTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Task.Factory.StartNew(
                () => this.RenderVisual(this.Image), 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                this.taskScheduler);
        }

        #endregion
    }
}