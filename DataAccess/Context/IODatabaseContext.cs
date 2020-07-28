using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.DataAccess.Context
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
        public virtual DbSet<IOResourceEntity> Resources { get; set; }

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

            modelBuilder.Entity<IOResourceEntity>().HasIndex(
                resourceEntity => new { resourceEntity.ResourceKey }).IsUnique(true);

            AddResources(modelBuilder);
            GenerateClientMenu(modelBuilder);
            GenerateUserMenu(modelBuilder);
            GenerateConfigurationMenu(modelBuilder);
            GenerateMenuEditorMenu(modelBuilder);
            GenerateMessagesMenu(modelBuilder);
            GenerateNotificationMenu(modelBuilder);
            GenerateResourceMenu(modelBuilder);
            GenerateImagesMenu(modelBuilder);
        }

        private void AddResources(ModelBuilder modelBuilder)
        {
            IOResourceEntity editEntity = new IOResourceEntity()
            {
                ID = 1,
                ResourceKey = "BackOffice.Edit",
                ResourceValue = "Edit",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(editEntity);

            IOResourceEntity deleteEntity = new IOResourceEntity()
            {
                ID = 2,
                ResourceKey = "BackOffice.Delete",
                ResourceValue = "Delete",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(deleteEntity);

            IOResourceEntity optionsEntity = new IOResourceEntity()
            {
                ID = 3,
                ResourceKey = "BackOffice.Options",
                ResourceValue = "Options",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(optionsEntity);

            IOResourceEntity selectEntity = new IOResourceEntity()
            {
                ID = 4,
                ResourceKey = "BackOffice.Select",
                ResourceValue = "Select",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(selectEntity);

            IOResourceEntity homeEntity = new IOResourceEntity()
            {
                ID = 5,
                ResourceKey = "BackOffice.Home",
                ResourceValue = "Home",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(homeEntity);

            IOResourceEntity usersEntity = new IOResourceEntity()
            {
                ID = 6,
                ResourceKey = "BackOffice.Users",
                ResourceValue = "Users",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(usersEntity);

            IOResourceEntity changePasswordEntity = new IOResourceEntity()
            {
                ID = 7,
                ResourceKey = "BackOffice.ChangePassword",
                ResourceValue = "Change Password",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(changePasswordEntity);

            IOResourceEntity idEntity = new IOResourceEntity()
            {
                ID = 8,
                ResourceKey = "BackOffice.ID",
                ResourceValue = "ID",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(idEntity);

            IOResourceEntity nameEntity = new IOResourceEntity()
            {
                ID = 9,
                ResourceKey = "BackOffice.Name",
                ResourceValue = "Name",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(nameEntity);

            IOResourceEntity roleEntity = new IOResourceEntity()
            {
                ID = 10,
                ResourceKey = "BackOffice.Role",
                ResourceValue = "Role",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(roleEntity);

            IOResourceEntity lastLoginDateEntity = new IOResourceEntity()
            {
                ID = 11,
                ResourceKey = "BackOffice.LastLoginDate",
                ResourceValue = "Last Login Date",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(lastLoginDateEntity);

            IOResourceEntity errorEntity = new IOResourceEntity()
            {
                ID = 12,
                ResourceKey = "BackOffice.Error",
                ResourceValue = "An error occured.",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(errorEntity);

            IOResourceEntity pushNotificationMessagesEntity = new IOResourceEntity()
            {
                ID = 13,
                ResourceKey = "BackOffice.PushNotificationMessages",
                ResourceValue = "Push Notification Messages",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(pushNotificationMessagesEntity);

            IOResourceEntity sendingEntity = new IOResourceEntity()
            {
                ID = 14,
                ResourceKey = "BackOffice.Sending",
                ResourceValue = "Sending",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(sendingEntity);

            IOResourceEntity completedEntity = new IOResourceEntity()
            {
                ID = 15,
                ResourceKey = "BackOffice.Completed",
                ResourceValue = "Completed",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(completedEntity);

            IOResourceEntity clientEntity = new IOResourceEntity()
            {
                ID = 16,
                ResourceKey = "BackOffice.Client",
                ResourceValue = "Client",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(clientEntity);

            IOResourceEntity dateEntity = new IOResourceEntity()
            {
                ID = 17,
                ResourceKey = "BackOffice.Date",
                ResourceValue = "Date",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(dateEntity);

            IOResourceEntity categoryEntity = new IOResourceEntity()
            {
                ID = 18,
                ResourceKey = "BackOffice.Category",
                ResourceValue = "Category",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(categoryEntity);

            IOResourceEntity dataEntity = new IOResourceEntity()
            {
                ID = 19,
                ResourceKey = "BackOffice.Data",
                ResourceValue = "Data",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(dataEntity);

            IOResourceEntity messageEntity = new IOResourceEntity()
            {
                ID = 20,
                ResourceKey = "BackOffice.Message",
                ResourceValue = "Message",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(messageEntity);

            IOResourceEntity titleEntity = new IOResourceEntity()
            {
                ID = 21,
                ResourceKey = "BackOffice.Title",
                ResourceValue = "Title",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(titleEntity);

            IOResourceEntity statusEntity = new IOResourceEntity()
            {
                ID = 22,
                ResourceKey = "BackOffice.Status",
                ResourceValue = "Status",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(statusEntity);

            IOResourceEntity sendedDevicesEntity = new IOResourceEntity()
            {
                ID = 23,
                ResourceKey = "BackOffice.SendedDevices",
                ResourceValue = "Sended Devices",
            };
            modelBuilder.Entity<IOResourceEntity>().HasData(sendedDevicesEntity);
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
                Action = "restartApp",
                CssClass = "fa-circle-o",
                Name = "Restart App",
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
