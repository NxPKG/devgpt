﻿// Copyright (c) Khulnasoft Ltd. All rights reserved.
// KernelFunctionExtensionTests.cs

using System.ComponentModel;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using DevGpt.SemanticKernel.Extension;
using FluentAssertions;
using Khulnasoft.SemanticKernel;
using Newtonsoft.Json;
using Xunit;

namespace DevGpt.SemanticKernel.Tests;

public class TestPlugin
{
    public bool IsOn { get; set; } = false;

    [KernelFunction]
    [Description("Gets the state of the light.")]
    public string GetState() => this.IsOn ? "on" : "off";

    [KernelFunction]
    [Description("Changes the state of the light.'")]
    public string ChangeState(
        [Description("new state")] bool newState)
    {
        this.IsOn = newState;
        var state = this.GetState();

        // Print the state to the console
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine($"[Light is now {state}]");
        Console.ResetColor();

        return $"The status of the light is now {state}";
    }
}
public class KernelFunctionExtensionTests
{
    private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
        StringEscapeHandling = StringEscapeHandling.Default,
    };

    [Fact]
    [UseReporter(typeof(DiffReporter))]
    [UseApprovalSubdirectory("ApprovalTests")]
    public void ItCreateFunctionContractsFromTestPlugin()
    {
        var kernel = new Kernel();
        var plugin = kernel.ImportPluginFromType<TestPlugin>("test_plugin");

        var functionContracts = plugin.Select(f => f.Metadata.ToFunctionContract()).ToList();

        functionContracts.Count.Should().Be(2);
        var json = JsonConvert.SerializeObject(functionContracts, _serializerSettings);

        Approvals.Verify(json);
    }

    [Fact]
    [UseReporter(typeof(DiffReporter))]
    [UseApprovalSubdirectory("ApprovalTests")]
    public void ItCreateFunctionContractsFromMethod()
    {
        var kernel = new Kernel();
        var sayHelloFunction = KernelFunctionFactory.CreateFromMethod(() => "Hello, World!");
        var echoFunction = KernelFunctionFactory.CreateFromMethod((string message) => message);

        var functionContracts = new[]
        {
            sayHelloFunction.Metadata.ToFunctionContract(),
            echoFunction.Metadata.ToFunctionContract(),
        };

        var json = JsonConvert.SerializeObject(functionContracts, _serializerSettings);

        functionContracts.Length.Should().Be(2);
        Approvals.Verify(json);
    }

    [Fact]
    [UseReporter(typeof(DiffReporter))]
    [UseApprovalSubdirectory("ApprovalTests")]
    public void ItCreateFunctionContractsFromPrompt()
    {
        var kernel = new Kernel();
        var sayHelloFunction = KernelFunctionFactory.CreateFromPrompt("Say {{hello}}, World!", functionName: "sayHello");

        var functionContracts = new[]
        {
            sayHelloFunction.Metadata.ToFunctionContract(),
        };

        var json = JsonConvert.SerializeObject(functionContracts, _serializerSettings);

        functionContracts.Length.Should().Be(1);
        Approvals.Verify(json);
    }
}
