using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.DotNet.Test.Util
{
    class TestData
    {
        public static string RootDirectory
        {
            get
            {
                string path = FilePath.ResolveRelativePath(@"");
                return path.Replace(@"DsmSuite.Analyzer.DotNet.Test", "DsmSuite.Analyzer.DotNet.Test.Data");
            }
        }
    }
}
