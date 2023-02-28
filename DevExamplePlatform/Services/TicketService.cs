using MongoDB.Driver;
using System;
using System.Collections.Generic;
using DevExample.Private.Models;
using DevExample.Private.MongoDB;

namespace DevExample.Platform.Services
{
    public class TicketService
    {
        public static TicketService Instance { get; private set; }
        private readonly string Database = "DevExamplePlatform";
        private readonly string Collection = "Tickets";

        private IMongoDatabase TicketDatabase;
        private IMongoCollection<TicketModel> TicketCollection;

        public TicketService()
        {
            Instance = this;
            MDBClient = new MongoDBClient(ConfigManager.GetVariable("mongodb-connection-string"));
            TicketDatabase = MDBClient.GetDatabase(Database);
            TicketCollection = TicketDatabase.GetCollection<TicketModel>(Collection);
        }

        public MongoDBClient MDBClient;
        public List<TicketModel> GetAllTickets(string userNameIdentifier)
        {
            var isAdmin = AccountService.Instance.IsIronMan(userNameIdentifier);
            if (AccountService.Instance.AccountExists(userNameIdentifier) && isAdmin)
            {
                return TicketCollection.Find(_ => true).ToListAsync().Result;
            }
            return null;
        }

        public List<TicketModel> GetAllUserTickets(string userNameIdentifier)
        {
            if (AccountService.Instance.AccountExists(userNameIdentifier) || AccountService.Instance.IsIronMan(userNameIdentifier))
            {
                return TicketCollection.Find(x => x.userNameIdentifier == userNameIdentifier).ToListAsync().Result;
            }
            return null;
        }

