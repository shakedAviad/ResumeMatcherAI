using Core.Interfaces;
using Domain.Models;
using Microsoft.Agents.AI;
using System.Text.Json;

namespace Infrastructure.AI.Agents
{
    public class OpenAiCandidateRankingAgent : ICandidateRankingAgent
    {
        private readonly ChatClientAgent _agent;

        public OpenAiCandidateRankingAgent(ChatClientAgent agent)
        {
            _agent = agent;
        }

        public async Task<IReadOnlyList<CandidateMatch>> RankAsync(JobSearchQuery jobSearchQuery, IReadOnlyList<ResumeDocument> candidateResumes, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(jobSearchQuery);
            ArgumentNullException.ThrowIfNull(candidateResumes);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var jobSearchQueryJson = JsonSerializer.Serialize(jobSearchQuery, options);
            var candidateResumesJson = JsonSerializer.Serialize(candidateResumes, options);

            var prompt = $"Job search query: {jobSearchQueryJson}, Candidate resumes:  {candidateResumesJson}";
            var response = await _agent.RunAsync<List<CandidateMatch>>(prompt, cancellationToken: cancellationToken);
            var rankedCandidates = response.Result;

            return rankedCandidates switch
            {
                List<CandidateMatch> => rankedCandidates
                .Where(candidate => candidate is not null)
                .OrderByDescending(candidate => candidate.MatchScore)
                .ToList(),
                _ => throw new InvalidOperationException("LLM returned an invalid candidate matches.")
            };

        }
    }
}
