﻿using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Findx.Redis.StackExchangeRedis
{
    internal static class StackExchangeRedisStreamManagerExtensions
    {
        public static async IAsyncEnumerable<StreamPosition> TryCreateConsumerGroup(this IDatabase database, StreamPosition[] positions, string consumerGroup, ILogger logger = null)
        {
            foreach (var position in positions)
            {
                var created = false;
                try
                {
                    var stream = position.Key;
                    var streamExist = await database.KeyTypeAsync(stream);
                    if (streamExist == RedisType.None)
                    {
                        if (await database.StreamCreateConsumerGroupAsync(stream, consumerGroup, StreamPosition.NewMessages))
                        {
                            logger.LogInformation($"Redis stream [{position.Key}] created with consumer group [{consumerGroup}]");
                            created = true;
                        }
                    }
                    else
                    {
                        var groupInfo = await database.StreamGroupInfoAsync(stream);

                        if (groupInfo.All(g => g.Name != consumerGroup))
                        {
                            if (await database.StreamCreateConsumerGroupAsync(stream, consumerGroup,
                                StreamPosition.NewMessages))
                            {
                                logger.LogInformation($"Redis stream [{position.Key}] created with consumer group [{consumerGroup}]");
                                created = true;
                            }
                        }
                        else
                        {
                            created = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, $"Redis error while creating consumer group [{consumerGroup}] of stream [{position.Key}]");
                }

                if (created)
                    yield return position;
            }
        }
    }
}
