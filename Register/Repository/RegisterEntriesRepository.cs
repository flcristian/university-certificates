using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UniversityCertificates.Data;
using UniversityCertificates.Register.DTOs;
using UniversityCertificates.Register.Models;
using UniversityCertificates.Register.Repository.Interfaces;

namespace UniversityCertificates.Register.Repository;

public class RegisterEntriesRepository : IRegisterEntriesRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RegisterEntriesRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RegisterEntry>> GetRegisterEntriesAsync()
    {
        return await _context
            .RegisterEntries.Include(re => re.Student)
            .Include(re => re.SelectedTemplate)
            .ToListAsync();
    }

    public async Task<RegisterEntry?> GetRegisterEntryByIdAsync(int id)
    {
        return await _context
            .RegisterEntries.Include(re => re.Student)
            .Include(re => re.SelectedTemplate)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RegisterEntry> AddRegisterEntryAsync(CreateRegisterEntryRequest request)
    {
        RegisterEntry registerEntry = _mapper.Map<RegisterEntry>(request);

        RegisterEntry addedRegistryEntry = _context.RegisterEntries.Add(registerEntry).Entity;
        await _context.SaveChangesAsync();
        return addedRegistryEntry;
    }

    public async Task<RegisterEntry> UpdateRegisterEntryAsync(UpdateRegisterEntryRequest request)
    {
        RegisterEntry registerEntry = (await _context.RegisterEntries.FindAsync(request.Id))!;

        registerEntry.StudentSerialNumber =
            request.StudentSerialNumber ?? registerEntry.StudentSerialNumber;
        registerEntry.DateOfIssue = request.DateOfIssue ?? registerEntry.DateOfIssue;
        registerEntry.Reason = request.Reason ?? registerEntry.Reason;
        registerEntry.Reviewed = request.Reviewed ?? registerEntry.Reviewed;
        registerEntry.Accepted = request.Accepted ?? registerEntry.Accepted;
        registerEntry.SelectedTemplateId =
            request.SelectedTemplate ?? registerEntry.SelectedTemplateId;

        RegisterEntry updatedRegisterEntry = _context.RegisterEntries.Update(registerEntry).Entity;
        await _context.SaveChangesAsync();
        return updatedRegisterEntry;
    }

    public async Task<RegisterEntry> DeleteRegisterEntryAsync(int id)
    {
        RegisterEntry registerEntry = (await _context.RegisterEntries.FindAsync(id))!;
        _context.RegisterEntries.Remove(registerEntry);
        await _context.SaveChangesAsync();
        return registerEntry;
    }
}
