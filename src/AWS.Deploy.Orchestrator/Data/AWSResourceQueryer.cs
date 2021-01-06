// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.ECS;
using Amazon.ECS.Model;
using Amazon.ElasticBeanstalk;
using Amazon.ElasticBeanstalk.Model;
using Amazon.EC2;
using AWS.Deploy.Common;
using Amazon.EC2.Model;
using System.IO;
using System.Net;
using Amazon.Auth.AccessControlPolicy;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;

namespace AWS.Deploy.Orchestrator.Data
{
    public interface IAWSResourceQueryer
    {
        Task<List<string>> GetListOfECSClusters(OrchestratorSession session);
        Task<List<ApplicationDescription>> ListOfElasticBeanstalkApplications(OrchestratorSession session);
        Task<List<EnvironmentDescription>> ListOfElasticBeanstalkEnvironments(OrchestratorSession session, string applicationName);
        Task<List<KeyPairInfo>> ListOfEC2KeyPairs(OrchestratorSession session);
        Task<string> CreateEC2KeyPair(OrchestratorSession session, string keyName, string saveLocation);
        Task<List<Role>> ListOfIAMRoles(OrchestratorSession session, string servicePrincipal);
    }

    public class AWSResourceQueryer : IAWSResourceQueryer
    {
        private readonly IAWSClientFactory _awsClientFactory;

        public AWSResourceQueryer(IAWSClientFactory awsClientFactory)
        {
            _awsClientFactory = awsClientFactory;
        }

        public async Task<List<string>> GetListOfECSClusters(OrchestratorSession session)
        {
            var ecsClient = _awsClientFactory.GetAWSClient<IAmazonECS>(session.AWSCredentials, session.AWSRegion);

            var results = new List<string>();

            var request = new ListClustersRequest();

            do
            {
                var response = await ecsClient.ListClustersAsync(request);
                request.NextToken = response.NextToken;

                results.AddRange(response.ClusterArns);


            } while (!string.IsNullOrEmpty(request.NextToken));

            return results;
        }

        public async Task<List<ApplicationDescription>> ListOfElasticBeanstalkApplications(OrchestratorSession session)
        {
            var beanstalkClient = _awsClientFactory.GetAWSClient<IAmazonElasticBeanstalk>(session.AWSCredentials, session.AWSRegion);
            var applications = await beanstalkClient.DescribeApplicationsAsync();
            return applications.Applications;
        }

        public async Task<List<EnvironmentDescription>> ListOfElasticBeanstalkEnvironments(OrchestratorSession session, string applicationName)
        {
            var beanstalkClient = _awsClientFactory.GetAWSClient<IAmazonElasticBeanstalk>(session.AWSCredentials, session.AWSRegion);
            var environments = new List<EnvironmentDescription>();
            var request = new DescribeEnvironmentsRequest
            {
                ApplicationName = applicationName
            };

            do
            {
                var response = await beanstalkClient.DescribeEnvironmentsAsync(request);
                request.NextToken = response.NextToken;

                environments.AddRange(response.Environments);

            } while (!string.IsNullOrEmpty(request.NextToken));

            return environments;
        }

        public async Task<List<KeyPairInfo>> ListOfEC2KeyPairs(OrchestratorSession session)
        {
            var ec2Client = _awsClientFactory.GetAWSClient<IAmazonEC2>(session.AWSCredentials, session.AWSRegion);
            var response = await ec2Client.DescribeKeyPairsAsync();

            return response.KeyPairs;
        }

        public async Task<string> CreateEC2KeyPair(OrchestratorSession session, string keyName, string saveLocation)
        {
            var ec2Client = _awsClientFactory.GetAWSClient<IAmazonEC2>(session.AWSCredentials, session.AWSRegion);

            var request = new CreateKeyPairRequest() { KeyName = keyName };

            var response = await ec2Client.CreateKeyPairAsync(request);

            File.WriteAllText(Path.Combine(saveLocation, $"{keyName}.pem"), response.KeyPair.KeyMaterial);

            return response.KeyPair.KeyName;
        }

        public async Task<List<Role>> ListOfIAMRoles(OrchestratorSession session, string servicePrincipal)
        {
            var identityManagementServiceClient = _awsClientFactory.GetAWSClient<IAmazonIdentityManagementService>(session.AWSCredentials, session.AWSRegion);

            var listRolesRequest = new ListRolesRequest();
            var roles = new List<Role>();

            var listStacksPaginator = identityManagementServiceClient.Paginators.ListRoles(listRolesRequest);
            await foreach (var response in listStacksPaginator.Responses)
            {
                var filteredRoles = response.Roles.Where(role => AssumeRoleServicePrincipalSelector(role, servicePrincipal));
                roles.AddRange(filteredRoles);
            }

            return roles;
        }

        private static bool AssumeRoleServicePrincipalSelector(Role role, string servicePrincipal)
        {
            return !string.IsNullOrEmpty(role.AssumeRolePolicyDocument) && role.AssumeRolePolicyDocument.Contains(servicePrincipal);
        }
    }
}
