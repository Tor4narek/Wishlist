using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Presenter;
using Repository;
using View;
namespace View;

public interface IWishlistView
{
    void ShowUserWishlists(List<Wishlist> wishlists); 
    void UpdateWishlist();
}