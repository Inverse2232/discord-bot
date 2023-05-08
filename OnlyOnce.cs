using Discord;
using Discord.WebSocket;

namespace DiscordBotLinuxTest
{
    internal class OnlyOnce
    {
        public async Task CreateSlashCommands(DiscordSocketClient _client, SocketGuild _main)
        {
            var commandMessage = new SlashCommandBuilder()
    .WithName("message")
    .WithDescription("Send a message from within the bot")
    .WithDefaultMemberPermissions(GuildPermission.ManageChannels);
            await _client.Rest.CreateGuildCommand(commandMessage.Build(), _main.Id);

            var commandModify = new SlashCommandBuilder()
            .WithName("modify")
            .WithDescription("Edit a message sent by the bot")
            .AddOption("string", ApplicationCommandOptionType.String, "Enter message link", true)
            .WithDefaultMemberPermissions(GuildPermission.ManageChannels);
            await _client.Rest.CreateGuildCommand(commandModify.Build(), _main.Id);

            var commandGiveaway = new SlashCommandBuilder()
            .WithName("giveaway")
            .WithDescription("Start a giveaway")
            .WithDefaultMemberPermissions(GuildPermission.ManageChannels);
            await _client.Rest.CreateGuildCommand(commandGiveaway.Build(), _main.Id);

            var commandAvailability = new SlashCommandBuilder()
            .WithName("availability")
            .WithDescription("Start an availability check")
            .WithDefaultMemberPermissions(GuildPermission.ManageChannels);
            await _client.Rest.CreateGuildCommand(commandAvailability.Build(), _main.Id);
        }
        public async Task CreateVerifyMessage(SocketTextChannel channelRules, SocketTextChannel channelVerify)
        {
            var embed = new EmbedBuilder { };
            embed.WithColor(Discord.Color.Blue)
            .WithTitle("Welcome to the Wild Dreams Host Club")
            .WithDescription("In order to gain full access to the server you need to agree to our Discord rules \n" + channelRules.Mention + "\n\r" +
            "Visit our website [here](https://wilddreamshostclub.carrd.co/)!")
            .WithFooter(footer => footer.Text = "See you soon");
            await channelVerify.SendMessageAsync("", false, embed.Build());

            var buttonAccept = new ButtonBuilder()
            {
                Label = "Accept Rules",
                CustomId = "buttonRules",
                Style = ButtonStyle.Success
            };
            var component1 = new ComponentBuilder();
            component1.WithButton(buttonAccept);
            await channelVerify.SendMessageAsync(" ", false, components: component1.Build());
        }

        /*
            public async Task CreateRolesAssignment(SocketTextChannel channelVerify)
            {
                var embedRoles = new EmbedBuilder { };
                embedRoles.WithColor(Discord.Color.Blue)
                .WithTitle("Roles Assignment")
                .WithDescription("React below to assign chosen roles"
                + "\n 🗨 - " + roleRP.Mention
                + "\n 💗 - " + roleERP.Mention
                + "\n 🔞 - " + roleNSFW.Mention
                + "\n ♂️ - " + roleHe.Mention
                + "\n ♀️ - " + roleShe.Mention
                + "\n ⚧ - " + roleThey.Mention
                + "\n 💬 - " + roleAsk.Mention
                );
                await channelVerify.SendMessageAsync("", false, embedRoles.Build());

                var messagesd = await channelVerify.GetMessagesAsync(1).FlattenAsync();
                rolesMessage = messagesd.First().Id;

                await messages.First().AddReactionAsync(new Emoji("🗨"));
                await messages.First().AddReactionAsync(new Emoji("💗"));
                await messages.First().AddReactionAsync(new Emoji("🔞"));
                await messages.First().AddReactionAsync(new Emoji("♂️"));
                await messages.First().AddReactionAsync(new Emoji("♀️"));
                await messages.First().AddReactionAsync(new Emoji("⚧"));
                await messages.First().AddReactionAsync(new Emoji("💬"));

            }
            /* public async Task EditRolesAssignment(SocketTextChannel channelVerify)
             {
                 var msg = await channelVerify.GetMessageAsync(1020449323772809277);
                 if (msg is IUserMessage message)
                 {
                     await message.ModifyAsync(x =>
                     {
                         EmbedBuilder embed = new EmbedBuilder();
                         embed.WithColor(Discord.Color.Blue)
                         .WithTitle("Roles Assignment")
                         .WithDescription("React below to assign chosen roles"
                         + "\n 🗨 - " + roleRP.Mention
                         + "\n 💗 - " + roleERP.Mention
                         + "\n 🔞 - " + roleNSFW.Mention
                         + "\n ♂️ - " + roleHe.Mention
                         + "\n ♀️ - " + roleShe.Mention
                         + "\n ⚧ - " + roleThey.Mention
                         + "\n 💬 - " + roleAsk.Mention);
                         x.Embed = embed.Build();
                     });
                 }
                 await messages.First().AddReactionAsync(new Emoji("🗨"));
                 await messages.First().AddReactionAsync(new Emoji("💗"));
                 await messages.First().AddReactionAsync(new Emoji("🔞"));
                 await messages.First().AddReactionAsync(new Emoji("♂️"));
                 await messages.First().AddReactionAsync(new Emoji("♀️"));
                 await messages.First().AddReactionAsync(new Emoji("⚧"));
                 await messages.First().AddReactionAsync(new Emoji("💬"));
        */
        public async Task CreateTicketMessage(SocketTextChannel channelTickets)
        {
            var component = new ComponentBuilder()
                .WithButton(new ButtonBuilder { Label = "VIP Tickets", CustomId = "buttonVIP", Style = ButtonStyle.Primary })
                .WithButton(new ButtonBuilder { Label = "Room Booking", CustomId = "buttonRoom", Style = ButtonStyle.Primary })
                .WithButton(new ButtonBuilder { Label = "Host Booking", CustomId = "buttonHost", Style = ButtonStyle.Primary })
                .WithButton(new ButtonBuilder { Label = "Other Inquiries", CustomId = "buttonOther", Style = ButtonStyle.Primary });

            var embed = new EmbedBuilder()
                .WithTitle("Booking and general inquiries")
                .WithDescription("\r\nLet us know if you need help or want to book one of our services\nPlease select a category according to your inquiry")
                .WithFooter("We'll get back to you as soon as possible.")
                .WithColor(Discord.Color.Blue);

            await channelTickets.SendMessageAsync("", false, embed: embed.Build(), components: component.Build());
        }
        public async Task CreateTicketStaffMessage(SocketTextChannel channelFeedback)
        {
            var component = new ComponentBuilder()
                .WithButton(new ButtonBuilder { Label = "Feedback", CustomId = "buttonFeedback", Style = ButtonStyle.Danger });

            var embed = new EmbedBuilder()
                .WithTitle("Staff Feedback")
                .WithDescription("\r\nPlease let us know if you have any feedback for the Head Staff\nWe would be really happy to talk about it with you\nThanks 💗")
                .WithFooter("We'll get back to you as soon as possible.")
                .WithColor(Discord.Color.Red);

            await channelFeedback.SendMessageAsync("", false, embed: embed.Build(), components: component.Build());
        }
    }
}
