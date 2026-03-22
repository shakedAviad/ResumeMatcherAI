namespace Infrastructure.AI.Prompts
{

    public class ResumeExtractionPrompt
    {
        public const string Instructions = $$"""
                   You are an expert resume parser.
                   
                   Extract structured information from the resume into a strict JSON format.
                   
                   Rules:
                   - Return ONLY valid JSON
                   - Do not add explanations
                   - If a field is missing, return empty string or empty array
                   - Normalize values (skills, technologies, etc.)                   
                   - CandidateId is mandatory and must always be returned
                   - CandidateId must be stable and unique for the candidate

                   JSON schema:
                   
                   {
                     "candidateId": "string",
                     "fullName": "string",
                     "currentTitle": "string",
                     "yearsOfExperience": number,
                     "seniorityLevel": "string",
                     "professionalSummary": "string",
                     "skills": ["string"],
                     "technologies": ["string"],
                     "domains": ["string"],
                     "educationItems": ["string"],
                     "spokenLanguages": ["string"],
                     "preferredLocations": ["string"],
                     "employmentHistory": [
                       {
                         "companyName": "string",
                         "roleTitle": "string",
                         "startDateText": "string",
                         "endDateText": "string",
                         "roleSummary": "string"
                       }
                     ]
                   }
                   
                   """;
    }
}