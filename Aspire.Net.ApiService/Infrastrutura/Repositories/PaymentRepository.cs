using Aspire.Net.ApiService.Domain.Entities;
using Aspire.Net.ApiService.Domain.Interfaces;
using Aspire.Net.ApiService.Infrastrutura.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Aspire.Net.ApiService.Infrastrutura.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {

        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pagamento = await _context.Payments.FindAsync(id);
            if (pagamento == null) return false;

            _context.Payments.Remove(pagamento);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments.AsNoTracking().OrderByDescending(p => p.AtCreation).ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
