using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
namespace View;

public interface IUserView
{
    Task ShowUser();
    Task AuthUser();
    void UpdateUserList();
    
}