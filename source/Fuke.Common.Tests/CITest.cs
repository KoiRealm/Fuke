// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using JetBrains.Annotations;
using Fuke.Common.CI;
using Fuke.Common.CI.AppVeyor;
using Fuke.Common.CI.AzurePipelines;
using Fuke.Common.CI.Bitrise;
using Fuke.Common.CI.GitLab;
using Fuke.Common.CI.Jenkins;
using Fuke.Common.CI.TeamCity;
using Fuke.Common.CI.TravisCI;
using Xunit;

namespace Fuke.Common.Tests;

public class CITest
{
    [CITheory(typeof(AppVeyor))]
    [MemberData(nameof(Properties), typeof(AppVeyor))]
    public void TestAppVeyor(PropertyInfo property, AppVeyor instance)
    {
        AssertProperty(instance, property);
    }

    [CITheory(typeof(Bitrise))]
    [MemberData(nameof(Properties), typeof(Bitrise))]
    public void TestBitrise(PropertyInfo property, Bitrise instance)
    {
        AssertProperty(instance, property);
    }

    [CITheory(typeof(TeamCity))]
    [MemberData(nameof(Properties), typeof(TeamCity))]
    public void TestTeamCity(PropertyInfo property, TeamCity instance)
    {
        AssertProperty(instance, property);
    }

    [CITheory(typeof(AzurePipelines))]
    [MemberData(nameof(Properties), typeof(AzurePipelines))]
    public void TestAzureDevOps(PropertyInfo property, AzurePipelines instance)
    {
        AssertProperty(instance, property);
    }

    [CITheory(typeof(Jenkins))]
    [MemberData(nameof(Properties), typeof(Jenkins))]
    public void TestJenkins(PropertyInfo property, Jenkins instance)
    {
        AssertProperty(instance, property);
    }

    [CITheory(typeof(TravisCI))]
    [MemberData(nameof(Properties), typeof(TravisCI))]
    public void TestTravisCI(PropertyInfo property, TravisCI instance)
    {
        AssertProperty(instance, property);
        Assert.True(instance.Ci);
        Assert.True(instance.ContinousIntegration);
    }

    [CITheory(typeof(GitLab))]
    [MemberData(nameof(Properties), typeof(GitLab))]
    public void TestGitLab(PropertyInfo property, GitLab instance)
    {
        AssertProperty(instance, property);
        Assert.True(instance.Ci);
    }

    public static IEnumerable<object[]> Properties(Type type)
    {
        var instance = CreateInstance(type);

        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(x => new[] { x, instance }).ToArray();
    }

    private static void AssertProperty(object instance, PropertyInfo property)
    {
        object value;
        try
        {
            value = property.GetValue(instance);
        }
        catch (TargetInvocationException exception)
        {
            throw exception.InnerException.NotNull();
        }

        if (property.GetCustomAttribute<CanBeNullAttribute>() == null)
            value.Should().NotBeNull();
        else if (property.PropertyType != typeof(string))
            Nullable.GetUnderlyingType(property.PropertyType).Should().NotBeNull();

        if (value is not string strValue || property.GetCustomAttribute<NoConvertAttribute>() != null)
            return;

        bool.TryParse(strValue, out _).Should().BeFalse("boolean");
        long.TryParse(strValue, out _).Should().BeFalse("long");
        decimal.TryParse(strValue, out _).Should().BeFalse("decimal");
        DateTime.TryParse(strValue, out _).Should().BeFalse("DateTime");
        TimeSpan.TryParse(strValue, out _).Should().BeFalse("TimeSpan");
        Guid.TryParse(strValue, out _).Should().BeFalse("Guid");
    }

    private static object CreateInstance(Type type)
    {
        var bindingFlags = BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.OptionalParamBinding;
        return Activator.CreateInstance(type, bindingFlags, binder: null, args: new object[0], culture: CultureInfo.CurrentCulture);
    }

    private static bool IsRunning(Type type)
    {
        var property = type.GetProperty($"IsRunning{type.Name}", BindingFlags.NonPublic | BindingFlags.Static).NotNull();
        return (bool) property.GetValue(obj: null);
    }

    private class CITheoryAttribute : TheoryAttribute
    {
        public CITheoryAttribute(
            Type type,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = -1)
            : base(sourceFilePath, sourceLineNumber)
        {
            if (!IsRunning(type))
                Skip = $"仅适用于 {type.Name}。";
        }
    }
}
