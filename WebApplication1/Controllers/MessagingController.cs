﻿using Findx.Messaging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApplication1.Messaging;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        private readonly IApplicationEventPublisher _applicationEventPublisher;

        public MessagingController(IApplicationEventPublisher applicationEventPublisher)
        {
            _applicationEventPublisher = applicationEventPublisher;
        }

        [HttpGet]
        public async Task<IActionResult> Send()
        {
            await _applicationEventPublisher.PublishAsync(new PayedOrderCommand(new Random().Next(-999, 999)));
            return Ok("111" + await _applicationEventPublisher.SendAsync(new CancelOrderCommand(new Random().Next(-99, 99))));
        }
    }
}
