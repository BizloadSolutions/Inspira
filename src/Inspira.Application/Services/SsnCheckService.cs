using System;
using System.Threading.Tasks;
using Inspira.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Inspira.Domain.Entities;
using Inspira.Application.Services;

namespace Inspira.Application.Services;

public class SsnCheckService : ISsnCheckService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ISubmissionPropertyRepository _submissionPropertyRepository;
    private readonly IMtcSoapClient _mtcSoapClient;
    private readonly ILogger<SsnCheckService> _logger;

    public SsnCheckService(ISubmissionRepository submissionRepository,
        ISubmissionPropertyRepository submissionPropertyRepository,
        IMtcSoapClient mtcSoapClient,
        ILogger<SsnCheckService> logger)
    {
        _submissionRepository = submissionRepository;
        _submissionPropertyRepository = submissionPropertyRepository;
        _mtcSoapClient = mtcSoapClient;
        _logger = logger;
    }

    public async Task<string> SsnCheckAsync(int? submissionId, string ssn, string role)
    {
        try
        {
            Submission? submission = await _submissionRepository.GetByIdAsync(submissionId.Value);

            if (submission == null)
            {
                throw new ArgumentException($"Submission with ID {submissionId} not found.");
            }

            // Basic redaction check: consider redacted if length==4 or contains '•'
            var redactedTaxId = !string.IsNullOrEmpty(ssn) && (ssn.Length == 4 || ssn.Contains("•"));

            if (redactedTaxId)
            {
                var submissionProperty = await _submissionPropertyRepository.GetByIdAsync(submission.SubmissionPropertyId);

                if (submissionProperty == null)
                {
                    throw new ArgumentException($"SubmissionProperty with ID {submission.SubmissionPropertyId} not found.");
                }

                ssn = submissionProperty.OwnerTaxId;
            }

            var roleInt = string.Equals(role, "Owner", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

            // Call SOAP service
            var contactIdVal = await _mtcSoapClient.SsnInternalCheckAsync(ssn, roleInt);

            if (string.Equals(role, "Owner", StringComparison.OrdinalIgnoreCase))
            {
                // Persist owner contact id
                var submissionProperty = await _submissionPropertyRepository.GetByIdAsync(submission.SubmissionPropertyId);
                if (submissionProperty != null)
                {
                    submissionProperty.OwnerContactId = contactIdVal.ToString();
                    await _submissionPropertyRepository.UpdateAsync(submissionProperty);
                }
            }

            var result = new
            {
                result = contactIdVal != 0,
                contactId = contactIdVal
            };

            return JsonSerializer.Serialize(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SsnCheck failed");

            var result = new
            {
                result = "error",
                contactId = 0
            };

            return JsonSerializer.Serialize(result);
        }
    }
}
