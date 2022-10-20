using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using IMessage = Discord.IMessage;
using Image = System.Drawing.Image;
using System.IO;
using System.Net;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Messaging;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;

namespace DiscordBot
{
    public class Program
    {

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        ulong serverID = 1015024187716403230;
        ulong rolesMessage = 1;
        ulong categoryTickets = 1020448455719665745;
        ulong botID = 1019368036811149424;
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
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            var _config = new DiscordSocketConfig
            {
                MessageCacheSize = 1000,
                GatewayIntents = GatewayIntents.All
            };

            _client = new DiscordSocketClient(_config);

            _client.Log += Log;
            _client.MessageReceived += OnMessageReceived;
            _client.ReactionAdded += OnReactionAdded;
            //  _client.MessageUpdated += OnMessageUpdated;
            _client.ReactionRemoved += OnReactionRemoved;
            //   _client.MessageDeleted += OnMessageDeleted;
            _client.ButtonExecuted += OnButtonExecuted;
            _client.Ready += OnReady;
            _client.UserJoined += OnUserJoined;
            _client.SlashCommandExecuted += OnSlashCommandExecuted;
            _client.ModalSubmitted += OnModalSubmitted;

            var token = "";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();



            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
        }
        public async Task PurgeAsync(SocketTextChannel Channel)
        {

            var messages = await Channel.GetMessagesAsync(225).FlattenAsync();

            var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

            var count = filteredMessages.Count();

            if (count == 0)
            { }
            else
            {

                await (Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
            }
        }


        private async Task OnUserJoined(SocketGuildUser user)
        {

            //Random rnd = new Random();
            //int month = rnd.Next(1, 16581375); //255*255*255 i guess it make sense; works? works.

            var embed = new EmbedBuilder { };
            // embed.WithColor(new Color(Convert.ToUInt32(month)))
            embed.WithColor(Discord.Color.Blue)
            .WithTitle("Welcome to your Wild Dreams Host Club " + user.DisplayName + "!")
            .WithDescription("Please head to " + channelVerify.Mention + " to make your Dreams come true"
            + "\nAlso make sure to read our " + channelRules.Mention
            + "\nFor more information take a look at our  ▁𝗩𝗘𝗡𝗨𝗘▁ category"
            + "\n\n" + user.Mention + " 💗");
            //.WithFooter(footer => footer.Text = "💗 " + user.Mention);
            await channelWelcome.SendMessageAsync("", false, embed.Build());

            /*string url = user.GetAvatarUrl(ImageFormat.Png, 256) ?? user.GetDefaultAvatarUrl();
            Image img = Image.FromFile("C:\\Users\\mark\\Desktop\\WD.png");
            Image img1 = Image.FromFile("C:\\Users\\mark\\Desktop\\WD2.png");
            Image img2 = GetImageFromPicPath(url);
            using (Graphics g = Graphics.FromImage(img))
                g.DrawImage(img2, 472, 180, img2.Width, img2.Height);
            using (Graphics g = Graphics.FromImage(img))
                g.DrawImage(img1, 0, 0, img1.Width, img1.Height);
            img.Save("C:\\Users\\mark\\Desktop\\test.png");
            await channelWelcome.SendFileAsync("C:\\Users\\mark\\Desktop\\test.png", "");*/


        }
        private async Task OnModalSubmitted(SocketModal modal)
        {
            List<SocketMessageComponentData> components = modal.Data.Components.ToList();
            string title = components.First(x => x.CustomId == "title").Value;
            string description = components.First(x => x.CustomId == "description").Value;
            string link = components.First(x => x.CustomId == "link").Value;


            if (modal.Data.CustomId == "bot_giveaway")
            {
                if (title != "" || description != "")
                {

                    var author = new EmbedAuthorBuilder()
                    .WithName("Wild Dreams Host Club")
                    .WithIconUrl(_main.GetUser(botID).GetAvatarUrl());

                    var embed = new EmbedBuilder { };
                    embed.WithColor(Discord.Color.Blue)
                    //.WithAuthor(author)
                    .WithTitle(title)
                    .WithDescription(description)
                    .WithFooter(footer => footer.Text = "React below with 🎉 below to participate in the giveaway 💗");
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
                        Label = "Choose winner",
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


            if (modal.Data.CustomId == "bot_message")
            {
                if (title != "" || description != "")
                {
                    var embed = new EmbedBuilder { };
                    embed.WithColor(Discord.Color.Blue)
                    .WithTitle(title)
                    .WithDescription(description);
                    //.WithFooter(footer => footer.Text = "💗 " + user.Mention);
                    await modal.Channel.SendMessageAsync("", false, embed.Build());
                }

                if (link != "")
                {
                    await modal.Channel.SendMessageAsync(link);
                }

                await modal.RespondAsync("Message sent", null, false, true);
                return;
            }
            //
            if (modal.Data.CustomId == "bot_modify")
            {

                var msg = await modal.Channel.GetMessageAsync(Convert.ToUInt64(link));

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
        }
        private async Task OnSlashCommandExecuted(SocketSlashCommand arg)
        {
            if (arg.Data.Name == "message")
            {
                // arg.RespondAsync("Opening message menu", null, false, true);

                var mb = new ModalBuilder()
            .WithTitle("Bot message")
            .WithCustomId("bot_message")
            .AddTextInput("Title", "title", TextInputStyle.Paragraph, "Title", null, null, false, null)
             .AddTextInput("Description", "description", TextInputStyle.Paragraph, "Description", null, null, false, null)
             .AddTextInput("Picture Link", "link", TextInputStyle.Paragraph, "", null, null, false, null);

                await arg.RespondWithModalAsync(mb.Build());
            }

            if (arg.Data.Name == "modify")
            {
                string linktomessage = (string)arg.Data.Options.First().Value;
                var result = linktomessage.Substring(linktomessage.LastIndexOf('/') + 1);

                var mb = new ModalBuilder()
            .WithTitle("Edit message")
            .WithCustomId("bot_modify")
            .AddTextInput("Title", "title", TextInputStyle.Paragraph, "Title", null, null, false, null)
             .AddTextInput("Description", "description", TextInputStyle.Paragraph, "Description", null, null, false, null)
             .AddTextInput("Message ID (Do not touch)", "link", TextInputStyle.Paragraph, result, 19, 19, false, result);

                await arg.RespondWithModalAsync(mb.Build());
            }

            if (arg.Data.Name == "giveaway")
            {
                // arg.RespondAsync("Opening message menu", null, false, true);

                var mb = new ModalBuilder()
            .WithTitle("Bot giveaway")
            .WithCustomId("bot_giveaway")
            .AddTextInput("Title", "title", TextInputStyle.Paragraph, "Title", null, null, false, null)
             .AddTextInput("Description", "description", TextInputStyle.Paragraph, "Description", null, null, false, null)
             .AddTextInput("Picture Link", "link", TextInputStyle.Paragraph, "", null, null, false, null);
                await arg.RespondWithModalAsync(mb.Build());

            }

        }
        private async Task OnReady()
        {


            _main = _client.GetGuild(serverID);

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
            // channelGlobal = _main.GetTextChannel(1018508433709858859);

            //IMessage message = await channelGlobal.GetMessageAsync(1020022057427292290);
            //string text = message.Content;
            //Console.WriteLine(text);

            //await PurgeAsync(channelTickets);
            //await PurgeAsync(channelVerify);
            //await PurgeAsync(channelLogs);
            //await PurgeAsync(channelWelcome);
            //await PurgeAsync(channelGlobal);
            //await _client.Rest.DeleteAllGlobalCommandsAsync();

            var commandMessage = new SlashCommandBuilder()
            .WithName("message")
            .WithDescription("Send a message from within the bot.")
            .WithDefaultMemberPermissions(GuildPermission.Administrator);
            await _client.Rest.CreateGuildCommand(commandMessage.Build(), _main.Id);

            var commandModify = new SlashCommandBuilder()
            .WithName("modify")
            .WithDescription("Edit a message sent by the bot.")
            .AddOption("string", ApplicationCommandOptionType.String, "Enter message link", true)
            .WithDefaultMemberPermissions(GuildPermission.Administrator);
            await _client.Rest.CreateGuildCommand(commandModify.Build(), _main.Id);

            var commandGiveaway = new SlashCommandBuilder()
            .WithName("giveaway")
            .WithDescription("Start a giveaway.")
            .WithDefaultMemberPermissions(GuildPermission.Administrator);
            await _client.Rest.CreateGuildCommand(commandGiveaway.Build(), _main.Id);


            var messages = await channelVerify.GetMessagesAsync(1).FlattenAsync();
            rolesMessage = messages.First().Id;

            /*
            var embed = new EmbedBuilder { };
            embed.WithColor(Discord.Color.Blue)
            .WithTitle("Welcome to the Wild Dreams Host Club")
            .WithDescription("In order to gain full access to the server you need to agree to our Discord rules \n" + channelRules.Mention + "\n\r" +
            "Visit our website [here](https://wilddreamshostclub.carrd.co/)!")
            .WithFooter(footer => footer.Text = "See you soon");
            //await channelVerify.SendMessageAsync("", false, embed.Build());

            var buttonAccept = new ButtonBuilder()
            {
                Label = "Accept Rules",
                CustomId = "buttonRules",
                Style = ButtonStyle.Success
            };
            var component1 = new ComponentBuilder();
            component1.WithButton(buttonAccept);
            await channelVerify.SendMessageAsync(" ", false, components: component1.Build());

            

            var embedRoles = new EmbedBuilder { };
            embedRoles.WithColor(Discord.Color.Blue)
            .WithTitle("Roles Assignment")
            .WithDescription("React below to assign chosen roles"
            + "\n 🗨 - " + roleRP.Mention
            + "\n 💗 - " + roleERP.Mention
            + "\n 🔞 - " + roleNSFW.Mention
            + "\n 🤠 - " + roleHe.Mention
            + "\n 👧 - " + roleShe.Mention
            + "\n ♾️ - " + roleThey.Mention
            );
            // .WithFooter(footer => footer.Text = "See you soon");
            await channelVerify.SendMessageAsync("", false, embedRoles.Build());

            messages = await channelVerify.GetMessagesAsync(1).FlattenAsync();
            rolesMessage = messages.First().Id;
            */

            var msg = await channelVerify.GetMessageAsync(1020449323772809277);

            if (msg is IUserMessage message)
            {
                Console.WriteLine("here2");

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
                    + "\n 💬 - " + roleAsk.Mention
            );
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

            /*

            var embedTicket = new EmbedBuilder { };
            embedTicket.WithColor(Discord.Color.Blue)
            .WithTitle("Booking and general inquiries")
            .WithDescription("\r\nLet us know if you need help or want to book one of our services\n" +
            "Please select a category according to your inquiry")
            .WithFooter(footer => footer.Text = "We'll get back to you as soon as possible.");
            await channelTickets.SendMessageAsync("", false, embedTicket.Build());


            var buttonVIP = new ButtonBuilder()
            {
                Label = "VIP Tickets",
                CustomId = "buttonVIP",
                Style = ButtonStyle.Primary,
            };
            var buttonRoom = new ButtonBuilder()
            {
                Label = "Room Booking",
                CustomId = "buttonRoom",
                Style = ButtonStyle.Primary,
            };
            var buttonHost = new ButtonBuilder()
            {
                Label = "Host Booking",
                CustomId = "buttonHost",
                Style = ButtonStyle.Primary,
            };
            var buttonOther = new ButtonBuilder()
            {
                Label = "Other Inquiries",
                CustomId = "buttonOther",
                Style = ButtonStyle.Primary,
            };
            var component = new ComponentBuilder();
            component.WithButton(buttonVIP);
            component.WithButton(buttonRoom);
            component.WithButton(buttonHost);
            component.WithButton(buttonOther);
            await channelTickets.SendMessageAsync(" ", false, components: component.Build());
            */

            //await channelLogs.SendMessageAsync("starting at: " + DateTime.Now.ToString("MM/dd/yyyy h:mm tt"));

        }
        public static Image GetImageFromPicPath(string strUrl)
        {
            using (WebResponse wrFileResponse = WebRequest.Create(strUrl).GetResponse())
            using (Stream objWebStream = wrFileResponse.GetResponseStream())
            {
                MemoryStream ms = new MemoryStream();
                objWebStream.CopyTo(ms, 8192);
                return System.Drawing.Image.FromStream(ms);
            }
        }
        private async Task OnButtonExecuted(SocketMessageComponent arg)
        {
            var which = arg.Data.CustomId;

            if (which == "buttonWinner")
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
                Random random = new Random(); int randomID = random.Next(count);

                var winner = winners2.Result.ElementAt(0).ElementAt(randomID);
                if (winner.IsBot) goto again;

                await messageGivewayChannel.SendMessageAsync(winner.Mention + " has won the Giveaway");
                await arg.RespondAsync("Winner chosen (" + winner.Mention + ")", null, false, true);


                //IUser test = winner.Result.;


            }
            if (which == "buttonRules")
            {

                var userrr = _main.GetUser(arg.User.Id);
                var test = userrr.Roles;
                if (!userrr.Roles.Contains(roleGuest))
                {

                    await _main.GetUser(arg.User.Id).AddRoleAsync(roleGuest);
                    await channelLogs.SendMessageAsync(arg.User + " has accepted the rules");
                    await arg.RespondAsync("You've been verified and given the Guest role\r\n" +
                        "Enjoy the Wild Dreams Host Club! ", null, false, true);
                }
                else
                {
                    await arg.RespondAsync("You've already been verified", null, false, true);
                }
            }


            if (which == "buttonClose")
            {
                /*arg.RespondAsync("Ticket will be closed in 5 sec", null, false, true);
                await Task.Delay(2000);
                arg.Channel.SendMessageAsync("Ticket closed in 3 sec");
                await Task.Delay(1000);
                arg.Channel.SendMessageAsync("Ticket closed in 2 sec");
                await Task.Delay(1000); 
                arg.Channel.SendMessageAsync("Ticket closed in 1 sec");*/



                SocketTextChannel test = (SocketTextChannel)arg.Channel;
                await test.DeleteAsync();

                await channelLogs.SendMessageAsync(arg.User + " closed a ticket (" + arg.Channel.Name + ")");



                /* var capture = await test.GetMessagesAsync().FlattenAsync();
                 // var filteredMessages = capture.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

                 var count = capture.Count();

                 string logged = "";
                 foreach (var message in capture)
                 {
                     logged += "\r\n[+] " + message.Author + " - " + message.Timestamp.ToString().Split('+')[0] + ": " + message.Content;
                 }

                 logged = string.Join("\n", logged.Split('\n').Reverse());

                 await arg.Channel.SendMessageAsync(logged);*/

            }


            if (which == "buttonVIP" || which == "buttonRoom" || which == "buttonHost" || which == "buttonOther")
            {
                string username = arg.User.ToString().Split('#')[0].ToLower();
                username = Regex.Replace(username, "[^a-zA-Z0-9]", "");
                //TODO check if channel of the same name exists already, if yes then dont create new one
                var channelFound = _main.Channels.SingleOrDefault(x => x.Name == "ticket-" + username);
                if (channelFound != null)
                {
                    await arg.RespondAsync("You already have one currently open ticket", null, false, true);
                    return;

                }
                //arg.RespondAsync("ticket-" + arg.User.ToString().Split('#')[0].ToLower(), null, false, true);

                List<Overwrite> permissions = new List<Overwrite>
                         {
                            new Overwrite(_main.EveryoneRole.Id, PermissionTarget.Role,
                            new OverwritePermissions(viewChannel: PermValue.Deny)),
                            new Overwrite(arg.User.Id, PermissionTarget.User,
                            new OverwritePermissions(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow, sendMessages: PermValue.Allow,
                    attachFiles: PermValue.Allow, addReactions: PermValue.Allow))
                            };

                TextChannelProperties props = new TextChannelProperties()
                {
                    CategoryId = categoryTickets,
                    PermissionOverwrites = permissions
                };

                RestTextChannel newChannel = await _main.CreateTextChannelAsync
                    ("ticket-" + username, daa => daa.PermissionOverwrites = permissions);
                await newChannel.ModifyAsync(prop => prop.CategoryId = categoryTickets);

                //  await newChannel.AddPermissionOverwriteAsync(_main.EveryoneRole, OverwritePermissions.DenyAll(newChannel));
                //  await newChannel.AddPermissionOverwriteAsync(arg.User, OverwritePermissions.DenyAll(newChannel)
                //     .Modify(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow, sendMessages: PermValue.Allow,
                //     attachFiles: PermValue.Allow, addReactions: PermValue.Allow));

                string chosentopic = "Other Inquiries";
                if (which == "buttonHost") { chosentopic = "Host Booking"; }
                if (which == "buttonVIP") { chosentopic = "VIP Tickets"; }
                if (which == "buttonRoom") { chosentopic = "Room Booking"; }

                var embedTicket = new EmbedBuilder { };
                embedTicket.WithColor(Discord.Color.Blue)
                .WithTitle("Thank you for contacting us ")
                .WithDescription("\r\nChosen topic: " + chosentopic +
                "\nWe will get to you at once " + arg.User.Mention)
                .WithFooter(footer => footer.Text = "In the meantime, you may leave us some details about your inquiry");
                await newChannel.SendMessageAsync("", false, embedTicket.Build());

                await arg.RespondAsync("Your ticket has been created at " + newChannel.Mention, null, false, true);

                var buttonClose = new ButtonBuilder()
                {
                    Label = "Close ticket",
                    CustomId = "buttonClose",
                    Style = ButtonStyle.Danger,
                };
                var component = new ComponentBuilder();
                component.WithButton(buttonClose);
                await newChannel.SendMessageAsync(" ", false, components: component.Build());

                // SocketGuildUser user = (SocketGuildUser)arg.User;
                // await user.ModifyAsync(x => x.ChannelId = newChannel.Id);

                await channelLogs.SendMessageAsync(arg.User + " created a ticket (" + newChannel.Mention + ")");

            }



        }
        private async Task OnMessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.DownloadAsync();
            Console.WriteLine($"{before.GetOrDownloadAsync().ToString()} -> {after.Content}");
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (arg.Author.IsBot) { return; }

            if (arg.Channel.GetChannelType() != ChannelType.DM)

            {
                if (arg.Author.Id == 407600275948437504)
                {
                    if (arg.Content.StartsWith("!asbot "))
                    { }
                }

                return;
            }

            await arg.Channel.SendMessageAsync($"Hello my beloved {arg.Author.Username}" +
                "\r\nUnfortunately, I am just a dummy bot and can't offer human help" +
                "\r\nPlease open a ticket on our server in case you need support :heart:");

        }
        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            if (reaction.UserId == _client.CurrentUser.Id) { return; }
            if (message.Id == rolesMessage)
            {
                if (reaction.Emote.Name == "🗨") { await _main.GetUser(reaction.UserId).AddRoleAsync(roleRP); }
                if (reaction.Emote.Name == "💗") { await _main.GetUser(reaction.UserId).AddRoleAsync(roleERP); }
                if (reaction.Emote.Name == "🔞") { await _main.GetUser(reaction.UserId).AddRoleAsync(roleNSFW); }
                if (reaction.Emote.Name == "♂️") { await _main.GetUser(reaction.UserId).AddRoleAsync(roleHe); }
                if (reaction.Emote.Name == "♀️") { await _main.GetUser(reaction.UserId).AddRoleAsync(roleShe); }
                if (reaction.Emote.Name == "⚧") { await _main.GetUser(reaction.UserId).AddRoleAsync(roleThey); }
                if (reaction.Emote.Name == "💬") { await _main.GetUser(reaction.UserId).AddRoleAsync(roleAsk); }

            }
        }

        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {

            if (message.Id == rolesMessage)
            {
                if (reaction.Emote.Name == "🗨") { await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleRP); }
                if (reaction.Emote.Name == "💗") { await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleERP); }
                if (reaction.Emote.Name == "🔞") { await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleNSFW); }
                if (reaction.Emote.Name == "♂️") { await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleHe); }
                if (reaction.Emote.Name == "♀️") { await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleShe); }
                if (reaction.Emote.Name == "⚧") { await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleThey); }
                if (reaction.Emote.Name == "💬") { await _main.GetUser(reaction.UserId).RemoveRoleAsync(roleAsk); }
            }
        }

        private async Task OnMessageDeleted(Cacheable<Discord.IMessage, ulong> arg, Cacheable<IMessageChannel, ulong> channel)
        {
            SocketTextChannel challened = channelLogs;

            //var test = arg.;

            // Console.WriteLine(test);

            await challened.SendMessageAsync($"Deleted message: '{arg.GetOrDownloadAsync()}'");
            // challened.SendMessageAsync($"Deleted message: \"'{test}'\"");

            //channeld.SendM
        }
    }
}
