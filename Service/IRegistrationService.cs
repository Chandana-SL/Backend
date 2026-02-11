using TimeTrack.API.DTOs.Registration;
using TaskAsync = System.Threading.Tasks.Task;

namespace TimeTrack.API.Service;

public interface IRegistrationService
{
    System.Threading.Tasks.Task<RegistrationResponseDto> SubmitRegistrationAsync(RegistrationRequestDto request);
    System.Threading.Tasks.Task<IEnumerable<PendingRegistrationDto>> GetAllRegistrationsAsync();
    System.Threading.Tasks.Task<IEnumerable<PendingRegistrationDto>> GetPendingRegistrationsAsync();
    System.Threading.Tasks.Task<IEnumerable<PendingRegistrationDto>> GetApprovedRegistrationsAsync();
    System.Threading.Tasks.Task<IEnumerable<PendingRegistrationDto>> GetRejectedRegistrationsAsync();
    System.Threading.Tasks.Task<RegistrationResponseDto> ApproveRegistrationAsync(int registrationId, int adminUserId);
    System.Threading.Tasks.Task<RegistrationResponseDto> RejectRegistrationAsync(int registrationId, int adminUserId, string? reason);
    TaskAsync DeleteRegistrationAsync(int registrationId);
    System.Threading.Tasks.Task<int> GetPendingCountAsync();
}