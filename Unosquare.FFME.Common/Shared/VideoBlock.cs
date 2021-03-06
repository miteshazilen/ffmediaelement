﻿namespace Unosquare.FFME.Shared
{
    using ClosedCaptions;
    using Decoding;
    using FFmpeg.AutoGen;
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A pre-allocated, scaled video block. The buffer is in BGR, 24-bit format
    /// </summary>
    public sealed class VideoBlock : MediaBlock, IDisposable
    {
        #region Constructors and Descrutors

        /// <summary>
        /// Finalizes an instance of the <see cref="VideoBlock"/> class.
        /// </summary>
        ~VideoBlock()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the media type of the data
        /// </summary>
        public override MediaType MediaType => MediaType.Video;

        /// <summary>
        /// Gets the number of horizontal pixels in the image.
        /// </summary>
        public int PixelWidth { get; private set; }

        /// <summary>
        /// Gets the number of vertical pixels in the image.
        /// </summary>
        public int PixelHeight { get; private set; }

        /// <summary>
        /// Gets the width of the aspect ratio.
        /// </summary>
        public int AspectWidth { get; internal set; }

        /// <summary>
        /// Gets the height of the aspect ratio.
        /// </summary>
        public int AspectHeight { get; internal set; }

        /// <summary>
        /// Gets the SMTPE time code.
        /// </summary>
        public string SmtpeTimecode { get; internal set; }

        /// <summary>
        /// Gets the display picture number (frame number).
        /// If not set by the decoder, this attempts to obtain it by dividing the start time by the
        /// frame duration
        /// </summary>
        public long DisplayPictureNumber { get; internal set; }

        /// <summary>
        /// Gets the coded picture number set by the decoder.
        /// </summary>
        public long CodedPictureNumber { get; internal set; }

        /// <summary>
        /// Gets the closed caption packets for this video block.
        /// </summary>
        public ReadOnlyCollection<ClosedCaptionPacket> ClosedCaptions { get; internal set; }

        /// <summary>
        /// Gets the picture buffer stride.
        /// </summary>
        internal int PictureBufferStride { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Allocates a block of memory suitable for a picture buffer
        /// and sets the corresponding properties.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        internal unsafe void Allocate(VideoFrame source, AVPixelFormat pixelFormat)
        {
            // Ensure proper allocation of the buffer
            // If there is a size mismatch between the wanted buffer length and the existing one,
            // then let's reallocate the buffer and set the new size (dispose of the existing one if any)
            var targetLength = ffmpeg.av_image_get_buffer_size(pixelFormat, source.Pointer->width, source.Pointer->height, 1);
            Allocate(targetLength);

            // Update related properties
            PictureBufferStride = ffmpeg.av_image_get_linesize(pixelFormat, source.Pointer->width, 0);
            PixelWidth = source.Pointer->width;
            PixelHeight = source.Pointer->height;
        }

        /// <summary>
        /// Deallocates the picture buffer and resets the related buffer properties
        /// </summary>
        protected override void Deallocate()
        {
            base.Deallocate();
            PictureBufferStride = 0;
            PixelWidth = 0;
            PixelHeight = 0;
        }

        #endregion

    }
}
