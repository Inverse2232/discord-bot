using Discord;
using Discord.WebSocket;
using System.Threading.Channels;

public class DiscordBot
{
    private DiscordSocketClient _client;

    bool isTestVersion = true;

    ulong rolesMessage;
    ulong categoryTickets;
    SocketRole roleRP;
    SocketRole roleERP;
    SocketRole roleNSFW;
    SocketRole roleGuest;
    SocketRole roleHe;
    SocketRole roleShe;
    SocketRole roleThey;
    SocketRole roleAsk;
    SocketGuild _main;
    SocketTextChannel channelLogs;
    SocketTextChannel channelTickets;
    SocketTextChannel channelVerify;
    SocketTextChannel channelWelcome;
    SocketTextChannel channelRules;
    SocketTextChannel channelGlobal;
    SocketTextChannel channelFeedback;
    SocketTextChannel channelAvailability;
    SocketRole roleSecurity;
    SocketRole roleDealer;
    SocketRole roleBarkeeper;
    SocketRole roleHost;
    SocketRole rolePhotographer;

    private Dictionary<ulong, List<IMessage>> _messageCache = new Dictionary<ulong, List<IMessage>>();

    public static void Main(string[] args) => new DiscordBot().MainAsync().GetAwaiter().GetResult();

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();
        var _config = new DiscordSocketConfig
        {
            MessageCacheSize = 10000000,
            GatewayIntents = GatewayIntents.All,
            UseInteractionSnowflakeDate = false // no need. Added because had wrong time set on windows :clown:
        };

        _client = new DiscordSocketClient(_config);
        _client.Log += Log;
        _client.MessageReceived += OnMessageReceived;
        _client.ReactionAdded += OnReactionAdded;
        _client.ReactionRemoved += OnReactionRemoved;
        _client.MessageDeleted += OnMessageDeleted;
        //_client.MessageUpdated += OnMessageUpdated;
        _client.ButtonExecuted += OnButtonExecuted;
        _client.Ready += OnReady;
        _client.UserJoined += OnUserJoined;
        _client.SlashCommandExecuted += OnSlashCommandExecuted;
        _client.ModalSubmitted += OnModalSubmitted;