        public TicketModel GetTicket(string userNameIdentifier, string ticketId)
        {
            try
            {
                bool isAdmin = AccountService.Instance.IsIronMan(userNameIdentifier);
                if (AccountService.Instance.AccountExists(userNameIdentifier))
                {
                    if (TicketExists(ticketId))
                    {
                        var ticket = TicketCollection.Find<TicketModel>(t => t._id == ticketId).First();

                        if (isAdmin)
                        {
                            ticket.ViewedBy = AccountService.Instance.GetAccount(userNameIdentifier).name;
                            UpdateTicket(ticket.userNameIdentifier, ticket);
                        }
                        if (ticket.userNameIdentifier == userNameIdentifier || isAdmin)
                        {
                            return ticket;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public bool AddTicket(string userNameIdentifier, string adminuserNameIdentifier, TicketModel ticket)
        {
            try
            {
                if (AccountService.Instance.AccountExists(userNameIdentifier) || AccountService.Instance.IsIronMan(userNameIdentifier))
                {
                    ticket.userNameIdentifier = userNameIdentifier;
                    if (ticket.userNameIdentifier == null || ticket.Priority == null || ticket.Subject == null)
                    {
                        return false;
                    }
                    if (AccountService.Instance.IsIronMan(adminuserNameIdentifier))
                    {
                        ticket.CreatedBy = AccountService.Instance.GetAccount(adminuserNameIdentifier).email;
                    }
                    ticket.Status = "Open";
                    ticket.LastOpened = "No Updates.";
                    ticket.Disabled = false;
                    ticket.CreatedBy = AccountService.Instance.GetAccount(userNameIdentifier).email;
                    if (ticket.Description != null || ticket.Status != null || ticket.Priority != null)
                    {
                        TicketCollection.InsertOne(ticket);
                        SendEmailNotification();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public bool UpdateTicket(string userNameIdentifier, TicketModel updatedTicket)
        {
            try
            {
                if (AccountService.Instance.AccountExists(userNameIdentifier) || AccountService.Instance.IsIronMan(userNameIdentifier))
                {
                    if (TicketExists(updatedTicket._id))
                    {
                        var oldTicket = TicketCollection.Find<TicketModel>(t => t._id == updatedTicket._id).First();
                        var newTicket = oldTicket;

                        var isAdmin = AccountService.Instance.IsIronMan(userNameIdentifier);
                        if (updatedTicket.Disabled == false || oldTicket.Disabled == false || isAdmin)
                        {
                            if (isAdmin || oldTicket.userNameIdentifier == userNameIdentifier)
                            {
                                if (updatedTicket.Status == "Disabled" || updatedTicket.Status == "Closed" || updatedTicket.Status == "Resolved")
                                {
                                    newTicket.Disabled = true;
                                }
                                else
                                {
                                    newTicket.Disabled = false;
                                }
                                if (updatedTicket.Subject != oldTicket.Subject)
                                {
                                    newTicket.Subject = updatedTicket.Subject;
                                }
                                if (updatedTicket.Description != oldTicket.Description)
                                {
                                    newTicket.Description = updatedTicket.Description;
                                }
                                if (updatedTicket.Status != oldTicket.Status || updatedTicket.Status != null)
                                {
                                    newTicket.Status = updatedTicket.Status;
                                }
                                if (updatedTicket.Priority != oldTicket.Priority || updatedTicket.Priority != null)
                                {
                                    newTicket.Priority = updatedTicket.Priority;
                                }
                                if (updatedTicket.Attachments.Count != newTicket.Attachments.Count)
                                {
                                    newTicket.Attachments = updatedTicket.Attachments;
                                }

                                newTicket.LastOpened = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                                var result = TicketCollection.ReplaceOne<TicketModel>(a => a._id == oldTicket._id, newTicket);
                                if (result.IsAcknowledged)
                                {
                                    SendEmailNotification();
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public bool DeleteTicket(string userNameIdentifier, string ticketId)
        {
            try
            {
                if (AccountService.Instance.AccountExists(userNameIdentifier) || AccountService.Instance.IsIronMan(userNameIdentifier))
                {
                    if (TicketExists(ticketId))
                    {
                        var ticket = TicketCollection.Find<TicketModel>(t => t._id == ticketId).First();
                        var isAdmin = AccountService.Instance.IsIronMan(userNameIdentifier);
                        if (isAdmin || ticket.userNameIdentifier == userNameIdentifier)
                        {
                            {
                                var filter = Builders<TicketModel>.Filter.Eq("_id", ticket._id);
                                var result = TicketCollection.DeleteOne(filter);
                                if (result.DeletedCount > 0)
                                {
                                    SendEmailNotification();
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public bool AddComment(string userNameIdentifier, string ticketId, CommentModel newComment)
        {
            try
            {
                if (AccountService.Instance.AccountExists(userNameIdentifier) || AccountService.Instance.IsIronMan(userNameIdentifier))
                {
                    if (TicketExists(ticketId))
                    {
                        var ticket = TicketCollection.Find<TicketModel>(t => t._id == ticketId).First();
                        var isAdmin = AccountService.Instance.IsIronMan(userNameIdentifier);
                        if (isAdmin || ticket.userNameIdentifier == userNameIdentifier)
                        {
                            if (ticket != null)
                            {
                                if (!ticket.Disabled)
                                {
                                    if (ticket.Comments == null)
                                    {
                                        ticket.Comments = new List<CommentModel>();
                                    }
                                    newComment.ReplierName = AccountService.Instance.GetAccount(userNameIdentifier).name;
                                    newComment.RepliedOn = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                                    newComment.CreatedOn = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                                    ticket.Comments.Add(newComment);
                                    ticket.LastOpened = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                                    var result = TicketCollection.ReplaceOne<TicketModel>(a => a._id == ticketId, ticket);
                                    if (result.IsAcknowledged)
                                    {
                                        SendEmailNotification();
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return false;
        }

        public bool DeleteComment(string userNameIdentifier, string ticketId, string commentId)
        {
            if (AccountService.Instance.AccountExists(userNameIdentifier) || AccountService.Instance.IsIronMan(userNameIdentifier))
            {
                if (TicketExists(ticketId))
                {
                    var isAdmin = AccountService.Instance.IsIronMan(userNameIdentifier);
                    var ticket = TicketCollection.Find<TicketModel>(t => t._id == ticketId).First();
                    if (ticket != null)
                    {
                        if (ticket.Disabled == false)
                        {
                            if (isAdmin || ticket.userNameIdentifier == userNameIdentifier)
                            {
                                foreach (var comment in ticket.Comments)
                                {
                                    if (comment._id == commentId)
                                    {
                                        comment.Message = "This message was deleted.";
                                        comment.LastUpdated = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                                        comment.CanEdit = false;
                                        ticket.LastOpened = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                                        var result = TicketCollection.ReplaceOne<TicketModel>(a => a._id == ticketId, ticket);
                                        if (result.IsAcknowledged)
                                        {
                                            SendEmailNotification();
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool UpdateComment(string userNameIdentifier, string ticketId, CommentModel editedComment)
        {
            if (AccountService.Instance.AccountExists(userNameIdentifier) || AccountService.Instance.IsIronMan(userNameIdentifier))
            {
                if (TicketExists(ticketId))
                {
                    var ticket = TicketCollection.Find<TicketModel>(t => t._id == ticketId).First();
                    var isAdmin = AccountService.Instance.IsIronMan(userNameIdentifier);
                    if (ticket != null)
                    {
                        if (isAdmin || userNameIdentifier == ticket.userNameIdentifier)
                        {
                            if (ticket._id == ticketId)
                            {
                                foreach (var comment in ticket.Comments)
                                {
                                    if (comment._id == editedComment._id)
                                    {
                                        comment.Message = editedComment.Message;
                                        comment.LastUpdated = DateTime.Now.ToString("MM/dd/yyyy HH:mm") + " (Edited)";
                                        comment.RepliedOn = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                                        ticket.LastOpened = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                                        var result = TicketCollection.ReplaceOne<TicketModel>(a => a._id == ticketId, ticket);
                                        if (result.IsAcknowledged)
                                        {
                                            SendEmailNotification();
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool AddSingleAttachment(string userNameIdentifier, string ticketId, FileModel attachment)
        {
            var ticket = TicketCollection.Find<TicketModel>(t => t._id == ticketId).First();
            var isAdmin = AccountService.Instance.IsIronMan(userNameIdentifier);
            bool result = false;
            if (ticket.Attachments != null)
            {
                if (attachment != null)
                {
                    var newComment = new CommentModel()
                    {
                        Message = "Attachment Added: " + attachment.Name,
                        CanEdit = false
                    };
                    AddComment(userNameIdentifier, ticket._id, newComment);

                    ticket = TicketCollection.Find<TicketModel>(t => t._id == ticketId).First();
                    ticket.Attachments.Add(attachment);
                    result = UpdateTicket(userNameIdentifier, ticket);
                }
            }
            return result;
        }

        public bool AddMultipleAttachments(string userNameIdentifier, string ticketId, List<FileModel> attachments)
        {

            bool result = false;
            var ticket = TicketCollection.Find<TicketModel>(t => t._id == ticketId).First();
            var isAdmin = AccountService.Instance.IsIronMan(userNameIdentifier);

            var ticketAttachments = new List<FileModel>();

            if (ticket != null)
            {
                if (ticket.Attachments == null)
                {
                    ticket.Attachments = new List<FileModel>();
                }
                ticketAttachments = ticket.Attachments;

                foreach (var file in attachments)
                {
                    ticketAttachments.Add(file);
                    var newComment = new CommentModel() 
                    {
                        Message = "Attachment Added: " + file.Name,
                        CanEdit = false
                    };
                    AddComment(userNameIdentifier, ticket._id, newComment);
                }
                ticket = TicketCollection.Find<TicketModel>(t => t._id == ticketId).First();
                ticket.Attachments = ticketAttachments;
                result = UpdateTicket(userNameIdentifier, ticket);

                return result;
            }
            return false;
        }

        public bool DeleteAttachment(string userNameIdentifier, string ticketId, FileModel file)
        {
            if (AccountService.Instance.AccountExists(userNameIdentifier) || AccountService.Instance.IsIronMan(userNameIdentifier))
            {
                if (TicketExists(ticketId))
                {
                    var ticket = TicketCollection.Find<TicketModel>(t => t._id == ticketId).First();
                    var isAdmin = AccountService.Instance.IsIronMan(userNameIdentifier);

                    if (ticket.Attachments != null)
                    {
                        foreach (var attachment in ticket.Attachments)
                        {
                            if (attachment.Id == file.Id)
                            {
                                ticket.Attachments.Remove(attachment);
                                var newComment = new CommentModel()
                                {
                                    Message = "Attachment Deleted: " + attachment.Name,
                                    CanEdit = false
                                };
                                ticket.Comments.Add(newComment);
                                AddComment(userNameIdentifier, ticket._id, newComment);

                                return UpdateTicket(userNameIdentifier, ticket);
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool TicketExists(string ticketId)
        {
            var ticket = TicketCollection.Find<TicketModel>(t => t._id == ticketId).First();
            if (ticket != null)
            {
                return true;
            }
            return false;
        }

        public void SendEmailNotification()
        {
            Console.WriteLine("Send Email Notification.");
        }
    }
}
