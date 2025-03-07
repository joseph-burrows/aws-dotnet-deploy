// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: Apache-2.0

namespace AWS.Deploy.CLI.TypeHintResponses
{
    /// <summary>
    /// The <see cref="ECSClusterTypeHintResponse"/> class encapsulates
    /// <see cref="OptionSettingTypeHint.ECSCluster"/> type hint response
    /// </summary>
    public class ECSClusterTypeHintResponse : IDisplayable
    {
        public bool CreateNew { get; set; }
        public string ClusterArn { get; set; }
        public string NewClusterName { get; set; }

        public string ToDisplayString()
        {
            if (CreateNew)
                return NewClusterName;

            return ClusterArn;
        }
    }
}
