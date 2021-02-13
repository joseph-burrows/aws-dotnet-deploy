// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using AWS.Deploy.Common;
using AWS.Deploy.Recipes.CDK.Common;
using YamlDotNet.RepresentationModel;

namespace AWS.Deploy.Orchestrator.Utilities
{
    public class TemplateMetadataReader
    {
        private readonly string _templateBody;

        public TemplateMetadataReader(string templateBody)
        {
            _templateBody = templateBody;
        }


        public CloudApplicationMetadata ReadSettings()
        {
            try
            {
                var metadataSection = ExtractMetadataSection();

                var yamlMetadata = new YamlStream();
                yamlMetadata.Load(new StringReader(metadataSection));
                var root = (YamlMappingNode)yamlMetadata.Documents[0].RootNode;
                var metadataNode = (YamlMappingNode)root.Children[new YamlScalarNode("Metadata")];

                var cloudApplicationMetadata = new CloudApplicationMetadata();
                cloudApplicationMetadata.RecipeId = ((YamlScalarNode)metadataNode.Children[new YamlScalarNode(CloudFormationIdentifierContants.StackMetadataRecipeId)]).Value;
                cloudApplicationMetadata.RecipeVersion = ((YamlScalarNode)metadataNode.Children[new YamlScalarNode(CloudFormationIdentifierContants.StackMetadataRecipeVersion)]).Value;

                var jsonString = ((YamlScalarNode)metadataNode.Children[new YamlScalarNode(CloudFormationIdentifierContants.StackMetadataSettings)]).Value;
                cloudApplicationMetadata.Settings = JsonSerializer.Deserialize<IDictionary<string, object>>(jsonString);

                return cloudApplicationMetadata;
            }
            catch(Exception e)
            {
                throw new ParsingExistingCloudApplicationMetadataException($"Error parsing existing application's metadata", e);
            }
        }

        private string ExtractMetadataSection()
        {
            var builder = new StringBuilder();
            bool inMetadata = false;
            using var reader = new StringReader(_templateBody);
            string line;
            while((line = reader.ReadLine()) != null)
            {
                if(!inMetadata)
                {
                    // See if we found the start of the Metadata section
                    if(line.StartsWith("Metadata:"))
                    {
                        builder.AppendLine(line);
                        inMetadata = true;
                    }
                }
                else
                {
                    // See if we have found the next top level node signaling the end of the Metadata section
                    if (line.Length > 0 && char.IsLetterOrDigit(line[0]))
                    {
                        break;
                    }

                    builder.AppendLine(line);
                }
            }


            return builder.ToString();
        }
    }
}
