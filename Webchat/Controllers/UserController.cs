﻿using Chat.Data.Entities;
using Chat.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Chat.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ChatDbContext _context;
        public UserController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ChatDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;


        }


        [HttpPost]

        public async Task<IActionResult> PostUser(UserCreateRequest request) // vì khởi tạo lên ta dùng request
        {
            var dob = DateTime.Parse(request.Dob);
            var user = new User() // vì tạo một User lên ta dùng User Entites luân vì nó có đủ các tường
            {
                //Id = Guid.NewGuid().ToString(),
                //Email = request.Email,
                //BirthDay = DateTime.Parse(request.Dob),
                //UserName = request.UserName,
                //DisPlayName = request.LastName,

                //PhoneNumber = request.PhoneNumber,

            };
            var result = await _userManager.CreateAsync(user, request.Password); // phương thức CreateAsync đã được Identity.Core, hỗ trợ , bài miên phí ta phải viết nó
            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse(result));
            }
        }




        [HttpGet]

        public async Task<IActionResult> GetUsers()
        {
            var users = _userManager.Users;

            var uservms = await users.Select(u => new UserViewModel() // vì muốn xem lên ta dùng UserVm
            {
                //Id = u.Id,
                //UserName = u.UserName,
                //Dob = u.BirthDay,
                //Email = u.Email,
                //PhoneNumber = u.PhoneNumber,
                //FirstName = u.DisPlayName,

            }).ToListAsync();

            return Ok(uservms);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new ApiNotFoundResponse($"Cannot found user with id: {id}"));

            var userVm = new UserViewModel()
            {
                //Id = user.Id,
                //UserName = user.UserName,
                //Dob = user.BirthDay,
                //Email = user.Email,
                //PhoneNumber = user.PhoneNumber,
                //FirstName = user.DisPlayName,

            };
            return Ok(userVm);
        }
    }
}