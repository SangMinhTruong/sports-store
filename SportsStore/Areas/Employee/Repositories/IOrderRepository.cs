
using Microsoft.EntityFrameworkCore;
using SportsStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAllOrders();
        Task<Order> Create(Order orderToCreate, IList<ProductItem> orderedProductsData);
        Order Read(int? Id);
        Order Update(Order order);
        Order Delete(int? Id);
    }
    public class EFOrderRepository : IOrderRepository
    {
        private readonly StoreDbContext _context;
        public EFOrderRepository(StoreDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders;
        }
        public async Task<Order> Create(Order orderToCreate, IList<ProductItem> orderedProductsData)
        {
            Order order = new Order()
            {
                PlacementDate = orderToCreate?.PlacementDate ?? DateTime.MinValue,
                Customer = orderToCreate.Customer,
                RecipientName = orderToCreate.RecipientName,
                RecipientAddress = orderToCreate.RecipientAddress,
                RecipientPhone = orderToCreate.RecipientPhone,
            };
            var orderedProducts = new List<OrderedProduct>();
            var products = await _context.Products
                                    .Include(p => p.OrderedProducts)
                                        .ThenInclude(op => op.Order)
                                    .ToListAsync();

            foreach (var productItem in orderedProductsData)
            {
                var product = products.FirstOrDefault(p => p.ID == productItem.Product.ID);
                var orderedProduct = new OrderedProduct
                {
                    Product = product,
                    Order = order,
                    Quantity = productItem.Quantity
                };
                product.OrderedProducts.Add(orderedProduct);
                orderedProducts.Add(orderedProduct);
                _context.Update(product);
            }
            order.OrderedProducts = orderedProducts;
            _context.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public Order Read(int? Id)
        {
            throw new System.NotImplementedException();
        }

        public Order Update(Order order)
        {
            throw new System.NotImplementedException();
        }

        public Order Delete(int? Id)
        {
            throw new System.NotImplementedException();
        }
    }
}
