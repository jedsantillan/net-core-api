using System.Threading.Tasks;

namespace FarmHub.Application.Services.Services.Interface
{
    public interface IConfirmationEmailService<in T>
    {
        Task<int> SendConfirmationEmail(T obj);
    }
}