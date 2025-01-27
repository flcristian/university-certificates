using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SkiaSharp;
using UniversityCertificates.Certificates.Repository.Interfaces;
using UniversityCertificates.Register.Models;
using UniversityCertificates.Register.Repository.Interfaces;
using UniversityCertificates.Register.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;
using UniversityCertificates.System.Utility.DTOs;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

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
        byte[] resultStream = ReplaceTextWithImage(documentTemplateBytes, qrCode, "[QR]");

        return resultStream;
    }

    private byte[] ReplaceTextWithImage(byte[] docxBytes, byte[] imageBytes, string replaceText)
    {
        using (MemoryStream docxStream = new MemoryStream())
        {
            docxStream.Write(docxBytes, 0, docxBytes.Length);

            using (WordprocessingDocument doc = WordprocessingDocument.Open(docxStream, true))
            {
                MainDocumentPart mainPart = doc.MainDocumentPart!;
                string relationshipId = AddImageToDocument(mainPart, imageBytes);

                // Find and replace [QR] text with image
                foreach (var text in mainPart.Document.Descendants<Text>())
                {
                    if (text.Text.Contains(replaceText))
                    {
                        // Create the image element
                        Drawing drawing = CreateImageElement(relationshipId, 200, 200); // Adjust size as needed

                        // Replace the paragraph containing [QR] with the image
                        Paragraph para = text.Ancestors<Paragraph>().FirstOrDefault()!;
                        if (para != null)
                        {
                            // Clear existing content
                            para.RemoveAllChildren();
                            // Add the image
                            Run run = new Run(drawing);
                            para.AppendChild(run);
                        }
                    }
                }

                doc.MainDocumentPart!.Document.Save();
            }

            return docxStream.ToArray();
        }
    }

    private static string AddImageToDocument(MainDocumentPart mainPart, byte[] imageBytes)
    {
        ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
        using (MemoryStream imageStream = new MemoryStream(imageBytes))
        {
            imagePart.FeedData(imageStream);
        }
        return mainPart.GetIdOfPart(imagePart);
    }

    private static Drawing CreateImageElement(string relationshipId, long width, long height)
    {
        long emuWidth = width * 9525; // Convert pixels to EMUs
        long emuHeight = height * 9525;

        var element = new Drawing(
            new DW.Inline(
                new DW.Extent() { Cx = emuWidth, Cy = emuHeight },
                new DW.EffectExtent()
                {
                    LeftEdge = 0L,
                    TopEdge = 0L,
                    RightEdge = 0L,
                    BottomEdge = 0L,
                },
                new DW.DocProperties() { Id = 1U, Name = "QR Code Image" },
                new DW.NonVisualGraphicFrameDrawingProperties(
                    new A.GraphicFrameLocks() { NoChangeAspect = true }
                ),
                new A.Graphic(
                    new A.GraphicData(
                        new PIC.Picture(
                            new PIC.NonVisualPictureProperties(
                                new PIC.NonVisualDrawingProperties() { Id = 0U, Name = "QR.jpg" },
                                new PIC.NonVisualPictureDrawingProperties()
                            ),
                            new PIC.BlipFill(
                                new A.Blip(
                                    new A.BlipExtensionList(
                                        new A.BlipExtension()
                                        {
                                            Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}",
                                        }
                                    )
                                )
                                {
                                    Embed = relationshipId,
                                    CompressionState = A.BlipCompressionValues.Print,
                                },
                                new A.Stretch(new A.FillRectangle())
                            ),
                            new PIC.ShapeProperties(
                                new A.Transform2D(
                                    new A.Offset() { X = 0L, Y = 0L },
                                    new A.Extents() { Cx = emuWidth, Cy = emuHeight }
                                ),
                                new A.PresetGeometry(new A.AdjustValueList())
                                {
                                    Preset = A.ShapeTypeValues.Rectangle,
                                }
                            )
                        )
                    )
                    {
                        Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture",
                    }
                )
            )
            {
                DistanceFromTop = 0U,
                DistanceFromBottom = 0U,
                DistanceFromLeft = 0U,
                DistanceFromRight = 0U,
            }
        );

        return element;
    }
}