        string token = "secret";
        if (isTestVersion) token = "secret2";

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    private async Task OnReady() //executes on bot launch
    {
        if (isTestVersion)
        {
            _main = _client.GetGuild(1018508433709858856);
            roleRP = _main.GetRole(1019952509470638100);
            roleERP = _main.GetRole(1019952363399811173);
            roleNSFW = _main.GetRole(1019952251634200658);
            roleGuest = _main.GetRole(1018512899578282145);
            roleHe = _main.GetRole(1022007373348479077);
            roleShe = _main.GetRole(1022007403350331395);
            roleThey = _main.GetRole(1022007424640634910);
            roleAsk = _main.GetRole(1022132533200167003);
            channelLogs = _main.GetTextChannel(1019759417727655936);
            channelTickets = _main.GetTextChannel(1019802284919631982);
            channelVerify = _main.GetTextChannel(1018510038563815424);
            channelWelcome = _main.GetTextChannel(1019924716221374505);
            channelRules = _main.GetTextChannel(1018515260082237482);
            channelFeedback = _main.GetTextChannel(1054024568949452920);
            channelAvailability = _main.GetTextChannel(1082225352815939664);
            roleSecurity = _main.GetRole(1082225509284454410);
            roleDealer = _main.GetRole(1082232438450831360);
            roleBarkeeper = _main.GetRole(1082232836800659476);
            roleHost = _main.GetRole(1082232866437607507);
            rolePhotographer = _main.GetRole(1082232961270829136);
            rolesMessage = 1103028844912455722;
            categoryTickets = 1019789132362432572;
        }
        else
        {
            _main = _client.GetGuild(1015024187716403230);
            roleRP = _main.GetRole(1016414340896800921);
            roleERP = _main.GetRole(1016414435134427206);
            roleNSFW = _main.GetRole(1015208764074381312);
            roleGuest = _main.GetRole(1015231451291209729);
            roleHe = _main.GetRole(1015594920876179506);
            roleShe = _main.GetRole(1015595073473355806);
            roleThey = _main.GetRole(1015595124568379442);
            roleAsk = _main.GetRole(1015595161440501841);
            channelLogs = _main.GetTextChannel(1016403694914383944);
            channelTickets = _main.GetTextChannel(1016413184875298996);
            channelVerify = _main.GetTextChannel(1015195657964879952);
            channelWelcome = _main.GetTextChannel(1015198476600090625);
            channelRules = _main.GetTextChannel(1015024347385180250);
            channelFeedback = _main.GetTextChannel(1054016094223990815);
            channelAvailability = _main.GetTextChannel(1015024386778087524);
            roleSecurity = _main.GetRole(1015231391614644324);
            roleDealer = _main.GetRole(1023514076929863721);
            roleBarkeeper = _main.GetRole(1015231422551818280);
            roleHost = _main.GetRole(1015231365458968596);
            rolePhotographer = _main.GetRole(1017082084633878598);
            rolesMessage = 1020449323772809277;
            categoryTickets = 1020448455719665745;
        }
        
        //cache 10000 latest messages of each channel to allow deleted/edited messages logging
        foreach (var channel in _main.TextChannels)
        {
            var messages = await channel.GetMessagesAsync(10000).FlattenAsync();
            _messageCache[channel.Id] = messages.ToList();
        }

        //OnlyOnce onlyOnce = new OnlyOnce();
        //await onlyOnce.CreateSlashCommands(_client, _main);
        //await onlyOnce.CreateVerifyMessage(channelRules, channelVerify);
        //await onlyOnce.CreateTicketMessage(channelTickets);
        //await onlyOnce.CreateTicketStaffMessage(channelFeedback);
    }
    private async Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
    }
    public async Task PurgeAsync(SocketTextChannel Channel)
    {
        var messages = await Channel.GetMessagesAsync(225).FlattenAsync();
        var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);
        if (filteredMessages.Count() != 0)
        {
            await (Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
        }
    }
    private async Task OnUserJoined(SocketGuildUser user)
    {
        var embed = new EmbedBuilder { Color = Color.Blue }
            .WithAuthor(user)
            .WithTitle($"Welcome to your Wild Dreams Host Club {user.DisplayName}!")
            .WithDescription(
                $"Head to {channelVerify.Mention} to make your Dreams come true\n" +
                $"Make sure to read our {channelRules.Mention}\n" +
                $"Check out our ▁𝗩𝗘𝗡𝗨𝗘▁ category for more information");
        await channelWelcome.SendMessageAsync("", false, embed.Build());
    }
    private async Task OnModalSubmitted(SocketModal modal)
    {
        List<SocketMessageComponentData> components = modal.Data.Components.ToList();
        string title = components.First(x => x.CustomId == "title").Value;
        string description = components.First(x => x.CustomId == "description").Value;

        if (modal.Data.CustomId == "bot_giveaway")
        {
            string link = components.First(x => x.CustomId == "link").Value;

            if (title != "" || description != "")
            {
                var author = new EmbedAuthorBuilder()
                .WithName("Wild Dreams Host Club")
                .WithIconUrl(_main.CurrentUser.GetAvatarUrl());

                var embed = new EmbedBuilder { };
                embed.WithColor(Discord.Color.Blue)
                //.WithAuthor(author)
                .WithTitle(title)
                .WithDescription(description)
                .WithFooter(footer => footer.Text = "React below with 🎉 to participate in the giveaway 💗");
                await modal.Channel.SendMessageAsync("", false, embed.Build());

                await Task.Delay(100);

                var messages = await modal.Channel.GetMessagesAsync(1).FlattenAsync();
                await messages.First().AddReactionAsync(new Emoji("🎉"));

                ulong messageID = messages.First().Id;

                var embedLogs = new EmbedBuilder { };
                embedLogs.WithColor(Discord.Color.Blue)
                .WithAuthor(author)
                .WithTitle("Giveaway: " + title)
                .WithFooter(footer => footer.Text = "Choose a random winner with the button below 💗");
                //.WithFooter(footer => footer.Text = "💗 " + user.Mention);
                await channelLogs.SendMessageAsync("", false, embedLogs.Build());

                var buttonClose = new ButtonBuilder()
                {
                    Label = "Choose a winner",
                    CustomId = "buttonWinner",
                    Style = ButtonStyle.Success,
                };
                var component = new ComponentBuilder();
                component.WithButton(buttonClose);
                await channelLogs.SendMessageAsync(" ", false, components: component.Build());

                await channelLogs.SendMessageAsync(modal.Channel.Id.ToString());
                await channelLogs.SendMessageAsync(messages.First().Id.ToString());

            }

            if (link != "")
            {
                await modal.Channel.SendMessageAsync(link);
            }

            await modal.RespondAsync("Giveaway started", null, false, true);
            return;
        }
        else if (modal.Data.CustomId == "bot_message")
        {
            string link = components.First(x => x.CustomId == "link").Value;

            if (title != "" || description != "")
            {
                var embed = new EmbedBuilder { };
                embed.WithColor(Discord.Color.Blue)
                .WithTitle(title)
                .WithDescription(description);
                await modal.Channel.SendMessageAsync("", false, embed.Build());
            }

            if (link != "")
            {
                await modal.Channel.SendMessageAsync(link);
            }

            await modal.RespondAsync("Message sent", null, false, true);
            return;
        }
        else if (modal.Data.CustomId == "bot_modify")
        {
            string messageID = components.First(x => x.CustomId == "messageID").Value;
            string channelID = components.First(x => x.CustomId == "channelID").Value;

            SocketTextChannel channelModify = _main.GetTextChannel(Convert.ToUInt64(channelID));

            var msg = await channelModify.GetMessageAsync(Convert.ToUInt64(messageID));
            if (msg is IUserMessage message)
            {
                await message.ModifyAsync(x =>
                {
                    EmbedBuilder embed = new EmbedBuilder();
                    embed.WithColor(Discord.Color.Blue);
                    embed.WithTitle(title);
                    embed.WithDescription(description);
                    x.Embed = embed.Build();
                });
            }
            await modal.RespondAsync("Message edited", null, false, true);
            return;
        }
        else if (modal.Data.CustomId == "bot_availability")
        {
            await modal.RespondAsync("Starting availability check", null, false, true);
            await PurgeAsync(channelAvailability);

            string finalTitle = title;
            if (description != "" && title != "") { finalTitle += " - "; }

            var embed = new EmbedBuilder()
            .WithColor(Discord.Color.Blue)
            .WithTitle(finalTitle + description)
            .WithDescription("✅  - I will be there\r\n❎  - I will not be there\r\n❓  - I am unsure");
            await channelAvailability.SendMessageAsync("", false, embed.Build());

            foreach (var role in new[] { roleHost, roleDealer, roleBarkeeper, rolePhotographer, roleSecurity })
            {
                var message = await channelAvailability.SendMessageAsync(role.Mention);
                foreach (var emoji in new[] { "✅", "❎", "❓" }) await message.AddReactionAsync(new Emoji(emoji));
            }
            return;
        }
    }
    private async Task OnSlashCommandExecuted(SocketSlashCommand arg)
    {
        if (arg.Data.Name == "message")
        {
            var mb = new ModalBuilder()
            .WithTitle("Bot message")
            .WithCustomId("bot_message")
            .AddTextInput("Title", "title", TextInputStyle.Paragraph, "Title", null, null, false, null)
            .AddTextInput("Description", "description", TextInputStyle.Paragraph, "Description", null, null, false, null)
            .AddTextInput("Picture Link", "link", TextInputStyle.Paragraph, "Leave blank if no picture", null, null, false, null);
            await arg.RespondWithModalAsync(mb.Build());
        }

        else if (arg.Data.Name == "modify")
        {
            string messageLink = (string)arg.Data.Options.First().Value;
            var messageID = messageLink.Substring(messageLink.LastIndexOf('/') + 1);
            var channelID = messageLink.Substring(messageLink.LastIndexOf('/') - 19);
            channelID = channelID.Substring(0, channelID.LastIndexOf('/'));
            SocketTextChannel channelModify = _main.GetTextChannel(Convert.ToUInt64(channelID));
            var msg = await channelModify.GetMessageAsync(Convert.ToUInt64(messageID));

            if (msg is IUserMessage message)
            {
                var mb = new ModalBuilder()
                .WithTitle("Edit message").WithCustomId("bot_modify")
                .AddTextInput("Title", "title", TextInputStyle.Paragraph, null, null, null, false, msg.Embeds.First().Title)
                .AddTextInput("Description", "description", TextInputStyle.Paragraph, null, null, null, false, msg.Embeds.First().Description)
                .AddTextInput("Message ID (Do not change)", "messageID", TextInputStyle.Paragraph, messageID, 19, 19, false, messageID)
                .AddTextInput("Channel ID (Do not change)", "channelID", TextInputStyle.Paragraph, channelID, 19, 19, false, channelID);
                await arg.RespondWithModalAsync(mb.Build());
            }
            else
                await arg.RespondAsync("Message not found", null, false, true);
        }

        if (arg.Data.Name == "giveaway")
        {
            var mb = new ModalBuilder()
            .WithTitle("Bot giveaway")
            .WithCustomId("bot_giveaway")
            .AddTextInput("Title", "title", TextInputStyle.Paragraph, "Title", null, null, false, null)
            .AddTextInput("Description", "description", TextInputStyle.Paragraph, "Description", null, null, false, null)
            .AddTextInput("Picture Link", "link", TextInputStyle.Paragraph, "", null, null, false, null);
            await arg.RespondWithModalAsync(mb.Build());
        }

        if (arg.Data.Name == "availability")
        {
            var mb = new ModalBuilder()
            .WithTitle("Availability check")
            .WithCustomId("bot_availability")
            .AddTextInput("Event name (optional)", "title", TextInputStyle.Paragraph, "Grand Opening", null, null, false, null)
            .AddTextInput("Date (optional)", "description", TextInputStyle.Paragraph, "23.09.2022", null, null, false, null);
            await arg.RespondWithModalAsync(mb.Build());
        }
    }
    private async Task OnButtonExecuted(SocketMessageComponent arg)
    {
        if (arg.Data.CustomId == "buttonWinner")
        {
            var test = arg.Message;
            var test2 = arg.Message.Id;

            var messages = await arg.Channel.GetMessagesAsync().FlattenAsync();
            var messagesList = messages.ToList();
            var result = messagesList.FindIndex(x => x.Id == test2);


            SocketTextChannel messageGivewayChannel = _main.GetTextChannel(Convert.ToUInt64(messagesList[result - 1].Content));
            var msg = await messageGivewayChannel.GetMessageAsync(Convert.ToUInt64(messagesList[result - 2].Content));
            var possibleWinners = msg.GetReactionUsersAsync(new Emoji("🎉"), 3331);
            var winners2 = possibleWinners.ToListAsync();

            var count = winners2.Result.ElementAt(0).Count();
            if (count == 1)
            {
                await arg.RespondAsync("Can't choose winner (no reactions on giveaway)", null, false, true);
                return;
            }

        again:
            Random random = new Random();
            int randomID = random.Next(count);

            var winner = winners2.Result.ElementAt(0).ElementAt(randomID);
            if (winner.IsBot) goto again;

            await messageGivewayChannel.SendMessageAsync(winner.Mention + " has won the Giveaway");
            await arg.RespondAsync("Winner chosen (" + winner.Mention + ")", null, false, true);
        }

        if (arg.Data.CustomId == "buttonRules")
        {
            if (!_main.GetUser(arg.User.Id).Roles.Contains(roleGuest))
            {
                await _main.GetUser(arg.User.Id).AddRoleAsync(roleGuest);
                //await channelLogs.SendMessageAsync(arg.User + " has accepted the rules");
                await arg.RespondAsync("You've been given the Guest role\r\n" + "Enjoy the Wild Dreams Host Club! ", null, false, true);
            }
            else await arg.RespondAsync("You've already accepted the rules", null, false, true);
        }

        if (arg.Data.CustomId == "buttonClose")
        {
            var messages = await arg.Channel.GetMessagesAsync().FlattenAsync();
            await (arg.Channel as SocketTextChannel).DeleteAsync();

            var logMessage = "";
            foreach (var message in messages.Reverse())
            {
                if (!message.Author.IsBot)
                {
                    var timeString = message.CreatedAt.ToLocalTime().ToString("g");
                    logMessage += $"[{timeString}] {message.Author}: {message.Content}\n";
                }
            }

            var embed = new EmbedBuilder()
            .WithColor(Color.Purple)
            .WithAuthor(arg.User)
            .WithDescription("Ticket closed (" + arg.Channel.Name + ")");

            if (logMessage.Length > 0)
            {
                await File.WriteAllTextAsync("ticket-log.txt", "Ticket transcript\n\n" + logMessage);
                await channelLogs.SendFileAsync("ticket-log.txt", "", false, embed: embed.Build());
            }
            else
            {
                await channelLogs.SendMessageAsync(embed: embed.Build());
            }
        }

        if (new[] { "buttonVIP", "buttonRoom", "buttonHost", "buttonOther", "buttonFeedback" }.Contains(arg.Data.CustomId))
        {
            var username = new string(arg.User.ToString().Split('#')[0].ToLower().Where(c => Char.IsLetterOrDigit(c)).ToArray());
            if (arg.Data.CustomId == "buttonFeedback") username = "staff-" + username;

            var existingChannel = _main.Channels.SingleOrDefault(x => x.Name == "ticket-" + username) as SocketTextChannel;
            if (existingChannel != null) { await arg.RespondAsync("You already have one currently open ticket at " + existingChannel.Mention, null, false, true); return; }

            var newChannel = await _main.CreateTextChannelAsync("ticket-" + username, props =>
            {
                props.PermissionOverwrites = new List<Overwrite>
                {
                    new Overwrite(_main.EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny)),
                    new Overwrite(arg.User.Id, PermissionTarget.User, new OverwritePermissions(viewChannel: PermValue.Allow))
                };
                props.CategoryId = categoryTickets;
            });

            await arg.RespondAsync("Your ticket has been created at " + newChannel.Mention, null, false, true);

            string chosentopic = "Other Inquiries";
            Color colorEmbedTicket = Color.Blue;
            if (arg.Data.CustomId == "buttonHost") chosentopic = "Host Booking";
            else if (arg.Data.CustomId == "buttonVIP") chosentopic = "VIP Tickets";
            else if (arg.Data.CustomId == "buttonRoom") chosentopic = "Room Booking";
            else if (arg.Data.CustomId == "buttonFeedback") { chosentopic = "Feedback"; colorEmbedTicket = Color.Red; }

            var embedTicket = new EmbedBuilder { };
            embedTicket.WithColor(colorEmbedTicket)
            .WithTitle("Thank you for contacting us ")
            .WithDescription("\r\nChosen topic: " + chosentopic + "\nWe will get to you at once " + arg.User.Mention)
            .WithFooter(footer => footer.Text = "In the meantime, you may leave us some details about your inquiry");
            await newChannel.SendMessageAsync("", false,embed: embedTicket.Build(), components: new ComponentBuilder().WithButton(new ButtonBuilder() { Label = "Close ticket", CustomId = "buttonClose", Style = ButtonStyle.Danger }).Build());

            var embed = new EmbedBuilder()
            .WithColor(Color.Blue)
            .WithAuthor(arg.User)
            .WithDescription("Ticket created (" + newChannel.Mention + ")");
            await channelLogs.SendMessageAsync(embed: embed.Build());
        }
    }

    private async Task OnMessageReceived(SocketMessage arg)
    {
        if (arg.Author.IsBot) return;

        if (arg.Channel.GetChannelType() == ChannelType.DM)
        {
            await arg.Channel.SendMessageAsync($"Hello my beloved {arg.Author.Username}" +
                "\r\nUnfortunately, I am just a dummy bot and can't offer human help" +
                "\r\nPlease open a ticket on our server in case you need support :heart:");
        }
        // if (arg.Author.Id == 407600275948437504) { if (arg.Content.StartsWith("!test "))  { } }
    }

    private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (reaction.User.Value.IsBot) return;

        if (message.Id == rolesMessage)
        {
            if (reaction.Emote.Name == "🗨") await _main.GetUser(reaction.UserId).AddRoleAsync(roleRP);
            else if (reaction.Emote.Name == "💗") await _main.GetUser(reaction.UserId).AddRoleAsync(roleERP);
            else if (reaction.Emote.Name == "🔞") await _main.GetUser(reaction.UserId).AddRoleAsync(roleNSFW);
            else if (reaction.Emote.Name == "♂️") await _main.GetUser(reaction.UserId).AddRoleAsync(roleHe);
            else if (reaction.Emote.Name == "♀️") await _main.GetUser(reaction.UserId).AddRoleAsync(roleShe);
            else if (reaction.Emote.Name == "⚧") await _main.GetUser(reaction.UserId).AddRoleAsync(roleThey);
            else if (reaction.Emote.Name == "💬") await _main.GetUser(reaction.UserId).AddRoleAsync(roleAsk);
        }
    }

    private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (reaction.User.Value.IsBot) return;

        if (message.Id == rolesMessage)
        {
            if (reaction.Emote.Name == "🗨") await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleRP);
            else if (reaction.Emote.Name == "💗") await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleERP);
            else if (reaction.Emote.Name == "🔞") await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleNSFW);
            else if (reaction.Emote.Name == "♂️") await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleHe);
            else if (reaction.Emote.Name == "♀️") await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleShe);
            else if (reaction.Emote.Name == "⚧") await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleThey);
            else if (reaction.Emote.Name == "💬") await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleAsk);
        }
    }
    private async Task OnMessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        var messageBefore = await before.GetOrDownloadAsync();
        if (messageBefore?.Author?.IsBot != false) return;

        if (messageBefore.Content == after.Content)
        {
            // Use cached message if available
            if (_messageCache.TryGetValue(channel.Id, out var messageList))
            {
                var cachedMessage = messageList.FirstOrDefault(m => m.Id == messageBefore.Id);
                if (cachedMessage != null)
                {
                    messageBefore = cachedMessage;
                    _messageCache[channel.Id][messageList.FindIndex(m => m.Id == before.Id)] = after;
                }
            }
        }
        //TODO field supports only up to 1024 characters. 
        var embed = new EmbedBuilder()
            .WithColor(Color.Green)
            .WithAuthor($"{messageBefore.Author.Username}#{messageBefore.Author.Discriminator}", messageBefore.Author.GetAvatarUrl())
            .WithDescription($"**Message edited** in #" + channel.Name)
            .AddField("Before", messageBefore.Content != after.Content ? messageBefore.Content : "<Previous content unknown>", true)
            .AddField("After", after.Content, true);

        await channelLogs.SendMessageAsync(embed: embed.Build());
    }
    private async Task OnMessageDeleted(Cacheable<Discord.IMessage, ulong> cacheableMessage, Cacheable<IMessageChannel, ulong> channel)
    {
        var message = await cacheableMessage.GetOrDownloadAsync();

        if (message == null)
        {
            if (_messageCache.TryGetValue(channel.Id, out var messageList))
            {
                message = messageList.FirstOrDefault(m => m.Id == cacheableMessage.Id);
            }
        }

        if (message?.Author?.IsBot != false) return;

        var embed = new EmbedBuilder()
            .WithColor(Color.DarkRed)
            .WithAuthor($"{message.Author.Username}#{message.Author.Discriminator}", message.Author.GetAvatarUrl())
            .WithDescription("**Message deleted** in #" + channel.Value.Name + "\n" + message.Content);

        if (message.Attachments.Any())
        {
            var imageAttachments = message.Attachments.Where(a => a.Url.EndsWith(".png")).ToList();
            if (imageAttachments.Any())
            {
                embed.WithImageUrl(imageAttachments.First().Url);
            }
            //TODO or just simply attach them? No need for Url just SendMessageAsync with those attachments
            var attachmentUrls = message.Attachments.Select(a => a.Url).ToList();
            embed.AddField("Attachments", string.Join("\n", attachmentUrls));
        }

        await channelLogs.SendMessageAsync(embed: embed.Build());
    }
}
