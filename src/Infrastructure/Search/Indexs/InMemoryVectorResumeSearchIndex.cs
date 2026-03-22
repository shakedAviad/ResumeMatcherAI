using Core.Interfaces;
using Domain.Models;
using Infrastructure.Search.Records;
using Microsoft.Extensions.VectorData;

namespace Infrastructure.Search.Indexs
{
    public class InMemoryVectorResumeSearchIndex : IResumeSearchIndex
    {
        private readonly VectorStoreCollection<string, ResumeVectorRecord> _collection;

        public InMemoryVectorResumeSearchIndex(VectorStoreCollection<string, ResumeVectorRecord> collection)
        {
            _collection = collection;
        }

        public async Task IndexAsync(ResumeDocument resumeDocument, CancellationToken cancellationToken = default)
        {
            await _collection.EnsureCollectionExistsAsync(cancellationToken);

            var record = new ResumeVectorRecord
            {
                CandidateId = resumeDocument.CandidateId,
                ResumeDocument = resumeDocument,
                FullName = resumeDocument.FullName,
                CurrentTitle = resumeDocument.CurrentTitle,
                SeniorityLevel = resumeDocument.SeniorityLevel,
                SearchableText = BuildSearchableText(resumeDocument)
            };

            await _collection.UpsertAsync(record, cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<ResumeDocument>> SearchAsync(JobSearchQuery jobSearchQuery, int maxResults = 10, CancellationToken cancellationToken = default)
        {
            await _collection.EnsureCollectionExistsAsync(cancellationToken);

            var searchText = BuildSearchText(jobSearchQuery);
            var results = new List<ResumeDocument>();

            await foreach (var result in _collection.SearchAsync(searchText, top: maxResults, cancellationToken: cancellationToken))
            {
                results.Add(result.Record.ResumeDocument);
            }

            return results;
        }

        private static string BuildSearchableText(ResumeDocument resumeDocument)
        {
            var parts = new List<string>
            {
                resumeDocument.FullName,
                resumeDocument.CurrentTitle,
                resumeDocument.SeniorityLevel,
                resumeDocument.ProfessionalSummary
            };

            parts.AddRange(resumeDocument.Skills);
            parts.AddRange(resumeDocument.Technologies);
            parts.AddRange(resumeDocument.Domains);
            parts.AddRange(resumeDocument.EducationItems);
            parts.AddRange(resumeDocument.SpokenLanguages);
            parts.AddRange(resumeDocument.PreferredLocations);

            foreach (var employmentRecord in resumeDocument.EmploymentHistory)
            {
                parts.Add(employmentRecord.CompanyName);
                parts.Add(employmentRecord.RoleTitle);
                parts.Add(employmentRecord.RoleSummary);
            }

            return string.Join(" ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static string BuildSearchText(JobSearchQuery jobSearchQuery)
        {
            var parts = new List<string>
            {
                jobSearchQuery.NormalizedRoleTitle,
                jobSearchQuery.SeniorityLevel,
                jobSearchQuery.Notes
            };

            parts.AddRange(jobSearchQuery.RequiredSkills);
            parts.AddRange(jobSearchQuery.PreferredSkills);
            parts.AddRange(jobSearchQuery.RequiredTechnologies);
            parts.AddRange(jobSearchQuery.PreferredTechnologies);
            parts.AddRange(jobSearchQuery.DomainKeywords);

            return string.Join(" ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));
        }
    }
}
