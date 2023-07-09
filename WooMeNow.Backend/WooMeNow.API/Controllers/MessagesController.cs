using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WooMeNow.API.Data.Repository;
using WooMeNow.API.Data.Repository.IRepository;
using WooMeNow.API.Data.UnitOfWork;
using WooMeNow.API.Extensions;
using WooMeNow.API.Helpers;
using WooMeNow.API.Models;
using WooMeNow.API.Models.DTOs;

namespace WooMeNow.API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MessagesController(
            IUnitOfWork uow,
            IMapper mapper) 
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername) 
                return BadRequest("You cannot sent message to yourserf");

            var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _uow.MessageRepository.AddMessage(message);

            if (await _uow.CompleteAsync()) 
                return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser(
            [FromQuery] MessageParams messageParams)
        {
            var username = User.GetUsername();
            messageParams.Username = username;

            var messages = await _uow.MessageRepository.GetMessagesForUserAsync(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(
                messages.CurrentPage, 
                messages.PageSize, 
                messages.TotalCount, 
                messages.TotalPages));

            return messages;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _uow.MessageRepository.GetMessageAsync(id);

            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Unauthorized();

            if (message.SenderUsername == username) message.SenderDeleted = true;

            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted)
            {
                _uow.MessageRepository.DeleteMessage(message);
            }

            if (await _uow.CompleteAsync()) return Ok();

            return BadRequest("Problem deleting the message");

        }
    }
}
