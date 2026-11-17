using AgriCompany.Web.Models;
using AgriCompany.Web.Repositories;

namespace AgriCompany.Web.Services
{
    public class HomepageService : IHomepageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly LocalImageMover _imageMover;

        public HomepageService(IUnitOfWork unitOfWork, IFileUploadService fileUploadService, LocalImageMover imageMover)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
            _imageMover = imageMover;
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
    
        public async Task<ImageMoveResult> MoveLocalImagesToCloudinaryAsync()
        {
            if (!_imageMover.CloudinaryEnabled)
                return ImageMoveResult.Disabled();

            var moved = 0;
            var failed = new List<string>();
            var slides = await _unitOfWork.HomepageSlides.FindAsync(s => s.ImagePath.StartsWith("/"));

            foreach (var slide in slides)
            {
                var (newPath, error) = await _imageMover.MoveAsync(slide.ImagePath, "slides");
                if (newPath == null)
                {
                    failed.Add($"{slide.TitleAr}: {error}");
                    continue;
                }

                slide.ImagePath = newPath;
                _unitOfWork.HomepageSlides.Update(slide);
                moved++;
            }

            if (moved > 0)
                await _unitOfWork.CompleteAsync();

            return new ImageMoveResult(true, moved, 0, failed);
        }
    }
}
