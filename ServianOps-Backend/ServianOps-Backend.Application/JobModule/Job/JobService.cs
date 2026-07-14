using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.JobModule.Job.JobDto;
using ServianOps_Backend.Core.Entities.Jobs;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.JobModule.Job
{
    public class JobService : IJobService
    {
        private readonly IGenericRepository<Core.Entities.Jobs.Job> _jobRepository;
        private readonly IGenericRepository<JobAttachment> _attachmentRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public JobService(
            IGenericRepository<Core.Entities.Jobs.Job> jobRepository, 
            IGenericRepository<JobAttachment> attachmentRepository,
            IWebHostEnvironment env, 
            IMapper mapper)
        {
            _jobRepository = jobRepository;
            _attachmentRepository = attachmentRepository;
            _env = env;
            _mapper = mapper;
        }

        private async Task<string> UploadFileAsync(IFormFile file, string folder, long jobId)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "Uploads", folder, jobId.ToString());
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"Uploads/{folder}/{jobId}/{uniqueFileName}";
        }

        public async Task<StandardResponse<JobDetailDto>> CreateJob(CreateJobDto dto)
        {
            var job = _mapper.Map<Core.Entities.Jobs.Job>(dto);

            // Generate JobNumber (Placeholder logic)
            job.JobNumber = $"JOB-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";

            await _jobRepository.AddAsync(job); // Add first to get ID

            if (dto.Attachments != null && dto.Attachments.Count > 0)
            {
                job.Attachments = new List<JobAttachment>();
                foreach (var file in dto.Attachments)
                {
                    var fileUrl = await UploadFileAsync(file, "jobs", job.Id);
                    job.Attachments.Add(new JobAttachment { Link = fileUrl });
                }
                await _jobRepository.UpdateAsync(job);
            }

            var createdJob = await _jobRepository.GetQueryable()
                .Include(j => j.Customer)
                .Include(j => j.Site)
                .Include(j => j.Trade)
                .Include(j => j.Attachments)
                .FirstOrDefaultAsync(j => j.Id == job.Id);

            return StandardResponse<JobDetailDto>.Ok(_mapper.Map<JobDetailDto>(createdJob));
        }

        public async Task<StandardResponse<JobDetailDto>> UpdateJob(long id, UpdateJobDto dto)
        {
            var job = await _jobRepository.GetQueryable()
                .Include(j => j.Customer)
                .Include(j => j.Site)
                .Include(j => j.Trade)
                .Include(j => j.Attachments)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return StandardResponse<JobDetailDto>.Error("Job not found.");

            _mapper.Map(dto, job);

            if (dto.Attachments != null && dto.Attachments.Count > 0)
            {
                job.Attachments ??= new List<JobAttachment>();
                foreach (var file in dto.Attachments)
                {
                    var fileUrl = await UploadFileAsync(file, "jobs", job.Id);
                    job.Attachments.Add(new JobAttachment { Link = fileUrl });
                }
            }

            await _jobRepository.UpdateAsync(job);

            return StandardResponse<JobDetailDto>.Ok(_mapper.Map<JobDetailDto>(job));
        }

        public async Task<StandardResponse<JobDetailDto>> GetJobById(long id)
        {
            var job = await _jobRepository.GetQueryable()
                .Include(j => j.Customer)
                .Include(j => j.Site)
                .Include(j => j.Trade)
                .Include(j => j.Attachments)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return StandardResponse<JobDetailDto>.Error("Job not found.");
            
            return StandardResponse<JobDetailDto>.Ok(_mapper.Map<JobDetailDto>(job));
        }

        public async Task<StandardResponse<PagedResultDto<JobListDto>>> GetAllJobs(JobFilterDto filter)
        {
            var query = _jobRepository.GetQueryable()
                .Include(j => j.Customer)
                .Include(j => j.Site)
                .Include(j => j.Trade)
                .AsQueryable();

            if (filter.CustomerId.HasValue) query = query.Where(j => j.CustomerId == filter.CustomerId.Value);
            if (filter.SiteId.HasValue) query = query.Where(j => j.SiteId == filter.SiteId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(j => j.JobNumber.Contains(filter.Search) || j.Description.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PagedResultDto<JobListDto>(
                _mapper.Map<IReadOnlyList<JobListDto>>(items),
                totalCount,
                filter.PageNumber,
                filter.PageSize);

            return StandardResponse<PagedResultDto<JobListDto>>.Ok(result);
        }

        public async Task<StandardResponse<bool>> DeleteJob(long id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job != null)
            {
                await _jobRepository.DeleteAsync(job);
                return StandardResponse<bool>.Ok(true, "Job deleted.");
            }
            return StandardResponse<bool>.Error("Job not found.");
        }

        public async Task<StandardResponse<bool>> DeleteAttachment(long jobId, long attachmentId)
        {
            var attachment = await _attachmentRepository.GetQueryable()
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.Id == attachmentId);

            if (attachment != null)
            {
                var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", attachment.Link);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                await _attachmentRepository.DeleteAsync(attachment);
                return StandardResponse<bool>.Ok(true, "Attachment deleted.");
            }

            return StandardResponse<bool>.Error("Attachment not found.");
        }
    }
}
