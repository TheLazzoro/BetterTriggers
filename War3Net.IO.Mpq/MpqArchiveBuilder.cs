﻿// ------------------------------------------------------------------------------
// <copyright file="MpqArchiveBuilder.cs" company="Drake53">
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>
// ------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using War3Net.IO.Mpq.Extensions;

namespace War3Net.IO.Mpq
{
    public class MpqArchiveBuilder : IEnumerable<MpqFile>
    {
        private readonly ushort? _originalHashTableSize;
        private readonly List<MpqFile> _originalFiles;
        private readonly List<MpqFile> _modifiedFiles;
        private readonly List<ulong> _removedFiles;

        public MpqArchiveBuilder()
        {
            _originalHashTableSize = null;
            _originalFiles = new List<MpqFile>();
            _modifiedFiles = new List<MpqFile>();
            _removedFiles = new List<ulong>();
        }

        public MpqArchiveBuilder(MpqArchive originalMpqArchive)
        {
            if (originalMpqArchive is null)
            {
                throw new ArgumentNullException(nameof(originalMpqArchive));
            }

            _originalHashTableSize = (ushort)originalMpqArchive.HashTable.Size;
            _originalFiles = new List<MpqFile>(originalMpqArchive.GetMpqFiles());
            _modifiedFiles = new List<MpqFile>();
            _removedFiles = new List<ulong>();
        }

        public IReadOnlyList<MpqFile> OriginalFiles => _originalFiles.AsReadOnly();

        public IReadOnlyList<MpqFile> ModifiedFiles => _modifiedFiles.AsReadOnly();

        public IReadOnlyList<ulong> RemovedFiles => _removedFiles.AsReadOnly();

        public void AddFile(MpqFile file)
        {
            AddFile(file, MpqFileFlags.Exists | MpqFileFlags.CompressedMulti);
        }

        public void AddFile(MpqFile file, MpqFileFlags targetFlags)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            file.TargetFlags = targetFlags;
            _modifiedFiles.Add(file);
        }

        public void RemoveFile(ulong hashedFileName)
        {
            _removedFiles.Add(hashedFileName);
        }

        public void RemoveFile(string fileName)
        {
            RemoveFile(fileName.GetStringHash());
        }

        public void RemoveFile(MpqArchive mpqArchive, int blockIndex)
        {
            foreach (var mpqHash in mpqArchive.HashTable._hashes)
            {
                if (mpqHash.BlockIndex == blockIndex)
                {
                    RemoveFile(mpqHash.Name);
                }
            }
        }

        public void RemoveFile(MpqArchive mpqArchive, MpqEntry mpqEntry)
        {
            var blockIndex = mpqArchive.BlockTable._entries.IndexOf(mpqEntry);
            if (blockIndex == -1)
            {
                throw new ArgumentException("The given mpq entry could not be found in the archive.", nameof(mpqEntry));
            }

            RemoveFile(mpqArchive, blockIndex);
        }

        public void SaveTo(string fileName)
        {
            using (var stream = FileProvider.CreateFileAndFolder(fileName))
            {
                SaveTo(stream);
            }
        }

        public void SaveTo(string fileName, MpqArchiveCreateOptions createOptions)
        {
            using (var stream = FileProvider.CreateFileAndFolder(fileName))
            {
                SaveTo(stream, createOptions);
            }
        }

        public void SaveTo(Stream stream, bool leaveOpen = false)
        {
            var createOptions = new MpqArchiveCreateOptions
            {
                HashTableSize = _originalHashTableSize,
                AttributesFlags = AttributesFlags.Crc32,
            };

            SaveTo(stream, createOptions, leaveOpen);
        }

        public void SaveTo(Stream stream, MpqArchiveCreateOptions createOptions, bool leaveOpen = false)
        {
            if (createOptions is null)
            {
                throw new ArgumentNullException(nameof(createOptions));
            }

            if (!createOptions.ListFileCreateMode.HasValue)
            {
                createOptions.ListFileCreateMode = _removedFiles.Contains(ListFile.FileName.GetStringHash()) ? MpqFileCreateMode.Prune : MpqFileCreateMode.Overwrite;
            }

            if (!createOptions.AttributesCreateMode.HasValue)
            {
                createOptions.AttributesCreateMode = _removedFiles.Contains(Attributes.FileName.GetStringHash()) ? MpqFileCreateMode.Prune : MpqFileCreateMode.Overwrite;
            }

            createOptions.HashTableSize ??= _originalHashTableSize;
            MpqArchive.Create(stream, GetMpqFiles().ToArray(), createOptions, leaveOpen).Dispose();
        }

        /// <inheritdoc/>
        public IEnumerator<MpqFile> GetEnumerator() => GetMpqFiles().GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetMpqFiles().GetEnumerator();

        protected virtual IEnumerable<MpqFile> GetMpqFiles()
        {
            return _modifiedFiles.Concat(_originalFiles).Where(mpqFile => !_removedFiles.Contains(mpqFile.Name));
        }
    }
}