// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace AWS.Deploy.Common
{
    public class CloudApplicationMetadata
    {
        public string RecipeId { get; set; }

        public string RecipeVersion { get; set; }

        public IDictionary<string, object> Settings { get; set; }
    }
}
