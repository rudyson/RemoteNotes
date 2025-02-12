using FPECS.ISTK.Database;
using FPECS.ISTK.Shared.Requests.MemberProfile;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPECS.ISTK.Business.Services;
public interface IMemberProfileService
{
    Task<GetMemberProfileResponse> GetMemberProfileAsync(long memberId, CancellationToken cancellationToken = default);
    Task<GetMemberProfileResponse> UpdateMemberProfileAsync(UpdateMemberProfileRequest request, CancellationToken cancellationToken = default);
}

public class MemberProfileService : IMemberProfileService
{
    private readonly ApplicationDbContext _dbContext;
    public MemberProfileService(ApplicationDbContext context)
    {
        _dbContext = context;
    }
    public Task<GetMemberProfileResponse> GetMemberProfileAsync(long memberId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AsNoTracking()
            .Include(x => x.Roles)
            .ProjectToType<GetMemberProfileResponse>()
            .FirstAsync(x => x.Id == memberId, cancellationToken);
    }

    public async Task<GetMemberProfileResponse> UpdateMemberProfileAsync(UpdateMemberProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.AsTracking()
            .Include(x => x.Roles)
            .FirstAsync(x => x.Id == request.Id, cancellationToken);

        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;
        user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;
        user.Sex = request.Sex ?? user.Sex;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return user.Adapt<GetMemberProfileResponse>();
    }
}
