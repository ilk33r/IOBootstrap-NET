using System;
using System.Collections.Generic;
using System.Linq;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.WebApi.BackOffice.Entities;
using IOBootstrap.NET.WebApi.BackOffice.Models;

namespace IOBootstrap.NET.WebApi.BackOffice.ViewModels
{
    public class IOBackOfficeMessagesViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Initialization Methods

        public IOBackOfficeMessagesViewModel() : base()
        {
        }

        #endregion

        public IList<IOMessageModel> GetMessages()
        {
            DateTime currentDate = DateTime.Now;
            var messages = _databaseContext.Messages.Where((arg) => arg.MessageStartDate < currentDate && arg.MessageEndDate > currentDate)
                                           .OrderByDescending((arg) => arg.MessageCreateDate);

            return messages.ToList().ConvertAll<IOMessageModel>((input) =>
            {
                IOMessageModel messageModel = new IOMessageModel();
                messageModel.ID = input.ID;
                messageModel.Message = input.Message;
                messageModel.MessageCreateDate = input.MessageCreateDate;
                return messageModel;
            });
        }

        public void AddMessage(IOMessageAddRequestModel request) 
        {
            IOBackOfficeMessageEntity messageEntity = new IOBackOfficeMessageEntity()
            {
                Message = request.Message,
                MessageCreateDate = DateTimeOffset.Now,
                MessageStartDate = request.MessageStartDate,
                MessageEndDate = request.MessageEndDate
            };

            _databaseContext.Add(messageEntity);
            _databaseContext.SaveChanges();
        }
    }
}
