using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nursing_Home.Application.Repositories;
using Nursing_Home.Domain.Constants;
using Nursing_Home.Domain.Entities.Identities;
using Nursing_Home.Infrastructure.Persistence.Data;

namespace Nursing_Home.Infrastructure.Persistence.SendData;
public class BaseDataInitial
{
    private readonly ILogger<BaseDataInitial> _logger;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _applicationDbContext;
    public BaseDataInitial(ILogger<BaseDataInitial> logger,
       UserManager<User> userManager,
       RoleManager<Role> roleManager,
       IUnitOfWork unitOfWork,
       ApplicationDbContext applicationDbContext)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _applicationDbContext = applicationDbContext;
    }
    public async Task MigrateAsync()
    {
        try
        {
            await _applicationDbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
    private async Task TrySeedAsync()
    {
        if (!await _unitOfWork.Repository<Role>().ExistsByAsync())
        {
            foreach (var item in RoleInitial.Default)
            {
                await _roleManager.CreateAsync(item);
            }
            await _unitOfWork.CommitAsync();
        }

        if (!await _unitOfWork.Repository<User>().ExistsByAsync())
        {
            var userAdmin = new User
            {
                UserName = "Admin",
                Avatar = "https://kenh14cdn.com/thumb_w/660/2020/7/17/brvn-15950048783381206275371.jpg",
                DateOfBirth = "09-09-2001"
            };
            await _userManager.CreateAsync(userAdmin, "1");
            await _userManager.AddToRolesAsync(userAdmin, new[] { RoleName.Admin });

            var userUsers = new User
            {
                UserName = "User",
                Avatar = "https://img.baoninhbinh.org.vn//DATA/ARTICLES/2021/5/17/cuoc-dua-lot-vao-top-100-anh-dep-di-san-van-hoa-va-thien-7edf3.jpg",
                DateOfBirth = "09-09-2001"
            };
            await _userManager.CreateAsync(userUsers, "1");
            await _userManager.AddToRolesAsync(userUsers, new[] { RoleName.User });

            var userStaff = new User
            {
                UserName = "Staff",
                Avatar = "https://img.pikbest.com/ai/illus_our/20230418/64e0e89c52dec903ce07bb1821b4bcc8.jpg!w700wp",
                DateOfBirth = "09-09-2001"
            };
            await _userManager.CreateAsync(userStaff, "1");
            await _userManager.AddToRolesAsync(userStaff, new[] { RoleName.Staff });
            await _unitOfWork.CommitAsync();
        }

    }
}
