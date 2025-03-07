using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using UniversityCertificates.System.Constants;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace UniversityCertificates.System.Utility.Services;

public static class DocxFileHandler
{
    public static byte[] ReplaceText(byte[] docxBytes, string searchText, string replacementText)
    {
        using (MemoryStream docxStream = new MemoryStream())
        {
            docxStream.Write(docxBytes, 0, docxBytes.Length);

            using (WordprocessingDocument doc = WordprocessingDocument.Open(docxStream, true))
            {
                MainDocumentPart mainPart = doc.MainDocumentPart!;

                var paragraphs = mainPart.Document.Descendants<Paragraph>().ToList();

                foreach (var paragraph in paragraphs)
                {
                    string paragraphText = paragraph.InnerText;

                    if (paragraphText.Contains(searchText))
                    {
                        var originalRuns = paragraph.Elements<Run>().ToList();
                        var firstRunProperties = originalRuns
                            .FirstOrDefault()
                            ?.RunProperties?.CloneNode(true);

                        string newText = paragraphText.Replace(searchText, replacementText);

                        var newRun = new Run();
                        if (firstRunProperties != null)
                        {
                            newRun.AppendChild(firstRunProperties);
                        }
                        newRun.AppendChild(new Text(newText));

                        paragraph.RemoveAllChildren<Run>();
                        paragraph.AppendChild(newRun);
                    }
                }

                mainPart.Document.Save();
            }

            return docxStream.ToArray();
        }
    }

    public static byte[] ReplaceTextWithImage(
        byte[] docxBytes,
        byte[] imageBytes,
        string searchText
    )
    {
        using (MemoryStream docxStream = new MemoryStream())
        {
            docxStream.Write(docxBytes, 0, docxBytes.Length);

            using (WordprocessingDocument doc = WordprocessingDocument.Open(docxStream, true))
            {
                MainDocumentPart mainPart = doc.MainDocumentPart!;
                string relationshipId = AddImageToDocument(mainPart, imageBytes);

                foreach (var text in mainPart.Document.Descendants<Text>())
                {
                    if (text.Text.Contains(searchText))
                    {
                        Drawing drawing = CreateImageElement(
                            relationshipId,
                            ConstantValues.QR_CODE_SIZE,
                            ConstantValues.QR_CODE_SIZE
                        );

                        Paragraph para = text.Ancestors<Paragraph>().FirstOrDefault()!;
                        if (para != null)
                        {
                            para.RemoveAllChildren();
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
