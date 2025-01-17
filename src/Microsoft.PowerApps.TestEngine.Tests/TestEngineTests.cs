﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.PowerApps.TestEngine.Config;
using Microsoft.PowerApps.TestEngine.Reporting;
using Microsoft.PowerApps.TestEngine.System;
using Microsoft.PowerApps.TestEngine.Tests.Helpers;
using Moq;
using Xunit;

namespace Microsoft.PowerApps.TestEngine.Tests
{
    public class TestEngineTests
    {
        private Mock<ITestState> MockState;
        private Mock<ITestReporter> MockTestReporter;
        private Mock<IFileSystem> MockFileSystem;
        private Mock<ISingleTestRunner> MockSingleTestRunner;
        private IServiceProvider ServiceProvider;
        private Mock<ILoggerFactory> MockLoggerFactory;
        private Mock<ILogger> MockLogger;

        public TestEngineTests()
        {
            MockState = new Mock<ITestState>(MockBehavior.Strict);
            MockTestReporter = new Mock<ITestReporter>(MockBehavior.Strict);
            MockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            MockSingleTestRunner = new Mock<ISingleTestRunner>(MockBehavior.Strict);
            ServiceProvider = new ServiceCollection()
                            .AddSingleton(MockSingleTestRunner.Object)
                            .BuildServiceProvider();
            MockLoggerFactory = new Mock<ILoggerFactory>(MockBehavior.Strict);
            MockLogger = new Mock<ILogger>(MockBehavior.Strict);
        }

        [Fact]
        public async Task TestEngineWithDefaultParamsTest()
        {
            var testSettings = new TestSettings()
            {
                Locale = "en-US",
                WorkerCount = 2,
                BrowserConfigurations = new List<BrowserConfiguration>()
                {
                    new BrowserConfiguration()
                    {
                        Browser = "Chromium"
                    }
                }
            };
            var testSuiteDefinition = new TestSuiteDefinition()
            {
                TestSuiteName = "Test1",
                TestSuiteDescription = "First test",
                AppLogicalName = "logicalAppName1",
                Persona = "User1",
                TestCases = new List<TestCase>()
                {
                    new TestCase
                    {
                        TestCaseName = "Test Case Name",
                        TestCaseDescription = "Test Case Description",
                        TestSteps = "Assert(1 + 1 = 2, \"1 + 1 should be 2 \")"
                    }
                }
            };
            var testConfigFile = "C:\\testPlan.fx.yaml";
            var environmentId = "defaultEnviroment";
            var tenantId = "tenantId";
            var testRunId = Guid.NewGuid().ToString();
            var expectedOutputDirectory = "TestOutput";
            var testRunDirectory = Path.Combine(expectedOutputDirectory, testRunId.Substring(0, 6));
            var domain = "apps.powerapps.com";

            var expectedTestReportPath = "C:\\test.trx";

            SetupMocks(expectedOutputDirectory, testSettings, testSuiteDefinition, testRunId, expectedTestReportPath);

            var testEngine = new TestEngine(MockState.Object, ServiceProvider, MockTestReporter.Object, MockFileSystem.Object, MockLoggerFactory.Object);
            var testReportPath = await testEngine.RunTestAsync(testConfigFile, environmentId, tenantId, "", domain, "");

            Assert.Equal(expectedTestReportPath, testReportPath);

            Verify(testConfigFile, environmentId, tenantId, domain, "", expectedOutputDirectory, testRunId, testRunDirectory, testSuiteDefinition, testSettings);
        }

