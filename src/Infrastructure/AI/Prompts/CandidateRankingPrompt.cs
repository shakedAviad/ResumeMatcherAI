namespace Infrastructure.AI.Prompts
{
    public class CandidateRankingPrompt
    {
        public const string Instructions = $$"""
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

""";

    }
}
