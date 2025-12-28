using System;
using IOBootstrap.NET.Common.Messages.Logs;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using IOBootstrap.NET.Core.Interfaces;

namespace IOBootstrap.NET.BackOffice.Logs.ViewModels;

public class IOBackOfficeLogsViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>, IIOImageViewModel, IIOImageAssetViewModel
where TDBContext : IOBaseDatabaseContext<TDBContext>
{
    #region Initialization Methods

    public IOBackOfficeLogsViewModel() : base()
    {
    }

    #endregion

    #region View Model Methods

    public IOGetLogsResponseModel GetLogs(IOGetLogsRequestModel requestModel)
    {
        IQueryable<IOLogsEntity> logs = DatabaseContext.Logs;
        int logCount = logs.Count();
        IList<IOLogModel> paginatedImages = logs
                                                .Select(log => new IOLogModel()
                                                {
                                                    ID = log.ID,
                                                    RequestDate = log.RequestDate,
                                                    IPV4 = log.IPV4,
                                                    Port = log.Port,
                                                    ResponseCode = log.ResponseCode,
                                                    RequestPath = log.RequestPath,
                                                    RequestHeaders = log.RequestHeaders,
                                                    ResponseHeaders = log.ResponseHeaders,
                                                    RequestBody = log.RequestBody,
                                                    ResponseBody = log.ResponseBody
                                                })
                                                .OrderByDescending(i => i.ID)
                                                .Skip(requestModel.Start ?? 0)
                                                .Take(requestModel.Count ?? 0)
                                                .ToList();

        return new IOGetLogsResponseModel(logCount, paginatedImages);
    }

    public IOGetLogsResponseModel GetExceptions(IOGetLogsRequestModel requestModel)
    {
        IQueryable<IOExceptionEntity> exceptions = DatabaseContext.Exceptions;
        int exceptionCount = exceptions.Count();
        IList<IOLogModel> paginatedImages = exceptions
                                                .Select(log => new IOLogModel()
                                                {
                                                    ID = log.ID,
                                                    RequestDate = log.RequestDate,
                                                    RequestPath = log.RequestPath,
                                                    RequestHeaders = log.RequestHeaders,
                                                    ResponseHeaders = log.ExceptionMessage,
                                                    RequestBody = log.RequestBody,
                                                    ResponseBody = log.ExceptionStackTrace
                                                })
                                                .OrderByDescending(i => i.RequestDate)
                                                .Skip(requestModel.Start ?? 0)
                                                .Take(requestModel.Count ?? 0)
                                                .ToList();

        return new IOGetLogsResponseModel(exceptionCount, paginatedImages);
    }

    #endregion
}
