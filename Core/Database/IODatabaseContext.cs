﻿using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Entities.Configuration;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.WebApi.BackOffice.Entities;
using IOBootstrap.NET.WebApi.PushNotification.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.Core.Database
{
    public abstract class IODatabaseContext<TContext> : DbContext where TContext : DbContext
    {

        public virtual DbSet<IOConfigurationEntity> Configurations { get; set; }
        public virtual DbSet<IOClientsEntity> Clients { get; set; }
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

            this.GenerateClientMenu(modelBuilder);
            this.GenerateUserMenu(modelBuilder);
            this.GenerateConfigurationMenu(modelBuilder);
            this.GenerateMenuEditorMenu(modelBuilder);
            this.GenerateMessagesMenu(modelBuilder);
            this.GenerateNotificationMenu(modelBuilder);
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

            IOMenuEntity userUpdateEntity = new IOMenuEntity()
            {
                ID = 9,
                Action = "usersUpdate",
                CssClass = "fa-circle-o",
                Name = "Update User",
                MenuOrder = 9,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = 6
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(userUpdateEntity);

            IOMenuEntity userDeleteEntity = new IOMenuEntity()
            {
                ID = 10,
                Action = "usersDelete",
                CssClass = "fa-circle-o",
                Name = "Delete User",
                MenuOrder = 10,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = 6
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(userDeleteEntity);
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
    }
}
