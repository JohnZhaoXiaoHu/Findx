﻿using System.Threading;
using System.Threading.Tasks;
using Findx.Scheduling;
namespace Findx.WebHost.Scheduling
{
    [Scheduled(Cron = "* * * * * ?", Name = "测试任务4")]
    public class TestScheduledTask10 : IScheduledTask
    {
        public async Task ExecuteAsync(ITaskContext context, CancellationToken cancellationToken = default)
        {
            await Task.Delay(5);
            //Console.WriteLine($"我是{context.TaskId}条测试任务========" + DateTime.Now + "==========" + Thread.CurrentThread.ManagedThreadId);
        }
    }
}
