using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ServianOps_Backend.Application.DTOs.Jobs;
using ServianOps_Backend.Application.DTOs.Shared;
using ServianOps_Backend.Application.Interfaces.Jobs;
using ServianOps_Backend.Core.Entities.Jobs;
using ServianOps_Backend.Core.Interfaces.Repositories.Crm;
using ServianOps_Backend.Core.Interfaces.Repositories.Jobs;

namespace ServianOps_Backend.Application.Services.Jobs
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IJobAttachmentRepository _attachmentRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ITradeRepository _tradeRepository;
        private readonly IWebHostEnvironment _env;

        public JobService(
            IJobRepository jobRepository, 
            IJobAttachmentRepository attachmentRepository, 
            ISiteRepository siteRepository,
            ITradeRepository tradeRepository,
            IWebHostEnvironment env)
        {
            _jobRepository = jobRepository;
            _attachmentRepository = attachmentRepository;
            _siteRepository = siteRepository;
            _tradeRepository = tradeRepository;
            _env = env;
        }

        public async Task<JobDetailDto> CreateAsync(CreateJobDto dto)
        {
            // Validate Site belongs to Customer
            var site = await _siteRepository.GetByIdAsync(dto.SiteId);
            if (site == null || site.CustomerId != dto.CustomerId)
            {
                throw new Exception("Invalid Site for the selected Client.");
            }

            var trade = await _tradeRepository.GetByIdAsync(dto.TradeId);
            if (trade == null) throw new Exception("Trade not found.");

            try
            {
                var job = new Job
                {
                    JobNumber = GenerateJobNumber(),
                    CustomerId = dto.CustomerId,
                    SiteId = dto.SiteId,
                    TradeId = dto.TradeId,
                    Description = dto.Description,
                    Priority = dto.Priority,
                    PONumber = dto.PONumber,
                    Budget = dto.Budget,
                    NTE = dto.NTE,
                    Attachments = new List<JobAttachment>()
                };

                await _jobRepository.AddAsync(job);

                if (dto.Attachments != null && dto.Attachments.Count > 0)
                {
                    var uploadedFiles = await UploadFilesAsync(job.Id, dto.Attachments);
                    foreach (var file in uploadedFiles)
                    {
                        var attachment = new JobAttachment
                        {
                            JobId = job.Id,
                            Link = file
                        };
                        await _attachmentRepository.AddAsync(attachment);
                    }
                }

                return await GetByIdAsync(job.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<JobDetailDto> UpdateAsync(long id, UpdateJobDto dto)
        {
            var job = await _jobRepository.GetJobWithDetailsAsync(id);
            if (job == null) throw new Exception("Job not found.");

            // Validate Site
            var site = await _siteRepository.GetByIdAsync(dto.SiteId);
            if (site == null || site.CustomerId != dto.CustomerId)
            {
                throw new Exception("Invalid Site for the selected Client.");
            }

            try
            {
                job.CustomerId = dto.CustomerId;
                job.SiteId = dto.SiteId;
                job.TradeId = dto.TradeId;
                job.Description = dto.Description;
                job.Priority = dto.Priority;
                job.PONumber = dto.PONumber;
                job.Budget = dto.Budget;
                job.NTE = dto.NTE;

                await _jobRepository.UpdateAsync(job);

                if (dto.Attachments != null && dto.Attachments.Count > 0)
                {
                    var uploadedFiles = await UploadFilesAsync(job.Id, dto.Attachments);
                    foreach (var file in uploadedFiles)
                    {
                        await _attachmentRepository.AddAsync(new JobAttachment
                        {
                            JobId = job.Id,
                            Link = file
                        });
                    }
                }

                return await GetByIdAsync(job.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<JobDetailDto> GetByIdAsync(long id)
        {
            var job = await _jobRepository.GetJobWithDetailsAsync(id);
            if (job == null) return null;

            return new JobDetailDto
            {
                Id = job.Id,
                JobNumber = job.JobNumber,
                CustomerId = job.CustomerId,
                Customer = job.Customer != null ? new CustomerSummaryDto 
                { 
                    Id = job.Customer.Id, 
                    Name = job.Customer.Name, 
                    CompanyName = job.Customer.CompanyName, 
                    MobileNumber = job.Customer.MobileNumber 
                } : null,
                SiteId = job.SiteId,
                Site = job.Site != null ? new SiteSummaryDto 
                { 
                    Id = job.Site.Id, 
                    SiteName = job.Site.SiteName, 
                    City = job.Site.City, 
                    PostCode = job.Site.PostCode 
                } : null,
                TradeId = job.TradeId,
                Trade = job.Trade != null ? new TradeDto 
                { 
                    Id = job.Trade.Id, 
                    Name = job.Trade.Name 
                } : null,
                Description = job.Description,
                Priority = job.Priority,
                PONumber = job.PONumber,
                Budget = job.Budget,
                NTE = job.NTE,
                CreationTime = job.CreationTime,
                IsActive = job.IsActive,
                Attachments = job.Attachments?.Where(x => !x.IsDeleted).Select(a => new JobAttachmentDto
                {
                    Id = a.Id,
                    Link = a.Link
                }).ToList() ?? new List<JobAttachmentDto>()
            };
        }

        public async Task<IReadOnlyList<JobListDto>> GetAllPagedAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var jobs = await _jobRepository.GetPagedJobsWithDetailsAsync(pageNumber, pageSize);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                jobs = jobs.Where(x => 
                    x.JobNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    x.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return jobs.Select(job => new JobListDto
            {
                Id = job.Id,
                JobNumber = job.JobNumber,
                Customer = job.Customer != null ? new CustomerSummaryDto 
                { 
                    Id = job.Customer.Id, 
                    Name = job.Customer.Name, 
                    CompanyName = job.Customer.CompanyName, 
                    MobileNumber = job.Customer.MobileNumber 
                } : null,
                Site = job.Site != null ? new SiteSummaryDto 
                { 
                    Id = job.Site.Id, 
                    SiteName = job.Site.SiteName, 
                    City = job.Site.City, 
                    PostCode = job.Site.PostCode 
                } : null,
                Trade = job.Trade != null ? new TradeDto 
                { 
                    Id = job.Trade.Id, 
                    Name = job.Trade.Name 
                } : null,
                Priority = job.Priority,
                CreationTime = job.CreationTime
            }).ToList();
        }

        public async Task DeleteAsync(long id)
        {
            var job = await _jobRepository.GetJobWithDetailsAsync(id);
            if (job != null)
            {
                foreach (var attachment in job.Attachments)
                {
                    await _attachmentRepository.DeleteAsync(attachment);
                }
                await _jobRepository.DeleteAsync(job);
            }
        }

        public async Task DeleteAttachmentAsync(long jobId, long attachmentId)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (attachment == null || attachment.JobId != jobId)
            {
                throw new Exception("Attachment not found.");
            }

            // Optional: Physically delete file here. Soft delete in DB.
            var filePath = Path.Combine(_env.WebRootPath, attachment.Link);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            await _attachmentRepository.DeleteAsync(attachment);
        }

        private string GenerateJobNumber()
        {
            return $"JOB-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }

        private async Task<List<string>> UploadFilesAsync(long jobId, IFormFileCollection files)
        {
            var uploadedPaths = new List<string>();
            var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", "Jobs", jobId.ToString());

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var file in files)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative path to store in DB
                uploadedPaths.Add($"Uploads/Jobs/{jobId}/{uniqueFileName}");
            }

            return uploadedPaths;
        }
    }
}
