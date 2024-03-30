using BetterTriggers.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Tests
{
    public class ImageTest : TestBase
    {
        static string sourceFolder = System.IO.Directory.GetCurrentDirectory() + "/Resources/Images";

        [DataTestMethod]
        [DataRow("FrostmourneNew.blp")]
        [DataRow("HelmOfDom.blp")]
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
