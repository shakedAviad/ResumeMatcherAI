using Domain.Models;
using Microsoft.Extensions.VectorData;

namespace Infrastructure.Search.Records
{
    public class ResumeVectorRecord
    {
        [VectorStoreKey]
        public string CandidateId { get; set; } = string.Empty;

        [VectorStoreData]
        public ResumeDocument ResumeDocument { get; set; } = new();

        [VectorStoreData]
        public string FullName { get; set; } = string.Empty;

        [VectorStoreData]
        public string CurrentTitle { get; set; } = string.Empty;

        [VectorStoreData]
        public string SeniorityLevel { get; set; } = string.Empty;

        [VectorStoreData]
        public string SearchableText { get; set; } = string.Empty;

        [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Flat)]
        public string SearchableTextEmbedding => SearchableText;

        public static string CollectionName => "resume-records";
    }
}
