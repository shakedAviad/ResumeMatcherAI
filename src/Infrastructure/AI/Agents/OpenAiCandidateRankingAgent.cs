using Application.Interfaces;
using Domain.Models;
using Infrastructure.AI.Prompts;
using Microsoft.Agents.AI;

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

            var prompt = CandidateRankingPrompt.Build(jobSearchQuery, candidateResumes);
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
