using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Exceptions.Files;
using IOBootstrap.NET.Common.Extensions;
using IOBootstrap.NET.Common.Messages.Files;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Extensions;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.BackOffice.Files.ViewModels;

public class IOBackOfficeFilesViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>, IIOFileViewModel, IIOFileAssetViewModel
where TDBContext : IOBaseDatabaseContext<TDBContext>
{
    #region Initialization Methods

    public IOBackOfficeFilesViewModel() : base()
    {
    }

    #endregion

    #region View Model Methods

    public void CheckFilesIsEnabled()
    {
        bool isEnabled = Configuration.GetValue<bool>(IOConfigurationConstants.ImagesEnabled);
        if (!isEnabled)
        {
            throw new IOInvalidAPIException();
        }
    }

    public IOGetFilesResponseModel GetFiles(IOGetFilesRequestModel requestModel)
    {
        IQueryable<IOFilesEntity> files = DatabaseContext.Files;

        string? description = requestModel.Description?.SanitizeHtml().Trim();
        if (!String.IsNullOrEmpty(description))
        {
            files = files.Where(i => i.Description == description);
        }

        int fileCount = files.Count();
        IList<IOFileVariationsModel> paginatedFiles = files
                                                        .Select(i => new IOFileVariationsModel()
                                                        {
                                                            ID = i.ID,
                                                            FileName = i.FileName,
                                                            FileType = i.FileType,
                                                            Description = i.Description,
                                                            AdditionalData = i.AdditionalData,
                                                            CreatedBy = i.CreatedBy,
                                                            CreatedDate = i.CreatedDate
                                                        })
                                                        .OrderBy(i => i.ID)
                                                        .Skip(requestModel.Start ?? 0)
                                                        .Take(requestModel.Count ?? 0)
                                                        .ToList();

        foreach (IOFileVariationsModel file in paginatedFiles)
        {
            file.PublicId = this.CreateFilePublicId(file.FileName ?? string.Empty);
        }

        return new IOGetFilesResponseModel(fileCount, paginatedFiles);
    }

    public string SaveFile(IFormFile file)
    {
        return this.SaveRawFile(file);
    }

    public IOFileVariationsModel SaveFilesMetaData(string filePath, string contentType, string description, string additionalData)
    {
        string fileName = Path.GetFileName(filePath);
        string sanitizedDescription = description.SanitizeHtml().Trim();

        if (String.IsNullOrWhiteSpace(sanitizedDescription) || sanitizedDescription.Length > 128)
        {
            throw new IOInvalidRequestException();
        }

        if (!File.Exists(filePath))
        {
            throw new IOFileNotFoundException();
        }

        IOFilesEntity fileEntity = new IOFilesEntity()
        {
            FileName = fileName,
            FileType = contentType,
            Description = sanitizedDescription,
            AdditionalData = additionalData,
            CreatedBy = UserModel?.UserName,
            CreatedDate = DateTimeOffset.UtcNow
        };

        DatabaseContext.Add(fileEntity);
        DatabaseContext.SaveChanges();

        return new IOFileVariationsModel()
        {
            ID = fileEntity.ID,
            FileName = fileEntity.FileName,
            FileType = fileEntity.FileType,
            Description = fileEntity.Description,
            AdditionalData = fileEntity.AdditionalData,
            CreatedBy = fileEntity.CreatedBy,
            CreatedDate = fileEntity.CreatedDate
        };
    }

    public virtual void DeleteFile(IODeleteFilesRequestModel requestModel)
    {
        if (requestModel.FileId == null)
        {
            throw new IOInvalidRequestException();
        }

        IOFilesEntity? fileEntity = DatabaseContext.Files.Find(requestModel.FileId);
        if (fileEntity == null)
        {
            throw new IOFileNotFoundException();
        }

        if (fileEntity.FileName != null)
        {
            this.RemoveFile(fileEntity.FileName);
        }

        DatabaseContext.Remove(fileEntity);
        DatabaseContext.SaveChanges();
    }

    #endregion
}
