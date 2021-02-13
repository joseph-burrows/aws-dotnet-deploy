using System;
using System.Collections.Generic;
using System.Text.Json;

using Amazon.CDK;

namespace AWS.Deploy.Recipes.CDK.Common
{
    public class StackFactory
    {
        public static Stack InitializeAWSDeployStack<C>(Stack stack, RecipeConfiguration<C> recipeConfiguration)
        {
            stack.Tags.SetTag(CloudFormationIdentifierContants.StackTag, $"{recipeConfiguration.RecipeId}", applyToLaunchedInstances:false);

            var json = JsonSerializer.Serialize(recipeConfiguration.Settings, new JsonSerializerOptions { WriteIndented = false });

            Dictionary<string, object> metadata;
            if(stack.TemplateOptions.Metadata?.Count > 0)
            {
                metadata = new Dictionary<string, object>(stack.TemplateOptions.Metadata);
            }
            else
            {
                metadata = new Dictionary<string, object>();
            }

            metadata[CloudFormationIdentifierContants.StackMetadataSettings] = json;
            metadata[CloudFormationIdentifierContants.StackMetadataRecipeId] = recipeConfiguration.RecipeId;
            metadata[CloudFormationIdentifierContants.StackMetadataRecipeVersion] = recipeConfiguration.RecipeVersion;

            stack.TemplateOptions.Metadata = metadata;

            if(string.IsNullOrEmpty(stack.TemplateOptions.Description))
            {
                stack.TemplateOptions.Description = CloudFormationIdentifierContants.StackDescriptionPrefix;
            }
            else
            {
                stack.TemplateOptions.Description = CloudFormationIdentifierContants.StackDescriptionPrefix + ": " + stack.TemplateOptions.Description;
            }

            return stack;
        }
    }
}
