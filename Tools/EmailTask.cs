using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using MimeKit;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


namespace Login.Tools
{
    public class EmailTask
    {

        private static SmtpClient client = null;
        private static MailboxAddress FROM = null;
        private static Task connection = null;
        private static List<TryoutMessages> tryouts = new List<TryoutMessages>();
        private static List<EmailMessage> pipeline = new List<EmailMessage>();

        private static async Task Connect(int tryout)
        {
            if (tryout < 10)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

                FROM = new MailboxAddress(configuration.GetSection("EmailServer")["Title"], configuration.GetSection("EmailServer")["Sender"]);

                Thread.Sleep(10000);
                var thisTryout = tryout;
                if (client == null || !client.IsConnected)
                {
                    using (client = new SmtpClient())
                    {
                        await client.ConnectAsync(configuration.GetSection("EmailServer")["ServerAddress"], 465, true);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        await client.AuthenticateAsync(FROM.Address, configuration.GetSection("EmailServer")["ServerPassword"]);
                        while (client != null && pipeline.Count > 0)
                        {
                            EmailMessage msg = pipeline[0];
                            bool ok = await ProcessPipeline(msg);
                            if (!ok)
                                tryouts.Add(new TryoutMessages { message = msg });
                        }
                        foreach (var item in tryouts.OrderBy(x => Guid.NewGuid()))
                        {
                            bool ok = await ProcessPipeline(item.message);
                            if (!ok && item.tryout > 4)
                                tryouts.Remove(item);
                        }
                    }
                }
                client = null;
                if (pipeline.Count() > 0 || tryouts.Count() > 0)
                    connection = Connect(thisTryout);
                else connection = null;
            }
        }

        private static async Task SendMsg(MimeMessage message)
        {
            await client.SendAsync(message);
        }

        public static void SendFormatedMail(string Subject, string name, string email, string textbody)
        {
            if (email != null)
            {
                var body = new BodyBuilder();
                var pathImage = "wwwroot/img/w-logo-small.png";
                var image = body.LinkedResources.Add(pathImage);

                image.ContentId = MimeUtils.GenerateMessageId();              

                var b = "<html><body><span style=\"text-align:center; border: none;\"><table style=\"width: 150%; width: 100%; background-color: rgb(8, 36, 63); border:5px solid rgb(240,240,240); " +
                    " padding: 90px 40px 40px 40px; text-align: left; border:none; \"><tr><th style=\" position: relative; " +
                    "font-size: 25px; color: rgb(115, 115, 115);\"><div style=\"width: 100%; display: flex; align-items: center; justify-content: center;\"><img style=\"display:flex; justify-content:center; align-content:center; margin-bottom: 40px;\" src=\"cid:" + image.ContentId + "\" /></div></th></tr><tr>" +
                    "<td style= \"background-color: rgba(219, 219, 219, 0.3); border-radius: 10px; box-sizing: border-box; padding: 40px; font-size: 16px; backdrop-filter: blur(10px); color: #fff; font-family: Arial, sans-serif;\">"+ textbody +"</td></tr><tr><td style=\"font-size:12px; color:rgb(175, 173, 173);  font-family: Arial, sans-serif; text-align: justify; margin-top:10px;\"><br/>(PT) Esta mensagem pode conter informação confidencial ou privilegiada, " +
                    "sendo seu sigilo protegido por lei. Se você não for o destinatário ou a pessoa autorizada a receber " +
                    "esta mensagem, não pode usar, copiar ou divulgar as informações nela contidas ou tomar qualquer" +
                    " ação baseada nessas informações. Se você recebeu esta mensagem por engano, por favor, avise " +
                    "imediatamente ao remetente, respondendo o e-mail e em seguida apague-a. Agradecemos sua cooperação." +
                    "<br/><br/>(EN) This message may contain confidential or privileged information and its confidentiality " +
                    "is protected by law. If you are not the addressed or authorized person to receive this message, " +
                    "you must not use, copy, disclose or take any action based on it or any information herein. If you " +
                    "have received this message by mistake, please advise the sender immediately by replying the e-mail " +
                    "and then deleting it. Thank you for your cooperation.</td></tr></table></span></body></html>";

                body.HtmlBody = b;
                SendMsg(Subject, body.ToMessageBody(),
                    (new MailboxAddress(name, email)));
            }
        }

        private static async void KeepAlive()
        {
            while (client != null && client.IsConnected)
            {
                lock (client.SyncRoot)
                {
                    client.NoOpAsync();
                }
                Thread.Sleep(1000);
                await Task.CompletedTask;
            }
        }

        public static void SendMsg(string subject, MimeEntity body, MailboxAddress toAddress)
        {

            pipeline.Add(new EmailMessage
            {
                subject = subject,
                body = body,
                toAddress = toAddress
            });
            if (client == null && (connection == null || connection.IsCompleted))
                connection = Task.Run(() => Connect(0));
        }

        public static async Task<bool> ProcessPipeline(EmailMessage msg)
        {
            var ok = false;
            if (pipeline.Contains(msg))
            {
                if (client != null && client.IsConnected && client.IsAuthenticated)
                {
                    var message = new MimeMessage();
                    message.From.Add(FROM);
                    message.To.Add(msg.toAddress);
                    message.Subject = msg.subject;
                    message.Body = msg.body;
                    try
                    {
                        await SendMsg(message);
                        pipeline.Remove(msg);
                        ok = true;
                    }
                    catch
                    {
                        if (client.IsConnected && client.IsAuthenticated)
                        {
                            pipeline.Remove(msg);
                        }
                        else
                        {
                            try
                            {
                                client.Disconnect(true);
                                client.Dispose();
                            }
                            catch
                            {

                            }
                            client = null;
                        }
                    }
                }
                else
                {
                    try
                    {
                        client.Disconnect(true);
                        client.Dispose();
                    }
                    catch
                    {

                    }
                    client = null;
                }
            }
            return ok;
        }

        public class TryoutMessages
        {
            public int tryout { get; set; } = 0;
            public EmailMessage message { get; set; }
        }
        public class EmailMessage
        {
            public string subject { get; set; }
            public MimeEntity body { get; set; }
            public MailboxAddress toAddress { get; set; }
        }
    }
}
