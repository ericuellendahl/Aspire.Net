using Aspire.Net.ApiService.Domain.DTOs;
using Aspire.Net.ApiService.Domain.Entities;
using Aspire.Net.ApiService.Domain.Enums;
using Aspire.Net.ApiService.Domain.Interfaces;
using Aspire.Net.ApiService.Infrastrutura.Brokers;
using System.Text.Json;

namespace Aspire.Net.ApiService.Application.Services
{
    public class PaymentService(IPaymentRepository paymentRepository, PagamentoProducerMQ pagamentoProducer) : IPaymentService
    {

        private readonly IPaymentRepository _paymentRepository = paymentRepository;
        private readonly PagamentoProducerMQ _pagamentoProducer = pagamentoProducer;

        public async Task<PaymentResponseDto> CreateAsync(PaymentRequestDto paymentRequestDto)
        {
            var entitie = new Payment(paymentRequestDto.ProductId, (TypePayment)paymentRequestDto.TypePayment, paymentRequestDto.Amount, paymentRequestDto.Details);

            var createPayment = await _paymentRepository.CreateAsync(entitie);

            if (createPayment is not null)
            {
                var json = JsonSerializer.Serialize(entitie);
                _pagamentoProducer.EnviarMensagem(json);
            }

            return MapearParaDto(createPayment);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetAllAsync()
        {
            var getPayments = await _paymentRepository.GetAllAsync();

            return getPayments.Select(MapearParaDto);
        }

        public async Task<PaymentResponseDto?> GetByIdAsync(int id)
        {
            var getPaymentId = await _paymentRepository.GetByIdAsync(id);

            return getPaymentId is null ? null : MapearParaDto(getPaymentId);
        }

        private static PaymentResponseDto MapearParaDto(Payment? payment)
        {
            return new PaymentResponseDto
            {
                Id = payment?.Id,
                ProductId = payment?.ProductId,
                TypePayment = Convert.ToInt32(payment?.TypePayment),
                Amount = payment?.Amount,
                Details = payment?.Details,
                AtCreation = payment?.AtCreation,
                Status = payment?.Status
            };
        }
    }
}
