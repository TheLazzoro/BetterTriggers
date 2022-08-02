// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#if false
namespace Bridge.ClientTest.IO
{
    [Category(Constants.MODULE_IO)]
    [TestFixture(TestNameFormat = "StreamReader_ctorTests - {0}")]
    public class StreamReader_ctorTests
    {
        [Test]
        public static void StreamReaderNullPath()
        {
            Assert.Throws<ArgumentNullException>(() => new StreamReader((Stream)null, true));
        }
        [Test]
        public static void InputStreamClosed()
        {
            var ms2 = new MemoryStream();
            ms2.Dispose();

            Assert.Throws<ArgumentException>(() => new StreamReader(ms2, false));
        }

        [Test]
        public static void CreationFromMemoryStreamWithEncodingFalse()
        {
            var ms2 = new MemoryStream();
            ms2.Write(new byte[] { 65, 66, 67, 68 }, 0, 4);
            ms2.Position = 0;
            var sr2 = new StreamReader(ms2, false);

            Assert.AreEqual("ABCD", sr2.ReadToEnd());
            sr2.Dispose();
        }

        [Test]
        public static void CreationFromMemoryStreamWithEncodingTrue()
        {
            var ms2 = new MemoryStream();
            ms2.Write(new byte[] { 65, 66, 67, 68 }, 0, 4);
            ms2.Position = 0;
            var sr2 = new StreamReader(ms2, false);

            Assert.AreEqual("ABCD", sr2.ReadToEnd());
            sr2.Dispose();
        }
    }
}
#endif
