// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace AWS.Deploy.Recipes.CDK.Common
{
    public class RecipeConfiguration<T>
    {
        public string StackName { get; set; }
        public string ProjectPath { get; set; }
        public string ProjectSolutionPath { get; set; }
        public string DockerfileDirectory { get; set; }

        public string RecipeId { get; set; }
        public string RecipeVersion { get; set; }

        public T Settings { get; set; }
    }
}
