﻿///<reference path="./../entities/TestRun.ts"/>
///<reference path="./../../../dto/TestRunDto.ts"/>
///<reference path="./ItemInfoDtoMapper.ts"/>
///<reference path="./SimpleItemInfoDtoMapper.ts"/>

class TestRunDtoMapper {
    static map(testRun: TestRun): TestRunDto {
        let eventDtos = new Array<TestEventDto>(testRun.events.length);
        let screenshotDtos = new Array<SimpleItemInfoDto>(testRun.screenshots.length);
        let testDataDtos = new Array<TestDataDto>(testRun.testData.length);

        for (let i = 0; i < testRun.events.length; i++) {
            let event = testRun.events[i];
            let eventDto = new TestEventDto();
            eventDto.name = event.name;
            eventDto.started = event.started;
            eventDto.finished = event.finished;
            eventDto.eventInfo = SimpleItemInfoDtoMapper.map(event.eventInfo);
            eventDtos[i] = eventDto;
        }

        for (let i = 0; i < testRun.screenshots.length; i++) {
            screenshotDtos[i] = SimpleItemInfoDtoMapper.map(testRun.screenshots[i]);
        }

        for (let i = 0; i < testRun.testData.length; i++) {
            let testData = testRun.testData[i];
            let testDataDto = new TestDataDto();
            testDataDto.actual = testData.actual;
            testDataDto.expected = testData.expected;
            testDataDto.comment = testData.comment;
            testDataDto.testDataInfo = SimpleItemInfoDtoMapper.map(testData.testDataInfo);
            testDataDtos[i] = testDataDto;
        }

        let testRunDto = new TestRunDto();
        testRunDto.name = testRun.name;
        testRunDto.categories = testRun.categories;
        testRunDto.description = testRun.description;
        testRunDto.duration = testRun.duration;
        testRunDto.events = eventDtos;
        testRunDto.fullName = testRun.fullName;
        testRunDto.output = SimpleItemInfoDtoMapper.map(testRun.output);
        testRunDto.priority = testRun.priority;
        testRunDto.result = testRun.result;
        testRunDto.testInfo = ItemInfoDtoMapper.map(testRun.testInfo);
        testRunDto.testMessage = testRun.testMessage;
        testRunDto.testStackTrace = testRun.testStackTrace;
        testRunDto.runGuid = testRun.runGuid;
        testRunDto.testType = testRun.testType;
        testRunDto.screenshots = screenshotDtos;
        testRunDto.testData = testDataDtos;
        
        return testRunDto;
    }
}
