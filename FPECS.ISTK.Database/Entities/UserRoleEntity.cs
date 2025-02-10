using FPECS.ISTK.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPECS.ISTK.Database.Entities;
public class UserRoleEntity
{
    public long UserId { get; set; }
    public UserEntity? User { get; set; }
    public required AvailableRole Role { get; set; }
}