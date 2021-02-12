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
            stack.Tags.SetTag("aws-dotnet-deploy", $"{recipeConfiguration.RecipeId}", applyToLaunchedInstances:false);

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

            metadata["aws-dotnet-deploy-settings"] = json;
            metadata["aws-dotnet-deploy-recipe-id"] = recipeConfiguration.RecipeId;
            metadata["aws-dotnet-deploy-recipe-version"] = recipeConfiguration.RecipeVersion;

            stack.TemplateOptions.Metadata = metadata;

            return stack;
        }
    }
}
