using System;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using System.IO;


namespace iAgree
{
    [ApiVersion(1, 22)]
    public class iAgree : TerrariaPlugin
    {
        public static Config Config;
        public static bool[] HasAgreed = new bool[255];
        public static bool[] HasReadRules = new bool[255];

        #region TerrariaPlugin

        public override string Name
        {
            get
            {
                return "iAgree";
            }
        }

        public override string Author
        {
            get
            {
                return "Jewsus";
            }
        }

        public override string Description
        {
            get
            {
                return "";
            }
        }

        public override void Initialize()
        {
            if (!File.Exists(Config.SavePath))
            {
                Config = new Config();
                Config.Save();
            }
            else
                Config = Config.Read();


            ServerApi.Hooks.ServerChat.Register(this, OnChat);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
            Commands.ChatCommands.Add(new Command(Agree, "iagree"));
            Commands.ChatCommands.Add(new Command("iagree.helpme", new CommandDelegate(HelpMe), new string[]
                {
                "helpme"
                 }));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
            }
            base.Dispose(disposing);
        }
        public iAgree(Main game) : base(game)
        {
            Order = 1;
        }

        private void OnChat(ServerChatEventArgs e)
        {
            TSPlayer tSPlayer = TShock.Players[e.Who];
            if (e.Text.StartsWith("/login") && !tSPlayer.IsLoggedIn && !iAgree.HasAgreed[e.Who] && HasReadRules[e.Who])
            {
                e.Handled = true;
                tSPlayer.SendErrorMessage("Before logging in you have to confirm you have read the /rules, to do this type /iagree");
            }
            if (e.Text.StartsWith("/rules"))
            {
                HasReadRules[e.Who] = true;
            }

            if (!e.Text.StartsWith("/"))
            {
                if (!tSPlayer.IsLoggedIn)
                {
                    e.Handled = true;
                    tSPlayer.SendErrorMessage("You can only talk when registered & logged in!");
                    tSPlayer.SendErrorMessage("Please follow the instructions on the sign on how to register & log in!");
                    tSPlayer.SendErrorMessage("Trouble finding the sign/too lazy to read? Short instructions: /helpme");
                }
                else if (!tSPlayer.Group.HasPermission("iagree.bypass.chat") && e.Text.Length >= 10)
                {
                    int num = 0;
                    char[] array = e.Text.ToCharArray();
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (char.IsUpper(array[i]))
                        {
                            num++;
                        }
                    }
                    if (100 / e.Text.Length * num > 25)
                    {
                        e.Handled = true;
                        tSPlayer.SendErrorMessage("Your message has been ignored for containing more than 25% caps!");
                    }
                }
            }
        }

        private void OnLeave(LeaveEventArgs e)
        {
            iAgree.HasAgreed[e.Who] = false;
        }

        private void HelpMe(CommandArgs args)
        {
            if (!args.Player.IsLoggedIn)
            {
                args.Player.SendInfoMessage("Step #1: Type /register <password of choice>");
                args.Player.SendInfoMessage("Step #2: Type /login <password used for registering");
            }
        }

        private void Agree(CommandArgs args)
        {
            if (iAgree.HasReadRules[args.Player.Index])
            {
                iAgree.HasAgreed[args.Player.Index] = true;
                Console.WriteLine("Player agreed to Rules.");
                args.Player.SendSuccessMessage(Config.AgreeMessage);
            }
            else
                args.Player.SendErrorMessage("You must first read the rules! Type /rules to learn more.");
        }


        #endregion

    }
}
