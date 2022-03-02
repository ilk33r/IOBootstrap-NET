using System;
using IOBootstrap.NET.Common.Messages.Messages;
using IOBootstrap.NET.Common.Models.Messages;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.DataAccess.Entities;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public class IOBackOfficeMessagesViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        public IList<IOMessageModel> GetMessages()
        {
            DateTime currentDate = DateTime.Now;
            return DatabaseContext.Messages
                                        .Select(m => new IOMessageModel()
                                        {
                                            ID = m.ID,
                                            Message = m.Message,
                                            MessageCreateDate = m.MessageCreateDate,
                                            MessageStartDate = m.MessageStartDate,
                                            MessageEndDate = m.MessageEndDate
                                        })
                                        .Where(m => m.MessageStartDate < currentDate && m.MessageEndDate > currentDate)
                                        .OrderByDescending(m => m.MessageCreateDate)
                                        .ToList();
        }

        public IList<IOMessageModel> GetAllMessages()
        {
            return DatabaseContext.Messages
                                        .Select(m => new IOMessageModel()
                                        {
                                            ID = m.ID,
                                            Message = m.Message,
                                            MessageCreateDate = m.MessageCreateDate,
                                            MessageStartDate = m.MessageStartDate,
                                            MessageEndDate = m.MessageEndDate
                                        })
                                        .OrderByDescending(m => m.MessageCreateDate)
                                        .ToList();
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

            DatabaseContext.Add(messageEntity);
            DatabaseContext.SaveChanges();
        }

        public void DeleteMessage(int messageId)
        {
            IOBackOfficeMessageEntity messageEntity = DatabaseContext.Messages.Find(messageId);

            if (messageEntity != null) 
            {
                DatabaseContext.Remove(messageEntity);
                DatabaseContext.SaveChanges();
            }
        }

        public void UpdateMessage(IOMessageUpdateRequestModel request)
        {
            IOBackOfficeMessageEntity messageEntity = DatabaseContext.Messages.Find(request.MessageId);

            if (messageEntity != null)
            {
                messageEntity.Message = request.Message;
                messageEntity.MessageStartDate = request.MessageStartDate;
                messageEntity.MessageEndDate = request.MessageEndDate;

                DatabaseContext.Update(messageEntity);
                DatabaseContext.SaveChanges();
            }
        }
    }
}
