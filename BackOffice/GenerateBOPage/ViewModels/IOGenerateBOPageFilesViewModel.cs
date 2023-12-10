using System.IO.Compression;
using IOBootstrap.NET.Common;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.BackOffice;

public class IOGenerateBOPageFilesViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>
where TDBContext : IODatabaseContext<TDBContext> 
{
    #region Initialization Methods

    public IOGenerateBOPageFilesViewModel() : base()
    {
    }

    #endregion

    #region VM

    public string CreateAPIFiles(IOGenerateBOPageFilesRequestModel requestModel, string projectDir, string generatedFolderName, string generatedZipFileName)
    {
        string tempPath = Path.GetTempPath();
        string generatedFolderPath = Path.Join(tempPath, generatedFolderName);

        if (Directory.Exists(generatedFolderPath))
        {
            Directory.Delete(generatedFolderPath, true);
        }
        
        Directory.CreateDirectory(generatedFolderPath);

        string templatePath = Path.Combine(projectDir, "BOTemplates/API");
        Dictionary<string, string> variables = CreateVariables(requestModel);
        List<string> files = new List<string>();
        CopyFolder(requestModel, templatePath, generatedFolderPath, variables, ref files);

        return ZipFolder(generatedFolderPath, files, generatedZipFileName);
    }

    public string CreateUIFiles(IOGenerateBOPageFilesRequestModel requestModel, string projectDir, string generatedFolderName, string generatedZipFileName)
    {
        string tempPath = Path.GetTempPath();
        string generatedFolderPath = Path.Join(tempPath, generatedFolderName);

        if (Directory.Exists(generatedFolderPath))
        {
            Directory.Delete(generatedFolderPath, true);
        }
        
        Directory.CreateDirectory(generatedFolderPath);

        string templatePath = Path.Combine(projectDir, "BOTemplates/UI");
        Dictionary<string, string> variables = CreateVariables(requestModel);
        List<string> files = new List<string>();
        CopyFolder(requestModel, templatePath, generatedFolderPath, variables, ref files);

        return ZipFolder(generatedFolderPath, files, generatedZipFileName);
    }

    #endregion

    #region File Methods

    private void CopyFolder(IOGenerateBOPageFilesRequestModel requestModel, string source, string destination, Dictionary<string, string> variables, ref List<string> files)
    {
        string[] filePaths = Directory.GetFiles(source);
        foreach (string filePath in filePaths)
        {
            CopyFile(requestModel, filePath, destination, variables, ref files);
        }

        string[] directories = Directory.GetDirectories(source);
        foreach (string directoryPath in directories)
        {
            string directoryName = Path.GetFileName(directoryPath);
            string updatedDirectoryName = SetVariables(directoryName, variables);
            string destinationDirectory = Path.Combine(destination, updatedDirectoryName);
            Directory.CreateDirectory(destinationDirectory);
            CopyFolder(requestModel, directoryPath, destinationDirectory, variables, ref files);
        }
    }

    private void CopyFile(IOGenerateBOPageFilesRequestModel requestModel, string source, string destination, Dictionary<string, string> variables, ref List<string> files)
    {
        string fileName = Path.GetFileName(source);
        string updatedFileName = SetVariables(fileName, variables);

        if (updatedFileName.Equals("__DefaultEnumType__.ts"))
        {
            CopyEnumFile(requestModel, source, destination, variables, ref files);
            return;
        }

        string destinationPath = Path.Combine(destination, updatedFileName);

        File.Copy(source, destinationPath);
        UpdateFileContent(destinationPath, variables);

        files.Add(destinationPath);
    }

    private void CopyEnumFile(IOGenerateBOPageFilesRequestModel requestModel, string source, string destination, Dictionary<string, string> variables, ref List<string> files)
    {
        string fileName = Path.GetFileName(source);

        IList<IOBOPageEntityModel> enumProperties = requestModel.Properties.Where(p => p.Type == IOBOPagePropertyType.Enum)
                                                                        .ToList();
        foreach (IOBOPageEntityModel enumProperty in enumProperties)
        {
            Dictionary<string, string> enumVariables = CreateEnumVariables(enumProperty);
            string updatedFileName = SetVariables(fileName, enumVariables);
            string destinationPath = Path.Combine(destination, updatedFileName);

            File.Copy(source, destinationPath);
            UpdateFileContent(destinationPath, enumVariables);

            files.Add(destinationPath);
        }
    }

    private void UpdateFileContent(string filePath, Dictionary<string, string> variables)
    {
        string fileContent = File.ReadAllText(filePath);
        string updatedContent = SetVariables(fileContent, variables);
        File.WriteAllText(filePath, updatedContent);
    }

    #endregion

    #region Variables

    private Dictionary<string, string> CreateVariables(IOGenerateBOPageFilesRequestModel requestModel)
    {
        string dataAccessAssemblyName = Configuration.GetValue<string>(IOConfigurationConstants.DataAccessAssembly);
        string projectName = dataAccessAssemblyName.Split(".").First();
        
        Dictionary<string, string> variables = new Dictionary<string, string>();
        variables.Add(".ft", "");
        variables.Add("__ProjectName__", projectName);
        variables.Add("__EntityName__", requestModel.EntityName);
        variables.Add("__EntityDisplayName__", requestModel.EntityDisplayName);
        variables.Add("__EntityDisplayNameLowercased__", requestModel.EntityDisplayName.ToLower());
        variables.Add("__ListEntityDisplayName__", requestModel.ListEntityDisplayName);
        variables.Add("__ListEntityName__", requestModel.ListEntityName);
        variables.Add("__EntityItemName__", requestModel.EntityItemName);
        variables.Add("__ListEntityAPIPath__", requestModel.ListEntityAPIPath);
        variables.Add("__UpdateEntityDisplayName__", requestModel.UpdateEntityDisplayName);
        variables.Add("__UpdateEntityName__", requestModel.UpdateEntityName);
        variables.Add("__UpdateEntityAPIPath__", requestModel.UpdateEntityAPIPath);
        variables.Add("__DeleteEntityDisplayName__", requestModel.DeleteEntityDisplayName);
        variables.Add("__DeleteEntityName__", requestModel.DeleteEntityName);
        variables.Add("__DeleteEntityAPIPath__", requestModel.DeleteEntityAPIPath);
        variables.Add("__CreateEntityDisplayName__", requestModel.CreateEntityDisplayName);
        variables.Add("__CreateEntityName__", requestModel.CreateEntityName);
        variables.Add("__CreateEntityAPIPath__", requestModel.CreateEntityAPIPath);

        string itemNameLowercased = requestModel.EntityItemName.ToLower();
        variables.Add("__EntityItemNameLowercased__", itemNameLowercased);

        string entitySelectProperties = "";
        string entityUpdateProperties = "";
        string entityModelProperties = "";
        string idPropertyName = "";
        string idJsonPropertyName = "";
        string uiEnumImports = "";
        string uiModelProperties = "";
        string uiConstructorProperties = "";
        string uiListDataHeaders = "";
        string uiItemListParameters = "";
        string uiItemListParameterArray = "";
        string uiIetmListUpdateParameters = "";
        string uiEntityUpdateIDProperty = "";
        string uiEntityUpdateProperties = "";
        string uiEntityUpdateDateProperties = "";
        string uiEntityUpdateFormProperties = "";

        int index = 0;
        foreach (IOBOPageEntityModel item in requestModel.Properties)
        {
            entitySelectProperties += String.Format("                                                        {0} = e.{1},\n", item.PropertyName, item.PropertyName);;
            entityUpdateProperties += String.Format("        {0}.{1} = requestModel.{2};\n", itemNameLowercased, item.PropertyName, item.PropertyName);

            if (!item.Nullable)
            {
                entityModelProperties += "    [Required]\n";
            }

            if (item.StringLength != null)
            {
                entityModelProperties += String.Format("    [StringLength({0})]\n", item.StringLength);
            }

            string propertyAPITypeName = PropertyAPITypeName(item);
            entityModelProperties += String.Format("    public {0} {1} {{ get; set; }}\n\n", propertyAPITypeName, item.PropertyName);

            if (item.Type == IOBOPagePropertyType.Enum)
            {
                uiEnumImports += String.Format("import {0} from \"../enumerations/{0}\";\n", propertyAPITypeName, propertyAPITypeName);
            }

            string itemUIModelData = String.Format("    {0}: {1};\n", item.PropertyJsonKey, PropertyUITypeName(item));
            uiModelProperties += itemUIModelData;

            string itemUIConstructorData = String.Format("        this.{0} = {1};\n", item.PropertyJsonKey, PropertyUIConstructorValue(item));
            uiConstructorProperties += itemUIConstructorData;

            uiListDataHeaders += String.Format("            '{0}',\n", item.PropertyName);
            uiItemListParameters += PropertyItemParameter(item, itemNameLowercased);
            uiItemListParameterArray += String.Format("                {0},\n", item.PropertyJsonKey);
            uiIetmListUpdateParameters += String.Format("        updateRequestModel.{0} = current{1}.{2};\n", item.PropertyJsonKey, requestModel.EntityItemName, item.PropertyJsonKey);

            if (item.PropertyJsonKey.Equals("id"))
            {
                idPropertyName = item.PropertyName;
                idJsonPropertyName = item.PropertyJsonKey;
                uiEntityUpdateIDProperty = String.Format("        request.{0} = this._updateRequest.{1};", item.PropertyJsonKey, item.PropertyJsonKey);
            }
            else
            {
                GenerateUIUpdateProperties(item, index, ref uiEntityUpdateProperties, ref uiEntityUpdateDateProperties, ref uiEntityUpdateFormProperties);
                index += 1;
            }
        }

        variables.Add("__EntitySelectProperties__", entitySelectProperties);
        variables.Add("__EntityIDProperty__", idPropertyName);
        variables.Add("__EntityIDJsonProperty__", idJsonPropertyName);
        variables.Add("__EntityModelProperties__", entityModelProperties);
        variables.Add("__EntityUpdateProperties__", entityUpdateProperties);
        variables.Add("__UIEnumImports__", uiEnumImports);
        variables.Add("__UIModelProperties__", uiModelProperties);
        variables.Add("__UIConstructorProperties__", uiConstructorProperties);
        variables.Add("__UIListDataHeaders__", uiListDataHeaders);
        variables.Add("__UIIetmListParameters__", uiItemListParameters);
        variables.Add("__UIIetmListParameterArray__", uiItemListParameterArray);
        variables.Add("__UIIetmListUpdateParameters__", uiIetmListUpdateParameters);
        variables.Add("__UIEntityUpdateIDProperty__", uiEntityUpdateIDProperty);
        variables.Add("__UIEntityUpdateProperties__", uiEntityUpdateProperties);
        variables.Add("__UIEntityUpdateDateProperties__", uiEntityUpdateDateProperties);
        variables.Add("__UIEntityUpdateFormProperties__", uiEntityUpdateFormProperties);

        return variables;
    }

    private Dictionary<string, string> CreateEnumVariables(IOBOPageEntityModel item)
    {
        Dictionary<string, string> variables = new Dictionary<string, string>();
        variables.Add(".ft", "");
        variables.Add("__DefaultEnumType__", item.EnumTypeName);
        
        string enumValues = "";
        string enumTypeNames = "";
        foreach (IOBOPageEntityCustomEnumTypeModel enumType in item.EnumType)
        {
            enumValues += String.Format("    {0} = {1},\n", enumType.Name, enumType.IntValue);
            enumTypeNames += String.Format("        if (type === {0}.{1}) {{\n            return \"{2}\";\n        }}\n\n", item.EnumTypeName, enumType.Name, enumType.Name);
        }

        IOBOPageEntityCustomEnumTypeModel lastEnumType = item.EnumType.Last();
        enumTypeNames += String.Format("        return \"{0}\";", lastEnumType.Name);

        variables.Add("__ENUM_VALUES__", enumValues);
        variables.Add("__ENUM_TYPE_NAMES__", enumTypeNames);
        return variables;
    }

    private string PropertyAPITypeName(IOBOPageEntityModel item)
    {
        return item.Type switch
        {
            IOBOPagePropertyType.Int => "int",
            IOBOPagePropertyType.String => "string",
            IOBOPagePropertyType.Double => "double",
            IOBOPagePropertyType.Float => "float",
            IOBOPagePropertyType.DateTimeOffset => "DateTimeOffset",
            IOBOPagePropertyType.Enum => item.EnumTypeName,
            _ => throw new ArgumentException("Input IOBOPagePropertyType is not defined."),
        };
    }

    private string PropertyUITypeName(IOBOPageEntityModel item)
    {
        return item.Type switch
        {
            IOBOPagePropertyType.Int => (item.Nullable) ? "number | null" : "number",
            IOBOPagePropertyType.String => (item.Nullable) ? "string | null" : "string",
            IOBOPagePropertyType.Double => (item.Nullable) ? "number | null" : "number",
            IOBOPagePropertyType.Float => (item.Nullable) ? "number | null" : "number",
            IOBOPagePropertyType.DateTimeOffset => (item.Nullable) ? "string | null" : "string",
            IOBOPagePropertyType.Enum => (item.Nullable) ? item.EnumTypeName + " | null" : item.EnumTypeName,
            _ => throw new ArgumentException("Input IOBOPagePropertyType is not defined."),
        };
    }

    private string PropertyUIConstructorValue(IOBOPageEntityModel item)
    {
        if (item.Nullable)
        {
            return "null";
        }

        return item.Type switch
        {
            IOBOPagePropertyType.Int => "0",
            IOBOPagePropertyType.String => "\"\"",
            IOBOPagePropertyType.Double => "0",
            IOBOPagePropertyType.Float => "0",
            IOBOPagePropertyType.DateTimeOffset => "\"\"",
            IOBOPagePropertyType.Enum => item.EnumTypeName + "." + item.EnumType.First().Name,
            _ => throw new ArgumentException("Input IOBOPagePropertyType is not defined."),
        };
    }

    private string PropertyItemParameter(IOBOPageEntityModel item, string itemNameLowercased)
    {
        if (item.Type == IOBOPagePropertyType.Enum)
        {
            return String.Format("            const {0} = {1}.get{2}Name({3}.{4});\n", item.PropertyJsonKey, item.EnumTypeName, item.EnumTypeName, itemNameLowercased, item.PropertyJsonKey);
        }

        string toStringMethod = item.Type switch
        {
            IOBOPagePropertyType.Int => ".toString()",
            IOBOPagePropertyType.String => "",
            IOBOPagePropertyType.Double => ".toString()",
            IOBOPagePropertyType.Float => ".toString()",
            IOBOPagePropertyType.DateTimeOffset => "",
            IOBOPagePropertyType.Enum => "",
            _ => throw new ArgumentException("Input IOBOPagePropertyType is not defined."),
        };

        if (item.Nullable)
        {
            return String.Format("            const {0} = ({1}.{2} == null) ? \"-\" : {3}.{4}{5};\n", item.PropertyJsonKey, itemNameLowercased, item.PropertyJsonKey, itemNameLowercased, item.PropertyJsonKey, toStringMethod);
        }
        else
        {
            return String.Format("            const {0} = {1}.{2}{3};\n", item.PropertyJsonKey, itemNameLowercased, item.PropertyJsonKey, toStringMethod);
        }
    }

    private void GenerateUIUpdateProperties(IOBOPageEntityModel item, int index, ref string uiEntityUpdateProperties, ref string uiEntityUpdateDateProperties, ref string uiEntityUpdateFormProperties)
    {
        switch (item.Type)
        {
            case IOBOPagePropertyType.Int:
            uiEntityUpdateProperties += String.Format("        request.{0} = Number(values[{1}]);\n", item.PropertyJsonKey, index);
                if (item.Nullable)
                {
                    uiEntityUpdateFormProperties += String.Format("            FormTypeNumberProps.initialize(\"{0}\", (this._updateRequest.{1} ?? 0).toString(), true),\n",
                    item.PropertyName, item.PropertyJsonKey);
                }
                else
                {
                    uiEntityUpdateFormProperties += String.Format("            FormTypeNumberProps.initializeWithValidations(\"{0}\", this._updateRequest.{1}.toString(), true, [ ValidationRequiredRule.initialize(\"{2} is required.\", \"Invalid {3}.\") ]),\n",
                    item.PropertyName, item.PropertyJsonKey, item.PropertyName, item.PropertyName);
                }
                break;

            case IOBOPagePropertyType.String:
                uiEntityUpdateProperties += String.Format("        request.{0} = values[{1}];\n", item.PropertyJsonKey, index);
                if (item.Nullable)
                {
                    uiEntityUpdateFormProperties += String.Format("            FormTypeTextProps.initialize(\"{0}\", this._updateRequest.{1} ?? \"\", true),\n",
                    item.PropertyName, item.PropertyJsonKey);
                }
                else
                {
                    uiEntityUpdateFormProperties += String.Format("            FormTypeTextProps.initializeWithValidations(\"{0}\", this._updateRequest.{1}, true, [ ValidationRequiredRule.initialize(\"{2} is required.\", \"Invalid {3}.\") ]),\n",
                    item.PropertyName, item.PropertyJsonKey, item.PropertyName, item.PropertyName);
                }
                break;

            case IOBOPagePropertyType.Double:
            case IOBOPagePropertyType.Float:
                uiEntityUpdateProperties += String.Format("        request.{0} = Number(values[{1}]);\n", item.PropertyJsonKey, index);
                if (item.Nullable)
                {
                    uiEntityUpdateFormProperties += String.Format("            FormTypeTextProps.initialize(\"{0}\", (this._updateRequest.{1} ?? 0).toString(), true),\n",
                    item.PropertyName, item.PropertyJsonKey);
                }
                else
                {
                    uiEntityUpdateFormProperties += String.Format("            FormTypeTextProps.initializeWithValidations(\"{0}\", this._updateRequest.{1}.toString(), true, [ ValidationRequiredRule.initialize(\"{2} is required.\", \"Invalid {3}.\") ]),\n",
                    item.PropertyName, item.PropertyJsonKey, item.PropertyName, item.PropertyName);
                }
                break;

            case IOBOPagePropertyType.DateTimeOffset:
                uiEntityUpdateProperties += String.Format("        request.{0} = values[{1}];\n", item.PropertyJsonKey, index);
                uiEntityUpdateDateProperties += String.Format("        let {0}String = \"\";\n        if (this._updateRequest.{1} != null) {{\n            const {2} = new Date(this._updateRequest.{3});\n            {4}String = this.formatDate({5});\n        }}\n\n",
                item.PropertyJsonKey, item.PropertyJsonKey, item.PropertyJsonKey, item.PropertyJsonKey, item.PropertyJsonKey, item.PropertyJsonKey);

                if (item.Nullable)
                {
                    uiEntityUpdateFormProperties += String.Format("            FormTypeDateProps.initialize(\"{0}\", {1}String, true),\n",
                    item.PropertyName, item.PropertyJsonKey);
                }
                else
                {
                    uiEntityUpdateFormProperties += String.Format("            FormTypeDateProps.initializeWithValidations(\"{0}\", {1}String, true, [ ValidationRequiredRule.initialize(\"{2} is required.\", \"Invalid {3}.\") ]),\n",
                    item.PropertyName, item.PropertyJsonKey, item.PropertyName, item.PropertyName);
                }
                break;

            case IOBOPagePropertyType.Enum:
                uiEntityUpdateProperties += String.Format("        request.{0} = Number(values[{1}]);\n", item.PropertyJsonKey, index);
                string enumCases = "";
                foreach (IOBOPageEntityCustomEnumTypeModel enumItem in item.EnumType)
                {
                    enumCases += String.Format("                FormDataOptionModel.initialize({0}.get{1}Name({2}.{3}), {4}.{5}.toString()),\n",
                    item.EnumTypeName, item.EnumTypeName, item.EnumTypeName, enumItem.Name, item.EnumTypeName, enumItem.Name);
                }

                if (item.Nullable)
                {
                    uiEntityUpdateFormProperties += String.Format("            FormTypeSelectProps.initialize(\"{0}\", this._updateRequest.{1}.toString(), true, [\n{2}            ]),\n",
                    item.PropertyName, item.PropertyJsonKey, enumCases);
                }
                else
                {
                    uiEntityUpdateFormProperties += String.Format("            FormTypeSelectProps.initializeWithValidations(\"{0}\", this._updateRequest.{1}.toString(), true, [\n{2}            ], [ ValidationRequiredRule.initialize(\"{3} is required.\", \"Invalid {4}.\") ]),\n",
                    item.PropertyName, item.PropertyJsonKey, enumCases, item.PropertyName, item.PropertyName);
                }
                break;
        }
    }

    private string SetVariables(string from, Dictionary<string, string> variables)
    {
        string output = from;
        foreach (KeyValuePair<string, string> item in variables)
        {
            if (output.Contains(item.Key))
            {
                output = output.Replace(item.Key, item.Value);
            }
        }

        return output;
    }

    #endregion

    #region Zip

    private string ZipFolder(string source, IList<string> files, string generatedZipFileName)
    {
        string zipPath = Path.Join(source, generatedZipFileName);

        using (FileStream zipToOpen = new FileStream(zipPath, FileMode.Create))
        {
            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create, true))
            {
                foreach (string file in files)
                {
                    archive.CreateEntryFromFile(file, file.Replace(source, ""), CompressionLevel.Optimal);
                }
            }
        }

        return zipPath;
    }

    #endregion
}
