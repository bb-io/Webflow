﻿using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Tests.Webflow.Base;

public class TestBase
{
    public List<IEnumerable<AuthenticationCredentialsProvider>> CredentialGroups { get; private set; }
    public List<InvocationContext> InvocationContext { get; private set; }
    public IFileManagementClient FileManagementClient { get; private set; }

    public TestBase()
    {
        InitializeCredentials();
        InitializeInvocationContext();
        InitializeFileManager();
    }

    public InvocationContext GetInvocationContext(string connectionType)
    {
        var context = InvocationContext.FirstOrDefault(x => x.AuthenticationCredentialsProviders.Any(y => y.Value == connectionType));
        if (context == null)
            throw new Exception($"Invocation context was not found for this connection type: {connectionType}");
        else return context;
    }

    private void InitializeCredentials()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        CredentialGroups = config.GetSection("ConnectionDefinition")
            .GetChildren()
            .Select(section =>
                section.GetChildren()
               .Select(child => new AuthenticationCredentialsProvider(child.Key, child.Value))
            )
            .ToList();
    }

    private void InitializeInvocationContext()
    {
        InvocationContext = new List<InvocationContext>();
        foreach (var credentialGroup in CredentialGroups)
        {
            InvocationContext.Add(new InvocationContext
            {
                AuthenticationCredentialsProviders = credentialGroup
            });
        }
    }

    private void InitializeFileManager()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        FileManagementClient = new FileManager();
    }

    protected static void PrintJsonResult(object result)
    {
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
}
