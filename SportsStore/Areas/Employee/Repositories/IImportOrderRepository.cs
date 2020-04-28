
using Microsoft.EntityFrameworkCore;
using SportsStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public interface IImportOrderRepository
    {
        Task<IEnumerable<ImportOrder>> GetAllOrders();
        Task<ImportOrder> Create(ImportOrder orderToCreate, IList<ProductItem> orderedProductsData);
        Task<ImportOrder> Read(int? Id);
        Task<ImportOrder> Update(ImportOrder order);
        Task<ImportOrder> Delete(int? Id);
    }
    public class EFImportOrderRepository : IImportOrderRepository
    {
        private readonly StoreDbContext _context;
        public EFImportOrderRepository(StoreDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ImportOrder>> GetAllOrders()
        {
            return await _context.ImportOrders
                                .Include(io => io.ImportedProducts)
                                    .ThenInclude(ip => ip.Product)
                                .ToListAsync();
        }
        public async Task<ImportOrder> Create(ImportOrder orderToCreate, 
            IList<ProductItem> importedProductsData)
        {
            ImportOrder order = new ImportOrder()
            {
                PlacementDate = orderToCreate?.PlacementDate ?? DateTime.MinValue,
                WholesalerName = orderToCreate.WholesalerName,
                WholesalerAddress = orderToCreate.WholesalerAddress,
                WholesalerPhone = orderToCreate.WholesalerPhone,
            };
            var importedProducts = new List<ImportedProduct>();
            var productsQuery = _context.Products
                                        .Include(p => p.ImportedProducts)
                                            .ThenInclude(op => op.ImportOrder);

            foreach (var productItem in importedProductsData)
            {
                var product = await productsQuery
                                        .FirstOrDefaultAsync(p => p.ID == productItem.Product.ID);
                var importedProduct = new ImportedProduct
                {
                    Product = product,
                    ImportOrder = order,
                    Quantity = productItem.Quantity
                };
                product.ImportedProducts.Add(importedProduct);
                product.Stock = product.Stock + productItem.Quantity;

                importedProducts.Add(importedProduct);

                _context.Update(product);
            }
            order.ImportedProducts = importedProducts;
            _context.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<ImportOrder> Read(int? Id)
        {
            return await _context.ImportOrders
                                .Include(io => io.ImportedProducts)
                                    .ThenInclude(ip => ip.Product)
                                .FirstOrDefaultAsync(o => o.ID == Id);
        }

        public async Task<ImportOrder> Update(ImportOrder order)
        {
            var orderToUpdate = await _context.ImportOrders
                                .Include(io => io.ImportedProducts)
                                    .ThenInclude(ip => ip.Product)
                                .FirstOrDefaultAsync(o => o.ID == order.ID);
            
            orderToUpdate.PlacementDate = order.PlacementDate;
            orderToUpdate.WholesalerName = order.WholesalerName;
            orderToUpdate.WholesalerAddress = order.WholesalerAddress;
            orderToUpdate.WholesalerPhone = order.WholesalerPhone;

            _context.Update(orderToUpdate);
            await _context.SaveChangesAsync();
            return orderToUpdate;
        }

        public async Task<ImportOrder> Delete(int? Id)
        {
            var orderToRemove = await _context.ImportOrders
                                .Include(io => io.ImportedProducts)
                                    .ThenInclude(ip => ip.Product)
                                .FirstOrDefaultAsync(o => o.ID == Id);
            if (orderToRemove != null)
            {
                _context.Remove(orderToRemove);
            }
            await _context.SaveChangesAsync();
            return orderToRemove;
        }
    }
}
