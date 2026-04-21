using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.NEG.Services
{
    /// <summary>
    /// Singleton registry for managing active hub connections in a thread-safe way.
    /// Maps connection IDs to user/hook identifiers.
    /// </summary>
    public sealed class HubCommunicationService
    {
        private readonly ILogger<HubCommunicationService> _logger;
        private readonly ConcurrentDictionary<string, string> _hookConnections = new ConcurrentDictionary<string, string>();

        public HubCommunicationService(ILogger<HubCommunicationService> logger)
        {
            _logger = logger;
        }

        private bool TryAddConnection(string userId, string userName)
        {
            return _hookConnections.TryAdd(userId, userName);
        }

        private bool TryRemoveConnection(string userId, out string userName)
        {
            return _hookConnections.TryRemove(userId, out userName);
        }

        private IEnumerable<string> TryGetConnectionsForUser(string userName)
        {
            return _hookConnections.Where(kvp => kvp.Value == userName).Select(kvp => kvp.Key);
        }

        public int Count
        {
            get { return _hookConnections.Count; }
        }

        public IEnumerable<string> GetHookConnectionsForUser(string userName)
        {
            try
            {
                return TryGetConnectionsForUser(userName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving connections for user {UserName}", userName);
                return Enumerable.Empty<string>();
            }
        }

        public async Task AddToGroupAndRegisterAsync(string userId, string userName)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("AddToGroupAndRegisterAsync called with empty userId. UserName: {UserName}", userName);
                return;
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = "UnknownUser";
                _logger.LogWarning("AddToGroupAndRegisterAsync called with empty userName. UserId: {UserId}", userId);                
            }

            try
            {
                var wasAdded = TryAddConnection(userId, userName);
                if (!wasAdded)
                {
                    _logger.LogDebug(
                        "User already connected. UserId: {UserId}, UserName: {UserName}",
                        userId, userName);
                    return;
                }

                _logger.LogInformation(
                    "{UtcNow:o} | [{UserId}] -> [HubHookNet] | [AddToGroupAndRegisterAsync]: Connected (UserName: {UserName})",
                    DateTime.UtcNow, userId, userName);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{UtcNow:o} | [{UserId}] -> [HubHookNet] | [AddToGroupAndRegisterAsync]: Error while checking/adding connection. UserName: {UserName}",
                    DateTime.UtcNow, userId, userName);
            }
        }

        public void RemoveConnection(string userId)
        {
            if (TryRemoveConnection(userId, out var userName) && !string.IsNullOrWhiteSpace(userName))
            {
                _logger.LogInformation(
                    "{UtcNow:o} | [{UserId}] -> [HubHookNet] | [RemoveConnection]: Disconnected (UserName: {UserName})",
                    DateTime.UtcNow, userId, userName);
            }
        }
    }
}
