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
        public virtual DbSet<IOUserEntity> Users { get; set; }
        public virtual DbSet<PushNotificationEntity> PushNotifications { get; set; }
        public virtual DbSet<PushNotificationMessageEntity> PushNotificationMessageEntity { get; set; }
        public virtual DbSet<PushNotificationDeliveredMessagesEntity> PushNotificationDeliveredMessagesEntity { get; set; }

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

            this.GenerateClientMenu(modelBuilder);
            this.GenerateUserMenu(modelBuilder);
            this.GenerateNotificationMenu(modelBuilder);
            this.GenerateMenuEditorMenu(modelBuilder);
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

            IOMenuEntity clientUpdateEntity = new IOMenuEntity()
            {
                ID = 4,
                Action = "clientsUpdate",
                CssClass = "fa-circle-o",
                Name = "Update Client",
                MenuOrder = 4,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = 1
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(clientUpdateEntity);

            IOMenuEntity clientDeleteEntity = new IOMenuEntity()
            {
                ID = 5,
                Action = "clientsDelete",
                CssClass = "fa-circle-o",
                Name = "Delete Client",
                MenuOrder = 5,
                RequiredRole = (int)UserRoles.Admin,
                ParentEntityID = 1
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(clientDeleteEntity);
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

        private void GenerateNotificationMenu(ModelBuilder modelBuilder)
        {
            IOMenuEntity notificationEntity = new IOMenuEntity()
            {
                ID = 11,
                Action = "sendNotification",
                CssClass = "fa-send-o",
                Name = "Send Push Notification",
                MenuOrder = 11,
                RequiredRole = (int)UserRoles.User,
                ParentEntityID = null
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(notificationEntity);
        }

        private void GenerateMenuEditorMenu(ModelBuilder modelBuilder)
        {
            IOMenuEntity menuEditorEntity = new IOMenuEntity()
            {
                ID = 12,
                Action = "actionMenuEditor",
                CssClass = "fa-list",
                Name = "Menu Editor",
                MenuOrder = 12,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = null
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(menuEditorEntity);

            IOMenuEntity menuEditorListMenuEntity = new IOMenuEntity()
            {
                ID = 13,
                Action = "menuEditorList",
                CssClass = "fa-circle-o",
                Name = "List Menu Items",
                MenuOrder = 13,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = 12
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(menuEditorListMenuEntity);

            IOMenuEntity menuEditorAddMenuEntity = new IOMenuEntity()
            {
                ID = 14,
                Action = "menuEditorAdd",
                CssClass = "fa-circle-o",
                Name = "Add Menu Item",
                MenuOrder = 14,
                RequiredRole = (int)UserRoles.SuperAdmin,
                ParentEntityID = 12
            };
            modelBuilder.Entity<IOMenuEntity>().HasData(menuEditorAddMenuEntity);
        }
    }
}
