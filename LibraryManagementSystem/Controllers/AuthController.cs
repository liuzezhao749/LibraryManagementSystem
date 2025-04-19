using System.Text.RegularExpressions;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public AuthController(
            ApplicationDbContext context,
            IEmailService emailService,
            ITokenService tokenService)
        {
            _context = context;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // 验证姓拼音和名拼音首字母格式
            if (!Regex.IsMatch(request.LastNamePinyin, @"^[a-z]+$"))
                return BadRequest("姓拼音必须是小写字母");

            if (!Regex.IsMatch(request.FirstNameInitials, @"^[a-z]+$"))
                return BadRequest("名拼音首字母必须是小写字母");

            // 验证8位数字后缀
            if (!Regex.IsMatch(request.NumberSuffix, @"^\d{8}$"))
                return BadRequest("数字后缀必须为8位数字");

            // 生成邮箱（新格式：姓拼音 + 名拼音首字母 + 8位数字）
            var email = $"{request.LastNamePinyin}{request.FirstNameInitials}{request.NumberSuffix}@mails.jlu.edu.cn";

            // 检查邮箱是否已存在
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return BadRequest("该邮箱已被注册");

            // 创建用户
            var user = new User
            {
                Email = email,
                Name = request.Name,
                Role = "Reader"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Email = email, Message = "注册成功" });
        }

        // POST: api/auth/request-login
        [HttpPost("request-login")]
        public async Task<IActionResult> RequestLogin(LoginRequest request)
        {
            // 更新邮箱格式验证（包含8位数字）
            if (!Regex.IsMatch(request.Email, @"^[a-z]+[a-z]+\d{4}@mails\.jlu\.edu\.cn$"))
                return BadRequest("邮箱格式不正确");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return NotFound("用户不存在");

            // 生成一次性登录令牌（有效期10分钟）
            var token = Guid.NewGuid().ToString();
            user.MagicLinkToken = token;
            user.TokenExpiry = DateTime.UtcNow.AddMinutes(10);
            await _context.SaveChangesAsync();

            // 发送登录链接邮件
            var loginLink = $"{Request.Scheme}://{Request.Host}/api/auth/verify-login?token={token}";
            await _emailService.SendEmailAsync(
                request.Email,
                "图书馆系统登录链接",
                $"请点击以下链接登录系统（10分钟内有效）: {loginLink}");

            return Ok("登录链接已发送到您的邮箱");
        }

        // GET: api/auth/verify-login
        [HttpGet("verify-login")]
        public async Task<IActionResult> VerifyLogin(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.MagicLinkToken == token &&
                u.TokenExpiry > DateTime.UtcNow);

            if (user == null)
                return BadRequest("无效或过期的登录令牌");

            // 清除令牌
            user.MagicLinkToken = null;
            user.TokenExpiry = null;
            await _context.SaveChangesAsync();

            // 生成JWT令牌
            var jwtToken = _tokenService.GenerateToken(user);

            return Ok(new
            {
                Token = jwtToken,
                Expiry = DateTime.UtcNow.AddHours(1)
            });
        }
    }
}