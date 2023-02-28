using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevExample.Platform.Services;

namespace DevExample.Platform.Controllers
{
    [Route("api/support/ticket")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class TicketController : Controller
    {
        private TicketService TicketService = new TicketService();
        public TicketController()
        {

        }

        //[HttpGet("list")]
        //public List<TicketModel> List(string userNameIdentifier)
        //{
        //    var results = TicketService.GetAllTickets(userNameIdentifier);
        //    return results;
        //}

        //[HttpGet("user/list")]
        //public List<TicketModel> ListUserTickets(string userNameIdentifier)
        //{
        //    var results = TicketService.GetAllUserTickets(userNameIdentifier);
        //    return results;
        //}

        //[HttpGet("get/{ticketId}")]
        //public TicketModel GetTicket(string userNameIdentifier, string ticketId)
        //{
        //    var results = TicketService.GetTicket(userNameIdentifier, ticketId);
        //    return results;
        //}

        //[HttpPost("add")]
        //public DefaultResponse AddTicket(string userNameIdentifier, CreateTicketRequest request)
        //{
        //    var response = new DefaultResponse();
        //    var newTicket = new TicketModel()
        //    {
        //        subject = request.subject,
        //        description = request.message,
        //        priority = request.priority,
        //        status = request.status,
        //    };

        //    if (request.ticket_attachments.Count >= 1)
        //    {
        //        newTicket.ticket_attachments = new List<FileModel>();
        //        foreach (var attachment in request.ticket_attachments)
        //        {
        //            var newFile = new FileModel()
        //            {
        //                name = attachment.name,
        //                data = attachment.data
        //            };
        //            newTicket.ticket_attachments.Add(newFile);
        //        }
        //    }

        //    if (TicketService.AddTicket(userNameIdentifier, newTicket))
        //    {
        //        response.result = true;
        //    }
        //    else
        //    {
        //        response.result = false;
        //    }
        //    return response;
        //}

        //[HttpPost("file/add")]
        //public async Task<DefaultResponse> AttachFileAsync(IFormFile file, string userNameIdentifier, string ticketId)
        //{
        //    var response = new DefaultResponse() { result = false };
        //    response.result = await TicketService.AttachFile(userNameIdentifier, ticketId, file);

        //    return response;
        //}

        //[HttpPost("update")]
        //public DefaultResponse Update(string userNameIdentifier, UpdateTicketRequest request)
        //{
        //    var response = new DefaultResponse();
        //    var updatedTicket = new TicketModel()
        //    {
        //        subject = request.subject,
        //        priority = request.priority,
        //        status = request.status,
        //        description = request.description
        //    };

        //    if (TicketService.UpdateTicket(userNameIdentifier, updatedTicket))
        //    {
        //        response.result = true;
        //    }
        //    else
        //    {
        //        response.result = false;
        //    }
        //    return response;
        //}

        //[HttpDelete("delete/{ticketId}")]
        //public DefaultResponse Delete(string userNameIdentifier, string ticketId)
        //{
        //    var response = new DefaultResponse();
        //    if (TicketService.DeleteTicket(userNameIdentifier, ticketId))
        //    {
        //        response.result = true;
        //    }
        //    else
        //    {
        //        response.result = false;
        //    }
        //    return response;
        //}

        //[HttpPost("comment")]
        //public DefaultResponse AddComment(string userNameIdentifier, string ticketId, CreateCommentRequest request)
        //{
        //    var response = new DefaultResponse();

        //    var newComment = new CommentModel()
        //    {
        //        message = request.message,
        //        _id = request.ticketId
        //    };
        //    if (TicketService.AddComment(userNameIdentifier, request.ticketId, newComment))
        //    {
        //        response.result = true;
        //    }
        //    else
        //    {
        //        response.result = false;
        //    }
        //    return response;
        //}

        //[HttpPut("comment/delete")]
        //public DefaultResponse DeleteComment(string userNameIdentifier, string ticketId, string commentId)
        //{
        //    var response = new DefaultResponse();
        //    if (TicketService.DeleteComment(userNameIdentifier, ticketId, commentId))
        //    {
        //        response.result = true;
        //    }
        //    else
        //    {
        //        response.result = false;
        //    }
        //    return response;
        //}
    }
}
