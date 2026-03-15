using System;

namespace WhatsappSaaS.Application.DTOs;

// Twilio-style webhook payload
public class WhatsAppWebhookPayload
{
    public string MessageSid { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string AccountSid { get; set; } = string.Empty;
    public string NumMedia { get; set; } = "0";
}

// 360dialog style
public class WhatsAppCloudMessage
{
    public string From { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public TextMessage? Text { get; set; }

    public class TextMessage
    {
        public string Body { get; set; } = string.Empty;
    }
}
