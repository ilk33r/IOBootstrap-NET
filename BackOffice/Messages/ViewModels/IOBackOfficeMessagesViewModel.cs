using System;
using IOBootstrap.NET.Core.ViewModels;

namespace IOBootstrap.NET.BackOffice.Messages.ViewModels
{
    public class IOBackOfficeMessagesViewModel : IOBackOfficeViewModel
    {

        #region Initialization Methods

        public IOBackOfficeMessagesViewModel() : base()
        {
        }

        #endregion

        //TODO: Migrate with MW.
        /*
        public IList<IOMessageModel> GetMessages()
        {
            DateTime currentDate = DateTime.Now;
            var messages = DatabaseContext.Messages.Where((arg) => arg.MessageStartDate < currentDate && arg.MessageEndDate > currentDate)
                                          .OrderByDescending((arg) => arg.MessageCreateDate);

            return messages.ToList().ConvertAll<IOMessageModel>(input =>
            {
                IOMessageModel messageModel = new IOMessageModel() 
                {
                    ID = input.ID,
                    Message = input.Message,
                    MessageCreateDate = input.MessageCreateDate,
                    MessageStartDate = input.MessageStartDate,
                    MessageEndDate = input.MessageEndDate
                };

                return messageModel;
            });
        }

        public IList<IOMessageModel> GetAllMessages()
        {
            var messages = DatabaseContext.Messages.OrderByDescending((arg) => arg.MessageCreateDate);

            return messages.ToList().ConvertAll<IOMessageModel>(input =>
            {
                IOMessageModel messageModel = new IOMessageModel() 
                {
                    ID = input.ID,
                    Message = input.Message,
                    MessageCreateDate = input.MessageCreateDate,
                    MessageStartDate = input.MessageStartDate,
                    MessageEndDate = input.MessageEndDate
                };

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
        */
    }
}
