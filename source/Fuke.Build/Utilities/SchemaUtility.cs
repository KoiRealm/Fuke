// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
using NuGet.Packaging;
using Fuke.Common.Utilities;
using Fuke.Common.ValueInjection;
using static Fuke.Common.Constants;

namespace Fuke.Common.Execution;

public class SchemaUtility
{
    private class SchemaGenerator : JsonSchemaGenerator
    {
        private class Resolver : DefaultContractResolver
        {
            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                return objectType == typeof(ExecutableTarget) || objectType == typeof(Host)
                    ? new List<MemberInfo>()
                    : base.GetSerializableMembers(objectType);
            }
        }

        public static JsonSchema Generate<T>(T build) where T : IFukeBuild
        {
            return new SchemaGenerator(
                build,
                new NewtonsoftJsonSchemaGeneratorSettings
                {
                    FlattenInheritanceHierarchy = true,
                    SerializerSettings =
                        new JsonSerializerSettings
                        {
                            ContractResolver = new Resolver(),
                            Converters = new JsonConverter[] { new StringEnumConverter() }
                        }
                }).Generate();
        }

        private readonly IFukeBuild _build;

        private SchemaGenerator(IFukeBuild build, JsonSchemaGeneratorSettings settings)
            : base(settings)
        {
            _build = build;
        }

        private JsonSchema Generate()
        {
            var topLevelSchema = new JsonSchema();
            var baseSchema = new JsonSchema();
            var userSchema = new JsonSchema();
            var schemaResolver = new JsonSchemaResolver(topLevelSchema, Settings);

            var parameterMembers = ValueInjectionUtility.GetParameterMembers(_build.GetType(), includeUnlisted: true);
            foreach (var parameterMember in parameterMembers)
            {
                var schema = parameterMember.DeclaringType == typeof(FukeBuild) ? baseSchema : userSchema;
                var name = ParameterService.GetParameterMemberName(parameterMember);
                var property = CreateProperty(parameterMember, schemaResolver);
                schema.Properties[name] = property;
            }

            // ValueInjectionUtility.GetParameterMembers(_build.GetType(), includeUnlisted: true)
            //     // .Where(x => x.Name.EqualsAnyOrdinalIgnoreCase(
            //     //     nameof(FukeBuild.SkippedTargets),
            //     //     nameof(FukeBuild.InvokedTargets),
            //     //     nameof(FukeBuild.Verbosity)
            //     // ))
            //     .ToDictionary(ParameterService.GetParameterMemberName, x => CreateProperty(x, schemaResolver))
            //     .ForEach(x =>
            //     {
            //         baseSchema.Properties[x.Key] = x.Value;
            //     });

            // TODO: why can't this use value sets?
            var targetNames = ExecutableTargetFactory.GetTargetProperties(_build.GetType()).Select(x => x.GetDisplayShortName()).OrderBy(x => x);
            var executableTargetSchema = UpdatePropertySchema(nameof(ExecutableTarget), targetNames);
            baseSchema.Properties[InvokedTargetsParameterName].Item =
                baseSchema.Properties[SkippedTargetsParameterName].Item = new JsonSchema { Reference = executableTargetSchema };

            var hostNames = Host.AvailableTypes.Select(x => x.Name).OrderBy(x => x);
            var hostSchema = UpdatePropertySchema(nameof(FukeBuild.Host), hostNames);
            baseSchema.Properties[nameof(FukeBuild.Host)].Reference = hostSchema;

            RemoveXEnumValues();

            topLevelSchema.AllOf.Add(userSchema);
            topLevelSchema.AllOf.Add(new JsonSchema { Reference = baseSchema });
            topLevelSchema.Definitions[nameof(FukeBuild)] = baseSchema;
            return topLevelSchema;

            JsonSchema UpdatePropertySchema(string name, IEnumerable<string> values)
            {
                var schema = topLevelSchema.Definitions[name];
                schema.Type = JsonObjectType.String;
                schema.AllowAdditionalProperties = true;
                schema.Enumeration.AddRange(values);
                return schema;
            }

            void RemoveXEnumValues()
            {
                foreach (var definition in topLevelSchema.Definitions.Values)
                {
                    definition.EnumerationNames.Clear();
                    definition.AllowAdditionalProperties = true;
                }
            }
        }

        private JsonSchemaProperty CreateProperty(MemberInfo parameterMember, JsonSchemaResolver schemaResolver)
        {
            var property = parameterMember.GetCustomAttribute<ParameterAttribute>().NotNull().GetType() == typeof(ParameterAttribute)
                ? GenerateWithReference<JsonSchemaProperty>(
                    parameterMember.ToContextualAccessor().AccessorType,
                    schemaResolver)
                : new JsonSchemaProperty { Type = JsonObjectType.String };

            property.Description = parameterMember.GetCustomAttribute<ParameterAttribute>().NotNull().Description;
            property.Default = parameterMember.HasCustomAttribute<SecretAttribute>()
                ? "Enter the secret through 'fuke :secrets [profile]'."
                : null;

            var values = ParameterService.GetParameterValueSet(parameterMember, _build)
                ?.Select(x => (object)x.Text);
            if (values != null && !parameterMember.GetMemberType().IsEnum)
            {
                property.Type = !parameterMember.GetMemberType().IsCollectionLike()
                    ? JsonObjectType.String
                    : JsonObjectType.Array;
                var propertySchema = property.Reference ?? property;
                if (property.Type == JsonObjectType.String)
                    propertySchema.Enumeration.AddRange(values);
                else
                    propertySchema.Item.Enumeration.AddRange(values);
            }

            if (Nullable.GetUnderlyingType(parameterMember.GetMemberType()) != null)
                property.Type |= JsonObjectType.Null;

            return property;
        }
    }

    public static string GetJsonString(IFukeBuild build)
    {
        return SchemaGenerator.Generate(build).ToJson();
    }

    public static JsonDocument GetJsonDocument(IFukeBuild build)
    {
        return JsonDocument.Parse(GetJsonString(build));
    }
}
