using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;

namespace GmailDb.Managers
{
    // Starting to realize this project will probably be more tedious than it is challanging.. 
    // abandoning for now.
    public class GmailApi
    {
        private static string[] Scopes = { GmailService.Scope.GmailCompose, GmailService.Scope.GmailInsert, GmailService.Scope.GmailModify
                , GmailService.Scope.GmailReadonly, GmailService.Scope.GmailSend, GmailService.Scope.GmailSettingsBasic
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
            }

            service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // TODO get the Meta table
            //var response = service.Users.Drafts.List("me").Execute(); //"1622666d16d21169" messageid
            //var result = new List<Draft>();
            //result.AddRange(response.Drafts);

            var messages = service.Users.Messages.List("me").Execute();
            var request = service.Users.Messages.Get("me", "1622666d16d21169");
            request.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
            var response = request.Execute();
            // Wellllll damn. Looks like List basically can only get you IDs which you then have to request individually.
            // In order for this to work I'll need to batch up my rows. Thats annoying.



            // TODO If Meta table DNE, build a blank one

            // TODO Map the tables from Meta into our Table object
            //      should also probably take this opportunity to create table -> id list buckets
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
                Subject = "Meta",
                Body = @"CANTSTOP!@#$%^&*()_+-=/\""WONTSTOP",
                From = new MailAddress($"{DbName}@gmail.com")
            };
            msg.To.Add(new MailAddress($"{DbName}@gmail.com"));
            msg.ReplyTo.Add(msg.From); // Bounces without this!!

            var gmailMsg = ConvertToGmailMessage(msg);



            //////////////
            var draft = new Draft
            {
                Message = gmailMsg
            };
            //service.Users.Drafts.Create(draft, "me").Execute();

            //Next stop.... how to add an attachment.
        }

        private static Message ConvertToGmailMessage(AE.Net.Mail.MailMessage message)
        {
            var msgStr = new StringWriter();
            message.Save(msgStr);
            return new Message
            {
                Raw = Base64UrlEncode(msgStr.ToString())
            };
        }

        private static string Base64UrlEncode(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }





    }



}