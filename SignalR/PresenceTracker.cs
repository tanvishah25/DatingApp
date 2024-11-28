using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DatingApp.SignalR
{
    // This class is used to track list of users who are connected to application
    public class PresenceTracker
    {
        // Key will be username and value will be represent connnection Id
        //User can connnect to application trough serval devices ie. from mobile,laptop
        //For each device it will generate different connection ID
        private static readonly Dictionary<string, List<string>> OnlineUsers = [];

        public Task UserConnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    // alternative way to write  new List<string> { connectionId} this to [connectionId]
                    OnlineUsers.Add(username, new List<string> { connectionId });
                }
            }
            return Task.CompletedTask;
        }

        public Task UserDisConnected(string username, string connectionId)
        {

            lock (OnlineUsers)
            {
                // Chcek if that username is there in dictionary if not then retun complete task
                if (!OnlineUsers.ContainsKey(username))
                {
                    return Task.CompletedTask;
                }
                // We are not removing entire username becuase there might be user connected with application with different connection Id
                OnlineUsers[username].Remove(connectionId);

                //if for that particular username we don't have any connection ID so remove that username
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }
            }
            return Task.CompletedTask;
        }
        //ToList() is a synchronous operation: The ToList() method is a synchronous operation, meaning it does not require an await.Therefore, using await in front of it would result in a compiler warning or error because it's not an asynchronous operation.
        public Task<List<string>> GetOnlineUsers()
        {
            //OnlineUsers.Keys.ToList(): This converts the keys of the OnlineUsers dictionary
            //(or whatever collection OnlineUsers is) into a List<string>.
            //This is a synchronous operation.

            //Task.FromResult(): Since your method signature returns a Task<List<string>>, you need to wrap the result in a Task.
            //Task.FromResult() is a simple way to create a Task from a result when you don't actually need to perform an asynchronous operation.
            List<string> onlineUsers;
            lock (OnlineUsers) {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k=>k.Key).ToList();
            }   
            return Task.FromResult(onlineUsers);
        }
    }
}

