using Novacode;
using SkiaSharp;
using UniversityCertificates.Certificates.Repository.Interfaces;
using UniversityCertificates.Register.Models;
using UniversityCertificates.Register.Repository.Interfaces;
using UniversityCertificates.Register.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;
using UniversityCertificates.System.Utility.DTOs;

namespace UniversityCertificates.Register.Services;

public class RegisterEntryDocumentsService : IRegisterEntryDocumentsService
{
    private readonly IRegisterEntriesRepository _registerEntriesRepository;
    private readonly ICertificateTemplateFilesRepository _certificateTemplateFilesRepository;

    public RegisterEntryDocumentsService(
        IRegisterEntriesRepository registerEntriesRepository,
        ICertificateTemplateFilesRepository certificateTemplateFilesRepository
    )
    {
        _registerEntriesRepository = registerEntriesRepository;
        _certificateTemplateFilesRepository = certificateTemplateFilesRepository;
    }

    public async Task<byte[]> GenerateCertificateForRegisterEntryByIdAsync(int id, byte[] qrCode)
    {
        RegisterEntry? registerEntry = await _registerEntriesRepository.GetRegisterEntryByIdAsync(
            id
        );

        if (registerEntry == null)
        {
            throw new ItemDoesNotExistException(Constants.REGISTER_ENTRY_DOES_NOT_EXIST);
        }

        GetFileRequest? fileRequest =
            await _certificateTemplateFilesRepository.GetCertificateTemplateFileByIdAsync(
                registerEntry.SelectedTemplateId!.Value
            );

        if (fileRequest == null)
        {
            throw new ItemDoesNotExistException(Constants.CERTIFICATE_TEMPLATE_DOES_NOT_EXIST);
        }

        byte[] documentTemplateBytes = fileRequest.FileContents;
        using var templateStream = new MemoryStream(documentTemplateBytes);
        using var resultStream = new MemoryStream();

        // Load document from template bytes
        using (var document = DocX.Load(templateStream))
        {
            var qrParagraph = document.Paragraphs.FirstOrDefault(p => p.Text.Contains("[QR]"));
            if (qrParagraph != null)
            {
                qrParagraph.RemoveText(0);

                using (var qrStream = new MemoryStream(qrCode))
                using (var skBitmap = SKBitmap.Decode(qrStream))
                {
                    var imageInfo = new SKImageInfo(128, 128);
                    var samplingOptions = new SKSamplingOptions(
                        SKFilterMode.Linear,
                        SKMipmapMode.None
                    );

                    using (var resizedBitmap = skBitmap.Resize(imageInfo, samplingOptions))
                    using (var resizedImage = SKImage.FromBitmap(resizedBitmap))
                    using (var memStream = new MemoryStream())
                    {
                        resizedImage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(memStream);
                        memStream.Position = 0;

                        var image = document.AddImage(memStream);
                        var picture = image.CreatePicture(128, 128);
                        qrParagraph.InsertPicture(picture);
                    }
                }
            }

            document.SaveAs(resultStream);
        }

        return resultStream.ToArray();
    }
}
