using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;

namespace GmailDb.Managers
{

    public class GmailApi
    {
        private static string[] Scopes = { GmailService.Scope.GmailCompose, GmailService.Scope.GmailInsert, GmailService.Scope.GmailModify
                , GmailService.Scope.GmailMetadata, GmailService.Scope.GmailReadonly, GmailService.Scope.GmailSend, GmailService.Scope.GmailSettingsBasic
                , GmailService.Scope.GmailSettingsSharing, GmailService.Scope.MailGoogleCom};
        private static string ApplicationName = "Gmail API .NET";
        private static string DbName = "hobbzDb1";
        private GmailService service;

        public GmailApi()
        {
            UserCredential credential;
            var path = HttpContext.Current.Server.MapPath(@"~/client_secret.json");
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var credPath = HttpContext.Current.Server.MapPath(@"~/SecretResp/");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public void ApiTest()
        {
            //UsersResource.LabelsResource.ListRequest request = service.Users.Labels.List("me");

            // List labels.
            //IList<Label> labels = request.Execute().Labels;
            //var msg = new Message();
            /////////
            var msg = new AE.Net.Mail.MailMessage
            {
                Subject = "Subject Hur",
                Body = @"CANTSTOP!@#$%^&*()_+-=/\""WONTSTOP",
                From = new MailAddress($"{DbName}@gmail.com")
            };
            msg.To.Add(new MailAddress($"Meta@{DbName}.com"));
            msg.ReplyTo.Add(msg.From); // Bounces without this!!
            var msgStr = new StringWriter();
            msg.Save(msgStr);

            var gmailMsg = new Message
            {
                Raw = Base64UrlEncode(msgStr.ToString())
            };


            //////////////
            var draft = new Draft
            {
                Message = gmailMsg
            };
            service.Users.Drafts.Create(draft, "me").Execute();

            //Next stop.... how to add an attachment.
        }

        private static string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }



    }



}