using System.Text.Json;
using System;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json.Nodes;

namespace BlazorPawAgentX.Commands
{
    public class AbstractCommand<C, R> where C : AbstractCommand<C, R>
    {
        public string? TypeName => typeof(C).FullName;
        
        public string Serialize()
        {
            var cmd = (C)this;
            return JsonSerializer.Serialize(cmd);
        }

        public async Task<R> InvokeCommand(HubConnection hubConnection, string connectionId)
        {
            var jsonRes = await hubConnection.InvokeAsync<string>("ExecuteCommand", Serialize(), connectionId);
            var type = typeof(R);
            var res = JsonSerializer.Deserialize(jsonRes, type);
            if (res == null) throw new Exception($"Command {typeof(C).Name} did not returned any value");
            return (R)res;
        }

        public async Task<string> ReturnResult(R result)
        {
            await Task.CompletedTask;
            return JsonSerializer.Serialize(result);
        }
    }
}
