using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Users;

namespace IOBootstrap.NET.Common.Messages.Users;

public class IOListUserResponseModel : IOResponseModel
{
	#region Properties
	public int Count { get; set; }
	public IList<IOUserInfoModel> Users { get; }

	#endregion

	#region Initialization Methods

	public IOListUserResponseModel(int count, IList<IOUserInfoModel> users) : base()
	{
		// Setup properties
		Count = count;
		Users = users;
	}

	#endregion

}
