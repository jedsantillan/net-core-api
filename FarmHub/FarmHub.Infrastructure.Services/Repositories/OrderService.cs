using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace FarmHub.Domain.Services.Repositories
{
    public class OrderService : GenericRepository<Order, CatalogDbContext>, IOrderService
    {
        private ILogger<Order> _logger;
        private CatalogDbContext _dbContext;

        public OrderService(ILogger<Order> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public new async Task InsertAsync(Order order)
        {
            DateTime currentDate = DateTime.UtcNow;

            order.CreatedDate = currentDate;
            order.ModifiedDate = currentDate;

            foreach (OrderItem item in order.OrderItems)
            {
                item.CreatedDate = currentDate;
                item.ModifiedDate = currentDate;
            }

            await _dbContext.AddAsync(order);
            await _dbContext.SaveChangesAsync();
        }

        public new async Task Update(Order order)
        {
            DateTime currentDate = DateTime.UtcNow;

            order.ModifiedDate = currentDate;

            foreach (OrderItem item in order.OrderItems)
            {
                item.ModifiedDate = currentDate;
            }

            _dbContext.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public new async Task<List<Order>> GetAllListAsync()
        {
            var orderList = await _dbContext.Orders
                .Include(o => o.Discount)
                .Include(o => o.HarvestPeriod)
                .Include(o => o.ShippingAddress)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Include(o => o.CardPayments)
                .ToListAsync();

            return orderList;
        }

        public new async Task<Order> GetByIdAsync(int id)
        {
            var order = await _dbContext.Orders
                .Include(c => c.Discount)
                .Include(c => c.HarvestPeriod)
                .Include(c => c.ShippingAddress)
                .Include(c => c.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.ProductPortion)
                    .ThenInclude(pp => pp.Portion)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.ProductPortion)
                    .ThenInclude(pp => pp.Product)
                    .ThenInclude(p => p.UnitOfMeasure)
                .Include(o => o.CardPayments)
                .FirstOrDefaultAsync(s => s.Id == id);

            return order;
        }

        public async Task UpdatePaymentStatusAsync(int requestOrderId, PaymentStatus paymentStatus, string paymentData,
            string pspReference,
            string refusalReason)
        {
            var order = await GetByIdAsync(requestOrderId);

            if (paymentData != null)
            {
                var existingPayment = order.CardPayments.FirstOrDefault(p => p.PaymentReference == pspReference);
                if (existingPayment == null)
                {
                    var payment = new CardPayment
                    {
                        Status = paymentStatus,
                        PaymentData = paymentData,
                        PaymentReference = pspReference,
                        RefusalReason = refusalReason,
                    };
                    order.CardPayments.Add(payment);
                }
                else
                {
                    existingPayment.Status = paymentStatus;
                    existingPayment.RefusalReason = "";
                    _dbContext.CardPayments.Update(existingPayment);
                }
            }

            _dbContext.SaveChanges();
        }

        public async Task<bool> ConfirmOrderById(int orderId)
        {
            throw new System.NotImplementedException();
        }
    }
}