﻿// ------------------------------------------------------------------------------
// <copyright file="HashTable.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.IO;

namespace War3Net.IO.Mpq
{
    /// <summary>
    /// The <see cref="HashTable"/> of an <see cref="MpqArchive"/> contains the list of <see cref="MpqHash"/> objects.
    /// </summary>
    internal sealed class HashTable : MpqTable
    {
        /// <summary>
        /// The key used to encrypt and decrypt the <see cref="HashTable"/>.
        /// </summary>
        internal const string TableKey = "(hash table)";

        internal readonly MpqHash[] _hashes;
        private readonly uint _mask;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashTable"/> class.
        /// </summary>
        /// <param name="size">The maximum amount of entries that can be contained in this table. This value is automatically rounded up to the nearest power of two.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="size"/> argument is larger than <see cref="MpqTable.MaxSize"/>.</exception>
        public HashTable(uint size)
        {
            if (size > MaxSize)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            _mask = GenerateMask(size);
            size = Size;

            _hashes = new MpqHash[size];
            for (var i = 0; i < size; i++)
            {
                _hashes[i] = MpqHash.NULL;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashTable"/> class.
        /// </summary>
        /// <param name="minimumSize">The minimum capacity that this <see cref="HashTable"/> should have.</param>
        /// <param name="freeSpace">Determines how much space is available for files with known filenames. Use 1 if no files with an unknown filename will be added.</param>
        /// <param name="multiplier">Multiplier for the size of the hashtable. By increasing the size beyond the minimum, the amount of collisions with StringHash will be reduced.</param>
        /// <exception cref="DivideByZeroException">Thrown when <paramref name="freeSpace"/> is zero.</exception>
        public HashTable(uint minimumSize, float freeSpace, float multiplier)
            : this(Math.Min(MaxSize, Math.Max(minimumSize, (uint)(multiplier * minimumSize / freeSpace))))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashTable"/> class.
        /// </summary>
        /// <param name="knownFiles">The amount of files with a known filename that will be added to the <see cref="HashTable"/>.</param>
        /// <param name="unknownFiles">The amount of files with an unknown filename that will be added to the <see cref="HashTable"/>.</param>
        /// <param name="oldTableSize">The size of the smallest <see cref="HashTable"/> from which the unknown files came.</param>
        /// <param name="multiplier">Multiplier for the size of the hashtable. By increasing the size beyond the minimum, the amount of collisions with StringHash will be reduced.</param>
        /// <exception cref="DivideByZeroException">Thrown when <paramref name="unknownFiles"/> is equal to <paramref name="oldTableSize"/>.</exception>
        /// <remarks>
        /// If the unknown files are sourced from multiple <see cref="HashTable"/>s with different sizes, it's recommended to use a different constructor.
        /// </remarks>
        public HashTable(uint knownFiles, uint unknownFiles, uint oldTableSize, float multiplier)
            : this(knownFiles, 1 - ((float)unknownFiles / oldTableSize), multiplier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashTable"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> from which to read the contents of the <see cref="HashTable"/>.</param>
        /// <param name="size">The amount of <see cref="MpqHash"/> objects to be added to the <see cref="HashTable"/>.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="size"/> argument is not a power of two.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="size"/> argument is larger than <see cref="MpqTable.MaxSize"/>.</exception>
        internal HashTable(BinaryReader reader, uint size)
        {
            if (size > MaxSize)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            if (size != GenerateMask(size) + 1)
            {
                throw new ArgumentException($"Size {size} is not a power of two.", nameof(size));
            }

            _hashes = new MpqHash[size];
            _mask = size - 1;

            var hashdata = reader.ReadBytes((int)(size * MpqHash.Size));
            Decrypt(hashdata);

            using (var stream = new MemoryStream(hashdata))
            {
                using (var streamReader = new BinaryReader(stream))
                {
                    for (var i = 0; i < size; i++)
                    {
                        _hashes[i] = new MpqHash(streamReader, _mask);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the capacity of the <see cref="HashTable"/>.
        /// </summary>
        public override uint Size => _mask + 1;

        /// <summary>
        /// Gets the mask for this <see cref="HashTable"/>.
        /// </summary>
        public uint Mask => _mask;

        /// <summary>
        /// Gets the key used to encrypt and decrypt the <see cref="HashTable"/>.
        /// </summary>
        protected override string Key => TableKey;

        /// <summary>
        /// Gets the length (in bytes) of a single <see cref="MpqHash"/> in the <see cref="HashTable"/>.
        /// </summary>
        protected override int EntrySize => (int)MpqHash.Size;

        /// <summary>
        /// Gets or sets the <see cref="MpqHash"/> at specified index.
        /// </summary>
        /// <param name="i">The zero-based index of the <see cref="MpqHash"/> to get.</param>
        /// <returns>The <see cref="MpqHash"/> at index <paramref name="i"/> of the <see cref="HashTable"/>.</returns>
        internal MpqHash this[int i]
        {
            get => _hashes[i];
            set => _hashes[i] = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="MpqHash"/> at specified index.
        /// </summary>
        /// <param name="i">The zero-based index of the <see cref="MpqHash"/> to get.</param>
        /// <returns>The <see cref="MpqHash"/> at index <paramref name="i"/> of the <see cref="HashTable"/>.</returns>
        internal MpqHash this[uint i]
        {
            get => _hashes[i];
            set => _hashes[i] = value;
        }

        /// <summary>
        /// Generates a bit mask for the given <paramref name="size"/>.
        /// </summary>
        /// <param name="size">The size for which to generate a bit mask.</param>
        /// <returns>Returns the bit mask for the given <paramref name="size"/>.</returns>
        public static uint GenerateMask(uint size)
        {
            size--;
            size |= size >> 1;
            size |= size >> 2;
            size |= size >> 4;
            size |= size >> 8;
            size |= size >> 16;
            return size;
        }

        /// <summary>
        /// Adds an <see cref="MpqHash"/> to the <see cref="HashTable"/>.
        /// </summary>
        /// <param name="hash">The <see cref="MpqHash"/> to be added to the <see cref="HashTable"/>.</param>
        /// <param name="hashIndex">The index at which to add the <see cref="MpqHash"/>.</param>
        /// <param name="hashCollisions">The maximum amount of collisions, if the <see cref="MpqFile"/> came from another <see cref="MpqArchive"/> and has an unknown filename.</param>
        /// <returns>
        /// Returns the amount of <see cref="MpqHash"/> objects that have been added.
        /// This is usually 1, but can be more if the <see cref="MpqFile"/> came from another <see cref="MpqArchive"/>, has an unknown filename,
        /// and the <see cref="HashTable"/> of the <see cref="MpqArchive"/> it came from has a smaller size than this one.
        /// </returns>
        public uint Add(MpqHash hash, uint hashIndex, uint hashCollisions)
        {
            return AddEntry(hash, hashIndex, hashCollisions, hash.Mask + 1);
        }

        /// <summary>
        /// Writes the <see cref="MpqHash"/> at index <paramref name="i"/>.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the content to.</param>
        /// <param name="i">The index of the <see cref="MpqHash"/> to write.</param>
        protected override void WriteEntry(BinaryWriter writer, int i)
        {
            _hashes[i].WriteTo(writer);
        }

        private uint AddEntry(MpqHash hash, uint hashIndex, uint hashCollisions, uint step)
        {
            // If the old archive had a smaller hashtable, it masked less bits to determine the index for the hash entry, and cannot recover the bits that were masked away.
            // As a result, need to add this hash entry in every index where the bits match with the old archive's mask.
            var copy = 0U;
            for (var i = hashIndex & _mask; i <= _mask; i += step)
            {
                // Console.WriteLine( "Try to add file #{0}'s hash at index {1}", hash.BlockIndex, i );
                var mpqHash = new MpqHash(hash.Name, hash.Locale, hash.BlockIndex + copy, hash.Mask);
                TryAdd(mpqHash, i, hashCollisions);
                copy++;
            }

            return copy;
        }

        private void TryAdd(MpqHash hash, uint index, uint hashCollisions)
        {
            if (hashCollisions > 0)
            {
                var startIndex = (int)index - (int)hashCollisions;
                if (startIndex < 0)
                {
                    startIndex += (int)Size;
                }

                for (var i = 0; i < hashCollisions; i++)
                {
                    var j = (startIndex + 1) & _mask;
                    if (_hashes[j].IsEmpty)
                    {
                        _hashes[j] = MpqHash.DELETED;
                    }
                }
            }

            while (!_hashes[index].IsAvailable)
            {
                index = (index + 1) & _mask;

                // or: if (++index)>_mask index=0;
            }

            _hashes[index] = hash;
        }
    }
}