﻿using System;
using System.Collections.Generic;

namespace Ghpr.Core.Common
{
    public class RunDto
    {
        public List<string> TestRunFiles { get; set; }
        public ItemInfoDto RunInfo { get; set; }
        public string Sprint { get; set; }
        public string Name { get; set; }
        public RunSummaryDto RunSummary { get; set; }

        public RunDto(ReporterSettings reporterSettings, DateTime startDateTime)
        {
            RunInfo = new ItemInfoDto
            {
                Guid = reporterSettings.RunGuid.Equals("") || reporterSettings.RunGuid.Equals("null")
                    ? Guid.NewGuid() : Guid.Parse(reporterSettings.RunGuid),
                Start = startDateTime
            };
            Name = reporterSettings.RunName;
            Sprint = reporterSettings.Sprint;
            TestRunFiles = new List<string>();
            RunSummary = new RunSummaryDto();
        }
    }
}