using AgriCompany.Web.Models;
using AgriCompany.Web.Repositories;

namespace AgriCompany.Web.Services
{
    public class CompanyInfoService : ICompanyInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;

        public CompanyInfoService(IUnitOfWork unitOfWork, IFileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
        }

        public async Task<CompanyInformation> GetAsync()
        {
            var info = (await _unitOfWork.CompanyInformation.GetAllAsync()).FirstOrDefault();
            if (info == null)
            {
                info = new CompanyInformation
                {
                    CompanyNameAr = "اسم الشركة",
                    CompanyNameEn = "Company Name",
                    Phone = "00000000000",
                    WhatsAppNumber = "00000000000"
                };
                await _unitOfWork.CompanyInformation.AddAsync(info);
                await _unitOfWork.CompleteAsync();
            }
            return info;
        }

        //public async Task SaveAsync(CompanyInformation info, IFormFile? logo)
        //{
        //    var existing = (await _unitOfWork.CompanyInformation.GetAllAsync()).FirstOrDefault();

        //    if (logo != null && logo.Length > 0)
        //    {
        //        if (existing != null) _fileUploadService.DeleteImage(existing.LogoPath);
        //        info.LogoPath = await _fileUploadService.SaveImageAsync(logo, "uploads");
        //    }
        //    else if (existing != null)
        //    {
        //        info.LogoPath = existing.LogoPath;
        //    }

        //    info.UpdatedAt = DateTime.Now;

        //    if (existing == null)
        //    {
        //        await _unitOfWork.CompanyInformation.AddAsync(info);
        //    }
        //    else
        //    {
        //        info.Id = existing.Id;
        //        _unitOfWork.CompanyInformation.Update(info);
        //    }

        //    await _unitOfWork.CompleteAsync();
        //}
        public async Task SaveAsync(CompanyInformation info, IFormFile? logo = null)
        {
            var existing = (await _unitOfWork.CompanyInformation.GetAllAsync()).FirstOrDefault();

            if (existing == null)
            {
                // First time creation
                if (logo != null && logo.Length > 0)
                {
                    info.LogoPath = await _fileUploadService.SaveImageAsync(logo, "uploads");
                }
                await _unitOfWork.CompanyInformation.AddAsync(info);
            }
            else
            {
                // Update existing
                existing.CompanyNameAr = info.CompanyNameAr;
                existing.CompanyNameEn = info.CompanyNameEn;
                existing.AboutText = info.AboutText;
                existing.Vision = info.Vision;
                existing.Mission = info.Mission;
                existing.Address = info.Address;
                existing.Phone = info.Phone;
                existing.WhatsAppNumber = info.WhatsAppNumber;
                existing.Email = info.Email;
                existing.FacebookUrl = info.FacebookUrl;
                existing.InstagramUrl = info.InstagramUrl;
                existing.TwitterUrl = info.TwitterUrl;
                existing.YoutubeUrl = info.YoutubeUrl;
                existing.WorkingHours = info.WorkingHours;
                existing.GoogleMapsEmbedUrl = info.GoogleMapsEmbedUrl;
                existing.UpdatedAt = DateTime.Now;

                if (logo != null && logo.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existing.LogoPath))
                        _fileUploadService.DeleteImage(existing.LogoPath);

                    existing.LogoPath = await _fileUploadService.SaveImageAsync(logo, "uploads");
                }

                _unitOfWork.CompanyInformation.Update(existing);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
