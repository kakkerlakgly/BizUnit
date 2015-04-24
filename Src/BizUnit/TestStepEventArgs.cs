using System;

namespace BizUnit
{
    /// <summary>
    /// </summary>
    public class TestStepEventArgs : EventArgs
    {
        private readonly TestStage _stage;
        private readonly string _testCaseName;
        private readonly string _testStepTypeName;

        internal TestStepEventArgs(TestStage stage, string testCaseName, string testStepTypeName)
        {
            _stage = stage;
            _testCaseName = testCaseName;
            _testStepTypeName = testStepTypeName;
        }

        /// <summary>
        /// </summary>
        public TestStage Stage
        {
            get { return _stage; }
        }

        /// <summary>
        /// </summary>
        public string TestCaseName
        {
            get { return _testCaseName; }
        }

        /// <summary>
        /// </summary>
        public string TestStepTypeName
        {
            get { return _testStepTypeName; }
        }
    }
}