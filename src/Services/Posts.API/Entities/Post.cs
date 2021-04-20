﻿using System.Collections.Generic;
namespace Posts.API.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string Text { get; set; }
        //todo make enum
        public int RequiredSubscriptionType { get; set; }
        public string ImageName { get; set; }
        
        public List<Comment> Comments { get; set; }
        public List<File> Files { get; set; }

        public Post(int userId, int projectId, string text, int requiredSubscriptionType, string imageName)
        {
            UserId = userId;
            ProjectId = projectId;
            Text = text;
            RequiredSubscriptionType = requiredSubscriptionType;
            ImageName = imageName;
        }
        
        public Post()
        {
        }
    }
}