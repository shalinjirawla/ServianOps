using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper _mapper;

        public JobService(
            IJobRepository jobRepository, 
            IJobAttachmentRepository attachmentRepository, 
            ISiteRepository siteRepository,
            ITradeRepository tradeRepository,
            IWebHostEnvironment env,
            IMapper mapper)
        {
            _jobRepository = jobRepository;
            _attachmentRepository = attachmentRepository;
            _siteRepository = siteRepository;
            _tradeRepository = tradeRepository;
            _env = env;
            _mapper = mapper;
        }

        public async Task<JobDetailDto> CreateAsync(CreateJobDto dto)
        {
            var site = await _siteRepository.GetByIdAsync(dto.SiteId);
            if (site == null || site.CustomerId != dto.CustomerId)
            {
                throw new Exception("Invalid Site for the selected Client.");
            }

            var trade = await _tradeRepository.GetByIdAsync(dto.TradeId);
            if (trade == null) throw new Exception("Trade not found.");

            try
            {
                var job = _mapper.Map<Job>(dto);
                job.JobNumber = GenerateJobNumber();
                job.Attachments = new List<JobAttachment>();

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
            var job = await _jobRepository.GetQueryable()
                .Include(j => j.Attachments)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) throw new Exception("Job not found.");

            var site = await _siteRepository.GetByIdAsync(dto.SiteId);
            if (site == null || site.CustomerId != dto.CustomerId)
            {
                throw new Exception("Invalid Site for the selected Client.");
            }

            try
            {
                _mapper.Map(dto, job);
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
            var job = await _jobRepository.GetQueryable()
                .Include(j => j.Customer)
                .Include(j => j.Site)
                .Include(j => j.Trade)
                .Include(j => j.Attachments)
                .FirstOrDefaultAsync(j => j.Id == id);

            return job == null ? null : _mapper.Map<JobDetailDto>(job);
        }

        public async Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<JobListDto>> GetAllPagedAsync(JobFilterDto filter)
        {
            var query = _jobRepository.GetQueryable()
                .Include(j => j.Customer)
                .Include(j => j.Site)
                .Include(j => j.Trade)
                .AsQueryable();

            if (filter.CustomerId.HasValue)
            {
                query = query.Where(j => j.CustomerId == filter.CustomerId.Value);
            }

            if (filter.SiteId.HasValue)
            {
                query = query.Where(j => j.SiteId == filter.SiteId.Value);
            }



            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(x => 
                    x.JobNumber.Contains(filter.Search) ||
                    x.Description.Contains(filter.Search)
                );
            }

            var totalCount = await query.CountAsync();
            var jobs = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<JobListDto>
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = _mapper.Map<List<JobListDto>>(jobs)
            };
        }

        public async Task DeleteAsync(long id)
        {
            var job = await _jobRepository.GetQueryable()
                .Include(j => j.Attachments)
                .FirstOrDefaultAsync(j => j.Id == id);

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

                uploadedPaths.Add($"Uploads/Jobs/{jobId}/{uniqueFileName}");
            }

            return uploadedPaths;
        }
    }
}
