using BukaToko.ASyncService;
using BukaToko.DTOS;
using BukaToko.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;


//TODO: kelarin GetAllOrder
namespace BukaToko.Data
{
    public class OrderRepo : IOrderRepo
    {
        private readonly BukaTokoDbContext _context;
        private readonly IMessageBusClient _messageBusClient;

        public OrderRepo(BukaTokoDbContext context, IMessageBusClient messageBusClient)
        {
            _context = context;
            _messageBusClient = messageBusClient;
        }

        public async Task AddToCart(int userId,Cart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }
            //kalau order belom ada bikin baru
            var order = await _context.Orders.Where(o=>o.UserId == userId && o.Checkout == false).FirstOrDefaultAsync();

            if (order == null)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var tempOrder = new Order
                        {
                            UserId = userId,
                            Checkout = false,
                            Shipped = false
                        };
                        await _context.Orders.AddAsync(tempOrder);
                        await _context.SaveChangesAsync();

                        var t = tempOrder;
                        var tempCart = new Cart
                        {
                            OrderId = tempOrder.Id,
                            Name = cart.Name,
                            Price = cart.Price,
                            Quantity = cart.Quantity,

                        };
                        await _context.Carts.AddAsync(tempCart);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            else
            {
                //cek jika barang ada di kerangjang
                var getCart = await _context.Carts.Where(o => o.Name == cart.Name && o.OrderId == order.Id).FirstOrDefaultAsync();

                //kalau gk ada add, kalau gk update qty
                if (getCart == null)
                {
                    var tempCart = new Cart
                    {
                        OrderId = order.Id,
                        Name = cart.Name,
                        Price = cart.Price,
                        Quantity = cart.Quantity,

                    };
                    await _context.Carts.AddAsync(tempCart);
                }
                else
                {
                    getCart.Quantity += cart.Quantity;
                }
                
                await _context.SaveChangesAsync();
            }
        }

        public async Task Checkout(int userId)
        {
            var order = await _context.Orders.Where(o => o.UserId==userId && o.Checkout==false ).FirstOrDefaultAsync();
            if (order!=null)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var totalPrice = 0;
                        var cart = await _context.Carts.Where(o=>o.OrderId == order.Id).ToListAsync();
                        if (cart==null) throw new Exception("cart empty");
                        //loop semua cart yang ada
                        foreach (var item in cart)
                        {
                            //check qty product
                            var product = await _context.Products.Where(o=>o.Name == item.Name).FirstOrDefaultAsync();
                            if (product == null) throw new Exception("product not found");

                            //cek kalau stok tidak cukup
                            if((product.Stock - item.Quantity)<0) throw new Exception("insufficient stock");

                            //kuraingin stok barang
                            totalPrice += product.Price * item.Quantity;
                            product.Stock -= item.Quantity;
                            await _context.SaveChangesAsync();
                        }

                        //get wallet
                        var user = await _context.Users.Where(o => o.Id==userId).FirstOrDefaultAsync();
                        var wallet = await _context.Wallets.Where(o => o.Id == user.WalletId).FirstOrDefaultAsync();
                        //cek kalau stok tidak cukup
                        if ((wallet.Cash - totalPrice) < 0) throw new Exception("no cash");

                        //kurangin cash
                        wallet.Cash -= totalPrice;
                        order.Checkout = true;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        // publish new wallet message to message bus
                        var walletPublishDto = new WalletPublishDto
                        {
                            Username = user.Username,
                            Cash = totalPrice ,
                            Event = "CashoutWallet_NewPublished"
                        };
                        _messageBusClient.PublishNewWallet(walletPublishDto);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

                 
            }
            
        }

        public async Task DeleteFromCart(int userId, int id)
        {

            //check id exist
            var temp = await GetListCartUser(userId);
            if (temp != null)
            {
                var item = temp.Find(o => o.Id == id);
                if (item != null)
                {
                    _context.Carts.Remove(item);
                }
                await _context.SaveChangesAsync();
            }
            
        }

        public async Task<IEnumerable<Order>?> GetAllOrder()
        {
            return await _context.Orders.ToListAsync();

        }

        public async Task<Cart?> GetCartById(int userId, int id)
        {
            var temp = await GetListCartUser(userId);
            if (temp != null)
            {
                var item = temp.Find(o => o.Id == id);
                if (item != null)
                {
                    return item;
                }
            }
            return null;
        }

        public async Task<List<Cart>?> GetListCartUser(int userId)
        {
            var listItem = await _context.Carts.Where(o => o.Order.UserId == userId && o.Order.Checkout == false).ToListAsync();
            
            if (listItem != null)
            {
                return listItem;
            }
            else return null;
        }

        public async Task<int?> GetUserId(string username)
        {
            //throw new NotImplementedException();
            var user = await _context.Users.Where(o => o.Username == username).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            return user.Id;
        }

        public async Task Shipped(int orderId)
        {
            var order = await _context.Orders.Where(o => o.Shipped == false && o.Checkout == true).FirstOrDefaultAsync();
            if(order != null)
            {
                order.Shipped = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateQty(int userId, int id, int qty)
        {
            var cart = await GetCartById(userId,id);
            if (cart != null)
            {
                cart.Quantity = qty;
                await _context.SaveChangesAsync();
            }
        }
    }
}
