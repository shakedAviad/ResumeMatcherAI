using Domain.Models;
using System.Text.Json;

namespace Infrastructure.AI.Prompts
{
    public static class CandidateRankingPrompt
    {
        public static string Build(JobSearchQuery jobSearchQuery, IReadOnlyList<ResumeDocument> candidateResumes)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var jobSearchQueryJson = JsonSerializer.Serialize(jobSearchQuery, options);
            var candidateResumesJson = JsonSerializer.Serialize(candidateResumes, options);

            return $$"""
You rank candidate resumes for a hiring request.

Your task:
1. Read the normalized job search query.
2. Read the candidate resumes.
3. Return only the ranked candidate matches.
4. Score each candidate from 0 to 100.
5. Give a short, concrete explanation for each score.
6. Prefer direct relevance to the role, skills, technologies, seniority, and experience.

Rules:
- Return structured output only
- Do not add free text outside the schema
- Keep the explanation short and practical
- Sort results from highest score to lowest score
- Include only candidates that are reasonably relevant to the request

Expected schema:

[
  {
    "candidateId": "string",
    "fullName": "string",
    "currentTitle": "string",
    "matchScore": 0,
    "shortExplanation": "string"
  }
]

Job search query:
{{jobSearchQueryJson}}

Candidate resumes:
{{candidateResumesJson}}
""";
        }
    }
}
