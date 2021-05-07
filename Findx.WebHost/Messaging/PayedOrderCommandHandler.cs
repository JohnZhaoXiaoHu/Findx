﻿using Findx.DependencyInjection;
using Findx.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace Findx.WebHost.Messaging
{
    public class PayedOrderCommandHandler : IMessageNotifyHandler<PayedOrderCommand>, ITransientDependency
    {
        public async Task Handle(PayedOrderCommand message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
