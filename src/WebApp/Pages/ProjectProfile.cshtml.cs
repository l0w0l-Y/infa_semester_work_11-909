﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;
using WebApp.Models.Chats;
using WebApp.Models.Developer;
using WebApp.Models.Identity;
using WebApp.Models.Posts;
using WebApp.Models.Subscription;
using WebApp.Services;
using WebApp.Services.Chats;
using WebApp.Services.Developer;
using WebApp.Services.Posts;

namespace WebApp.Pages
{
    public class ProjectProfile : PageModel
    {
        private readonly IDeveloperService _developerService;
        private readonly IPostsService _postsService;
        private readonly IChatService _chatService;
        
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectProfile(IDeveloperService developerService, IPostsService postsService, IChatService chatService, UserManager<ApplicationUser> userManager)
        {
            _developerService = developerService;
            _postsService = postsService;
            _userManager = userManager;
            _chatService = chatService;
        }

        public ProjectModel ProjectModel { get; private set; }
        public IEnumerable<PostModel> PostModels { get; private set; }
        public List<(string, MessageModel)> Messages { get; private set; }
        
        public async Task<ActionResult> OnGetAsync(int id)
        {
            ProjectModel = await _developerService.GetProject(id);

            if (ProjectModel is null)
                return NotFound();
            
            ProjectModel.Tags = await _developerService.GetTags(ProjectModel) ?? new List<TagModel>();
            ProjectModel.Company = await _developerService.GetProjectCompany(id);
            ProjectModel.Users = await _developerService.GetProjectUsers(id) ?? new List<UserModel>();
            PostModels = await _postsService.GetProjectPosts(id) ?? new List<PostModel>();
            Messages = await GetAllMessages(id);
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int id, string type, string text)
        {
            //todo add image
            //todo add files
            ProjectModel = await _developerService.GetProject(id);

            if (ProjectModel is null)
                return NotFound();
            
            ProjectModel.Users = await _developerService.GetProjectUsers(id) ?? new List<UserModel>();
            var userId = (await _userManager.GetUserAsync(User)).UserId;
            
            if (!ProjectModel.Users.Select(u => u.Id).Contains(userId))
                return Forbid();
            
            var requiredType = type switch
            {
                "free" => PriceType.Free,
                "basic" => PriceType.Basic,
                "improved" => PriceType.Improved,
                "max" => PriceType.Max,
                _ => throw new NotSupportedException()
            };
            var post = new PostModel {ProjectId = id, UserId = userId, RequiredSubscriptionType = requiredType, Text = text};
            await _postsService.CreatePost(post);
            return Redirect($"/ProjectProfile?id={id}");
        }

        public async Task<List<(string, MessageModel)>> GetAllMessages(int projectId)
        {
            var messages = new List<(string, MessageModel)>();
            var allMesages = (await _chatService.GetMessagesByProjectId(projectId)).OrderBy(x=> x.DateTime);
            foreach (var message in allMesages)
            {
                var userId = message.UserId;
                //var name = (await _developerService.GetUser(userId)).Name;
                var name = userId.ToString();
                messages.Add((name, message));
            }

            return messages;
        }
    }
}