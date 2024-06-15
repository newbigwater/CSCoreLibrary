using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using _000.Common;
using _000.Common._04._Struct;

namespace _999.UnitTest._000._Common
{
    [TestClass]
    public partial class _000_Common
    {
        [TestCategory("[CATEGORY] [000_Common] Common")]
        [TestMethod("[File System] GetDirectorySize() Check")]
        public void UnitTest_GetDirectorySize()
        {
            long size = Common.GetDirectorySize($@"..\UnitTest\DirectorySizeTest");
            Assert.AreEqual(1202, size);
        }

        [TestCategory("[CATEGORY] [000_Common] Common")]
        [TestMethod("[File System] GetDirectorySize() Check")]
        public void UnitTest_GetDriveSize()
        {
            ExtDriveInfo drvSizeInfoApdeptor = Common.GetDriveSize(System.IO.Directory.GetCurrentDirectory());
            if (drvSizeInfoApdeptor.TotalGBSize == 0)
                throw new Exception($"'{drvSizeInfoApdeptor.Name}' Drive is zero in size.");
        }
    }
}
