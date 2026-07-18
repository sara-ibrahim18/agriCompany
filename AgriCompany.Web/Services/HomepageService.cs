using AgriCompany.Web.Models;
using AgriCompany.Web.Repositories;

namespace AgriCompany.Web.Services
{
    public class HomepageService : IHomepageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;

        public HomepageService(IUnitOfWork unitOfWork, IFileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
        }

        public async Task<List<HomepageSlide>> GetActiveSlidesAsync() =>
            (await _unitOfWork.HomepageSlides.FindAsync(s => s.IsActive)).OrderBy(s => s.DisplayOrder).ToList();

        public async Task<List<HomepageSlide>> GetAllSlidesAsync() =>
            (await _unitOfWork.HomepageSlides.GetAllAsync()).OrderBy(s => s.DisplayOrder).ToList();

        public async Task<HomepageSlide?> GetByIdAsync(int id) => await _unitOfWork.HomepageSlides.GetByIdAsync(id);

        public async Task CreateAsync(HomepageSlide slide, IFormFile? image)
        {
            if (image != null && image.Length > 0)
                slide.ImagePath = await _fileUploadService.SaveImageAsync(image, "slides");

            await _unitOfWork.HomepageSlides.AddAsync(slide);
            await _unitOfWork.CompleteAsync();
        }

        //public async Task<bool> UpdateAsync(HomepageSlide slide, IFormFile? image)
        //{
        //    var existing = await _unitOfWork.HomepageSlides.GetByIdAsync(slide.Id);
        //    if (existing == null) return false;

        //    existing.TitleAr = slide.TitleAr;
        //    existing.SubtitleAr = slide.SubtitleAr;
        //    existing.ButtonText = slide.ButtonText;
        //    existing.ButtonUrl = slide.ButtonUrl;
        //    existing.IsActive = slide.IsActive;
        //    existing.DisplayOrder = slide.DisplayOrder;

        //    if (image != null && image.Length > 0)
        //    {
        //        _fileUploadService.DeleteImage(existing.ImagePath);
        //        existing.ImagePath = await _fileUploadService.SaveImageAsync(image, "slides");
        //    }

        //    _unitOfWork.HomepageSlides.Update(existing);
        //    await _unitOfWork.CompleteAsync();
        //    return true;
        //}
        public async Task<bool> UpdateAsync(HomepageSlide slide, IFormFile? image)
{
    var existing = await _unitOfWork.HomepageSlides.GetByIdAsync(slide.Id);
    if (existing == null) return false;

    // Update properties
    existing.TitleAr = slide.TitleAr;
    //existing.TitleEn = slide.TitleEn ?? existing.TitleEn;
    existing.SubtitleAr = slide.SubtitleAr;
    //existing.SubtitleEn = slide.SubtitleEn ?? existing.SubtitleEn;
    existing.ButtonText = slide.ButtonText;
    existing.ButtonUrl = slide.ButtonUrl;
    existing.DisplayOrder = slide.DisplayOrder;
    existing.IsActive = slide.IsActive;

    if (image != null && image.Length > 0)
    {
        if (!string.IsNullOrEmpty(existing.ImagePath))
            _fileUploadService.DeleteImage(existing.ImagePath);

        existing.ImagePath = await _fileUploadService.SaveImageAsync(image, "slides");
    }

    _unitOfWork.HomepageSlides.Update(existing);
    await _unitOfWork.CompleteAsync();
    return true;
}


        public async Task<bool> DeleteAsync(int id)
        {
            var slide = await _unitOfWork.HomepageSlides.GetByIdAsync(id);
            if (slide == null) return false;

            _fileUploadService.DeleteImage(slide.ImagePath);
            _unitOfWork.HomepageSlides.Remove(slide);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
