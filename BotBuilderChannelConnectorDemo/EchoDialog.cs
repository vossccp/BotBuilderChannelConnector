using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

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

            if ("keyboard".Equals(message.Text))
            {
                var reply = context.MakeMessage();
                reply.AddKeyboardCard("Keyboard Card example", new[] { "Option 1", "Option 2" });
                await context.PostAsync(reply);
            }
            else if ("typing".Equals(message.Text))
            {
                var typing = context.MakeMessage();
                typing.Type = "typing";
                await context.PostAsync(typing);
            }
            else if ("hero".Equals(message.Text))
            {
                var reply = context.MakeMessage();

                var heroCard = new HeroCard
                {
                    Title = "Deco!",
                    Subtitle = "I am Brasilian",
                    Images = new List<CardImage>
                    {
                        new CardImage(url: "https://upload.wikimedia.org/wikipedia/commons/5/5c/Chelsea_Deco.jpg")
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction()
                        {
                            Value = "https://de.wikipedia.org/wiki/Deco",
                            Type = "openUrl",
                            Title = "Get me to Deco"
                        }
                    }
                };

                reply.Attachments = new List<Attachment>
                {
                    heroCard.ToAttachment(),
                    heroCard.ToAttachment()
                };

                await context.PostAsync(reply);
            }
            else if ("attachment".Equals(message.Text))
            {
                var reply = context.MakeMessage();
                reply.Attachments.Add(new Attachment
                {
                    ContentUrl = "http://aihelpwebsite.com/portals/0/Images/AIHelpWebsiteLogo_Large.png",
                    ContentType = "image/png",
                    Name = "AIHelpWebsiteLogo_Large.png"
                });
                await context.PostAsync(reply);
            }
            else if (message.Text == "reset")
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