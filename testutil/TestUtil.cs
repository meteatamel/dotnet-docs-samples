﻿/*
 * Copyright (c) 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;

namespace GoogleCloudSamples
{
    public class RetryRobot
    {
        public int FirstRetryDelayMs { get; set; } = 1000;
        public float DelayMultiplier { get; set; } = 2;
        public int MaxTryCount { get; set; } = 6;
        public IEnumerable<Type> RetryWhenExceptions { get; set; } = new Type[0];
        public Func<Exception, bool> ShouldRetry { get; set; }

        /// <summary>
        /// Retry action when assertion fails.
        /// </summary>
        /// <param name="func"></param>
        public T Eventually<T>(Func<T> func)
        {
            int delayMs = FirstRetryDelayMs;
            for (int i = 0; ; ++i)
            {
                try
                {
                    return func();
                }
                catch (Exception e)
                when (ShouldCatch(e) && i < MaxTryCount)
                {
                    Thread.Sleep(delayMs);
                    delayMs *= (int)DelayMultiplier;
                }
            }
        }

        private bool ShouldCatch(Exception e)
        {
            if (ShouldRetry != null)
                return ShouldRetry(e);
            foreach (var exceptionType in RetryWhenExceptions)
            {
                if (exceptionType.IsAssignableFrom(e.GetType()))
                    return true;
            }
            return false;
        }

        public void Eventually(Action action)
        {
            Eventually(() => { action(); return 0; });
        }
    }

    public struct ConsoleOutput
    {
        public int ExitCode;
        public string Stdout;

        public void AssertSucceeded()
        {
            Assert.True(0 == ExitCode, $"Exit code: {ExitCode}\n{Stdout}");
        }
    };

    public class CommandLineRunner
    {
        public Func<string[], int> Main { get; set; }
        public Action<string[]> VoidMain { get; set; }
        public string Command { get; set; }

        /// <summary>Runs StorageSample.exe with the provided arguments</summary>
        /// <returns>The console output of this program</returns>
        public ConsoleOutput Run(params string[] arguments)
        {
            Console.Write($"{Command} ");
            Console.WriteLine(string.Join(" ", arguments));

            TextWriter consoleOut = Console.Out;
            StringWriter stringOut = new StringWriter();
            Console.SetOut(stringOut);
            try
            {
                int exitCode = 0;
                if (null == VoidMain)
                    exitCode = Main(arguments);
                else
                    VoidMain(arguments);
                var consoleOutput = new ConsoleOutput()
                {
                    ExitCode = exitCode,
                    Stdout = stringOut.ToString()
                };
                Console.Write(consoleOutput.Stdout);
                return consoleOutput;
            }
            finally
            {
                Console.SetOut(consoleOut);
            }
        }
    }
}
