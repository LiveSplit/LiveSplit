// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BroadcasterPlugin.cs" company="Starboard">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <author> William Eddins </author>
// <summary>
//   Encapsulates logic behind sending plugin updates to the XSplit rendering system. Instances should be
//   created through the CreateInstance static method, which ensures XSplit is installed when attempting
//   to create the COM object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XSplit.Wpf
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using VHMediaCOMLib;

    /// <summary>
    /// Encapsulates logic behind sending plugin updates to the XSplit rendering system. Instances should be
    ///   created through the CreateInstance static method, which ensures XSplit is installed when attempting
    ///   to create the COM object.
    /// </summary>
    public class BroadcasterPlugin
    {
        #region Constants and Fields

        /// <summary>
        ///   Instance of the XSplit COM object.
        /// </summary>
        private readonly VHCOMRenderEngineExtSrc2 xsplit;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcasterPlugin"/> class. 
        ///   Prevents the class from being manually created.
        /// </summary>
        /// <param name="xsplit">
        /// The xsplit instance to attach.
        /// </param>
        protected BroadcasterPlugin(VHCOMRenderEngineExtSrc2 xsplit)
        {
            this.xsplit = xsplit;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets a value indicating whether the XSplit connection is ready to receive images.
        /// </summary>
        public bool ConnectionIsReady
        {
            get
            {
                return (xsplit.ConnectionStatus & 3) == 3;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to create an instance of the BroadcasterPlugin class. If XSplit is not installed, null is returned.
        /// </summary>
        /// <param name="connectionUID">
        /// Unique ID to apply to this application, should match the accompanying .xbs file.
        /// </param>
        /// <returns>
        /// Returns an instance of BroadcasterPlugin if XSplit is installed on the system, else null is returned.
        /// </returns>
        public static BroadcasterPlugin CreateInstance(string connectionUID)
        {
            BroadcasterPlugin plugin = null;

            try
            {
                var extsrc = new VHCOMRenderEngineExtSrc2 { ConnectionUID = connectionUID };
                plugin = new BroadcasterPlugin(extsrc);
            }
            catch (COMException)
            {
                // Do nothing, the plugin failed to load so null will be returned.
            }

            return plugin;
        }

        /// <summary>
        /// Renders an object and sends the image to the XSplit plugin.
        ///   This method must be called on the same thread as the owner of the Visual, in most cases the UI (main) thread.
        /// </summary>
        /// <param name="obj">
        /// Visual object to render and send to XSplit.
        /// </param>
        /// <param name="width">
        /// Desired output width, in pixels.
        /// </param>
        /// <param name="height">
        /// Desired output height, in pixels.
        /// </param>
        /// <returns>
        /// Returns whether the call was successful and the object was rendered. If obj is null, false will always be returned.
        /// </returns>
        public bool RenderVisual(Bitmap img)
        {
            if (img == null)
            {
                return false;
            }

            if (ConnectionIsReady)
            {
                // The remaining work (format conversion, sending to xsplit) can be done on a seperate thread)
                Task.Factory.StartNew(
                    () =>
                        {

                            using (var stream = new MemoryStream())
                            {
                                img.Save(stream, ImageFormat.Bmp);

                                stream.Position = 0;

                                byte[] bytes = stream.ToArray();

                                // Length of output data we're going to send.
                                int length = img.Width * img.Height * 4;

                                // Allocate memory for bitmap transfer to COM
                                IntPtr dataptr = Marshal.AllocCoTaskMem(length);
                                Marshal.Copy(bytes, bytes.Length - length, dataptr, length);
                                xsplit.SendFrame((uint)img.Width, (uint)img.Height, dataptr.ToInt32());

                                // Send to broadcaster
                                Marshal.FreeCoTaskMem(dataptr);
                            }
                        });

                return true;
            }

            return false;
        }

        #endregion
    }
}