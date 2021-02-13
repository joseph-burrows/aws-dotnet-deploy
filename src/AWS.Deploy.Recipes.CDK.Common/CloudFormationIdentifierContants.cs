// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace AWS.Deploy.Recipes.CDK.Common
{
    public class CloudFormationIdentifierContants
    {
        public const string StackTag = "aws-dotnet-deploy";
        public const string StackDescriptionPrefix = "AWSDotnetDeployCDKStack";

        public const string StackMetadataSettings = "aws-dotnet-deploy-settings";
        public const string StackMetadataRecipeId = "aws-dotnet-deploy-recipe-id";
        public const string StackMetadataRecipeVersion = "aws-dotnet-deploy-recipe-version";
    }
}
