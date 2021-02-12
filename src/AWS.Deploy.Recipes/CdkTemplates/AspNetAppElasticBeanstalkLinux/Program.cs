using Amazon.CDK;
using System.Threading.Tasks;
using AspNetAppElasticBeanstalkLinux.Utilities;
using Microsoft.Extensions.Configuration;

using AWS.Deploy.Recipes.CDK.Common;

namespace AspNetAppElasticBeanstalkLinux
{
    sealed class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, false);
            var configuration = builder.Build().Get<RecipeConfiguration<Configuration>>();

            var zipPublisher = new ZipPublisher();
            configuration.Settings.AssetPath = zipPublisher.GetZipPath(configuration.ProjectPath);

            var solutionStackNameProvider = new SolutionStackNameProvider();
            configuration.Settings.SolutionStackName = await solutionStackNameProvider.GetSolutionStackNameAsync();

            var app = new App();

            StackFactory.InitializeAWSDeployStack<Configuration>(new AppStack(app, configuration.StackName, configuration.Settings, new StackProps
            {
                Env = new Environment
                {
                    Account = "AWSAccountId",
                    Region = "AWSRegion"
                }
            }), configuration);

            app.Synth();
        }
    }
}
