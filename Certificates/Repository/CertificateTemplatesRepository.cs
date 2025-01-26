using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UniversityCertificates.Certificates.DTOs;
using UniversityCertificates.Certificates.Models;
using UniversityCertificates.Certificates.Repository.Interfaces;
using UniversityCertificates.Data;

namespace UniversityCertificates.Certificates.Repository;

public class CertificateTemplatesRepository : ICertificateTemplatesRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CertificateTemplatesRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CertificateTemplate>> GetAllCertificateTemplatesAsync()
    {
        return await _context.CertificateTemplates.Include(ct => ct.RegisterEntries).ToListAsync();
    }

    public async Task<CertificateTemplate?> GetCertificateTemplateByIdAsync(int id)
    {
        return await _context
            .CertificateTemplates.Include(ct => ct.RegisterEntries)
            .FirstOrDefaultAsync(ct => ct.Id == id);
    }

    public async Task<CertificateTemplate> AddCertificateTemplateAsync(
        CreateCertificateTemplateRequest request
    )
    {
        CertificateTemplate template = _mapper.Map<CertificateTemplate>(request);

        CertificateTemplate addedTemplate = _context.CertificateTemplates.Add(template).Entity;
        await _context.SaveChangesAsync();
        return addedTemplate;
    }

    public async Task<CertificateTemplate> UpdateCertificateTemplateAsync(
        UpdateCertificateTemplateRequest request
    )
    {
        CertificateTemplate template = (await _context.CertificateTemplates.FindAsync(request.Id))!;

        template.Name = request.Name ?? template.Name;
        template.Description = request.Description ?? template.Description;
        template.Active = request.Active ?? template.Active;

        CertificateTemplate updatedTemplate = _context.CertificateTemplates.Update(template).Entity;
        await _context.SaveChangesAsync();
        return updatedTemplate;
    }

    public async Task<CertificateTemplate> DeleteCertificateTemplateAsync(int id)
    {
        CertificateTemplate template = (await _context.CertificateTemplates.FindAsync(id))!;

        _context.CertificateTemplates.Remove(template);
        await _context.SaveChangesAsync();
        return template;
    }

    public async Task<CertificateTemplate> SoftDeleteCertificateTemplateAsync(int id)
    {
        CertificateTemplate template = (await _context.CertificateTemplates.FindAsync(id))!;

        template.Active = false;
        CertificateTemplate softDeletedTemplate = _context
            .CertificateTemplates.Update(template)
            .Entity;
        await _context.SaveChangesAsync();
        return softDeletedTemplate;
    }
}
