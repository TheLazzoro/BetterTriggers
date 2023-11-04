using BetterTriggers.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tests
{
    [TestClass]
    public class ImageTest
    {
        static string sourceFolder = System.IO.Directory.GetCurrentDirectory() + "/Resources/Images";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING IMAGE TESTS");
            Console.WriteLine("-----------");
            Console.WriteLine("");
        }

        [DataTestMethod]
        [DataRow("FrostmourneNew.blp")]
        [DataRow("HelmOfDom.blp")]
        [DataRow("LichKing.blp")]
        [DataRow("LichKing.blp")]
        [DataRow("BTNspell_arcane_manatap.dds")]
        [DataRow("previewFirelord.dds")]
        [DataRow("HeroAbilities.tga")]
        public void ImageReadTest(string fileName)
        {
            string fullPath = Path.Combine(sourceFolder, fileName);
            using (Stream s = new FileStream(fullPath, FileMode.Open))
            {
                Images.ReadImage(s);
            }
        }
    }
}