        [Fact]
        public async Task TestEngineWithInvalidLocaleTest()
        {
            var testSettings = new TestSettings()
            {
                Locale = "de=DEE",     // in case user enters a typo
                WorkerCount = 2,
                BrowserConfigurations = new List<BrowserConfiguration>()
                {
                    new BrowserConfiguration()
                    {
                        Browser = "Chromium"
                    }
                }
            };
            var testSuiteDefinition = new TestSuiteDefinition()
            {
                TestSuiteName = "Test1",
                TestSuiteDescription = "First test",
                AppLogicalName = "logicalAppName1",
                Persona = "User1",
                TestCases = new List<TestCase>()
                {
                    new TestCase
                    {
                        TestCaseName = "Test Case Name",
                        TestCaseDescription = "Test Case Description",
                        TestSteps = "Assert(1 + 1 = 2, \"1 + 1 should be 2 \")"
                    }
                }
            };
            var testConfigFile = "C:\\testPlan.fx.yaml";
            var environmentId = "defaultEnviroment";
            var tenantId = "tenantId";
            var testRunId = Guid.NewGuid().ToString();
            var expectedOutputDirectory = "TestOutput";
            var testRunDirectory = Path.Combine(expectedOutputDirectory, testRunId.Substring(0, 6));
            var domain = "apps.powerapps.com";

            var expectedTestReportPath = "C:\\test.trx";

            SetupMocks(expectedOutputDirectory, testSettings, testSuiteDefinition, testRunId, expectedTestReportPath);

            var testEngine = new TestEngine(MockState.Object, ServiceProvider, MockTestReporter.Object, MockFileSystem.Object, MockLoggerFactory.Object);

            await Assert.ThrowsAsync<CultureNotFoundException>(() => testEngine.RunTestAsync(testConfigFile, environmentId, tenantId, "", domain, ""));
        }

        [Fact]
        public async Task TestEngineWithUnspecifiedLocaleShowsWarning()
        {
            var testSettings = new TestSettings()
            {
                WorkerCount = 2,
                BrowserConfigurations = new List<BrowserConfiguration>()
                {
                    new BrowserConfiguration()
                    {
                        Browser = "Chromium"
                    }
                }
            };
            var testSuiteDefinition = new TestSuiteDefinition()
            {
                TestSuiteName = "Test1",
                TestSuiteDescription = "First test",
                AppLogicalName = "logicalAppName1",
                Persona = "User1",
                TestCases = new List<TestCase>()
                {
                    new TestCase
                    {
                        TestCaseName = "Test Case Name",
                        TestCaseDescription = "Test Case Description",
                        TestSteps = "Assert(1 + 1 = 2, \"1 + 1 should be 2 \")"
                    }
                }
            };
            var testConfigFile = "C:\\testPlan.fx.yaml";
            var environmentId = "defaultEnviroment";
            var tenantId = "tenantId";
            var testRunId = Guid.NewGuid().ToString();
            var expectedOutputDirectory = "TestOutput";
            var testRunDirectory = Path.Combine(expectedOutputDirectory, testRunId.Substring(0, 6));
            var domain = "apps.powerapps.com";

            var expectedTestReportPath = "C:\\test.trx";

            SetupMocks(expectedOutputDirectory, testSettings, testSuiteDefinition, testRunId, expectedTestReportPath);

            var testEngine = new TestEngine(MockState.Object, ServiceProvider, MockTestReporter.Object, MockFileSystem.Object, MockLoggerFactory.Object);
            var testReportPath = await testEngine.RunTestAsync(testConfigFile, environmentId, tenantId, "", domain, "");

            Assert.Equal(expectedTestReportPath, testReportPath);
            LoggingTestHelper.VerifyLogging(MockLogger, $"Locale property not specified in testSettings. Using current system locale: {CultureInfo.CurrentCulture.Name}", LogLevel.Warning, Times.Once());

            Verify(testConfigFile, environmentId, tenantId, domain, "", expectedOutputDirectory, testRunId, testRunDirectory, testSuiteDefinition, testSettings);
        }

        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public async Task TestEngineTest(string outputDirectory, string domain, TestSettings testSettings, TestSuiteDefinition testSuiteDefinition)
        {
            var testConfigFile = "C:\\testPlan.fx.yaml";
            var environmentId = "defaultEnviroment";
            var tenantId = "tenantId";
            var testRunId = Guid.NewGuid().ToString();
            var expectedOutputDirectory = outputDirectory;
            if (string.IsNullOrEmpty(expectedOutputDirectory))
            {
                expectedOutputDirectory = "TestOutput";
            }
            var testRunDirectory = Path.Combine(expectedOutputDirectory, testRunId.Substring(0, 6));

            if (string.IsNullOrEmpty(domain))
            {
                domain = "apps.powerapps.com";
            }

            var expectedTestReportPath = "C:\\test.trx";

            SetupMocks(expectedOutputDirectory, testSettings, testSuiteDefinition, testRunId, expectedTestReportPath);

            var testEngine = new TestEngine(MockState.Object, ServiceProvider, MockTestReporter.Object, MockFileSystem.Object, MockLoggerFactory.Object);
            var testReportPath = await testEngine.RunTestAsync(testConfigFile, environmentId, tenantId, outputDirectory, domain, "");

            Assert.Equal(expectedTestReportPath, testReportPath);

            Verify(testConfigFile, environmentId, tenantId, domain, "", expectedOutputDirectory, testRunId, testRunDirectory, testSuiteDefinition, testSettings);
        }

