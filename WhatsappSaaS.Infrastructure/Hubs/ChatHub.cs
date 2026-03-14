using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WhatsappSaaS.Infrastructure.Hubs;

public class ChatHub : Hub
{
    public async Task JoinCompanyGroup(string companyId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, companyId);
    }

    public async Task SendMessageToCompany(string companyId, string message)
    {
        await Clients.Group(companyId).SendAsync("ReceiveMessage", message);
    }
}
