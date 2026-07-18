using AgriCompany.Web.Models;
using AgriCompany.Web.Repositories;

namespace AgriCompany.Web.Services
{
    public class FaqService : IFaqService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FaqService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<FAQ>> GetActiveAsync() =>
            (await _unitOfWork.FAQs.FindAsync(f => f.IsActive)).OrderBy(f => f.DisplayOrder).ToList();

        public async Task<List<FAQ>> GetAllAsync() =>
            (await _unitOfWork.FAQs.GetAllAsync()).OrderBy(f => f.DisplayOrder).ToList();

        public async Task<FAQ?> GetByIdAsync(int id) => await _unitOfWork.FAQs.GetByIdAsync(id);

        public async Task CreateAsync(FAQ faq)
        {
            await _unitOfWork.FAQs.AddAsync(faq);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> UpdateAsync(FAQ faq)
        {
            var existing = await _unitOfWork.FAQs.GetByIdAsync(faq.Id);
            if (existing == null) return false;

            existing.QuestionAr = faq.QuestionAr;
            existing.AnswerAr = faq.AnswerAr;
            existing.IsActive = faq.IsActive;
            existing.DisplayOrder = faq.DisplayOrder;

            _unitOfWork.FAQs.Update(existing);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var faq = await _unitOfWork.FAQs.GetByIdAsync(id);
            if (faq == null) return false;

            _unitOfWork.FAQs.Remove(faq);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