        private void SetupMocks(string outputDirectory, TestSettings testSettings, TestSuiteDefinition testSuiteDefinition, string testRunId, string testReportPath)
        {
            MockState.Setup(x => x.ParseAndSetTestState(It.IsAny<string>()));
            MockState.Setup(x => x.SetEnvironment(It.IsAny<string>()));
            MockState.Setup(x => x.SetTenant(It.IsAny<string>()));
            MockState.Setup(x => x.SetDomain(It.IsAny<string>()));
            MockState.Setup(x => x.SetOutputDirectory(It.IsAny<string>()));
            MockState.Setup(x => x.GetOutputDirectory()).Returns(outputDirectory);
            MockState.Setup(x => x.GetTestSettings()).Returns(testSettings);
            MockState.Setup(x => x.GetTestSuiteDefinition()).Returns(testSuiteDefinition);
            MockState.Setup(x => x.GetWorkerCount()).Returns(testSettings.WorkerCount);

            MockTestReporter.Setup(x => x.CreateTestRun(It.IsAny<string>(), It.IsAny<string>())).Returns(testRunId);
            MockTestReporter.Setup(x => x.StartTestRun(It.IsAny<string>()));
            MockTestReporter.Setup(x => x.EndTestRun(It.IsAny<string>()));
            MockTestReporter.Setup(x => x.GenerateTestReport(It.IsAny<string>(), It.IsAny<string>())).Returns(testReportPath);

            MockFileSystem.Setup(x => x.CreateDirectory(It.IsAny<string>()));

            MockSingleTestRunner.Setup(x => x.RunTestAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TestSuiteDefinition>(), It.IsAny<BrowserConfiguration>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CultureInfo>())).Returns(Task.CompletedTask);

            MockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(MockLogger.Object);
            LoggingTestHelper.SetupMock(MockLogger);
        }


        private void Verify(string testConfigFile, string environmentId, string tenantId, string domain, string queryParams,
            string outputDirectory, string testRunId, string testRunDirectory, TestSuiteDefinition testSuiteDefinition, TestSettings testSettings)
        {
            MockState.Verify(x => x.ParseAndSetTestState(testConfigFile), Times.Once());
            MockState.Verify(x => x.SetEnvironment(environmentId), Times.Once());
            MockState.Verify(x => x.SetTenant(tenantId), Times.Once());
            MockState.Verify(x => x.SetDomain(domain), Times.Once());
            MockState.Verify(x => x.SetOutputDirectory(outputDirectory), Times.Once());

            MockTestReporter.Verify(x => x.CreateTestRun("Power Fx Test Runner", "User"), Times.Once());
            MockTestReporter.Verify(x => x.StartTestRun(testRunId), Times.Once());

            MockFileSystem.Verify(x => x.CreateDirectory(testRunDirectory), Times.Once());

            var locale = string.IsNullOrEmpty(testSettings.Locale) ? CultureInfo.CurrentCulture : new CultureInfo(testSettings.Locale);

            foreach (var browserConfig in testSettings.BrowserConfigurations)
            {
                MockSingleTestRunner.Verify(x => x.RunTestAsync(testRunId, testRunDirectory, testSuiteDefinition, browserConfig, domain, queryParams, locale), Times.Once());
            }

            MockTestReporter.Verify(x => x.EndTestRun(testRunId), Times.Once());
            MockTestReporter.Verify(x => x.GenerateTestReport(testRunId, testRunDirectory), Times.Once());
        }

