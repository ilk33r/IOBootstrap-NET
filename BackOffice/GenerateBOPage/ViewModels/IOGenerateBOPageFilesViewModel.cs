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
        CopyFolder(templatePath, generatedFolderPath, variables, ref files);

        return ZipFolder(generatedFolderPath, files, generatedZipFileName);
    }

    #endregion

    #region File Methods

    private void CopyFolder(string source, string destination, Dictionary<string, string> variables, ref List<string> files)
    {
        string[] filePaths = Directory.GetFiles(source);
        foreach (string filePath in filePaths)
        {
            CopyFile(filePath, destination, variables, ref files);
        }

        string[] directories = Directory.GetDirectories(source);
        foreach (string directoryPath in directories)
        {
            string directoryName = Path.GetFileName(directoryPath);
            string updatedDirectoryName = SetVariables(directoryName, variables);
            string destinationDirectory = Path.Combine(destination, updatedDirectoryName);
            Directory.CreateDirectory(destinationDirectory);
            CopyFolder(directoryPath, destinationDirectory, variables, ref files);
        }
    }

    private void CopyFile(string source, string destination, Dictionary<string, string> variables, ref List<string> files)
    {
        string fileName = Path.GetFileName(source);
        string updatedFileName = SetVariables(fileName, variables);
        string destinationPath = Path.Combine(destination, updatedFileName);

        File.Copy(source, destinationPath);
        UpdateFileContent(destinationPath, variables);

        files.Add(destinationPath);
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
        variables.Add("__ListEntityDisplayName__", requestModel.ListEntityDisplayName);
        variables.Add("__ListEntityName__", requestModel.ListEntityName);
        variables.Add("__EntityItemName__", requestModel.EntityItemName);

        string entitySelectProperties = "";
        string entityModelProperties = "";
        string idPropertyName = "";
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
            string itemModelData = String.Format("    public {0} {1} {{ get; set; }}\n\n", PropertyAPITypeName(item.Type), item.PropertyName);
            entityModelProperties += itemModelData;
        }

        variables.Add("__EntitySelectProperties__", entitySelectProperties);
        variables.Add("__EntityIDProperty__", idPropertyName);
        variables.Add("__EntityModelProperties__", entityModelProperties);

        return variables;
    }

    private string PropertyAPITypeName(IOBOPagePropertyType type)
    {
        return type switch
        {
            IOBOPagePropertyType.Int => "int",
            IOBOPagePropertyType.String => "string",
            IOBOPagePropertyType.Double => "double",
            IOBOPagePropertyType.Float => "float",
            IOBOPagePropertyType.DateTimeOffset => "DateTimeOffset",
            IOBOPagePropertyType.Enum => "Enum",
            _ => throw new ArgumentException("Input IOBOPagePropertyType is not defined."),
        };
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
