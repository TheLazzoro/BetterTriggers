﻿// ------------------------------------------------------------------------------
// <copyright file="BinaryWriterExtensions.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;

namespace War3Net.Common.Extensions
{
    public static class BinaryWriterExtensions
    {
        /// <param name="endWithNullChar">Set this to <see langword="false"/> when writing a length-prefixed string.</param>
        public static void WriteString(this BinaryWriter writer, string? s, bool endWithNullChar = true)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            char? storedSurrogateChar = null;
            var endsWithNullChar = false;
            foreach (var c in s ?? string.Empty)
            {
                if (endsWithNullChar && c != char.MinValue)
                {
                    throw new ArgumentException("String is not allowed to contain \0, unless it is the last character.", nameof(s));
                }

                if (storedSurrogateChar.HasValue)
                {
                    if (!char.IsSurrogatePair(storedSurrogateChar.Value, c))
                    {
                        throw new ArgumentException("Invalid surrogate pair.", nameof(s));
                    }

                    writer.Write(new[] { storedSurrogateChar.Value, c });
                    storedSurrogateChar = null;
                }
                else if (char.IsSurrogate(c))
                {
                    if (!char.IsHighSurrogate(c))
                    {
                        throw new ArgumentException("Surrogate pair must start with high surrogate.", nameof(s));
                    }

                    storedSurrogateChar = c;
                }
                else
                {
                    writer.Write(c);
                    endsWithNullChar = c == char.MinValue;
                }
            }

            if (storedSurrogateChar.HasValue)
            {
                throw new ArgumentException("Expected surrogate pair.", nameof(s));
            }

            if (!endsWithNullChar && endWithNullChar)
            {
                writer.Write(char.MinValue);
            }
        }

        public static void WriteString(this BinaryWriter writer, string? s, int length)
        {
            s ??= string.Empty;

            var stringLength = s.Length;
            if (stringLength > length)
            {
                throw new ArgumentOutOfRangeException(nameof(s));
            }

            if (stringLength < length)
            {
                // Add padding
                s = new string(s.Concat(new string(char.MinValue, length - stringLength)).ToArray());
            }

            foreach (var c in s)
            {
                writer.Write(c);
            }
        }

        /// <summary>
        /// Writes a four-byte <see cref="bool"/> value to the current stream, with 0 representing <see langword="false"/> and 1 representing <see langword="true"/>.
        /// </summary>
        public static void WriteBool(this BinaryWriter writer, bool b)
        {
            writer.Write(b ? 1 : 0);
        }

        public static void WriteInt24(this BinaryWriter writer, int value)
        {
            if (value >= 1 << 23 || value < -(1 << 23))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (value < 0)
            {
                value += 1 << 24;
            }

            writer.Write((byte)(value & 0xFF));
            writer.Write((byte)((value >> 8) & 0xFF));
            writer.Write((byte)(value >> 16));
        }

        public static void WriteUInt24(this BinaryWriter writer, uint value)
        {
            if (value >= 1 << 24)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            writer.Write((byte)(value & 0xFF));
            writer.Write((byte)((value >> 8) & 0xFF));
            writer.Write((byte)(value >> 16));
        }
    }
}