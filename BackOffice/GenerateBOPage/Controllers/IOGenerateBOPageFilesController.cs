using IOBootstrap.NET.Common;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice;

[IOBackoffice]
public class IOGenerateBOPageFilesController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
where TViewModel : IOGenerateBOPageFilesViewModel<TDBContext>, new()
{
    #region Controller Lifecycle

    public IOGenerateBOPageFilesController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<IOLoggerType> logger, TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    #region Controller Methods

    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateAPIFiles([FromBody] IOGenerateBOPageFilesRequestModel requestModel)
    {
        string projectDir = Environment.ContentRootPath;
        string generatedFolderName = "GeneratedAPI";
        string generatedZipFileName = "APIFiles.zip";
        string apiFilesPath = ViewModel.CreateAPIFiles(requestModel, projectDir, generatedFolderName, generatedZipFileName);
        byte[] result = await System.IO.File.ReadAllBytesAsync(apiFilesPath);

        FileContentResult fileResult = File(result, "application/octet-stream", generatedZipFileName);
        
        string tempPath = Path.GetTempPath();
        string generatedFolderPath = Path.Join(tempPath, generatedFolderName);

        if (Directory.Exists(generatedFolderPath))
        {
            Directory.Delete(generatedFolderPath, true);
        }

        return fileResult;
    }

    #endregion
}
