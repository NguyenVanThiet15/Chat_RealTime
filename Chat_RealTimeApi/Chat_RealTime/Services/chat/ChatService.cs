using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using Chat_RealTime.Controllers.chat.Dtos;
using Chat_RealTime.Models;
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using MongoDB.Driver;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Chat_RealTime.Services.chat
{
    public class ChatService : IChatService
    {
        private readonly IMongoCollection<Message> _messages;
        private readonly IMongoCollection<Chat> _chats;


        private readonly IMinioClient _minioClient;
        private readonly MiniOSetting _settings;

        public ChatService(
            MongoDBContext contextMessage,
            MongoDBContext contextChat,
            IMinioClient minioClient,
            IOptions<MiniOSetting> settings
            )
        {

            _messages = contextMessage.Message;
            _chats = contextChat.Chat;
            _minioClient = minioClient;
            _settings = settings.Value;
            if (_minioClient == null)
            {
                Console.WriteLine("⚠️ MinIO client is not available");
            }
        }


        public async Task SaveMesageAsync(Message input)
        {
            await _messages.InsertOneAsync(input);
        }
        public async Task<Chat> CreateChatRoom(CreateChatRoom input)
        {
            var listChatroom = await _chats.Find(x => x.Name.Trim().ToLower().Equals(input.NameRoom.Trim().ToLower())).FirstOrDefaultAsync();
            if (listChatroom != null)
            {
                return listChatroom;
            }
            else
            {
                var roomChat = new Chat
                {
                    Name = input.NameRoom,
                    Participants = input.NguoiThamGia,
                    Type = ChatType.Group,
                };
                await _chats.InsertOneAsync(roomChat);
                return roomChat;
            }

        }
        public async Task<List<Chat>> GetUserChatsAsync(string userId)
        {
            return await _chats.Find(c => c.Participants.Contains(userId) && c.Type == ChatType.Group).ToListAsync();
        }
        public async Task<Chat> CreateOrGetChat(CreateOrGetChatInput input)
        {
            if (!Enum.TryParse<ChatType>(input.ChatType, true, out var chatTypeInput))
            {
                chatTypeInput = ChatType.Private;
            }
            if (chatTypeInput == ChatType.Group)
            {
                var existringRoom = await _chats.Find(x => x.Type == ChatType.Group &&
                (x.Id == input.roomId || x.Name.Trim().ToLower().Equals(input.NameRoom.Trim().ToLower()))).FirstOrDefaultAsync();

                if (existringRoom != null)
                {
                    return existringRoom;
                }
                else
                {
                    var newChatRoom = new Chat
                    {
                        Name = input.NameRoom,
                        Type = ChatType.Group,
                        Id = input.roomId,
                        Participants = input.participant,
                        CreatedAt = DateTime.Now,
                        //        UpdatedAt = DateTime.Now,
                    };
                    await _chats.InsertOneAsync(newChatRoom);
                    return newChatRoom;
                }
            }
            else
            {
                var existringChat = await _chats.Find(c => c.Type == ChatType.Private &&
                c.Participants.Contains(input.NguoiGuiID) &&
                c.Participants.Contains(input.NguoiNhanId)).FirstOrDefaultAsync();
                if (existringChat != null)
                {
                    return existringChat;
                }
                else
                {
                    var newChat = new Chat
                    {
                        Name = "",
                        Type = ChatType.Private,
                        Participants = new List<string> { input.NguoiGuiID, input.NguoiNhanId },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };
                    await _chats.InsertOneAsync(newChat);

                    return newChat;
                }
            }
        }

        public async Task<List<Message>> GetMessage(string ChatId, int skip)
        {
            var mesages = await _messages.Find(c => c.ChatId == ChatId)
                .Sort(Builders<Message>.Sort.Descending(m => m.CreatedAt))
                .Skip(skip)
                .Limit(20)
                .ToListAsync();
            mesages.Reverse();
            return mesages;
        }


        public async Task<string> UploadFIleAsync(IFormFile file)
        {
            if (_minioClient == null)
                throw new InvalidOperationException("MinIO is not configured");

            using var inputStream = file.OpenReadStream();
            using var outStream = new MemoryStream();

            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(inputStream)) {

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(500, 0)
                }));
                await image.SaveAsJpegAsync(outStream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                {
                    Quality = 50
                });
            }
            outStream.Position = 0;  // ❗ Bắt buộc reset

            var bucketName = _settings.BucketName;
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";

            bool found = await _minioClient.BucketExistsAsync(
                new Minio.DataModel.Args.BucketExistsArgs().WithBucket(bucketName));

            if (!found) {
                await _minioClient.MakeBucketAsync(
                    new Minio.DataModel.Args.MakeBucketArgs().WithBucket(bucketName));
            }
            //using var stream = file.OpenReadStream();
            await _minioClient.PutObjectAsync(
            new PutObjectArgs()
             .WithBucket(bucketName)
             .WithObject(fileName)
             .WithStreamData(outStream)
             .WithObjectSize(outStream.Length)
             .WithContentType(file.ContentType)
             );

            var url = await _minioClient.PresignedGetObjectAsync(
                new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName)
            .WithExpiry(60 * 60)
            );// 1 giờ

            return url;
            //return $"https://{_settings.Endpoint}/{bucketName}/{fileName}";

        }
        public async Task InsertdataMessage(InsertMessageInput input)
        {
            var list = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                list.Add(new Message
                {
                    ChatId = input.ChatId,
                    SenderId = input.SenderId,
                    SenderName = input.SenderName,
                    Content = $"Tin nhan {i}",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                    IsRead = false,
                    Type = 0
                }
                    );
            }
            await _messages.InsertManyAsync(list);

        } 
    }

}
