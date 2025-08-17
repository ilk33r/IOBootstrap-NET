#if DEBUG
using System.Threading.Tasks;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.WebApi.DatabaseContentGenerator.ViewModels;

public class IODatabaseContentGeneratorViewModel<TDBContext> : IOViewModel<TDBContext>
    where TDBContext : IODatabaseContext<TDBContext>
{

    public override void CheckAuthorizationHeader()
    {
#if DEBUG
        return;
#else
        base.CheckAuthorizationHeader();
#endif
    }

    public async Task CreateBOUser(string userName)
    {
        using (var passwordUtilities = new IOPasswordUtilities())
        {
            await passwordUtilities.HashPassword("admin", hashed =>
            {
                IOUserEntity userEntity = new IOUserEntity()
                {
                    UserName = userName,
                    Password = hashed,
                    UserRole = 999,
                    UserToken = "",
                    TokenDate = new DateTimeOffset(),
                    IsActive = false,
                    ActivationEndDate = DateTimeOffset.UtcNow,
                    CreatedBy = "swagger-ui",
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdateDate = DateTimeOffset.UtcNow,
                    WrongPasswordAttemptCount = 0,
                    PasswordExpireDate = DateTimeOffset.UtcNow.AddYears(-1),
                    LastWrongPasswordAttemptDate = new DateTimeOffset()
                };

                DatabaseContext.Add(userEntity);
                DatabaseContext.SaveChanges();
            });
        }
    }

    public void CreateBODefaultData()
    {
        AddDefaultConfiguration();
        GenerateUserMenu();
        GenerateConfigurationMenu();
        GenerateMenuEditorMenu();
        GenerateMessagesMenu();
        GenerateNotificationMenu();
        GenerateImagesMenu();
        GenerateTemplatesMenu();
        GenerateLogsMenu();
    }

    private void AddDefaultConfiguration()
    {
        IOConfigurationEntity isMaintenanceModeOn = new IOConfigurationEntity()
        {
            ConfigKey = IOConfigurationKeys.IsMaintenanceModeOn,
            ConfigIntValue = 0,
            ConfigStringValue = null
        };
        DatabaseContext.Add(isMaintenanceModeOn);

        IOConfigurationEntity forgotPasswordEmailTitle = new IOConfigurationEntity()
        {
            ConfigKey = "ForgotPasswordEmailTitle",
            ConfigIntValue = 0,
            ConfigStringValue = "IOBootstrapt Reset Password"
        };
        DatabaseContext.Add(forgotPasswordEmailTitle);

        IOConfigurationEntity forgotPasswordEmailHtmlBody = new IOConfigurationEntity()
        {
            ConfigKey = "ForgotPasswordEmailHtmlBody",
            ConfigIntValue = 0,
            ConfigStringValue = "<p>This email has been sent upon your \'Change My Password\' request. If you don\'t have such request please ignore this email.\nPlease click to the link to  <a href=\"{0}\">Change your password.</a></p>\n<br />\n<p>Best Regards, IOBootstrapt</p>"
        };
        DatabaseContext.Add(forgotPasswordEmailHtmlBody);

        IOConfigurationEntity forgotPasswordEmailTextBody = new IOConfigurationEntity()
        {
            ConfigKey = "ForgotPasswordEmailTextBody",
            ConfigIntValue = 0,
            ConfigStringValue = "This email has been sent upon your \'Change My Password\' request. If you don\'t have such request please ignore this email.\nPlease click to the link to  {0}\n\nBest Regards, IOBootstrapt"
        };
        DatabaseContext.Add(forgotPasswordEmailTextBody);

        IOConfigurationEntity eMailFromName = new IOConfigurationEntity()
        {
            ConfigKey = "EMailFromName",
            ConfigIntValue = 0,
            ConfigStringValue = "IOBootstrapt Support"
        };
        DatabaseContext.Add(eMailFromName);
        DatabaseContext.SaveChanges();
    }

    private void GenerateUserMenu()
    {
        IOMenuEntity usersEntity = new IOMenuEntity()
        {
            Action = "actionUsers",
            CssClass = "fa-users",
            Name = "Users",
            MenuOrder = 6,
            RequiredRole = (int)UserRoles.Admin,
            ParentEntityID = null
        };
        DatabaseContext.Add(usersEntity);

        IOMenuEntity userListEntity = new IOMenuEntity()
        {
            Action = "usersList",
            CssClass = "fa-circle-o",
            Name = "List Users",
            MenuOrder = 7,
            RequiredRole = (int)UserRoles.Admin,
            ParentEntityID = null
        };
        DatabaseContext.Add(userListEntity);

        IOMenuEntity userAddEntity = new IOMenuEntity()
        {
            Action = "usersAdd",
            CssClass = "fa-circle-o",
            Name = "Add User",
            MenuOrder = 8,
            RequiredRole = (int)UserRoles.Admin,
            ParentEntityID = null
        };
        DatabaseContext.Add(userAddEntity);
        DatabaseContext.SaveChanges();
    }

    private void GenerateConfigurationMenu()
    {
        IOMenuEntity configurationEntity = new IOMenuEntity()
        {
            Action = "actionConfiguration",
            CssClass = "fa-wrench",
            Name = "Configurations",
            MenuOrder = 11,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(configurationEntity);

        IOMenuEntity configurationListEntity = new IOMenuEntity()
        {
            Action = "configurationsList",
            CssClass = "fa-circle-o",
            Name = "Edit Configurations",
            MenuOrder = 12,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(configurationListEntity);

        IOMenuEntity configurationAddEntity = new IOMenuEntity()
        {
            Action = "configurationsAdd",
            CssClass = "fa-circle-o",
            Name = "Add Configuration",
            MenuOrder = 13,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(configurationAddEntity);

        IOMenuEntity recycleAppEntity = new IOMenuEntity()
        {
            Action = "resetCache",
            CssClass = "fa-circle-o",
            Name = "Reset Cache",
            MenuOrder = 14,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(recycleAppEntity);
        DatabaseContext.SaveChanges();
    }

    private void GenerateMenuEditorMenu()
    {
        IOMenuEntity menuEditorEntity = new IOMenuEntity()
        {
            Action = "actionMenuEditor",
            CssClass = "fa-list",
            Name = "Menu Editor",
            MenuOrder = 15,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(menuEditorEntity);

        IOMenuEntity menuEditorListMenuEntity = new IOMenuEntity()
        {
            Action = "menuEditorList",
            CssClass = "fa-circle-o",
            Name = "List Menu Items",
            MenuOrder = 16,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(menuEditorListMenuEntity);

        IOMenuEntity menuEditorAddMenuEntity = new IOMenuEntity()
        {
            Action = "menuEditorAdd",
            CssClass = "fa-circle-o",
            Name = "Add Menu Item",
            MenuOrder = 17,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(menuEditorAddMenuEntity);
        DatabaseContext.SaveChanges();
    }

    private void GenerateMessagesMenu()
    {
        IOMenuEntity messagesEntity = new IOMenuEntity()
        {
            Action = "actionMessages",
            CssClass = "fa-envelope",
            Name = "Messages",
            MenuOrder = 18,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(messagesEntity);

        IOMenuEntity messagesListEntity = new IOMenuEntity()
        {
            Action = "messagesList",
            CssClass = "fa-circle-o",
            Name = "List Messages",
            MenuOrder = 19,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(messagesListEntity);

        IOMenuEntity messagesAddEntity = new IOMenuEntity()
        {
            Action = "messagesAdd",
            CssClass = "fa-circle-o",
            Name = "Add Message",
            MenuOrder = 20,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(messagesAddEntity);
        DatabaseContext.SaveChanges();
    }

    private void GenerateNotificationMenu()
    {
        IOMenuEntity notificationEntity = new IOMenuEntity()
        {
            Action = "actionPushNotification",
            CssClass = "fa-comment-alt",
            Name = "Push Notifications",
            MenuOrder = 21,
            RequiredRole = (int)UserRoles.User,
            ParentEntityID = null
        };
        DatabaseContext.Add(notificationEntity);

        IOMenuEntity listNotificationEntity = new IOMenuEntity()
        {
            Action = "pushNotificationList",
            CssClass = "fa-circle-o",
            Name = "List Messages",
            MenuOrder = 22,
            RequiredRole = (int)UserRoles.User,
            ParentEntityID = null
        };
        DatabaseContext.Add(listNotificationEntity);

        IOMenuEntity sendNotificationEntity = new IOMenuEntity()
        {
            Action = "pushNotificationSend",
            CssClass = "fa-circle-o",
            Name = "Send",
            MenuOrder = 23,
            RequiredRole = (int)UserRoles.User,
            ParentEntityID = null
        };
        DatabaseContext.Add(sendNotificationEntity);
        DatabaseContext.SaveChanges();
    }

    private void GenerateImagesMenu()
    {
        IOMenuEntity imagesEntity = new IOMenuEntity()
        {
            Action = "actionImages",
            CssClass = "fa-image",
            Name = "Images",
            MenuOrder = 27,
            RequiredRole = (int)UserRoles.Admin,
            ParentEntityID = null
        };
        DatabaseContext.Add(imagesEntity);

        IOMenuEntity imagesListEntity = new IOMenuEntity()
        {
            Action = "imagesEdit",
            CssClass = "fa-circle-o",
            Name = "Edit Images",
            MenuOrder = 28,
            RequiredRole = (int)UserRoles.Admin,
            ParentEntityID = null
        };
        DatabaseContext.Add(imagesListEntity);

        IOMenuEntity imageAddEntity = new IOMenuEntity()
        {
            Action = "imageAdd",
            CssClass = "fa-circle-o",
            Name = "Add Image",
            MenuOrder = 29,
            RequiredRole = (int)UserRoles.Admin,
            ParentEntityID = null
        };
        DatabaseContext.Add(imageAddEntity);
        DatabaseContext.SaveChanges();
    }

    private void GenerateTemplatesMenu()
    {
        IOMenuEntity templatesEntity = new IOMenuEntity()
        {
            Action = "actionGenerateBOPage",
            CssClass = "fa-file-code",
            Name = "Generate BO Page",
            MenuOrder = 30,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(templatesEntity);
        DatabaseContext.SaveChanges();
    }

    private void GenerateLogsMenu()
    {
        IOMenuEntity templatesEntity = new IOMenuEntity()
        {
            Action = "logsEdit",
            CssClass = "fa-book",
            Name = "Logs",
            MenuOrder = 31,
            RequiredRole = (int)UserRoles.SuperAdmin,
            ParentEntityID = null
        };
        DatabaseContext.Add(templatesEntity);
        DatabaseContext.SaveChanges();
    }
}
#endif