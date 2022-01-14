using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Cpp.Test.Util
{
    class TestData
    {
        public static string RootDirectory
        {
            get
            {
                return FilePath.ResolveRelativePath(@"..\..\..\..\DsmSuite.Analyzer.Cpp.Test.Data");
            }
        }
    }
}
