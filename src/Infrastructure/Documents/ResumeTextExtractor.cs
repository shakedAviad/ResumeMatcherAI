using Application.Interfaces;
using DocumentFormat.OpenXml.Packaging;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace Infrastructure.Documents
{
    public sealed class ResumeTextExtractor : IResumeTextExtractor
    {
        public Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => Task.FromResult(ExtractPdfText(filePath)),
                ".docx" => Task.FromResult(ExtractDocxText(filePath)),
                ".doc" => throw new NotSupportedException("Legacy .doc files are not supported. Please convert to .docx."),
                _ => throw new NotSupportedException($"Unsupported file type: {extension}")
            };
        }
        
        private static string ExtractDocxText(string filePath)
        {
            using var document = WordprocessingDocument.Open(filePath, false);
            var body = document.MainDocumentPart?.Document?.Body;

            return body?.InnerText?.Trim() ?? string.Empty;
        }
        private static string ExtractPdfText(string filePath)
        {
            throw new NotImplementedException("PDF text extraction is not implemented yet. Please use .docx format for now.");
        }
    }
}
