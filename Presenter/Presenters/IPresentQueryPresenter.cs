using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Presenter;

public interface IPresentQueryPresenter
{
    Task LoadWishlistPresentsAsync(string wishlistId);
    Task SearchPresentsByKeywordAsync(string keyword);
    Task LoadReservedPresentsAsync(string userId);
}