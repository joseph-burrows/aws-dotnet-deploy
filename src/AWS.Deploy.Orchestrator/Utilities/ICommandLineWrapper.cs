// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AWS.Deploy.Orchestrator.Utilities
{
    public interface ICommandLineWrapper
    {
        /// <summary>
        /// Forks a new shell process and executes <paramref name="command"/>.
        /// </summary>
        /// <param name="command">
        /// Shell script to execute
        /// </param>
        /// <param name="workingDirectory">
        /// Default directory for the shell.  This needs to have the correct pathing for
        /// the current OS
        /// </param>
        /// <param name="streamOutputToInteractiveService">
        /// By default standard out/error will be piped to a <see cref="IOrchestratorInteractiveService"/>.
        /// Set this to false to disable sending output.
        /// </param>
        /// <param name="onComplete">
        /// Async callback to inspect/manipulate the completed <see cref="Process"/>.  Useful
        /// if you need to get an exit code or <see cref="Process.StandardOutput"/>.
        /// </param>
        /// <param name="redirectIO">
        /// By default, <see cref="Process.StandardInput"/>, <see cref="Process.StandardOutput"/> and <see cref="Process.StandardError"/> will be redirected.
        /// Set this to false to avoid redirection.
        /// </param>
        /// <param name="cancelToken">
        /// <see cref="CancellationToken"/>
        /// </param>
        public Task Run(
            string command,
            string workingDirectory = "",
            bool streamOutputToInteractiveService = true,
            Action<TryRunResult> onComplete = null,
            bool redirectIO = true,
            CancellationToken cancelToken = default);
    }

    public static class CommandLineWrapperExtensions
    {
        /// <summary>
        /// Convenience extension to <see cref="ICommandLineWrapper.Run"/>
        /// that returns a <see cref="TryRunWithResult"/> with the full contents
        /// of <see cref="Process.StandardError"/> and <see cref="Process.StandardOutput"/>
        /// </summary>
        /// <param name="commandLineWrapper">
        /// See <see cref="ICommandLineWrapper"/>
        /// </param>
        /// <param name="command">
        /// Shell script to execute
        /// </param>
        /// <param name="workingDirectory">
        /// Default directory for the shell.  This needs to have the correct pathing for
        /// the current OS
        /// </param>
        /// <param name="streamOutputToInteractiveService">
        /// By default standard out/error will be piped to a <see cref="IOrchestratorInteractiveService"/>.
        /// Set this to false to disable sending output.
        /// </param>
        /// <param name="redirectIO">
        /// By default, <see cref="Process.StandardInput"/>, <see cref="Process.StandardOutput"/> and <see cref="Process.StandardError"/> will be redirected.
        /// Set this to false to avoid redirection.
        /// </param>
        /// <param name="cancelToken">
        /// <see cref="CancellationToken"/>
        /// </param>
        public static async Task<TryRunResult> TryRunWithResult(
            this ICommandLineWrapper commandLineWrapper,
            string command,
            string workingDirectory = "",
            bool streamOutputToInteractiveService = false,
            bool redirectIO = true,
            CancellationToken cancelToken = default)
        {
            var result = new TryRunResult();

            await commandLineWrapper.Run(
                command,
                workingDirectory,
                streamOutputToInteractiveService,
                onComplete: runResult => result = runResult,
                redirectIO: redirectIO,
                cancelToken);

            return result;
        }
    }

    public class TryRunResult
    {
        /// <summary>
        /// Indicates if this command was run successfully.  This checks that
        /// <see cref="StandardError"/> is empty.
        /// </summary>
        public bool Success => string.IsNullOrEmpty(StandardError);

        /// <summary>
        /// Fully read <see cref="Process.StandardOutput"/>
        /// </summary>
        public string StandardOut { get; set; }

        /// <summary>
        /// Fully read <see cref="Process.StandardError"/>
        /// </summary>
        public string StandardError { get; set; }

        /// <summary>
        /// Fully read <see cref="Process.ExitCode"/>
        /// </summary>
        public int ExitCode { get; set; }
    }
}
