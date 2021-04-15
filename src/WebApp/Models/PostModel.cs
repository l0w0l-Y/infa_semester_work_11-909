﻿namespace WebApp.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string Text { get; set; }
        public int RequiredSubscriptionType { get; set; }
    }
}