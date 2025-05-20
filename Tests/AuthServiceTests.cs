using Microsoft.Extensions.Configuration;
using CodeLifeAcademy.Application.DTOs;
using CodeLifeAcademy.Application.Interfaces;
using CodeLifeAcademy.Core.Entities;
using CodeLifeAcademy.Core.Enums;
using CodeLifeAcademy.Infrastructure.Persistence;
using CodeLifeAcademy.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Tests;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IJwtService> _jwtServiceMock;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        _configurationMock = new Mock<IConfiguration>();
        _jwtServiceMock = new Mock<IJwtService>();

        _authService = new AuthService(_dbContext,
                                       _passwordHasherMock.Object,
                                       _configurationMock.Object,
                                       _jwtServiceMock.Object);
    }


    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenDataIsValid()
    {
        var studentRole = new Role { Name = UserRoleEnum.Student.ToString() };
        _dbContext.Roles.AddAsync(studentRole);
        await _dbContext.SaveChangesAsync();

        var registerDto = new RegisterUserDto
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "securepassword"
        };

        _passwordHasherMock
            .Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
            .Returns("hashed_password");

        var result = await _authService.RegisterAsync(registerDto);

        Assert.NotNull(result);
        var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == result);
        Assert.NotNull(userInDb);
        Assert.Equal("testuser", userInDb.Username);
        Assert.Equal("hashed_password", userInDb.PasswordHash);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentails_ShouldReturnAuthResult()
    {
        var user = new User
        {
            Username = "testuser",
            Email = "testuser@example.com",
            PasswordHash = "hashed_password",
            UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role
                    { Name = UserRoleEnum.Student.ToString()
                    }
                }
            }
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var loginDto = new LoginUserDto
        {
            Username = "testuser",
            Password = "plain_password"
        };

        _passwordHasherMock
        .Setup(p => p.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password))
        .Returns(PasswordVerificationResult.Success);

        // Мокаем генерацию токена
        _jwtServiceMock
            .Setup(j => j.CreateAccessToken(It.IsAny<List<Claim>>()))
            .Returns("fake_token");

        _jwtServiceMock
            .Setup(j => j.GenerateRefreshToken())
            .Returns("fake_refresh_token");

        var httpResponseMock = new Mock<HttpResponse>();
        httpResponseMock
            .Setup(r => r.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()));

        // Act
        var result = await _authService.LoginAsync(loginDto, httpResponseMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("fake_token", result.AccesToken);
    }
}
