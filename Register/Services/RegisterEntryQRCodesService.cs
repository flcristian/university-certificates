using System.Text.Json;
using SkiaSharp;
using UniversityCertificates.Register.Models;
using UniversityCertificates.Register.Repository.Interfaces;
using UniversityCertificates.Register.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;
using UniversityCertificates.System.Utility.Models;
using ZXing;
using ZXing.Rendering;

namespace UniversityCertificates.Register.Services;

public class RegisterEntryQRCodesService : IRegisterEntryQRCodesService
{
    private readonly IRegisterEntriesRepository _registerEntriesRepository;
    private const int QR_CODE_SIZE = 256;

    public RegisterEntryQRCodesService(IRegisterEntriesRepository registerEntriesRepository)
    {
        _registerEntriesRepository = registerEntriesRepository;
    }

    public async Task<byte[]> GenerateQRCodeForRegisterEntryByIdAsync(int id)
    {
        RegisterEntry? registerEntry = await _registerEntriesRepository.GetRegisterEntryByIdAsync(
            id
        );

        if (registerEntry == null)
        {
            throw new ItemDoesNotExistException(Constants.REGISTER_ENTRY_DOES_NOT_EXIST);
        }

        QRCodeData qrCodeData = new()
        {
            SerialNumber = registerEntry.Student.SerialNumber,
            FirstName = registerEntry.Student.FirstName,
            LastName = registerEntry.Student.LastName,
            StudyYear = registerEntry.Student.StudyYear,
            DegreeType = registerEntry.Student.DegreeType.ToString(),
            Department = registerEntry.Student.Department,
            DateOfIssue = registerEntry.DateOfIssue.ToString("dd.MM.yyyy"),
            Reason = registerEntry.Reason,
        };

        return GenerateQRCode(JsonSerializer.Serialize(qrCodeData));
    }

    private byte[] GenerateQRCode(string qrDataJson)
    {
        BarcodeWriterPixelData writer = new()
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Height = QR_CODE_SIZE,
                Width = QR_CODE_SIZE,
                Margin = 1,
            },
        };

        PixelData pixelData = writer.Write(qrDataJson);

        using (SKBitmap bitmap = new SKBitmap(pixelData.Width, pixelData.Height))
        {
            for (int y = 0; y < pixelData.Height; y++)
            {
                for (int x = 0; x < pixelData.Width; x++)
                {
                    int index = y * pixelData.Width * 4 + x * 4;
                    bitmap.SetPixel(
                        x,
                        y,
                        new SKColor(
                            pixelData.Pixels[index + 2],
                            pixelData.Pixels[index + 1],
                            pixelData.Pixels[index],
                            pixelData.Pixels[index + 3]
                        )
                    );
                }
            }

            using (SKImage image = SKImage.FromBitmap(bitmap))
            using (SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                return encoded.ToArray();
            }
        }
    }
}
