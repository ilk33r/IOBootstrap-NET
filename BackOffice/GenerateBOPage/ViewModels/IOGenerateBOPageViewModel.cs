using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Extensions;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.BackOffice;

public class IOGenerateBOPageViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>
where TDBContext : IODatabaseContext<TDBContext> 
{
    #region Initialization Methods

    public IOGenerateBOPageViewModel() : base()
    {
    }

    #endregion

    #region VM

    public IOGenerateBOPageResponseModel CreateModel(string entityName)
    {
        string dataAccessAssemblyName = Configuration.GetValue<string>(IOConfigurationConstants.DataAccessAssembly);
        Assembly dataAccessAssembly = Assembly.Load(dataAccessAssemblyName);
        Type entityClass = dataAccessAssembly.GetType(dataAccessAssemblyName + ".Entities." + entityName);
        var entityClassInstance = Activator.CreateInstance(entityClass);

        string serializedBody = JsonSerializer.Serialize(entityClassInstance, new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Dictionary<string, object> jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(serializedBody);
        List<IOBOPageEntityModel> properties = new List<IOBOPageEntityModel>();
        foreach (MemberInfo member in entityClass.GetMembers())
        {
            IOBOPageEntityModel propertyEntity = PageEntity(member, jsonObject);
            if (propertyEntity != null)
            {
                properties.Add(propertyEntity);
            }
        }

        IOGenerateBOPageResponseModel response = new IOGenerateBOPageResponseModel();
        response.EntityName = entityName;
        response.EntityDisplayName = entityName.Replace("Entity", "s");
        response.EntityItemName = entityName.Replace("Entity", "");
        response.Properties = properties;
        response.ListEntityDisplayName = entityName.Replace("Entity", "List");
        response.ListEntityName = response.ListEntityDisplayName.ToLowerFirst();
        response.ListEntityAPIPath = String.Format("BackOffice{0}/Get{1}", response.EntityDisplayName, response.EntityDisplayName);
        response.UpdateEntityDisplayName = entityName.Replace("Entity", "Update");
        response.UpdateEntityName = response.UpdateEntityDisplayName.ToLowerFirst();
        response.UpdateEntityAPIPath = String.Format("BackOffice{0}/Update{1}", response.EntityDisplayName, response.EntityItemName);

        return response;
    }

    #endregion

    #region Helper Methods

    private IOBOPageEntityModel PageEntity(MemberInfo member, Dictionary<string, object> jsonObject)
    {
        var jsonObjectPair = jsonObject.Where(o => o.Key.ToLower().Equals(member.Name.ToLower()))
                                        .FirstOrDefault();
        if (jsonObjectPair.Key == null)
        {
            return null;
        }

        IOBOPageEntityModel entityModel = new IOBOPageEntityModel();
        entityModel.PropertyName = member.Name;
        entityModel.PropertyJsonKey = jsonObjectPair.Key;
        entityModel.Nullable = true;

        Type underlayingType = GetMemberUnderlyingType(member);
        if (underlayingType.FullName.Contains("Collections"))
        {
            return null;
        }

        entityModel.Type = PropertyTypeFromName(underlayingType.FullName);

        IEnumerable<CustomAttributeData> propertyAttributes = member.CustomAttributes;
        foreach (CustomAttributeData propertyAttribute in propertyAttributes)
        {
            if (propertyAttribute.AttributeType.Name.Contains("Required"))
            {
                entityModel.Nullable = false;
                break;
            }
        }

        if (entityModel.Type == IOBOPagePropertyType.Enum && !underlayingType.IsEnum)
        {
            return null;
        }
        else if (underlayingType.IsEnum)
        {
            entityModel.EnumTypeName = underlayingType.FullName.Split(".").Last();
            entityModel.EnumType = CustomEnumTypeFromName(underlayingType.FullName);
        }
        
        return entityModel;
    }

    private Type GetMemberUnderlyingType(MemberInfo member)
    {
        return member.MemberType switch
        {
            MemberTypes.Event => ((EventInfo)member).EventHandlerType,
            MemberTypes.Field => ((FieldInfo)member).FieldType,
            MemberTypes.Method => ((MethodInfo)member).ReturnType,
            MemberTypes.Property => ((PropertyInfo)member).PropertyType,
            _ => throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"),
        };
    }

    private IOBOPagePropertyType PropertyTypeFromName(string typeName)
    {
        if (typeName.Contains("System.Int"))
        {
            return IOBOPagePropertyType.Int;
        }
        else if (typeName.Contains("System.String"))
        {
            return IOBOPagePropertyType.String;
        }
        else if (typeName.Contains("System.Double"))
        {
            return IOBOPagePropertyType.Double;
        }
        else if (typeName.Contains("System.Float"))
        {
            return IOBOPagePropertyType.Float;
        }
        else if (typeName.Contains("System.DateTimeOffset"))
        {
            return IOBOPagePropertyType.DateTimeOffset;
        }

        return IOBOPagePropertyType.Enum;
    }

    private IList<IOBOPageEntityCustomEnumTypeModel> CustomEnumTypeFromName(string typeName)
    {
        string[] typeNameParts = typeName.Split(".");
        int assemblyNamePartCount = typeNameParts.Length - 2;
        if (assemblyNamePartCount <= 0)
        {
            return null;
        }

        string[] assemblyNames = new string[assemblyNamePartCount];
        for (int i = 0; i < assemblyNamePartCount; i++)
        {
            assemblyNames[i] = typeNameParts[i];
        }

        string assemblyName = String.Join(".", assemblyNames);
        Assembly assembly = Assembly.Load(assemblyName);
        Type enumType = assembly.GetType(typeName);
        var values = Enum.GetValues(enumType);

        List<IOBOPageEntityCustomEnumTypeModel> enumTypes = new List<IOBOPageEntityCustomEnumTypeModel>();
        foreach (var value in values)
        {
            IOBOPageEntityCustomEnumTypeModel typeModel = new IOBOPageEntityCustomEnumTypeModel() 
            {
                Name = value.ToString(),
                IntValue = (int)value
            };
            enumTypes.Add(typeModel);
        }

        return enumTypes;
    }

    #endregion
}
