﻿// Copyright (c) Khulnasoft Ltd. All rights reserved.
// AzureOpenAIConfig.cs

namespace DevGpt.OpenAI;

public class AzureOpenAIConfig : ILLMConfig
{
    public AzureOpenAIConfig(string endpoint, string deploymentName, string apiKey, string? modelId = null)
    {
        this.Endpoint = endpoint;
        this.DeploymentName = deploymentName;
        this.ApiKey = apiKey;
        this.ModelId = modelId;
    }

    public string Endpoint { get; }

    public string DeploymentName { get; }

    public string ApiKey { get; }

    public string? ModelId { get; }
}
