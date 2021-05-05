using System.Threading.Tasks;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;

namespace FarmHub.Application.Services.Repositories
{
    public interface IOrderService:IGenericRepository<Order>
    {
        Task UpdatePaymentStatusAsync(int requestOrderId, PaymentStatus paymentStatus, string paymentData, string pspReference, string refusalReason);
        Task<bool> ConfirmOrderById(int orderId);
    }
}