        [Theory]
        [InlineData("", "defaultEnvironment", "tenantId")]
        [InlineData("C:\\testPlan.fx.yaml", "", "tenantId")]
        [InlineData("C:\\testPlan.fx.yaml", "defaultEnvironment", "")]
        public async Task TestEngineThrowsOnNullArguments(string testConfigFile, string environmentId, string tenantId)
        {
            MockTestReporter.Setup(x => x.CreateTestRun(It.IsAny<string>(), It.IsAny<string>())).Returns(Guid.NewGuid().ToString());
            MockTestReporter.Setup(x => x.StartTestRun(It.IsAny<string>()));
            MockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(MockLogger.Object);
            LoggingTestHelper.SetupMock(MockLogger);
            MockState.Setup(x => x.SetOutputDirectory(It.IsAny<string>()));
            MockState.Setup(x => x.GetOutputDirectory()).Returns("MockOutputDirectory");
            MockFileSystem.Setup(x => x.CreateDirectory(It.IsAny<string>()));
            var testEngine = new TestEngine(MockState.Object, ServiceProvider, MockTestReporter.Object, MockFileSystem.Object, MockLoggerFactory.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testEngine.RunTestAsync(testConfigFile, environmentId, tenantId, "", "", ""));
        }

        class TestDataGenerator : TheoryData<string, string, TestSettings, TestSuiteDefinition>
        {
            public TestDataGenerator()
            {
                // Simple test
                Add("C:\\testResults",
                    "GCC",
                    new TestSettings()
                    {
                        Locale = string.Empty,
                        BrowserConfigurations = new List<BrowserConfiguration>()
                        {
                            new BrowserConfiguration()
                            {
                                Browser = "Chromium"
                            }
                        }
                    },
                    new TestSuiteDefinition()
                    {
                        TestSuiteName = "Test1",
                        TestSuiteDescription = "First test",
                        AppLogicalName = "logicalAppName1",
                        Persona = "User1",
                        TestCases = new List<TestCase>()
                        {
                            new TestCase
                            {
                                TestCaseName = "Test Case Name",
                                TestCaseDescription = "Test Case Description",
                                TestSteps = "Assert(1 + 1 = 2, \"1 + 1 should be 2 \")"
                            }
                        }
                    });

                // Simple test with null params
                Add(null,
                    null,
                    new TestSettings()
                    {
                        Locale = string.Empty,
                        BrowserConfigurations = new List<BrowserConfiguration>()
                        {
                            new BrowserConfiguration()
                            {
                                Browser = "Chromium"
                            }
                        }
                    },
                    new TestSuiteDefinition()
                    {
                        TestSuiteName = "Test1",
                        TestSuiteDescription = "First test",
                        AppLogicalName = "logicalAppName1",
                        Persona = "User1",
                        TestCases = new List<TestCase>()
                        {
                            new TestCase
                            {
                                TestCaseName = "Test Case Name",
                                TestCaseDescription = "Test Case Description",
                                TestSteps = "Assert(1 + 1 = 2, \"1 + 1 should be 2 \")"
                            }
                        }
                    });

                // Simple test in en-US locale (this should be like every other test)
                // For the rest of the tests where Locale = string.Empty, CurrentCulture should be used
                // and the test should pass
                Add("C:\\testResults",
                    "GCC",
                    new TestSettings()
                    {
                        Locale = "en-US",
                        BrowserConfigurations = new List<BrowserConfiguration>()
                        {
                            new BrowserConfiguration()
                            {
                                Browser = "Chromium"
                            }
                        }
                    },
                    new TestSuiteDefinition()
                    {
                        TestSuiteName = "Test1",
                        TestSuiteDescription = "First test",
                        AppLogicalName = "logicalAppName1",
                        Persona = "User1",
                        TestCases = new List<TestCase>()
                        {
                            new TestCase
                            {
                                TestCaseName = "Test Case Name",
                                TestCaseDescription = "Test Case Description",
                                TestSteps = "Assert(1 + 1 = 2, \"1 + 1 should be 2 \")"
                            }
                        }
                    });

                // Simple test with empty string params
                Add("",
                    "",
                    new TestSettings()
                    {
                        Locale = string.Empty,
                        BrowserConfigurations = new List<BrowserConfiguration>()
                        {
                            new BrowserConfiguration()
                            {
                                Browser = "Chromium"
                            }
                        }
                    },
                    new TestSuiteDefinition()
                    {
                        TestSuiteName = "Test1",
                        TestSuiteDescription = "First test",
                        AppLogicalName = "logicalAppName1",
                        Persona = "User1",
                        TestCases = new List<TestCase>()
                        {
                            new TestCase
                            {
                                TestCaseName = "Test Case Name",
                                TestCaseDescription = "Test Case Description",
                                TestSteps = "Assert(1 + 1 = 2, \"1 + 1 should be 2 \")"
                            }
                        }
                    });

                // Simple test in a different locale
                Add("C:\\testResults",
                    "GCC",
                    new TestSettings()
                    {
                        Locale = "de-DE",
                        BrowserConfigurations = new List<BrowserConfiguration>()
                        {
                            new BrowserConfiguration()
                            {
                                Browser = "Chromium"
                            }
                        }
                    },
                    new TestSuiteDefinition()
                    {
                        TestSuiteName = "Test1",
                        TestSuiteDescription = "First test",
                        AppLogicalName = "logicalAppName1",
                        Persona = "User1",
                        TestCases = new List<TestCase>()
                        {
                            new TestCase
                            {
                                TestCaseName = "Test Case Name",
                                TestCaseDescription = "Test Case Description",
                                TestSteps = "Assert(1 + 1 = 2; \"1 + 1 should be 2 \")"
                            }
                        }
                    });

                // Multiple browsers
                Add("C:\\testResults",
                    "Prod",
                    new TestSettings()
                    {
                        Locale = string.Empty,
                        BrowserConfigurations = new List<BrowserConfiguration>()
                        {
                            new BrowserConfiguration()
                            {
                                Browser = "Chromium"
                            },
                            new BrowserConfiguration()
                            {
                                Browser = "Firefox"
                            },
                            new BrowserConfiguration()
                            {
                                Browser = "Chromium",
                                Device = "Pixel 2"
                            }
                        }
                    },
                    new TestSuiteDefinition()
                    {
                        TestSuiteName = "Test1",
                        TestSuiteDescription = "First test",
                        AppLogicalName = "logicalAppName1",
                        Persona = "User1",
                        TestCases = new List<TestCase>()
                        {
                            new TestCase
                            {
                                TestCaseName = "Test Case Name",
                                TestCaseDescription = "Test Case Description",
                                TestSteps = "Assert(1 + 1 = 2, \"1 + 1 should be 2 \")"
                            }
                        }
                    });

                // Multiple tests
                Add("C:\\testResults",
                    "Prod",
                    new TestSettings()
                    {
                        Locale = string.Empty,
                        BrowserConfigurations = new List<BrowserConfiguration>()
                        {
                            new BrowserConfiguration()
                            {
                                Browser = "Chromium"
                            }
                        }
                    },
                    new TestSuiteDefinition()
                    {
                        TestSuiteName = "Test1",
                        TestSuiteDescription = "First test",
                        AppLogicalName = "logicalAppName1",
                        Persona = "User1",
                        TestCases = new List<TestCase>()
                        {
                            new TestCase
                            {
                                TestCaseName = "Test1",
                                TestCaseDescription = "First test",
                                TestSteps = "Assert(1 + 1 = 2, \"1 + 1 should be 2 \")"
                            },
                            new TestCase
                            {
                                TestCaseName = "Test2",
                                TestCaseDescription = "Second test",
                                TestSteps = "Assert(2 + 1 = 3, \"2 + 1 should be 3 \")"
                            }
                        }
                    });

                // Multiple tests and browsers
                Add("C:\\testResults",
                    "Prod",
                    new TestSettings()
                    {
                        Locale = string.Empty,
                        BrowserConfigurations = new List<BrowserConfiguration>()
                        {
                            new BrowserConfiguration()
                            {
                                Browser = "Chromium"
                            },
                            new BrowserConfiguration()
                            {
                                Browser = "Firefox"
                            },
                            new BrowserConfiguration()
                            {
                                Browser = "Chromium",
                                Device = "Pixel 2"
                            }
                        }
                    },
                    new TestSuiteDefinition()
                    {
                        TestSuiteName = "Test1",
                        TestSuiteDescription = "First test",
                        AppLogicalName = "logicalAppName1",
                        Persona = "User1",
                        TestCases = new List<TestCase>()
                        {
                            new TestCase
                            {
                                TestCaseName = "Test1",
                                TestCaseDescription = "First test",
                                TestSteps = "Assert(1 + 1 = 2, \"1 + 1 should be 2 \")"
                            },
                            new TestCase
                            {
                                TestCaseName = "Test2",
                                TestCaseDescription = "Second test",
                                TestSteps = "Assert(2 + 1 = 3, \"2 + 1 should be 3 \")"
                            }
                        }
                    });
            }
        }
    }
}
