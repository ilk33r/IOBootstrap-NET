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

        string itemNameLowercased = requestModel.EntityItemName.ToLower();
        variables.Add("__EntityItemNameLowercased__", itemNameLowercased);

        string entitySelectProperties = "";
        string entityModelProperties = "";
        string idPropertyName = "";
        string uiEnumImports = "";
        string uiModelProperties = "";
        string uiConstructorProperties = "";
        string uiListDataHeaders = "";
        string uiItemListParameters = "";
        string uiItemListParameterArray = "";

        foreach (IOBOPageEntityModel item in requestModel.Properties)
        {
            if (item.PropertyJsonKey.Equals("id"))
            {
                idPropertyName = item.PropertyName;
            }

            string itemData = String.Format("                                                        {0} = e.{1},\n", item.PropertyName, item.PropertyName);
            entitySelectProperties += itemData;

            if (!item.Nullable)
            {
                entityModelProperties += "    [Required]\n";
            }

            string propertyAPITypeName = PropertyAPITypeName(item);
            string itemModelData = String.Format("    public {0} {1} {{ get; set; }}\n\n", propertyAPITypeName, item.PropertyName);
            entityModelProperties += itemModelData;

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
        }

        variables.Add("__EntitySelectProperties__", entitySelectProperties);
        variables.Add("__EntityIDProperty__", idPropertyName);
        variables.Add("__EntityModelProperties__", entityModelProperties);
        variables.Add("__UIEnumImports__", uiEnumImports);
        variables.Add("__UIModelProperties__", uiModelProperties);
        variables.Add("__UIConstructorProperties__", uiConstructorProperties);
        variables.Add("__UIListDataHeaders__", uiListDataHeaders);
        variables.Add("__UIIetmListParameters__", uiItemListParameters);
        variables.Add("__UIIetmListParameterArray__", uiItemListParameterArray);

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
