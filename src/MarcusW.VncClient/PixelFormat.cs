using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using MarcusW.VncClient.Protocol;
using MarcusW.VncClient.Rendering;

namespace MarcusW.VncClient
{
    /// <summary>
    /// Represents a pixel format that's used for RFB encodings.
    /// </summary>
    public readonly struct PixelFormat : IEquatable<PixelFormat>
    {
        /// <summary>
        /// An invalid pixel format representing unknown values.
        /// </summary>
        public static readonly PixelFormat Unknown = new PixelFormat("Unknown", 0, 0, false, false, false, 0, 0, 0, 0, 0, 0, 0, 0);

        /// <summary>
        /// Gets the name of this pixel format.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the number of bits used for each pixel on the wire.
        /// </summary>
        public byte BitsPerPixel { get; }

        /// <summary>
        /// Gets the number of useful bits in the pixel value. Must be greater than or equal to <see cref="BitsPerPixel"/>.
        /// </summary>
        public byte Depth { get; }

        /// <summary>
        /// Gets if multi-byte pixels are interpreted as big endian.
        /// </summary>
        public bool BigEndian { get; }

        /// <summary>
        /// Gets whether the pixel value is composed from the color values (True), or if the color values serve as indices into a color map (False).
        /// </summary>
        public bool TrueColor { get; }

        /// <summary>
        /// Gets whether this pixel format contains an alpha channel.
        /// </summary>
        public bool HasAlpha { get; }

        /// <summary>
        /// Gets the maximum value for the color red.
        /// </summary>
        public ushort RedMax { get; }

        /// <summary>
        /// Gets the maximum value for the color green.
        /// </summary>
        public ushort GreenMax { get; }

        /// <summary>
        /// Gets the maximum value for the color blue.
        /// </summary>
        public ushort BlueMax { get; }

        /// <summary>
        /// Gets the maximum value for the alpha value.
        /// </summary>
        public ushort AlphaMax { get; }

        /// <summary>
        /// Gets the number of right-shifts needed to get the red value in a pixel.
        /// </summary>
        public byte RedShift { get; }

        /// <summary>
        /// Gets the number of right-shifts needed to get the green value in a pixel.
        /// </summary>
        public byte GreenShift { get; }

        /// <summary>
        /// Gets the number of right-shifts needed to get the blue value in a pixel.
        /// </summary>
        public byte BlueShift { get; }

        /// <summary>
        /// Gets the number of right-shifts needed to get the alpha value in a pixel.
        /// </summary>
        public byte AlphaShift { get; }

        /// <summary>
        /// Gets the number of bytes used for each pixel on the wire.
        /// </summary>
        public byte BytesPerPixel => (byte)(BitsPerPixel / 8);

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelFormat"/> structure.
        /// </summary>
        /// <param name="name">The name of the pixel format.</param>
        /// <param name="bitsPerPixel">The number of bits used for each pixel on the wire.</param>
        /// <param name="depth">The number of useful bits in the pixel value. Must be greater than or equal to <paramref name="bitsPerPixel"/>.</param>
        /// <param name="bigEndian">True if multi-byte pixels are interpreted as big endian, otherwise false.</param>
        /// <param name="trueColor">True if the pixel value is composed from the color values, or false if the color values serve as indices into a color map.</param>
        /// <param name="hasAlpha">True if this pixel format contains an alpha channel, otherwise false.</param>
        /// <param name="redMax">The maximum value for the color red.</param>
        /// <param name="greenMax">The maximum value for the color green.</param>
        /// <param name="blueMax">The maximum value for the color blue.</param>
        /// <param name="alphaMax">The maximum value for the alpha value.</param>
        /// <param name="redShift">The number of right-shifts needed to get the red value in a pixel.</param>
        /// <param name="greenShift">The number of right-shifts needed to get the green value in a pixel.</param>
        /// <param name="blueShift">The number of right-shifts needed to get the blue value in a pixel.</param>
        /// <param name="alphaShift">The number of right-shifts needed to get the alpha value in a pixel.</param>
        public PixelFormat(string name, byte bitsPerPixel, byte depth, bool bigEndian, bool trueColor, bool hasAlpha, ushort redMax, ushort greenMax, ushort blueMax,
            ushort alphaMax, byte redShift, byte greenShift, byte blueShift, byte alphaShift)
        {
            Name = name;
            BitsPerPixel = bitsPerPixel;
            Depth = depth;
            BigEndian = bigEndian;
            TrueColor = trueColor;
            HasAlpha = hasAlpha;
            RedMax = redMax;
            GreenMax = greenMax;
            BlueMax = blueMax;
            AlphaMax = HasAlpha ? alphaMax : (ushort)0;
            RedShift = redShift;
            GreenShift = greenShift;
            BlueShift = blueShift;
            AlphaShift = HasAlpha ? alphaShift : (byte)0;
        }

        /// <summary>
        /// Checks for equality between two <see cref="PixelFormat"/>s.
        /// </summary>
        /// <param name="left">The first pixel format.</param>
        /// <param name="right">The second pixel format.</param>
        /// <returns>True if the sizes are equal, otherwise false.</returns>
        public static bool operator ==(PixelFormat left, PixelFormat right) => left.Equals(right);

        /// <summary>
        /// Checks for inequality between two <see cref="PixelFormat"/>s.
        /// </summary>
        /// <param name="left">The first pixel format.</param>
        /// <param name="right">The second pixel format.</param>
        /// <returns>True if the sizes are unequal, otherwise false.</returns>
        public static bool operator !=(PixelFormat left, PixelFormat right) => !left.Equals(right);

        /// <inheritdoc />
        public bool Equals(PixelFormat other)
        {
            if (BitsPerPixel != other.BitsPerPixel || Depth != other.Depth || BigEndian != other.BigEndian || TrueColor != other.TrueColor || HasAlpha != other.HasAlpha
                || RedMax != other.RedMax || GreenMax != other.GreenMax || BlueMax != other.BlueMax || RedShift != other.RedShift || GreenShift != other.GreenShift
                || BlueShift != other.BlueShift)
                return false;

            if (HasAlpha)
            {
                if (AlphaMax != other.AlphaMax || AlphaShift != other.AlphaShift)
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is PixelFormat other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(BitsPerPixel);
            hashCode.Add(Depth);
            hashCode.Add(BigEndian);
            hashCode.Add(TrueColor);
            hashCode.Add(HasAlpha);
            hashCode.Add(RedMax);
            hashCode.Add(GreenMax);
            hashCode.Add(BlueMax);
            hashCode.Add(AlphaMax);
            hashCode.Add(RedShift);
            hashCode.Add(GreenShift);
            hashCode.Add(BlueShift);
            hashCode.Add(AlphaShift);
            return hashCode.ToHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (this == Unknown)
                return "Unknown";

            string str =
                $"{Name}, Depth {Depth} ({BitsPerPixel}bpp), {(BigEndian ? "BE" : "LE")}, {(TrueColor ? "True-Color" : "Color-Map")}, R: {RedMax} (>>{RedShift}), G: {GreenMax} (>>{GreenShift}), B: {BlueMax} (>>{BlueShift})";
            if (HasAlpha)
                str += $", B: {AlphaMax} (>>{AlphaShift})";
            return str;
        }
    }
}
