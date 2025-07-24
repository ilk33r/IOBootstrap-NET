using System;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.BackOffice.Logs.ViewModels;

public class IOBackOfficeLogsDefaultViewModel : IOBackOfficeLogsViewModel<IODatabaseContextDefaultImpl>
{
    public IOBackOfficeLogsDefaultViewModel() : base()
    {
    }
}
