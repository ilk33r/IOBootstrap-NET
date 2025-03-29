using System;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Users;

public class IOUserInfoModel : IOModel
{

	#region Properties

	public int ID { get; set; }
	public string? Password { get; set; }
	public string? UserName { get; set; }
	public int UserRole { get; set; }
	public string? UserToken { get; set; }
	public DateTimeOffset TokenDate { get; set; }
	public bool IsActive { get; set; }
    public DateTimeOffset ActivationEndDate { get; set; }
	public string? CreatedBy { get; set; }
	public DateTimeOffset CreatedDate { get; set; }
	public DateTimeOffset UpdateDate { get; set; }

	#endregion

}
