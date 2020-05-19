﻿using Backtrace.Unity.Model;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.TestTools;

namespace Backtrace.Unity.Tests.Runtime
{
    public class BacktraceReportTests
    {
        private readonly Exception exception = new DivideByZeroException("fake exception message");
        private readonly Dictionary<string, string> reportAttributes = new Dictionary<string, string>()
            {
                { "test_attribute", "test_attribute_value" },
                { "temporary_attribute", "123" },
                { "temporary_attribute_bool", "true"}
            };
        private readonly List<string> attachemnts = new List<string>() { "path", "path2" };

        [Test]
        public void TestReportCreation_CreateCorrectMessageReport_ShouldCreateValidaReport()
        {
            Assert.DoesNotThrow(() => new BacktraceReport("message"));
            Assert.DoesNotThrow(() => new BacktraceReport("message", new Dictionary<string, string>(), new List<string>()));
            Assert.DoesNotThrow(() => new BacktraceReport("message", attachmentPaths: attachemnts));

        }


        [Test]
        public void TestReportCreation_CreateCorrectExceptionReport_ShouldCreateValidaReport()
        {
            var exception = new FileNotFoundException();
            Assert.DoesNotThrow(() => new BacktraceReport(exception));
            Assert.DoesNotThrow(() => new BacktraceReport(exception, new Dictionary<string, string>(), new List<string>()));
            Assert.DoesNotThrow(() => new BacktraceReport(exception, attachmentPaths: attachemnts));

        }

        [Test]
        public void TestReportStackTrace_ShouldGenerateStackTraceForExceptionReport_ExceptionReportHasStackTrace()
        {
            //simulate real exception to generate an exception with stack trace.
            Exception exception = null;
            try
            {
                var arr = new List<int>() { 1, 2, 3, 4 };
                arr.ElementAt(arr.Count + 1);
            }
            catch (Exception e)
            {
                exception = e;
            }


            var report = new BacktraceReport(
              exception: exception,
              attributes: reportAttributes,
              attachmentPaths: attachemnts);
            Assert.IsTrue(report.DiagnosticStack.Any());
        }


        [Test]
        public void TestReportStackTrace_ShouldGenerateStackTraceForMessageReport_MessageReportHasStackTrace()
        {
            var report = new BacktraceReport(
              message: "foo",
              attributes: reportAttributes,
              attachmentPaths: attachemnts);
            Assert.IsTrue(report.DiagnosticStack.Any());
        }

        [Test]
        public void TestReportClassifier_ShouldntSetClassifier_MessageReportClassifierShouldBeEmpty()
        {
            var report = new BacktraceReport(
              message: "foo",
              attributes: reportAttributes,
              attachmentPaths: attachemnts);
            Assert.IsFalse(report.Classifier.Any());
        }


        [Test]
        public void TestReportClassifier_ShouldSetErrorClassifier_SetCorrectExceptionReportClassifier()
        {
            var exception = new ArgumentException(string.Empty);
            var report = new BacktraceReport(
              exception: exception,
              attributes: reportAttributes,
              attachmentPaths: attachemnts);
            Assert.AreEqual(report.Classifier, exception.GetType().Name);
        }

        [Test]
        public void TestReportClassifier_ShouldSetErrorClassifierBasedOnUnhandledExceptionMessage_SetCorrectExceptionReportClassifier()
        {
            string expectedExceptionName = "ArgumentException";
            var exception = new BacktraceUnhandledException(expectedExceptionName, string.Empty);
            var report = new BacktraceReport(
              exception: exception,
              attributes: reportAttributes,
              attachmentPaths: attachemnts);
            Assert.AreEqual(expectedExceptionName, report.Classifier);
        }
    }
}