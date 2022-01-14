using System;

namespace DsmSuite.Analyzer.Cpp.Settings
{
    [Serializable]
    public class TransformationModuleMergeRule
    {
        public string From { get; set; }

        public string To { get; set; }
    }
}
