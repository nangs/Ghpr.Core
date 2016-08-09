﻿using System;
using System.Collections.Generic;
using System.IO;
using Ghpr.Core.Common;
using Ghpr.Core.EmbeddedResources;
using Ghpr.Core.Extensions;
using Ghpr.Core.Helpers;
using Ghpr.Core.Interfaces;
using Ghpr.Core.Utils;

namespace Ghpr.Core
{
    public class Reporter
    {
        private IRun _currentRun;
        private List<ITestRun> _currentRunningTests;

        private static readonly ResourceExtractor Extractor = new ResourceExtractor(OutputPath);
        
        public static string OutputPath => Properties.Settings.Default.OutputPath;
        public static bool TakeScreenshotAfterFail => Properties.Settings.Default.TakeScreenshotAfterFail;
        public static string Sprint => Properties.Settings.Default.Sprint;
        public static string RunName => Properties.Settings.Default.RunName;
        public static bool RealTimeGeneration => Properties.Settings.Default.RealTime;

        public const string TestsFolderName = "tests";
        public const string RunsFolderName = "runs";

        private void SetUp()
        {
            ActionHelper.SafeAction(() =>
            {
                _currentRun = new Run(Guid.NewGuid())
                {
                    TestRunFiles = new List<string>(),
                    RunSummary = new RunSummary()
                };
                _currentRunningTests = new List<ITestRun>();

                _currentRun.Name = RunName;
                _currentRun.Sprint = Sprint;
                Extractor.ExtractReportBase();
                _currentRun.RunInfo.Start = DateTime.Now;
            });
        }

        private void UpdateCurrentRunSummary(ITestRun finalTest)
        {
            ActionHelper.SafeAction(() =>
            {
                _currentRun.RunSummary = _currentRun.RunSummary.Update(finalTest);
            });
        }

        private void GenerateReport()
        {
            ActionHelper.SafeAction(() =>
            {
                _currentRun.RunInfo.Finish = DateTime.Now;
                var runsPath = Path.Combine(OutputPath, RunsFolderName);
                _currentRun.Save(runsPath);
                RunsHelper.SaveCurrentRunInfo(runsPath, _currentRun.RunInfo);
            });
        }

        public void RunStarted()
        {
            SetUp();
        }

        public void RunFinished()
        {
            GenerateReport();
        }
        
        public void TestStarted(ITestRun testRun)
        {
            ActionHelper.SafeAction(() =>
            {
                _currentRunningTests.Add(testRun);
            });
        }

        public void TestFinished(ITestRun testRun)
        {
            ActionHelper.SafeAction(() =>
            {
                _currentRun.RunSummary.Total++;

                var finishDateTime = DateTime.Now;
                var currentTest = _currentRunningTests.GetTest(testRun);
                var finalTest = testRun.Update(currentTest);
                var testGuid = finalTest.TestInfo.Guid.ToString();
                var testsPath = Path.Combine(OutputPath, TestsFolderName);
                var testPath = Path.Combine(testsPath, testGuid);
                var fileName = finishDateTime.GetTestName();

                UpdateCurrentRunSummary(finalTest);

                finalTest.TestInfo.FileName = fileName;
                finalTest.RunGuid = _currentRun.RunInfo.Guid;
                finalTest.TestInfo.Start = finalTest.TestInfo.Start.Equals(default(DateTime)) ? finishDateTime : finalTest.TestInfo.Start;
                finalTest.TestInfo.Finish = finalTest.TestInfo.Finish.Equals(default(DateTime)) ? finishDateTime : finalTest.TestInfo.Finish;
                finalTest
                    .TakeScreenshot(testPath, TakeScreenshotAfterFail)
                    .Save(testPath, fileName);
                _currentRunningTests.Remove(currentTest);
                _currentRun.TestRunFiles.Add($"{testGuid}\\{fileName}");
                Extractor.ExtractTestPage(testsPath);

                TestRunsHelper.SaveCurrentTestInfo(testPath, finalTest.TestInfo);

                if (RealTimeGeneration)
                {
                    GenerateReport();
                }
            });
        }
    }
}
