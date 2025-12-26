using Amazon.Runtime.Internal.Util;
using Inspira.Application.Services;
using Inspira.Domain.Entities;
using Inspira.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public async Task<SsnCheckServiceResult> SsnCheckAsync(int? submissionId, string ssn, string role)
    {
        try
        {
            Submission? submission = await _submissionRepository.GetByIdAsync(submissionId.Value);

            if (submission == null)
            {
                throw new ArgumentException($"Submission with ID {submissionId} not found.");
            }

            // Basic redaction check: consider redacted if length==4 or contains '•'
            var redactedTaxId = ssn.Length == 4 || ssn.Contains("•");

            SubmissionProperty? submissionProperty = null;

            submissionProperty = await _submissionPropertyRepository.GetByIdAsync(submission.SubmissionPropertyId);

            if (submissionProperty == null)
            {
                throw new ArgumentException($"SubmissionProperty with ID {submission.SubmissionPropertyId} not found.");
            }

            if (redactedTaxId)
            {
                ssn = submissionProperty.OwnerTaxId;
            }

            var roleInt = role == "Owner" ? 1 : 0;

            // Call SOAP service
            var contactIdVal = await _mtcSoapClient.SsnInternalCheckAsync(ssn, roleInt);

            if (role == "Owner")
            {
                submissionProperty.OwnerContactId = contactIdVal.ToString();
                await _submissionPropertyRepository.UpdateAsync(submissionProperty);
            }

            return BuildResult(contactIdVal, string.Empty);
        }
        catch (Exception ex)
        {
            var errorMsg = $"{nameof(SsnCheckService)} failed to run correctly. ";
            var multipleActiveMessage = "Multiple active contact master records found";

            if (ex.Message != null && ex.Message.Contains(multipleActiveMessage, StringComparison.OrdinalIgnoreCase))
            {
                errorMsg += multipleActiveMessage;
            }

            errorMsg += "\n" + ex.ToString();

            _logger.LogWarning(errorMsg);

            return BuildResult(0, "error");
        }
    }

    private SsnCheckServiceResult BuildResult(int contactIdVal, string error)
    {
        string resultValue = !string.IsNullOrEmpty(error) ? error : (contactIdVal != 0).ToString().ToLower();
        return new SsnCheckServiceResult
        {
            Result = resultValue,
            ContactId = contactIdVal
        };
    }
}
