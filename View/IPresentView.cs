using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
namespace View;
public interface IPresentView
{
    Task ShowUserPresents(string userId);
    Task AddPresent(string userId, string wishlistId); 
    void ShowSearchedPresents(List<Present> presents);
    void ShowReservedPresents(List<Present> reservedPresents);
    Task UpdatePresentList();
}