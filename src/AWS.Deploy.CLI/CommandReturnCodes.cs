using AWS.Deploy.Common;

namespace AWS.Deploy.CLI
{
    /// <summary>
    /// Standardized cli return codes for Commands.
    /// </summary>
    public class CommandReturnCodes
    {
        /// <summary>
        /// Command completed and honored user's intention.
        /// </summary>
        public const int SUCCESS = 0;
        /// <summary>
        /// A command could not finish its work because an unexpected
        /// exception was thrown.  This usually means there is an intermittent io problem
        /// or bug in the code base.
        /// <para />
        /// Unexpected exception are any exception not marked by
        /// <see cref="AWSDeploymentExpectedExceptionAttribute"/>
        /// </summary>
        public const int UNHANDLED_EXCEPTION = -1;
        /// <summary>
        /// A command could not finish of an expected problem like a user
        /// configuration or system configuration problem.  For example, a required
        /// dependency like Docker is not installed.
        /// <para />
        /// Expected problems are usually indicated by throwing an exception that is
        /// decorated with <see cref="AWSDeploymentExpectedExceptionAttribute"/>
        /// </summary>
        public const int USER_ERROR = 1;
    }
}
