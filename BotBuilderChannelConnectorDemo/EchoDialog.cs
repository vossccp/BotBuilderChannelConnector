﻿using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Bot.Builder.ChannelConnector.Demo
{   
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        public int Count { get; set; } = 1;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message.Text == "reset")
            {
                Count = 1;
                await context.PostAsync("Count has been reset");
            }
            else
            {
                await context.PostAsync($"{Count++}: You said {message.Text}");                 
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}