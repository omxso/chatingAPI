using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessagesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }

        // [HttpPost]
        // public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        // {
        //     var username = User.GetUsername();
        //     if (username == createMessageDto.RecipientUsername.ToLower())
        //         return BadRequest("You cannot send messages to yourself");

        //     var sender = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);
        //     var recipient = await _unitOfWork.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);

        //     if (recipient == null) return NotFound();

        //     var message = new Message
        //     {
        //         Sender = sender,
        //         Recipient = recipient,
        //         SenderUsername = sender.UserName,
        //         RecipientUsername = recipient.UserName,
        //         Contant = createMessageDto.Content
        //     };

        //     _unitOfWork.MessageRepository.AddMessage(message);

        //     if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));

        //     return BadRequest("Faild to send message");
        // }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesFroUser([FromQuery]
         MessagesParams messagesParams)
        {
            messagesParams.Username = User.GetUsername();

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messagesParams);

            Response.AddpaginationHeader(messages.CurrentPage, messages.PageSize,
            messages.ToltalCount, messages.TotalPages);

            return messages;
        }

        //  [HttpGet("thread/{username}")] // spicify the name of the other user inthis requset
        //  public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        //  {
        //      var currentUsername = User.GetUsername();

        //      return Ok(await _unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
        //  } this inside SignalR Hub

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized();

            if (message.Sender.UserName == username) message.SenderDeleated = true;

            if (message.Sender.UserName == username) message.RecipientDeleted = true;

            if (message.SenderDeleated && message.RecipientDeleted)
                _unitOfWork.MessageRepository.DeleteMessage(message);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting the messge");
        }
    }
}