using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.MW.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.MW.DataAccess.Context
{
    public abstract class IODatabaseContext<TContext> : DbContext where TContext : DbContext
    {

        public virtual DbSet<IOConfigurationEntity> Configurations { get; set; }
        public virtual DbSet<IOClientsEntity> Clients { get; set; }
        public virtual DbSet<IOImagesEntity> Images { get; set; }
        public virtual DbSet<IOMenuEntity> Menu { get; set; }
        public virtual DbSet<IOBackOfficeMessageEntity> Messages { get; set; }
        public virtual DbSet<IOUserEntity> Users { get; set; }
        public virtual DbSet<PushNotificationEntity> PushNotifications { get; set; }
        public virtual DbSet<PushNotificationMessageEntity> PushNotificationMessages { get; set; }
        public virtual DbSet<PushNotificationDeliveredMessagesEntity> PushNotificationDeliveredMessages { get; set; }

        public IODatabaseContext(DbContextOptions<TContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IOConfigurationEntity>().HasIndex(
                configurationEntity => new { configurationEntity.ConfigKey }).IsUnique(true);

            modelBuilder.Entity<IOClientsEntity>().HasIndex(
                clientEntity => new { clientEntity.ClientId }).IsUnique(true);

            modelBuilder.Entity<IOMenuEntity>().HasIndex(
                menuEntity => new { menuEntity.ParentEntityID, menuEntity.MenuOrder, menuEntity.RequiredRole }).IsUnique(false);

            modelBuilder.Entity<IOUserEntity>().HasIndex(
                userEntity => new { userEntity.UserName }).IsUnique(true);

            modelBuilder.Entity<PushNotificationEntity>().HasIndex(
                pushNotificationEntity => new
                {
                    pushNotificationEntity.DeviceId,
                    pushNotificationEntity.DeviceType,
                    pushNotificationEntity.LastUpdateTime
                }).IsUnique(false);
                
            modelBuilder.Entity<PushNotificationMessageEntity>().HasIndex(
                pushNotificationMessageEntity => new
                {
                    pushNotificationMessageEntity.NotificationDate,
                    pushNotificationMessageEntity.DeviceType,
                    pushNotificationMessageEntity.IsCompleted
                });

            modelBuilder.Entity<IOBackOfficeMessageEntity>().HasIndex(
                messagesEntity => new
                {
                    messagesEntity.MessageCreateDate,
                    messagesEntity.MessageEndDate,
                    messagesEntity.MessageStartDate
                });

            AddDefaultConfiguration(modelBuilder);
            GenerateClientMenu(modelBuilder);
            GenerateUserMenu(modelBuilder);
            GenerateConfigurationMenu(modelBuilder);
            GenerateMenuEditorMenu(modelBuilder);
            GenerateMessagesMenu(modelBuilder);
            GenerateNotificationMenu(modelBuilder);
            GenerateResourceMenu(modelBuilder);
            GenerateImagesMenu(modelBuilder);
        }

        private void AddDefaultConfiguration(ModelBuilder modelBuilder)
        {
            IOConfigurationEntity isMaintenanceModeOn = new IOConfigurationEntity()
            {
                ID = 1,
                ConfigKey = IOConfigurationKeys.IsMaintenanceModeOn,
                ConfigIntValue = 0,
                ConfigStringValue = null
            };
            modelBuilder.Entity<IOConfigurationEntity>().HasData(isMaintenanceModeOn);
        }

        private void GenerateClientMenu(ModelBuilder modelBuilder)
        {
            IOMenuEntity clientsEntity = new IOMenuEntity()
            {
                ID = 1,
                Action = "actionClients",
                CssClass = "fa-cloud-download",
                Name = "Clients",
                MenuOrder = 1,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = null
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(clientsEntity);

            IOMenuEntity clientListEntity = new IOMenuEntity()
            {
                ID = 2,
                Action = "clientsList",
                CssClass = "fa-circle-o",
                Name = "List Clients",
                MenuOrder = 2,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = 1
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(clientListEntity);

            IOMenuEntity clientAddEntity = new IOMenuEntity()
            {
                ID = 3,
                Action = "clientsAdd",
                CssClass = "fa-circle-o",
                Name = "Add Client",
                MenuOrder = 3,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = 1
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(clientAddEntity);
        }

        private void GenerateUserMenu(ModelBuilder modelBuilder)
        {
            IOMenuEntity usersEntity = new IOMenuEntity()
            {
                ID = 6,
                Action = "actionUsers",
                CssClass = "fa-users",
                Name = "Users",
                MenuOrder = 6,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = null
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(usersEntity);

            IOMenuEntity userListEntity = new IOMenuEntity()
            {
                ID = 7,
                Action = "usersList",
                CssClass = "fa-circle-o",
                Name = "List Users",
                MenuOrder = 7,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = 6
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(userListEntity);

            IOMenuEntity userAddEntity = new IOMenuEntity()
            {
                ID = 8,
                Action = "usersAdd",
                CssClass = "fa-circle-o",
                Name = "Add User",
                MenuOrder = 8,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = 6
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(userAddEntity);
        }

        private void GenerateConfigurationMenu(ModelBuilder modelBuilder)
        {
            IOMenuEntity configurationEntity = new IOMenuEntity()
            {
                ID = 11,
                Action = "actionConfiguration",
                CssClass = "fa-wrench",
                Name = "Configurations",
                MenuOrder = 11,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = null
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(configurationEntity);

            IOMenuEntity configurationListEntity = new IOMenuEntity()
            {
                ID = 12,
                Action = "configurationsList",
                CssClass = "fa-circle-o",
                Name = "Edit Configurations",
                MenuOrder = 12,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = 11
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(configurationListEntity);

            IOMenuEntity configurationAddEntity = new IOMenuEntity()
            {
                ID = 13,
                Action = "configurationsAdd",
                CssClass = "fa-circle-o",
                Name = "Add Configuration",
                MenuOrder = 13,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = 11
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(configurationAddEntity);

            IOMenuEntity recycleAppEntity = new IOMenuEntity()
            {
                ID = 14,
                Action = "resetCache",
                CssClass = "fa-circle-o",
                Name = "Reset Cache",
                MenuOrder = 14,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = 11
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(recycleAppEntity);
        }

        private void GenerateMenuEditorMenu(ModelBuilder modelBuilder)
        {
            IOMenuEntity menuEditorEntity = new IOMenuEntity()
            {
                ID = 15,
                Action = "actionMenuEditor",
                CssClass = "fa-list",
                Name = "Menu Editor",
                MenuOrder = 15,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = null
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(menuEditorEntity);

            IOMenuEntity menuEditorListMenuEntity = new IOMenuEntity()
            {
                ID = 16,
                Action = "menuEditorList",
                CssClass = "fa-circle-o",
                Name = "List Menu Items",
                MenuOrder = 16,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = 15
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(menuEditorListMenuEntity);

            IOMenuEntity menuEditorAddMenuEntity = new IOMenuEntity()
            {
                ID = 17,
                Action = "menuEditorAdd",
                CssClass = "fa-circle-o",
                Name = "Add Menu Item",
                MenuOrder = 17,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = 15
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(menuEditorAddMenuEntity);
        }

        private void GenerateMessagesMenu(ModelBuilder modelBuilder)
        {
            IOMenuEntity messagesEntity = new IOMenuEntity()
            {
                ID = 18,
                Action = "actionMessages",
                CssClass = "fa-envelope",
                Name = "Messages",
                MenuOrder = 18,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = null
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(messagesEntity);

            IOMenuEntity messagesListEntity = new IOMenuEntity()
            {
                ID = 19,
                Action = "messagesList",
                CssClass = "fa-circle-o",
                Name = "List Messages",
                MenuOrder = 19,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = 18
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(messagesListEntity);

            IOMenuEntity messagesAddEntity = new IOMenuEntity()
            {
                ID = 20,
                Action = "messagesAdd",
                CssClass = "fa-circle-o",
                Name = "Add Message",
                MenuOrder = 20,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = 18
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(messagesAddEntity);
        }

        private void GenerateNotificationMenu(ModelBuilder modelBuilder)
        {
            IOMenuEntity notificationEntity = new IOMenuEntity()
            {
                ID = 21,
                Action = "actionPushNotification",
                CssClass = "fa-send-o",
                Name = "Push Notifications",
                MenuOrder = 21,
                RequiredRole = (int)UserRoles.User,
                ParentEntityID = null
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(notificationEntity);

            IOMenuEntity listNotificationEntity = new IOMenuEntity()
            {
                ID = 22,
                Action = "pushNotificationList",
                CssClass = "fa-circle-o",
                Name = "List Messages",
                MenuOrder = 22,
                RequiredRole = (int)UserRoles.User,
                ParentEntityID = 21
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(listNotificationEntity);

            IOMenuEntity sendNotificationEntity = new IOMenuEntity()
            {
                ID = 23,
                Action = "pushNotificationSend",
                CssClass = "fa-circle-o",
                Name = "Send",
                MenuOrder = 23,
                RequiredRole = (int)UserRoles.User,
                ParentEntityID = 21
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(sendNotificationEntity);
        }

        private void GenerateResourceMenu(ModelBuilder modelBuilder)
        {
            IOMenuEntity resourceEntity = new IOMenuEntity()
            {
                ID = 24,
                Action = "actionResource",
                CssClass = "fa-address-book",
                Name = "Resources",
                MenuOrder = 24,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = null
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(resourceEntity);

            IOMenuEntity resourceListEntity = new IOMenuEntity()
            {
                ID = 25,
                Action = "resourcesList",
                CssClass = "fa-circle-o",
                Name = "Edit Resources",
                MenuOrder = 25,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = 24
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(resourceListEntity);

            IOMenuEntity resourceAddEntity = new IOMenuEntity()
            {
                ID = 26,
                Action = "resourceAdd",
                CssClass = "fa-circle-o",
                Name = "Add Resource",
                MenuOrder = 26,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = 24
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(resourceAddEntity);
        }
    
        private void GenerateImagesMenu(ModelBuilder modelBuilder)
        {
            IOMenuEntity imagesEntity = new IOMenuEntity()
            {
                ID = 27,
                Action = "actionImages",
                CssClass = "fa-image",
                Name = "Images",
                MenuOrder = 27,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = null
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(imagesEntity);

            IOMenuEntity imagesListEntity = new IOMenuEntity()
            {
                ID = 28,
                Action = "imagesEdit",
                CssClass = "fa-circle-o",
                Name = "Edit Images",
                MenuOrder = 28,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = 27
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(imagesListEntity);

            IOMenuEntity imageAddEntity = new IOMenuEntity()
            {
                ID = 29,
                Action = "imageAdd",
                CssClass = "fa-circle-o",
                Name = "Add Image",
                MenuOrder = 29,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = 27
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(imageAddEntity);
        }
    }
}
